
using UnityEngine;

[AddComponentMenu("Modifiers/Noise")]
public class MegaNoise : MegaModifier
{
	public float	Scale		= 1.0f;
	public bool		Fractal		= false;
	public float	Freq		= 0.25f;
	public float	Iterations	= 6.0f;
	public bool		Animate		= false;
	public float	Phase		= 0.0f;
	public float	Rough		= 0.0f;
	public Vector3	Strength	= new Vector3(0.0f, 0.0f, 0.0f);
	MegaPerlin		iperlin		= MegaPerlin.Instance;
	float			time		= 0.0f;
	float			scale;
	float			rt;

	Vector3 sp = Vector3.zero;
	Vector3 d = Vector3.zero;

	public override string ModName() { return "Noise"; }
	public override string GetHelpURL() { return "?page_id=262"; }

	public override void Modify(MegaModifiers mc)
	{
		for ( int i = 0; i < verts.Length; i++ )
		{
			Vector3 p = tm.MultiplyPoint3x4(verts[i]);

			sp.x = p.x * scale + 0.5f;
			sp.y = p.y * scale + 0.5f;
			sp.z = p.z * scale + 0.5f;

			if ( Fractal )
			{
				d.x = iperlin.fBm1(sp.y, sp.z, time, rt, 2.0f, Iterations);
				d.y = iperlin.fBm1(sp.x, sp.z, time, rt, 2.0f, Iterations);
				d.z = iperlin.fBm1(sp.x, sp.y, time, rt, 2.0f, Iterations);
			}
			else
			{
				d.x = iperlin.Noise(sp.y, sp.z, time);
				d.y = iperlin.Noise(sp.x, sp.z, time);
				d.z = iperlin.Noise(sp.x, sp.y, time);
			}

			p.x += d.x * Strength.x;
			p.y += d.y * Strength.y;
			p.z += d.z * Strength.z;

			sverts[i] = invtm.MultiplyPoint3x4(p);
		}
	}

	public override void ModStart(MegaModifiers mc)
	{
	}

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		float spx = p.x * scale + 0.5f;
		float spy = p.y * scale + 0.5f;
		float spz = p.z * scale + 0.5f;

		float dx,dy,dz;

		if ( Fractal )
		{
			dx = iperlin.fBm1(spy, spz, time, rt, 2.0f, Iterations);
			dy = iperlin.fBm1(spx, spz, time, rt, 2.0f, Iterations);
			dz = iperlin.fBm1(spx, spy, time, rt, 2.0f, Iterations);
		}
		else
		{
			dx = iperlin.Noise(spy, spz, time);
			dy = iperlin.Noise(spx, spz, time);
			dz = iperlin.Noise(spx, spy, time);
		}

		p.x += dx * Strength.x;
		p.y += dy * Strength.y;
		p.z += dz * Strength.z;

		return invtm.MultiplyPoint3x4(p);
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		if ( Animate )
			Phase += Time.deltaTime * Freq;
		time = Phase;

		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		// Need this in a GetDeformer type method, then drawgizmo can be common
		if ( Scale == 0.0f )
			scale = 0.000001f;
		else
			scale = 1.0f / Scale;

		rt = 1.0f - Rough;

		return true;
	}
}
