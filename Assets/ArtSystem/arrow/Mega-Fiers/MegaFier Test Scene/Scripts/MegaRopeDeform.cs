
using UnityEngine;
using System.Collections.Generic;

// We only need do vertical movement for first draft
[AddComponentMenu("Modifiers/Rope Deform")]
public class MegaRopeDeform : MegaModifier
{
	public float	floorOff = 0.0f;	// floor offset
	public int		NumMasses = 8;		// masses

	public override string ModName()	{ return "RopeDeform"; }
	public override string GetHelpURL() { return "?page_id=1524"; }

	float minx;
	float width;

	public MegaSoft2D	soft = new MegaSoft2D();

	//public float			SpringRate		= 1.0f;
	//public float			Damping			= 1.0f;
	public float			timeStep		= 0.01f;
	public float			Mass			= 10.0f;	// Mass of system
	public MegaAxis			axis			= MegaAxis.Z;
	public AnimationCurve	stiffnessCrv	= new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 1));
	public float			stiffspring		= 1.0f;
	public float			stiffdamp		= 0.1f;
	public float			spring			= 1.0f;
	public float			damp			= 1.0f;

	public float off = 0.0f;

	public bool	init = false;

	int ax;

	// We could add a rotate just lerp a normal etc
	// y is minor axis
	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		// We could precalc this
		float alpha = (p[ax] - minx) / width;
		//int m = (int)((float)NumMasses * alpha);
		//float a = alpha - (float)m;

		// Cubic from rope for this
		Vector2 y = Interp1a(alpha);	//masses[m].p.y + ((masses[m + 1].p.y - masses[m].p.y) * a);
		p.y += y.y + (off * 0.01f);
		p[ax] = y.x;

		return invtm.MultiplyPoint3x4(p);
	}

	// Should use Map
	//public override void Modify(MegaModifiers mc)
	//{
	//	for ( int i = 0; i < verts.Length; i++ )
	//	{
	//		sverts[i] = verts[i];
	//	}
	//}

	public override bool ModLateUpdate(MegaModContext mc)
	{
		ax = (int)axis;
		minx = bbox.min[ax];
		width = bbox.max[ax] - bbox.min[ax];

		if ( init )
		{
			init = false;
			Init();
		}

		AddWeight();
		UpdateRope();
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		return true;
	}

	// Going to need length constraint
	// Assume X axis to be major for now
	public void Build(MegaModContext mc)
	{
	}

	// Do physics

	public void UpdateRope()
	{
		if ( soft != null )
		{
			//Debug.Log("Update Rope " + Time.deltaTime);
			soft.Update();

			for ( int i = 0; i < soft.masses.Count; i++ )
			{
				masspos[i + 1] = soft.masses[i].pos;

				soft.masses[i].forcec = Vector2.zero;
			}

			masspos[0] = soft.masses[0].pos - (soft.masses[1].pos - soft.masses[0].pos);
			masspos[masspos.Length - 1] = soft.masses[soft.masses.Count - 1].pos + (soft.masses[soft.masses.Count - 1].pos - soft.masses[soft.masses.Count - 2].pos);

			if ( left != null )	//&& pconl != null )
			{
				Vector3 p = transform.worldToLocalMatrix.MultiplyPoint(left.position);
				soft.constraints[pconl].pos.x = p[ax];
				soft.constraints[pconl].pos.y = p.y;
			}

			if ( right != null )	//&& pconr != null )
			{
				Vector3 p = transform.worldToLocalMatrix.MultiplyPoint(right.position);
				soft.constraints[pconr].pos.x = p[ax];
				soft.constraints[pconr].pos.y = p.y;
			}
		}
	}

	public float SpringCompress = 1.0f;
	public bool BendSprings = false;
	public bool Constraints = false;
	public float DampingRatio = 0.5f;

	public void Init()
	{
		if ( soft.masses == null )
			soft.masses = new List<Mass2D>();

		soft.masses.Clear();
		float ms = Mass / (float)(NumMasses);

		int ax = (int)axis;

		Vector2 pos = Vector2.zero;

		//DampingRatio = Mathf.Clamp01(DampingRatio);

		damp = (DampingRatio * 0.45f) * (2.0f * Mathf.Sqrt(ms * spring));

		for ( int i = 0; i < NumMasses; i++ )
		{
			float alpha = (float)i / (float)(NumMasses - 1);

			pos.x = Mathf.Lerp(bbox.min[ax], bbox.max[ax], alpha);
			//Debug.Log("m[" + i + "] alpha " + alpha + " " + pos.x);

			Mass2D rm = new Mass2D(ms, pos);
			soft.masses.Add(rm);
		}

		masspos = new Vector2[soft.masses.Count + 2];

		for ( int i = 0; i < soft.masses.Count; i++ )
			masspos[i + 1] = soft.masses[i].pos;

		if ( soft.springs == null )
			soft.springs = new List<Spring2D>();

		soft.springs.Clear();

		if ( soft.constraints == null )
			soft.constraints = new List<Constraint2D>();

		soft.constraints.Clear();

		for ( int i = 0; i < soft.masses.Count - 1; i++ )
		{
			Spring2D spr = new Spring2D(i, i + 1, spring, damp, soft);

			//float len = spr.restLen;
			spr.restLen *= SpringCompress;
			soft.springs.Add(spr);

			if ( Constraints )
			{
				// Do we use restLen or len here?
				Constraint2D lcon = Constraint2D.CreateLenCon(i, i + 1, spr.restLen);
				soft.constraints.Add(lcon);
			}
		}
#if true
		if ( BendSprings )
		{
			int gap = 2;
			for ( int i = 0; i < soft.masses.Count - gap; i++ )
			{
				float alpha = (float)i / (float)soft.masses.Count;
				Spring2D spr = new Spring2D(i, i + gap, stiffspring * stiffnessCrv.Evaluate(alpha), stiffdamp * stiffnessCrv.Evaluate(alpha), soft);
				soft.springs.Add(spr);

				Constraint2D lcon = Constraint2D.CreateLenCon(i, i + gap, spr.restLen);
				soft.constraints.Add(lcon);
			}
		}
#endif
		// Apply fixed end constraints
		Constraint2D pcon;

		//if ( left )
		//{
		//	pcon = Constraint2D.CreatePointTargetCon(0, left);
		//}
		//else
		{
			pos.x = bbox.min[ax];
			pos.y = 0.0f;
			pcon = Constraint2D.CreatePointCon(0, pos);
		}
		pconl = soft.constraints.Count;
		soft.constraints.Add(pcon);

		//if ( right )
		//{
		//	pcon = Constraint2D.CreatePointTargetCon(soft.masses.Count - 1, left);
		//}
		//else
		{
			pos.x = bbox.max[ax];
			pcon = Constraint2D.CreatePointCon(soft.masses.Count - 1, pos);
		}
		pconr = soft.constraints.Count;
		soft.constraints.Add(pcon);

		soft.DoConstraints();
	}

	public int pconl;
	public int pconr;


	public Vector2[]	masspos;

	// MegaSoft2D should have a display method to show springs and masses and constraints
	void DrawSpline(int steps)	//, float t)
	{
		if ( soft.masses != null && soft.masses.Count != 0 )
		{
			//Gizmos.color = Color.white;
			Vector3 prevPt = Interp1a(0.0f);

			if ( ax == 2 )
			{
				float x = prevPt.x;
				prevPt.x = prevPt.z;
				prevPt.z = x;
			}

			for ( int i = 1; i <= steps; i++ )
			{
				if ( (i & 1) == 1 )
					Gizmos.color = Color.white;
				else
					Gizmos.color = Color.black;

				float pm = (float)i / (float)steps;

				Vector3 currPt = Interp1a(pm);

				if ( ax == 2 )
				{
					float x = currPt.x;
					currPt.x = currPt.z;
					currPt.z = x;
				}
				Gizmos.DrawLine(prevPt, currPt);
				prevPt = currPt;
			}

			//Gizmos.color = Color.blue;
			//Vector3 pos = Interp(t);
			//Gizmos.DrawLine(pos, pos + Velocity(t));
		}
	}

	public void OnDrawGizmos()
	{
		Display();
	}

	public bool DisplayDebug = true;
	public int drawsteps = 20;
	public float boxsize = 0.01f;

	public Transform left;
	public Transform right;

	// Mmm should be in gizmo code
	void Display()
	{
		Gizmos.matrix = transform.localToWorldMatrix;

		if ( DisplayDebug && soft != null && soft.masses != null )
		{
			DrawSpline(drawsteps);	//, vel * 0.0f);

			Vector3 p = Vector3.zero;

			Gizmos.color = Color.yellow;

			for ( int i = 0; i < soft.masses.Count; i++ )
			{
				if ( ax == 0 )
				{
					p.x = soft.masses[i].pos.x;
					p.y = soft.masses[i].pos.y;	// + (off * 0.01f);
					p.z = 0.0f;
				}
				else
				{
					p.z = soft.masses[i].pos.x;
					p.y = soft.masses[i].pos.y;	// + (off * 0.01f);
					p.x = 0.0f;
				}
				Gizmos.DrawCube(p, Vector3.one * boxsize * 0.1f);
			}

			if ( weightPos >= 0.0f && weightPos < 100.0f )
			{
				Gizmos.color = Color.blue;
				Vector2 pos = Interp1a(weightPos * 0.01f);

				if ( ax == 0 )
				{
					p.x = pos.x;
					p.y = pos.y;	// + (off * 0.01f);
					p.z = 0.0f;
				}
				else
				{
					p.z = pos.x;
					p.y = pos.y;	// + (off * 0.01f);
					p.x = 0.0f;
				}

				Gizmos.DrawCube(p, Vector3.one * boxsize * 0.2f);
			}
		}
		Gizmos.matrix = Matrix4x4.identity;
	}

	// Spline interp etc
	public Vector2 Interp1(float t)
	{
		int numSections = soft.masses.Count - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
		float u = t * (float)numSections - (float)currPt;

		Vector2 a = soft.masses[currPt].pos;
		Vector2 b = soft.masses[currPt + 1].pos;
		Vector2 c = soft.masses[currPt + 2].pos;
		Vector2 d = soft.masses[currPt + 3].pos;

		return 0.5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
	}

	// Need to build coefs after sim then this becomes faster
	public Vector2 Interp1a(float t)
	{
		int numSections = masspos.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float)numSections), numSections - 1);
		float u = t * (float)numSections - (float)currPt;

		Vector2 a = masspos[currPt];
		Vector2 b = masspos[currPt + 1];
		Vector2 c = masspos[currPt + 2];
		Vector2 d = masspos[currPt + 3];

		return 0.5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
	}

	public float weight = 0.0f;
	public float weightPos = 0.5f;
	void AddWeight()
	{
		return;
#if false
		if ( weightPos >= 0.0f && weightPos < 100.0f )
		{
			float num = (float)(soft.masses.Count - 1);
			int m1 = (int)(num * weightPos * 0.01f);
			int m2 = m1 + 1;

			float alpha = ((float)num * weightPos * 0.01f) - (float)m1;

			Vector3 frc = Vector2.zero;

			frc.y = weight * (1.0f - alpha);
			soft.masses[m1].forcec = frc;
			frc.y = weight * alpha;
			soft.masses[m2].forcec = frc;
			
			//Debug.Log("m1 " + m1 + " alpha " + alpha);
		}
#endif
	}

	public float GetPos(float alpha)
	{
		Vector2 p = Interp1a(alpha);
		return p.y;
	}

	public Vector2 GetPos2(float alpha)
	{
		return Interp1a(alpha);
	}

	public Vector2 GetPos3(float v)
	{
		for ( int i = 1; i < masspos.Length - 1; i++ )
		{
			if ( v > masspos[i].x && v < masspos[i + 1].x )
			{
				float u = (v - masspos[i].x) / (masspos[i + 1].x - masspos[i].x);
				//Debug.Log("i " + i + " u " + u);
				Vector2 a = masspos[i - 1];
				Vector2 b = masspos[i];
				Vector2 c = masspos[i + 1];
				Vector2 d = masspos[i + 2];

				return 0.5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
			}
		}

		return Vector2.zero;
	}


	public Vector2 SetWeight(float v, float weight)
	{
		for ( int i = 1; i < masspos.Length - 2; i++ )
		{
			if ( v > masspos[i].x && v < masspos[i + 1].x )
			{
				float u = (v - masspos[i].x) / (masspos[i + 1].x - masspos[i].x);
				//Debug.Log("i " + i + " u " + u);
				Vector2 a = masspos[i - 1];
				Vector2 b = masspos[i];
				Vector2 c = masspos[i + 1];
				Vector2 d = masspos[i + 2];

				Vector2 frc = Vector2.zero;
				frc.y = weight * (1.0f - u);

				soft.masses[i - 1].forcec = frc;

				frc.y = weight * u;
				soft.masses[i].forcec = frc;

				return 0.5f * ((-a + 3.0f * b - 3.0f * c + d) * (u * u * u) + (2.0f * a - 5.0f * b + 4.0f * c - d) * (u * u) + (-a + c) * u + 2.0f * b);
			}
		}

		return Vector2.zero;
	}
}

