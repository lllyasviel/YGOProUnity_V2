
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaRipple))]
public class MegaRippleEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Ripple Modifier by Chris West"; }
	public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("ripple_help.png"); }

	public override bool Inspector()
	{
		MegaRipple mod = (MegaRipple)target;

		EditorGUIUtility.LookLikeControls();
		mod.amp = EditorGUILayout.FloatField("Amp", mod.amp);
		mod.amp2 = EditorGUILayout.FloatField("Amp 2", mod.amp2);
		mod.flex = EditorGUILayout.FloatField("Flex", mod.flex);
		mod.wave = EditorGUILayout.FloatField("Wave", mod.wave);
		mod.phase = EditorGUILayout.FloatField("Phase", mod.phase);
		mod.Decay = EditorGUILayout.FloatField("Decay", mod.Decay);
		mod.animate = EditorGUILayout.Toggle("Animate", mod.animate);
		mod.Speed = EditorGUILayout.FloatField("Speed", mod.Speed);
		mod.radius = EditorGUILayout.FloatField("Radius", mod.radius);
		mod.segments = EditorGUILayout.IntField("Segments", mod.segments);
		mod.circles = EditorGUILayout.IntField("Circles", mod.circles);

		return false;
	}
}