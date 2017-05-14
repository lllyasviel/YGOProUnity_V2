
using UnityEngine;

[AddComponentMenu("MegaShapes/Circle")]
public class MegaShapeCircle : MegaShape
{
	public float Radius = 1.0f;

	const float CIRCLE_VECTOR_LENGTH = 0.5517861843f;

	public override void MakeShape()
	{
		Matrix4x4 tm = GetMatrix();

		float vector = CIRCLE_VECTOR_LENGTH * Radius;
		
		MegaSpline spline = NewSpline();

		for ( int ix = 0; ix < 4; ++ix )
		{
			float angle = (Mathf.PI * 2.0f) * (float)ix / (float)4;
			float sinfac = Mathf.Sin(angle);
			float cosfac = Mathf.Cos(angle);
			Vector3 p = new Vector3(cosfac * Radius, sinfac * Radius, 0.0f);
			Vector3 rotvec = new Vector3(sinfac * vector, -cosfac * vector, 0.0f);
			spline.AddKnot(p, p + rotvec, p - rotvec, tm);
		}

		spline.closed = true;
		CalcLength(10);
	}
}