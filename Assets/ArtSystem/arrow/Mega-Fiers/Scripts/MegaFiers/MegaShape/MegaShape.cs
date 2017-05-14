
using UnityEngine;
using System.IO;
using System.Collections.Generic;

// option for spline profile for border?
// TODO: Add option for border strip, so get edge, duplicate points, move originals in by amount and normal of tangent
// if we do meshes with edge loops then easy to do borders, bevels, extrudes
// TODO: Split code to shape, spline, knot, and same for edit code
// TODO: Each spline in a shape should have its own transform
// TODO: split knot and spline out to files
// Need to draw and edit multiple splines, and work on them, then mesh needs to work on those indi
[System.Serializable]
public class MegaKnotAnim
{
	public int	p;	// point index
	public int	t;	// handle or val
	public int	s;	// spline

	public MegaBezVector3KeyControl	con;
}

[System.Serializable]
public class MegaKnot
{
	public Vector3	p;
	public Vector3	invec;
	public Vector3	outvec;
	public float	seglength;
	public float	length;
	public bool		notlocked;

	public MegaKnot()
	{
		p = new Vector3();
		invec = new Vector3();
		outvec = new Vector3();
		length = 0.0f;
		seglength = 0.0f;
	}

	public Vector3 Interpolate(float t, MegaKnot k)
	{
		float omt = 1.0f - t;

		float omt2 = omt * omt;
		float omt3 = omt2 * omt;

		float t2 = t * t;
		float t3 = t2 * t;

		omt2 = 3.0f * omt2 * t;
		omt = 3.0f * omt * t2;

		Vector3 tp = Vector3.zero;

		tp.x = (omt3 * p.x) + (omt2 * outvec.x) + (omt * k.invec.x) + (t3 * k.p.x);
		tp.y = (omt3 * p.y) + (omt2 * outvec.y) + (omt * k.invec.y) + (t3 * k.p.y);
		tp.z = (omt3 * p.z) + (omt2 * outvec.z) + (omt * k.invec.z) + (t3 * k.p.z);

		return tp;
	}
}

[System.Serializable]
public class MegaSpline
{
	public float				length;
	public bool					closed;
	public List<MegaKnot>		knots = new List<MegaKnot>();
	public List<MegaKnotAnim>	animations;
	public Vector3				offset = Vector3.zero;
	public Vector3				rotate = Vector3.zero;
	public Vector3				scale = Vector3.one;

	public void AddKnot(Vector3 p, Vector3 invec, Vector3 outvec)
	{
		MegaKnot knot = new MegaKnot();
		knot.p = p;
		knot.invec = invec;
		knot.outvec = outvec;
		knots.Add(knot);
	}

	public void AddKnot(Vector3 p, Vector3 invec, Vector3 outvec, Matrix4x4 tm)
	{
		MegaKnot knot = new MegaKnot();
		knot.p = tm.MultiplyPoint3x4(p);
		knot.invec = tm.MultiplyPoint3x4(invec);
		knot.outvec = tm.MultiplyPoint3x4(outvec);
		knots.Add(knot);
	}

	// Assumes minor axis to be y
	public bool Contains(Vector3 p)
	{
		if ( !closed )
			return false;

		int		j = knots.Count - 1;
		bool	oddNodes = false;

		for ( int i = 0; i < knots.Count; i++ )
		{
			if ( knots[i].p.z < p.z && knots[j].p.z >= p.z || knots[j].p.z < p.z && knots[i].p.z >= p.z )
			{
				if ( knots[i].p.x + (p.z - knots[i].p.z) / (knots[j].p.z - knots[i].p.z) * (knots[j].p.x - knots[i].p.x) < p.x )
					oddNodes = !oddNodes;
			}

			j = i;
		}

		return oddNodes;
	}

	// Assumes minor axis to be y
	public float Area()
	{
		float area = 0.0f;

		if ( closed )
		{
			for ( int i = 0; i < knots.Count; i++ )
			{
				int i1 = (i + 1) % knots.Count;
				area += (knots[i].p.z + knots[i1].p.z) * (knots[i1].p.x - knots[i].p.x);
			}
		}

		return area * 0.5f;
	}

