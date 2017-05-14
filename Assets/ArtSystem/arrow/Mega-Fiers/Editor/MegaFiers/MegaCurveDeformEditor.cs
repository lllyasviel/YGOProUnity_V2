
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaCurveDeform))]
public class MegaCurveDeformEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Mega Curve Deform Modifier by Chris West"; }

	public override bool Inspector()
	{
		MegaCurveDeform mod = (MegaCurveDeform)target;

		EditorGUIUtility.LookLikeControls();

		mod.axis = (MegaAxis)EditorGUILayout.EnumPopup("Axis", mod.axis);
		mod.defCurve = EditorGUILayout.CurveField("Curve", mod.defCurve);
		mod.MaxDeviation = EditorGUILayout.FloatField("Max Deviation", mod.MaxDeviation);

		return false;
	}
}
