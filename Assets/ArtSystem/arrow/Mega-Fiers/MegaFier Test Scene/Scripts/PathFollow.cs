
using UnityEngine;

[ExecuteInEditMode]
public class PathFollow : MonoBehaviour
{
	public	float	tangentDist = 1.0f;	// how far it looks ahead or behind to calc rotation
	public	float	alpha	= 0.0f;		// how far along curve as a percent
	public	float	speed	= 0.0f;		// how fast it moves
	public	bool	rot		= false;	// check if you want to change rotation
	public	float	time	= 0.0f;		// how long to take to travel whole shape (system checks UseDistance then time then speed for which method it chooses, set non used to 0)
	public	float	ctime	= 0.0f;		// current time for time animation
	public	int		curve	= 0;		// curve to use in shape
	public	MegaShape target;			// Shape to follow
	public	float	distance = 0.0f;	// distance along shape
	public	bool	animate = false;	// automatically moves the object
	public	bool	UseDistance = true;	// use distance method

	public void SetPos(float a)
	{
		if ( target != null )
		{
			Vector3	pos = target.transform.TransformPoint(target.InterpCurve3D(curve, a, target.normalizedInterp));

			transform.position = pos;

			if ( rot )
			{
				float ta = tangentDist / target.GetCurveLength(curve);
				transform.LookAt(target.transform.TransformPoint(target.InterpCurve3D(curve, a + ta, target.normalizedInterp)));
			}
		}
	}

	public void SetPosFomDist(float dist)
	{
		if ( target != null )
		{
			float a = Mathf.Repeat(dist / target.GetCurveLength(curve), 1.0f);
			transform.position = target.transform.TransformPoint(target.InterpCurve3D(curve, a, target.normalizedInterp));

			if ( rot )
			{
				float ta = tangentDist / target.GetCurveLength(curve);
				transform.LookAt(target.transform.TransformPoint(target.InterpCurve3D(curve, a + ta, target.normalizedInterp)));
			}
		}
	}

	public void Start()
	{
		ctime = 0.0f;
		curve = 0;
	}

	void Update()
	{
		if ( animate )
		{
			if ( UseDistance )
				distance += speed * Time.deltaTime;
			else
			{
				if ( time > 0.0f )
				{
					ctime += Time.deltaTime;

					if ( ctime > time )
						ctime = 0.0f;

					alpha = (ctime / time) * 100.0f;
				}
				else
				{
					if ( speed != 0.0f )
					{
						alpha += speed * Time.deltaTime;

						if ( alpha > 100.0f )
							alpha = 0.0f;
						else
						{
							if ( alpha < 0.0f )
								alpha = 100.0f;
						}
					}
				}
			}
		}

		if ( UseDistance )
			SetPosFomDist(distance);
		else
			SetPos(alpha * 0.01f);
	}
}
