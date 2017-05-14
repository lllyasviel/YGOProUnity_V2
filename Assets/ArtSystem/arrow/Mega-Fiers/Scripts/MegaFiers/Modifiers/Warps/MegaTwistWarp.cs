
using UnityEngine;

[AddComponentMenu("Modifiers/Warps/Twist")]
public class MegaTwistWarp : MegaWarp
{
	public override string WarpName() { return "Twist"; }
	public override string GetIcon() { return "MegaTwist icon.png"; }

	// Deformer
	public float	angle		= 0.0f;
	public bool		doRegion	= false;
	public float	from		= 0.0f;
	public float	to			= 0.0f;
	public float	Bias		= 0.0f;
	public MegaAxis	axis		= MegaAxis.X;
	bool			doBias		= false;
	float			height		= 0.0f;
	float			angleOverHeight = 0.0f;
	float			theAngle;
	float			bias;
	Matrix4x4		mat = new Matrix4x4();

	void CalcHeight(MegaAxis axis, float angle)
	{
		switch ( axis )
		{
			case MegaAxis.X: height = Width; break;
			case MegaAxis.Z: height = Height; break;
			case MegaAxis.Y: height = Length; break;
		}

		if ( height == 0.0f )
		{
			theAngle = 0.0f;
			angleOverHeight = 0.0f;
		}
		else
		{
			theAngle = angle;
			angleOverHeight = angle / height;
		}
	}

	public override Vector3 Map(int i, Vector3 p)
	{
		float z, a;
		if ( theAngle == 0.0f )
			return p;

		p = tm.MultiplyPoint3x4(p);

		Vector3 ip = p;
		float dist = p.magnitude;
		float dcy = Mathf.Exp(-totaldecay * Mathf.Abs(dist));

		float x = p.x;
		float y = p.z;

		if ( doRegion )
		{
			if ( p.y < from )
				z = from;
			else
			{
				if ( p.y > to )
					z = to;
				else
					z = p.y;
			}
		}
		else
			z = p.y;

		if ( doBias )
		{
			float u = z / height;
			a = theAngle * (float)Mathf.Pow(Mathf.Abs(u), bias);
			if ( u < 0.0f )
				a = -a;
		}
		else
			a = z * angleOverHeight;

		float cosine = Mathf.Cos(Mathf.Deg2Rad * a);
		float sine = Mathf.Sin(Mathf.Deg2Rad * a);
		p.x = cosine * x + sine * y;
		p.z = -sine * x + cosine * y;

		p = Vector3.Lerp(ip, p, dcy);
		p = invtm.MultiplyPoint3x4(p);

		return p;
	}

	public override bool Prepare(float decay)
	{
		tm = transform.worldToLocalMatrix;
		invtm = tm.inverse;

		mat = Matrix4x4.identity;

		switch ( axis )
		{
			case MegaAxis.X: MegaMatrix.RotateZ(ref mat, Mathf.PI * 0.5f); break;
			case MegaAxis.Y: MegaMatrix.RotateX(ref mat, -Mathf.PI * 0.5f); break;
			case MegaAxis.Z: break;
		}

		SetAxis(mat);

		if ( Bias != 0.0f )
		{
			bias = 1.0f - (Bias + 100.0f) / 200.0f;
			if ( bias < 0.00001f )
				bias = 0.00001f;

			if ( bias > 0.99999f )
				bias = 0.99999f;

			bias = Mathf.Log(bias) / Mathf.Log(0.5f);
			doBias = true;
		}
		else
		{
			bias = 1.0f;
			doBias = false;
		}

		CalcHeight(axis, angle);

		totaldecay = Decay + decay;
		if ( totaldecay < 0.0f )
			totaldecay = 0.0f;

		if ( from > to ) from = to;
		if ( to < from ) to = from;

		return true;
	}

	public override void ExtraGizmo()
	{
		if ( doRegion )
			DrawFromTo(axis, from, to);
	}
}