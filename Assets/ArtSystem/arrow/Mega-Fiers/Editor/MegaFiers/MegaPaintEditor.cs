
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaPaint))]
public class MegaPaintEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Vertex Paint Modifier by Chris West"; }

	public override bool Inspector()
	{
		MegaPaint mod = (MegaPaint)target;

		EditorGUIUtility.LookLikeControls();

		mod.radius = EditorGUILayout.FloatField("Radius", mod.radius);
		mod.amount = EditorGUILayout.FloatField("Amount", mod.amount);
		mod.usedecay = EditorGUILayout.Toggle("Use Decay", mod.usedecay);

		if ( mod.usedecay )
			mod.decay = EditorGUILayout.FloatField("Decay", mod.decay);

		mod.fallOff = (MegaFallOff)EditorGUILayout.EnumPopup("Falloff Mode", mod.fallOff);
		mod.gaussc = EditorGUILayout.FloatField("Falloff", mod.gaussc);

		mod.useAvgNorm = EditorGUILayout.Toggle("Use Avg Norm", mod.useAvgNorm);

		if ( !mod.useAvgNorm )
			mod.normal = EditorGUILayout.Vector3Field("Normal", mod.normal);

		mod.mode = (MegaPaintMode)EditorGUILayout.EnumPopup("Paint Mode", mod.mode);

		return false;
	}
}
