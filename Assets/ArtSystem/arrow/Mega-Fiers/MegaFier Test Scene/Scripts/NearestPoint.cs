using UnityEngine;
using System.Collections;

[System.Serializable]
public class VertList
{
	public int[]	t;
}

[System.Serializable]
public class VertTriList
{
	public VertList[]	list;

	public VertTriList(int[] tri, int numVerts)
	{
		Init(tri, numVerts);
	}

	public VertTriList(Mesh mesh)
	{
		Init(mesh.triangles, mesh.vertexCount);
	}

	//	You don't usually need to call this - it's just to assist the implementation of the constructors.
	public void Init(int[] tri, int numVerts)
	{
		//	First, go through the triangles, keeping a count of how many times each vert is used.
		int[] counts = new int[numVerts];

		for ( int i = 0; i < tri.Length; i++ )
			counts[tri[i]]++;

		//	Initialise an empty jagged array with the appropriate number of elements for each vert.
		//list = new int[numVerts][];
		list = new VertList[numVerts];	//[];

		for ( int i = 0; i < counts.Length; i++ )
		{
			list[i] = new VertList();
			list[i].t = new int[counts[i]];
		}

		//	Assign the appropriate triangle number each time a given vert is encountered in the triangles.
		for ( int i = 0; i < tri.Length; i++ )
		{
			int vert = tri[i];
			list[vert].t[--counts[vert]] = i / 3;
		}
	}
}

//  to get if inside do delta between point and closest point and dot with normal of face or face vert, neg is inside
public class NearestPointTest
{
	public static Vector3 NearestPointOnMesh(Vector3 pt, Vector3[] verts, KDTree vertProx, int[] tri, VertTriList vt)
	{
		//	First, find the nearest vertex (the nearest point must be on one of the triangles
		//	that uses this vertex if the mesh is convex).
		int nearest = vertProx.FindNearest(pt);

		//	Get the list of triangles in which the nearest vert "participates".
		VertList nearTris = vt.list[nearest];

		Vector3 nearestPt = Vector3.zero;
		float nearestSqDist = float.MaxValue;

		for ( int i = 0; i < nearTris.t.Length; i++ )
		{
			int triOff = nearTris.t[i] * 3;
			Vector3 a = verts[tri[triOff]];
			Vector3 b = verts[tri[triOff + 1]];
			Vector3 c = verts[tri[triOff + 2]];

			Vector3 possNearestPt = NearestPointOnTri(pt, a, b, c);
			float possNearestSqDist = (pt - possNearestPt).sqrMagnitude;

			if ( possNearestSqDist < nearestSqDist )
			{
				nearestPt = possNearestPt;
				nearestSqDist = possNearestSqDist;
			}
		}

		return nearestPt;
	}

	public static Vector3 NearestPointOnMesh(Vector3 pt, Vector3[] verts, int[] tri, VertTriList vt)
	{
		//	First, find the nearest vertex (the nearest point must be on one of the triangles
		//	that uses this vertex if the mesh is convex).
		int nearest = -1;
		float nearestSqDist = float.MaxValue;

		for ( int i = 0; i < verts.Length; i++ )
		{
			float sqDist = (verts[i] - pt).sqrMagnitude;

			if ( sqDist < nearestSqDist )
			{
				nearest = i;
				nearestSqDist = sqDist;
			}
		}

		//	Get the list of triangles in which the nearest vert "participates".
		VertList nearTris = vt.list[nearest];

		Vector3 nearestPt = Vector3.zero;
		nearestSqDist = float.MaxValue;

		for ( int i = 0; i < nearTris.t.Length; i++ )
		{
			int triOff = nearTris.t[i] * 3;
			Vector3 a = verts[tri[triOff]];
			Vector3 b = verts[tri[triOff + 1]];
			Vector3 c = verts[tri[triOff + 2]];

			Vector3 possNearestPt = NearestPointOnTri(pt, a, b, c);
			float possNearestSqDist = (pt - possNearestPt).sqrMagnitude;

			if ( possNearestSqDist < nearestSqDist )
			{
				nearestPt = possNearestPt;
				nearestSqDist = possNearestSqDist;
			}
		}

		return nearestPt;
	}

	public static Vector3 NearestPointOnTri(Vector3 pt, Vector3 a, Vector3 b, Vector3 c)
	{
		Vector3 edge1 = b - a;
		Vector3 edge2 = c - a;
		Vector3 edge3 = c - b;
		float edge1Len = edge1.magnitude;
		float edge2Len = edge2.magnitude;
		float edge3Len = edge3.magnitude;

		// unroll these ti make faster
		Vector3 ptLineA = pt - a;
		Vector3 ptLineB = pt - b;
		Vector3 ptLineC = pt - c;
		Vector3 xAxis = edge1 / edge1Len;
		Vector3 zAxis = Vector3.Cross(edge1, edge2).normalized;
		Vector3 yAxis = Vector3.Cross(zAxis, xAxis);

		// Can speed this up, do dots by hand and only do edge 2 and 3 if needed
		Vector3 edge1Cross = Vector3.Cross(edge1, ptLineA);
		Vector3 edge2Cross = Vector3.Cross(edge2, -ptLineC);
		Vector3 edge3Cross = Vector3.Cross(edge3, ptLineB);

		bool edge1On = Vector3.Dot(edge1Cross, zAxis) > 0.0f;
		bool edge2On = Vector3.Dot(edge2Cross, zAxis) > 0.0f;
		bool edge3On = Vector3.Dot(edge3Cross, zAxis) > 0.0f;

		//	If the point is inside the triangle then return its coordinate.
		if ( edge1On && edge2On && edge3On )
		{
			float xExtent = Vector3.Dot(ptLineA, xAxis);
			float yExtent = Vector3.Dot(ptLineA, yAxis);
			return a + xAxis * xExtent + yAxis * yExtent;
		}

		//	Otherwise, the nearest point is somewhere along one of the edges.
		Vector3 edge1Norm = xAxis;
		Vector3 edge2Norm = edge2.normalized;
		Vector3 edge3Norm = edge3.normalized;

		float edge1Ext = Mathf.Clamp(Vector3.Dot(edge1Norm, ptLineA), 0.0f, edge1Len);
		float edge2Ext = Mathf.Clamp(Vector3.Dot(edge2Norm, ptLineA), 0.0f, edge2Len);
		float edge3Ext = Mathf.Clamp(Vector3.Dot(edge3Norm, ptLineB), 0.0f, edge3Len);

		Vector3 edge1Pt = a + edge1Ext * edge1Norm;
		Vector3 edge2Pt = a + edge2Ext * edge2Norm;
		Vector3 edge3Pt = b + edge3Ext * edge3Norm;

		float sqDist1 = (pt - edge1Pt).sqrMagnitude;
		float sqDist2 = (pt - edge2Pt).sqrMagnitude;
		float sqDist3 = (pt - edge3Pt).sqrMagnitude;

		if ( sqDist1 < sqDist2 )
		{
			if ( sqDist1 < sqDist3 )
				return edge1Pt;
			else
				return edge3Pt;
		}
		else
		{
			if ( sqDist2 < sqDist3 )
				return edge2Pt;
			else
				return edge3Pt;
		}
	}
}
// 185