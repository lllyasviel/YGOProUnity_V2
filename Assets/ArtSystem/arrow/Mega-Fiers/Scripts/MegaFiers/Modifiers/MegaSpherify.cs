
using UnityEngine;

[AddComponentMenu("Modifiers/Spherify")]
public class MegaSpherify : MegaModifier
{
	public float		percent = 0.0f;
	public float		FallOff = 0.0f;
	float per;
	float xsize,ysize,zsize;
	float size;
	float cx,cy,cz;
	public override string ModName()	{ return "Spherify"; }
	public override string GetHelpURL() { return "?page_id=322"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		float xw,yw,zw;

		xw = p.x - cx; yw = p.y - cy; zw = p.z - cz;
		if ( xw == 0.0f && yw == 0.0f && zw == 0.0f )
			xw = yw = zw = 1.0f;
		float vdist = Mathf.Sqrt(xw * xw + yw * yw + zw * zw);
		float mfac = size / vdist;

		float dcy = Mathf.Exp(-FallOff * Mathf.Abs(vdist));

		p.x = cx + xw + (Mathf.Sign(xw) * ((Mathf.Abs(xw * mfac) - Mathf.Abs(xw)) * per) * dcy);
		p.y = cy + yw + (Mathf.Sign(yw) * ((Mathf.Abs(yw * mfac) - Mathf.Abs(yw)) * per) * dcy);
		p.z = cz + zw + (Mathf.Sign(zw) * ((Mathf.Abs(zw * mfac) - Mathf.Abs(zw)) * per) * dcy);
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
		per = percent / 100.0f;
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
		per = percent / 100.0f;

		return true;
	}
}
