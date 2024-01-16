// ==================================================
// Copyright (c) All rights reserved.
// @Author: GodWinTsai
// @Maintainer: 
// @Date: 
// @Desc: 
// ==================================================

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Debug = UnityEngine.Debug;

namespace UnityEditor.Tilemaps
{
    /// <summary>
    /// This Brush instances and places a containing prefab onto the targeted location and parents the instanced object to the paint target.
    /// </summary>
    [CustomGridBrush(false, false, false, "Prefab Brush")]
    public class PrefabBrush : GridBrush
    {
        #pragma warning disable 0649
        /// <summary>
        /// Anchor Point of the Instantiated Prefab in the cell when painting
        /// </summary>
        public Vector3 m_Anchor = new Vector3(0.34f, 0.34f, 0f);
        
        /// <summary>
        /// The selection of Prefab to paint from
        /// </summary>
        [SerializeField] GameObject m_Prefab;
        Quaternion m_Rotation = default;
        
        #pragma warning restore 0649

        /// <summary>
        /// If true, erases any GameObjects that are in a given position within the selected layers with Erasing.
        /// Otherwise, erases only GameObjects that are created from owned Prefab in a given position within the selected layers with Erasing.
        /// </summary>
        public bool m_EraseAnyObjects = true;
        
        /// <summary>
        /// Gets all children of the parent Transform which are within the given Grid's cell position.
        /// </summary>
        /// <param name="grid">Grid to determine cell position.</param>
        /// <param name="parent">Parent transform to get child Objects from.</param>
        /// <param name="position">Cell position to get Objects from.</param>
        /// <returns>A list of GameObjects within the given Grid's cell position.</returns>
        private List<GameObject> GetObjectsInCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            var results = new List<GameObject>();
            var childCount = parent.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = parent.GetChild(i);
                var tileMapComponent = child.GetComponent<PrefabTileComponent>();
                if (tileMapComponent != null && tileMapComponent.cellCoordinate == position)
                {
                    results.Add(child.gameObject);
                }
            }

