using UnityEngine;

[AddComponentMenu("Modifiers/Vertical Noise")]
public class MegaVertNoise : MegaModifier
{
	public float	Scale		= 1.0f;
	public bool		Fractal		= false;
	public float	Freq		= 0.25f;
	public float	Iterations	= 6.0f;
	public bool		Animate		= false;
	public float	Phase		= 0.0f;
	public float	Rough		= 0.0f;
	public float	Strength	= 0.0f;
	MegaPerlin		iperlin		= MegaPerlin.Instance;
	float			time		= 0.0f;
	float			scale;
	float			rt;
	//public Vector3			half		= new Vector3(0.5f, 0.5f, 0.5f);
	//Vector3			d			= new Vector3();
	public float decay = 0.0f;

	public override string ModName() { return "Vertical Noise"; }
	public override string GetHelpURL() { return "?page_id=1063"; }

	public override void Modify(MegaModifiers mc)
	{
		for ( int i = 0; i < verts.Length; i++ )
		{
			Vector3 p = tm.MultiplyPoint3x4(verts[i]);
			float ipy = p.y;

			float spx = p.x * scale + 0.5f;	//half;
			float spz = p.z * scale + 0.5f;	//half;

			float dist = Mathf.Sqrt(p.x * p.x + p.z * p.z);
			float dcy = Mathf.Exp(-decay * Mathf.Abs(dist));

			float dy = 0.0f;
			if ( Fractal )
				dy = iperlin.fBm1(spx, spz, time, rt, 2.0f, Iterations);
			else
				dy = iperlin.Noise(spx, spz, time);

			p.y += dy * Strength;

			p.y = ipy + ((p.y - ipy) * dcy);

			sverts[i] = invtm.MultiplyPoint3x4(p);
		}
	}

	public override void ModStart(MegaModifiers mc)
	{
	}

	//Vector3 ip = Vector3.zero;
	//Vector3 d = Vector3.zero;
	//Vector3 sp = Vector3.zero;

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);
		float ipy = p.y;

		float dist = Mathf.Sqrt(p.x * p.x + p.z * p.z);
		float dcy = Mathf.Exp(-decay * Mathf.Abs(dist));

		float spx = p.x * scale + 0.5f;
		float spz = p.z * scale + 0.5f;

		float dy = 0.0f;

		if ( Fractal )
			dy = iperlin.fBm1(spx, spz, time, rt, 2.0f, Iterations);
		else
			dy = iperlin.Noise(spx, spz, time);

		p.y += dy * Strength;
		p.y = ipy + ((p.y - ipy) * dcy);

		return invtm.MultiplyPoint3x4(p);	// + Vector3.Scale(d, Strength));
	}

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
