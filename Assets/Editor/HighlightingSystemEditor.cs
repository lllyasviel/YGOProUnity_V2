using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(HighlightingEffect))]
public class HighlightingSystemEditor : Editor
{
	private string[] downsampleOptions = new string[]{"None", "Half", "Quarter"};
	
	private HighlightingEffect he;
	
	void OnEnable()
	{
		he = (HighlightingEffect) target as HighlightingEffect;
	}
	
	public override void OnInspectorGUI()
	{
		#if UNITY_IPHONE
		if (Handheld.use32BitDisplayBuffer == false)
		{
			EditorGUILayout.HelpBox("Highlighting System requires 32-bit display buffer. Set the 'Use 32-bit Display Buffer' checkbox under the 'Resolution and Presentation' section of Player Settings.", MessageType.Error);
		}
		#endif
		
		EditorGUILayout.Space();
		
		he.stencilZBufferEnabled = EditorGUILayout.Toggle("Use Z-Buffer", he.stencilZBufferEnabled);
		EditorGUILayout.HelpBox("Always enable 'Use Z-Buffer' if you wish to use highlighting occluders in your project. Otherwise - keep it unchecked (in terms of performance optimization).", MessageType.Info);
		
		EditorGUILayout.LabelField("Preset:");
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Default"))
		{
			he.downsampleFactor = 2;
			he.iterations = 2;
			he.blurMinSpread = 0.65f;
			he.blurSpread = 0.25f;
			he.blurIntensity = 0.3f;
		}
		if (GUILayout.Button("Strong"))
		{
			he.downsampleFactor = 2;
			he.iterations = 2;
			he.blurMinSpread = 0.5f;
			he.blurSpread = 0.15f;
			he.blurIntensity = 0.325f;
		}
		if (GUILayout.Button("Speed"))
		{
			he.downsampleFactor = 2;
			he.iterations = 1;
			he.blurMinSpread = 0.75f;
			he.blurSpread = 0.0f;
			he.blurIntensity = 0.35f;
		}
		if (GUILayout.Button("Quality"))
		{
			he.downsampleFactor = 1;
			he.iterations = 3;
			he.blurMinSpread = 1.0f;
			he.blurSpread = 0.0f;
			he.blurIntensity = 0.28f;
		}
		EditorGUILayout.EndHorizontal();
		
		EditorGUILayout.Space();
		
		he.downsampleFactor = EditorGUILayout.Popup("Downsampling:", he.downsampleFactor, downsampleOptions);
		he.iterations = Mathf.Clamp(EditorGUILayout.IntField("Iterations:", he.iterations), 0, 50);
		he.blurMinSpread = EditorGUILayout.Slider("Min Spread:", he.blurMinSpread, 0f, 3f);
		he.blurSpread = EditorGUILayout.Slider("Spread:", he.blurSpread, 0f, 3f);
		he.blurIntensity = EditorGUILayout.Slider("Intensity:", he.blurIntensity, 0f, 1f);
	}
}
