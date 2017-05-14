
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaNoise))]
public class MegaNoiseEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Noise Modifier by Chris West"; }
	public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers\\noise_help.png"); }

	public override bool Inspector()
	{
		MegaNoise mod = (MegaNoise)target;

		EditorGUIUtility.LookLikeControls();
		mod.Scale = EditorGUILayout.FloatField("Scale", mod.Scale);
		mod.Freq = EditorGUILayout.FloatField("Freq", mod.Freq);
		mod.Phase = EditorGUILayout.FloatField("Phase", mod.Phase);
		mod.Fractal = EditorGUILayout.Toggle("Fractal", mod.Fractal);
		if ( mod.Fractal )
		{
			mod.Iterations = EditorGUILayout.FloatField("Iterations", mod.Iterations);
			mod.Rough = EditorGUILayout.FloatField("Rough", mod.Rough);
		}
		mod.Strength = EditorGUILayout.Vector3Field("Strength", mod.Strength);
		mod.Animate = EditorGUILayout.Toggle("Animate", mod.Animate);

		return false;
	}
}