// ==================================================
// Copyright (c) All rights reserved.
// @Author: GodWinTsai
// @Maintainer: 
// @Date: 
// @Desc: 
// ==================================================

using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

public static class PrefabTileTool
{
	[MenuItem("Assets/TileMapTool/Generate PrefabTile")]
	public static void GeneratePrefabTiles()
	{
		var prefabs = CommonToolUtil.GetSelectPrefabs();
		CommonToolUtil.ExecutePrefabs(prefabs, GeneratePrefabTile, "GeneratePrefabTiles", false);
		PrefabTilePreviewUtil.Reset();
	}
	
	[MenuItem("Assets/TileMapTool/ClearProgressBar")]
	public static void ClearProgressBar()
	{
		EditorUtility.ClearProgressBar();
	}
	
	private static bool GeneratePrefabTile(GameObject prefab)
	{
		if (prefab == null)
		{
			return false;
		}
		var instance = Object.Instantiate(prefab);
		var sprite = GeneratePrefabSprite(prefab, instance);
		GeneratePrefabTile(prefab, sprite);
		return false;
	}

	private static Sprite GeneratePrefabSprite(GameObject prefab, GameObject prefabIns)
	{
		var sprite = PrefabTilePreviewUtil.GetPreviewSprite(prefabIns);
		var spritePath = GetSaveSpritePath(prefab);
		sprite = PrefabTilePreviewUtil.SaveAndLoadSprite(sprite, spritePath);
		Debug.Log($"[TileMap]GeneratePrefabSprite,prefab:{prefab.name}, sprite:{sprite.name}");
		return sprite;
	}
	
	private static void GeneratePrefabTile(GameObject prefab, Sprite sprite)
	{
		var tilePath = GetSaveTilePath(prefab);
		var dirPath = Path.GetDirectoryName(tilePath);
		if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
		{
			Directory.CreateDirectory(dirPath);
		}

		var prefabTile = ScriptableObject.CreateInstance<PrefabTile>();
		prefabTile.colliderType = Tile.ColliderType.None;
		prefabTile.name = sprite.name;
		prefabTile.prefab = prefab;
		prefabTile.sprite = sprite;
		AssetDatabase.CreateAsset(prefabTile, tilePath);
		AssetDatabase.Refresh();
		AssetDatabase.SaveAssets();
		
		Debug.Log($"[TileMap]GeneratePrefabTile,prefab:{prefab.name}, tile:{prefabTile.name}");
	}

	private static string GetSaveSpritePath(GameObject prefab)
	{
		GetPrefabDirInfo(prefab, out string prefabPath, out string dirName);
		var spritePath = prefabPath.Replace($"Prefab/{dirName}", $"Tile/{dirName}Prefab/Preview").Replace(".prefab", ".png");
		return spritePath;
	}
	
	private static string GetSaveTilePath(GameObject prefab)
	{
		GetPrefabDirInfo(prefab, out string prefabPath, out string dirName);
		var spritePath = prefabPath.Replace($"Prefab/{dirName}", $"Tile/{dirName}Prefab").Replace(".prefab", ".asset");
		return spritePath;
	}

	private static void GetPrefabDirInfo(GameObject prefab, out string prefabPath, out string dirName)
	{
		prefabPath = AssetDatabase.GetAssetPath(prefab);
		var dirPath = Path.GetDirectoryName(prefabPath);
		dirName = Path.GetFileName(dirPath);
	}
}