
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaRadialSkew))]
public class MegaRadialSkewEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Radial Skew Modifier by Chris West"; }
	public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers/skew_help.png"); }

	public override bool Inspector()
	{
		MegaRadialSkew mod = (MegaRadialSkew)target;

		EditorGUIUtility.LookLikeControls();
		mod.angle = EditorGUILayout.FloatField("Angle", mod.angle);
		mod.axis = (MegaAxis)EditorGUILayout.EnumPopup("Axis", mod.axis);
		mod.eaxis = (MegaAxis)EditorGUILayout.EnumPopup("Effective Axis", mod.eaxis);
		mod.biaxial = EditorGUILayout.Toggle("Bi Axial", mod.biaxial);
		return false;
	}
}