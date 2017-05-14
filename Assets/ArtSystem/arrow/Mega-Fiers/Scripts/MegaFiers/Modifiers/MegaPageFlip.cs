
using UnityEngine;

[AddComponentMenu("Modifiers/Page Flip")]
public class MegaPageFlip : MegaModifier
{
	public bool		animT		= false;
	public bool		autoMode	= true;
	public bool		lockRho		= true;
	public bool		lockTheta	= true;
	public float	timeStep	= 0.01f;
	public float	rho			= 0.0f;
	public float	theta		= 0.0f;
	public float	deltaT		= 0.0f;
	public float	kT			= 1.0f;
	public float	turn		= 0.0f;
	public float	ap1			= -15.0f;
	public float	ap2			= -2.5f;
	public float	ap3			= -3.5f;
	public bool		flipx		= true;
	Vector2			_pageSize;
	Vector3			apex		= new Vector3(0.0f, 0.0f, -3.0f);
	Vector3			_cornerP;
	Vector3			_pageOrigin;
	float			fx			= 1.0f;

	public void calcAuto(float t)
	{
		float num = 90.0f * Mathf.Deg2Rad;
		if ( t == 0.0f )
		{
			rho = 0.0f;
			theta = num;
			apex.z = ap1;	//-15.0f;
		}
		else
		{
			float num2;
			float num3;
			float num4;
			if ( t <= 0.15f )
			{
				num2 = t / 0.15f;
				num3 = Mathf.Sin((Mathf.PI * Mathf.Pow(num2, 0.05f)) / 2.0f);
				num4 = Mathf.Sin((Mathf.PI * Mathf.Pow(num2, 0.5f)) / 2.0f);
				rho = t * 180.0f;
				theta = funcLinear(num3, 90.0f * Mathf.Deg2Rad, 8.0f * Mathf.Deg2Rad);
				apex.z = funcLinear(num4, ap1, ap2);	//-15.0f, -2.5f);
			}
			else
			{
				if ( t <= 0.4f )
				{
					num2 = (t - 0.15f) / 0.25f;
					rho = t * 180f;
					theta = funcLinear(num2, 8.0f * Mathf.Deg2Rad, 6.0f * Mathf.Deg2Rad);
					apex.z = funcLinear(num2, ap2, ap3);	//-2.5f, -3.5f);
				}
				else
				{
					if ( t <= 1.0f )
					{
						num2 = (t - 0.4f) / 0.6f;
						rho = t * 180.0f;
						num3 = Mathf.Sin((Mathf.PI * Mathf.Pow(num2, 10.0f)) / 2.0f);
						num4 = Mathf.Sin((Mathf.PI * Mathf.Pow(num2, 2.0f)) / 2.0f);
						theta = funcLinear(num3, 6.0f * Mathf.Deg2Rad, 90.0f * Mathf.Deg2Rad);
						apex.z = funcLinear(num4, ap3, ap1);	//-3.5f, -15.0f);
					}
				}
			}
		}
	}

	public float calcTheta(float _rho)
	{
		int num = 0;
		float num2 = 1.0f;
		float num3 = 0.05f;
		float num4 = 90.0f * Mathf.Deg2Rad;
		float num5 = (num2 - num3) * num4;
		float num6 = _rho / 180.0f;
		if ( num6 < 0.25f )
			num = (int)(num6 / 0.25f);
		else
		{
			if ( num6 < 0.5f )
				num = 1;
			else
			{
				if ( num6 <= 1.0f )
					num = (int)((1.0f - num6) * 0.5f);
			}
		}

		return (num4 - (num * num5));
	}

	public float calcTheta2(float t)
	{
		float num = 0.1f;
		float num2 = 45.0f * Mathf.Deg2Rad;
		float num3 = Mathf.Abs(1.0f - (t * 2.0f));
		return ((num * num2) + (num3 * num2));
	}

	public Vector3 curlTurn(Vector3 p)
	{
		float rhs = Mathf.Sqrt((p.x * p.x) + Mathf.Pow((p.z - apex.z), 2.0f));
		float num2 = rhs * Mathf.Sin(theta);
		float f = Mathf.Asin(p.x / rhs) / Mathf.Sin(theta);
		p.x = num2 * Mathf.Sin(f);
		p.z = (rhs + apex.z) - ((num2 * (1.0f - Mathf.Cos(f))) * Mathf.Sin(theta));
		p.y = (num2 * (1.0f - Mathf.Cos(f))) * Mathf.Cos(theta);
		return p;
	}

	public Vector3 flatTurn(Vector3 p)
	{
		theta = (deltaT * Mathf.PI) * 2.0f;
		float rhs = p.x / _pageSize.x;
		p.x = Mathf.Cos(theta) * rhs * _pageSize.x;
		p.y = Mathf.Sin(theta) * rhs * _pageSize.x;
		return p;
	}

	public float funcLinear(float ft, float f0, float f1)
	{
		return (f0 + ((f1 - f0) * ft));
	}

	public float funcQuad(float ft, float f0, float f1, float p)
	{
		return (f0 + ((f1 - f0) * Mathf.Pow(ft, p)));
	}

	public override string ModName() { return "PageFlip"; }
	public override string GetHelpURL() { return "?page_id=271"; }

	public Vector3 flatTurn1(Vector3 p)
	{
		float rhs = p.x;
		p.x = Mathf.Cos(rho * Mathf.Deg2Rad) * rhs;
		p.y = Mathf.Sin(rho * Mathf.Deg2Rad) * -rhs;
		return p;
	}

	public Vector3 rotpage(Vector3 p)
	{
		float x = p.x;
		float y = p.y;
		p.x = Mathf.Cos(rho * Mathf.Deg2Rad) * x + Mathf.Sin(rho * Mathf.Deg2Rad) * y;
		p.y = Mathf.Sin(rho * Mathf.Deg2Rad) * -x + Mathf.Cos(rho * Mathf.Deg2Rad) * y;
		return p;
	}

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);
		p = curlTurn(p);
		p.x *= fx;
		p = rotpage(p);
		p.x *= fx;
		return invtm.MultiplyPoint3x4(p);
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		if ( flipx )
			fx = -1.0f;
		else
			fx = 1.0f;

		theta = 15.0f * Mathf.Deg2Rad;

		if ( turn < 0.0f )
			turn = 0.0f;

		if ( turn > 100.0f )
			turn = 100.0f;

		deltaT = turn / 100.0f;

		if ( animT )
			deltaT = (kT * Time.time) % 1.0f;
		if ( autoMode )
			calcAuto(deltaT);

		return true;
	}
}

// If this works can do as a max plugin
// New book builder
// need spine model, height is defined by page count, arc value
// angle range is front cover angle - back cover angle
// each page has a start angle based on the cover angles, though some covers wont effect so its the number of pages turned
// times the cover angle
// can we do ring binders as well
// if page is covered then only build edge or edge and a small area around the edge, ie one segment in