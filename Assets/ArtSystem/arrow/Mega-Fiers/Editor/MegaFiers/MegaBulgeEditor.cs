
using UnityEditor;

[CustomEditor(typeof(MegaBulge))]
public class MegaBulgeEditor : MegaModifierEditor
{

	public override bool Inspector()
	{
		MegaBulge mod = (MegaBulge)target;

		EditorGUIUtility.LookLikeControls();
		mod.Amount = EditorGUILayout.Vector3Field("Radius", mod.Amount);
		mod.FallOff = EditorGUILayout.Vector3Field("Falloff", mod.FallOff);
		mod.LinkFallOff = EditorGUILayout.Toggle("Link Falloff", mod.LinkFallOff);
		return false;
	}
}
