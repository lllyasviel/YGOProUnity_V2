
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaSinusCurve))]
public class MegaSinusCurveEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Sinus Curve Modifier by Unity"; }
	//public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers/bend_help.png"); }

	public override bool Inspector()
	{
		MegaSinusCurve mod = (MegaSinusCurve)target;

		EditorGUIUtility.LookLikeControls();
		mod.scale = EditorGUILayout.FloatField("Scale", mod.scale);
		mod.speed = EditorGUILayout.FloatField("Speed", mod.speed);
		mod.phase = EditorGUILayout.FloatField("Phase", mod.phase);
		mod.animate = EditorGUILayout.Toggle("Animate", mod.animate);
		return false;
	}
}
