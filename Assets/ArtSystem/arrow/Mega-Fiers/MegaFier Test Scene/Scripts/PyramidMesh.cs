
using UnityEngine;
using System.Collections.Generic;

#if false
public class BaseMesh : MonoBehaviour
{
	public bool	tangents = false;
	public bool	optimize = false;
	public bool	genUVs = true;

	public virtual void BuildMesh(Mesh mesh)
	{

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
				BuildMesh(mesh);
		}
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

// This will be a basic box mesh but need to have different uvs for front and back, and maybe different mtlids
// if have thickness then again different mtlid maybe and or uvs
// first version ignore thickness

// each mesh should have an ption to set pivot, or centre it, also rotate

// should derive from SimpleMesh or some class as will be adding a few of these
public class PyramidMesh : BaseMesh
{
	public float	Width = 1.0f;
	public float	Depth = 1.41f;
	public float	Height = 0.1f;
	public int		WidthSegs = 1;
	public int		DepthSegs = 1;
	public int		HeightSegs = 1;

	public override void BuildMesh(Mesh mesh)
	{
		Width = Mathf.Clamp(Width, 0.0f, float.MaxValue);
		Depth = Mathf.Clamp(Depth, 0.0f, float.MaxValue);
		Height = Mathf.Clamp(Height, 0.0f, float.MaxValue);

		DepthSegs = Mathf.Clamp(DepthSegs, 1, 200);
		HeightSegs = Mathf.Clamp(HeightSegs, 1, 200);
		WidthSegs = Mathf.Clamp(WidthSegs, 1, 200);

		List<Vector3> verts = new List<Vector3>();
		List<Vector2> uvs = new List<Vector2>();
		List<int>			tris = new List<int>();

		mesh.Clear();

		mesh.vertices = verts.ToArray();
		mesh.uv = uvs.ToArray();
		mesh.triangles = tris.ToArray();

		mesh.RecalculateNormals();

		if ( tangents )
			BuildTangents(mesh);

		if ( optimize )
			mesh.Optimize();

		mesh.RecalculateBounds();
	}
}
#endif