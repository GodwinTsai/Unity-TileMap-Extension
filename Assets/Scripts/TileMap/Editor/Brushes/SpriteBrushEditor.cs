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
	
	[CustomEditor(typeof(SpriteBrush))]
	public class SpriteBrushEditor : GridBrushEditor
	{
		
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