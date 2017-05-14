
using UnityEngine;

[AddComponentMenu("Modifiers/Cylindrify")]
public class MegaCylindrify : MegaModifier
{
	public float Percent = 0.0f;
	public float Decay = 0.0f;

	public override string ModName() { return "Cylindrify"; }
	public override string GetHelpURL() { return "?page_id=166"; }

	float size;
	float per;

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		float dcy = Mathf.Exp(-Decay * p.magnitude);

		float k = ((size / Mathf.Sqrt(p.x * p.x + p.z * p.z) / 2.0f - 1.0f) * per * dcy) + 1.0f;
		p.x *= k;
		p.z *= k;
		return invtm.MultiplyPoint3x4(p);
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public void SetTM1()
	{
		tm = Matrix4x4.identity;
		//Quaternion rot = Quaternion.Euler(-gizmoRot);

		MegaMatrix.RotateZ(ref tm, -gizmoRot.z * Mathf.Deg2Rad);
		MegaMatrix.RotateY(ref tm, -gizmoRot.y * Mathf.Deg2Rad);
		MegaMatrix.RotateX(ref tm, -gizmoRot.x * Mathf.Deg2Rad);

		MegaMatrix.SetTrans(ref tm, gizmoPos + Offset);

		//tm.SetTRS(gizmoPos + Offset, rot, gizmoScale);
		invtm = tm.inverse;
	}

	public override void DrawGizmo(MegaModContext context)
	{
		if ( !Prepare(context) )
			return;

		Vector3 min = context.bbox.min;
		Vector3 max = context.bbox.max;

		Matrix4x4 gtm = Matrix4x4.identity;
		Vector3 pos = gizmoPos;
		pos.x = -pos.x;
		pos.y = -pos.y;
		pos.z = -pos.z;

		Vector3 scl = gizmoScale;
		scl.x = 1.0f - (scl.x - 1.0f);
		scl.y = 1.0f - (scl.y - 1.0f);
		gtm.SetTRS(pos, Quaternion.Euler(gizmoRot), scl);

		if ( context.mod.sourceObj != null )
			Gizmos.matrix = context.mod.sourceObj.transform.localToWorldMatrix * gtm;
		else
			Gizmos.matrix = transform.localToWorldMatrix * gtm;

		Gizmos.color = Color.yellow;
		corners[0] = new Vector3(min.x, min.y, min.z);
		corners[1] = new Vector3(min.x, max.y, min.z);
		corners[2] = new Vector3(max.x, max.y, min.z);
		corners[3] = new Vector3(max.x, min.y, min.z);

		corners[4] = new Vector3(min.x, min.y, max.z);
		corners[5] = new Vector3(min.x, max.y, max.z);
		corners[6] = new Vector3(max.x, max.y, max.z);
		corners[7] = new Vector3(max.x, min.y, max.z);

		DrawEdge(corners[0], corners[1]);
		DrawEdge(corners[1], corners[2]);
		DrawEdge(corners[2], corners[3]);
		DrawEdge(corners[3], corners[0]);

		DrawEdge(corners[4], corners[5]);
		DrawEdge(corners[5], corners[6]);
		DrawEdge(corners[6], corners[7]);
		DrawEdge(corners[7], corners[4]);

		DrawEdge(corners[0], corners[4]);
		DrawEdge(corners[1], corners[5]);
		DrawEdge(corners[2], corners[6]);
		DrawEdge(corners[3], corners[7]);

		ExtraGizmo(context);
	}

	public override bool Prepare(MegaModContext mc)
	{
		SetTM1();

		float xsize = bbox.max.x - bbox.min.x;
		float zsize = bbox.max.z - bbox.min.z;
		size = (xsize > zsize) ? xsize : zsize;

		// Get the percentage to spherify at this time
		per = Percent / 100.0f;

		return true;
	}
}
