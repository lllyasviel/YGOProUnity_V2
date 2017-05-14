
using UnityEngine;
using System.Collections.Generic;

public enum MegaRubberType
{
	Custom,
	SoftRubber,
	HardRubber,
	Jelly,
	SoftLatex
}

[System.Serializable]
public class VertexRubber
{
	public Vector3	pos;
	public Vector3	cpos;
	public Vector3	force;
	public Vector3	acc;
	public Vector3	vel;
	public int[]	indices;
	public float	weight;
	public float	stiff;

	public VertexRubber(Vector3 v_target, float w, float s)	{ pos = v_target; weight = w; stiff = s; }
}

// Maybe rename to flex?
[AddComponentMenu("Modifiers/Rubber")]
public class MegaRubber : MegaModifier
{
	public override string ModName()	{ return "Rubber"; }
	public override string GetHelpURL() { return "?page_id=1254"; }

	// From other system
	public MegaRubberType		Presets			= MegaRubberType.Custom;
	public MegaWeightChannel	channel			= MegaWeightChannel.Red;
	public MegaWeightChannel	stiffchannel	= MegaWeightChannel.None;
	public Vector3				Intensity		= Vector3.one;
	public float				gravity			= 0.0f;
	public float				mass			= 1.0f;
	public Vector3				stiffness		= new Vector3(0.2f, 0.2f, 0.2f);
	public Vector3				damping			= new Vector3(0.7f, 0.7f, 0.7f);
	public float				threshold		= 0.0f;
	public float				size			= 0.001f;
	public bool					showweights		= true;

	float						oomass;
	float						grav;
	bool						defined		= false;
	public VertexRubber[]		vr;
	int[]						notmoved;

	[ContextMenu("Reset Physics")]
	public void ResetPhysics()
	{
		MegaModifiers mod = GetComponent<MegaModifiers>();
		if ( mod )
			Init(mod);
	}

	int[] FindVerts(Vector3[] vts, Vector3 p)
	{
		List<int>	indices = new List<int>();
		for ( int i = 0; i < vts.Length; i++ )
		{
			if ( p.x == vts[i].x && p.y == vts[i].y && p.z == vts[i].z )
				indices.Add(i);
		}
		return indices.ToArray();
	}

	bool HavePoint(Vector3[] vts, List<int> points, Vector3 p)
	{
		for ( int i = 0; i < points.Count; i++ )
		{
			if ( p.x == vts[points[i]].x && p.y == vts[points[i]].y && p.z == vts[points[i]].z )
				return true;
		}

		return false;
	}

	// remove dups
	void Init(MegaModifiers mod)
	{
		if ( mod.verts == null )
			return;

		List<int> noweights = new List<int>();
		List<int> ActiveVertex = new List<int>();

		int wc = (int)channel;

		for ( int i = 0; i < mod.verts.Length; i++ )
		{
			// Dont add if we have already
			if ( channel == MegaWeightChannel.None || mod.cols == null || mod.cols.Length == 0 )
			{
				//if ( !HavePoint(mod.verts, ActiveVertex, mod.verts[i]) )
					//ActiveVertex.Add(i);
			}
			else
			{
				if ( mod.cols[i][wc] > threshold )
				{
					if ( !HavePoint(mod.verts, ActiveVertex, mod.verts[i]) )
						ActiveVertex.Add(i);
				}
				else
					noweights.Add(i);
			}
		}

		notmoved = noweights.ToArray();

		if ( ActiveVertex.Count > 0 )
		{
			vr = new VertexRubber[ActiveVertex.Count];

			for ( int i = 0; i < ActiveVertex.Count; i++ )
			{
				int ref_index = (int)ActiveVertex[i];

				float stiff = 1.0f;
				if ( stiffchannel != MegaWeightChannel.None && mod.cols != null && mod.cols.Length > 0 )
				{
					stiff = mod.cols[ref_index][(int)stiffchannel];
				}

				//if ( channel != MegaWeightChannel.None && mod.cols != null && mod.cols.Length > 0 )
				float intens = (mod.cols[ref_index][wc] - threshold) / (1.0f - threshold);

				vr[i] = new VertexRubber(transform.TransformPoint(mod.verts[ref_index]), intens, stiff);
				vr[i].indices = FindVerts(mod.verts, mod.verts[ref_index]);
			}
		}
		else
			vr = null;

		defined = true;
	}

	public override bool ModLateUpdate(MegaModContext mc)
	{
		if ( weightsChanged )
		{
			ResetPhysics();
			weightsChanged = false;
		}

		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		tm = transform.localToWorldMatrix;
		invtm = transform.worldToLocalMatrix;

		oomass = 1.0f / mass;
		grav = gravity * 0.1f;

		if ( !defined )
			Init(mc.mod);

		if ( vr != null )
			return true;

		return false;
	}

	public override void Modify(MegaModifiers mc)
	{
		UpdateVerts(0, vr.Length);
		for ( int i = 0; i < notmoved.Length; i++ )
			sverts[notmoved[i]] = verts[notmoved[i]];
	}

