
using UnityEngine;
using System.Collections.Generic;

// Should weights come into this?
// Put into SoftBody2D lib

// bary should be uv coords really
// Need a pin constraint, will pin a square, will find the bary for th epin inside the square

public enum MegaIntegrator
{
	Euler,
	Verlet,
	VerletTimeCorrected,
	MidPoint,
}

public class BaryVert2D
{
	public int		gx;	// Grid position
	public int		gy;
	public Vector2	bary;
}

[System.Serializable]
public class Constraint2D
{
	public bool		active;
	public int		p1;
	public int		p2;
	public float	length;
	public Vector2	pos;
	public int		contype = 0;
	public Transform obj;

	public static Constraint2D CreatePointTargetCon(int _p1, Transform trans)
	{
		Constraint2D con = new Constraint2D();
		con.p1 = _p1;
		con.active = true;
		con.contype = 2;
		con.obj = trans;

		return con;
	}

	public static Constraint2D CreateLenCon(int _p1, int _p2, float _len)
	{
		Constraint2D con = new Constraint2D();
		con.p1 = _p1;
		con.p2 = _p2;
		con.length = _len;
		con.active = true;
		con.contype = 0;

		return con;
	}

	public static Constraint2D CreatePointCon(int _p1, Vector2 _pos)
	{
		Constraint2D con = new Constraint2D();
		con.p1 = _p1;
		con.pos = _pos;
		con.active = true;
		con.contype = 1;

		return con;
	}

	public void Apply(MegaSoft2D soft)
	{
		switch ( contype )
		{
			case 0:	ApplyLengthConstraint2D(soft);	break;
			case 1: ApplyPointConstraint2D(soft); break;
			//case 2: ApplyPointTargetConstraint2D(soft); break;
		}
	}

	// Can have a one that has a range to keep in
	public void ApplyLengthConstraint2D(MegaSoft2D soft)
	{
		if ( active && soft.applyConstraints )
		{
			//calculate direction
			Vector2 direction = soft.masses[p2].pos - soft.masses[p1].pos;

			//calculate current length
			float currentLength = direction.magnitude;

			//check for zero vector
			//if ( direction != Vector3.zero )
			if ( currentLength != 0.0f )	//direction.x != 0.0f || direction.y != 0.0f || direction.y != 0.0f )
			{
				//normalize direction vector
				//direction.Normalize();
				direction.x /= currentLength;
				direction.y /= currentLength;

				//move to goal positions
				Vector2 moveVector = 0.5f * (currentLength - length) * direction;

				soft.masses[p1].pos.x += moveVector.x;
				soft.masses[p1].pos.y += moveVector.y;

				soft.masses[p2].pos.x += -moveVector.x;
				soft.masses[p2].pos.y += -moveVector.y;

				//soft.masses[p1].pos += moveVector;
				//soft.masses[p2].pos += -moveVector;
			}
		}
	}

	public void ApplyPointConstraint2D(MegaSoft2D soft)
	{
		if ( active )
			soft.masses[p1].pos = pos;
	}

	public void ApplyAngleConstraint(MegaSoft2D soft)
	{
		//Vector2 d1 = soft.masses[p1].pos - soft.masses[p1 + 1].pos;
		//Vector2 d2 = soft.masses[p1 + 2].pos - soft.masses[p1 + 1].pos;

		//float ang = Vector2.Dot(d1, d2);
	}

	//public void ApplyPointTargetConstraint2D(MegaSoft2D soft)
	//{
	//	if ( active && obj )
	//		soft.masses[p1].pos = tobj.position;
	//}

	// Semi rigid job
#if false
	public static void CreateSemiRigidCon(int _p1, int _p2, float min, float mid, float max, float force)
	{
		Constraint2D con = new Constraint2D();
		con.p1 = _p1;
		con.p2 = _p2;
		con.min = min;
		con.mid = min;
		con.max = min;
		con.force = min;
	}