	// Should actually go through segments, what about scale?
	public float CalcLength(int steps)
	{
		length = 0.0f;

		int kend = knots.Count - 1;

		if ( closed )
			kend++;

		for ( int knot = 0; knot < kend; knot++ )
		{
			int k1 = (knot + 1) % knots.Count;

			Vector3 p1 = knots[knot].p;
			float step = 1.0f / (float)steps;
			float pos = step;

			knots[knot].seglength = 0.0f;

			for ( int i = 1; i < steps; i++ )
			{
				Vector3 p2 = knots[knot].Interpolate(pos, knots[k1]);

				knots[knot].seglength += Vector3.Magnitude(p2 - p1);
				p1 = p2;
				pos += step;
			}

			knots[knot].seglength += Vector3.Magnitude(knots[k1].p - p1);

			length += knots[knot].seglength;

			knots[knot].length = length;
			length = knots[knot].length;
		}

		return length;
	}

	/*  So this should work for curves or splines, no sep code for curve, derive from common base */
	/*  Could save a hint for next time through, ie spline and seg */
	public Vector3 Interpolate(float alpha, bool type, ref int k)
	{
		int	seg = 0;

		// Special case if closed
		if ( closed )
		{
			if ( type )
			{
				float dist = alpha * length;

				if ( dist > knots[knots.Count - 1].length )
				{
					k = knots.Count - 1;
					alpha = 1.0f - ((length - dist) / knots[knots.Count - 1].seglength);
					return knots[knots.Count - 1].Interpolate(alpha, knots[0]);
				}
				else
				{
					for ( seg = 0; seg < knots.Count; seg++ )
					{
						if ( dist <= knots[seg].length )
							break;
					}
				}
				alpha = 1.0f - ((knots[seg].length - dist) / knots[seg].seglength);
			}
			else
			{
				float segf = alpha * knots.Count;

				seg = (int)segf;

				if ( seg == knots.Count )
				{
					seg--;
					alpha = 1.0f;
				}
				else
					alpha = segf - seg;
			}

			if ( seg < knots.Count - 1 )
			{
				k = seg;

				return knots[seg].Interpolate(alpha, knots[seg + 1]);
			}
			else
			{
				k = seg;

				return knots[seg].Interpolate(alpha, knots[0]);
			}

			//return knots[0].p;
		}
		else
		{
			if ( type )
			{
				float dist = alpha * length;

				for ( seg = 0; seg < knots.Count; seg++ )
				{
					if ( dist <= knots[seg].length )
						break;
				}

				alpha = 1.0f - ((knots[seg].length - dist) / knots[seg].seglength);
			}
			else
			{
				float segf = alpha * knots.Count;

				seg = (int)segf;

				if ( seg == knots.Count )
				{
					seg--;
					alpha = 1.0f;
				}
				else
					alpha = segf - seg;
			}

			// Should check alpha
			if ( seg < knots.Count - 1 )
			{
				k = seg;
				return knots[seg].Interpolate(alpha, knots[seg + 1]);
			}
			else
			{
				k = seg;	//knots.Length - 1;
				return knots[seg].p;
			}
		}
	}
}

public enum MeshShapeType
{
	Fill,	// options, height, doublesided
	Tube,	// sides, cap ends, start, end, step
	Line,	// height, rotate, offset, segments for height
	Lathe,	// segs
}

[ExecuteInEditMode]
public class MegaShape : MonoBehaviour
{
	public MegaAxis	axis				= MegaAxis.Y;
	public Color	col1				= new Color(1.0f, 1.0f, 1.0f, 1.0f);
	public Color	col2				= new Color(0.1f, 0.1f, 0.1f, 1.0f);
	public Color	KnotCol				= new Color(0.0f, 1.0f, 0.0f, 1.0f);
	public Color	HandleCol			= new Color(1.0f, 0.0f, 0.0f, 1.0f);
	public Color	VecCol				= new Color(0.1f, 0.1f, 0.2f, 0.5f);
	public float	KnotSize			= 10.0f;
	public float	stepdist			= 1.0f;	// Distance along whole shape
	public bool		normalizedInterp	= true;
	public bool		drawHandles			= true;
	public bool		drawKnots			= true;
	public bool		drawspline			= true;
	public bool		lockhandles			= true;

	public float	CursorPos = 0.0f;
	public List<MegaSpline>	splines = new List<MegaSpline>();

	public virtual void MakeShape() { }

	//public List<MegaKnotAnim>	animations;
	public float	time = 0.0f;
	public bool		animate;
	public float	speed = 1.0f;

	//public int		CurrentCurve = 0;

	public virtual string GetHelpURL() { return "?page_id=390"; }

	[ContextMenu("Help")]
	public void Help()
	{
		Application.OpenURL("http://www.west-racing.com/mf/" + GetHelpURL());
	}

	[ContextMenu("Reset Mesh Info")]
	public void ResetMesh()
	{
		shapemesh = null;
		BuildMesh();
	}

