
#if false
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaSelection))]
public class MegaSelectionEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Selection Modifier by Chris West"; }
	//public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers\\bend_help.png"); }

	public override bool DisplayCommon() { return false; }

	public override bool Inspector()
	{
		MegaSelection mod = (MegaSelection)target;

		EditorGUIUtility.LookLikeControls();
		mod.weight = EditorGUILayout.FloatField("Weight", mod.weight);
		return false;
	}

	public override void DrawSceneGUI()
	{
		MegaSelection mod = (MegaSelection)target;

		MegaModifiers mc = mod.gameObject.GetComponent<MegaModifiers>();

		float[] sel = mod.GetSel();

		if ( mc != null && sel != null )
		{
			Color col = Color.black;

			Matrix4x4 tm = mod.gameObject.transform.localToWorldMatrix;
			Handles.matrix = Matrix4x4.identity;

			for ( int i = 0; i < sel.Length; i++ )
			{
				float w = sel[i];
				if ( w > 0.5f )
					col = Color.Lerp(Color.green, Color.red, (w - 0.5f) * 2.0f);
				else
					col = Color.Lerp(Color.blue, Color.green, w * 2.0f);
				Handles.color = col;

				Vector3 p = tm.MultiplyPoint(mc.sverts[i]);
				Handles.DotCap(i, p, Quaternion.identity, 0.01f);
			}

			Handles.matrix = Matrix4x4.identity;
		}
	}
}
#endif
