// ==================================================
// Copyright (c) All rights reserved.
// @Author: GodWinTsai
// @Maintainer: 
// @Date: 
// @Desc: 
// ==================================================

using System.Text;
using UnityEngine;

public static class TileMapUtil
{
	public static void SetSortingOrderAndSortPoint(GameObject go, int sortingOrder, SpriteSortPoint sortPoint)
	{
		if (go == null)
		{
			return;
		}
		var renders = go.GetComponentsInChildren<Renderer>(true);
		foreach (var render in renders)
		{
			render.sortingOrder = sortingOrder;
			if (render is SpriteRenderer spriteRenderer)
			{
				spriteRenderer.spriteSortPoint = sortPoint;
			}
		}
	}
	
	public static string GetHierarchyPath(this GameObject gameObject)
	{
		StringBuilder sb = new();
		sb.Append(gameObject.name);

		Transform parent = gameObject.transform.parent;
		while (parent != null)
		{
			sb.Insert(0, parent.name + "/");
			parent = parent.parent;
		}
		return sb.ToString();
	}
	
	public static Transform FindChild(this GameObject gameObject, string name)
	{
		return gameObject.transform.Find(name);
	}
}