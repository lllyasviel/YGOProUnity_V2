
using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class MegaModifierTarget
{
	public Vector3[]	verts;
	public Vector3[]	sverts;
	public Vector2[]	uvs;
	public Vector2[]	suvs;
	public Mesh			mesh;
	public Mesh			cachedMesh;
	public Vector4[]	tangents;
	public Vector3[]	tan1;
	public Vector3[]	tan2;
	public MeshCollider	meshCol;
	public GameObject	go;
	public MegaBox3		bbox;
	public Vector3		Offset;
}

// This should be the only one we need with just a single target for normal use

#if true	//false
//[AddComponentMenu("Modifiers/Modify Group")]
[ExecuteInEditMode]
public class MegaModifyGroup : MegaModifyObject
{
	public List<MegaModifierTarget>	targets = new List<MegaModifierTarget>();

	private static int CompareOrder(MegaModifier m1, MegaModifier m2)
	{
		return m1.Order - m2.Order;
	}

	[ContextMenu("Resort")]
	public override void Resort()
	{
		BuildList();
	}

	[ContextMenu("Help")]
	public override void Help()
	{
		Application.OpenURL("http://www.west-racing.com/mf/?page_id=444");
	}

	// Will need to save start transform as well for reset on any objects that have a transform update
	[ContextMenu("Reset Group Info")]
	public void GroupResetMeshInfo()
	{
		Debug.Log("reset");
		for ( int i = 0; i < targets.Count; i++ )
		{
			Debug.Log("1");
			if ( targets[i].cachedMesh == null )
			{
				Debug.Log("2");
				targets[i].cachedMesh = (Mesh)Mesh.Instantiate(GetMesh(targets[i].go));
			}
		}

		GroupReStart1(false);

		for ( int i = 0; i < targets.Count; i++ )
		{
			if ( targets[i].mesh != null )
			{
				targets[i].mesh.vertices = verts;	//_verts;	// mesh.vertices = GetVerts(true);
				targets[i].mesh.uv = uvs;	//GetUVs(true);

				if ( recalcnorms )
					targets[i].mesh.RecalculateNormals();

				if ( recalcbounds )
					targets[i].mesh.RecalculateBounds();
			}
		}
	}

	void Reset()
	{
		for ( int i = 0; i < targets.Count; i++ )
		{
			if ( targets[i].cachedMesh == null )
				targets[i].cachedMesh = (Mesh)Mesh.Instantiate(GetMesh(targets[i].go));
		}

		BuildList();
		GroupReStart1(true);
	}

	void Update()
	{
		if ( !DoLateUpdate )
			GroupModify();
	}

	void LateUpdate()
	{
		if ( DoLateUpdate )
			GroupModify();
	}

	void SetTarget(MegaModifierTarget target)
	{
		InitVertSource();
	}

	// TODO: Gizmo/bounds box is for whole group now
	public void GroupModify()
	{
		//print("groupmod");
		if ( Enabled && mods != null )
		{
			dirtyChannels = MegaModChannel.None;

			//if ( GrabVerts )
			//{
				//if ( sverts.Length < mesh.vertexCount )
					//sverts = new Vector3[mesh.vertexCount];

				//verts = mesh.vertices;
			//}

			int um = UpdateMesh;

			modContext.mod = this;

			foreach ( MegaModifierTarget target in targets )
			{
				if ( target.go )
				{
					modContext.Offset = target.Offset;
					modContext.bbox = target.bbox;

					SetTarget(target);

					UpdateMesh = um;

					foreach ( MegaModifier mod in mods )
					{
						if ( mod != null && mod.ModEnabled )
						{
							if ( (mod.ChannelsReq() & MegaModChannel.Verts) != 0 )	// should be changed
							{
								Debug.Log("set verts");
								mod.verts = GetSourceVerts(target);
								mod.sverts = GetDestVerts(target);
							}

							// Setting up the context basically
							mod.Offset = target.Offset;
							mod.bbox = target.bbox;

							mod.SetTM(mod.Offset);
							//if ( mod.ModLateUpdate(this) )
							if ( mod.ModLateUpdate(modContext) )
							{
								//verts = GetSourceVerts(target);
								//sverts = GetDestVerts(target);

								if ( UpdateMesh < 1 )
								{
									Debug.Log("a");
									mod.Modify(this);	//sverts, verts);
									UpdateMesh = 1;
								}
								else
								{
									//Debug.Log("b");
									mod.Modify(this);	//sverts, verts);
								}

								dirtyChannels |= mod.ChannelsChanged();
								mod.ModEnd(this);
							}
						}
					}

					if ( UpdateMesh == 1 )
					{
						SetMesh(target, ref target.sverts);
						UpdateMesh = 0;
					}
					else
					{
						if ( UpdateMesh == 0 )
						{
							SetMesh(target, ref target.verts);
							UpdateMesh = -1;	// Dont need to set verts again until a mod is enabled
						}
					}
				}
			}
		}
	}

