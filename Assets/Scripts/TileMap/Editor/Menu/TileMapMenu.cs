// ==================================================
// Copyright (c) All rights reserved.
// @Author: GodWinTsai
// @Maintainer: 
// @Date: 
// @Desc: 
// ==================================================

using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
	public class TileMapMenu
	{
		[MenuItem("Assets/Create/2D/Tiles/Prefab Tile", priority = 200)]
		static void CreatePrefabTile()
		{
			ProjectWindowUtil.CreateAsset(ScriptableObject.CreateInstance<PrefabTile>(), "New Prefab Tile.asset");
		}
	}
}