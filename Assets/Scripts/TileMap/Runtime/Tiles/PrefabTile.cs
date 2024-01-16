// ==================================================
// Copyright (c) All rights reserved.
// @Author: GodWinTsai
// @Maintainer: 
// @Date: 
// @Desc: 
// ==================================================

using System;

namespace UnityEngine.Tilemaps
{
	[Serializable]
	public class PrefabTile : Tile
	{
		[SerializeField]
		public GameObject prefab;

	}
}