
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class MegaMorphChan //: ScriptableObject
{
	public string					mName			= "Empty";
	public float					Percent			= 0.0f;
	public bool						mActiveOverride = true;
	public bool						mUseLimit		= false;
	public float					mSpinmax		= 100.0f;
	public float					mSpinmin		= 0.0f;
	public Vector3[]				mDeltas;
	public float					mCurvature		= 0.5f;
	public bool						showparams		= true;
	public bool						showtargets		= true;
	public List<MegaMorphTarget>	mTargetCache;

	public MegaBezFloatKeyControl	control = null;
	public int[]	mapping;
	public bool		cubic;
	public int[]	morphedVerts;
	public Vector3[]	oPoints;

	float speed = 0.0f;
	float targetPercent = 0.0f;

	// for threaded version
	public float fChannelPercent;
	public int   targ;	//float lastPercent;
	public float fProgression;
	//public float alpha;
	public int segment;
	public Vector3[]	p1;
	public Vector3[]	p2;
	public Vector3[]	p3;
	public Vector3[]	p4;

	public Vector3[]	diff;

	public void SetTarget(float target, float spd)
	{
		speed = spd;
		targetPercent = target;
	}

	public void UpdatePercent()
	{
		if ( speed != 0.0f )
		{
			if ( Percent < targetPercent )
			{
				Percent += speed * Time.deltaTime;
				if ( Percent >= targetPercent )
				{
					Percent = targetPercent;
					speed = 0.0f;
				}
			}
			else
			{
				Percent -= speed * Time.deltaTime;
				if ( Percent <= targetPercent )
				{
					Percent = targetPercent;
					speed = 0.0f;
				}
			}
		}
	}

	public float GetTargetPercent(int which)
	{
		if ( which < -1 || which >= mTargetCache.Count ) return 0.0f;
		if ( which == -1 ) return mTargetCache[0].percent;	//mTargetPercent;
		return mTargetCache[which + 1].percent;
	}

	// needs to check each vert, if all targets equal opoint then unmorphed so remove 
	public void CompressChannel()
	{

	}

	public void ResetPercent()
	{
		// Make a function to reset percents
		int num = mTargetCache.Count;

		for ( int i = 0; i < mTargetCache.Count; i++ )
		{
			mTargetCache[i].percent = ((float)(i + 1) / (float)num) * 100.0f;
		}
	}

	// delta is only used if we have a single target
	public void Rebuild(MegaMorph mp)
	{
		if ( mTargetCache != null && mTargetCache.Count > 0 && mp.oPoints != null && mTargetCache[0].points != null )
		{
			if ( mp.oPoints.Length == mTargetCache[0].points.Length )
			{
				//Debug.Log("oplen " + mp.oPoints.Length + " tclen " + mTargetCache[0].points.Length);
				mDeltas = new Vector3[mp.oPoints.Length];
				for ( int i = 0; i < mTargetCache[0].points.Length; i++ )
				{
					mDeltas[i] = (mTargetCache[0].points[i] - mp.oPoints[i]) / 100.0f;
				}
			}
		}
	}

	// This is for new morpher
#if false
	public void Rebuild(MegaMorphOMatic mp, Vector3[] op)
	{
		if ( mTargetCache != null && mTargetCache.Count > 0 && op != null && mTargetCache[0].points != null )
		{
			if ( mp.oPoints.Length == mTargetCache[0].points.Length )
			{
				//Debug.Log("oplen " + mp.oPoints.Length + " tclen " + mTargetCache[0].points.Length);
				mDeltas = new Vector3[op.Length];
				for ( int i = 0; i < mTargetCache[0].points.Length; i++ )
				{
					mDeltas[i] = (mTargetCache[0].points[i] - op[i]) / 100.0f;
				}
			}
		}
	}
#endif

	public MegaMorphTarget GetTarget(string name)
	{
		if ( mTargetCache == null )
			return null;

		for ( int i = 0; i < mTargetCache.Count; i++ )
		{
			if ( mTargetCache[i].name == name )
				return mTargetCache[i];
		}

		return null;
	}

	public void ChannelMapping(MegaMorph mr)
	{
		mapping = new int[mr.oPoints.Length];

		for ( int i = 0; i < mr.oPoints.Length; i++ )
		{
			mapping[i] = i;
		}
	}
}

#if false
[System.Serializable]
public class MegaMorphChannel : MegaMorphChan
{
}

[System.Serializable]
public class MegaMOMChan : MegaMorphChan
{
}
#endif
