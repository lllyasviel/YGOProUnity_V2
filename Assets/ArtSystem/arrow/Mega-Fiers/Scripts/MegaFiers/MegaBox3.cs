
using UnityEngine;

[System.Serializable]
public class MegaBox3
{
	public Vector3	center;
	public Vector3	min;
	public Vector3	max;
	public float		radius = 0.0f;
	public Vector3[] verts = new Vector3[8];

	public Vector3 Size()
	{
		return max - min;
	}

	public void SetSize(Vector3 size)
	{
		min = -(size * 0.5f);
		max = (size * 0.5f);
		center = Vector3.zero;
		radius = size.magnitude;
		CalcVerts();
	}

	public float Radius()
	{
		if ( radius <= 0.0f )
			radius = max.magnitude;

		return radius;
	}

	public override string ToString()
	{
		return "cen " + center + " min " + min + " max " + max;
	}

	Vector3 GetVert(int i)
	{
		Vector3 extents = Size() * 0.5f;

		switch ( i )
		{
			case 0:	return center + extents;
			case 1:	return center + Vector3.Scale(extents, new Vector3(-1, 1, 1));
			case 2:	return center + Vector3.Scale(extents, new Vector3(1, 1, -1));
			case 3:	return center + Vector3.Scale(extents, new Vector3(-1, 1, -1));
			case 4:	return center + Vector3.Scale(extents, new Vector3(1, -1, 1));
			case 5:	return center + Vector3.Scale(extents, new Vector3(-1, -1, 1));
			case 6:	return center + Vector3.Scale(extents, new Vector3(1, -1, -1));
			default:	return center + Vector3.Scale(extents, new Vector3(-1, -1, -1));
		}
	}

	void CalcVerts()
	{
		for ( int i = 0; i < 8; i++ )
			verts[i] = GetVert(i);
	}

	public Vector3 this[int index]
	{
		get
		{
			return verts[index];
		}
		set
		{
			verts[index] = value;
		}
	}

	static public Vector3 GetVert(Bounds bounds, int i)
	{
		switch ( i )
		{
			case 0: return bounds.center + bounds.extents;
			case 1: return bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, 1, 1));
			case 2: return bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, 1, -1));
			case 3: return bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, 1, -1));
			case 4: return bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, -1, 1));
			case 5: return bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, -1, 1));
			case 6: return bounds.center + Vector3.Scale(bounds.extents, new Vector3(1, -1, -1));
			default: return bounds.center + Vector3.Scale(bounds.extents, new Vector3(-1, -1, -1));
		}
	}
}