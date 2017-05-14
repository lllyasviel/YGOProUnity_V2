
using UnityEngine;
using System.IO;

[AddComponentMenu("Modifiers/Bubble")]
public class MegaBubble : MegaModifier
{
	public float		radius = 0.0f;
	public float		falloff = 20.0f;
	Matrix4x4			mat = new Matrix4x4();

	public override string ModName()	{ return "Bubble"; }
	public override string GetHelpURL() { return "?page_id=111"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		float val = ((Vector3.Magnitude(p - bbox.center)) / falloff);
		p += radius * (Vector3.Normalize(p - bbox.center)) / (val * val + 1.0f);

		return invtm.MultiplyPoint3x4(p);
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		mat = Matrix4x4.identity;

		SetAxis(mat);
		return true;
	}
}
