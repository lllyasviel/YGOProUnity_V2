
using UnityEngine;

[AddComponentMenu("Modifiers/Crumple")]
public class MegaCrumple : MegaModifier
{
	public float		scale = 1.0f;
	public float		speed = 1.0f;
	public float		phase = 0.0f;
	public bool			animate = false;
	Matrix4x4			mat = new Matrix4x4();

	MegaPerlin		iperlin		= MegaPerlin.Instance;

	public override string ModName() { return "Crumple"; }
	public override string GetHelpURL() { return "?page_id=653"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		p.x += iperlin.Noise(timex + p.x, timex + p.y, timex + p.z) * scale;
		p.y += iperlin.Noise(timey + p.x, timey + p.y, timey + p.z) * scale;
		p.z += iperlin.Noise(timez + p.x, timez + p.y, timez + p.z) * scale;

		return invtm.MultiplyPoint3x4(p);
	}

	float timex = 0.0f;
	float timey = 0.0f;
	float timez = 0.0f;

	public override bool ModLateUpdate(MegaModContext mc)
	{
		if ( animate )
			phase += Time.deltaTime * speed;

		timex = 0.1365143f + phase;
		timey = 1.21688f + phase;
		timez = 2.5564f + phase;

		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		mat = Matrix4x4.identity;

		SetAxis(mat);
		return true;
	}
}