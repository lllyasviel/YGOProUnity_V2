
using UnityEngine;
using System.Collections.Generic;

// TODO: Help as a popup window of text

// This will be a basic box mesh but need to have different uvs for front and back, and maybe different mtlids
// if have thickness then again different mtlid maybe and or uvs
// first version ignore thickness

// each mesh should have an ption to set pivot, or centre it, also rotate

// should derive from SimpleMesh or some class as will be adding a few of these
public class MegaMeshPage : MonoBehaviour
{
	public float	Width		= 1.0f;
	public float	Length		= 1.41f;
	public float	Height		= 0.1f;
	public int		WidthSegs	= 10;
	public int		LengthSegs	= 10;
	public int		HeightSegs	= 1;
	public bool		genUVs		= true;
	public float	rotate		= 0.0f;
	public bool		PivotBase	= false;
	public bool		PivotEdge	= true;
	public bool		tangents	= false;
	public bool		optimize	= false;

	[ContextMenu("Help")]
	public void Help()
	{
		Application.OpenURL("http://www.west-racing.com/mf/?page_id=853");
	}

	void Reset()
	{
		Rebuild();
	}

	public void Rebuild()
	{
		MeshFilter mf = GetComponent<MeshFilter>();

		if ( mf != null )
		{
			Mesh mesh = mf.sharedMesh;	//Utils.GetMesh(gameObject);

			if ( mesh == null )
			{
				mesh = new Mesh();
				mf.sharedMesh = mesh;
			}

			if ( mesh != null )
			{
				BuildMesh(mesh);
				MegaModifyObject mo = GetComponent<MegaModifyObject>();
				if ( mo != null )
				{
					mo.MeshUpdated();
				}
			}
		}
	}

	static void MakeQuad1(List<int> f, int a, int b, int c, int d)
	{
		f.Add(a);
		f.Add(b);
		f.Add(c);

		f.Add(c);
		f.Add(d);
		f.Add(a);
	}

	// Put in utils
	int MaxComponent(Vector3 v)
	{
		if ( Mathf.Abs(v.x) > Mathf.Abs(v.y) )
		{
			if ( Mathf.Abs(v.x) > Mathf.Abs(v.z) )
				return 0;
			else
				return 2;
		}
		else
		{
			if ( Mathf.Abs(v.y) > Mathf.Abs(v.z) )
				return 1;
			else
				return 2;
		}
	}

	void BuildMesh(Mesh mesh)
	{
		Width = Mathf.Clamp(Width, 0.0f, float.MaxValue);
		Length = Mathf.Clamp(Length, 0.0f, float.MaxValue);
		Height = Mathf.Clamp(Height, 0.0f, float.MaxValue);

		LengthSegs = Mathf.Clamp(LengthSegs, 1, 200);
		HeightSegs = Mathf.Clamp(HeightSegs, 1, 200);
		WidthSegs = Mathf.Clamp(WidthSegs, 1, 200);

		Vector3 vb = new Vector3(Width, Height, Length) / 2.0f;
		Vector3 va = -vb;

		if ( PivotBase )
		{
			va.y = 0.0f;
			vb.y = Height;
		}

		if ( PivotEdge )
		{
			va.x = 0.0f;
			vb.x = Width;
		}

		float dx = Width / (float)WidthSegs;
		float dy = Height / (float)HeightSegs;
		float dz = Length / (float)LengthSegs;

		Vector3 p = va;

		// Lists should be static, clear out to reuse
		List<Vector3>	verts = new List<Vector3>();
		List<Vector2>	uvs = new List<Vector2>();
		List<int>		tris = new List<int>();
		List<int>		tris1 = new List<int>();
		List<int>		tris2 = new List<int>();

		Vector2 uv = Vector2.zero;

		// Do we have top and bottom
		if ( Width > 0.0f && Length > 0.0f )
		{
			Matrix4x4 tm1 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, rotate, 0.0f), Vector3.one);

			Vector3 uv1 = Vector3.zero;

