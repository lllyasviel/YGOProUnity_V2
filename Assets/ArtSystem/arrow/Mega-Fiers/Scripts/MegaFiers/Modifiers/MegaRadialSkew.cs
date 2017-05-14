
using UnityEngine;

[AddComponentMenu("Modifiers/Radial Skew")]
public class MegaRadialSkew : MegaModifier
{
	public float	angle		= 0.0f;
	public MegaAxis	axis		= MegaAxis.X;
	public MegaAxis	eaxis		= MegaAxis.X;
	public bool		biaxial	= false;

	public override string ModName() { return "RaidalSkew"; }
	public override string GetHelpURL() { return "?page_id=305"; }

	Vector3 GetSkew(Vector3 p)
	{
		if ( biaxial )
		{
			switch ( axis )
			{
				case MegaAxis.X:
					switch ( eaxis )
					{
						case MegaAxis.Y: p.x = p.z = 0.0f; break;
						case MegaAxis.Z: p.x = p.y = 0.0f; break;
						default: p.x = p.y = 0.0f; break;
					}
					break;

				case MegaAxis.Y:
					switch ( eaxis )
					{
						case MegaAxis.X: p.y = p.z = 0.0f; break;
						case MegaAxis.Z: p.x = p.y = 0.0f; break;
						default: p.x = p.y = 0.0f; break;
					}
					break;

				case MegaAxis.Z:
					switch ( eaxis )
					{
						case MegaAxis.X: p.y = p.z = 0.0f; break;
						case MegaAxis.Y: p.x = p.z = 0.0f; break;
						default: p.y = p.z = 0.0f; break;
					}
					break;
			}
		}
		else
		{
			switch ( axis )
			{
				case MegaAxis.X: p.x = 0.0f; break;
				case MegaAxis.Y: p.y = 0.0f; break;
				case MegaAxis.Z: p.z = 0.0f; break;
			}
		}

		return p.normalized;
	}

	public override Vector3 Map(int i, Vector3 p)
	{
		p = tm.MultiplyPoint3x4(p);

		float skewamount = Mathf.Atan(Mathf.Deg2Rad * angle);

		Vector3 skewv = GetSkew(p) * skewamount * p[(int)axis];

		p += skewv;

		return invtm.MultiplyPoint3x4(p);
	}
}
