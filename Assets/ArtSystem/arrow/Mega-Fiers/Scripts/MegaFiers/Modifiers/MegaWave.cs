
using UnityEngine;

[AddComponentMenu("Modifiers/Wave")]
public class MegaWave : MegaModifier
{
	public float	amp		= 0.0f;
	public float	amp2	= 0.0f;
	public float	flex	= 1.0f;
	public float	wave	= 1.0f;
	public float	phase	= 0.0f;
	public float	Decay	= 0.0f;
	public float	dir		= 0.0f;
	public bool		animate	= false;
	public float	Speed	= 1.0f;
	public int		divs = 4;
	public int		numSegs = 4;
	public int		numSides = 4;
	float time	= 0.0f;
	float dy	= 0.0f;
	float dist	= 0.0f;

	public override string ModName() { return "Wave"; }
	public override string GetHelpURL() { return "?page_id=357"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		//if ( dist == 0.0f )
			//return p;
		
		p = tm.MultiplyPoint3x4(p);

		float u = Mathf.Abs(2.0f * p.x / dist);
		u = u * u;
		p.z += flex * MegaUtils.WaveFunc(p.y, time, amp * (1.0f - u) + amp2 * u, wave, phase, dy);
		return invtm.MultiplyPoint3x4(p);
	}

	float t = 0.0f;	// TODO: put t in modifier and have as general anim value, maybe even modifiers as a context

	Matrix4x4 mat = new Matrix4x4();

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		if ( animate )
		{
			t += Time.deltaTime * Speed;
			phase = t;
		}
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		mat = Matrix4x4.identity;

		MegaMatrix.RotateZ(ref mat, Mathf.Deg2Rad * dir);
		SetAxis(mat);

		dy = Decay / 1000.0f;

		dist = (wave / 10.0f) * 4.0f * 5.0f;	//float(numSides);

		if ( dist == 0.0f )
			dist = 1.0f;

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

		for ( int i = 0; i <= numSides; i++ )
		{
			pos.x   = startx + Dx * (float)i;
			float u   = Mathf.Abs(pos.x / ((den != 0) ? den : 0.00001f));
			u   = u * u;

			for ( int j = 0; j <= numSegs; j++ )
			{
				pos.y = starty + (float)j * Dy;
				pos.z = MegaUtils.WaveFunc(pos.y, t, amp * (1.0f - u) + amp2 * u, wave, phase, Decay / 1000.0f);

				if ( j > 0 )
					Gizmos.DrawLine(last, pos);

				last = pos;
			}
		}

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

	public override void DrawGizmo(MegaModContext context)
	{
		Gizmos.color = Color.yellow;
		Matrix4x4 gtm = Matrix4x4.identity;
		Vector3 pos = gizmoPos;
		pos.x = -pos.x;
		pos.y = -pos.y;
		pos.z = -pos.z;

		Vector3 scl = gizmoScale;
		scl.x = 1.0f - (scl.x - 1.0f);
		scl.y = 1.0f - (scl.y - 1.0f);
		gtm.SetTRS(pos, Quaternion.Euler(gizmoRot), scl);

		//if ( context.sourceObj != null )
			//Gizmos.matrix = context.sourceObj.transform.localToWorldMatrix * gtm;
		//else
			//Gizmos.matrix = transform.localToWorldMatrix * gtm;

		Gizmos.matrix = transform.localToWorldMatrix * gtm;

		BuildMesh(t);
	}
}
