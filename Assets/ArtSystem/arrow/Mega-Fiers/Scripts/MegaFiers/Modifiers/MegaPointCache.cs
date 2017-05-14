
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class MegaPCVert
{
	public int[]		indices;
	public Vector3[]	points;
}

public enum MegaInterpMethod
{
	None,
	Linear,
	Bez,
}

public enum MegaBlendAnimMode
{
	Replace,
	Additive,
}

[AddComponentMenu("Modifiers/Point Cache")]
public class MegaPointCache : MegaModifier
{
	public float			time		= 0.0f;
	public bool				animated	= false;
	public float			speed		= 1.0f;
	public float			maxtime		= 1.0f;
	public MegaRepeatMode	LoopMode	= MegaRepeatMode.PingPong;
	public MegaInterpMethod	interpMethod = MegaInterpMethod.Linear;
	public MegaPCVert[]	Verts;
	public float weight = 1.0f;

	public MegaBlendAnimMode	blendMode = MegaBlendAnimMode.Additive;	// local space

	int     numPoints;            // Number of points per sample
	float   startFrame;           // Corresponds to the UI value of the same name.
	float   sampleRate;           // Corresponds to the UI value of the same name.
	int     numSamples;           // Defines how many samples are stored in the file.
	float	t;
	float	alpha = 0.0f;
	float	dalpha = 0.0f;
	int		sindex;

	public override string ModName()	{ return "Point Cache"; }
	public override string GetHelpURL() { return "?page_id=1335"; }

	void LinearAbs(MegaModifiers mc, int start, int end)
	{
		for ( int i = start; i < end; i++ )
		{
			Vector3 p = Verts[i].points[sindex];
			Vector3 p1 = Verts[i].points[sindex + 1];
			p.x = p.x + ((p1.x - p.x) * dalpha);
			p.y = p.y + ((p1.y - p.y) * dalpha);
			p.z = p.z + ((p1.z - p.z) * dalpha);

			for ( int v = 0; v < Verts[i].indices.Length; v++ )
				sverts[Verts[i].indices[v]] = p;
		}
	}

	void LinearAbsWeighted(MegaModifiers mc, int start, int end)
	{
		//Vector3[] vts = mc.GetSourceVerts();

		//int wc = (int)weightChannel;

		for ( int i = start; i < end; i++ )
		{
			Vector3 p = Verts[i].points[sindex];
			Vector3 p1 = Verts[i].points[sindex + 1];
			p.x = p.x + ((p1.x - p.x) * dalpha);
			p.y = p.y + ((p1.y - p.y) * dalpha);
			p.z = p.z + ((p1.z - p.z) * dalpha);

			//float w = mc.cols[Verts[i].indices[0]][wc];
			float w = mc.selection[Verts[i].indices[0]];	//[wc];
			p1 = verts[Verts[i].indices[0]];

			p = p1 + ((p - p1) * w);
			for ( int v = 0; v < Verts[i].indices.Length; v++ )
				sverts[Verts[i].indices[v]] = p;
		}
	}

	void LinearRel(MegaModifiers mc, int start, int end)
	{
		//Vector3[] vts = mc.GetSourceVerts();

		for ( int i = start; i < end; i++ )
		{
			int ix = Verts[i].indices[0];

			Vector3 basep = mc.verts[ix];

			Vector3 p = Verts[i].points[sindex];
			Vector3 p1 = Verts[i].points[sindex + 1];
			p.x += (((p1.x - p.x) * dalpha) - basep.x);	// * weight;	//mc.verts[ix].x;
			p.y += (((p1.y - p.y) * dalpha) - basep.y);	// * weight;	//mc.verts[ix].y;
			p.z += (((p1.z - p.z) * dalpha) - basep.z);	// * weight;	//mc.verts[ix].z;

			//p.x *= weight;
			//p.y *= weight;
			//p.z *= weight;

			p1 = verts[Verts[i].indices[0]];

			p.x = p1.x + (p.x * weight);
			p.y = p1.y + (p.y * weight);
			p.z = p1.z + (p.z * weight);

			for ( int v = 0; v < Verts[i].indices.Length; v++ )
			{
				int idx = Verts[i].indices[v];
				sverts[idx] = p;
				//mc.sverts[idx].x = vts[idx].x + p.x;
				//mc.sverts[idx].y = vts[idx].y + p.y;
				//mc.sverts[idx].z = vts[idx].z + p.z;
			}
		}
	}

