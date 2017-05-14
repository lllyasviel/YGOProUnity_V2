
using UnityEngine;

public enum MegaNormType
{
	Normals = 0,
	Vertices,
	Average,
}

[AddComponentMenu("Modifiers/Push")]
public class MegaPush : MegaModifier
{
	public float		amount	= 0.0f;
	public MegaNormType	method	= MegaNormType.Normals;
	Vector3[]			normals;

	public override string ModName() { return "Push"; }
	public override string GetHelpURL() { return "?page_id=282"; }

	public override Vector3 Map(int i, Vector3 p)
	{
		if ( i >= 0 )
			p += normals[i] * amount;

		return p;
	}

	void CalcNormals(Mesh mesh)
	{
		if ( mesh != null )
		{
			switch ( method )
			{
				case MegaNormType.Normals:
					normals = mesh.normals;
					break;

				case MegaNormType.Vertices:
					normals = new Vector3[mesh.normals.Length];

					for ( int i = 0; i < mesh.vertexCount; i++ )
						normals[i] = Vector3.Normalize(mesh.vertices[i]);
					break;

				case MegaNormType.Average:
					normals = mesh.normals;

					for ( int i = 0; i < mesh.vertexCount; i++ )
					{
						for ( int j = 0; j < mesh.vertexCount; j++ )
						{
							if ( mesh.vertices[i] == mesh.vertices[j] )
							{
								normals[i] = (normals[i] + normals[j]) / 2.0f;
								normals[j] = (normals[i] + normals[j]) / 2.0f;
							}
						}
					}
					break;
			}
		}
	}

	// 3 ways, use normals as is, use a normal made from vertex, use quick normal calc, or for exported stuff
	// we could include averaged normals
	public override void ModStart(MegaModifiers mc)
	{
		CalcNormals(mc.mesh);
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		if ( normals != null )
			return true;

		return false;
	}

	void Reset()
	{
		if ( GetComponent<Renderer>() != null )
		{
			Mesh ms = MegaUtils.GetSharedMesh(gameObject);

			if ( ms != null )
			{
				CalcNormals(ms);

				Bounds b = ms.bounds;
				Offset = -b.center;
				bbox.min = b.center - b.extents;
				bbox.max = b.center + b.extents;
			}
		}
	}
}
