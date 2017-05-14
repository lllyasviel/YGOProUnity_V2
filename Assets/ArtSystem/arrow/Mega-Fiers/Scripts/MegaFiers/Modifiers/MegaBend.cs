
using UnityEngine;

[AddComponentMenu("Modifiers/Bend")]
public class MegaBend : MegaModifier
{
	[HideInInspector]
	public float	angle		= 0.0f;
	[HideInInspector]
	public float	dir			= 0.0f;
	[HideInInspector]
	public MegaAxis	axis		= MegaAxis.X;
	[HideInInspector]
	public bool		doRegion	= false;
	[HideInInspector]
	public float	from		= 0.0f;
	[HideInInspector]
	public float	to			= 0.0f;
	Matrix4x4		mat			= new Matrix4x4();
	Matrix4x4		tmAbove		= new Matrix4x4();
	Matrix4x4		tmBelow		= new Matrix4x4();
	float			r			= 0.0f;

	public override string ModName()	{ return "Bend"; }
	public override string GetHelpURL() { return "?page_id=41"; }

	// Virtual method for all mods
	public override void SetValues(MegaModifier mod)
	{
		MegaBend bm = (MegaBend)mod;
		angle = bm.angle;
		dir = bm.dir;
		axis = bm.axis;
		doRegion = bm.doRegion;
		from = bm.from;
		to = bm.to;
	}

	void CalcR(MegaAxis axis, float ang)
	{
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

		if ( Mathf.Abs(ang) < 0.000001f )
			r = 0.0f;
		else
			r = len / ang;
	}

	public override Vector3 Map(int i, Vector3 p)
	{
		if ( r == 0.0f && !doRegion )
			return p;

		p = tm.MultiplyPoint3x4(p);	// tm may have an offset gizmo etc

		if ( doRegion )
		{
			if ( p.y <= from )
				return invtm.MultiplyPoint3x4(tmBelow.MultiplyPoint3x4(p));
			else
			{
				if ( p.y >= to )
					return invtm.MultiplyPoint3x4(tmAbove.MultiplyPoint3x4(p));
			}
		}

		if ( r == 0.0f )
			return invtm.MultiplyPoint3x4(p);

		float x = p.x;
		float y = p.y;

		float yr = y / r;

		float c = Mathf.Cos(Mathf.PI - yr);
		float s = Mathf.Sin(Mathf.PI - yr);
		float px = r * c + r - x * c;
		p.x = px;
		float pz = r * s - x * s;
		p.y = pz;
		p = invtm.MultiplyPoint3x4(p);
		return p;
	}

	void Calc()
	{
		if ( from > to)	from = to;
		if ( to < from ) to = from;
		//if ( from > 0.0f)	from = 0.0f;
		//if ( to < 0.0f )	to = 0.0f;

		mat = Matrix4x4.identity;

		switch ( axis )
		{
			case MegaAxis.X: MegaMatrix.RotateZ(ref mat, Mathf.PI * 0.5f); break;
			case MegaAxis.Y: MegaMatrix.RotateX(ref mat, -Mathf.PI * 0.5f); break;
			case MegaAxis.Z: break;
		}

		MegaMatrix.RotateY(ref mat, Mathf.Deg2Rad * dir);
		SetAxis(mat);

		CalcR(axis, Mathf.Deg2Rad * -angle);

		if ( doRegion )
		{
			doRegion = false;
			float len  = to - from;
			float rat1, rat2;

			if ( len == 0.0f )
				rat1 = rat2 = 1.0f;
			else
			{
				rat1 = to / len;
				rat2 = from / len;
			}

			Vector3 pt;
			tmAbove = Matrix4x4.identity;
			MegaMatrix.Translate(ref tmAbove, 0.0f, -to, 0.0f);
			MegaMatrix.RotateZ(ref tmAbove, -Mathf.Deg2Rad * angle * rat1);
			MegaMatrix.Translate(ref tmAbove, 0.0f, to, 0.0f);
			pt = new Vector3(0.0f, to, 0.0f);
			MegaMatrix.Translate(ref tmAbove, tm.MultiplyPoint3x4(Map(0, invtm.MultiplyPoint3x4(pt))) - pt);

			tmBelow = Matrix4x4.identity;
			MegaMatrix.Translate(ref tmBelow, 0.0f, -from, 0.0f);
			MegaMatrix.RotateZ(ref tmBelow, -Mathf.Deg2Rad * angle * rat2);
			MegaMatrix.Translate(ref tmBelow, 0.0f, from, 0.0f);
			pt = new Vector3(0.0f, from, 0.0f);
			MegaMatrix.Translate(ref tmBelow, tm.MultiplyPoint3x4(Map(0, invtm.MultiplyPoint3x4(pt))) - pt);

			doRegion = true;
		}
	}

	// gizmo might make path deform go twice as fast?
	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		Calc();
		return true;
	}

	public override void ExtraGizmo(MegaModContext mc)
	{
		if ( doRegion )
			DrawFromTo(axis, from, to, mc);
	}
}
