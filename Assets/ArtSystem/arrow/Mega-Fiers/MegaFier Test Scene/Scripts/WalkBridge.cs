
using UnityEngine;

[ExecuteInEditMode]
public class WalkBridge : MonoBehaviour
{
	public GameObject		bridge;
	[HideInInspector]
	public MegaCurveDeform	mod;
	public float offset = 0.0f;	// Character offset
	public float smooth = 0.0f;

	void Update()
	{
		if ( bridge )
		{
			// Get the bridge modifier
			if ( mod == null )
				mod = bridge.GetComponent<MegaCurveDeform>();

			if ( mod )
			{
				int ax = (int)mod.axis;
				Vector3 pos = transform.position;

				// Get into local space
				Vector3 lpos = mod.transform.worldToLocalMatrix.MultiplyPoint(pos);

				// Are we on the bridge
				if ( lpos.x > mod.bbox.min.x && lpos.x < mod.bbox.max.x && lpos.z > mod.bbox.min.z && lpos.z < mod.bbox.max.z )
				{
					// How far across are we
					float alpha = (lpos[ax] - mod.bbox.min[ax]) / (mod.bbox.max[ax] - mod.bbox.min[ax]);

					// Deform the bridge
					SetPos(mod, alpha);
					// Place object on deformed bridge
					lpos.y = mod.GetPos(alpha) + (offset * 0.01f);	// 0.01 is just to make inspector easier to control in my test scene which is obvioulsy very small

					transform.position = bridge.transform.localToWorldMatrix.MultiplyPoint(lpos);
				}
				else
					SetPos(mod, 0.0f);
			}
		}
	}

	private float easeInOutSine(float start, float end, float value)
	{
		end -= start;
		return -end / 2.0f * (Mathf.Cos(Mathf.PI * value / 1.0f) - 1.0f) + start;
	}

	// Change the keys
	public void SetPos(MegaCurveDeform mod, float alpha)
	{
		float val = 0.0f;

		if ( alpha < 0.5f )
			val = easeInOutSine(0.0f, -mod.MaxDeviation * 0.01f, alpha / 0.5f);
		else
			val = easeInOutSine(-mod.MaxDeviation * 0.01f, 0.0f, (alpha - 0.5f) / 0.5f);

		mod.SetKey(0, 0.0f, 0.0f, Mathf.Tan(0.0f), Mathf.Tan(Mathf.Atan2(val, alpha)));

		float inTangent = Mathf.Lerp(Mathf.Tan(0.0f), Mathf.Tan(Mathf.Atan2(val, alpha)), smooth);
		float outTangent = Mathf.Lerp(Mathf.Tan(Mathf.PI), Mathf.Tan(Mathf.Atan2(val, alpha - 1.0f)), smooth);

		mod.SetKey(1, Mathf.Clamp(alpha, 0.001f, 0.999f), val, inTangent, outTangent);

		mod.SetKey(2, 1.0f, 0.0f, Mathf.Tan(Mathf.Atan2(-val, 1.0f - alpha)), Mathf.Tan(Mathf.PI));
	}
}