// ==================================================
// Copyright (c) All rights reserved.
// @Author: GodWinTsai
// @Maintainer: 
// @Date: 
// @Desc: 
// ==================================================

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
public class TileMapCamera : MonoBehaviour
{
	public Camera cam;
	public TransparencySortMode mode = TransparencySortMode.CustomAxis;
	public Vector3 axis;

	private void Awake()
	{
		cam = gameObject.GetComponent<Camera>();
		UpdateSort();
	}

	private void Update()
	{
		if (cam == null)
		{
			return;
		}
		mode = cam.transparencySortMode;
		axis = cam.transparencySortAxis;
	}

	public void SetTransparencySortDefault()
	{
		cam.transparencySortMode = TransparencySortMode.Default;
		cam.transparencySortAxis = Vector3.forward;
		UpdateSort();
	}
	
	public void SetTransparencySortCustom()
	{
		cam.transparencySortMode = TransparencySortMode.CustomAxis;
		cam.transparencySortAxis = new Vector3(0, 1f, -0.26f);
		UpdateSort();
	}

	private void UpdateSort()
	{
		mode = cam.transparencySortMode;
		axis = cam.transparencySortAxis;
		
#if UNITY_EDITOR
		foreach (SceneView sv in SceneView.sceneViews)
		{
			sv.camera.transparencySortMode = mode;
			sv.camera.transparencySortAxis = axis;
			EditorUtility.SetDirty(sv);
			sv.Repaint();
		}
#endif
	}
}