
using UnityEngine;

[AddComponentMenu("Modifiers/Warps/Skew")]
public class MegaSkewWarp : MegaWarp
{
	public float	amount		= 0.0f;
	public bool		doRegion	= false;
	public float	to			= 0.0f;
	public float	from		= 0.0f;
	public float	dir			= 0.0f;
	public MegaAxis	axis		= MegaAxis.X;
	Matrix4x4		mat			= new Matrix4x4();
	float			amountOverLength = 0.0f;

	public override string WarpName() { return "Skew"; }
	public override string GetIcon() { return "MegaSkew icon.png"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		Vector3 ip = p;
		float dist = p.magnitude;
		float dcy = Mathf.Exp(-totaldecay * Mathf.Abs(dist));

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

		p = Vector3.Lerp(ip, p, dcy);

		return invtm.MultiplyPoint3x4(p);
	}

	public override bool Prepare(float decay)
	{
		tm = transform.worldToLocalMatrix;
		invtm = tm.inverse;

		if ( from > 0.0f )
			from = 0.0f;

		if ( to < 0.0f )
			to = 0.0f;

		mat = Matrix4x4.identity;

		switch ( axis )
		{
			case MegaAxis.X: MegaMatrix.RotateZ(ref mat, Mathf.PI * 0.5f); break;
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
				case MegaAxis.X: len = Width; break;
				case MegaAxis.Z: len = Height; break;
				case MegaAxis.Y: len = Length; break;
			}
		}
		else
			len = to - from;

		if ( len == 0.0f )
			len = 0.000001f;

		amountOverLength = amount / len;

		totaldecay = Decay + decay;
		if ( totaldecay < 0.0f )
			totaldecay = 0.0f;

		return true;
	}

	public override void ExtraGizmo()
	{
		if ( doRegion )
			DrawFromTo(axis, from, to);
	}
}