	public Matrix4x4 GetMatrix()
	{
		Matrix4x4 tm = Matrix4x4.identity;

		switch ( axis )
		{
			case MegaAxis.X: MegaMatrix.RotateY(ref tm, Mathf.PI * 0.5f); break;
			case MegaAxis.Y: MegaMatrix.RotateX(ref tm, Mathf.PI * 0.5f); break;
			case MegaAxis.Z: break;	//Matrix.RotateY(ref tm, Mathf.PI * 0.5f); break;
		}

		return tm;
	}

	// Should be in MegaShape
	public MegaSpline NewSpline()
	{
		if ( splines.Count == 0 )
		{
			MegaSpline newspline = new MegaSpline();
			splines.Add(newspline);
		}

		MegaSpline spline = splines[0];

		spline.knots.Clear();
		spline.closed = false;
		return spline;
	}

	void Reset()
	{
		MakeShape();
	}

	void Awake()
	{
		if ( splines.Count == 0 )
		{
			MakeShape();
		}
	}

	float t = 0.0f;
	public float MaxTime = 1.0f;
	public MegaRepeatMode	LoopMode;

    public void up()
    {
        Update();
    }

	void Update()
	{
		if ( animate )
		{
			time += Time.deltaTime * speed;

			switch ( LoopMode )
			{
				case MegaRepeatMode.Loop:		t = Mathf.Repeat(time, MaxTime); break;
				case MegaRepeatMode.PingPong:	t = Mathf.PingPong(time, MaxTime); break;
				case MegaRepeatMode.Clamp:		t = Mathf.Clamp(time, 0.0f, MaxTime); break;
			}

			for ( int s = 0; s < splines.Count; s++ )
			{
				if ( splines[s].animations != null && splines[s].animations.Count > 0 )
				{
					for ( int i = 0; i < splines[s].animations.Count; i++ )
					{
						Vector3 pos = splines[s].animations[i].con.GetVector3(t);

						switch ( splines[s].animations[i].t )
						{
							case 0:	splines[splines[s].animations[i].s].knots[splines[s].animations[i].p].invec		= pos;	break;
							case 1:	splines[splines[s].animations[i].s].knots[splines[s].animations[i].p].p			= pos;	break;
							case 2:	splines[splines[s].animations[i].s].knots[splines[s].animations[i].p].outvec	= pos;	break;
						}
					}

					splines[s].CalcLength(10);	// could use less here
				}
			}
		}

		// Options here:
		// Uv scale, offset, rotate, physuv, genuv
		// Optimize
		// recalcnorms
		// tangents
		// fill in shape
		// pipe along shape
		// wall along shape
		// double sided
		// extrude on fill
		//BuildMesh();
#if false
		if ( makeMesh )
		{
			//makeMesh = false;
			List<Vector3>	verts = new List<Vector3>();
			List<Vector2>	uvs = new List<Vector2>();

			float sdist = stepdist * 0.1f;
			if ( splines[0].length / sdist > 1500.0f )
				sdist = splines[0].length / 1500.0f;

			int[] tris = MegaTriangulator.Triangulate(this, splines[0], sdist, ref verts, ref uvs);

			if ( shapemesh == null )
			{
				MeshFilter mf = gameObject.GetComponent<MeshFilter>();

				if ( mf == null )
					mf = gameObject.AddComponent<MeshFilter>();

				mf.sharedMesh = new Mesh();
				MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
				if ( mr == null )
				{
					mr = gameObject.AddComponent<MeshRenderer>();
				}

				Material[] mats = new Material[1];

				mr.sharedMaterials = mats;

				shapemesh = mf.sharedMesh;	//Utils.GetMesh(gameObject);
			}

			//Vector3[] verts = new Vector3[splines[0].knots.Count];
			//for ( int i = 0; i < splines[0].knots.Count; i++ )
			//{
			//	verts[i] = splines[0].knots[i].p;
			//}

			shapemesh.Clear();
			shapemesh.vertices = verts.ToArray();
			shapemesh.uv = uvs.ToArray();

			shapemesh.triangles = tris;
			shapemesh.RecalculateNormals();
			shapemesh.RecalculateBounds();
		}
#endif
	}

	// Meshing options
	public bool				makeMesh = false;
	public MeshShapeType	meshType = MeshShapeType.Fill;
	public bool				DoubleSided = true;
	public bool				CalcTangents = false;
	public bool				GenUV = true;
	public bool				PhysUV = false;
	public float			Height = 0.0f;
	public int				HeightSegs = 1;

