
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaTwistWarp))]
public class MegaTwistWarpEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
	}
}