static class AABB_Triangle_Intersection
{
	static void FINDMINMAX(float x0, float x1, float x2, out float min, out float max)
	{
		min = max = x0;

		if ( x1 < min ) min = x1;
		if ( x1 > max ) max = x1;
		if ( x2 < min ) min = x2;
		if ( x2 > max ) max = x2;
	}

	static bool planeBoxOverlap(Vector3 normal, Vector3 vert, Vector3 maxbox)
	{
		Vector3 vmin, vmax;

		float v = vert.x;
		if ( normal.x > 0.0f )
		{
			vmin.x = -maxbox.x - v;
			vmax.x = maxbox.x - v;
		}
		else
		{
			vmin.x = maxbox.x - v;
			vmax.x = -maxbox.x - v;
		}

		v = vert.y;
		if ( normal.y > 0.0f )
		{
			vmin.y = -maxbox.y - v;
			vmax.y = maxbox.y - v;
		}
		else
		{
			vmin.y = maxbox.y - v;
			vmax.y = -maxbox.y - v;
		}

		v = vert.z;
		if ( normal.z > 0.0f )
		{
			vmin.z = -maxbox.z - v;
			vmax.z = maxbox.z - v;
		}
		else
		{
			vmin.z = maxbox.z - v;
			vmax.z = -maxbox.z - v;
		}

		if ( Vector3.Dot(normal, vmin) > 0.0f ) return false;

		if ( Vector3.Dot(normal, vmax) >= 0.0f ) return true;

		return false;
	}

