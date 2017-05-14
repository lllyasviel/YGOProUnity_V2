
using UnityEngine;

[AddComponentMenu("MegaShapes/Ellipse")]
public class MegaShapeEllipse : MegaShape
{
	public float length = 1.0f;
	public float width = 1.0f;

	const float CIRCLE_VECTOR_LENGTH = 0.5517861843f;

	public override string GetHelpURL() { return "?page_id=1178"; }

	void MakeCircle(float radius, float xmult, float ymult)
	{
		Matrix4x4 tm = GetMatrix();

		float vector = CIRCLE_VECTOR_LENGTH * radius;
		
		MegaSpline spline = NewSpline();
		Vector3 mult = new Vector3(xmult, ymult, 1.0f);

		for ( int ix = 0; ix < 4; ++ix )
		{
			float angle = 6.2831853f * (float)ix / 4.0f;
			float sinfac = Mathf.Sin(angle);
			float cosfac = Mathf.Cos(angle);
			Vector3 p = new Vector3(cosfac * radius, sinfac * radius, 0.0f);
			Vector3 rotvec = new Vector3(sinfac * vector, -cosfac * vector, 0.0f);
			spline.AddKnot(Vector3.Scale(p, mult), Vector3.Scale((p + rotvec), mult), Vector3.Scale((p - rotvec), mult), tm);
		}

		spline.closed = true;
		CalcLength(10);
	}

	public override void MakeShape()
	{
		length = Mathf.Clamp(length, 0.0f, float.MaxValue);
		width = Mathf.Clamp(width, 0.0f, float.MaxValue);

		float radius, xmult, ymult;
		if ( length < width )
		{
			radius = width;
			xmult = 1.0f;
			ymult = length / width;
		}
		else
		{
			if ( width < length )
			{
				radius = length;
				xmult = width / length;
				ymult = 1.0f;
			}
			else
			{
				radius = length;
				xmult = ymult = 1.0f;
			}
		}

		MakeCircle(radius / 2.0f, xmult, ymult);
		CalcLength(10);
	}
}