            return results;
        }

        /// <summary>
        /// Instantiates a Prefab into the given Grid's cell position parented to the brush target.
        /// </summary>
        /// <param name="grid">Grid to determine cell position.</param>
        /// <param name="brushTarget">Target to instantiate child to.</param>
        /// <param name="position">Cell position to instantiate to.</param>
        /// <param name="prefab">Prefab to instantiate.</param>
        /// <param name="rotation"></param>
        private void InstantiatePrefabInCell(GridLayout grid, GameObject brushTarget, Vector3Int position, GameObject prefab, Quaternion rotation = default)
        {
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (instance != null)
            {
                var prefabTileComponent = instance.GetComponent<PrefabTileComponent>();
                if (prefabTileComponent == null)
                {
                    Debug.LogError($"[TileMap]Not Found PrefabTileComponent");
                    return;
                }
                Undo.MoveGameObjectToScene(instance, brushTarget.scene, "Paint Prefabs");
                Undo.RegisterCreatedObjectUndo(instance, "Paint Prefabs");
                instance.transform.SetParent(brushTarget.transform);
                var anchor = GetAnchor(brushTarget);
                var localPos = grid.CellToLocalInterpolated(position + anchor);
                instance.transform.position = grid.LocalToWorld(localPos);
                instance.transform.rotation = rotation;
                instance.transform.name += position.ToString();
                
                prefabTileComponent.cellCoordinate = position;
                prefabTileComponent.RefreshOccupyPos();
                prefabTileComponent.RefreshSortingOrder();
            }
        }

        /// <summary>
        /// Rotates the brush in the given direction.
        /// </summary>
        /// <param name="direction">Direction to rotate by.</param>
        /// <param name="layout">Cell Layout for rotating.</param>
        public override void Rotate(RotationDirection direction, GridLayout.CellLayout layout)
        {
            var angle = layout == GridLayout.CellLayout.Hexagon ? 60f : 90f;
            m_Rotation = Quaternion.Euler(0f, 0f, direction == RotationDirection.Clockwise ? m_Rotation.eulerAngles.z + angle : m_Rotation.eulerAngles.z - angle);
        }

        /// <summary>
        /// Paints GameObject from containing Prefab into a given position within the selected layers.
        /// The PrefabBrush overrides this to provide Prefab painting functionality.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the paint operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to paint data to.</param>
        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget == null)
            {
                return;
            }
            // Do not allow editing palettes
            if (brushTarget == null || brushTarget.layer == 31)
            {
                return;
            }

            var objectsInCell = GetObjectsInCell(grid, brushTarget.transform, position);
            var existPrefabObjectInCell = objectsInCell.Any(objectInCell => PrefabUtility.GetCorrespondingObjectFromSource(objectInCell) == m_Prefab);

            if (!existPrefabObjectInCell)
            {
                InstantiatePrefabInCell(grid, brushTarget, position, m_Prefab, m_Rotation);
            }
        }

        /// <summary>
        /// Paints the PrefabBrush instance's prefab into all positions specified by the box fill tool.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the box fill operation. By default the currently selected GameObject.</param>
        /// <param name="bounds">The cooridnate boundries to fill.</param>
        public override void BoxFill(GridLayout grid, GameObject brushTarget, BoundsInt bounds)
        {
            foreach(Vector3Int tilePosition in bounds.allPositionsWithin)
                Paint(grid, brushTarget, tilePosition);
        }

        public override void BoxErase(GridLayout grid, GameObject brushTarget, BoundsInt bounds)
        {
            base.BoxErase(grid, brushTarget, bounds);
            foreach (Vector3Int tilePosition in bounds.allPositionsWithin)
                Erase(grid, brushTarget, tilePosition);
        }

        /// <summary>
        /// If "Erase Any Objects" is true, erases any GameObjects that are in a given position within the selected layers.
        /// If "Erase Any Objects" is false, erases only GameObjects that are created from owned Prefab in a given position within the selected layers.
        /// The PrefabBrush overrides this to provide Prefab erasing functionality.
        /// </summary>
        /// <param name="grid">Grid used for layout.</param>
        /// <param name="brushTarget">Target of the erase operation. By default the currently selected GameObject.</param>
        /// <param name="position">The coordinates of the cell to erase data from.</param>
        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            if (brushTarget == null)
            {
                return;
            }
            if (brushTarget == null || brushTarget.layer == 31 || brushTarget.transform == null)
            {
                return;
            }

            foreach (var objectInCell in GetObjectsInCell(grid, brushTarget.transform, position))
            {
                var prefabSource = PrefabUtility.GetCorrespondingObjectFromSource(objectInCell);
                if (m_EraseAnyObjects || prefabSource == m_Prefab)
                {
                    try
                    {
                        Undo.DestroyObjectImmediate(objectInCell);
                    }
                    catch (Exception e)
                    {
                        DestroyObjectInPrefabAsset(objectInCell);
                    }
                }
            }
        }

        private void DestroyObjectInPrefabAsset(GameObject go)
        {
            var goName = go.name;
            try
            {
                var hierarchyPath = go.GetHierarchyPath();
                var prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
                var prefabName = prefabRoot.name;
                var childPath = hierarchyPath.Replace($"{prefabName}/", "");
                var sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(prefabRoot);
                var prefabPath = AssetDatabase.GetAssetPath(sourcePrefab);
                var prefab = PrefabUtility.LoadPrefabContents(prefabPath);
                
                var newGo = prefab.FindChild(childPath)?.gameObject;
                if (newGo != null)
                {
                    DestroyImmediate(newGo);
                    PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath, out bool _);
                }
                
                PrefabUtility.UnloadPrefabContents(prefab);
            }
            catch (Exception e)
            {
                Debug.LogError($"[TileMap]Destroy Error， name:{goName}, e:{e.Message}");
            }
        }

        /// <summary>
        /// Pick prefab from selected Tilemap, given the coordinates of the cells.
        /// </summary>
        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
        {
            base.Pick(gridLayout, brushTarget, position, pickStart);
            if (brushTarget == null)
            {
                return;
            }
        
            var tilemap = brushTarget.GetComponent<Tilemap>();
            
            foreach (Vector3Int location in position.allPositionsWithin)
            {
                var prefabTile = tilemap.GetTile<PrefabTile>(location);
                if (prefabTile == null)
                {
                    continue;
                }
                m_Prefab = prefabTile.prefab;
            }
        }

        private Vector3 GetAnchor(GameObject brushTarget)
        {
            var tilemap = brushTarget.GetComponent<Tilemap>();
            return tilemap != null ? tilemap.tileAnchor : m_Anchor;
        }
    }
}
