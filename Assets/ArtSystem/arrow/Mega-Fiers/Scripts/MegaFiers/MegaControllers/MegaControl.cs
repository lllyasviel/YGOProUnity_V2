
using UnityEngine;

// Two kinds, a mono versions and one for loading to objects
[System.Serializable]
public class MegaControl
{
	//public float f;
	public float[]	Times;
	[HideInInspector]
	public int			lastkey = 0;
	[HideInInspector]
	public float		lasttime = 0.0f;

	public virtual float GetFloat(float time)	{ return 0.0f; }
	public virtual Vector3 GetVector3(float time) { return Vector3.zero; }

	int BinSearch(float t, int low, int high)
	{
		int	probe = 0;

		while ( high - low > 1 )
		{
			probe = (high + low) / 2;

			if ( t < Times[probe] )
				high = probe;
			else
			{
				if ( t > Times[probe + 1] )
					low = probe;
				else
					break;	// found
			}
		}

		return probe;
	}

	// get index
	// do a range check, anim code should keep the t in range
	public int GetKey(float t)
	{
		if ( t <= Times[1] )
			return 0;

		if ( t >= Times[Times.Length - 1] )
			return Times.Length - 2;

		// Cache result and then do a bin search
		int	key = lastkey;

		//Debug.Log("keys " + Times.Length + " key " + key);
		if ( t >= Times[key] && t < Times[key + 1] )
			return key;	// we get past this if out of time range of whole anim

		return BinSearch(t, -1, Times.Length - 1);
	}
}