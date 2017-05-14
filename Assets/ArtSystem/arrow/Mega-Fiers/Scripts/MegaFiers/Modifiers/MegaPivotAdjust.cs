
using UnityEngine;

[AddComponentMenu("Modifiers/Pivot Adjust")]
public class MegaPivotAdjust : MegaModifier
{
	Matrix4x4 mat;
	public override string ModName() { return "PivotAdjust"; }
	public override string GetHelpURL() { return "?page_id=280"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		p -= Offset;
		return mat.MultiplyPoint(p);
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		tm = Matrix4x4.identity;
		invtm = tm.inverse;
		mat = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(gizmoRot), gizmoScale);
		return true;
	}

	public override void DrawGizmo(MegaModContext context)
	{
		tm = Matrix4x4.identity;
		invtm = tm.inverse;

		if ( !Prepare(context) )
			return;

		Vector3 min = context.bbox.min;
		Vector3 max = context.bbox.max;

		if ( context.mod.sourceObj != null )
			Gizmos.matrix = context.mod.sourceObj.transform.localToWorldMatrix;
		else
			Gizmos.matrix = transform.localToWorldMatrix;

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
}
