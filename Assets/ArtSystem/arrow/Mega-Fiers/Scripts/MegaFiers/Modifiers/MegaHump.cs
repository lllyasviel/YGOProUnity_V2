
using UnityEngine;

[AddComponentMenu("Modifiers/Hump")]
public class MegaHump : MegaModifier
{
	public float	amount	= 0.0f;
	public float	cycles	= 1.0f;
	public float	phase	= 0.0f;
	public bool		animate	= false;
	public float	speed	= 1.0f;
	public MegaAxis	axis	= MegaAxis.Z;
	float amt;
	Vector3	size = Vector3.zero;

	public override string ModName() { return "Hump"; }
	public override string GetHelpURL() { return "?page_id=207"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		switch ( axis )
		{
			case MegaAxis.X: p.x += amt * Mathf.Sin(Mathf.Sqrt(p.x * p.x / size.x) + Mathf.Sqrt(p.y * p.y / size.y) * Mathf.PI / 0.1f * (Mathf.Deg2Rad * cycles) + phase); break;
			case MegaAxis.Y: p.y += amt * Mathf.Sin(Mathf.Sqrt(p.y * p.y / size.y) + Mathf.Sqrt(p.x * p.x / size.x) * Mathf.PI / 0.1f * (Mathf.Deg2Rad * cycles) + phase); break;
			case MegaAxis.Z: p.z += amt * Mathf.Sin(Mathf.Sqrt(p.x * p.x / size.x) + Mathf.Sqrt(p.y * p.y / size.y) * Mathf.PI / 0.1f * (Mathf.Deg2Rad * cycles) + phase); break;
		}
		return invtm.MultiplyPoint3x4(p);
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		if ( animate )
			phase += Time.deltaTime * speed;
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		size = bbox.Size();
		amt = amount / 100.0f;

		return true;
	}
}
