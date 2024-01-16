// ==================================================
// Copyright (c) All rights reserved.
// @Author: GodWinTsai
// @Maintainer: 
// @Date: 
// @Desc: 
// ==================================================

using UnityEngine;

namespace UnityEditor.Tilemaps
{
	/// <summary>
	/// The Brush Editor for a Prefab Brush.
	/// </summary>
	[CustomEditor(typeof(PrefabBrush))]
	public class PrefabBrushEditor : GridBrushEditor
	{
		const string eraseAnyObjectsTooltip =
			"If true, erases any GameObjects that are in a given position " +
			"within the selected layers with Erasing. " +
			"Otherwise, erases only GameObjects that are created " +
			"from owned Prefab in a given position within the selected layers with Erasing.";
		
		private SerializedProperty m_Anchor;

		/// <summary>
		/// SerializedObject representation of the target Prefab Brush
		/// </summary>
		protected SerializedObject m_SerializedObject;
		
		private PrefabBrush prefabBrush => target as PrefabBrush;
		private SerializedProperty m_Prefab;

		/// <summary>
		/// OnEnable for the PrefabBrushEditor
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			m_SerializedObject = new SerializedObject(target);
			m_Anchor = m_SerializedObject.FindProperty("m_Anchor");
			m_Prefab = m_SerializedObject.FindProperty(nameof(m_Prefab));
		}

		/// <summary>
		/// Callback for painting the inspector GUI for the PrefabBrush in the Tile Palette.
		/// The PrefabBrush Editor overrides this to have a custom inspector for this Brush.
		/// </summary>
		public override void OnPaintInspectorGUI()
		{
			m_SerializedObject.UpdateIfRequiredOrScript();
			EditorGUILayout.PropertyField(m_Anchor);
			m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();

			m_SerializedObject.UpdateIfRequiredOrScript();
			EditorGUILayout.PropertyField(m_Prefab, true);
			prefabBrush.m_EraseAnyObjects = EditorGUILayout.Toggle(
				new GUIContent("Erase Any Objects", eraseAnyObjectsTooltip),
				prefabBrush.m_EraseAnyObjects);

			m_SerializedObject.ApplyModifiedPropertiesWithoutUndo();
		}
		
		public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
		{
			base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);

			var labelText = "Pos: " + position.position;
			if (position.size.x > 1 || position.size.y > 1) {
				labelText += " Size: " + position.size;
			}

			Handles.Label(grid.CellToWorld(position.position), labelText);
		}
	}
}