	public void SetMesh(MegaModifierTarget target, ref Vector3[] _verts)
	{
		if ( (dirtyChannels & MegaModChannel.Verts) != 0 )
			target.mesh.vertices = _verts;	// mesh.vertices = GetVerts(true);

		if ( (dirtyChannels & MegaModChannel.UV) != 0 )
			target.mesh.uv = suvs;	//GetUVs(true);

		if ( recalcnorms )
			target.mesh.RecalculateNormals();

		if ( recalcTangents )
			BuildTangents(target);

		if ( recalcbounds )
			target.mesh.RecalculateBounds();

		if ( recalcCollider )
		{
			if ( target.meshCol == null )
				target.meshCol = GetComponent<MeshCollider>();

			if ( target.meshCol != null )
			{
				target.meshCol.sharedMesh = null;
				target.meshCol.sharedMesh = target.mesh;
				//bool con = meshCol.convex;
				//meshCol.convex = con;
			}
		}
	}

	// Plop into modifiertarget class
	void BuildTangents(MegaModifierTarget target)
	{
		if ( uvs == null )
			return;

		int triangleCount = target.mesh.triangles.Length;
		int vertexCount = target.mesh.vertices.Length;

		if ( target.tan1 == null || target.tan1.Length != vertexCount )
			target.tan1 = new Vector3[vertexCount];

		if ( target.tan2 == null || target.tan2.Length != vertexCount )
			target.tan2 = new Vector3[vertexCount];

		if ( target.tangents == null || target.tangents.Length != vertexCount )
			target.tangents = new Vector4[vertexCount];

		Vector3[] norms	= target.mesh.normals;
		int[]			tris	= target.mesh.triangles;

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

			target.tan1[i1] += sdir;
			target.tan1[i2] += sdir;
			target.tan1[i3] += sdir;

			target.tan2[i1] += tdir;
			target.tan2[i2] += tdir;
			target.tan2[i3] += tdir;
		}

