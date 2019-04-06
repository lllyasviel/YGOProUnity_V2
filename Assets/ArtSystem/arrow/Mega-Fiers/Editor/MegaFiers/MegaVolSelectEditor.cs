
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaVolSelect))]
public class MegaVolSelectEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Vol Select Modifier by Chris West"; }
	//public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers/bend_help.png"); }

	public override bool DisplayCommon() { return false; }

	public override bool Inspector()
	{
		MegaVolSelect mod = (MegaVolSelect)target;

		EditorGUIUtility.LookLikeControls();
		mod.ModEnabled = EditorGUILayout.Toggle("Enabled", mod.ModEnabled);
		mod.radius = EditorGUILayout.FloatField("Radius", mod.radius);
		mod.falloff = EditorGUILayout.FloatField("Falloff", mod.falloff);
		mod.origin = EditorGUILayout.Vector3Field("Origin", mod.origin);
		mod.useCurrentVerts = EditorGUILayout.Toggle("Use Stack Verts", mod.useCurrentVerts);

		mod.displayWeights = EditorGUILayout.Toggle("Show Weights", mod.displayWeights);
		mod.gizCol = EditorGUILayout.ColorField("Gizmo Col", mod.gizCol);
		mod.gizSize = EditorGUILayout.FloatField("Gizmo Size", mod.gizSize);

		return false;
	}

	// option to use base verts or current stack verts for distance calc
	// flag to display weights
	// size of weights and color of gizmo

	public override void DrawSceneGUI()
	{
		MegaVolSelect mod = (MegaVolSelect)target;

		MegaModifiers mc = mod.gameObject.GetComponent<MegaModifiers>();

		float[] sel = mod.GetSel();

		if ( mc != null && sel != null )
		{
			Color col = Color.black;

			Matrix4x4 tm = mod.gameObject.transform.localToWorldMatrix;
			Handles.matrix = Matrix4x4.identity;

			if ( mod.displayWeights )
			{
				for ( int i = 0; i < sel.Length; i++ )
				{
					float w = sel[i];
					if ( w > 0.5f )
						col = Color.Lerp(Color.green, Color.red, (w - 0.5f) * 2.0f);
					else
						col = Color.Lerp(Color.blue, Color.green, w * 2.0f);
					Handles.color = col;

					Vector3 p = tm.MultiplyPoint(mc.sverts[i]);

					if ( w > 0.001f )
						Handles.DotCap(i, p, Quaternion.identity, mod.gizSize);
				}
			}

			Handles.color = mod.gizCol;	//new Color(0.5f, 0.5f, 0.5f, 0.2f);

			//Handles.DrawWireDisc(0, tm.MultiplyPoint(mod.origin), Quaternion.identity, mod.radius);
			//Handles.DrawWireDisc(tm.MultiplyPoint(mod.origin), Vector3.up, mod.radius);
			//Handles.DrawWireDisc(tm.MultiplyPoint(mod.origin), Vector3.right, mod.radius);
			//Handles.DrawWireDisc(tm.MultiplyPoint(mod.origin), Vector3.forward, mod.radius);
			Handles.SphereCap(0, tm.MultiplyPoint(mod.origin), Quaternion.identity, mod.radius * 2.0f);

			Handles.matrix = tm;
			mod.origin = Handles.PositionHandle(mod.origin, Quaternion.identity);
			Handles.matrix = Matrix4x4.identity;
		}
	}
}