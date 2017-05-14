
using UnityEngine;

[ExecuteInEditMode]
public class BallBounce : MonoBehaviour
{
	public float ground		= 0.0f;		// Ground position
	public float radius		= 1.0f;		// radius of the ball/object
	public float vel		= 0.0f;		// vertical velocity
	public float spring		= 10.0f;	// spring rate of the object
	public float py			= 1.0f;		// vertical position
	public float mass		= 1.0f;		// objects mass
	public float timescale	= 1.0f;		// time multiplier

	MegaStretch mod;
	MegaModifyObject modobj;

	// Simple physics timsetp
	void FixedUpdate()
	{
		float t = Time.fixedDeltaTime * timescale;
		float fy = -9.81f * t;

		if ( py < ground )
			fy += (spring * (ground - py)) / mass;

		vel += fy * t;
		py += vel * t;
	}

	void Update()
	{
		// Find the stretch mod
		if ( !mod )
		{
			mod = GetComponent<MegaStretch>();
			modobj = GetComponent<MegaModifyObject>();
		}

		if ( mod )
		{
			Vector3 pos = transform.position;

			float amt = py - ground;

			if ( amt > 0.0f )
				amt = 0.0f;

			float y = py;
			if ( y < ground )
				y = ground;

			if ( mod.amount == 0.0f && amt == 0.0f )
				modobj.enabled = false;
			else
				modobj.enabled = true;

			mod.amount = (amt / radius);
			pos.y = y;
			transform.position = pos;
		}
	}
}
// TODO: Signal to turn off after this pass, saves doing the check for 0