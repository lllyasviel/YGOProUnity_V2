
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MegaShapeHelix))]
public class MegaShapeHelixEditor : MegaShapeEditor
{
	public float radius1 = 1.0f;
	public float radius2 = 1.0f;
	public float height = 0.0f;
	public float turns = 0.0f;
	public float bias = 0.0f;

	public bool clockwise = true;

	public override bool Params()
	{
		MegaShapeHelix shape = (MegaShapeHelix)target;

		bool rebuild = false;

		float v = EditorGUILayout.FloatField("Radius 1", shape.radius1);
		if ( v != shape.radius1 )
		{
			shape.radius1 = v;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("Radius 2", shape.radius2);
		if ( v != shape.radius2 )
		{
			shape.radius2 = v;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("Height", shape.height);
		if ( v != shape.height )
		{
			shape.height = v;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("Turns", shape.turns);
		if ( v != shape.turns )
		{
			shape.turns = v;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("Bias", shape.bias);
		if ( v != shape.bias )
		{
			shape.bias = v;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("Adjust", shape.adjust);
		if ( v != shape.adjust )
		{
			shape.adjust = v;
			rebuild = true;
		}

		int iv = EditorGUILayout.IntField("Points Per Turn", shape.PointsPerTurn);
		if ( iv != shape.PointsPerTurn )
		{
			shape.PointsPerTurn = iv;
			rebuild = true;
		}

		bool bv = EditorGUILayout.Toggle("Clockwise", shape.clockwise);
		if ( bv != shape.clockwise )
		{
			shape.clockwise = bv;
			rebuild = true;
		}

		return rebuild;
	}
}