	public int				Sides = 4;
	public float			TubeStep = 0.1f;
	public float			Start = 0.0f;
	public float			End = 100.0f;
	public float			Rotate = 0.0f;
	public Vector3			Pivot = Vector3.zero;

	// Material 1
	public Vector2			UVOffset = Vector2.zero;
	public Vector2			UVRotate = Vector2.zero;
	public Vector2			UVScale = Vector2.one;

	public Vector2			UVOffset1 = Vector2.zero;
	public Vector2			UVRotate1 = Vector2.zero;
	public Vector2			UVScale1 = Vector2.one;

	public Vector2			UVOffset2 = Vector2.zero;
	public Vector2			UVRotate3 = Vector2.zero;
	public Vector2			UVScale3 = Vector2.one;

	// TODO:
	// Pivot
	// UV info per submesh
	// work on multiple splines
	// new poly to tri to handle holes
	// merge to one mesh
	// height segs


	public bool				UseHeightCurve = false;
	public AnimationCurve	heightCrv = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
	// May need wall UV info as well


	public Mesh shapemesh;

	// Need a scale method?
	public void Scale(float scale)
	{
		for ( int i = 0; i < splines.Count; i++ )
		{
			for ( int k = 0; k < splines[i].knots.Count; k++ )
			{
				splines[i].knots[k].invec *= scale;
				splines[i].knots[k].p *= scale;
				splines[i].knots[k].outvec *= scale;
			}

			if ( splines[i].animations != null )
			{
				for ( int a = 0; a < splines[i].animations.Count; a++ )
				{
					if ( splines[i].animations[a].con != null )
						splines[i].animations[a].con.Scale(scale);
				}
			}
		}

		CalcLength(10);
	}

	public int GetSpline(int p, ref MegaKnotAnim ma)	//int spl, ref int sp, ref int pt)
	{
		int index = 0;
		int pn = p / 3;
		for ( int i = 0; i < splines.Count; i++ )
		{
			int nx = index + splines[i].knots.Count;

			if ( pn < nx )
			{
				ma.s = i;
				ma.p = pn - index;
				ma.t = p % 3;
				return i;
			}

			index = nx;
		}

		Debug.Log("Cant find point in spline");
		return 0;
	}


	public float GetCurveLength(int curve)
	{
		if ( curve < splines.Count )
			return splines[curve].length;

		return splines[0].length;
	}

	public float CalcLength(int curve, int step)
	{
		if ( curve < splines.Count )
			return splines[curve].CalcLength(step);

		return 0.0f;
	}

	[ContextMenu("Recalc Length")]
	public void ReCalcLength()
	{
		CalcLength(10);
	}

	public float CalcLength(int step)
	{
		float length = 0.0f;
		for ( int i = 0; i < splines.Count; i++ )
			length += CalcLength(i, step);

		return length;
	}

	public Vector3 GetKnotPos(int curve, int knot)
	{
		return splines[curve].knots[knot].p;
	}

	public Vector3 GetKnotInVec(int curve, int knot)
	{
		return splines[curve].knots[knot].invec;
	}

	public Vector3 GetKnotOutVec(int curve, int knot)
	{
		return splines[curve].knots[knot].outvec;
	}

	public void SetKnotPos(int curve, int knot, Vector3 p)
	{
		splines[curve].knots[knot].p = p;
		CalcLength(10);
	}

	public void SetKnot(int curve, int knot, Vector3 p, Vector3 intan, Vector3 outtan)
	{
		splines[curve].knots[knot].p = p;
		splines[curve].knots[knot].invec = p;
		splines[curve].knots[knot].outvec = p;
		CalcLength(10);
	}

	public void SetKnotEx(int curve, int knot, Vector3 p, Vector3 intan, Vector3 outtan)
	{
		splines[curve].knots[knot].p = p;
		splines[curve].knots[knot].invec = intan;
		splines[curve].knots[knot].outvec = outtan;
		CalcLength(10);
	}


