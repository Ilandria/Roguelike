﻿using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace CCB.Roguelike
{
	[CustomEditor(typeof(MeshRenderer))]
	public class MeshRendererSortingLayersEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			MeshRenderer renderer = target as MeshRenderer;

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();

			string name = EditorGUILayout.TextField("Sorting Layer Name", renderer.sortingLayerName);

			if (EditorGUI.EndChangeCheck())
			{
				renderer.sortingLayerName = name;
			}

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();

			int order = EditorGUILayout.IntField("Sorting Order", renderer.sortingOrder);

			if (EditorGUI.EndChangeCheck())
			{
				renderer.sortingOrder = order;
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginChangeCheck();

			LayerMask layerMask = EditorGUILayout.MaskField("Rendering Layer Mask", (int)renderer.renderingLayerMask, InternalEditorUtility.layers);

			if (EditorGUI.EndChangeCheck())
			{
				renderer.renderingLayerMask = (uint)layerMask.value;
			}

			EditorGUILayout.EndHorizontal();

			DrawDefaultInspector();
		}
	}
}