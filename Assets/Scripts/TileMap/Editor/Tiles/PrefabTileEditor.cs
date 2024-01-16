// ==================================================
// Copyright (c) All rights reserved.
// @Author: GodWinTsai
// @Maintainer: 
// @Date: 
// @Desc: 
// ==================================================

using UnityEditor;

namespace UnityEngine.Tilemaps
{
	[CustomEditor(typeof(PrefabTile))]
	public class PrefabTileEditor : Editor
	{
		private SerializedProperty _color;
		private SerializedProperty _colliderType;
		private SerializedProperty _sprite;
		private SerializedProperty _prefab;

		private PrefabTile _prefabTile;

		public void OnEnable()
		{
			_prefabTile = target as PrefabTile;
			_color = serializedObject.FindProperty("m_Color");
			_colliderType = serializedObject.FindProperty("m_ColliderType");
			_sprite = serializedObject.FindProperty("m_Sprite");
			_prefab = serializedObject.FindProperty("Prefab");
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUI.BeginChangeCheck();

			EditorGUILayout.Space();
			
			EditorGUILayout.PropertyField(_color);
			EditorGUILayout.PropertyField(_colliderType);
			EditorGUILayout.PropertyField(_sprite);
			EditorGUILayout.PropertyField(_prefab);

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(_prefabTile);
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}