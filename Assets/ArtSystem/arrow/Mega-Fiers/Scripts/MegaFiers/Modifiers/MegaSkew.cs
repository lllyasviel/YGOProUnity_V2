
using UnityEngine;

[AddComponentMenu("Modifiers/Skew")]
public class MegaSkew : MegaModifier
{
	public float	amount			= 0.0f;
	public bool		doRegion		= false;
	public float	to				= 0.0f;
	public float	from			= 0.0f;
	public float	dir				= 0.0f;
	public MegaAxis	axis			= MegaAxis.X;
	Matrix4x4		mat				= new Matrix4x4();
	float			amountOverLength = 0.0f;

	public override string ModName() { return "Skew"; }
	public override string GetHelpURL() { return "?page_id=319"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);
		float z = p.y;

		if ( doRegion )
		{
			if ( p.y < from )
				z = from;
			else
				if ( p.y > to )
					z = to;
		}

		p.x -= z * amountOverLength;
		return invtm.MultiplyPoint3x4(p);
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		if ( from > 0.0f )
			from = 0.0f;

		if ( to < 0.0f )
			to = 0.0f;

		mat = Matrix4x4.identity;

		switch ( axis )
		{
			case MegaAxis.X: MegaMatrix.RotateZ(ref mat, Mathf.PI * 0.5f);	break;
			case MegaAxis.Y: MegaMatrix.RotateX(ref mat, -Mathf.PI * 0.5f); break;
			case MegaAxis.Z: break;
		}

		MegaMatrix.RotateY(ref mat, Mathf.Deg2Rad * dir);
		SetAxis(mat);

		float len = 0.0f;
		if ( !doRegion )
		{
			switch ( axis )
			{
				case MegaAxis.X: len = bbox.max.x - bbox.min.x; break;
				case MegaAxis.Z: len = bbox.max.y - bbox.min.y; break;
				case MegaAxis.Y: len = bbox.max.z - bbox.min.z; break;
			}
		}
		else
			len = to - from;

		if ( len == 0.0f )
			len = 0.000001f;

		amountOverLength = amount / len;
		return true;
	}

	public override void ExtraGizmo(MegaModContext mc)
	{
		if ( doRegion )
			DrawFromTo(axis, from, to, mc);
	}
}
