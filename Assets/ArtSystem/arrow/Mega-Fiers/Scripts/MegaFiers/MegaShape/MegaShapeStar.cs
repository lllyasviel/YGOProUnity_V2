
using UnityEngine;

[AddComponentMenu("MegaShapes/Star")]
public class MegaShapeStar : MegaShape
{
	const float CIRCLE_VECTOR_LENGTH = 0.5517861843f;

	const int		MIN_POINTS	= 3;
	const int		MAX_POINTS	= 100;
	const float		MIN_RADIUS	= 0.0f;
	const float		MAX_RADIUS	= float.MaxValue;
	const float		MIN_DIST	= -180.0f;
	const float		MAX_DIST	= 180.0f;
	const int		DEF_POINTS	= 6;
	const float		DEF_DIST	= 0.0f;
	const float		PI180		= 0.0174532f;

	public float	radius1		= 2.0f;
	public float	radius2		= 1.0f;
	public int		points		= DEF_POINTS;
	public float	distortion	= DEF_DIST;
	public float	fillet1		= 0.0f;
	public float	fillet2		= 0.0f;

	public override string GetHelpURL() { return "?page_id=396"; }

	public override void MakeShape()
	{
		Matrix4x4 tm = GetMatrix();

		Vector3 p = Vector3.zero;								// The actual point
		float angle;							// Angle of the current point

		radius1		= Mathf.Clamp(radius1, MIN_RADIUS, MAX_RADIUS);
		radius2		= Mathf.Clamp(radius2, MIN_RADIUS, MAX_RADIUS);
		distortion	= Mathf.Clamp(distortion, MIN_DIST, MAX_DIST);
		points		= Mathf.Clamp(points, MIN_POINTS, MAX_POINTS);
		fillet1		= Mathf.Clamp(fillet1, MIN_RADIUS, MAX_RADIUS);
		fillet2		= Mathf.Clamp(fillet2, MIN_RADIUS, MAX_RADIUS);

		// Delete the existing shape and create a new spline in it
		if ( splines.Count == 0 )
		{
			MegaSpline newspline = new MegaSpline();
			splines.Add(newspline);
		}

		MegaSpline spline = splines[0];

		spline.knots.Clear();

		float distort = PI180 * distortion;
		float PIpts = Mathf.PI / (float)points;

		// Now add all the necessary points
		for ( int ix = 0; ix < (2 * points); ++ix )
		{
			if ( (ix % 2) != 0 ) 	// Points for radius 1
			{
				angle = Mathf.PI * (float)ix / (float)points;
				p.x = Mathf.Cos(angle) * radius1;
				p.y = Mathf.Sin(angle) * radius1;
				p.z = 0.0f;

				if ( fillet1 > 0.0f )
				{
					float theta1 = angle - PIpts;
					float theta2 = angle + PIpts;
					float stheta1 = Mathf.Sin(theta1);
					float stheta2 = Mathf.Sin(theta2);
					float ctheta1 = Mathf.Cos(theta1);
					float ctheta2 = Mathf.Cos(theta2);
					Vector3 plast = new Vector3(radius2 * ctheta1, radius2 * stheta1, 0.0f);
					Vector3 pnext = new Vector3(radius2 * ctheta2, radius2 * stheta2, 0.0f);
					Vector3 n1 = Vector3.Normalize(plast - p) * fillet1;
					Vector3 n2 = Vector3.Normalize(pnext - p) * fillet1;
					Vector3 nk1 = n1 * CIRCLE_VECTOR_LENGTH;
					Vector3 nk2 = n2 * CIRCLE_VECTOR_LENGTH;
					Vector3 p1 = p + n1;
					Vector3 p2 = p + n2;

					spline.AddKnot(p1, p1 + nk1, p1 - nk1, tm);
					spline.AddKnot(p2, p2 - nk2, p2 + nk2, tm);
				}
				else
					spline.AddKnot(p, p, p, tm);
			}
			else  // Points for radius 2 with optional angular offset
			{
				angle = PIpts * (float)ix + distort;
				p.x = Mathf.Cos(angle) * radius2;
				p.y = Mathf.Sin(angle) * radius2;
				p.z = 0.0f;

				if ( fillet2 > 0.0f )
				{
					float theta1 = angle - PIpts - distort;
					float theta2 = angle + PIpts + distort;
					float stheta1 = Mathf.Sin(theta1);
					float stheta2 = Mathf.Sin(theta2);
					float ctheta1 = Mathf.Cos(theta1);
					float ctheta2 = Mathf.Cos(theta2);
					Vector3 plast = new Vector3(radius1 * ctheta1, radius1 * stheta1, 0.0f);
					Vector3 pnext = new Vector3(radius1 * ctheta2, radius1 * stheta2, 0.0f);
					Vector3 n1 = Vector3.Normalize(plast - p) * fillet2;
					Vector3 n2 = Vector3.Normalize(pnext - p) * fillet2;
					Vector3 nk1 = n1 * CIRCLE_VECTOR_LENGTH;
					Vector3 nk2 = n2 * CIRCLE_VECTOR_LENGTH;
					Vector3 p1 = p + n1;
					Vector3 p2 = p + n2;
					spline.AddKnot(p1, p1 + nk1, p1 - nk1, tm);
					spline.AddKnot(p2, p2 - nk2, p2 + nk2, tm);
				}
				else
					spline.AddKnot(p, p, p, tm);
			}
		}

		spline.closed = true;
		CalcLength(10);
		//stepdist = spline.length / 80.0f;
	}
}