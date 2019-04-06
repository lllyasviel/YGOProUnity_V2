
using UnityEngine;
using UnityEditor;

public class MegaFFDEditor : MegaModifierEditor
{
	Vector3 pm = new Vector3();

	public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers/ffd_help.png"); }

	bool showpoints = true;

	public override bool Inspector()
	{
		MegaFFD mod = (MegaFFD)target;

		EditorGUIUtility.LookLikeControls();
		mod.KnotSize = EditorGUILayout.FloatField("Knot Size", mod.KnotSize);
		mod.inVol = EditorGUILayout.Toggle("In Vol", mod.inVol);

		showpoints = EditorGUILayout.Foldout(showpoints, "Points");

		if ( showpoints )
		{
			int gs = mod.GridSize();
			//int num = gs * gs * gs;

			for ( int x = 0; x < gs; x++ )
			{
				for ( int y = 0; y < gs; y++ )
				{
					for ( int z = 0; z < gs; z++ )
					{
						int i = (x * gs * gs) + (y * gs) + z;
						mod.pt[i] = EditorGUILayout.Vector3Field("p[" + x + "," + y + "," + z + "]", mod.pt[i]);
					}
				}
			}
		}
		return false;
	}

	public override void DrawSceneGUI()
	{
		MegaFFD ffd = (MegaFFD)target;

		if ( ffd.DisplayGizmo )
		{
			MegaModifiers context = ffd.GetComponent<MegaModifiers>();

			Vector3 size = ffd.lsize;
			Vector3 osize = ffd.lsize;
			osize.x = 1.0f / size.x;
			osize.y = 1.0f / size.y;
			osize.z = 1.0f / size.z;

			Matrix4x4 tm1 = Matrix4x4.identity;
			Quaternion rot = Quaternion.Euler(ffd.gizmoRot);
			tm1.SetTRS(-(ffd.gizmoPos + ffd.Offset), rot, Vector3.one);

			if ( context != null && context.sourceObj != null )
				Handles.matrix = context.sourceObj.transform.localToWorldMatrix * tm1;
			else
				Handles.matrix = ffd.transform.localToWorldMatrix * tm1;

			DrawGizmos(ffd, Handles.matrix);

			Handles.color = Color.yellow;

			int pc = ffd.GridSize();
			pc = pc * pc * pc;
			for ( int i = 0; i < pc; i++ )
			{
				Vector3 p = ffd.GetPoint(i) + ffd.bcenter;

				//pm = Handles.PositionHandle(p, Quaternion.identity);
				pm = Handles.FreeMoveHandle(p, Quaternion.identity, ffd.KnotSize * 0.1f, Vector3.zero, Handles.CircleCap);

				pm -= ffd.bcenter;
				p = Vector3.Scale(pm, osize);
				p.x += 0.5f;
				p.y += 0.5f;
				p.z += 0.5f;

				ffd.pt[i] = p;
			}

			Handles.matrix = Matrix4x4.identity;
		}
	}

	Vector3[] pp3 = new Vector3[3];

	public void DrawGizmos(MegaFFD ffd, Matrix4x4 tm)
	{
		Handles.color = Color.red;

		int pc = ffd.GridSize();

		for ( int  i = 0; i < pc; i++ )
		{
			for ( int j = 0; j < pc; j++ )
			{
				for ( int k = 0; k < pc; k++ )
				{
					pp3[0] = tm.MultiplyPoint(ffd.GetPoint(i, j, k) + ffd.bcenter);

					if ( i < pc - 1 )
					{
						pp3[1] = tm.MultiplyPoint(ffd.GetPoint(i + 1, j, k) + ffd.bcenter);
						Handles.DrawLine(pp3[0], pp3[1]);
					}

					if ( j < pc - 1 )
					{
						pp3[1] = tm.MultiplyPoint(ffd.GetPoint(i, j + 1, k) + ffd.bcenter);
						Handles.DrawLine(pp3[0], pp3[1]);
					}

					if ( k < pc - 1 )
					{
						pp3[1] = tm.MultiplyPoint(ffd.GetPoint(i, j, k + 1) + ffd.bcenter);
						Handles.DrawLine(pp3[0], pp3[1]);
					}
				}
			}
		}
	}
}