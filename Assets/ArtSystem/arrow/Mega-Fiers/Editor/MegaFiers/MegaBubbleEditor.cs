
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaBubble))]
public class MegaBubbleEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Bubble Modifier by Chris West"; }
	public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers/bubble_help.png"); }

	public override bool Inspector()
	{
		MegaBubble mod = (MegaBubble)target;

		EditorGUIUtility.LookLikeControls();
		mod.radius = EditorGUILayout.FloatField("Radius", mod.radius);
		mod.falloff = EditorGUILayout.FloatField("Falloff", mod.falloff);
		return false;
	}
}