
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaStretch))]
public class MegaStretchEditor : MegaModifierEditor
{
	public override string GetHelpString()	{ return "Stretch Modifier by Chris West"; }
	public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers/stretch_help.png"); }

	public override bool Inspector()
	{
		MegaStretch mod = (MegaStretch)target;

		EditorGUIUtility.LookLikeControls();
		mod.amount = EditorGUILayout.FloatField("Amount", mod.amount);
		mod.amplify = EditorGUILayout.FloatField("Amplify", mod.amplify);
		mod.axis = (MegaAxis)EditorGUILayout.EnumPopup("Axis", mod.axis);

		mod.doRegion = EditorGUILayout.Toggle("Do Region", mod.doRegion);
		mod.from = EditorGUILayout.FloatField("From", mod.from);
		mod.to = EditorGUILayout.FloatField("To", mod.to);
		return false;
	}
}