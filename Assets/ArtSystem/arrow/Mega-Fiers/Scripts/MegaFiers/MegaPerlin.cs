using UnityEngine;

public class MegaPerlin
{
	private static MegaPerlin instance;

	public MegaPerlin()
	{
		if ( instance != null )
		{
			Debug.LogError("Cannot have two instances of ImprovedPerlin.");
			return;
		}

		instance = this;

		for ( int i = 0; i < 256; i++ )
			p[256 + i] = p[i] = permutation[i];
	}

	public static MegaPerlin Instance
	{
		get
		{
			if ( instance == null )
				new MegaPerlin();

			return instance;
		}
	}

	//int X,Y,Z = 0;
	//float u,v,w = 0.0f;
	//int A, AA, AB, B, BA, BB = 0;
	//float floorx, floory, floorz = 0.0f;
	static int[] p = new int[512];
	static int[] permutation = { 151,160,137,91,90,15,
	131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
	190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
	88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
	77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
	102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
	135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
	5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
	223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
	129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
	251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
	49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
	138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
	};
   
	public int Perm(int i)
	{
		return p[i & 255];
	}

	public float Noise(float x)
	{
		// Compute the cell coordinates
		int X = (int)Mathf.Floor(x) & 255;

		// Retrieve the decimal part of the cell
		x -= (float)Mathf.Floor(x);

		float u = fade(x);

		return lerp(u, grad(p[X], x), grad(p[X + 1], x - 1));
	}

	public float Noise(float x, float y)
	{
		// Compute the cell coordinates
		int X = (int)Mathf.Floor(x) & 255;
		int Y = (int)Mathf.Floor(y) & 255;

		// Retrieve the decimal part of the cell
		x -= (float)Mathf.Floor(x);
		y -= (float)Mathf.Floor(y);

		// Smooth the curve
		float u = fade(x);
		float v = fade(y);

		// Fetch some randoms values from the table
		int A = p[X] + Y;
		int B = p[X + 1] + Y;

		// Interpolate between directions
		return lerp(v, lerp(u, grad(p[A], x, y), grad(p[B], x - 1, y)), lerp(u, grad(p[A + 1], x, y - 1), grad(p[B + 1], x - 1, y - 1)));
	}

	public float Noise(float x, float y, float z)
	{
		// FIND UNIT CUBE THAT CONTAINS POINT
		float floorx = Mathf.Floor(x);
		float floory = Mathf.Floor(y);
		float floorz = Mathf.Floor(z);
		int X = (int)floorx & 255;
		int Y = (int)floory & 255;
		int Z = (int)floorz & 255;

		// FIND RELATIVE X,Y,Z OF POINT IN CUBE.
		x -= floorx;
		y -= floory;
		z -= floorz;

		// COMPUTE FADE CURVES FOR EACH X,Y,Z
		float u = fade(x);
		float v = fade(y);
		float w = fade(z);

		// HASH COORDINATES OF THE 8 CUBE CORNERS
		int A = p[X]+Y; 
		int AA = p[A]+Z; 
		int AB = p[A+1]+Z;      
		int B = p[X+1]+Y;
		int BA = p[B]+Z; 
		int BB = p[B+1]+Z;
	  
		// ADD BLENDED RESULTS FROM 8 CORNERS OF CUBE
		return lerp(w, lerp(v, lerp(u, grad(p[AA], x, y, z), grad(p[BA], x - 1, y, z)), lerp(u, grad(p[AB], x, y - 1, z), 
		grad(p[BB], x - 1, y - 1, z))), lerp(v, lerp(u, grad(p[AA + 1], x, y, z - 1), grad(p[BA + 1], x - 1 , y, z - 1)),
		lerp(u, grad(p[AB + 1], x, y - 1, z - 1), grad(p[BB + 1], x - 1 , y - 1 , z - 1))));
	}
   
	private float fade(float t) 
	{ 
		return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f); 
	}
	
	private float lerp(float t, float a, float b)
	{ 
		return a + t * (b - a); 
	}

	private static float grad(int hash, float x)
	{
		return (hash & 1) == 0 ? x : -x;
	}

	private float grad(int hash, float x, float y)
	{
		// Fetch the last 3 bits
		int h = hash & 3;
 
		float u = (h & 2) == 0 ? x : -x;
		float v = (h & 1) == 0 ? y : -y;
 
		return u + v;
	}

	private float grad(int hash, float x, float y, float z) 
	{
		int h = hash & 15;                     // CONVERT LO 4 BITS OF HASH CODE
		float u = h < 8 ? x : y;                 // INTO 12 GRADIENT DIRECTIONS.
		float v = h < 4 ? y : h == 12 || h == 14 ? x : z;
		return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0.0f ? v : -v);
	}

	public float fBm1(float x, float H, float lacunarity, float octaves)
	{
		float value = 0.0f;
		float frequency = 1.0f;

		for ( int i = 0; i < (int)octaves; i++ )
		{
			value = Noise(x) * Mathf.Pow(frequency, -H);
			x *= lacunarity;
			frequency *= lacunarity;
		}

		float remainder = octaves - (int)octaves;

		if ( remainder != 0.0f )
			value += remainder * Noise(x) * Mathf.Pow(frequency, -H);

		return value;
	}

	public float fBm1(Vector3 vertex, float H, float lacunarity, float octaves)
	{
		return fBm1(vertex.x, vertex.y, vertex.z, H, lacunarity, octaves);
	}

	public float fBm1(float x, float y, float z, float H, float lacunarity, float octaves)
	{
		float value = 0.0f;
		float frequency = 1.0f;

		for ( int i = 0; i < octaves; i++ )
		{
			value += Noise(x, y, z) * Mathf.Pow(frequency, -H);
			x *= lacunarity;
			y *= lacunarity;
			z *= lacunarity;

			frequency *= lacunarity;
		}

		float remainder = octaves - (int)octaves;
		if ( remainder != 0.0f )
			value += remainder * Noise(x, y, z) * Mathf.Pow(frequency, -H);

		return value;
	}
}
