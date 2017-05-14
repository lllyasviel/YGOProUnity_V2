
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MOMVert
{
	public int		id;
	public float	w;
	public Vector3	start;
	public Vector3	delta;
}

[System.Serializable]
public class MegaMomVertMap
{
	public int[]				indices;
}

[AddComponentMenu("Modifiers/Morph-O-Matic")]
public class MegaMorphOMatic : MegaMorphBase
{
	public bool					UseLimit;
	public float				Max;
	public float				Min;
	public float				importScale	= 1.0f;
	public bool					flipyz		= false;
	public bool					negx		= false;
	public bool					glUseLimit	= false;
	public float				glMin		= 0.0f;
	public float				glMax		= 1.0f;
	public float				tolerance	= 0.0001f;
	public bool					animate		= false;
	public float				atime		= 0.0f;
	public float				animtime	= 0.0f;
	public float				looptime	= 0.0f;
	public MegaRepeatMode		repeatMode	= MegaRepeatMode.Loop;
	public float				speed		= 1.0f;

	public Vector3[]			oPoints;	// Base points

	public MegaMomVertMap[]		mapping;

	public override string ModName()	{ return "Morph-O-Matic"; }
	public override string GetHelpURL() { return "?page_id=1521"; }

	public override bool ModLateUpdate(MegaModContext mc)
	{
		if ( animate )
		{
			animtime += Time.deltaTime * speed;

			switch ( repeatMode )
			{
				case MegaRepeatMode.Loop:	animtime = Mathf.Repeat(animtime, looptime); break;
				case MegaRepeatMode.Clamp:	animtime = Mathf.Clamp(animtime, 0.0f, looptime); break;
			}
			SetAnim(animtime);
		}

		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		if ( chanBank != null && chanBank.Count > 0 )
			return true;

		return false;
	}

	Vector3 Cubic(MegaMorphTarget t, int pointnum, float alpha)
	{
		// Linear for now, will have coefs in here for cubic
		Vector3 v = t.mompoints[pointnum].delta;
		v.x *= alpha;
		v.y *= alpha;
		v.z *= alpha;
		return v;
	}

	static public void Bez3D(out Vector3 b, ref Vector3[] p, float u)
	{
		Vector3 t01 = p[0] + (p[1] - p[0]) * u;
		Vector3 t12 = p[1] + (p[2] - p[1]) * u;
		Vector3 t02 = t01 + (t12 - t01) * u;

		t01 = p[2] + (p[3] - p[2]) * u;

		Vector3 t13 = t12 + (t01 - t12) * u;

		b = t02 + (t13 - t02) * u;
	}

