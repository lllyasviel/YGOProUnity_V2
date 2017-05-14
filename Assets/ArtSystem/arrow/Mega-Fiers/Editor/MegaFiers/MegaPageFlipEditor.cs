
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MegaPageFlip))]
public class MegaPageFlipEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Page Flip Modifier by Chris West"; }
	//public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers\\bend_help.png"); }

	bool advanced = false;

	public override bool Inspector()
	{
		MegaPageFlip mod = (MegaPageFlip)target;

		EditorGUIUtility.LookLikeControls();
		mod.turn = EditorGUILayout.FloatField("Turn", mod.turn);
		mod.ap1 = EditorGUILayout.FloatField("Ap1", mod.ap1);
		mod.ap2 = EditorGUILayout.FloatField("Ap2", mod.ap2);
		mod.ap3 = EditorGUILayout.FloatField("Ap3", mod.ap3);
		mod.flipx = EditorGUILayout.Toggle("Flip X", mod.flipx);

		advanced = EditorGUILayout.Foldout(advanced, "Advanced");
		if ( advanced )
		{
			mod.animT = EditorGUILayout.Toggle("Anim T", mod.animT);
			mod.autoMode = EditorGUILayout.Toggle("Auto Mode", mod.autoMode);
			mod.lockRho = EditorGUILayout.Toggle("Lock Rho", mod.lockRho);
			mod.lockTheta = EditorGUILayout.Toggle("Lock Theta", mod.lockTheta);
			mod.timeStep = EditorGUILayout.FloatField("TimeStep", mod.timeStep);
			mod.rho = EditorGUILayout.FloatField("Rho", mod.rho);
			mod.theta = EditorGUILayout.FloatField("Theta", mod.theta);
			mod.deltaT = EditorGUILayout.FloatField("DeltaT", mod.deltaT);
			mod.kT = EditorGUILayout.FloatField("kT", mod.kT);

		}
		return false;
	}
}