	public static bool TriangleBoxOverlap(Vector3 A, Vector3 B, Vector3 C, Bounds Box)
	{
		//return triBoxOverlap((Box.min + Box.max) / 2, (Box.max - Box.min) / 2, new Vector3[] { A, B, C });
		return triBoxOverlap(Box.center, Box.extents, new Vector3[] { A, B, C });
	}

	static bool triBoxOverlap(Vector3 boxcenter, Vector3 boxhalfsize, Vector3[] triverts)
	{
		float min, max, p0, p1, p2, rad;

		Vector3 v0 = triverts[0] - boxcenter;
		Vector3 v1 = triverts[1] - boxcenter;
		Vector3 v2 = triverts[2] - boxcenter;

		Vector3 e0 = v1 - v0;
		Vector3 e1 = v2 - v1;
		Vector3 e2 = v0 - v2;

		float fex = Mathf.Abs(e0.x);
		float fey = Mathf.Abs(e0.y);
		float fez = Mathf.Abs(e0.z);

		#region AXISTEST_X01(e0.z, e0.y, fez, fey);
		{
			p0 = e0.z * v0.y - e0.y * v0.z;

			p2 = e0.z * v2.y - e0.y * v2.z;

			if ( p0 < p2 ) { min = p0; max = p2; } else { min = p2; max = p0; }

			rad = fez * boxhalfsize.y + fey * boxhalfsize.z;

			if ( min > rad || max < -rad ) return false;
		}
		#endregion
		//Debug.Log("post axis");

		#region AXISTEST_Y02(e0.z, e0.x, fez, fex);
		{
			p0 = -e0.z * v0.x + e0.x * v0.z;

			p2 = -e0.z * v2.x + e0.x * v2.z;

			if ( p0 < p2 ) { min = p0; max = p2; } else { min = p2; max = p0; }

			rad = fez * boxhalfsize.x + fex * boxhalfsize.z;

			if ( min > rad || max < -rad ) return false;
		}
		#endregion
		#region AXISTEST_Z12(e0.y, e0.x, fey, fex);
		{
			p1 = e0.y * v1.x - e0.x * v1.y;

			p2 = e0.y * v2.x - e0.x * v2.y;

			if ( p2 < p1 ) { min = p2; max = p1; } else { min = p1; max = p2; }

			rad = fey * boxhalfsize.x + fex * boxhalfsize.y;

			if ( min > rad || max < -rad ) return false;
		}
		#endregion

		fex = Mathf.Abs(e1.x);
		fey = Mathf.Abs(e1.y);
		fez = Mathf.Abs(e1.z);

		#region AXISTEST_X01(e1.z, e1.y, fez, fey);
		{
			p0 = e1.z * v0.y - e1.y * v0.z;

			p2 = e1.z * v2.y - e1.y * v2.z;

			if ( p0 < p2 ) { min = p0; max = p2; } else { min = p2; max = p0; }

			rad = fez * boxhalfsize.y + fey * boxhalfsize.z;

			if ( min > rad || max < -rad ) return false;
		}
		#endregion
		#region AXISTEST_Y02(e1.z, e1.x, fez, fex);
		{
			p0 = -e1.z * v0.x + e1.x * v0.z;

			p2 = -e1.z * v2.x + e1.x * v2.z;

			if ( p0 < p2 ) { min = p0; max = p2; } else { min = p2; max = p0; }

			rad = fez * boxhalfsize.x + fex * boxhalfsize.z;

			if ( min > rad || max < -rad ) return false;
		}
		#endregion
		#region AXISTEST_Z0(e1.y, e1.x, fey, fex)
		{
			p0 = e1.y * v0.x - e1.x * v0.y;

			p1 = e1.y * v1.x - e1.x * v1.y;

			if ( p0 < p1 ) { min = p0; max = p1; } else { min = p1; max = p0; }

			rad = fey * boxhalfsize.x + fex * boxhalfsize.y;

			if ( min > rad || max < -rad ) return false;
		}
		#endregion

		fex = Mathf.Abs(e2.x);
		fey = Mathf.Abs(e2.y);
		fez = Mathf.Abs(e2.z);

		#region AXISTEST_X2(e2.z, e2.y, fez, fey);
		{
			p0 = e2.z * v0.y - e2.y * v0.z;

			p1 = e2.z * v1.y - e2.y * v1.z;

			if ( p0 < p1 ) { min = p0; max = p1; } else { min = p1; max = p0; }

			rad = fez * boxhalfsize.y + fey * boxhalfsize.z;

			if ( min > rad || max < -rad ) return false;
		}
		#endregion
		#region AXISTEST_Y1(e2.z, e2.x, fez, fex);
		{
			p0 = -e2.z * v0.x + e2.x * v0.z;

			p1 = -e2.z * v1.x + e2.x * v1.z;

			if ( p0 < p1 ) { min = p0; max = p1; } else { min = p1; max = p0; }

			rad = fez * boxhalfsize.z + fex * boxhalfsize.z;

			if ( min > rad || max < -rad ) return false;
		}
		#endregion
		#region AXISTEST_Z12(e2.y, e2.x, fey, fex);
		{
			p1 = e2.y * v1.x - e2.x * v1.y;

			p2 = e2.y * v2.x - e2.x * v2.y;

			if ( p2 < p1 ) { min = p2; max = p1; } else { min = p1; max = p2; }

			rad = fey * boxhalfsize.x + fex * boxhalfsize.y;

			if ( min > rad || max < -rad ) return false;
		}
		#endregion

		FINDMINMAX(v0.x, v1.x, v2.x, out min, out max);
		if ( min > boxhalfsize.x || max < -boxhalfsize.x ) return false;

		FINDMINMAX(v0.y, v1.y, v2.y, out min, out max);
		if ( min > boxhalfsize.y || max < -boxhalfsize.y ) return false;

		FINDMINMAX(v0.z, v1.z, v2.z, out min, out max);
		if ( min > boxhalfsize.z || max < -boxhalfsize.z ) return false;

		Vector3 normal = Vector3.Cross(e0, e1);
		if ( !planeBoxOverlap(normal, v0, boxhalfsize) ) return false;  // -NJMP-

		return true;
	}
}

