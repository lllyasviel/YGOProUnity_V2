
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MegaUVAdjust))]
public class MegaUVAdjustEditor : MegaModifierEditor
{
	public override bool Inspector()
	{
		MegaUVAdjust mod = (MegaUVAdjust)target;

		EditorGUIUtility.LookLikeControls();
		mod.animate = EditorGUILayout.Toggle("Animate", mod.animate);
		mod.rotspeed = EditorGUILayout.FloatField("Rot Speed", mod.rotspeed);
		mod.spiralspeed = EditorGUILayout.FloatField("Spiral Speed", mod.spiralspeed);
		mod.speed = EditorGUILayout.Vector3Field("Speed", mod.speed);
		mod.spiral = EditorGUILayout.FloatField("Spiral", mod.spiral);
		mod.spirallim = EditorGUILayout.FloatField("Spiral Lim", mod.spirallim);
		return false;
	}

	public override void DrawSceneGUI()
	{
		MegaModifier mod = (MegaModifier)target;

		if ( mod.ModEnabled && mod.DisplayGizmo && MegaModifiers.GlobalDisplay )
		{
			MegaModifiers context = mod.GetComponent<MegaModifiers>();

			if ( context != null && context.Enabled && context.DrawGizmos )
			{
				float a = mod.gizCol1.a;
				Color col = Color.white;

				Quaternion rot = mod.transform.localRotation;

				Handles.matrix = Matrix4x4.identity;

				if ( mod.Offset != Vector3.zero )
				{
					Vector3 pos = mod.transform.localToWorldMatrix.MultiplyPoint(Vector3.Scale(-mod.gizmoPos - mod.Offset, mod.bbox.Size()));
					Handles.Label(pos, mod.ModName() + " Pivot\n" + mod.Offset.ToString("0.000"));
					col = Color.blue;
					col.a = a;
					Handles.color = col;
					Handles.ArrowCap(0, pos, rot * Quaternion.Euler(180.0f, 0.0f, 0.0f), mod.GizmoSize());
					col = Color.green;
					col.a = a;
					Handles.color = col;
					Handles.ArrowCap(0, pos, rot * Quaternion.Euler(90.0f, 0.0f, 0.0f), mod.GizmoSize());
					col = Color.red;
					col.a = a;
					Handles.color = col;
					Handles.ArrowCap(0, pos, rot * Quaternion.Euler(0.0f, -90.0f, 0.0f), mod.GizmoSize());
				}

				Handles.matrix = Matrix4x4.identity;
			}
		}
	}
}