	public Vector3 InterpCurve3D(int curve, float alpha, bool type)
	{
		Vector3	ret;
		int k = 0;

		if ( curve < splines.Count )
		{
			if ( alpha < 0.0f )
			{
				if ( splines[curve].closed )
					alpha = Mathf.Repeat(alpha, 1.0f);
				else
				{
					Vector3 ps = splines[curve].Interpolate(0.0f, type, ref k);

					// Need a proper tangent function
					Vector3 ps1 = splines[curve].Interpolate(0.01f, type, ref k);

					// Calc the spline in out vecs
					Vector3	delta = ps1 - ps;
					delta.Normalize();
					return ps + ((splines[curve].length * alpha) * delta);
				}
			}
			else
			{
				if ( alpha > 1.0f )
				{
					if ( splines[curve].closed )
						alpha = alpha % 1.0f;
					else
					{
						Vector3 ps = splines[curve].Interpolate(1.0f, type, ref k);

						// Need a proper tangent function
						Vector3 ps1 = splines[curve].Interpolate(0.99f, type, ref k);

						// Calc the spline in out vecs
						Vector3	delta = ps1 - ps;
						delta.Normalize();
						return ps + ((splines[curve].length * (1.0f - alpha)) * delta);
					}
				}
			}

			ret = splines[curve].Interpolate(alpha, type, ref k);
		}
		else
			ret = splines[0].Interpolate(1.0f, type, ref k);

		return ret;
	}

	static float lastout = 0.0f;
	static float lastin = -9999.0f;

	static public float veccalc(float angstep)
	{
		if ( lastin == angstep )
			return lastout;

		float totdist;
		float sinfac = Mathf.Sin(angstep);
		float cosfac = Mathf.Cos(angstep);
		float test;
		int ix;
		MegaSpline work = new MegaSpline();
		Vector3 k1 = new Vector3(Mathf.Cos(0.0f), Mathf.Sin(0.0f), 0.0f);
		Vector3 k2 = new Vector3(cosfac, sinfac, 0.0f);

		float hi = 1.5f;
		float lo = 0.0f;
		int count = 200;

		// Loop thru test vectors
	loop:
		work.knots.Clear();
		test = (hi + lo) / 2.0f;
		Vector3 outv = k1 + new Vector3(0.0f, test, 0.0f);
		Vector3 inv = k2 + new Vector3(sinfac * test, -cosfac * test, 0.0f);

		work.AddKnot(k1, k1, outv);
		work.AddKnot(k2, inv, k2);

		totdist = 0.0f;
		int k = 0;
		//totdist = work.CalcLength(10);
		for ( ix = 0; ix < 10; ++ix )
		{
			Vector3 terp = work.Interpolate((float)ix / 10.0f, false, ref k);
			totdist += Mathf.Sqrt(terp.x * terp.x + terp.y * terp.y);
		}

		totdist /= 10.0f;
		count--;

		if ( totdist == 1.0f || count <= 0 )
			goto done;

		if ( totdist > 1.0f )
		{
			hi = test;
			goto loop;
		}
		lo = test;
		goto loop;

	done:
		lastin = angstep;
		lastout = test;
		return test;
	}

	// Find nearest point
	public Vector3 FindNearestPoint(Vector3 p, int iterations, ref int kn, ref Vector3 tangent)
	{
		//Vector3 np = Vector3.zero;

		float positiveInfinity = float.PositiveInfinity;
		float num2 = 0.0f;
		iterations = Mathf.Clamp(iterations, 0, 5);
		int kt = 0;

		for ( float i = 0.0f; i <= 1.0f; i += 0.01f )
		{
			//Vector3 vector = this.GetPositionOnSpline(i) - p;
			//Vector3 vector = InterpCurve3D(0, i, true) - p;	//this.GetPositionOnSpline(i) - p;
			Vector3 vector = splines[0].Interpolate(i, true, ref kt) - p;	//this.GetPositionOnSpline(i) - p;
			float sqrMagnitude = vector.sqrMagnitude;
			if ( positiveInfinity > sqrMagnitude )
			{
				positiveInfinity = sqrMagnitude;
				num2 = i;
			}
		}

		for ( int j = 0; j < iterations; j++ )
		{
			float num6 = 0.01f * Mathf.Pow(10.0f, -((float)j));
			float num7 = num6 * 0.1f;
			for ( float k = Mathf.Clamp01(num2 - num6); k <= Mathf.Clamp01(num2 + num6); k += num7 )
			{
				//Vector3 vector2 = InterpCurve3D(0, k, true) - p;	//this.GetPositionOnSpline(k) - p;
				Vector3 vector2 = splines[0].Interpolate(k, true, ref kt) - p;	//this.GetPositionOnSpline(k) - p;
				float num9 = vector2.sqrMagnitude;

				if ( positiveInfinity > num9 )
				{
					positiveInfinity = num9;
					num2 = k;
				}
			}
		}

		kn = kt;
		tangent = InterpCurve3D(0, num2 + 0.01f, true);
		return InterpCurve3D(0, num2, true);	//num2;
		//return np;
	}