	Vector2	GetForce(MegaSoft2D soft)	//CVerletPoint* p_verlet)
	{
		Vector2	to_me = soft.masses[p1].pos - soft.masses[p2].pos;
		
		// handle case where points are the same
		if ( to_me.magnitude < 0.000001)
		{
			to_me = new Vector2(1.0f, 0.0f);
		}

		Vector2	mid = soft.masses[p2].pos + to_me.normalized * mid;
		Vector2	to_mid = mid - soft.masses[p1].pos;

		return to_mid * force;  // --- ????
	}

	// apply this constraint to a verlet (usually the one that owns this constraint).
	void Satisfy(MegaSoft2D soft)
	{
		// Get vector from other verlet to me
		Vector2	to_me = soft.masses[p1].pos - soft.masses[p2].pos;
		Vector2	mid = (soft.masses[p1].pos + soft.masses[p2].pos) / 2.0f;
		// handle case where points are the same
		if ( to_me.magnitude == 0.0f )
		{
			to_me = new Vector2(1.0f, 0.0f);
		}
		float radius = to_me.magnitude;

		if ( radius < min) radius = min;
		if ( radius > max) radius = max;

		// Scale it to the required radius
		to_me = radius * to_me.normalized;
		// and set the point
		soft.masses[p2].pos = mid - to_me / 2.0f;
	}
#endif
}

#if false
//[System.Serializable]
public class LengthConstraint2D : Constraint2D
{
	//public int		p1;
	//public int		p2;
	//public float	length;

	public LengthConstraint2D(int _p1, int _p2, float _len)
	{
		p1 = _p1;
		p2 = _p2;
		length = _len;
		active = true;
	}

	public override void Apply(MegaSoft2D soft)
	{
		if ( active )
		{
			//calculate direction
			Vector2 direction = soft.masses[p2].pos - soft.masses[p1].pos;

			//calculate current length
			float currentLength = direction.magnitude;

			//check for zero vector
			//if ( direction != Vector3.zero )
			if ( direction.x != 0.0f || direction.y != 0.0f || direction.y != 0.0f )
			{
				//normalize direction vector
				direction.Normalize();

				//move to goal positions
				Vector2 moveVector = 0.5f * (currentLength - length) * direction;

				soft.masses[p1].pos.x += moveVector.x;
				soft.masses[p1].pos.y += moveVector.y;

				soft.masses[p2].pos.x += -moveVector.x;
				soft.masses[p2].pos.y += -moveVector.y;

				//soft.masses[p1].pos += moveVector;
				//soft.masses[p2].pos += -moveVector;
			}
		}
	}
}

#if false
[System.Serializable]
public class NewPointConstraint2D : Constraint2D
{
	public int			p1;
	public int			p2;
	public float		length;
	public Transform	obj;

	public NewPointConstraint2D(int _p1, int _p2, float _len, Transform trans)
	{
		p1 = _p1;
		p2 = _p2;
		length = _len;
		obj = trans;
		active = true;
	}

	public override void Apply(MegaSoft2D soft)
	{
		if ( active )
		{
			//calculate direction
			soft.masses[p1].pos = obj.position;
			Vector2 direction = soft.masses[p2].pos - soft.masses[p1].pos;

			//calculate current length
			float currentLength = direction.magnitude;

			//check for zero vector
			if ( direction != Vector2.zero )
			{
				//normalize direction vector
				direction.Normalize();

				//move to goal positions
				Vector2 moveVector = 1.0f * (currentLength - length) * direction;
				//soft.masses[p1].pos += moveVector;
				soft.masses[p2].pos += -moveVector;
			}
		}
	}
}
#endif

[System.Serializable]
public class PointConstraint2D : Constraint2D
{
	//public int		p1;
	//public Vector2	pos;

	public PointConstraint2D(int _p1, Vector2 _pos)
	{
		p1 = _p1;
		pos = _pos;
		active = true;
	}

	public override void Apply(MegaSoft2D soft)
	{
		if ( active )
			soft.masses[p1].pos = pos;
	}
}
#endif

