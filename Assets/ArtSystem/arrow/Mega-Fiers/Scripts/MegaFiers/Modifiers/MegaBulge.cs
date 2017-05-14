
using UnityEngine;

// Test scene ideas
// mirrored surface
// lab or display type area
// shelves
// labels for item
// a flippable book
// nice text overlays like confetti/joe danger
// camera flyarounds
// vase type shapes for some modifiers
// multi material
// reflection of enviroment
// 3d gui by each item or on labels or built in steam punk type stuff in display base (how to handle multiple sliders)
// knobs/dials for values
// lights and switches for check boxes
// so from tony need a display base with easy addon components for gui
// could have rotating cyclinder that has bits attached (chain done with path follow)
// if cylinder could hide somewhere, could hide it and change sliders when a new modifier is selected on an object
// objects could levitate and slowly bob

[AddComponentMenu("Modifiers/Bulge")]
public class MegaBulge : MegaModifier
{
	public Vector3	Amount = Vector3.zero;
	public Vector3	FallOff = Vector3.zero;
	public bool		LinkFallOff = true;
	Vector3	per = Vector3.zero;
	float	xsize;
	float	ysize;
	float	zsize;
	float	size;
	float	cx,cy,cz;
	Vector3 dcy = Vector3.zero;

	public override string ModName()	{ return "Bulge"; }
	public override string GetHelpURL() { return "?page_id=163"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		float xw,yw,zw;

		xw = p.x - cx; yw = p.y - cy; zw = p.z - cz;
		if ( xw == 0.0f && yw == 0.0f && zw == 0.0f )
			xw = yw = zw = 1.0f;
		float vdist = Mathf.Sqrt(xw * xw + yw * yw + zw * zw);
		float mfac = size / vdist;

		dcy.x = Mathf.Exp(-FallOff.x * Mathf.Abs(xw));

		if ( !LinkFallOff )
		{
			dcy.y = Mathf.Exp(-FallOff.y * Mathf.Abs(yw));
			dcy.z = Mathf.Exp(-FallOff.z * Mathf.Abs(zw));
		}
		else
		{
			dcy.y = dcy.z = dcy.x;
		}

		p.x = cx + xw + (Mathf.Sign(xw) * ((Mathf.Abs(xw * mfac) - Mathf.Abs(xw)) * per.x) * dcy.x);
		p.y = cy + yw + (Mathf.Sign(yw) * ((Mathf.Abs(yw * mfac) - Mathf.Abs(yw)) * per.y) * dcy.y);
		p.z = cz + zw + (Mathf.Sign(zw) * ((Mathf.Abs(zw * mfac) - Mathf.Abs(zw)) * per.z) * dcy.z);
		return invtm.MultiplyPoint3x4(p);
	}

	public override void ModStart(MegaModifiers mc)
	{
		xsize = bbox.max.x - bbox.min.x;
		ysize = bbox.max.y - bbox.min.y;
		zsize = bbox.max.z - bbox.min.z;
		size = (xsize > ysize) ? xsize : ysize;
		size = (zsize > size) ? zsize : size;
		size /= 2.0f;
		cx = bbox.center.x;
		cy = bbox.center.y;
		cz = bbox.center.z;

		// Get the percentage to spherify at this time
		per = Amount / 100.0f;
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		xsize = bbox.max.x - bbox.min.x;
		ysize = bbox.max.y - bbox.min.y;
		zsize = bbox.max.z - bbox.min.z;
		size = (xsize > ysize) ? xsize : ysize;
		size = (zsize > size) ? zsize : size;
		size /= 2.0f;
		cx = bbox.center.x;
		cy = bbox.center.y;
		cz = bbox.center.z;

		// Get the percentage to spherify at this time
		per = Amount / 100.0f;

		return true;
	}
}
