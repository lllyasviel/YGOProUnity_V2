
#if false
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ColliderMesh
{
	public int[]		tris;
	public Vector3[]	verts;
	public Vector3[]	normals;
	public KDTree		tree;
	public VertTriList	trilist;
	public Mesh			mesh;
	public GameObject	obj;
}

#if true
//[AddComponentMenu("Modifiers/Deform")]
public class MegaDeform : MegaModifier
{
	public GameObject		obj;

	public float			decay = 1.0f;
	public bool				usedecay = false;
	public Vector3			normal = Vector3.up;

	bool		hadahit = false;
	Vector3		relativePoint = Vector3.zero;
	List<int>	affected = new List<int>();
	List<float>	distances = new List<float>();
	Matrix4x4	mat = new Matrix4x4();
	Vector3[]	offsets;
	Vector3[]	normals;

	public override string ModName() { return "Deform"; }
	public override string GetHelpURL() { return "Deform.htm"; }

	public ColliderMesh	colmesh;

#if false
	void DeformMesh()
	{
		for ( int i = 0; i < verts.Length; i++ )
		{
			Vector3 p = verts[i];	// + offsets[i];
			Vector3 origin = transform.TransformPoint(p);
			if ( col.bounds.Contains(origin) )
			{
				RaycastHit hit;

				// Do ray from a distance away to the point, if hit then inside mesh
				origin -= normals[i] * 10.0f;
				Ray ray = new Ray(origin, Vector3.up);	//normals[i]);

				if ( col.Raycast(ray, out hit, 10.0f) )
				{
					if ( hit.distance < 10.0f )	//&& hit.distance > 0.0f )
					{
						//Vector3 hp = transform.worldToLocalMatrix.MultiplyPoint(hit.point);
						//Vector3 hp = p - (normals[i] * (10.0f - hit.distance));

						float pen = 10.0f - hit.distance;

						if ( pen > penetration[i] )
						{
							penetration[i] = pen;
							//offsets[i] = -(normals[i] * pen);	//(10.0f - hit.distance));
							offsets[i] = -(Vector3.up * pen);	//(10.0f - hit.distance));
						}

						//p = hp;
					}
				}
			}

		}

		if ( !usedecay )
		{
			for ( int i = 0; i < verts.Length; i++ )
			{
				sverts[i].x = verts[i].x + offsets[i].x;
				sverts[i].y = verts[i].y + offsets[i].y;
				sverts[i].z = verts[i].z + offsets[i].z;
			}
		}
		else
		{
			for ( int i = 0; i < verts.Length; i++ )
			{
				offsets[i].x *= decay;
				offsets[i].y *= decay;
				offsets[i].z *= decay;

				sverts[i].x = verts[i].x + offsets[i].x;
				sverts[i].y = verts[i].y + offsets[i].y;
				sverts[i].z = verts[i].z + offsets[i].z;
			}
		}
	}
#else

	// To do falloff we need connected verts to each vert then just need to do recursive check until none effected
	void DeformMesh()
	{
		for ( int i = 0; i < verts.Length; i++ )
		{
			Vector3 p = verts[i];	// + offsets[i];
			Vector3 origin = transform.TransformPoint(p);
			if ( col.bounds.Contains(origin) )
			{
				// Point in collider space
				Vector3 lp = col.transform.worldToLocalMatrix.MultiplyPoint(origin);

				Vector3 np = NearestPointTest.NearestPointOnMesh(lp, colmesh.verts, colmesh.tris, colmesh.trilist);
				float dist = Vector3.Distance(lp, np);
				Debug.Log("[" + i + "] = " + dist.ToString("0.00000"));	//np " + np);
			}


			sverts[i] = verts[i];
		}
	}
#endif

	// So per point see if inside colliding mesh

	public override void Modify(MegaModifiers mc)
	{
		DeformMesh();
	}

	//public float standoff = 0.0f;

	// How to do this, (a) ray from each p along its normal and check for hit mesh, only need to do if point in
	// collider.bounds, need in local space
	// maybe best to use a sphere for first test
#if false
	public override Vector3 Map(int i, Vector3 p)
	{
		if ( i < 0 )
			return p;

		// First check against bounding box, should be in local space, actual sphere first?
		Vector3 origin = transform.TransformPoint(p);
		if ( col.bounds.Contains(origin) )
		{
			RaycastHit hit;

			// Do ray from a distance away to the point, if hit then inside mesh
			origin -= normals[i] * 10.0f;
			Ray ray = new Ray(origin, normals[i]);

			if ( col.Raycast(ray, out hit, 10.0f) )
			{
				if ( hit.distance < 10.0f )
				{
					//Vector3 hp = transform.worldToLocalMatrix.MultiplyPoint(hit.point);
					Vector3 hp = p - (normals[i] * (10.0f - hit.distance));

					p = hp;
				}
			}
		}

		return p;
	}
#endif

	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	Vector3	localPos = Vector3.zero;
	//public float radius = 1.0f;
	Collider col;

	float[] penetration;

