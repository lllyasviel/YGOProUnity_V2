
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaSqueeze))]
public class MegaSqueezeEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Squeeze Modifier by Chris West"; }
	//public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers/taper_help.png"); }

	public override bool Inspector()
	{
		MegaSqueeze mod = (MegaSqueeze)target;

		EditorGUIUtility.LookLikeControls();
		mod.amount = EditorGUILayout.FloatField("Amount", mod.amount);
		mod.crv = EditorGUILayout.FloatField("Crv", mod.crv);
		mod.radialamount = EditorGUILayout.FloatField("Radial Amount", mod.radialamount);
		mod.radialcrv = EditorGUILayout.FloatField("Radial Crv", mod.radialcrv);
		mod.doRegion = EditorGUILayout.Toggle("Do Region", mod.doRegion);
		mod.from = EditorGUILayout.FloatField("From", mod.from);
		mod.to = EditorGUILayout.FloatField("To", mod.to);
		return false;
	}
}