
using UnityEngine;

[AddComponentMenu("Modifiers/Sinus Curve")]
public class MegaSinusCurve : MegaModifier
{
	public float		scale = 1.0f;
	public float		speed = 1.0f;
	public float		phase = 0.0f;
	public bool			animate = false;
	Matrix4x4			mat = new Matrix4x4();

	public override string ModName() { return "Sinus Curve"; }
	public override string GetHelpURL() { return "Bubble.htm"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		p.y += Mathf.Sin(phase + p.x + p.y + p.z) * scale;

		return invtm.MultiplyPoint3x4(p);
	}

	public override bool ModLateUpdate(MegaModContext mc)
	{
		if ( animate )
			phase += Time.deltaTime * speed;

		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		mat = Matrix4x4.identity;

		SetAxis(mat);
		return true;
	}
}