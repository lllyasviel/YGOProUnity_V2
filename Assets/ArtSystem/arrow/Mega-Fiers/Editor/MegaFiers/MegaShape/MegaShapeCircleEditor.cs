
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MegaShapeCircle))]
public class MegaShapeCircleEditor : MegaShapeEditor
{
	public override bool Params()
	{
		MegaShapeCircle shape = (MegaShapeCircle)target;

		bool rebuild = false;

		float radius = EditorGUILayout.FloatField("Radius", shape.Radius);
		if ( radius != shape.Radius )
		{
			if ( radius < 0.001f )
				radius = 0.001f;

			shape.Radius = radius;
			rebuild = true;
		}

		return rebuild;
	}
}