	void LinearRelWeighted(MegaModifiers mc, int start, int end)
	{
		//Vector3[] vts = mc.GetSourceVerts();

		//int wc = (int)weightChannel;

		for ( int i = start; i < end; i++ )
		{
			int ix = Verts[i].indices[0];

			Vector3 basep = verts[ix];

			Vector3 p = Verts[i].points[sindex];
			Vector3 p1 = Verts[i].points[sindex + 1];
			p.x += (((p1.x - p.x) * dalpha) - basep.x);	// * weight;	//mc.verts[ix].x;
			p.y += (((p1.y - p.y) * dalpha) - basep.y);	// * weight;	//mc.verts[ix].y;
			p.z += (((p1.z - p.z) * dalpha) - basep.z);	// * weight;	//mc.verts[ix].z;

			//float w = mc.cols[Verts[i].indices[0]][wc] * weight;
			float w = mc.selection[Verts[i].indices[0]] * weight;	//[wc];

			//p.x *= w;
			//p.y *= w;
			//p.z *= w;

			p1 = verts[Verts[i].indices[0]];

			p.x = p1.x + (p.x * w);
			p.y = p1.y + (p.y * w);
			p.z = p1.z + (p.z * w);

			for ( int v = 0; v < Verts[i].indices.Length; v++ )
			{
				int idx = Verts[i].indices[v];
				sverts[idx] = p;
				//mc.sverts[idx].x = vts[idx].x + p.x;	// Optimize this
				//mc.sverts[idx].y = vts[idx].y + p.y;
				//mc.sverts[idx].z = vts[idx].z + p.z;
			}
		}
	}

	void NoInterpAbs(MegaModifiers mc, int start, int end)
	{
		for ( int i = start; i < end; i++ )
		{
			Vector3 p = Verts[i].points[sindex];

			for ( int v = 0; v < Verts[i].indices.Length; v++ )
				sverts[Verts[i].indices[v]] = p;
		}
	}

	void NoInterpAbsWeighted(MegaModifiers mc, int start, int end)
	{
		//Vector3[] vts = mc.GetSourceVerts();
		//int wc = (int)weightChannel;

		for ( int i = start; i < end; i++ )
		{
			Vector3 p = Verts[i].points[sindex];

			//float w = mc.cols[Verts[i].indices[0]][wc] * weight;
			float w = mc.selection[Verts[i].indices[0]] * weight;	//[wc];

			Vector3 p1 = verts[Verts[i].indices[0]];

			p = p1 + ((p - p1) * w);

			for ( int v = 0; v < Verts[i].indices.Length; v++ )
				sverts[Verts[i].indices[v]] = p;
		}
	}

	void NoInterpRel(MegaModifiers mc, int start, int end)
	{
		//Vector3[] vts = mc.GetSourceVerts();

		for ( int i = start; i < end; i++ )
		{
			int ix = Verts[i].indices[0];
			Vector3 p = Verts[i].points[sindex] - verts[ix];

			Vector3 p1 = verts[Verts[i].indices[0]];

			//p1 = vts[Verts[i].indices[0]];

			p.x = p1.x + (p.x * weight);
			p.y = p1.y + (p.y * weight);
			p.z = p1.z + (p.z * weight);

			for ( int v = 0; v < Verts[i].indices.Length; v++ )
			{
				int idx = Verts[i].indices[v];
				//Vector3 pp = vts[idx];
				sverts[idx] = p;
				//mc.sverts[idx].x = pp.x + p.x;
				//mc.sverts[idx].y = pp.y + p.y;
				//mc.sverts[idx].z = pp.z + p.z;
			}
		}
	}

	void NoInterpRelWeighted(MegaModifiers mc, int start, int end)
	{
		//Vector3[] vts = mc.GetSourceVerts();
		//int wc = (int)weightChannel;

		for ( int i = start; i < end; i++ )
		{
			int ix = Verts[i].indices[0];
			Vector3 p = Verts[i].points[sindex] - verts[ix];

			//float w = mc.cols[Verts[i].indices[0]][wc] * weight;
			float w = mc.selection[Verts[i].indices[0]] * weight;	//[wc];

			Vector3 p1 = verts[Verts[i].indices[0]];

			p = p1 + ((p - p1) * w);

			for ( int v = 0; v < Verts[i].indices.Length; v++ )
			{
				int idx = Verts[i].indices[v];
				//Vector3 pp = vts[idx];
				sverts[idx] = p;
				//mc.sverts[idx].x = pp.x + p.x;
				//mc.sverts[idx].y = pp.y + p.y;
				//mc.sverts[idx].z = pp.z + p.z;
			}
		}
	}

