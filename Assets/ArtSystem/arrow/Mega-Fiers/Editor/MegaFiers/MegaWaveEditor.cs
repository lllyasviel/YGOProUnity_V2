
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaWave))]
public class MegaWaveEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Wave Modifier by Chris West"; }
	public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers\\wave_help.png"); }

	public override bool Inspector()
	{
		MegaWave mod = (MegaWave)target;

		EditorGUIUtility.LookLikeControls();
		mod.amp = EditorGUILayout.FloatField("Amp", mod.amp);
		mod.amp2 = EditorGUILayout.FloatField("Amp 2", mod.amp2);
		mod.wave = EditorGUILayout.FloatField("Wave", mod.wave);
		mod.phase = EditorGUILayout.FloatField("Phase", mod.phase);
		mod.Decay = EditorGUILayout.FloatField("Decay", mod.Decay);
		mod.dir = EditorGUILayout.FloatField("Dir", mod.dir);
		mod.animate = EditorGUILayout.Toggle("Animate", mod.animate);
		mod.Speed = EditorGUILayout.FloatField("Speed", mod.Speed);
		mod.divs = EditorGUILayout.IntField("Divs", mod.divs);
		mod.numSegs = EditorGUILayout.IntField("Num Segs", mod.numSegs);
		mod.numSides = EditorGUILayout.IntField("Num Sides", mod.numSides);
		return false;
	}
}