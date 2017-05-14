
using UnityEngine;

[ExecuteInEditMode]
public class WalkRope : MonoBehaviour
{
	public GameObject		bridge;
	[HideInInspector]
	public MegaRopeDeform	mod;
	public float offset = 0.0f;	// Character offset

	//Vector3 lastpos = Vector3.zero;

	void LateUpdate()
	{
		if ( bridge )
		{
			// Get the bridge modifier
			if ( mod == null )
			{
				mod = bridge.GetComponent<MegaRopeDeform>();
			}

			if ( mod )
			{
				int ax = (int)mod.axis;
				Vector3 pos = transform.position;

				// Get into local space
				Vector3 lpos = mod.transform.worldToLocalMatrix.MultiplyPoint(pos);

				// Are we on the bridge
				//if ( lpos.x > mod.bbox.min.x && lpos.x < mod.bbox.max.x && lpos.z > mod.bbox.min.z && lpos.z < mod.bbox.max.z )
				//{
					// How far across are we
					//float alpha = (lpos[ax] - mod.bbox.min[ax]) / (mod.bbox.max[ax] - mod.bbox.min[ax]);
					float alpha = (lpos[ax] - mod.soft.masses[0].pos.x) / (mod.soft.masses[mod.soft.masses.Count - 1].pos.x - mod.soft.masses[0].pos.x);

					if ( alpha > 0.0f || alpha < 1.0f )
					{
						//if ( alpha < 0.0f )
						//	alpha = 0.0f;

						// Deform the bridge
						//SetPos(mod, alpha);
						// Place object on deformed bridge
						//Vector2 rpos = mod.GetPos3(lpos[ax]);
						Vector2 rpos = mod.SetWeight(lpos[ax], weight);

						//lpos[ax] = rpos.x;
						lpos.y = rpos.y + (offset * 0.01f);	// 0.01 is just to make inspector easier to control in my test scene which is obvioulsy very small
						//lpos.y = mod.GetPos(alpha) + (offset * 0.01f);	// 0.01 is just to make inspector easier to control in my test scene which is obvioulsy very small

						transform.position = bridge.transform.localToWorldMatrix.MultiplyPoint(lpos);
					}
				//}
				//else
				//{
					//SetPos(mod, 0.0f);
				//}
			}
		}
	}

	public float weight = 1.0f;

	public void SetPos(MegaRopeDeform mod, float alpha)
	{
		mod.weightPos = alpha * 100.0f;
		mod.weight = weight;
	}
}