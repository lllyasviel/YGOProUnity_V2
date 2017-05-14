
using UnityEngine;

[AddComponentMenu("Modifiers/Curve Deform")]
public class MegaCurveDeform : MegaModifier
{
	public MegaAxis			axis = MegaAxis.X;
	public AnimationCurve	defCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(0.5f, 0.0f), new Keyframe(1.0f, 0.0f));
	public float			MaxDeviation = 1.0f;
	float					width	= 0.0f;
	int						ax;

	Keyframe key = new Keyframe();

	public override string ModName()	{ return "CurveDeform"; }
	public override string GetHelpURL() { return "?page_id=655"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		float alpha = (p[ax] - bbox.min[ax]) / width;
		p.y += defCurve.Evaluate(alpha);

		return invtm.MultiplyPoint3x4(p);
	}

	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		ax = (int)axis;
		width = bbox.max[ax] - bbox.min[ax];
		return true;
	}

	public float GetPos(float alpha)
	{
		float y = defCurve.Evaluate(alpha);
		return y;
	}

	public void SetKey(int index, float t, float v, float intan, float outtan)
	{
		key.time = t;
		key.value = v;
		key.inTangent = intan;
		key.outTangent = outtan;
		defCurve.MoveKey(index, key);
	}
}