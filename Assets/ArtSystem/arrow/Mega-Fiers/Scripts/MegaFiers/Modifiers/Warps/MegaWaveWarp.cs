
using UnityEngine;

[AddComponentMenu("Modifiers/Warps/Wave")]
public class MegaWaveWarp : MegaWarp
{
	public float	amp			= 0.0f;
	public float	amp2		= 0.0f;
	public float	flex		= 1.0f;
	public float	wave		= 1.0f;
	public float	phase		= 0.0f;
	public bool		animate		= false;
	public float	Speed		= 1.0f;
	public int		divs		= 4;
	public int		numSegs		= 4;
	public int		numSides	= 4;
	float time = 0.0f;
	float dy = 0.0f;
	float dist = 0.0f;
	float t = 0.0f;	// TODO: put t in modifier and have as general anim value, maybe even modifiers as a context

	public override string WarpName() { return "Wave"; }
	public override string GetIcon() { return "MegaWave icon.png"; }
	public override string GetHelpURL() { return "?page_id=380"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		Vector3 ip = p;
		float dist = p.magnitude;
		float dcy = Mathf.Exp(-totaldecay * Mathf.Abs(dist));

		float u = Mathf.Abs(2.0f * p.x / dist);
		u = u * u;
		p.z += flex * MegaUtils.WaveFunc(p.y, time, amp * (1.0f - u) + amp2 * u, wave, phase, dy);

		p = Vector3.Lerp(ip, p, dcy);

		return invtm.MultiplyPoint3x4(p);
	}

	void Update()
	{
		if ( animate )
		{
			t += Time.deltaTime * Speed;
			phase = t;
		}
	}

	public override bool Prepare(float decay)
	{
		tm = transform.worldToLocalMatrix;
		invtm = tm.inverse;

		dist = (wave / 10.0f) * 4.0f * 5.0f;

		if ( dist == 0.0f )
			dist = 1.0f;

		dy = Decay / 1000.0f;

		totaldecay = dy + decay;
		if ( totaldecay < 0.0f )
			totaldecay = 0.0f;

		return true;
	}

	void BuildMesh(float t)
	{
		Vector3 pos = Vector3.zero;
		Vector3 last = Vector3.zero;

		float Dy     = wave / (float)divs;
		float Dx     = Dy * 4;
		int den = (int)(Dx * numSides * 0.5f);
		float starty = -(float)numSegs / 2.0f * Dy;
		float startx = -(float)numSides / 2.0f * Dx;

		Gizmos.color = gCol1;

		for ( int i = 0; i <= numSides; i++ )
		{
			pos.x = startx + Dx * (float)i;
			float u   = Mathf.Abs(pos.x / ((den != 0) ? den : 0.00001f));
			u = u * u;

			for ( int j = 0; j <= numSegs; j++ )
			{
				pos.y = starty + (float)j * Dy;
				pos.z = MegaUtils.WaveFunc(pos.y, t, amp * (1.0f - u) + amp2 * u, wave, phase, Decay / 1000.0f);

				if ( j > 0 )
					Gizmos.DrawLine(last, pos);

				last = pos;
			}
		}

		Gizmos.color = gCol2;

		for ( int j = 0; j <= numSegs; j++ )
		{
			pos.y = starty + (float)j * Dy;

			for ( int i = 0; i <= numSides; i++ )
			{
				pos.x = startx + Dx * (float)i;
				float u   = Mathf.Abs(pos.x / ((den != 0) ? den : 0.00001f));
				u = u * u;
				pos.z = MegaUtils.WaveFunc(pos.y, t, amp * (1.0f - u) + amp2 * u, wave, phase, Decay / 1000.0f);

				if ( i > 0 )
					Gizmos.DrawLine(last, pos);

				last = pos;
			}
		}
	}

	public override void DrawGizmo(Color col)
	{
		SetGizCols(col.a);
		tm = Matrix4x4.identity;
		invtm = tm.inverse;

		if ( !Prepare(0.0f) )
			return;

		tm = tm * transform.localToWorldMatrix;	// * tm;
		invtm = tm.inverse;

		//Gizmos.color = col;
		Gizmos.matrix = transform.localToWorldMatrix;

		BuildMesh(t);
	}
}