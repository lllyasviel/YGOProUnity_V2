
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MegaShapeEllipse))]
public class MegaShapeEllipseEditor : MegaShapeEditor
{
	public override bool Params()
	{
		MegaShapeEllipse shape = (MegaShapeEllipse)target;

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

		return rebuild;
	}
}