	// We should just modify the internal points then map them out at the end
	public override void Modify(MegaModifiers mc)
	{
		verts.CopyTo(sverts, 0);	// This should only blit totally untouched verts

		for ( int i = 0; i < chanBank.Count; i++ )
		{
			MegaMorphChan chan = chanBank[i];
			chan.UpdatePercent();

			float fChannelPercent = chan.Percent;

			// check for change since last frame on percent, if none just add in diff
			// Can we keep each chan delta then if not changed can just add it in
			if ( fChannelPercent == chan.fChannelPercent )
			{
				MegaMorphTarget trg = chan.mTargetCache[chan.targ];

				for ( int pointnum = 0; pointnum < trg.mompoints.Length; pointnum++ )
				{
					int p = trg.mompoints[pointnum].id;
					int c = mapping[p].indices.Length;

					Vector3 df = chan.diff[pointnum];

					for ( int m = 0; m < c; m++ )
					{
						int index = mapping[p].indices[m];
						sverts[index].x += df.x;
						sverts[index].y += df.y;
						sverts[index].z += df.z;
					}
				}
			}
			else
			{
				chan.fChannelPercent = fChannelPercent;

				if ( chan.mTargetCache != null && chan.mTargetCache.Count > 0 && chan.mActiveOverride )
				{
					if ( chan.mUseLimit || glUseLimit )
					{
						if ( glUseLimit )
							fChannelPercent = Mathf.Clamp(fChannelPercent, glMin, glMax);
						else
							fChannelPercent = Mathf.Clamp(fChannelPercent, chan.mSpinmin, chan.mSpinmax);
					}

					int targ = 0;
					float alpha = 0.0f;

					if ( fChannelPercent < chan.mTargetCache[0].percent )
					{
						targ = 0;
						alpha = 0.0f;
					}
					else
					{
						int last = chan.mTargetCache.Count - 1;
						if ( fChannelPercent >= chan.mTargetCache[last].percent )
						{
							targ = last - 1;
							alpha = 1.0f;
						}
						else
						{
							for ( int t = 1; t < chan.mTargetCache.Count; t++ )
							{
								if ( fChannelPercent < chan.mTargetCache[t].percent )
								{
									targ = t - 1;
									alpha = (fChannelPercent - chan.mTargetCache[targ].percent) / (chan.mTargetCache[t].percent - chan.mTargetCache[targ].percent);
									break;
								}
							}
						}
					}

					MegaMorphTarget trg = chan.mTargetCache[targ];
					chan.targ = targ;
#if false
					if ( chan.cubic )
					{
						for ( int pointnum = 0; pointnum < trg.mompoints.Length; pointnum++ )
						{
							int p = trg.mompoints[pointnum].id;
							int c = mapping[p].indices.Length;

							Vector3 df = trg.mompoints[pointnum].start;
							Vector3 cu = Cubic(trg, pointnum, alpha);

							df.x += cu.x * alpha;
							df.y += cu.y * alpha;
							df.z += cu.z * alpha;

							chan.diff[pointnum] = df;

							for ( int m = 0; m < c; m++ )
							{
								int index = mapping[p].indices[m];
								sverts[index].x += df.x;
								sverts[index].y += df.y;
								sverts[index].z += df.z;
							}
						}
					}
					else
#endif
					{
						for ( int pointnum = 0; pointnum < trg.mompoints.Length; pointnum++ )
						{
							int p = trg.mompoints[pointnum].id;

							// Save so if chan doesnt change we dont need to recalc
							Vector3 df = trg.mompoints[pointnum].start;

							df.x += trg.mompoints[pointnum].delta.x * alpha;
							df.y += trg.mompoints[pointnum].delta.y * alpha;
							df.z += trg.mompoints[pointnum].delta.z * alpha;

							chan.diff[pointnum] = df;

							for ( int m = 0; m < mapping[p].indices.Length; m++ )
							{
								int index = mapping[p].indices[m];
								sverts[index].x += df.x;
								sverts[index].y += df.y;
								sverts[index].z += df.z;
							}
						}
					}
				}
			}
		}
	}

	// Threaded version
	public override void PrepareMT(MegaModifiers mc, int cores)
	{
	}

	public override void DoWork(MegaModifiers mc, int index, int start, int end, int cores)
	{
		if ( index == 0 )
			Modify(mc);
	}

#if false
	public void PrepareForMT(MegaModifiers mc, int cores)
	{
		return;
		verts.CopyTo(sverts, 0);	// This should only blit totally untouched verts

		for ( int i = 0; i < chanBank.Count; i++ )
		{
			MegaMorphChan chan = chanBank[i];
			chan.UpdatePercent();

			float fChannelPercent = chan.Percent;

			// check for change since last frame on percent, if none just add in diff
			// Can we keep each chan delta then if not changed can just add it in
			if ( fChannelPercent != chan.fChannelPercent )
			{
				chan.fProgression = fChannelPercent;

				if ( chan.mTargetCache != null && chan.mTargetCache.Count > 0 && chan.mActiveOverride )
				{
					if ( chan.mUseLimit || glUseLimit )
					{
						if ( glUseLimit )
							fChannelPercent = Mathf.Clamp(fChannelPercent, glMin, glMax);
						else
							fChannelPercent = Mathf.Clamp(fChannelPercent, chan.mSpinmin, chan.mSpinmax);
					}

					int targ = 0;
					chan.alpha = 0.0f;

					if ( fChannelPercent < chan.mTargetCache[0].percent )
					{
						targ = 0;
						chan.alpha = 0.0f;	// need to calc correct alpha here, use first gap
					}
					else
					{
						int last = chan.mTargetCache.Count - 1;
						if ( fChannelPercent >= chan.mTargetCache[last].percent )
						{
							targ = last - 1;
							chan.alpha = 1.0f;	// need to calc correct alpha here, use first gap
						}
						else
						{
							for ( int t = 1; t < chan.mTargetCache.Count; t++ )
							{
								if ( fChannelPercent < chan.mTargetCache[t].percent )
								{
									targ = t - 1;
									chan.alpha = (fChannelPercent - chan.mTargetCache[targ].percent) / (chan.mTargetCache[t].percent - chan.mTargetCache[targ].percent);
									break;
								}
							}
						}
					}

					chan.targ = targ;
				}
			}
		}
	}

