
using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class MatchShape : MonoBehaviour
{
#if false
	void GetTriBounds(Vector3[] verts, int[] tris, int tri, Vector3 min, out int gxs, out int gxe, out int gys, out int gye, out int gzs, out int gze)
	{
		Vector3 p = verts[tris[tri]];

		//Debug.Log("p0 " + p);
		//Vector3 min = bounds.min;

		gxs = gxe = (int)((p.x - min.x) / CellSize);
		gys = gye = (int)((p.y - min.y) / CellSize);
		gzs = gze = (int)((p.z - min.z) / CellSize);

		for ( int i = 1; i < 3; i++ )
		{
			p = verts[tris[tri + i]];

			//Debug.Log("p" + i + " " + p);

			int x = (int)((p.x - min.x) / CellSize);
			int y = (int)((p.y - min.y) / CellSize);
			int z = (int)((p.z - min.z) / CellSize);
			if ( x < gxs )	gxs = x;
			if ( x > gxe )	gxe = x;
			if ( y < gys )	gys = y;
			if ( y > gye )	gye = y;
			if ( z < gzs )	gzs = z;
			if ( z > gze )	gze = z;
		}
	}

	int GetXCellOld(int[, ,] cells, int x, int y, int z, int dim)
	{
		for ( int i = x; i < dim; i++ )
		{
			if ( cells[i, y, z] != 0 )
			{
				return i;
			}
		}

		return -1;	// no cell
	}

	int GetXCell(int[, ,] cells, int x, int y, int z, int dim)
	{
		for ( int i = x; i < dim; i++ )
		{
			if ( cells[i, y, z] != 0 )
			{
				for ( int j = i + 1; j < dim; j++ )
				{
					if ( cells[j, y, z] == 0 )
						return j - 1;
				}

				return i;
			}
		}

		return -1;	// no cell
	}

	int GetYCell(int[, ,] cells, int x, int y, int z, int dim)
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

	int GetZCell(int[, ,] cells, int x, int y, int z, int dim)
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

	public int[,,] cells;
	public int xs = 0;
	public int ys = 0;
	public int zs = 0;
	public Vector3 cellsize = Vector3.one;
	public Vector3	origin;
	Mesh basemesh;
	GameObject voxobj;

	public bool update = false;
	public float CellSize = 1.0f;
	public bool Fill = false;

	void Update()
	{
		if ( update )
		{
			if ( !realtime )
				update = false;

			CellSize = Mathf.Clamp(CellSize, 0.1f, 1000.0f);
			VoxelMesh(gameObject, CellSize, Fill);
		}
	}

	void OnDrawGizmosSelected()
	{
		Display();
	}

	//public Vector3 Jitter = Vector3.zero;
	public bool realtime = true;

	// We only need to collision with outside masses so any mass not used 6 times, not if a shell
	// plus we give external links a different stiffness
	// should work from centre of mesh
	// Can easily thread this
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


		Vector3 meshsize = mesh.bounds.size;

		xs = Mathf.CeilToInt(meshsize.x / size);
		if ( xs == 0 )
			xs = 1;

		//Debug.Log("ys " + (meshsize.y / size).ToString("0.000"));
		ys = Mathf.CeilToInt(meshsize.y / size);
		if ( ys == 0 )
			ys = 1;

		zs = Mathf.CeilToInt(meshsize.z / size);
		if ( zs == 0 )
			zs = 1;

		origin.x = -((float)xs * 0.5f * size);	//mesh.bounds.min + Jitter;
		origin.y = -((float)ys * 0.5f * size);	//mesh.bounds.min + Jitter;
		origin.z = -((float)zs * 0.5f * size);	//mesh.bounds.min + Jitter;

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

		//Debug.Log("Origin " + origin.ToString("0.000"));
		for ( int i = 0; i < tris.Length; i += 3 )
		{
			// do bounds on tri
			GetTriBounds(verts, tris, i, origin, out gxs, out gxe, out gys, out gye, out gzs, out gze);

			//Debug.Log("b[" + i + "] " + gxs + " " + gxe + " " + gys + " " + gye + " " + gzs + " " + gze);
			//for ( int z = 0; z < zs; z++ )
			for ( int z = gzs; z <= gze; z++ )
			{
				min.z = origin.z + (z * size);

				//for ( int y = 0; y < ys; y++ )
				for ( int y = gys; y <= gye; y++ )
				{
					min.y = origin.y + (y * size);

					//for ( int x = 0; x < xs; x++ )
					for ( int x = gxs; x <= gxe; x++ )
					{
						min.x = origin.x + (x * size);

						box.SetMinMax(min, min + cellsize);
						//Debug.Log("box " + box.min.ToString("0.000"));
						// build box for cell
						if ( AABB_Triangle_Intersection.TriangleBoxOverlap(verts[tris[i]], verts[tris[i + 1]], verts[tris[i + 2]], box) )
						{
							if ( x < xs && y < ys && z < zs )
								cells[x, y, z] = 1;
							//Debug.Log("hit " + x + " " + y + " " + z);
						}
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
							cells[x, y, z] = 2;
						pd = pu + 1;	// pd is 
					}
				}
			}
		}

		BuildLattice();

		// Build bary coords
		BaryCoord[] barycoords = new BaryCoord[verts.Length];

		for ( int i = 0; i < verts.Length; i++ )
		{
			Vector3 p = verts[i];

			int x = (int)((p.x - origin.x) / CellSize);
			int y = (int)((p.y - origin.y) / CellSize);
			int z = (int)((p.z - origin.z) / CellSize);

			BaryCoord bc = new BaryCoord();
			bc.p.x = (p.x - origin.x - (float)x * CellSize) / CellSize;
			bc.p.y = (p.y - origin.y - (float)y * CellSize) / CellSize;
			bc.p.z = (p.z - origin.z - (float)z * CellSize) / CellSize;

			for ( int c = 0; c < 8; c++ )
			{
				bc.m[c] = GetMassIndex(x, y, z, c);
			}
			//bc.m = FindMassIndex(p);
			//Debug.Log("[" + i + "] " + x + " " + y + " " + z + " " + p);
			barycoords[i] = bc;
		}

		// Rebuild verts test
		for ( int i = 0; i < barycoords.Length; i++ )
		{
			BaryCoord bc = barycoords[i];

			Vector3 bl = Vector3.Lerp(voxmasses[bc.m[0]].p, voxmasses[bc.m[1]].p, bc.p.z);
			Vector3 tl = Vector3.Lerp(voxmasses[bc.m[4]].p, voxmasses[bc.m[5]].p, bc.p.z);

			Vector3 br = Vector3.Lerp(voxmasses[bc.m[3]].p, voxmasses[bc.m[2]].p, bc.p.z);
			Vector3 tr = Vector3.Lerp(voxmasses[bc.m[7]].p, voxmasses[bc.m[6]].p, bc.p.z);

			Vector3 l = Vector3.Lerp(bl, tl, bc.p.y);
			Vector3 r = Vector3.Lerp(br, tr, bc.p.y);

			verts[i] = Vector3.Lerp(l, r, bc.p.x);
		}

		//mesh.vertices = verts;
	}

	int FindMassIndex(Vector3 p)
	{
		for ( int i = 0; i < voxmasses.Count; i++ )
		{
			if ( voxmasses[i].p == p )
				return i;
		}

		return -1;
	}

	int GetMassIndex(int x, int y, int z, int c)
	{
		Vector3 p = Vector3.zero;

		p.z = origin.z + (z * CellSize);
		p.y = origin.y + (y * CellSize);
		p.x = origin.x + (x * CellSize);

		return FindMassIndex(GetCorner(p, c));
	}

	class BaryCoord
	{
		public int[]	m = new int[8];
		public Vector3	p;
	}

	public Color fillcol = Color.red;
	public Color edgecol = Color.green;

	void Display1()
	{
		if ( cells == null )
			return;

		Vector3 pos = Vector3.zero;

		Gizmos.matrix = transform.localToWorldMatrix;

		Vector3 half = cellsize * 0.5f;

		Vector3 org = origin + half;	// - Jitter;

		// Draw a cubecap for each cell, green for shell red for inside
		Gizmos.color = Color.green;

		if ( Fill )
		{
			//Gizmos.color = Color.red;

			Color col1 = fillcol;

			for ( int z = 0; z < zs; z++ )
			{
				pos.z = org.z + z * cellsize.z;

				col1.r -= 0.01f;

				for ( int y = 0; y < ys; y++ )
				{
					Gizmos.color = col1;	//Color.green;
					pos.y = org.y + y * cellsize.y;

					for ( int x = 0; x < xs; x++ )
					{
						pos.x = org.x + x * cellsize.x;

						int v = cells[x, y, z];

						if ( v == 2 )
							Gizmos.DrawCube(pos, cellsize);
					}
				}
			}
		}

		Color col = edgecol;

		for ( int z = 0; z < zs; z++ )
		{
			pos.z = org.z + z * cellsize.z;

			col.g -= 0.01f;
			for ( int y = 0; y < ys; y++ )
			{
				Gizmos.color = col;	//Color.green;
				pos.y = org.y + y * cellsize.y;

				for ( int x = 0; x < xs; x++ )
				{
					pos.x = org.x + x * cellsize.x;

					int v = cells[x, y, z];

					if ( v == 1 )
						Gizmos.DrawCube(pos, cellsize);
				}
			}
		}

	}


	void Display()
	{
		if ( cells == null )
			return;

		Gizmos.matrix = transform.localToWorldMatrix;

		Color col = edgecol;
		Gizmos.color = col;

		for ( int i = 0; i < links.Count; i++ )
		{
			Gizmos.DrawLine(voxmasses[links[i].m1].p, voxmasses[links[i].m2].p);
		}
	}

	int IsSolid(int x, int y, int z)
	{
		if ( x >= 0 && y >= 0 && x >= 0 )
		{
			return cells[x, y, z];
		}

		return 0;
	}

	class VoxMass
	{
		public VoxMass(Vector3 _p)
		{
			p = _p;
		}
		public Vector3 p;
	}

	class Link
	{
		public Link(int _m1, int _m2)
		{
			m1 = _m1;
			m2 = _m2;
		}
		public int m1;
		public int m2;
	}

	List<VoxMass>	voxmasses = new List<VoxMass>();
	List<Link>		links = new List<Link>();

	int AddMass(Vector3 p)
	{
		for ( int i = 0; i < voxmasses.Count; i++ )
		{
			if ( voxmasses[i].p == p )
				return i;
		}

		voxmasses.Add(new VoxMass(p));
		return voxmasses.Count - 1;
	}

	int AddLink(int m1, int m2)
	{
		for ( int i = 0; i < links.Count; i++ )
		{
			if ( links[i].m1 == m1 && links[i].m2 == m2  )
				return i;

			if ( links[i].m1 == m2 && links[i].m2 == m1 )
				return i;
		}

		links.Add(new Link(m1, m2));
		return links.Count - 1;
	}

	Vector3 GetCorner(Vector3 org, int corner)
	{
		Vector3 p = org;

		switch ( corner )
		{
			case 0:	break;	//return org;
			case 1: org.z += cellsize.z;	break;
			case 2: org.z += cellsize.z; org.x += cellsize.x; break;
			case 3: org.x += cellsize.x; break;

			case 4: org.y += cellsize.y; break;	//return org;
			case 5: org.y += cellsize.y; org.z += cellsize.z; break;
			case 6: org.y += cellsize.y; org.z += cellsize.z; org.x += cellsize.x; break;
			case 7: org.y += cellsize.y; org.x += cellsize.x; break;
		}

		return org;
	}

	// Can split submeshes or vcol out
	// Can have different stiffness for internals, also option to not fill (have single springs)
	void BuildLattice()
	{
		Vector3 pos = Vector3.zero;
		Vector3 org = origin;

		links.Clear();
		voxmasses.Clear();

		int[] corners = new int[8];

		for ( int z = 0; z < zs; z++ )
		{
			pos.z = org.z + z * cellsize.z;

			for ( int y = 0; y < ys; y++ )
			{
				pos.y = org.y + y * cellsize.y;

				for ( int x = 0; x < xs; x++ )
				{
					pos.x = org.x + x * cellsize.x;

					if ( IsSolid(x, y, z) > 0 )
					{
						// Add the 8 corner masses and the 12 links
						// bot square
						for ( int c = 0; c < 8; c++ )
						{
							Vector3 cp = GetCorner(pos, c);
							corners[c] = AddMass(cp);
						}

						// Add colours to show links type
						// Bottom
						AddLink(corners[0], corners[1]);
						AddLink(corners[1], corners[2]);
						AddLink(corners[2], corners[3]);
						AddLink(corners[3], corners[0]);

						// top
						AddLink(corners[4], corners[5]);
						AddLink(corners[5], corners[6]);
						AddLink(corners[6], corners[7]);
						AddLink(corners[7], corners[4]);

						// Sides
						AddLink(corners[0], corners[4]);
						AddLink(corners[1], corners[5]);
						AddLink(corners[2], corners[6]);
						AddLink(corners[3], corners[7]);

						// Option to add cross bracing, flag for each type of brace, could add cross grid braces
						// Bot brace
						AddLink(corners[0], corners[2]);
						AddLink(corners[1], corners[3]);

						// Top brace
						AddLink(corners[4], corners[6]);
						AddLink(corners[5], corners[7]);

						// Left brace
						AddLink(corners[0], corners[5]);
						AddLink(corners[1], corners[4]);

						// Back brace
						AddLink(corners[1], corners[6]);
						AddLink(corners[2], corners[5]);

						// Right brace
						AddLink(corners[2], corners[7]);
						AddLink(corners[3], corners[6]);

						// Front brace
						AddLink(corners[3], corners[4]);
						AddLink(corners[0], corners[7]);

						// Cross brace 1
						AddLink(corners[0], corners[6]);
						AddLink(corners[4], corners[2]);

						// Cross brace 2
						AddLink(corners[1], corners[7]);
						AddLink(corners[5], corners[3]);
					}
				}
			}
		}

		//Debug.Log("masses " + voxmasses.Count);
		//Debug.Log("links " + links.Count);
	}


	float Determinant(Matrix4x4 m)
	{
		Vector3 r0 = m.GetRow(0);
		Vector3 r1 = m.GetRow(1);
		Vector3 r2 = m.GetRow(2);

		return -(r0.z * r1.y * r2.x) + (r0.y * r1.z * r2.x) + (r0.z * r1.x * r2.y) - (r0.x * r1.z * r2.y) - (r0.y * r1.x * r2.z) + (r0.x * r1.y * r2.z);
	}

	Vector3 barycentricCoords(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p)
	{
		Vector3 bc = Vector3.zero;
		Vector3 q  = p - p3;
		Vector4 q0 = p0 - p3;
		Vector4 q1 = p1 - p3;
		Vector4 q2 = p2 - p3;

		Matrix4x4 m = Matrix4x4.identity;
		m.SetColumn(0, q0);
		m.SetColumn(1, q1);
		m.SetColumn(2, q2);

		float det = Determinant(m);

		m.SetColumn(0, q);
		bc.x = Determinant(m);

		m.SetColumn(0, q0);
		m.SetColumn(1, q);
		bc.y = Determinant(m);

		m.SetColumn(1, q1);
		m.SetColumn(2, q);
		bc.z = Determinant(m);

		if ( det != 0.0f )
			bc /= det;

		return bc;
	}
#endif
}