	// Need to support multiple objects
	public GameObject	hitObject;

	public override bool Prepare(MegaModContext mc)
	{
		if ( colmesh == null )
		{
			colmesh = new ColliderMesh();
		}

		if ( colmesh.obj != hitObject && hitObject != null )
		{
			//Debug.Log("building");

			colmesh = new ColliderMesh();

			// This is colldier mesh not this mesh
			colmesh.mesh = MegaUtils.GetMesh(hitObject);
			//Debug.Log("verts " + colmesh.mesh.vertexCount);

			colmesh.verts = colmesh.mesh.vertices;
			colmesh.tris = colmesh.mesh.triangles;
			colmesh.normals = colmesh.mesh.normals;
			colmesh.tree = KDTree.MakeFromPoints(colmesh.verts);
			colmesh.trilist = new VertTriList(colmesh.mesh);
			colmesh.obj = hitObject;
			Debug.Log("Col info built");
		}
		//Debug.Log("Done");

		if ( hitObject )
		{
			localPos = transform.worldToLocalMatrix.MultiplyPoint(hitObject.transform.position);
			col = hitObject.collider;
		}

		if ( hitObject == null )
			return false;
		//if ( col == null )
			//return false;


		affected.Clear();
		distances.Clear();

		if ( offsets == null || offsets.Length != mc.mod.verts.Length )
			offsets = new Vector3[mc.mod.verts.Length];

		if ( normals == null || normals.Length != verts.Length )
			normals = mc.mod.mesh.normals;	// get current normals

		if ( penetration == null || penetration.Length != mc.mod.verts.Length )
		{
			penetration = new float[mc.mod.verts.Length];
		}
		mat = Matrix4x4.identity;

		SetAxis(mat);

		return true;
	}

	public override void PrepareMT(MegaModifiers mc, int cores)
	{
	}

	public override void DoWork(MegaModifiers mc, int index, int start, int end, int cores)
	{
		if ( index == 0 )
			Modify(mc);
	}

	// Collision
	const float EPSILON = 0.000001f;

	int intersect_triangle(Vector3 orig, Vector3 dir, Vector3 vert0, Vector3 vert1, Vector3 vert2, out float t, out float u, out float v)
	{
		Vector3 edge1,edge2,tvec,pvec,qvec;
		float det,inv_det;

		// find vectors for two edges sharing vert0
		edge1 = vert1 - vert0;
		edge2 = vert2 - vert0;
		
		//SUB(edge1, vert1, vert0);
		//SUB(edge2, vert2, vert0);

		/* begin calculating determinant - also used to calculate U parameter */
		//CROSS(pvec, dir, edge2);
		pvec = Vector3.Cross(dir, edge2);

		/* if determinant is near zero, ray lies in plane of triangle */
		//det = DOT(edge1, pvec);
		det = Vector3.Dot(edge1, pvec);

		t = u = v = 0.0f;	// or use ref

#if TEST_CULL           // define TEST_CULL if culling is desired
		if ( det < EPSILON )
			return 0;

		// calculate distance from vert0 to ray origin
		//SUB(tvec, orig, vert0);
		tvec = orig - vert0;

		/* calculate U parameter and test bounds */
		//*u = DOT(tvec, pvec);
		u = Vector3.Dot(tvec, pvec);
		if ( u < 0.0f || u > det )
			return 0;

		/* prepare to test V parameter */
		//CROSS(qvec, tvec, edge1);
		qvec = Vector3.Cross(tvec, edge1);

		/* calculate V parameter and test bounds */
		//*v = DOT(dir, qvec);
		v = Vector3.Dot(dir, qvec);
		if ( v < 0.0f || u + v > det )
			return 0;

		/* calculate t, scale parameters, ray intersects triangle */
		//*t = DOT(edge2, qvec);
		t = Vector3.Dot(edge2, qvec);
		inv_det = 1.0f / det;
		t *= inv_det;
		u *= inv_det;
		v *= inv_det;
#else                    // the non-culling branch
		if ( det > -EPSILON && det < EPSILON )
			return 0;

		inv_det = 1.0f / det;

		/* calculate distance from vert0 to ray origin */
		//SUB(tvec, orig, vert0);
		tvec = orig - vert0;

		/* calculate U parameter and test bounds */
		//*u = DOT(tvec, pvec) * inv_det;
		u = Vector3.Dot(tvec, pvec) * inv_det;
		if ( u < 0.0f || u > 1.0f )
			return 0;

		/* prepare to test V parameter */
		//CROSS(qvec, tvec, edge1);
		qvec = Vector3.Cross(tvec, edge1);

		/* calculate V parameter and test bounds */
		//*v = DOT(dir, qvec) * inv_det;
		v = Vector3.Dot(dir, qvec) * inv_det;
		if ( v < 0.0f || u + v > 1.0f )
			return 0;

		/* calculate t, ray intersects triangle */
		//*t = DOT(edge2, qvec) * inv_det;
		t = Vector3.Dot(edge2, qvec) * inv_det;
#endif
   return 1;
}

}
#endif
#endif