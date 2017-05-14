
using UnityEditor;

[CustomEditor(typeof(MegaCylindrify))]
public class MegaCylindrifyEditor : MegaModifierEditor
{
	public override bool Inspector()
	{
		MegaCylindrify mod = (MegaCylindrify)target;

		EditorGUIUtility.LookLikeControls();
		mod.Percent = EditorGUILayout.FloatField("Percent", mod.Percent);
		mod.Decay = EditorGUILayout.FloatField("Decay", mod.Decay);
		return false;
	}
}
