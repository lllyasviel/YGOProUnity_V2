
using UnityEngine;
using System.Collections.Generic;

public enum MegaFallOff
{
	Gauss,
	Linear,
	Point,
}

public enum MegaPaintMode
{
	Add,
	Subtract,
	Set,
	Multiply,
}

[AddComponentMenu("Modifiers/Paint")]
public class MegaPaint : MegaModifier
{
	public float			radius = 1.0f;
	public float			amount = 10.0f;
	public float			decay = 1.0f;
	public bool				usedecay = false;
	public MegaFallOff		fallOff = MegaFallOff.Gauss;
	public float			gaussc = 0.25f;
	public bool				useAvgNorm = false;
	public Vector3			normal = Vector3.up;
	public MegaPaintMode	mode = MegaPaintMode.Add;	// 0 = add 1 = set 2 = mult

	bool		hadahit = false;
	Vector3		relativePoint = Vector3.zero;
	List<int>	affected = new List<int>();
	List<float>	distances= new List<float>();
	Matrix4x4	mat = new Matrix4x4();
	Vector3[]	offsets;
	Vector3[]	normals;

	public override string ModName()	{ return "Paint"; }
	public override string GetHelpURL() { return "?page_id=1292"; }

	static float LinearFalloff(float distance, float inRadius)
	{
		return Mathf.Clamp01(1.0f - distance / inRadius);
	}

	static float PointFalloff(float dist, float inRadius)
	{
		return -(dist * dist) / (inRadius * inRadius) + 1.0f;
	}

	static public float GaussFalloff(float v, float c)
	{
		float e = 2.718281828f;

		float a = 1.0f;
		float b = 0.5f;
		//float c = 0.4f;	//1.0f;

		float xb2 = (v - b) * (v - b);
		float c2 = 2.0f * c * c;

		float g = a * Mathf.Pow(e, -(xb2 / c2));
		return g;
	}

	float Gaussian(float dist, float width)
	{
		return Mathf.Exp((-dist * dist) / width);
	}

	void DeformMesh(Vector3 position, float power, float inRadius)
	{
		if ( hadahit )
		{
			float sqrRadius = inRadius * inRadius;
		
			// Calculate averaged normal of all surrounding vertices
			Vector3 averageNormal = Vector3.zero;

			for ( int i = 0; i < verts.Length; i++ )
			{
				Vector3 p = verts[i];

				float sqrMagnitude = (p - position).sqrMagnitude;
				if ( sqrMagnitude > sqrRadius )
					continue;

				affected.Add(i);
				float distance = Mathf.Sqrt(sqrMagnitude);
				distances.Add(distance);
				if ( useAvgNorm )
				{
					float falloff = LinearFalloff(distance, inRadius);
					averageNormal += falloff * normals[i];
				}
			}

			if ( useAvgNorm )
				averageNormal = averageNormal.normalized;
			else
				averageNormal = normal;
		
			// Deform vertices along averaged normal
			for ( int j = 0; j < affected.Count; j++ )
			{
				float falloff = 0.0f;

				switch ( fallOff )
				{
					case MegaFallOff.Gauss:	falloff = GaussFalloff(distances[j], inRadius);		break;
					case MegaFallOff.Point:	falloff = PointFalloff(distances[j], inRadius);	break;
					default: falloff = LinearFalloff(distances[j], inRadius); break;
				}

				switch ( mode )
				{
					case MegaPaintMode.Add:			offsets[affected[j]] += averageNormal * falloff * power;	break;
					case MegaPaintMode.Subtract:	offsets[affected[j]] -= averageNormal * falloff * power; break;
					case MegaPaintMode.Set:			offsets[affected[j]] = averageNormal * amount;					break;
					case MegaPaintMode.Multiply:	offsets[affected[j]] = Vector3.Scale(offsets[affected[j]], averageNormal * (1.0f + (falloff * power))); break;
				}
			}
		}

		if ( !usedecay )
		{
			for ( int i = 0; i < verts.Length; i++ )
			{
				sverts[i].x = verts[i].x + offsets[i].x;
				sverts[i].y = verts[i].y + offsets[i].y;
				sverts[i].z = verts[i].z + offsets[i].z;
			}
		}
		else
		{
			for ( int i = 0; i < verts.Length; i++ )
			{
				offsets[i].x *= decay;
				offsets[i].y *= decay;
				offsets[i].z *= decay;

				sverts[i].x = verts[i].x + offsets[i].x;
				sverts[i].y = verts[i].y + offsets[i].y;
				sverts[i].z = verts[i].z + offsets[i].z;
			}
		}
	}

	public override void Modify(MegaModifiers mc)
	{
		DeformMesh(relativePoint, amount * Time.deltaTime, radius);
	}

	public override Vector3 Map(int i, Vector3 p)
	{
		return p;
	}

	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		affected.Clear();
		distances.Clear();

		if ( offsets == null || offsets.Length != mc.mod.verts.Length )
		{
			offsets = new Vector3[mc.mod.verts.Length];
		}

		normals = mc.mod.mesh.normals;	// get current normals
		mat = Matrix4x4.identity;

		SetAxis(mat);

		hadahit = false;

		if ( Input.GetMouseButton(0) )
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if ( Physics.Raycast(ray, out hit) )
			{
				if ( hit.collider == gameObject.GetComponent<Collider>() )
				{
					hadahit = true;
					relativePoint = hit.collider.transform.InverseTransformPoint(hit.point);
				}
			}
		}
		else
		{

		}
		return true;
	}

	public override void PrepareMT(MegaModifiers mc, int cores)
	{
	}

	public override void DoWork(MegaModifiers mc, int index, int start, int end, int cores)
	{
		if ( index == 0 )
			Modify(mc);
	}
}