
using UnityEngine;

[AddComponentMenu("Modifiers/Warps/Noise")]
public class MegaNoiseWarp : MegaWarp
{
	public float	Scale		= 1.0f;
	public bool		Fractal		= false;
	public float	Freq		= 0.25f;
	public float	Iterations	= 6.0f;
	public bool		Animate		= false;
	public float	Phase		= 0.0f;
	public float	Rough		= 0.0f;
	public Vector3	Strength	= new Vector3(0.0f, 0.0f, 0.0f);
	MegaPerlin	iperlin		= MegaPerlin.Instance;
	float			time		= 0.0f;
	float			scale;
	float			rt;
	//Vector3			half		= new Vector3(0.5f, 0.5f, 0.5f);
	Vector3			d			= new Vector3();

	public override string WarpName() { return "Noise"; }
	public override string GetIcon() { return "MegaNoise icon.png"; }

	Vector3 sp = Vector3.zero;

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		Vector3 ip = p;
		float dist = p.magnitude;
		float dcy = Mathf.Exp(-totaldecay * Mathf.Abs(dist));

		//sp = p * scale + half;

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

		//p += Vector3.Scale(d, Strength);
		p.x += d.x * Strength.x;
		p.y += d.y * Strength.y;
		p.z += d.z * Strength.z;
		//p = Vector3.Lerp(ip, p, dcy);

		p.x = ip.x + ((p.x - ip.x) * dcy);
		p.y = ip.y + ((p.y - ip.y) * dcy);
		p.z = ip.z + ((p.z - ip.z) * dcy);

		//p.x = ip.x + ((p.x + (d.x * Strength.x) - ip.x) * dcy);
		//p.y = ip.y + ((p.y + (d.y * Strength.y) - ip.y) * dcy);
		//p.z = ip.z + ((p.z + (d.z * Strength.z) - ip.z) * dcy);

		return invtm.MultiplyPoint3x4(p);	// + Vector3.Scale(d, Strength));
	}

	void Update()
	{
		if ( Animate )
			Phase += Time.deltaTime * Freq;
		time = Phase;

		//return Prepare();
	}

	public override bool Prepare(float decay)
	{
		tm = transform.worldToLocalMatrix;
		invtm = tm.inverse;

		if ( Scale == 0.0f )
			scale = 0.000001f;
		else
			scale = 1.0f / Scale;

		rt = 1.0f - Rough;

		totaldecay = Decay + decay;
		if ( totaldecay < 0.0f )
			totaldecay = 0.0f;

		return true;
	}
}