	List<Vector3>	verts = new List<Vector3>();
	List<Vector2>	uvs = new List<Vector2>();
	List<int>		tris = new List<int>();
	List<int>		tris1 = new List<int>();
	List<int>		tris2 = new List<int>();

	// Best if we calc the normals to avoid issues at join
	public void BuildMesh()
	{
		if ( makeMesh )
		{
			//makeMesh = false;

			float sdist = stepdist * 0.1f;
			if ( splines[0].length / sdist > 1500.0f )
				sdist = splines[0].length / 1500.0f;

			verts.Clear();
			uvs.Clear();
			tris.Clear();
			tris1.Clear();
			tris2.Clear();
			tris = MegaTriangulator.Triangulate(this, splines[0], sdist, ref verts, ref uvs, ref tris, Pivot);

			if ( shapemesh == null )
			{
				MeshFilter mf = gameObject.GetComponent<MeshFilter>();

				if ( mf == null )
					mf = gameObject.AddComponent<MeshFilter>();

				mf.sharedMesh = new Mesh();
				MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
				if ( mr == null )
				{
					mr = gameObject.AddComponent<MeshRenderer>();
				}

				Material[] mats = new Material[3];

				mr.sharedMaterials = mats;

				shapemesh = mf.sharedMesh;	//Utils.GetMesh(gameObject);
			}

			int vcount = verts.Count;
			int tcount = tris.Count;

			float h = Mathf.Abs(Height);

			if ( GenUV )
			{
				uvs.Clear();	// need to stop triangulator doing uvs
				Vector2 uv = Vector2.zero;
				for ( int i = 0; i < verts.Count; i++ )
				{
					uv.x = (verts[i].x * UVScale.x) + UVOffset.x;	// * UVScale.x;
					uv.y = (verts[i].z * UVScale.y) + UVOffset.y;	// * UVScale.y;
					uvs.Add(uv);
				}
			}

			if ( DoubleSided && h != 0.0f )
			{
				//vcount = verts.Count;
				for ( int i = 0; i < vcount; i++ )
				{
					Vector3 p = verts[i];

					if ( UseHeightCurve )
					{
						float alpha = MegaTriangulator.m_points[i].z / splines[0].length;
						p.y -= h * heightCrv.Evaluate(alpha);
					}
					else
						p.y -= h;

					verts.Add(p);	//verts[i]);
					uvs.Add(uvs[i]);
				}

				//tcount = tris.Count;
				for ( int i = tcount - 1; i >= 0; i-- )
				{
					tris1.Add(tris[i] + vcount);
				}
			}
#if true
			// Do edge
			if ( h != 0.0f )
			{
				int vc = verts.Count;

				Vector3 ep = Vector3.zero;
				Vector3 euv = Vector2.zero;
				// Top loop
				for ( int i = 0; i < MegaTriangulator.m_points.Count; i++ )
				{
					ep = verts[i];
					//ep.x = MegaTriangulator.m_points[i].x;
					//ep.y = 0.0f;
					//ep.z = MegaTriangulator.m_points[i].y;
					verts.Add(ep);

					//euv.x = (MegaTriangulator.m_points[i].z / splines[0].length) * 4.0f;
					euv.x = (MegaTriangulator.m_points[i].z * UVScale1.x) + UVOffset1.x;	// / splines[0].length) * 4.0f;
					euv.y = UVOffset1.y;	//0.0f;
					uvs.Add(euv);
				}
				// Add first point again
				ep = verts[0];
				//ep.y -= h * heightCrv.Evaluate(0.0f);
				verts.Add(ep);

				//euv.x = 1.0f * 4.0f;	//MegaTriangulator.m_points[0].z / splines[0].length;
				euv.x = (splines[0].length * UVScale1.x) + UVOffset1.x;	//1.0f * 4.0f;	//MegaTriangulator.m_points[0].z / splines[0].length;
				euv.y = 0.0f + UVOffset1.y;
				uvs.Add(euv);

				// Bot loop
				float hd = 1.0f;

				for ( int i = 0; i < MegaTriangulator.m_points.Count; i++ )
				{
					float alpha = MegaTriangulator.m_points[i].z / splines[0].length;

					ep = verts[i];
					if ( UseHeightCurve )
						hd = heightCrv.Evaluate(alpha);

					ep.y -=  h * hd;	//heightCrv.Evaluate(alpha);

					verts.Add(ep);

					//euv.x = (MegaTriangulator.m_points[i].z / splines[0].length) * 4.0f;
					euv.x = (MegaTriangulator.m_points[i].z * UVScale1.x) + UVOffset1.x;
					euv.y = ((h * hd) * UVScale1.y) + UVOffset1.y;	//1.0f;
					uvs.Add(euv);
				}
				// Add first point again
				ep = verts[0];

				if ( UseHeightCurve )
				{
					hd = heightCrv.Evaluate(0.0f);
				}

				ep.y -= h * hd;	//heightCrv.Evaluate(0.0f);
				verts.Add(ep);

				//euv.x = 1.0f;	//MegaTriangulator.m_points[0].z / splines[0].length;
				euv.x = (splines[0].length * UVScale1.x) + UVOffset1.x;	//MegaTriangulator.m_points[0].z / splines[0].length;
				euv.y = (h * hd * UVScale1.y) + UVOffset1.y;	//1.0f;
				uvs.Add(euv);

				// Faces
				int ecount = MegaTriangulator.m_points.Count + 1;

				int ip = 0;
				for ( ip = 0; ip < MegaTriangulator.m_points.Count; ip++ )
				{
					tris2.Add(ip + vc);
					tris2.Add(ip + vc + ecount);
					tris2.Add(ip + vc + 1);

					tris2.Add(ip + vc + 1);
					tris2.Add(ip + vc + ecount);
					tris2.Add(ip + vc + ecount + 1);
				}
#if false
				tris.Add(ip + vc);
				tris.Add(ip + vc + ecount);
				tris.Add(vc);

				tris.Add(vc);
				tris.Add(ip + vc + ecount);
				tris.Add(vc + ecount);
#endif
			}
#endif
			shapemesh.Clear();

			shapemesh.vertices = verts.ToArray();
			shapemesh.uv = uvs.ToArray();

			shapemesh.subMeshCount = 3;
			shapemesh.SetTriangles(tris.ToArray(), 0);
			shapemesh.SetTriangles(tris1.ToArray(), 1);
			shapemesh.SetTriangles(tris2.ToArray(), 2);

			//shapemesh.triangles = tris.ToArray();
			shapemesh.RecalculateNormals();
			shapemesh.RecalculateBounds();
		}
	}
}

