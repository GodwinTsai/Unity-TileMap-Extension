// ==================================================
// Copyright (c) All rights reserved.
// @Author: GodWinTsai
// @Maintainer: 
// @Date: 
// @Desc: 
// ==================================================

using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class PrefabTileComponent : MonoBehaviour
{
	#region Mono Fields
	[Header("x:rows，y:cols")]
	public Vector2Int occupy = Vector2Int.one;
	
	public Vector3Int cellCoordinate;
	#endregion

	public void RefreshOccupyPos()
	{
		var tileMap = gameObject.GetComponentInParent<Tilemap>();
		if (tileMap == null)
		{
			return;
		}

		var worldPos = Vector3.zero;
		int index = 0;
		for (int i = 0; i < occupy.x; i++)
		{
			for (int j = 0; j < occupy.y; j++)
			{
				var cellPos = tileMap.GetCellCenterWorld(cellCoordinate + new Vector3Int(i, j, 0));
				worldPos += cellPos;
				index++;
			}
		}

		if (index > 0)
		{
			worldPos /= index;
		}

		transform.position = worldPos;
	}

	public void RefreshSortingOrder()
	{
		if (!Application.isPlaying)
		{
			var render = GetComponentInParent<TilemapRenderer>();
			if (render != null)
			{
				TileMapUtil.SetSortingOrderAndSortPoint(gameObject, render.sortingOrder, SpriteSortPoint.Pivot);
			}
		}
	}
}