#if false
public class Voxelize
{
	void GetTriBounds(Vector3[] verts, int[] tris, int i, Bounds bounds, out int gxs, out int  gxe, out int gys, out int gye, out int gzs, out int gze)
	{
		gxs = 0;
		gxe = 0;
		gys = 0;
		gye = 0;
		gzs = 0;
		gze = 0;
	}

	int GetXCell(int[,,] cells, int x, int y, int z, int dim)
	{
		for ( int i = x; i < dim; i++ )
		{
			if ( cells[i,y,z] != 0 )
			{
				return i;
			}
		}

		return -1;	// no cell
	}

	int GetYCell(int[,,] cells, int x, int y, int z, int dim)
	{
		for ( int i = y; i < dim; i++ )
		{
			if ( cells[x, i, z] != 0 )
			{
				return i;
			}
		}

		return -1;	// no cell
	}

	int GetZCell(int[,,] cells, int x, int y, int z, int dim)
	{
		for ( int i = z; i < dim; i++ )
		{
			if ( cells[x, y, i] != 0 )
			{
				return i;
			}
		}

		return -1;	// no cell
	}

	int[,,] cells;
	int xs;
	int ys;
	int zs;
	Vector3 cellsize = Vector3.one;
	Vector3	origin;
	Mesh basemesh;
	GameObject voxobj;

