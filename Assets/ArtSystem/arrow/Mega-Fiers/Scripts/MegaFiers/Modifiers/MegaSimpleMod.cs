
using UnityEngine;

[AddComponentMenu("Modifiers/Simple")]
public class MegaSimpleMod : MegaModifier
{
	public Vector3 a3;

	public override string ModName()	{ return "Simple"; }
	public override string GetHelpURL() { return "?page_id=317"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		p.x += (-1.0f + (Random.value * 0.5f)) * a3.x;
		p.y += (-1.0f + (Random.value * 0.5f)) * a3.y;
		p.z += (-1.0f + (Random.value * 0.5f)) * a3.z;
		return p;
	}
}
