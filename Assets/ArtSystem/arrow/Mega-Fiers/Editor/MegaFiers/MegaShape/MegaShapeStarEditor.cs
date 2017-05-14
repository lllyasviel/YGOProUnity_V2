
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MegaShapeStar))]
public class MegaShapeStarEditor : MegaShapeEditor
{
	public override bool Params()
	{
		MegaShapeStar shape = (MegaShapeStar)target;

		bool rebuild = false;
		float v = EditorGUILayout.FloatField("Radius1", shape.radius1);
		if ( v != shape.radius1 )
		{
			shape.radius1 = v;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("Radius2", shape.radius2);
		if ( v != shape.radius2 )
		{
			shape.radius2 = v;
			rebuild = true;
		}

		int iv = EditorGUILayout.IntField("Points", shape.points);
		if ( iv != shape.points )
		{
			shape.points = iv;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("Distortion", shape.distortion);
		if ( v != shape.distortion )
		{
			shape.distortion = v;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("Fillet Radius 1", shape.fillet1);
		if ( v != shape.fillet1 )
		{
			shape.fillet1 = v;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("Fillet Radius 2", shape.fillet2);
		if ( v != shape.fillet2 )
		{
			shape.fillet2 = v;
			rebuild = true;
		}

		return rebuild;
	}
}