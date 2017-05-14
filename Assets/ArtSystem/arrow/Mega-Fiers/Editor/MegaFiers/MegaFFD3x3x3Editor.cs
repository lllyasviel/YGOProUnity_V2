using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MegaFFD3x3x3))]
public class MegaFFD3x3x3Editor : MegaFFDEditor
{
	public override string GetHelpString() { return "FFD3x3x3 Modifier by Chris West"; }
}