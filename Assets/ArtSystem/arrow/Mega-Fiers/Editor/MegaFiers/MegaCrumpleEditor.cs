
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaCrumple))]
public class MegaCrumpleEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Crumple Modifier by Unity"; }

	public override bool Inspector()
	{
		MegaCrumple mod = (MegaCrumple)target;

		EditorGUIUtility.LookLikeControls();
		mod.scale = EditorGUILayout.FloatField("Scale", mod.scale);
		mod.speed = EditorGUILayout.FloatField("Speed", mod.speed);
		mod.phase = EditorGUILayout.FloatField("Phase", mod.phase);
		mod.animate = EditorGUILayout.Toggle("Animate", mod.animate);
		return false;
	}
}
