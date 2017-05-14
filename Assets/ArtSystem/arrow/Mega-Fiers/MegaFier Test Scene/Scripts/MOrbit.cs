
using UnityEngine;

public class MOrbit : MonoBehaviour
{
	public GameObject target;
	MeshRenderer render;
	SkinnedMeshRenderer srender;
	MeshFilter	filter;
	public float distance = 10.0f;
	public float xSpeed = 250.0f;
	public float ySpeed = 120.0f;
	public float zSpeed = 120.0f;
	public float yMinLimit = -20.0f;
	public float yMaxLimit = 80.0f;
	public float xMinLimit = -20.0f;
	public float xMaxLimit = 20.0f;
	private float x = 0.0f;
	private float y = 0.0f;
	private Vector3 center;
	public bool Dynamic = false;

	public Vector3	offset;
	public Vector3 EditTest;

	Vector3 tpos = new Vector3();	// target pos

	void Start()
	{
		NewTarget(target);

		if ( target )
		{
			tpos = target.transform.position;
			Vector3 angles = Quaternion.LookRotation(tpos - transform.position).eulerAngles;
			x = angles.y;
			y = angles.x;
			distance = (tpos - transform.position).magnitude;
		}
		else
		{
			Vector3 angles = transform.eulerAngles;
			x = angles.y;
			y = angles.x;
		}

		nx = x;
		ny = y;
		nz = distance;
		// Make the rigid body not change rotation
		if ( GetComponent<Rigidbody>() )
			GetComponent<Rigidbody>().freezeRotation = true;
	}

	private float easeInOutQuint(float start, float end, float value)
	{
		value /= .5f;
		end -= start;
		if ( value < 1 ) return end / 2 * value * value * value * value * value + start;
		value -= 2;
		return end / 2 * (value * value * value * value * value + 2) + start;
	}

	float easeInQuad(float start, float end, float value)
	{
		value /= 1.0f;
		end -= start;
		return end * value * value + start;
	}

	float easeInSine(float start, float end, float value)
	{
		end -= start;
		return -end * Mathf.Cos(value / 1.0f * (Mathf.PI / 2.0f)) + end + start;
	}

	float t = 0.0f;

	public void NewTarget(GameObject targ)
	{
		if ( target != targ )
		{
			target = targ;
			t = 0.0f;

			if ( target )
			{
				filter = (MeshFilter)target.GetComponent(typeof(MeshFilter));
				if ( filter != null )
					center = filter.mesh.bounds.center;
				else
				{
					render = (MeshRenderer)target.GetComponent(typeof(MeshRenderer));

					if ( render != null )
						center = render.bounds.center;
					else
					{
						srender = (SkinnedMeshRenderer)target.GetComponent(typeof(SkinnedMeshRenderer));

						if ( srender != null )
							center = srender.bounds.center;
					}
				}
			}
		}
	}

	public float trantime = 4.0f;

	float vx = 0.0f;
	float vy = 0.0f;
	float vz = 0.0f;
	float nx = 0.0f;
	float ny = 0.0f;
	float nz = 0.0f;

	public float delay = 0.2f;
	public float delayz = 0.2f;

	void LateUpdate()
	{
		if ( target )
		{
			if ( Input.GetMouseButton(1) )
			{
				nx = x + Input.GetAxis("Mouse X") * xSpeed * 0.02f;
				ny = y - Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			}

			x = Mathf.SmoothDamp(x, nx, ref vx, delay);	//0.21f);	//, 100.0f, Time.deltaTime);
			y = Mathf.SmoothDamp(y, ny, ref vy, delay);	//0.21f);	//, 100.0f, Time.deltaTime);

			// NOTE: If you get an exception for this line it means you dont have a scroll wheel input setup in
			// the input manager, go to Edit/Project Settings/Input and set the Mouse ScrollWheel setting to use 3rd mouse axis
			nz = nz - (Input.GetAxis("Mouse ScrollWheel") * zSpeed);

			y = ClampAngle(y, yMinLimit, yMaxLimit);
			//x = ClampAngle(x, xMinLimit, xMaxLimit);

			distance = Mathf.SmoothDamp(distance, nz, ref vz, delayz);
			//Vector3 crot = transform.localRotation.eulerAngles;

			if ( distance < 1.0f )
			{
				distance = 1.0f;
				nz = 1.0f;
			}

			Vector3 c;
			if ( Dynamic )
			{
				if ( filter != null )
				{
					c = target.transform.TransformPoint(filter.mesh.bounds.center + offset);
				}
				else
				{
					if ( render != null )
						c = target.transform.TransformPoint(render.bounds.center + offset);
					else
					{
						if ( srender != null )
							c = target.transform.TransformPoint(srender.bounds.center + offset);
						else
							c = target.transform.TransformPoint(center + offset);
					}
				}
			}
			else
				c = target.transform.TransformPoint(center + offset);

			// Want to ease to new target pos
			if ( t < trantime )
			{
				t = trantime;	//+= Time.deltaTime;

				tpos.x = easeInSine(tpos.x, c.x, t / trantime);
				tpos.y = easeInSine(tpos.y, c.y, t / trantime);
				tpos.z = easeInSine(tpos.z, c.z, t / trantime);
			}
			else
				tpos = c;

			Quaternion rotation = Quaternion.Euler(y, x, 0.0f);
			Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + tpos;	//c;

			transform.rotation = rotation;
			transform.position = position;
		}
	}

	static float ClampAngle(float angle, float min, float max)
	{
		if ( angle < -360.0f )
			angle += 360.0f;

		if ( angle > 360.0f )
			angle -= 360.0f;

		return Mathf.Clamp(angle, min, max);
	}
}