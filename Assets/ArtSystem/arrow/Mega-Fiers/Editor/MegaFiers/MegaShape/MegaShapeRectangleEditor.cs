
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MegaShapeRectangle))]
public class MegaShapeRectangleEditor : MegaShapeEditor
{
	public override bool Params()
	{
		MegaShapeRectangle shape = (MegaShapeRectangle)target;

		bool rebuild = false;

		float v = EditorGUILayout.FloatField("Length", shape.length);
		if ( v != shape.length )
		{
			shape.length = v;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("Width", shape.width);
		if ( v != shape.width )
		{
			shape.width = v;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("Fillet", shape.fillet);
		if ( v != shape.fillet )
		{
			shape.fillet = v;
			rebuild = true;
		}

		return rebuild;
	}
}