		for ( int a = 0; a < vertexCount; a++ )
		{
			Vector3 n = norms[a];
			Vector3 t = target.tan1[a];

			Vector3.OrthoNormalize(ref n, ref t);
			target.tangents[a].x = t.x;
			target.tangents[a].y = t.y;
			target.tangents[a].z = t.z;
			target.tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), target.tan2[a]) < 0.0f) ? -1.0f : 1.0f;
		}

		target.mesh.tangents = target.tangents;
	}

	Mesh GetMesh(GameObject go)
	{
		if ( go )
		{
			MeshFilter filter = (MeshFilter)go.GetComponentInChildren<MeshFilter>();

			if ( filter != null )
				return filter.sharedMesh;

			SkinnedMeshRenderer skin = (SkinnedMeshRenderer)go.GetComponentInChildren<SkinnedMeshRenderer>();
			
			if ( skin != null )
				return skin.sharedMesh;
		}

		return null;
	}

	Mesh GetMesh1(GameObject go)
	{
		if ( go )
		{
			MeshFilter filter = (MeshFilter)go.GetComponentInChildren<MeshFilter>();

			if ( filter != null )
				return filter.mesh;

			SkinnedMeshRenderer skin = (SkinnedMeshRenderer)go.GetComponentInChildren<SkinnedMeshRenderer>();
			if ( skin != null )
				return skin.sharedMesh;
		}

		return null;
	}

	public void GroupReStart1(bool force)
	{
		for ( int i = 0; i < targets.Count; i++ )
		{
			if ( force || targets[i].mesh == null )
				targets[i].mesh = GetMesh1(targets[i].go);

			// Do we use mesh anymore
			if ( targets[i].mesh != null )	// was mesh
			{
				bbox = targets[i].cachedMesh.bounds;	// Need to expand box

				targets[i].sverts = new Vector3[targets[i].cachedMesh.vertexCount];
				targets[i].verts = targets[i].cachedMesh.vertices;

				targets[i].uvs = targets[i].cachedMesh.uv;
				targets[i].suvs = new Vector2[targets[i].cachedMesh.uv.Length];
			}
		}

		// common to modobj
		mods = GetComponents<MegaModifier>();

		Array.Sort(mods, CompareOrder);

		foreach ( MegaModifier mod in mods )
		{
			if ( mod != null )
			{
				mod.ModStart(this);	// Some mods like push error if we dont do this, put in error check and disable 
			}
		}

		UpdateMesh = -1;
	}

	// We may need to recalc bound box every frame
	// Need add and remove targets
	public void AddTarget(GameObject go)
	{
	}

	public void RemoveTarget(GameObject go)
	{
	}

	// Need to put bbox into modcontext not use modifier one
	void Start()
	{
		Transform[] objs = (Transform[])gameObject.GetComponentsInChildren<Transform>(true);

		targets = new List<MegaModifierTarget>(objs.Length);

		for ( int i = 0; i < objs.Length; i++ )
		{
			MegaModifierTarget target = new MegaModifierTarget();
			target.go = objs[i].gameObject;

			Mesh ms = GetMesh(target.go);

			if ( ms != null )
			{
				target.cachedMesh = (Mesh)Mesh.Instantiate(ms);
			}

			targets.Add(target);
		}

		GroupReStart1(false);
	}


	[ContextMenu("Recalc Bounds")]
	public void TestBounds()
	{
		for ( int i = 0; i < mods.Length; i++ )
		{
			GroupModReset(mods[i]);
		}
	}

	// Needs to be an override for ModReset
	public void GroupModReset(MegaModifier m)
	{
		if ( m != null )
		{
			int i;

			for ( i = 0; i < targets.Count; i++ )
			{
				GameObject targ = targets[i].go;

				Bounds b = new Bounds();

				Mesh cm = targets[i].cachedMesh;
				if ( cm != null )
					b = cm.bounds;

				for ( int t = 0; t < targets.Count; t++ )
				{
					Mesh ms = targets[t].cachedMesh;

					if ( t != i && ms != null )
					{
						Vector3 pos = targets[t].go.transform.position;
						pos = targ.transform.InverseTransformPoint(pos);

						Bounds mb = new Bounds(pos, targets[t].cachedMesh.bounds.size);
						b.Encapsulate(mb);
					}
				}

				targets[i].bbox.min = b.min;
				targets[i].bbox.max = b.max;
				targets[i].Offset = -b.center;
			}
		}
	}

	void OnDrawGizmosSelected()
	{
		modContext.mod = this;

		if ( GlobalDisplay && DrawGizmos && Enabled )
		{
			foreach ( MegaModifierTarget target in targets )
			{
				modContext.Offset = target.Offset;
				modContext.bbox = target.bbox;
				modContext.go = target.go;

				foreach ( MegaModifier mod in mods )
				{
					if ( mod != null )
					{
						if ( mod.ModEnabled && mod.DisplayGizmo )
							mod.DrawGizmo(modContext);
					}
				}
			}
		}
	}
}
#endif