			p.y = vb.y;
			for ( int iz = 0; iz <= LengthSegs; iz++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= WidthSegs; ix++ )
				{
					verts.Add(p);

					if ( genUVs )
					{
						//uv.x = Mathf.Repeat((p.x + vb.x) / Width, 1.0f);
						uv.x = (p.x + vb.x) / Width;
						uv.y = (p.z + vb.z) / Length;

						uv1.x = uv.x - 0.5f;
						uv1.y = 0.0f;
						uv1.z = uv.y - 0.5f;

						uv1 = tm1.MultiplyPoint3x4(uv1);
						uv.x = 0.5f + uv1.x;
						uv.y = 0.5f + uv1.z;

						uvs.Add(uv);
					}
					p.x += dx;
				}
				p.z += dz;
			}

			for ( int iz = 0; iz < LengthSegs; iz++ )
			{
				int kv = iz * (WidthSegs + 1);
				for ( int ix = 0; ix < WidthSegs; ix++ )
				{
					MakeQuad1(tris, kv, kv + WidthSegs + 1, kv + WidthSegs + 2, kv + 1);
					kv++;
				}
			}

			int index = verts.Count;

			p.y = va.y;
			p.z = va.z;

			for ( int iy = 0; iy <= LengthSegs; iy++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= WidthSegs; ix++ )
				{
					verts.Add(p);
					if ( genUVs )
					{
						uv.x = 1.0f - ((p.x + vb.x) / Width);
						uv.y = ((p.z + vb.z) / Length);

						uv1.x = uv.x - 0.5f;
						uv1.y = 0.0f;
						uv1.z = uv.y - 0.5f;

						uv1 = tm1.MultiplyPoint3x4(uv1);
						uv.x = 0.5f + uv1.x;
						uv.y = 0.5f + uv1.z;

						uvs.Add(uv);
					}
					p.x += dx;
				}
				p.z += dz;
			}

			for ( int iy = 0; iy < LengthSegs; iy++ )
			{
				int kv = iy * (WidthSegs + 1) + index;
				for ( int ix = 0; ix < WidthSegs; ix++ )
				{
					MakeQuad1(tris1, kv, kv + 1, kv + WidthSegs + 2, kv + WidthSegs + 1);
					kv++;
				}
			}
		}

		// Front back
		if ( Width > 0.0f && Height > 0.0f )
		{
			int index = verts.Count;

			p.z = va.z;
			p.y = va.y;
			for ( int iz = 0; iz <= HeightSegs; iz++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= WidthSegs; ix++ )
				{
					verts.Add(p);
					if ( genUVs )
					{
						uv.x = (p.x + vb.x) / Width;
						uv.y = (p.y + vb.y) / Height;
						uvs.Add(uv);
					}
					p.x += dx;
				}
				p.y += dy;
			}

			for ( int iz = 0; iz < HeightSegs; iz++ )
			{
				int kv = iz * (WidthSegs + 1) + index;
				for ( int ix = 0; ix < WidthSegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + WidthSegs + 1, kv + WidthSegs + 2, kv + 1);
					kv++;
				}
			}

			index = verts.Count;

			p.z = vb.z;
			p.y = va.y;
			for ( int iy = 0; iy <= HeightSegs; iy++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= WidthSegs; ix++ )
				{
					verts.Add(p);
					if ( genUVs )
					{
						uv.x = (p.x + vb.x) / Width;
						uv.y = (p.y + vb.y) / Height;
						uvs.Add(uv);
					}
					p.x += dx;
				}
				p.y += dy;
			}

			for ( int iy = 0; iy < HeightSegs; iy++ )
			{
				int kv = iy * (WidthSegs + 1) + index;
				for ( int ix = 0; ix < WidthSegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + 1, kv + WidthSegs + 2, kv + WidthSegs + 1);
					kv++;
				}
			}
		}

		// Left Right
		if ( Length > 0.0f && Height > 0.0f )
		{
			int index = verts.Count;

			p.x = vb.x;
			p.y = va.y;
			for ( int iz = 0; iz <= HeightSegs; iz++ )
			{
				p.z = va.z;
				for ( int ix = 0; ix <= LengthSegs; ix++ )
				{
					verts.Add(p);
					if ( genUVs )
					{
						uv.x = (p.z + vb.z) / Length;
						uv.y = (p.y + vb.y) / Height;
						uvs.Add(uv);
					}
					p.z += dz;
				}
				p.y += dy;
			}

			for ( int iz = 0; iz < HeightSegs; iz++ )
			{
				int kv = iz * (LengthSegs + 1) + index;
				for ( int ix = 0; ix < LengthSegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + LengthSegs + 1, kv + LengthSegs + 2, kv + 1);
					kv++;
				}
			}

			index = verts.Count;

			p.x = va.x;
			p.y = va.y;
			for ( int iy = 0; iy <= HeightSegs; iy++ )
			{
				p.z = va.z;
				for ( int ix = 0; ix <= LengthSegs; ix++ )
				{
					verts.Add(p);
					if ( genUVs )
					{
						uv.x = (p.z + vb.z) / Length;
						uv.y = (p.y + vb.y) / Height;
						uvs.Add(uv);
					}

					p.z += dz;
				}
				p.y += dy;
			}

			for ( int iy = 0; iy < HeightSegs; iy++ )
			{
				int kv = iy * (LengthSegs + 1) + index;
				for ( int ix = 0; ix < LengthSegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + 1, kv + LengthSegs + 2, kv + LengthSegs + 1);
					kv++;
				}
			}
		}

		mesh.Clear();

		mesh.subMeshCount = 3;

		mesh.vertices = verts.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.SetTriangles(tris.ToArray(), 0);
		mesh.SetTriangles(tris1.ToArray(), 1);
		mesh.SetTriangles(tris2.ToArray(), 2);

		mesh.RecalculateNormals();

		if ( tangents )
			BuildTangents(mesh);

		if ( optimize )
			;

		mesh.RecalculateBounds();
	}


	void BuildMeshOld(Mesh mesh)
	{
		Width = Mathf.Clamp(Width, 0.0f, float.MaxValue);
		Length = Mathf.Clamp(Length, 0.0f, float.MaxValue);
		Height = Mathf.Clamp(Height, 0.0f, float.MaxValue);

		LengthSegs = Mathf.Clamp(LengthSegs, 1, 200);
		HeightSegs = Mathf.Clamp(HeightSegs, 1, 200);
		WidthSegs = Mathf.Clamp(WidthSegs, 1, 200);

		Vector3 vb = new Vector3(Width, Height, Length) / 2.0f;
		Vector3 va = -vb;

		if ( PivotBase )
		{
			va.y = 0.0f;
			vb.y = Height;
		}

		if ( PivotEdge )
		{
			va.x = 0.0f;
			vb.x = Width;
		}

		float dx = Width / (float)WidthSegs;
		float dy = Height / (float)HeightSegs;
		float dz = Length / (float)LengthSegs;

		Vector3 p = va;

		// Lists should be static, clear out to reuse
		List<Vector3> verts = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int>			tris = new List<int>();
		List<int>			tris1 = new List<int>();
		List<int>			tris2 = new List<int>();

		Vector2 uv = Vector2.zero;

		// Do we have top and bottom
		if ( Width > 0.0f && Length > 0.0f )
		{
			//Matrix4x4 tm1 = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0.0f, rotate, 0.0f), Vector3.one);

			p.y = vb.y;
			for ( int iz = 0; iz <= LengthSegs; iz++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= WidthSegs; ix++ )
				{
					verts.Add(p);

					if ( genUVs )
					{
						uv.x = (p.x + vb.x) / Width;
						uv.y = (p.z + vb.z) / Length;
						uvs.Add(uv);
					}
					p.x += dx;
				}
				p.z += dz;
			}

			for ( int iz = 0; iz < LengthSegs; iz++ )
			{
				int kv = iz * (WidthSegs + 1);
				for ( int ix = 0; ix < WidthSegs; ix++ )
				{
					MakeQuad1(tris, kv, kv + WidthSegs + 1, kv + WidthSegs + 2, kv + 1);
					kv++;
				}
			}

			int index = verts.Count;

			p.y = va.y;
			p.z = va.z;

			for ( int iy = 0; iy <= LengthSegs; iy++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= WidthSegs; ix++ )
				{
					verts.Add(p);
					if ( genUVs )
					{
						uv.x = 1.0f - ((p.x + vb.x) / Width);
						uv.y = ((p.z + vb.z) / Length);

						uvs.Add(uv);
					}
					p.x += dx;
				}
				p.z += dz;
			}

			for ( int iy = 0; iy < LengthSegs; iy++ )
			{
				int kv = iy * (WidthSegs + 1) + index;
				for ( int ix = 0; ix < WidthSegs; ix++ )
				{
					MakeQuad1(tris1, kv, kv + 1, kv + WidthSegs + 2, kv + WidthSegs + 1);
					kv++;
				}
			}
		}

		// Front back
		if ( Width > 0.0f && Height > 0.0f )
		{
			int index = verts.Count;

			p.z = va.z;
			p.y = va.y;
			for ( int iz = 0; iz <= HeightSegs; iz++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= WidthSegs; ix++ )
				{
					verts.Add(p);
					if ( genUVs )
					{
						uv.x = (p.x + vb.x) / Width;
						uv.y = (p.y + vb.y) / Height;
						uvs.Add(uv);
					}
					p.x += dx;
				}
				p.y += dy;
			}

			for ( int iz = 0; iz < HeightSegs; iz++ )
			{
				int kv = iz * (WidthSegs + 1) + index;
				for ( int ix = 0; ix < WidthSegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + WidthSegs + 1, kv + WidthSegs + 2, kv + 1);
					kv++;
				}
			}

			index = verts.Count;

			p.z = vb.z;
			p.y = va.y;
			for ( int iy = 0; iy <= HeightSegs; iy++ )
			{
				p.x = va.x;
				for ( int ix = 0; ix <= WidthSegs; ix++ )
				{
					verts.Add(p);
					if ( genUVs )
					{
						uv.x = (p.x + vb.x) / Width;
						uv.y = (p.y + vb.y) / Height;
						uvs.Add(uv);
					}
					p.x += dx;
				}
				p.y += dy;
			}

			for ( int iy = 0; iy < HeightSegs; iy++ )
			{
				int kv = iy * (WidthSegs + 1) + index;
				for ( int ix = 0; ix < WidthSegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + 1, kv + WidthSegs + 2, kv + WidthSegs + 1);
					kv++;
				}
			}
		}

		// Left Right
		if ( Length > 0.0f && Height > 0.0f )
		{
			int index = verts.Count;

			p.x = vb.x;
			p.y = va.y;
			for ( int iz = 0; iz <= HeightSegs; iz++ )
			{
				p.z = va.z;
				for ( int ix = 0; ix <= LengthSegs; ix++ )
				{
					verts.Add(p);
					if ( genUVs )
					{
						uv.x = (p.z + vb.z) / Length;
						uv.y = (p.y + vb.y) / Height;
						uvs.Add(uv);
					}
					p.z += dz;
				}
				p.y += dy;
			}

			for ( int iz = 0; iz < HeightSegs; iz++ )
			{
				int kv = iz * (LengthSegs + 1) + index;
				for ( int ix = 0; ix < LengthSegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + LengthSegs + 1, kv + LengthSegs + 2, kv + 1);
					kv++;
				}
			}

			index = verts.Count;

			p.x = va.x;
			p.y = va.y;
			for ( int iy = 0; iy <= HeightSegs; iy++ )
			{
				p.z = va.z;
				for ( int ix = 0; ix <= LengthSegs; ix++ )
				{
					verts.Add(p);
					if ( genUVs )
					{
						uv.x = (p.z + vb.z) / Length;
						uv.y = (p.y + vb.y) / Height;
						uvs.Add(uv);
					}

					p.z += dz;
				}
				p.y += dy;
			}

			for ( int iy = 0; iy < HeightSegs; iy++ )
			{
				int kv = iy * (LengthSegs + 1) + index;
				for ( int ix = 0; ix < LengthSegs; ix++ )
				{
					MakeQuad1(tris2, kv, kv + 1, kv + LengthSegs + 2, kv + LengthSegs + 1);
					kv++;
				}
			}
		}

		mesh.Clear();

		mesh.subMeshCount = 3;

		mesh.vertices = verts.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.SetTriangles(tris.ToArray(), 0);
		mesh.SetTriangles(tris1.ToArray(), 1);
		mesh.SetTriangles(tris2.ToArray(), 2);

		mesh.RecalculateNormals();

		if ( tangents )
			BuildTangents(mesh);

		if ( optimize )
			;

		mesh.RecalculateBounds();
	}


	static public void BuildTangents(Mesh mesh)
	{
		int triangleCount = mesh.triangles.Length;
		int vertexCount = mesh.vertices.Length;

		Vector3[] tan1 = new Vector3[vertexCount];
		Vector3[] tan2 = new Vector3[vertexCount];
		Vector4[] tangents = new Vector4[vertexCount];

		Vector3[] verts	= mesh.vertices;
		Vector2[] uvs		= mesh.uv;
		Vector3[] norms	= mesh.normals;
		int[]			tris	= mesh.triangles;

		for ( int a = 0; a < triangleCount; a += 3 )
		{
			long i1 = tris[a];
			long i2 = tris[a + 1];
			long i3 = tris[a + 2];

			Vector3 v1 = verts[i1];
			Vector3 v2 = verts[i2];
			Vector3 v3 = verts[i3];

			Vector2 w1 = uvs[i1];
			Vector2 w2 = uvs[i2];
			Vector2 w3 = uvs[i3];

			float x1 = v2.x - v1.x;
			float x2 = v3.x - v1.x;
			float y1 = v2.y - v1.y;
			float y2 = v3.y - v1.y;
			float z1 = v2.z - v1.z;
			float z2 = v3.z - v1.z;

			float s1 = w2.x - w1.x;
			float s2 = w3.x - w1.x;
			float t1 = w2.y - w1.y;
			float t2 = w3.y - w1.y;

			float r = 1.0f / (s1 * t2 - s2 * t1);

			Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
			Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

			tan1[i1] += sdir;
			tan1[i2] += sdir;
			tan1[i3] += sdir;

			tan2[i1] += tdir;
			tan2[i2] += tdir;
			tan2[i3] += tdir;
		}

		for ( int a = 0; a < vertexCount; a++ )
		{
			Vector3 n = norms[a];
			Vector3 t = tan1[a];

			Vector3.OrthoNormalize(ref n, ref t);
			tangents[a].x = t.x;
			tangents[a].y = t.y;
			tangents[a].z = t.z;
			tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}

		mesh.tangents = tangents;
	}
}