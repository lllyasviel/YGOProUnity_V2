
using UnityEngine;
using System;
using System.Collections.Generic;

// TODO: flag in target to say whether to deform or not, also to say if deform position only
// Need a apply to group button, which builds a replica of the main stack on each target
[AddComponentMenu("Modifiers/Modify Object")]
[ExecuteInEditMode]
public class MegaModifyObject : MegaModifiers
{
	[HideInInspector]
	public Mesh cachedMesh;

	private static int CompareOrder(MegaModifier m1, MegaModifier m2)
	{
		return m1.Order - m2.Order;
	}

	[ContextMenu("Resort")]
	public virtual void Resort()
	{
		BuildList();
	}

	[ContextMenu("Help")]
	public virtual void Help()
	{
		Application.OpenURL("http://www.west-racing.com/mf/?page_id=444");
	}

	[ContextMenu("Reset Mesh Info")]
	public void ResetMeshInfo()
	{
		if ( cachedMesh == null )
			cachedMesh = (Mesh)Mesh.Instantiate(FindMesh(gameObject, out sourceObj));

		ReStart1(false);

		mesh.vertices = verts;	//_verts;	// mesh.vertices = GetVerts(true);
		mesh.uv = uvs;	//GetUVs(true);

		if ( recalcnorms )
		{
			RecalcNormals();
			//mesh.RecalculateNormals();
		}

		if ( recalcbounds )
			mesh.RecalculateBounds();
	}

	void Reset()
	{
		if ( cachedMesh == null )
			cachedMesh = (Mesh)Mesh.Instantiate(FindMesh(gameObject, out sourceObj));

		BuildList();
		ReStart1(true);
	}

	void Start()
	{
		if ( cachedMesh == null )
			cachedMesh = (Mesh)Mesh.Instantiate(FindMesh(gameObject, out sourceObj));

		ReStart1(false);
	}

	public void MeshUpdated()
	{
		cachedMesh = (Mesh)Mesh.Instantiate(FindMesh(gameObject, out sourceObj));

		ReStart1(true);

		foreach ( MegaModifier mod in mods )
			mod.SetModMesh(cachedMesh);
	}

	public void ReStart1(bool force)
	{
		if ( force || mesh == null )
			mesh = FindMesh1(gameObject, out sourceObj);	//Utils.GetMesh(gameObject);

		// Do we use mesh anymore
		if ( mesh != null )	// was mesh
		{
			bbox = cachedMesh.bounds;
			sverts = new Vector3[cachedMesh.vertexCount];
			verts = cachedMesh.vertices;

			uvs = cachedMesh.uv;
			suvs = new Vector2[cachedMesh.uv.Length];
			cols = cachedMesh.colors;

			//BuildNormalMapping(cachedMesh, false);
			mods = GetComponents<MegaModifier>();

			Array.Sort(mods, CompareOrder);

			foreach ( MegaModifier mod in mods )
			{
				if ( mod != null )
				{
					mod.ModStart(this);	// Some mods like push error if we dont do this, put in error check and disable 
				}
			}
		}

		UpdateMesh = -1;
	}

	public void ModReset(MegaModifier m)
	{
		if ( m != null )
		{
			m.SetModMesh(cachedMesh);
			BuildList();
		}
	}

#if false
	public void ChangeMesh1(Mesh ms)
	{
		BuildList();

		bbox = cachedMesh.bounds;
		sverts = new Vector3[cachedMesh.vertexCount];
		verts = cachedMesh.vertices;

		foreach ( MegaModifier mod in mods )
			mod.SetModMesh(cachedMesh);
	}
#endif

	// Check, do we need these?
	void Update()
	{
		if ( !DoLateUpdate )
		{
			//ModifyObject();
			ModifyObjectMT();
		}
	}

	void LateUpdate()
	{
		if ( DoLateUpdate )
		{
			//ModifyObject();
			ModifyObjectMT();
		}
	}

	public Mesh FindMesh(GameObject go, out GameObject obj)
	{
		if ( go )
		{
			MeshFilter[] filters = (MeshFilter[])go.GetComponentsInChildren<MeshFilter>(true);

			if ( filters.Length > 0 )
			{
				if ( filters[0].gameObject != go )
					obj = filters[0].gameObject;
				else
					obj = null;

				return filters[0].sharedMesh;
			}

			SkinnedMeshRenderer[] skins = (SkinnedMeshRenderer[])go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			if ( skins.Length > 0 )
			{
				if ( skins[0].gameObject != go )
					obj = skins[0].gameObject;
				else
					obj = null;

				return skins[0].sharedMesh;
			}
		}

		obj = null;
		return null;
	}

	public Mesh FindMesh1(GameObject go, out GameObject obj)
	{
		if ( go )
		{
			MeshFilter[] filters = (MeshFilter[])go.GetComponentsInChildren<MeshFilter>(true);

			if ( filters.Length > 0 )
			{
				if ( filters[0].gameObject != go )
					obj = filters[0].gameObject;
				else
					obj = null;
				return filters[0].mesh;
			}

			SkinnedMeshRenderer[] skins = (SkinnedMeshRenderer[])go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			if ( skins.Length > 0 )
			{
				if ( skins[0].gameObject != go )
					obj = skins[0].gameObject;
				else
					obj = null;
				return skins[0].sharedMesh;
			}
		}

		obj = null;
		return null;
	}

	void RestoreMesh(GameObject go, Mesh mesh)
	{
		if ( go )
		{
			MeshFilter[] filters = (MeshFilter[])go.GetComponentsInChildren<MeshFilter>(true);

			if ( filters.Length > 0 )
			{
				filters[0].sharedMesh = (Mesh)Instantiate(mesh);
				return;
			}
			SkinnedMeshRenderer[] skins = (SkinnedMeshRenderer[])go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			if ( skins.Length > 0 )
			{
				skins[0].sharedMesh = (Mesh)Instantiate(mesh);
				return;
			}
		}
	}
}