[System.Serializable]
public class Mass2D
{
	public Vector2	pos;
	public Vector2	last;
	public Vector2	force;
	public Vector2	vel;
	public Vector2	posc;
	public Vector2	velc;
	public Vector2	forcec;
	public Vector2	coef1;
	public Vector2	coef2;
	public float	mass;
	public float	oneovermass;

	public Mass2D(float m, Vector2 p)
	{
		mass		= m;
		oneovermass	= 1.0f / mass;
		pos			= p;
		last		= p;
		force		= Vector2.zero;
		vel			= Vector2.zero;
	}
}

[System.Serializable]
public class Spring2D
{
	public int		p1;
	public int		p2;
	public float	restLen;
	public float	ks;
	public float	kd;
	public float	len;

	public Spring2D(int _p1, int _p2, float _ks, float _kd, MegaSoft2D mod)
	{
		p1 = _p1;
		p2 = _p2;
		ks = _ks;
		kd = _kd;
		restLen = Vector2.Distance(mod.masses[p1].pos, mod.masses[p2].pos);
		len = restLen;
	}

	// Should have a softbody lib
	public void doCalculateSpringForce(MegaSoft2D mod)
	{
		Vector2 deltaP = mod.masses[p1].pos - mod.masses[p2].pos;

		float dist = deltaP.magnitude;	//VectorLength(&deltaP); // Magnitude of deltaP

		float Hterm = (dist - restLen) * ks; // Ks * (dist - rest)

		Vector2	deltaV = mod.masses[p1].vel - mod.masses[p2].vel;
		float Dterm = (Vector2.Dot(deltaV, deltaP) * kd) / dist; // Damping Term

		Vector2 springForce = deltaP * (1.0f / dist);
		springForce *= -(Hterm + Dterm);

		mod.masses[p1].force += springForce;
		mod.masses[p2].force -= springForce;
	}

	public void doCalculateSpringForce1(MegaSoft2D mod)
	{
		//get the direction vector
		Vector2 direction = mod.masses[p1].pos - mod.masses[p2].pos;

		//check for zero vector
		if ( direction != Vector2.zero )
		{
			//get length
			float currLength = direction.magnitude;
			//normalize
			direction = direction.normalized;
			//add spring force
			Vector2 force = -ks * ((currLength - restLen) * direction);
			//add spring damping force
			//float v = (currLength - len) / mod.timeStep;

			//force += -kd * v * direction;
			//apply the equal and opposite forces to the objects
			mod.masses[p1].force += force;
			mod.masses[p2].force -= force;
			len = currLength;
		}
	}

	public void doCalculateSpringForce2(MegaSoft2D mod)
	{
		Vector2 deltaP = mod.masses[p1].pos - mod.masses[p2].pos;

		float dist = deltaP.magnitude;	//VectorLength(&deltaP); // Magnitude of deltaP

		float Hterm = (dist - restLen) * ks; // Ks * (dist - rest)

		//Vector2	deltaV = mod.masses[p1].vel - mod.masses[p2].vel;
		float v = (dist - len);	// / mod.timeStep;

		float Dterm = (v * kd) / dist; // Damping Term

		Vector2 springForce = deltaP * (1.0f / dist);
		springForce *= -(Hterm + Dterm);

		mod.masses[p1].force += springForce;
		mod.masses[p2].force -= springForce;
	}

}

// Want verlet for this as no collision will be done, solver type enum
// need to add contact forces for weights on bridge
[System.Serializable]
public class MegaSoft2D
{
	public List<Mass2D>			masses;
	public List<Spring2D>		springs;
	public List<Constraint2D>	constraints;

	public Vector2 gravity = new Vector2(0.0f, -9.81f);
	public float airdrag = 0.999f;
	public float friction = 1.0f;
	public float timeStep = 0.01f;
	public int	iters = 4;
	public MegaIntegrator	method = MegaIntegrator.Verlet;
	public bool	applyConstraints = true;

