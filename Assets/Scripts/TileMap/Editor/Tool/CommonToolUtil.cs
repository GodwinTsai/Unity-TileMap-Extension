// ==================================================
// Copyright (c) All rights reserved.
// @Author: GodWinTsai
// @Maintainer: 
// @Date: 
// @Desc: 
// ==================================================

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public static class CommonToolUtil
{
	public static void CheckPrefabs(Action<GameObject> action, string content, bool save = true)
	{
		var prefabs = GetSelectPrefabs();
		var count = prefabs.Count;
		var index = 0;
		foreach (var prefab in prefabs)
		{
			var isCancel = EditorUtility.DisplayCancelableProgressBar(content,
				$"{content}...{++index}/{count}", (float) index / count);
			if (isCancel)
			{
				break;
			}
			action.Invoke(prefab);
			if (save)
			{
				SavePrefab(prefab);
			}
		}

		if (save)
		{
			SaveAssets();
		}
            
		EditorUtility.ClearProgressBar();
	}
        
	/// <summary>
	/// 
	/// </summary>
	/// <param name="action"></param>
	/// <param name="content"></param>
	/// <param name="save"></param>
	public static void ExecuteSelectedPrefabs(Func<GameObject, bool> action, string content, bool save = true)
	{
		var prefabs = GetSelectPrefabs();
		ExecutePrefabs(prefabs, action, content, save);
	}
        
	public static void ExecutePrefabs(List<GameObject> prefabs, Func<GameObject, bool> action, string content, bool save = true)
	{
		var count = prefabs.Count;
		var index = 0;
		foreach (var prefab in prefabs)
		{
			var isCancel = EditorUtility.DisplayCancelableProgressBar(content,
				$"{content}...{++index}/{count}", (float) index / count);
			if (isCancel)
			{
				break;
			}

			try
			{
				bool result = action.Invoke(prefab);
				if (save && result)
				{
					SavePrefab(prefab);
				}
			}
			catch (Exception e)
			{
				Debug.LogError($"ExecutePrefabs Error, prefab:{prefab}, action:{action.Method.Name}, msg:{e.Message}");
				break;
			}
		}

		if (save)
		{
			SaveAssets();
		}
            
		EditorUtility.ClearProgressBar();
	}
	
	public static List<GameObject> GetSelectPrefabs()
	{
		List<GameObject> list = new List<GameObject>();
		Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
		foreach (Object asset in selectedAssets)
		{
			string filePath = AssetDatabase.GetAssetPath(asset);
			if (!filePath.EndsWith(".prefab"))
			{
				continue;
			}
			var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
			if (prefab == null)
			{
				Debug.LogError($"Load Prefab Null:{filePath}");
				continue;
			}
			list.Add(prefab);
		}
		return list;
	}
	
	public static List<string> GetSelectAssetPaths<T>(string extension) where  T : Object
	{
		List<string> list = new List<string>();
		Object[] selectedAssets = Selection.GetFiltered(typeof(T), SelectionMode.DeepAssets);
		foreach (Object selectAsset in selectedAssets)
		{
			string filePath = AssetDatabase.GetAssetPath(selectAsset);
			if (!filePath.EndsWith(extension))
			{
				continue;
			}
			
			list.Add(filePath);
		}
		return list;
	}
	
	public static List<T> GetSelectAssets<T>(string extension) where  T : Object
	{
		List<T> list = new List<T>();
		Object[] selectedAssets = Selection.GetFiltered(typeof(T), SelectionMode.DeepAssets);
		foreach (Object selectAsset in selectedAssets)
		{
			string filePath = AssetDatabase.GetAssetPath(selectAsset);
			if (!filePath.EndsWith(extension))
			{
				continue;
			}
			var asset = AssetDatabase.LoadAssetAtPath<T>(filePath);
			if (asset == null)
			{
				Debug.LogError($"Load Asset Null:{filePath}");
				continue;
			}
			list.Add(asset);
		}
		return list;
	}
	
	private static void SavePrefab(GameObject prefab)
	{
		PrefabUtility.SavePrefabAsset(prefab);
	}

	private static void SaveAssets()
	{
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}
}