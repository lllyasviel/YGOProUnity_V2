
using UnityEngine;

[AddComponentMenu("Modifiers/Warps/Taper")]
public class MegaTaperWarp : MegaWarp
{
	public override string WarpName() { return "Taper"; }
	public override string GetIcon() { return "MegaTaper icon.png"; }

	public float			amount		= 0.0f;
	public bool				doRegion	= false;
	public float			to			= 0.0f;
	public float			from		= 0.0f;
	public float			dir			= 0.0f;
	public MegaAxis			axis		= MegaAxis.X;
	public MegaEffectAxis	EAxis		= MegaEffectAxis.X;
	Matrix4x4				mat			= new Matrix4x4();
	public float			crv			= 0.0f;
	public bool				sym			= false;
	bool	doX = false;
	bool	doY = false;
	float k1;
	float k2;
	float l;

	void SetK(float K1, float K2)
	{
		k1 = K1;
		k2 = K2;
	}

	public override Vector3 Map(int i, Vector3 p)
	{
		float z;

		if ( l == 0.0f )
			return p;

		p = tm.MultiplyPoint3x4(p);

		Vector3 ip = p;
		float dist = p.magnitude;
		float dcy = Mathf.Exp(-totaldecay * Mathf.Abs(dist));

		if ( doRegion )
		{
			if ( p.y < from )
				z = from / l;
			else
			{
				if ( p.y > to )
					z = to / l;
				else
					z = p.y / l;
			}
		}
		else
			z = p.y / l;

		if ( sym && z < 0.0f )
			z = -z;

		float f =  1.0f + z * k1 + k2 * z * (1.0f - z);

		if ( doX )
			p.x *= f;

		if ( doY )
			p.z *= f;

		p = Vector3.Lerp(ip, p, dcy);

		return invtm.MultiplyPoint3x4(p);
	}

	public override bool Prepare(float decay)
	{
		tm = transform.worldToLocalMatrix;
		invtm = tm.inverse;

		switch ( EAxis )
		{
			case MegaEffectAxis.X: doX = true; doY = false; break;
			case MegaEffectAxis.Y: doX = false; doY = true; break;
			case MegaEffectAxis.XY: doX = true; doY = true; break;
		}

		mat = Matrix4x4.identity;
		switch ( axis )
		{
			case MegaAxis.X: MegaMatrix.RotateZ(ref mat, Mathf.PI * 0.5f); l = Width; break;
			case MegaAxis.Y: MegaMatrix.RotateX(ref mat, -Mathf.PI * 0.5f); l = Length; break;
			case MegaAxis.Z: l = Height; break;
		}

		MegaMatrix.RotateY(ref mat, Mathf.Deg2Rad * dir);

		SetAxis(mat);
		SetK(amount, crv);

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