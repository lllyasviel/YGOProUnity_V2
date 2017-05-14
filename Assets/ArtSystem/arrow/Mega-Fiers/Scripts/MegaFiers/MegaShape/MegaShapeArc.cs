
using UnityEngine;

[AddComponentMenu("MegaShapes/Arc")]
public class MegaShapeArc : MegaShape
{
	public float	from	= 0.0f;
	public float	to		= 270.0f;
	public float	radius	= 1.0f;
	public bool		pie		= false;

	public override void MakeShape()
	{
		Matrix4x4 tm = GetMatrix();
		//lastout = 0.0f;
		//lastin = -9999.0f;

		// Delete all points in the existing spline
		MegaSpline spline = NewSpline();
		Vector3 origin = Vector3.zero;
		
		float fromrad = from * Mathf.Deg2Rad;
		float torad = to * Mathf.Deg2Rad;

		// Order angles properly
		if ( fromrad > torad )
			torad += Mathf.PI * 2.0f;

		float totAngle = torad - fromrad;
		float vector = veccalc(totAngle / 3.0f) * radius;

		// Now add all the necessary points
		float angStep = totAngle / 3.0f;

		for ( int ix = 0; ix < 4; ++ix )
		{
			float angle = fromrad + (float)ix * angStep;
			float sinfac = Mathf.Sin(angle);
			float cosfac = Mathf.Cos(angle);
			Vector3 p = new Vector3(cosfac * radius, sinfac * radius, 0.0f);
			Vector3 rotvec = new Vector3(sinfac * vector, -cosfac * vector, 0.0f);
			Vector3 invec = (ix == 0) ? p : p + rotvec;
			Vector3 outvec = (ix == 3) ? p : p - rotvec;
			spline.AddKnot(p, invec, outvec, tm);
		}
		
		if ( pie )
		{
			spline.AddKnot(origin, origin, origin);
			spline.closed = true;
		}

		CalcLength(10);
		//if ( reverse )
			//spline->Reverse(TRUE);
	}
}