
using UnityEngine;

// Perhaps we should have UVModifier
[AddComponentMenu("Modifiers/UV/Adjust")]
public class MegaUVAdjust : MegaModifier
{
	public bool		animate		= false;
	public float	rotspeed	= 0.0f;
	public float	spiralspeed = 0.0f;
	public Vector3	speed		= Vector3.zero;
	public float	spiral		= 0.0f;
	public float	spirallim	= 360.0f;

	// 3 channels of UV
	public override MegaModChannel ChannelsReq()	 { return MegaModChannel.UV; }
	public override MegaModChannel ChannelsChanged() { return MegaModChannel.UV; }

	public override string ModName() { return "UVAdjust"; }
	public override string GetHelpURL() { return "?page_id=352"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		return p;
	}

	public override void Modify(MegaModifiers mc)
	{
		Vector2[]	uvs = mc.GetSourceUvs();
		Vector2[]	newuvs = mc.GetDestUvs();

		if ( uvs.Length > 0 )
		{
			Vector3 pos = -gizmoPos;
			Vector3 scl = gizmoScale;
			Vector3 rot = gizmoRot;

			Matrix4x4 tm1 = Matrix4x4.identity;
			Vector3 p = Vector3.zero;
			for ( int i = 0; i < uvs.Length; i++ )
			{
				p.x = uvs[i].x - Offset.x - 0.5f;
				p.z = uvs[i].y - Offset.z - 0.5f;
				p.y = 0.0f;

				float d = Mathf.Sqrt(p.x * p.x + p.z * p.z) * spiral;

				rot = new Vector3(gizmoRot.x, gizmoRot.y + d, gizmoRot.z);
				tm1 = Matrix4x4.TRS(pos, Quaternion.Euler(rot), scl);

				p = tm1.MultiplyPoint(p);
				newuvs[i].x = p.x;
				newuvs[i].y = p.z;
			}
		}
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		if ( animate )
		{
			spiral		+= spiralspeed * Time.deltaTime;
			if ( Mathf.Abs(spiral) > spirallim )
			{
				if ( spiral < 0.0f )
					spiral = -spirallim;
				else
					spiral = spirallim;

				spiralspeed = -spiralspeed;
			}
			gizmoRot.y	+= rotspeed * Time.deltaTime;
			gizmoRot.y	= Mathf.Repeat(gizmoRot.y, 360.0f);
			gizmoPos	+= speed * Time.deltaTime;
		}
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		return true;
	}

	public override void DrawGizmo(MegaModContext context)
	{
		tm = Matrix4x4.identity;
		invtm = tm.inverse;

		if ( !Prepare(context) )
			return;

		Vector3 min = new Vector3(-0.5f, 0.0f, -0.5f);
		Vector3 max = new Vector3(0.5f, 0.0f, 0.5f);

		min += Offset;
		max += Offset;

		Matrix4x4 mat = Matrix4x4.identity;

		Vector3 scl = gizmoScale;
		scl.x = 1.0f / scl.x;
		scl.y = 1.0f / scl.y;
		scl.z = 1.0f / scl.z;

		mat.SetTRS(Vector3.Scale(-gizmoPos - Offset, bbox.Size()), Quaternion.Euler(-gizmoRot), scl);

		if ( context.mod.sourceObj != null )
			Gizmos.matrix = context.mod.sourceObj.transform.localToWorldMatrix;
		else
			Gizmos.matrix = transform.localToWorldMatrix;

		corners[0] = mat.MultiplyPoint(Vector3.Scale(new Vector3(min.x, min.y, min.z), bbox.Size()));
		corners[1] = mat.MultiplyPoint(Vector3.Scale(new Vector3(min.x, min.y, max.z), bbox.Size()));
		corners[2] = mat.MultiplyPoint(Vector3.Scale(new Vector3(max.x, min.y, max.z), bbox.Size()));
		corners[3] = mat.MultiplyPoint(Vector3.Scale(new Vector3(max.x, min.y, min.z), bbox.Size()));

		DrawEdge(corners[0], corners[1]);
		DrawEdge(corners[1], corners[2]);
		DrawEdge(corners[2], corners[3]);
		DrawEdge(corners[3], corners[0]);
	}
}