// Need to find major axis and flatten to 2d, then revert back to 3d
#if !UNITY_FLASH
public class MegaTriangulator
{
	static public List<Vector3> m_points = new List<Vector3>();

	//public MegaTriangulator(MegaKnot[] points)
	//{
	//	m_points = new List<Vector2>();	//points);
	//
	//}
   
	static public List<int> Triangulate(MegaShape shape, MegaSpline spline, float dist, ref List<Vector3> verts, ref List<Vector2> uvs, ref List<int> indices, Vector3 pivot)
	{
		// Find 
		m_points.Clear();

		List<MegaKnot> knots = spline.knots;

		Vector3 min = knots[0].p;
		Vector3 max = knots[0].p;

		for ( int i = 1; i < knots.Count; i++ )
		{
			Vector3 p1 = knots[i].p;

			if ( p1.x < min.x )	min.x = p1.x;
			if ( p1.y < min.y ) min.y = p1.y;
			if ( p1.z < min.z ) min.z = p1.z;

			if ( p1.x > max.x ) max.x = p1.x;
			if ( p1.y > max.y ) max.y = p1.y;
			if ( p1.z > max.z ) max.z = p1.z;
		}

		Vector3 size = max - min;

		int removeaxis = 0;

		if ( Mathf.Abs(size.x) < Mathf.Abs(size.y) )
		{
			if ( Mathf.Abs(size.x) < Mathf.Abs(size.z) )
				removeaxis = 0;
			else
				removeaxis = 2;
		}
		else
		{
			if ( Mathf.Abs(size.y) < Mathf.Abs(size.z) )
				removeaxis = 1;
			else
				removeaxis = 2;
		}

		Vector3 tp = Vector3.zero;
#if false
		for ( int i = 0; i < knots.Count; i++ )
		{
			for ( int a = 0; a < steps; a ++ )
			{
				float alpha = (float)a / (float)steps;
				Vector3 p = spline.knots[i].Interpolate(alpha, spline.knots[i]);
				switch ( removeaxis )
				{
					case 0:	tp.x = p.y; tp.y = p.z;	break;
					case 1: tp.x = p.x; tp.y = p.z; break;
					case 2: tp.x = p.x; tp.y = p.y; break;
				}
				verts.Add(p);
				m_points.Add(tp);
			}
		}
#endif
		float ds = spline.length / (spline.length / dist);

		if ( ds > spline.length )
			ds = spline.length;

		//int c	= 0;
		int k	= -1;
		//int lk	= -1;

		//Vector3 first = spline.Interpolate(0.0f, shape.normalizedInterp, ref lk);
		Vector3 p = Vector3.zero;

		for ( float dst = 0.0f; dst < spline.length; dst += ds )
		{
			float alpha = dst / spline.length;
			p = spline.Interpolate(alpha, shape.normalizedInterp, ref k) + pivot;

			switch ( removeaxis )
			{
				case 0: tp.x = p.y; tp.y = p.z; break;
				case 1: tp.x = p.x; tp.y = p.z; break;
				case 2: tp.x = p.x; tp.y = p.y; break;
			}
			tp.z = dst;
			verts.Add(p);
			m_points.Add(tp);

			// Dont need this here as can do in post step
			//tp.x = (tp.x - min.x) / size.x;
			//tp.y = (tp.y - min.z) / size.z;
			tp.x = (tp.x - min.x);	// / size.x;
			tp.y = (tp.y - min.z);	// / size.z;
			uvs.Add(tp);
		}

		//if ( spline.closed )
		//	p = spline.Interpolate(0.0f, shape.normalizedInterp, ref k);
		//else
		//	p = spline.Interpolate(1.0f, shape.normalizedInterp, ref k);

		//switch ( removeaxis )
		//{
		//	case 0: tp.x = p.y; tp.y = p.z; break;
		//	case 1: tp.x = p.x; tp.y = p.z; break;
		//	case 2: tp.x = p.x; tp.y = p.y; break;
		//}

		//verts.Add(p);
		//m_points.Add(tp);

		return Triangulate(indices);
	}

