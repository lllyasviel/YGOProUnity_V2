
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MegaShapeArc))]
public class MegaShapeArcEditor : MegaShapeEditor
{
	public override bool Params()
	{
		MegaShapeArc shape = (MegaShapeArc)target;

		bool rebuild = false;

		float v = EditorGUILayout.FloatField("Radius", shape.radius);
		if ( v != shape.radius )
		{
			shape.radius = v;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("From", shape.from);
		if ( v != shape.from )
		{
			shape.from = v;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("To", shape.to);
		if ( v != shape.to )
		{
			shape.to = v;
			rebuild = true;
		}

		bool bv = EditorGUILayout.Toggle("Pie", shape.pie);
		if ( bv != shape.pie )
		{
			shape.pie = bv;
			rebuild = true;
		}

		return rebuild;
	}
}