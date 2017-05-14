
using UnityEngine;

[AddComponentMenu("Modifiers/FFD/FFD 4x4x4")]
public class MegaFFD4x4x4 : MegaFFD
{
	public override string ModName() { return "FFD4x4x4"; }

	public override int GridSize()
	{
		return 4;
	}

	float BPoly4(int i, float u)
	{
		float s = 1.0f - u;

		switch ( i )
		{
			case 0: return s * s * s;
			case 1: return 3.0f * u * s * s;
			case 2: return 3.0f * u * u * s;
			case 3: return u * u * u;
			default: return 0.0f;
		}
	}

	public override Vector3 Map(int ii, Vector3 p)
	{
		q = Vector3.zero;

		pp = tm.MultiplyPoint3x4(p);

		if ( inVol )
		{
			for ( int i = 0; i < 3; i++ )
			{
				if ( pp[i] < -EPSILON || pp[i] > 1.0f + EPSILON)
					return p;
			}
		}

		for ( int i = 0; i < 4; i++ )
		{
			for ( int j = 0; j < 4; j++ )
			{
				for ( int k = 0; k < 4; k++ )
					q += pt[(i << 4) + (j << 2) + k] * BPoly4(i, pp.x) * BPoly4(j, pp.y) * BPoly4(k, pp.z);
			}
		}
		
		return invtm.MultiplyPoint3x4(q);
	}

	public override int GridIndex(int i, int j, int k)
	{
		return (i << 4) + (j << 2) + k;
	}
}