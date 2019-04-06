
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaBend))]
public class MegaBendEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Bend Modifier by Chris West"; }
	public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers/bend_help.png"); }

	public override bool Inspector()
	{
		MegaBend mod = (MegaBend)target;

		EditorGUIUtility.LookLikeControls();
		mod.angle = EditorGUILayout.FloatField("Angle", mod.angle);
		mod.dir = EditorGUILayout.FloatField("Dir", mod.dir);
		mod.axis = (MegaAxis)EditorGUILayout.EnumPopup("Axis", mod.axis);
		mod.doRegion = EditorGUILayout.Toggle("Do Region", mod.doRegion);
		mod.from = EditorGUILayout.FloatField("From", mod.from);
		mod.to = EditorGUILayout.FloatField("To", mod.to);
		return false;
	}
}