	void doCalculateForceseuler()
	{
		for ( int i = 0; i < masses.Count; i++ )
		{
			masses[i].force = masses[i].mass * gravity;
			//masses[i].force += -masses[i].vel * airdrag;	// Should be vel sqr
			masses[i].force += masses[i].forcec;
		}

		for ( int i = 0; i < springs.Count; i++ )
			springs[i].doCalculateSpringForce(this);
	}

	void doCalculateForces()
	{
		for ( int i = 0; i < masses.Count; i++ )
		{
			masses[i].force = masses[i].mass * gravity;
			//masses[i].force += -masses[i].vel * airdrag;	// Should be vel sqr
			masses[i].force += masses[i].forcec;
		}

		for ( int i = 0; i < springs.Count; i++ )
			springs[i].doCalculateSpringForce1(this);
	}

	void doIntegration1(float dt)
	{
		doCalculateForceseuler();	// Calculate forces, only changes _f

		/*	Then do correction step by integration with central average (Heun) */
		for ( int i = 0; i < masses.Count; i++ )
		{
			masses[i].last = masses[i].pos;
			masses[i].vel += dt * masses[i].force * masses[i].oneovermass;
			masses[i].pos += masses[i].vel * dt;
			masses[i].vel *= friction;
		}

		DoConstraints();
		//DoCollisions(dt);
	}

	public float floor = 0.0f;
	void DoCollisions(float dt)
	{
		for ( int i = 0; i < masses.Count; i++ )
		{
			if ( masses[i].pos.y < floor )
				masses[i].pos.y = floor;
		}
	}


	// Change the base code over to Skeel or similar
#if true

	//public bool UseVerlet = false;

	// Can do drag per point using a curve
	// perform the verlet integration step
	void VerletIntegrate(float t, float lastt)
	{
		doCalculateForces();	// Calculate forces, only changes _f

		float t2 = t * t;
		/*	Then do correction step by integration with central average (Heun) */
		for ( int i = 0; i < masses.Count; i++ )
		{
			Vector2 last = masses[i].pos;
			masses[i].pos += airdrag * (masses[i].pos - masses[i].last) + masses[i].force * masses[i].oneovermass * t2;	// * t;

			//masses[i].pos += airdrag * masses[i].pos - masses[i].last + masses[i].force * masses[i].oneovermass * t2;	// * t;

			//masses[i].pos = masses[i].pos + (masses[i].pos - masses[i].last) * (t / lastt) + masses[i].force * masses[i].oneovermass * t * t;
			masses[i].last = last;
		}

		DoConstraints();
		//DoCollisions(t);
	}

	// Pointless
	void VerletIntegrateTC(float t, float lastt)
	{
		doCalculateForces();	// Calculate forces, only changes _f

		float t2 = t * t;
		float dt = t / lastt;

		/*	Then do correction step by integration with central average (Heun) */
		for ( int i = 0; i < masses.Count; i++ )
		{
			Vector2 last = masses[i].pos;
			masses[i].pos += airdrag * (masses[i].pos - masses[i].last) * dt + (masses[i].force * masses[i].oneovermass) * t2;	// * t;

			//masses[i].pos += airdrag * masses[i].pos - masses[i].last + masses[i].force * masses[i].oneovermass * t2;	// * t;

			//masses[i].pos = masses[i].pos + (masses[i].pos - masses[i].last) * (t / lastt) + masses[i].force * masses[i].oneovermass * t * t;
			masses[i].last = last;
		}

		DoConstraints();
	}

	void MidPointIntegrate(float t)
	{
	}

	//void Vertlet(Mass2D mass, Vector2 vel)
	//{
	//	Vector2 newPosition = (2.0f - drag) * mass.pos - (1.0f - drag) * mass.prevpos + acceleration * fixedTimeStep * fixedTimeStep;
	//	simObject.PrevPosition = simObject.CurrPosition;
	//	simObject.CurrPosition = newPosition;
	//}
#endif
	// Satisfy constraints
	public void DoConstraints()
	{
		for ( int i = 0; i < iters; i++ )
		{
			for ( int c = 0; c < constraints.Count; c++ )
			{
				constraints[c].Apply(this);
			}
		}
	}

