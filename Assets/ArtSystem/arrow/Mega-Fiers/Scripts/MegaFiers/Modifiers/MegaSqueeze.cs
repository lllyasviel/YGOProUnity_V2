
using UnityEngine;

[AddComponentMenu("Modifiers/Squeeze")]
public class MegaSqueeze : MegaModifier
{
	public float			amount		= 0.0f;
	public float			crv			= 0.0f;
	public float			radialamount = 0.0f;
	public float			radialcrv	= 0.0f;
	public bool				doRegion	= false;
	public float			to			= 0.0f;
	public float			from		= 0.0f;
	Matrix4x4				mat			= new Matrix4x4();
	float k1;
	float k2;
	float k3;
	float k4;
	float l;
	float l2;

	void SetK(float K1, float K2, float K3, float K4)
	{
		k1 = K1;
		k2 = K2;
		k3 = K3;
		k4 = K4;
	}

	public override string ModName() { return "Squeeze"; }
	public override string GetHelpURL() { return "?page_id=338"; }

	// Radial amount works on distance from pivot on the vertical axis, the lower the value the more effect
	// the other one works on distance from the vertical axis, the lower the value the more the effect
	public override Vector3 Map(int i, Vector3 p)
	{
		float z;

		p = tm.MultiplyPoint3x4(p);

		if ( l != 0.0f )
		{
			if ( doRegion )
			{
				if ( p.y < from )
					z = from / l;
				else
				{
					if ( p.y > to )
						z = to / l;
					else
						z = p.y / l;
				}
			}
			else
				z = Mathf.Abs(p.y / l);


			float f =  1.0f + z * k1 + k2 * z * (1.0f - z);

			p.y *= f;
		}

		if ( l2 != 0.0f )
		{
			float dist = Mathf.Sqrt(p.x * p.x + p.z * p.z);
			float xy = dist / l2;
			float f1 =  1.0f + xy * k3 + k4 * xy * (1.0f - xy);
			p.x *= f1;
			p.z *= f1;
		}

		return invtm.MultiplyPoint3x4(p);
	}

	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		mat = Matrix4x4.identity;
		SetAxis(mat);
		SetK(amount, crv, radialamount, radialcrv);
		Vector3 size = bbox.Size();
		l = size[1];	//bbox.max[1] - bbox.min[1];
		l2 = Mathf.Sqrt(size[0] * size[0] + size[2] * size[2]);
		return true;
	}

	public override void ExtraGizmo(MegaModContext mc)
	{
		if ( doRegion )
			DrawFromTo(MegaAxis.Z, from, to, mc);
	}
}