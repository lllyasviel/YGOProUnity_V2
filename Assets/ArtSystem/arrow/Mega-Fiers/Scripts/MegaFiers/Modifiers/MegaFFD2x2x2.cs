
using UnityEngine;

[AddComponentMenu("Modifiers/FFD/FFD 2x2x2")]
public class MegaFFD2x2x2 : MegaFFD
{
	public override string ModName() { return "FFD2x2x2"; }

	public override int GridSize()
	{
		return 2;
	}

	float BPoly2(int i, float u)
	{
		switch ( i )
		{
			case 0: return 1.0f - u;
			case 1: return u;
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
				if ( pp[i] < -EPSILON || pp[i] > 1.0f + EPSILON )
					return p;
			}
		}

		for ( int i = 0; i < 2; i++ )
		{
			for ( int j = 0; j < 2; j++ )
			{
				for ( int k = 0; k < 2; k++ )
					q += pt[(i * 4) + (j * 2) + k] * BPoly2(i, pp.x) * BPoly2(j, pp.y) * BPoly2(k, pp.z);
			}
		}

		return invtm.MultiplyPoint3x4(q);
	}

	public override int GridIndex(int i, int j, int k)
	{
		return (i * 4) + (j * 2) + k;
	}
}