	public float lasttime = 0.0f;
	public float simtime = 0.0f;

	public void Update()
	{
		if ( masses == null )
			return;

		simtime += Time.deltaTime;	// * fudge;

		if ( Time.deltaTime == 0.0f )
		{
			simtime = 0.01f;
		}

		if ( timeStep <= 0.0f )
			timeStep = 0.001f;

		float ts = 0.0f;

		if ( lasttime == 0.0f )
			lasttime = timeStep;

		while ( simtime > 0.0f)	//timeStep )	//0.0f )
		{
			//if ( time > timeStep )
			//	ts = timeStep;
			//else
			//	ts = time;

			simtime -= timeStep;
			ts = timeStep;

			switch ( method )
			{
				case MegaIntegrator.Euler:
					doIntegration1(ts);
					break;

				case MegaIntegrator.Verlet:
					VerletIntegrate(ts, lasttime);	//timeStep);
					break;

				case MegaIntegrator.VerletTimeCorrected:
					VerletIntegrateTC(ts, lasttime);	//timeStep);
					break;

				case MegaIntegrator.MidPoint:
					MidPointIntegrate(ts);	//timeStep);
					break;
			}

			lasttime = ts;
		}
	}
}

#if false
public class MegaSoft2D : MegaModifier
{
	// Grid up on an axis and do 2d springs on that and translate that back to mesh
	// Going to need to pin bits

	public BaryVert2D[]			baryverts;

	public List<Mass2D>			masses;
	public List<Spring2D>		springs;
	public List<Constraint2D>	constraints;

	public int WidthSegs = 4;
	public int HeightSegs = 4;

	public float spring = 1.0f;
	public float damp = 0.0f;

	public MegaAxis	axis = MegaAxis.Z;	// Axis to remove

	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		tm = transform.localToWorldMatrix;
		invtm = transform.worldToLocalMatrix;

