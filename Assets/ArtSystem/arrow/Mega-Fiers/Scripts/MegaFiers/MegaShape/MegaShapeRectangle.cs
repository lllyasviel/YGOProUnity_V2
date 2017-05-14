
using UnityEngine;

[AddComponentMenu("MegaShapes/Rectangle")]
public class MegaShapeRectangle : MegaShape
{
	public float length = 1.0f;
	public float width = 1.0f;
	public float fillet = 0.0f;

	const float CIRCLE_VECTOR_LENGTH = 0.5517861843f;

	public override string GetHelpURL() { return "?page_id=1189"; }

	public override void MakeShape()
	{
		Matrix4x4 tm = GetMatrix();

		length = Mathf.Clamp(length, 0.0f, float.MaxValue);
		width = Mathf.Clamp(width, 0.0f, float.MaxValue);
		fillet = Mathf.Clamp(fillet, 0.0f, float.MaxValue);
		MegaSpline spline = NewSpline();

		float l2 = length / 2.0f;
		float w2 = width / 2.0f;
		Vector3 p = new Vector3(w2, l2, 0.0f);

		if ( fillet > 0.0f )
		{
			float cf = fillet * CIRCLE_VECTOR_LENGTH;
			Vector3 wvec = new Vector3(fillet, 0.0f, 0.0f);
			Vector3 lvec = new Vector3(0.0f, fillet, 0.0f);
			Vector3 cwvec = new Vector3(cf, 0.0f, 0.0f);
			Vector3 clvec = new Vector3(0.0f, cf, 0.0f);
			Vector3 p3 = p - lvec;
			spline.AddKnot(p3, p3 - clvec, p3 + clvec, tm);
			p = p - wvec;
			spline.AddKnot(p, p + cwvec, p - cwvec, tm);
			p = new Vector3(-w2, l2, 0.0f);
			Vector3 p2 = p + wvec;
			spline.AddKnot(p2, p2 + cwvec, p2 - cwvec, tm);
			p = p - lvec;
			spline.AddKnot(p, p + clvec, p - clvec, tm);
			p = new Vector3(-w2, -l2, 0.0f);
			p3 = p + lvec;
			spline.AddKnot(p3, p3 + clvec, p3 - clvec, tm);
			p = p + wvec;
			spline.AddKnot(p, p - cwvec, p + cwvec, tm);
			p = new Vector3(w2, -l2, 0.0f);
			p3 = p - wvec;
			spline.AddKnot(p3, p3 - cwvec, p3 + cwvec, tm);
			p = p + lvec;
			spline.AddKnot(p, p - clvec, p + clvec, tm);
		}
		else
		{
			spline.AddKnot(p, p, p, tm);
			p = new Vector3(-w2, l2, 0.0f);
			spline.AddKnot(p, p, p, tm);
			p = new Vector3(-w2, -l2, 0.0f);
			spline.AddKnot(p, p, p, tm);
			p = new Vector3(w2, -l2, 0.0f);
			spline.AddKnot(p, p, p, tm);
		}

		spline.closed = true;
		CalcLength(10);
	}
}