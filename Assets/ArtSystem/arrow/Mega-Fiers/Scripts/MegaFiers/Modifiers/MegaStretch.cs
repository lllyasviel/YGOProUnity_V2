
using UnityEngine;

[AddComponentMenu("Modifiers/Stretch")]
public class MegaStretch : MegaModifier
{
	public float	amount		= 0.0f;
	public bool		doRegion	= false;
	public float	to			= 0.0f;
	public float	from		= 0.0f;
	public float	amplify		= 0.0f;
	public MegaAxis	axis		= MegaAxis.X;
	float			heightMax	= 0.0f;
	float			heightMin	= 0.0f;
	float			amplifier	= 0.0f;
	Matrix4x4		mat			= new Matrix4x4();

	public override string ModName()	{ return "Stretch"; }
	public override string GetHelpURL() { return "?page_id=334"; }

	void CalcBulge(MegaAxis axis, float stretch, float amplify)
	{
		amount = stretch;
		amplifier = (amplify >= 0.0f) ? amplify + 1.0f : 1.0f / (-amplify + 1.0f);

		if ( !doRegion )
		{
			switch ( axis )
			{
				case MegaAxis.X:
				heightMin = bbox.min.x;
				heightMax = bbox.max.x;
				break;

				case MegaAxis.Z:
				heightMin = bbox.min.y;
				heightMax = bbox.max.y;
				break;

				case MegaAxis.Y:
				heightMin = bbox.min.z;
				heightMax = bbox.max.z;
				break;
			}
		}
		else
		{
			heightMin = from;
			heightMax = to;
		}
	}

	public override Vector3 Map(int i, Vector3 p)
	{
		float normHeight;
		float xyScale, zScale;

		if ( amount == 0.0f || (heightMax - heightMin == 0) )
			return p;

		if ( (doRegion) && (to - from == 0.0f) )
			return p;

		p = tm.MultiplyPoint3x4(p);

		if ( doRegion && p.y > to )
			normHeight = (to - heightMin) / (heightMax - heightMin);
		else if ( doRegion && p.y < from )
			normHeight = (from - heightMin) / (heightMax - heightMin);
		else
			normHeight = (p.y - heightMin) / (heightMax - heightMin);

		if ( amount < 0.0f )
		{
			xyScale = (amplifier * -amount + 1.0f);
			zScale = (-1.0f / (amount - 1.0f));
		}
		else
		{
			xyScale = 1.0f / (amplifier * amount + 1.0f);
			zScale = amount + 1.0f;
		}

		float a = 4.0f * (1.0f - xyScale);
		float b = -4.0f * (1.0f - xyScale);
		float c = 1.0f;
		float fraction = (((a * normHeight) + b) * normHeight) + c;
		p.x *= fraction;
		p.z *= fraction;

		if ( doRegion && p.y < from )
			p.y += (zScale - 1.0f) * from;
		else if ( doRegion && p.y <= to )
			p.y *= zScale;
		else if ( doRegion && p.y > to )
			p.y += (zScale - 1.0f) * to;
		else
			p.y *= zScale;

		p = invtm.MultiplyPoint3x4(p);

		return p;
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		mat = Matrix4x4.identity;

		switch ( axis )
		{
			case MegaAxis.X: MegaMatrix.RotateZ(ref mat, Mathf.PI * 0.5f); break;
			case MegaAxis.Y: MegaMatrix.RotateX(ref mat, -Mathf.PI * 0.5f); break;
			case MegaAxis.Z: break;
		}

		SetAxis(mat);
		CalcBulge(axis, amount, amplify);
		return true;
	}

	public override void ExtraGizmo(MegaModContext mc)
	{
		if ( doRegion )
			DrawFromTo(axis, from, to, mc);
	}
}
