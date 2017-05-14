using UnityEngine;
using System.IO;

[AddComponentMenu("Modifiers/Ripple")]
public class MegaRipple : MegaModifier
{
	public float	amp			= 0.0f;
	public float	amp2		= 0.0f;
	public float	flex		= 1.0f;
	public float	wave		= 1.0f;
	public float	phase		= 0.0f;
	public float	Decay		= 0.0f;
	public bool		animate		= false;
	public float	Speed		= 1.0f;
	public float	radius		= 1.0f;
	public int		segments	= 10;
	public int		circles		= 4;
	float time	= 0.0f;
	float dy	= 0.0f;

	public override string ModName() { return "Ripple"; }
	public override string GetHelpURL() { return "?page_id=308"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);
		float a;
		
		if ( amp != amp2 )
		{
			float len  = p.magnitude;
			if ( len == 0.0f )
				a = amp;
			else
			{
				float u = (Mathf.Acos(p.x / len)) / Mathf.PI;
		 		u = (u > 0.5f) ? (1.0f - u) : u;
				u *= 2.0f;
		 		//u = u*u;
		 		u = Mathf.SmoothStep(0.0f, 1.0f, u);
	 			a = amp * (1.0f - u) + amp2 * u;
			}
		}
		else
			a = amp;
	
		float oldZ = p.y;
		p.y = 0.0f;
		float r = p.magnitude;
		p.y = oldZ + flex * MegaUtils.WaveFunc(r, time, a, wave, phase, dy);
		return invtm.MultiplyPoint3x4(p);
	}

	float t = 0.0f;

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		if ( animate )
		{
			float dt = Time.deltaTime;
			if ( dt == 0.0f )
				dt = 0.01f;
			t += dt * Speed;
			phase = t;
		}
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		dy = Decay / 1000.0f;

		return true;
	}

	Vector3 GetPos(float u, float radius)
	{
		Vector3 pos = Vector3.zero;

		pos.x = radius * Mathf.Cos(u * Mathf.PI * 2.0f);
		pos.z = radius * Mathf.Sin(u * Mathf.PI * 2.0f);

		float u2 = (u > 0.5f) ? (u - 0.5f) : u;
		u2 = (u2 > 0.25f) ? (0.5f - u2) : u2;
		u2 = u2 * 4.0f;
		u2 = u2 * u2;
		pos.y = MegaUtils.WaveFunc(radius, t, amp * (1.0f - u2) + amp2 * u2, wave, phase, dy);

		return pos;
	}

	void MakeCircle(float t, float radius, float r1, float a1, float a2, float w, float s, float d, int numCircleSegs)
	{
		Vector3 last = Vector3.zero;
		Vector3 pos = Vector3.zero;
		Vector3 pos1 = Vector3.zero;
		Vector3 first = Vector3.zero;

		for ( int i = 0; i < numCircleSegs; i++ )
		{
			float u = (float)i / (float)numCircleSegs;
			pos = GetPos(u, radius);
			pos1 = GetPos(u, r1);

			if ( (i & 1) == 1 )
			{
				Gizmos.color = gizCol1;
			}
			else
				Gizmos.color = gizCol2;

			if ( i > 0 )
			{
				Gizmos.DrawLine(last, pos);
			}
			else
				first = pos;

			Gizmos.DrawLine(pos1, pos);

			last = pos;
		}

		Gizmos.DrawLine(last, first);
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

		float r1 = 0.0f;
		for ( int i = 0; i < circles; i++ )
		{
			float r = ((float)i / (float)circles) * radius;
			MakeCircle(t, r, r1, amp, amp2, wave, phase, dy, segments);
			r1 = r;
		}
	}
}