	void UpdateVerts(int start, int end)
	{
		Vector3 p = Vector3.zero;

		//float t = Time.deltaTime;

		for ( int i = start; i < end; i++ )
		{
			int ix = vr[i].indices[0];
			Vector3 vp = verts[ix];
			Vector3 v3_target = tm.MultiplyPoint(vp);

			VertexRubber v = vr[i];

			v.force.x = (v3_target.x - v.pos.x) * stiffness.x * v.stiff;
			v.acc.x = v.force.x * oomass;
			v.vel.x = damping.x * (v.vel.x + v.acc.x);
			v.pos.x += v.vel.x;	// * t;

			v.force.y = (v3_target.y - v.pos.y) * stiffness.y * v.stiff;
			v.force.y -= grav;
			v.acc.y = v.force.y * oomass;
			v.vel.y = damping.y * (v.vel.y + v.acc.y);
			v.pos.y += v.vel.y;	// * t;

			v.force.z = (v3_target.z - v.pos.z) * stiffness.z * v.stiff;
			v.acc.z = v.force.z * oomass;
			v.vel.z = damping.z * (v.vel.z + v.acc.z);
			v.pos.z += v.vel.z;	// * t;

			v3_target = invtm.MultiplyPoint(vr[i].pos);

			p.x = vp.x + ((v3_target.x - vp.x) * v.weight * Intensity.x);
			p.y = vp.y + ((v3_target.y - vp.y) * v.weight * Intensity.y);
			p.z = vp.z + ((v3_target.z - vp.z) * v.weight * Intensity.z);

			v.cpos = p;

			for ( int v1 = 0; v1 < vr[i].indices.Length; v1++ )
			{
				int ix1 = vr[i].indices[v1];
				sverts[ix1] = p;
			}
		}
	}

	public void ChangeMaterial()
	{
		switch ( Presets )
		{
			case MegaRubberType.HardRubber:
				gravity		= 0.0f;
				mass		= 8.0f;
				stiffness	= new Vector3(0.5f, 0.5f, 0.5f);
				damping		= new Vector3(0.9f, 0.9f, 0.9f);
				Intensity	= new Vector3(0.5f, 0.5f, 0.5f);
				break;

			case MegaRubberType.Jelly:
				gravity		= 0.0f;
				mass		= 1.0f;
				stiffness	= new Vector3(0.95f, 0.95f, 0.95f);
				damping		= new Vector3(0.95f, 0.95f, 0.95f);
				Intensity	= Vector3.one;
				break;

			case MegaRubberType.SoftRubber:
				gravity		= 0.0f;
				mass		= 2.0f;
				stiffness	= new Vector3(0.5f, 0.5f, 0.5f);
				damping		= new Vector3(0.85f, 0.85f, 0.85f);
				Intensity	= Vector3.one;
				break;

			case MegaRubberType.SoftLatex:
				gravity		= 1.0f;
				mass		= 0.9f;
				stiffness	= new Vector3(0.3f, 0.3f, 0.3f);
				damping		= new Vector3(0.25f, 0.25f, 0.25f);
				Intensity	= Vector3.one;
				break;
		}
	}

	public void ChangeChannel()
	{
		MegaModifiers mod = GetComponent<MegaModifiers>();
		if ( mod )
			Init(mod);
	}

	// Threaded
	public override void DoWork(MegaModifiers mc, int index, int start, int end, int cores)
	{
		ModifyCompressedMT(mc, index, cores);
	}

	public void ModifyCompressedMT(MegaModifiers mc, int tindex, int cores)
	{
		int step = notmoved.Length / cores;
		int startvert = (tindex * step);
		int endvert = startvert + step;

		if ( tindex == cores - 1 )
			endvert = notmoved.Length;

		if ( notmoved != null )
		{
			for ( int i = startvert; i < endvert; i++ )
			{
				int index = notmoved[i];
				sverts[index] = verts[index];
			}
		}

		step = vr.Length / cores;
		startvert = (tindex * step);
		endvert = startvert + step;

		if ( tindex == cores - 1 )
			endvert = vr.Length;

		UpdateVerts(startvert, endvert);
	}

	bool weightsChanged = false;

	MegaModifiers mods = null;

	MegaModifiers GetMod()
	{
		if ( mods == null )
			mods = GetComponent<MegaModifiers>();

		return mods;
	}

	public void UpdateCols(int first, Color[] newcols)
	{
		GetMod();

		if ( mods )
			mods.UpdateCols(first, newcols);

		weightsChanged = true;
	}

	public void UpdateCol(int i, Color col)
	{
		GetMod();

		if ( mods )
			mods.UpdateCol(i, col);

		weightsChanged = true;
	}

	public void UpdateCols(Color[] newcols)
	{
		GetMod();

		if ( mods )
			mods.UpdateCols(newcols);

		weightsChanged = true;
	}

#if false
	public void ChangeWeights(GameObject obj)
	{
		MegaModifyObject modobj = (MegaModifyObject)obj.GetComponent<MegaModifyObject>();
		MegaRubber mod = (MegaRubber)obj.GetComponent<MegaRubber>();

		modobj.UpdateCol(0, Color.red);
		mod.ResetPhysics();
	}
#endif
}