		return false;
	}

	public override void Modify(MegaModifiers mc)
	{
	}

	public Vector2	gravity		= new Vector3(0.0f, -9.81f);
	public float	airdrag		= 0.02f;
	public float	friction	= 0.99f;
	public int		iters		= 4;

	// Physics
	void doCalculateForces()
	{
		for ( int i = 0; i < masses.Count; i++ )
		{
			masses[i].force = masses[i].mass * gravity;
			masses[i].force += -masses[i].vel * airdrag;	// Should be vel sqr
		}

		for ( int i = 0; i < springs.Count; i++ )
			springs[i].doCalculateSpringForce(this);
	}

	void doIntegration1(float dt)
	{
		doCalculateForces();	// Calculate forces, only changes _f

		/*	Then do correction step by integration with central average (Heun) */
		for ( int i = 0; i < masses.Count; i++ )
		{
			masses[i].last = masses[i].pos;
			masses[i].vel += dt * masses[i].force * masses[i].oneovermass;
			masses[i].pos += masses[i].vel * dt;
			masses[i].vel *= friction;
		}

		DoConstraints();
		DoCollisions(dt);
	}

	public float		colRadius		= 0.1f;
	public LayerMask	layer;
	public float		floorfriction	= 0.9f;
	public float		bounce			= 1.0f;

	void DoCollisions(float dt)
	{
		RaycastHit hit;

		for ( int i = 0; i < masses.Count; i++ )
		{
			//Vector3 dir = masses[i].vel.normalized;
			Vector3 start = masses[i].last - (new Vector2(0.0f, -10.0f));	//.down * 10.0f);

			//Collider[] hits = Physics.OverlapSphere(masses[i].last, ropeRadius, layer);
			if ( Physics.CheckSphere(masses[i].last, colRadius, layer) )
			{
				//if ( Physics.SphereCast(masses[i].last, ropeRadius, dir, out hit, masses[i].vel.magnitude * dt, layer) )
				//if ( Physics.SphereCast(masses[i].last, ropeRadius, dir, out hit, masses[i].vel.magnitude * dt, layer) )
				//if ( Physics.SphereCast(start, ropeRadius, dir, out hit, (masses[i].vel.magnitude * dt) + 10.0f, layer) )
				if ( Physics.SphereCast(start, colRadius, Vector3.down, out hit, (masses[i].vel.magnitude * dt) + 20.0f, layer) )
				{
					if ( hit.distance < 10.0f )
					{
						masses[i].pos = hit.point + (hit.normal * (colRadius * 1.001f));
						Response(i, hit);
					}
				}
			}
		}
	}

	void Response(int i, RaycastHit hit)
	{
		// CALCULATE Vn
		float VdotN = Vector2.Dot(hit.normal, masses[i].vel);
		Vector2 Vn = hit.normal * VdotN;
		// CALCULATE Vt
		Vector2 Vt = (masses[i].vel - Vn) * floorfriction;
		// SCALE Vn BY COEFFICIENT OF RESTITUTION
		Vn *= bounce;
		// SET THE VELOCITY TO BE THE NEW IMPULSE
		masses[i].vel = Vt - Vn;
	}

	// Satisfy constraints
	void DoConstraints()
	{
		for ( int i = 0; i < iters; i++ )
		{
			for ( int c = 0; c < constraints.Count; c++ )
			{
				constraints[c].Apply(this);
			}
		}
	}

	bool CalcBary(Vector2 P, Vector2 A, Vector2 B, Vector2 C, ref Vector2 bary)
	{
		// Compute vectors        
		Vector2 v0 = C - A;
		Vector2 v1 = B - A;
		Vector2 v2 = P - A;

		// Compute dot products
		float dot00 = Vector2.Dot(v0, v0);
		float dot01 = Vector2.Dot(v0, v1);
		float dot02 = Vector2.Dot(v0, v2);
		float dot11 = Vector2.Dot(v1, v1);
		float dot12 = Vector2.Dot(v1, v2);

		// Compute barycentric coordinates
		float invDenom = 1.0f / (dot00 * dot11 - dot01 * dot01);
		bary.x = (dot11 * dot02 - dot01 * dot12) * invDenom;
		bary.y = (dot00 * dot12 - dot01 * dot02) * invDenom;

		// Check if point is in triangle
		return (bary.x > 0.0f) && (bary.y > 0.0f) && (bary.x + bary.y < 1.0f);
	}

	public float timeStep = 0.01f;

	void SoftBody2DUpdate(float t)
	{
		float time = Time.deltaTime;	// * fudge;

		while ( time > 0.0f )
		{
			time -= timeStep;
			doIntegration1(timeStep);
		}
	}

	public float	DampingRatio	= 0.25f;	// 1 being critically damped
	public float	Density			= 1.0f;
	public float	Stiff			= 0.5f;

	void InitFromMesh(Mesh shape)
	{
		int a1 = 0;
		int a2 = 1;

		switch ( axis )
		{
			case MegaAxis.X: a1 = 2; a2 = 1; break;
			case MegaAxis.Y: a1 = 0; a2 = 2; break;
			case MegaAxis.Z: a1 = 0; a2 = 1; break;
		}

		float area = shape.bounds.size.x * shape.bounds.size.y;
		Debug.Log("Area " + area + "m2");
		float totalmass = Density * area;

		// Option for fill or set count, or count per unit
		int nummasses = (WidthSegs + 1) * (HeightSegs + 1);
		Debug.Log("Num Masses " + nummasses);

		float m = totalmass / (float)nummasses;

		if ( DampingRatio > 1.0f )
			DampingRatio = 1.0f;

		damp = (DampingRatio * 0.45f) * (2.0f * Mathf.Sqrt(m * spring));

		// The Max spring rate is based on m
		//float dmpratio = damp / (2.0f * Mathf.Sqrt(m * spring));
		//Debug.Log("Current Damp Ratio " + dmpratio);

		//float dmp = DampingRatio * (2.0f * Mathf.Sqrt(m * spring));
		Debug.Log("TotalMass " + totalmass + "kg element mass " + m + "kg damp " + damp);

		if ( masses == null )
			masses = new List<Mass2D>();

		transform.position = Vector3.zero;

		masses.Clear();

		Vector2 mpos = Vector2.zero;

		for ( int y = 0; y < HeightSegs + 1; y++ )
		{
			for ( int x = 0; x < WidthSegs + 1; x++ )
			{
				float ax = (float)x / (float)WidthSegs;
				float ay = (float)y / (float)HeightSegs;

				mpos.x = shape.bounds.min[a1] + (shape.bounds.size[a1] * ax);
				mpos.y = shape.bounds.min[a2] + (shape.bounds.size[a2] * ay);
#if false
				switch ( axis )
				{
					case MegaAxis.X:
						mpos.x = shape.bounds.min.z + (shape.bounds.size.z * ax);
						mpos.y = shape.bounds.min.y + (shape.bounds.size.y * ay);
						break;

					case MegaAxis.Y:
						mpos.x = shape.bounds.min.x + (shape.bounds.size.x * ax);
						mpos.y = shape.bounds.min.z + (shape.bounds.size.z * ay);
						break;

					case MegaAxis.Z:
						mpos.x = shape.bounds.min.x + (shape.bounds.size.x * ax);
						mpos.y = shape.bounds.min.y + (shape.bounds.size.y * ay);
						break;
				}
#endif
				Mass2D rm = new Mass2D(m, mpos);
				masses.Add(rm);
			}
		}

		if ( springs == null )
			springs = new List<Spring2D>();

		springs.Clear();

		if ( constraints == null )
			constraints = new List<Constraint2D>();

		constraints.Clear();

		// For proper soft need voxelize, use color to define areas 
		// Do I need cross members
		// If an grid is empty of verts or faces then dont add
		// Horizontal springs
		for ( int y = 0; y < HeightSegs + 1; y++ )
		{
			int off = y * (WidthSegs + 1);

			for ( int x = 0; x < WidthSegs; x++ )
			{
				Spring2D spr = new Spring2D(off + x, off + x + 1, spring, damp, this);
				springs.Add(spr);

				LengthConstraint2D lcon = new LengthConstraint2D(off + x, off + x + 1, spr.restLen);
				constraints.Add(lcon);
			}
		}

		// Vertical springs
		for ( int x = 0; x < WidthSegs + 1; x++ )
		{
			for ( int y = 0; y < HeightSegs; y++ )
			{
				int off = y * (WidthSegs + 1);

				Spring2D spr = new Spring2D(off + x, (off + WidthSegs + 1) + x, spring, damp, this);
				springs.Add(spr);

				LengthConstraint2D lcon = new LengthConstraint2D(off + x, (off + WidthSegs + 1) + x, spr.restLen);
				constraints.Add(lcon);
			}
		}

		baryverts = new BaryVert2D[shape.vertexCount];

		Vector2 p2 = Vector2.zero;

		Vector2 corner = Vector2.zero;

		//float sx,sy;

		corner.x = shape.bounds.min[a1];
		corner.y = shape.bounds.min[a2];

		float sx = shape.bounds.size[a1] / (float)WidthSegs;
		float sy = shape.bounds.size[a2] / (float)HeightSegs;
#if false
		switch ( axis )
		{
			case MegaAxis.X:
				corner.x = shape.bounds.min.z;
				corner.y = shape.bounds.min.y;

				sx = shape.bounds.size.z / (float)WidthSegs;
				sy = shape.bounds.size.y / (float)HeightSegs;
				break;

			case MegaAxis.Y:
				corner.x = shape.bounds.min.x;
				corner.y = shape.bounds.min.z;

				sx = shape.bounds.size.x / (float)WidthSegs;
				sy = shape.bounds.size.z / (float)HeightSegs;
				break;

			case MegaAxis.Z:
				corner.x = shape.bounds.min.x;
				corner.y = shape.bounds.min.y;

				sx = shape.bounds.size.x / (float)WidthSegs;
				sy = shape.bounds.size.y / (float)HeightSegs;
				break;
		}
#endif
		Vector2 A = Vector2.zero;
		Vector2 B = Vector2.zero;
		Vector2 C = Vector2.zero;

		for ( int i = 0; i < shape.vertexCount; i++ )
		{
			Vector3 p = shape.vertices[i];

			baryverts[i].gx = (int)(((p[a1] - shape.bounds.min[a1]) / shape.bounds.size[a1]) * (float)WidthSegs);
			baryverts[i].gy = (int)(((p[a2] - shape.bounds.min[a2]) / shape.bounds.size[a2]) * (float)HeightSegs);

			p2.x = p[a1];
			p2.y = p[a2];
#if false
			switch ( axis )
			{
				case MegaAxis.X:
					baryverts[i].gx = (int)(((p.z - shape.bounds.min.z) / shape.bounds.size.z) * (float)WidthSegs);
					baryverts[i].gy = (int)(((p.y - shape.bounds.min.y) / shape.bounds.size.y) * (float)HeightSegs);

					p2.x = p.z;
					p2.y = p.y;
					break;

				case MegaAxis.Y:
					baryverts[i].gx = (int)(((p.x - shape.bounds.min.x) / shape.bounds.size.x) * (float)WidthSegs);
					baryverts[i].gy = (int)(((p.z - shape.bounds.min.z) / shape.bounds.size.z) * (float)HeightSegs);

					p2.x = p.x;
					p2.y = p.z;
					break;

				case MegaAxis.Z:
					baryverts[i].gx = (int)(((p.x - shape.bounds.min.x) / shape.bounds.size.x) * (float)WidthSegs);
					baryverts[i].gy = (int)(((p.y - shape.bounds.min.y) / shape.bounds.size.y) * (float)HeightSegs);

					p2.x = p.x;
					p2.y = p.y;
					break;
			}
#endif
			A.x = corner.x + ((float)baryverts[i].gx * sx);
			A.y = corner.y + ((float)baryverts[i].gy * sy);

			B.x = corner.x + ((float)(baryverts[i].gx + 1) * sx);
			B.y = corner.y + ((float)baryverts[i].gy * sy);

			C.x = corner.x + ((float)baryverts[i].gx * sx);
			C.y = corner.y + ((float)(baryverts[i].gy + 1) * sy);

			CalcBary(p2, A, B, C, ref baryverts[i].bary);
		}
		// Now need to make barycentric coords for each vertex

		//top.position = masses[0].pos;
		//bottom.position = masses[masses.Count - 1].pos;

		//float ln = (masses[0].pos - masses[1].pos).magnitude;
		//NewPointConstraint pconn = new NewPointConstraint(0, 1, ln, top.transform);
		//constraints.Add(pconn);

		//ln = (masses[masses.Count - 1].pos - masses[masses.Count - 2].pos).magnitude;
		//pconn = new NewPointConstraint(masses.Count - 1, masses.Count - 2, ln, bottom.transform);
		//constraints.Add(pconn);

		//ln = (masses[masses.Count - 1].pos - masses[masses.Count - 2].pos).magnitude;
		//NewPointConstraint pc = new NewPointConstraint(masses.Count / 2, (masses.Count / 2) + 1, ln, middle.transform);
		//constraints.Add(pc);
#if false
		PointConstraint1 pcon1 = new PointConstraint1();
		pcon1.p1 = 1;
		pcon1.off = new Vector3(0.0f, springs[0].restlen, 0.0f);
		pcon1.obj = top.transform;	//.position;
		constraints.Add(pcon1);
		endcon = pcon1;

		masspos = new Vector3[masses.Count + 2];

		for ( int i = 0; i < masses.Count; i++ )
			masspos[i + 1] = masses[i].pos;

		masspos[0] = masspos[1];
		masspos[masspos.Length - 1] = masspos[masspos.Length - 2];
#endif
	}

	// Going to need a add force at position function
}
#endif