	static readonly object _locker = new object();

	public void ModifyMT(MegaModifiers mc, int tindex, int cores)
	{
		if ( tindex > 0 )
			return;

		Modify(mc);
		return;

		for ( int i = 0; i < chanBank.Count; i++ )
		{
			MegaMorphChan chan = chanBank[i];

			// check for change since last frame on percent, if none just add in diff
			// Can we keep each chan delta then if not changed can just add it in
			if ( chan.fProgression == chan.fChannelPercent )
			{
				MegaMorphTarget trg = chan.mTargetCache[chan.targ];

				int startvert = 0;	//(tindex * step);
				int endvert = trg.mompoints.Length;	//startvert + step;

				for ( int pointnum = startvert; pointnum < endvert; pointnum++ )
				{
					int p = trg.mompoints[pointnum].id;
					int c = mapping[p].indices.Length;

					Vector3 df = chan.diff[pointnum];

					for ( int m = 0; m < c; m++ )
					{
						int ix = mapping[p].indices[m];
						sverts[ix].x += df.x;
						sverts[ix].y += df.y;
						sverts[ix].z += df.z;
					}
				}
			}
			else
			{
				chan.fChannelPercent = chan.fProgression;
				float fChannelPercent = chan.fChannelPercent;

				if ( chan.mTargetCache != null && chan.mTargetCache.Count > 0 && chan.mActiveOverride )
				{
					MegaMorphTarget trg = chan.mTargetCache[chan.targ];

					//int step = trg.mompoints.Length / cores;
					int startvert = 0;	//(tindex * step);
					int endvert = trg.mompoints.Length;	//startvert + step;

					//if ( tindex == cores - 1 )
					//	endvert = trg.mompoints.Length;

					// So should we update channels then add them all up?
					if ( chan.cubic )
					{
						for ( int pointnum = startvert; pointnum < endvert; pointnum++ )
						{
							int p = trg.mompoints[pointnum].id;
							int c = mapping[p].indices.Length;

							Vector3 df = trg.mompoints[pointnum].start;
							Vector3 cu = Cubic(trg, pointnum, chan.alpha);

							df.x += cu.x * chan.alpha;
							df.y += cu.y * chan.alpha;
							df.z += cu.z * chan.alpha;

							chan.diff[pointnum] = df;

							for ( int m = 0; m < c; m++ )
							{
								int ix = mapping[p].indices[m];
								sverts[ix].x += df.x;
								sverts[ix].y += df.y;
								sverts[ix].z += df.z;
							}
						}
					}
					else
					{
						for ( int pointnum = startvert; pointnum < endvert; pointnum++ )
						{
							int p = trg.mompoints[pointnum].id;
							int c = mapping[p].indices.Length;

							// Save so if chan doesnt change we dont need to recalc
							Vector3 df = trg.mompoints[pointnum].start;

							df.x += trg.mompoints[pointnum].delta.x * chan.alpha;
							df.y += trg.mompoints[pointnum].delta.y * chan.alpha;
							df.z += trg.mompoints[pointnum].delta.z * chan.alpha;

							chan.diff[pointnum] = df;

							for ( int m = 0; m < c; m++ )
							{
								int ix = mapping[p].indices[m];
								sverts[ix].x += df.x;
								sverts[ix].y += df.y;
								sverts[ix].z += df.z;
							}
						}
					}
				}
			}
		}
	}
#endif
}