	// TODO: Option to lerp or even bez, depends on how many samples
	public override void Modify(MegaModifiers mc)
	{
		if ( Verts != null )
		{
			switch ( interpMethod )
			{
				case MegaInterpMethod.Linear:
					switch ( blendMode )
					{
						case MegaBlendAnimMode.Additive:	LinearRel(mc, 0, Verts.Length);	break;
						case MegaBlendAnimMode.Replace:		LinearAbs(mc, 0, Verts.Length);	break;
					}
					break;

				case MegaInterpMethod.Bez:
					switch ( blendMode )
					{
						case MegaBlendAnimMode.Additive:	LinearRel(mc, 0, Verts.Length); break;
						case MegaBlendAnimMode.Replace:		LinearAbs(mc, 0, Verts.Length); break;
					}
					break;

				case MegaInterpMethod.None:
					switch ( blendMode )
					{
						case MegaBlendAnimMode.Additive:	NoInterpRel(mc, 0, Verts.Length); break;
						case MegaBlendAnimMode.Replace:		NoInterpAbs(mc, 0, Verts.Length); break;
					}
					break;
			}
		}
		else
		{
			//Vector3[]	verts = mc.GetSourceVerts();
			//Vector3[]	sverts = mc.GetDestVerts();

			for ( int i = 0; i < verts.Length; i++ )
				sverts[i] = verts[i];
		}
	}

	public override bool ModLateUpdate(MegaModContext mc)
	{
		if ( !Prepare(mc) )
			return false;

		if ( animated )
			time += Time.deltaTime * speed;

		switch ( LoopMode )
		{
			case MegaRepeatMode.Loop:		t = Mathf.Repeat(time, maxtime); break;
			case MegaRepeatMode.PingPong:	t = Mathf.PingPong(time, maxtime); break;
			case MegaRepeatMode.Clamp:		t = Mathf.Clamp(time, 0.0f, maxtime); break;
		}

		alpha = t / maxtime;

		float val = (float)(Verts[0].points.Length - 1) * alpha;

		sindex = (int)val;

		dalpha = val - sindex;

		//Debug.Log("t " + t + " alpha " + alpha + " sindex " + sindex + " dalpha " + dalpha + " verts " + Verts[0].points.Length);

		return true;
	}

	public override bool Prepare(MegaModContext mc)
	{
		if ( Verts != null && Verts.Length > 0 && Verts[0].indices != null && Verts[0].indices.Length > 0 )
			return true;

		return false;
	}

	public override void DoWork(MegaModifiers mc, int index, int start, int end, int cores)
	{
		ModifyCompressedMT(mc, index, cores);
	}

	public void ModifyCompressedMT(MegaModifiers mc, int tindex, int cores)
	{
		if ( Verts != null )
		{
			int step = Verts.Length / cores;
			int startvert = (tindex * step);
			int endvert = startvert + step;

			if ( tindex == cores - 1 )
				endvert = Verts.Length;

			switch ( interpMethod )
			{
				case MegaInterpMethod.Linear:
					switch ( blendMode )
					{
						case MegaBlendAnimMode.Additive:	LinearRel(mc, startvert, endvert); break;
						case MegaBlendAnimMode.Replace:		LinearAbs(mc, startvert, endvert); break;
					}
					break;

				case MegaInterpMethod.Bez:
					switch ( blendMode )
					{
						case MegaBlendAnimMode.Additive:	LinearRel(mc, startvert, endvert); break;
						case MegaBlendAnimMode.Replace:		LinearAbs(mc, startvert, endvert); break;
					}
					break;

				case MegaInterpMethod.None:
					switch ( blendMode )
					{
						case MegaBlendAnimMode.Additive:	NoInterpRel(mc, startvert, endvert); break;
						case MegaBlendAnimMode.Replace:		NoInterpAbs(mc, startvert, endvert); break;
					}
					break;
			}
		}
	}
}
