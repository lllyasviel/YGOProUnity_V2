using UnityEngine;

[AddComponentMenu("Modifiers/FFD/FFD 3x3x3")]
public class MegaFFD3x3x3 : MegaFFD
{
	public override string ModName() { return "FFD3x3x3"; }

	public override int GridSize()
	{
		return 3;
	}

	float BPoly3(int i, float u)
	{
		float s = 1.0f - u;

		switch ( i )
		{
			case 0: return s * s;
			case 1: return 2.0f * u * s;
			case 2: return u * u;
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

		for ( int i = 0; i < 3; i++ )
		{
			for ( int j = 0; j < 3; j++ )
			{
				for ( int k = 0; k < 3; k++ )
					q += pt[(i * 9) + (j * 3) + k] * BPoly3(i, pp.x) * BPoly3(j, pp.y) * BPoly3(k, pp.z);
			}
		}

		return invtm.MultiplyPoint3x4(q);
	}

	public override int GridIndex(int i, int j, int k)
	{
		return (i * 9) + (j * 3) + k;
	}
}