	// We have spring builder in my max destruction soft body so use that
	public void VoxelMesh(GameObject obj, float size, bool fill)
	{
		if ( obj == null )
			return;

		voxobj = obj;
		Mesh mesh = MegaUtils.GetMesh(voxobj);
		basemesh = mesh;
		int[]		tris = mesh.triangles;
		Vector3[]	verts = mesh.vertices;

		origin = mesh.bounds.min;

		Vector3 meshsize = mesh.bounds.size;

		xs = Mathf.CeilToInt(meshsize.x / size);
		if ( xs == 0 )
			xs = 1;

		ys = Mathf.CeilToInt(meshsize.y / size);
		if ( ys == 0 )
			ys = 1;

		zs = Mathf.CeilToInt(meshsize.z / size);
		if ( zs == 0 )
			zs = 1;

		// So first check all tris against each cell
		// first get bounds of tri

		// is it quicker to do each tri, get bounds and check those
		// or do each cell and check all tris, (can early out on this)

		// alloc array to hold cell data
		cells = new int[xs, ys, zs];

		// Mmmm looks like a lot quicker otherway for 30 verts on a 4x4x4 we loop 4x4x4x30= 1920
		// if we do tri and get bounds then a lot quicker could be as low as 30 or 2x2x2x30 = 240
		Bounds box = new Bounds();

		int gxs,gxe;
		int gys,gye;
		int gzs,gze;

		Vector3 min = Vector3.zero;

		Vector3 half = new Vector3(size * 0.5f, size * 0.5f, size * 0.5f);
		cellsize = new Vector3(size, size, size);

		for ( int i = 0; i < tris.Length; i += 3 )
		{
			// do bounds on tri
			GetTriBounds(verts, tris, i, mesh.bounds, out gxs, out gxe, out gys, out gye, out gzs, out gze);

			for ( int z = gzs; z < gze; z++ )
			{
				min.z = mesh.bounds.min.z + (z * size);

				for ( int y = gys; y < gye; y++ )
				{
					min.y = mesh.bounds.min.y + (y * size);

					for ( int x = gxs; x < gxe; x++ )
					{
						min.x = mesh.bounds.min.x + (x * size);

						box.SetMinMax(min, min + cellsize);

						// build box for cell
						if ( AABB_Triangle_Intersection.TriangleBoxOverlap(verts[tris[i]], verts[tris[i + 1]], verts[tris[i + 2]], box) )
							cells[x,y,z] = 1;
					}
				}
			}
		}

		// We should have a shell now, so now to fill line by line
		// can do for each axis and combine result (bit per axis) any bit means voxel, this will fill holes

		// Fill it, should do for each axis
		// could be a bug here here cant assume pen changes state in next block, so for pen down we get
		// the first block but we need end of contigous block
		if ( fill )
		{
			for ( int z = 0; z < zs; z++ )
			{
				for ( int y = 0; y < ys; y++ )
				{
					int pd = 0;
					while ( true )	//x != -1 )
					{
						// get pen down
						pd = GetXCell(cells, pd, y, z, xs);
						if ( pd == -1 )
							break;

						// get pen up
						int pu = GetXCell(cells, pd + 1, y, z, xs);

						if ( pu == -1 )
							break;

						for ( int x = pd + 1; x < pu; x++ )
							cells[x,y,z] = 2;
						pd = pu + 1;	// pd is 
					}
				}
			}
		}
	}

	void Display()
	{
		Vector3 pos = Vector3.zero;

		Gizmos.matrix = voxobj.transform.localToWorldMatrix;

		// Draw a cubecap for each cell, green for shell red for inside
		for ( int z = 0; z < zs; z++ )
		{
			pos.z = origin.z + z * cellsize.z;

			for ( int y = 0; y < ys; y++ )
			{
				pos.y = origin.y + y * cellsize.y;

				for ( int x = 0; x < xs; x++ )
				{
					pos.x = origin.x + x * cellsize.x;

					int v = cells[x,y,z];

					if ( v != 0 )
					{
						Gizmos.color = v == 1 ? Color.green : Color.red;

						Gizmos.DrawCube(pos, cellsize);
					}
				}
			}
		}
	}
}
#endif