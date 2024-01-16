// ==================================================
// Copyright (c) All rights reserved.
// @Author: GodWinTsai
// @Maintainer: 
// @Date: 
// @Desc: 
// ==================================================
#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

public static class PrefabTilePreviewUtil
{
	private const float ORTHOGRAPHIC_SIZE = 1.5f;
	private const int PREFAB_WIDTH = 310;
	private const int PREFAB_HEIGHT = 210;
	
	private static PreviewRenderUtility _previewRenderUtility;
	
	public static Sprite GetPreviewSprite(GameObject obj, int width = PREFAB_WIDTH, int height = PREFAB_HEIGHT)
	{
		Init();
		var texture2D = CreatePreviewTexture2D(obj, width, height);
		var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100);
		return sprite;
	}
	
	public static Sprite GetPreviewSpriteFromTexture(GameObject obj, int width = PREFAB_WIDTH, int height = PREFAB_HEIGHT)
	{
		Init();
		var texture = CreatePreviewTexture(obj, width, height);
		var texture2D = Texture2D.CreateExternalTexture(
			texture.width,
			texture.height,
			TextureFormat.RGBA32,
			false, false,
			texture.GetNativeTexturePtr());
		
		var sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100);
		return sprite;
	}
	
	public static Texture2D GetPreviewTexture2D(GameObject obj, int width = PREFAB_WIDTH, int height = PREFAB_HEIGHT)
	{
		Init();
		var texture2D = CreatePreviewTexture2D(obj, width, height);
		return texture2D;
	}
	
	public static Texture GetPreviewTexture(GameObject obj, int width = PREFAB_WIDTH, int height = PREFAB_HEIGHT)
	{
		Init();
		var texture = CreatePreviewTexture(obj, width, height);
		return texture;
	}

	private static void Init()
	{
		if (_previewRenderUtility != null)
		{
			return;
		}

		_previewRenderUtility = new PreviewRenderUtility(true);
		
		System.GC.SuppressFinalize(_previewRenderUtility);

		var camera = _previewRenderUtility.camera;
		camera.orthographic = true;
		camera.orthographicSize = ORTHOGRAPHIC_SIZE;
		camera.nearClipPlane = 0.3f;
		camera.farClipPlane = 1000;
		camera.backgroundColor = Color.clear;
		camera.clearFlags = CameraClearFlags.Depth;
		camera.transform.position = new Vector3(0, 0, -10);
	}

	public static void Reset()
	{
		if (_previewRenderUtility != null)
		{
			_previewRenderUtility.Cleanup();
			_previewRenderUtility = null;
		}
	}
	
	private static Texture CreatePreviewTexture(GameObject obj, int width, int height)
	{
		Color color = Color.clear;
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture2D.SetPixel(0, 0, color);
		texture2D.Apply();
		
		GUIStyle previewBackground = GUIStyle.none;
		previewBackground.normal.background = texture2D;
		
		_previewRenderUtility.BeginPreview(new Rect(0, 0, width, height), previewBackground);

		_previewRenderUtility.lights[0].transform.localEulerAngles = new Vector3(30, 30, 0);
		_previewRenderUtility.lights[0].intensity = 2;
		_previewRenderUtility.AddSingleGO(obj);
		_previewRenderUtility.camera.Render();

		var tex =  _previewRenderUtility.EndPreview();
		Object.DestroyImmediate(obj);
		return tex;
	}
	
	private static Texture2D CreatePreviewTexture2D(GameObject obj, int width, int height)
	{
		Color color = Color.clear;
		Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
		texture2D.SetPixel(0, 0, color);
		texture2D.Apply();
		
		GUIStyle previewBackground = GUIStyle.none;
		previewBackground.normal.background = texture2D;
		
		_previewRenderUtility.BeginStaticPreview(new Rect(0, 0, width, height));

		_previewRenderUtility.lights[0].transform.localEulerAngles = new Vector3(30, 30, 0);
		_previewRenderUtility.lights[0].intensity = 2;
		_previewRenderUtility.AddSingleGO(obj);
		_previewRenderUtility.camera.Render();

		var tex2D = _previewRenderUtility.EndStaticPreview();
		Object.DestroyImmediate(obj);
		return tex2D;
	}

	public static Sprite SaveAndLoadSprite(Sprite sprite, string assetPath)
	{
		var dirPath = Path.GetDirectoryName(assetPath);
		if (!string.IsNullOrEmpty(dirPath) && !Directory.Exists(dirPath))
		{
			Directory.CreateDirectory(dirPath);
		}
		var fullPath = Path.Combine(Application.dataPath, "..", assetPath);
		
		File.WriteAllBytes(fullPath, sprite.texture.EncodeToPNG());
		AssetDatabase.Refresh();

		TextureImporter ti = AssetImporter.GetAtPath (assetPath) as TextureImporter;
 
		ti.textureType = TextureImporterType.Sprite;
		ti.spritePixelsPerUnit = sprite.pixelsPerUnit;
		ti.mipmapEnabled = false;
		EditorUtility.SetDirty (ti);
		ti.SaveAndReimport();

		var loadSprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
		return loadSprite;
	}
}
#endif
