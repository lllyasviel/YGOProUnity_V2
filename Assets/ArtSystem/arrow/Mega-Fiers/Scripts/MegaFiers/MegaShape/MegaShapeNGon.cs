
using UnityEngine;

[AddComponentMenu("MegaShapes/NGon")]
public class MegaShapeNGon : MegaShape
{
	public float	radius	= 1.0f;
	public float	fillet	= 0.0f;
	public int		sides	= 6;
	public bool		circular = false;
	public bool		scribe = false;

	const float CIRCLE_VECTOR_LENGTH = 0.5517861843f;

	public override string GetHelpURL() { return "?page_id=500"; }

	public override void MakeShape()
	{
		Matrix4x4 tm = GetMatrix();

		radius = Mathf.Clamp(radius, 0, float.MaxValue);
		sides = Mathf.Clamp(sides, 3, 100);
		//LimitValue( circular, MIN_CIRCULAR, MAX_CIRCULAR );
		//fillet = Mathf.Clamp(fillet, 0.0f, float.MaxValue);
		//LimitValue( scribe, CIRCUMSCRIBED, INSCRIBED );
		float vector;

		float userad = radius;
		// If circumscribed, modify the radius
		if ( scribe )	//== CIRCUMSCRIBED )
			userad = radius / Mathf.Cos((Mathf.PI * 2.0f) / ((float)sides * 2.0f));

		MegaSpline spline = NewSpline();

		// Determine the vector length if circular
		if ( circular )
			vector = veccalc(6.2831853f / (float)sides) * userad;
		else
			vector = 0.0f;

		// Now add all the necessary points
		if ( (fillet == 0.0f) || circular )
		{
			for ( int ix = 0; ix < sides; ++ix ) 
			{
				float angle = 6.2831853f * (float)ix / (float)sides;
				float sinfac = Mathf.Sin(angle);
				float cosfac = Mathf.Cos(angle);
				Vector3 p = new Vector3(cosfac * userad, sinfac * userad, 0.0f);
				Vector3 rotvec = new Vector3(sinfac * vector, -cosfac * vector, 0.0f);
				spline.AddKnot(p, p + rotvec, p - rotvec, tm);
			}

		  spline.closed = true;
		}
		else
		{
			for ( int ix = 0; ix < sides; ++ix )
			{
				float angle = 6.2831853f * (float)ix / (float)sides;
				float theta2 = (Mathf.PI * (sides - 2) / sides) / 2.0f;
				float fang = angle + theta2;
				float fang2 = angle - theta2;
				float f = fillet * Mathf.Tan((Mathf.PI * 0.5f) - theta2);
				float sinfac = Mathf.Sin(angle);
				float cosfac = Mathf.Cos(angle);
				Vector3 p = new Vector3(cosfac * userad, sinfac * userad, 0.0f);	//,p1,p2;
				Vector3 fvec1 = new Vector3(-Mathf.Cos(fang), -Mathf.Sin(fang), 0.0f) * f;
				Vector3 fvec2 = new Vector3(-Mathf.Cos(fang2), -Mathf.Sin(fang2), 0.0f) * f;
				Vector3 p1 = p + fvec1;
				Vector3 p2 = p + fvec2;
				Vector3 p1vec = fvec1 * CIRCLE_VECTOR_LENGTH;
				Vector3 p2vec = fvec2 * CIRCLE_VECTOR_LENGTH;

				spline.AddKnot(p1, p1 + p1vec, p1 - p1vec, tm);
				spline.AddKnot(p2, p2 - p2vec, p2 + p2vec, tm);
			}

			spline.closed = true;
			//spline->ComputeBezPoints();
		}

		CalcLength(10);
	}
}
