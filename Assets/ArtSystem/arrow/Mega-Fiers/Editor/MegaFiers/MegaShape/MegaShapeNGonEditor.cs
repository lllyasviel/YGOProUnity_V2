
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MegaShapeNGon))]
public class MegaShapeNGonEditor : MegaShapeEditor
{
	public float	fillet = 0.0f;
	public int		sides		= 6;
	public bool		circular = false;
	public bool		scribe = false;

	public override bool Params()
	{
		MegaShapeNGon shape = (MegaShapeNGon)target;

		bool rebuild = false;

		float v = EditorGUILayout.FloatField("Radius", shape.radius);
		if ( v != shape.radius )
		{
			shape.radius = v;
			rebuild = true;
		}

		v = EditorGUILayout.FloatField("Fillet", shape.fillet);
		if ( v != shape.fillet )
		{
			shape.fillet = v;
			rebuild = true;
		}

		int iv = EditorGUILayout.IntField("Side", shape.sides);
		if ( iv != shape.sides )
		{
			shape.sides = iv;
			rebuild = true;
		}

		bool bv = EditorGUILayout.Toggle("Circular", shape.circular);
		if ( bv != shape.circular )
		{
			shape.circular = bv;
			rebuild = true;
		}

		bv = EditorGUILayout.Toggle("Circumscribed", shape.scribe);
		if ( bv != shape.scribe )
		{
			shape.scribe = bv;
			rebuild = true;
		}

		return rebuild;
	}
}