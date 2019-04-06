
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaTaper))]
public class MegaTaperEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Taper Modifier by Chris West"; }
	public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers/taper_help.png"); }

	public override bool Inspector()
	{
		MegaTaper mod = (MegaTaper)target;

		EditorGUIUtility.LookLikeControls();
		mod.amount = EditorGUILayout.FloatField("Amount", mod.amount);
		mod.crv = EditorGUILayout.FloatField("Crv", mod.crv);
		mod.dir = EditorGUILayout.FloatField("Dir", mod.dir);
		mod.axis = (MegaAxis)EditorGUILayout.EnumPopup("Axis", mod.axis);
		mod.EAxis = (MegaEffectAxis)EditorGUILayout.EnumPopup("EAxis", mod.EAxis);
		mod.sym = EditorGUILayout.Toggle("Sym", mod.sym);
		mod.doRegion = EditorGUILayout.Toggle("Do Region", mod.doRegion);
		mod.from = EditorGUILayout.FloatField("From", mod.from);
		mod.to = EditorGUILayout.FloatField("To", mod.to);
		return false;
	}
}