	static public List<int> Triangulate(List<int> indices)
	{
		//List<int> indices = new List<int>();

		int n = m_points.Count;
		if ( n < 3 )
			return indices;	//.ToArray();

		int[] V = new int[n];
		if ( Area() > 0.0f )
		{
			for ( int v = 0; v < n; v++ )
				V[v] = v;
		}
		else
		{
			for ( int v = 0; v < n; v++ )
				V[v] = (n - 1) - v;
		}
       
		int nv = n;
		int count = 2 * nv;
		for ( int m = 0, v = nv - 1; nv > 2; )
		{
			if ( (count--) <= 0 )
				return indices;	//.ToArray();

			int u = v;
			if ( nv <= u )
				u = 0;
			v = u + 1;
			if ( nv <= v )
				v = 0;
			int w = v + 1;
			if ( nv <= w )
				w = 0;

			if ( Snip(u, v, w, nv, V) )
			{
				int a, b, c, s, t;
				a = V[u];
				b = V[v];
				c = V[w];
				indices.Add(c);
				indices.Add(b);
				indices.Add(a);
				m++;
				for ( s = v, t = v + 1; t < nv; s++, t++ )
					V[s] = V[t];
				nv--;
				count = 2 * nv;
			}
		}

		//indices.Reverse();
		return indices;	//.ToArray();
	}
   
	static private float Area()
	{
		int n = m_points.Count;
		float A = 0.0f;
		for ( int p = n - 1, q = 0; q < n; p = q++ )
		{
			Vector2 pval = m_points[p];
			Vector2 qval = m_points[q];
			A += pval.x * qval.y - qval.x * pval.y;
		}

		return A * 0.5f;
	}
   
	static private bool Snip(int u, int v, int w, int n, int[] V)
	{
		Vector2 A = m_points[V[u]];
		Vector2 B = m_points[V[v]];
		Vector2 C = m_points[V[w]];

		if ( Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))) )
			return false;

		for ( int p = 0; p < n; p++ )
		{
			if ( (p == u) || (p == v) || (p == w) )
				continue;
			Vector2 P = m_points[V[p]];

			if ( InsideTriangle(A, B, C, P) )
				return false;
		}
		return true;
	}
   
	static private bool InsideTriangle(Vector2 A, Vector2 B, Vector2 C, Vector2 P)
	{
		float ax = C.x - B.x;
		float ay = C.y - B.y;
		float bx = A.x - C.x;
		float by = A.y - C.y;
		float cx = B.x - A.x;
		float cy = B.y - A.y;
		float apx = P.x - A.x;
		float apy = P.y - A.y;
		float bpx = P.x - B.x;
		float bpy = P.y - B.y;
		float cpx = P.x - C.x;
		float cpy = P.y - C.y;

		float aCROSSbp = ax * bpy - ay * bpx;
		float cCROSSap = cx * apy - cy * apx;
		float bCROSScp = bx * cpy - by * cpx;

		return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
	}
}
#endif