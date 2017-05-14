
using UnityEngine;

public class MegaFFD : MegaModifier
{
	//public bool				DrawGizmos	= true;
	public float		KnotSize		= 0.1f;
	public bool			inVol			= false;
	public Vector3[]	pt				= new Vector3[64];
	[HideInInspector]
	public float		EPSILON			= 0.001f;
	[HideInInspector]
	public Vector3		q				= new Vector3();
	[HideInInspector]
	public Vector3		pp				= new Vector3();
	[HideInInspector]
	public Vector3		lsize			= Vector3.one;
	[HideInInspector]
	public Vector3		bsize			= new Vector3();
	[HideInInspector]
	public Vector3		bcenter			= new Vector3();

	public virtual int		GridSize()	{ return 1; }
	public virtual int		GridIndex(int i, int j, int k)	{ return 0; }
	public override string GetHelpURL() { return "?page_id=199"; }

	public Vector3 LatticeSize()
	{
		Vector3 size = bsize;
		if ( size.x == 0.0f ) size.x = 0.001f;
		if ( size.y == 0.0f ) size.y = 0.001f;
		if ( size.z == 0.0f ) size.z = 0.001f;
		return size;
	}

	void Init()
	{
		lsize = LatticeSize();

		int size = GridSize();
		float fsize = size - 1.0f;

		for ( int i = 0; i < size; i++ )		// TODO: dim for all ffd
		{
			for ( int j = 0; j < size; j++ )
			{
				for ( int k = 0; k < size; k++ )
				{
					int c = GridIndex(i, j, k);
					pt[c].x = (float)(i) / fsize;
					pt[c].y = (float)(j) / fsize;
					pt[c].z = (float)(k) / fsize;
				}
			}
		}
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		Vector3 s = LatticeSize();

		for ( int i = 0; i < 3; i++ )
		{
			if ( s[i] == 0.0f )
				s[i] = 1.0f;
			else
				s[i] = 1.0f / s[i];
		}

		Vector3 c = MegaMatrix.GetTrans(ref tm);

		MegaMatrix.SetTrans(ref tm, c - bbox.min - Offset);

		MegaMatrix.Scale(ref tm, s, false);

		invtm = tm.inverse;
		return true;
	}

	public Vector3 GetPoint(int i)
	{
		Vector3 p = pt[i];

		p.x -= 0.5f;
		p.y -= 0.5f;
		p.z -= 0.5f;

		return Vector3.Scale(p, lsize);
	}

	public Vector3 GetPoint(int i, int j, int k)
	{
		Vector3 p = pt[GridIndex(i, j, k)];

		p.x -= 0.5f;
		p.y -= 0.5f;
		p.z -= 0.5f;

		return Vector3.Scale(p, lsize);
	}

	void Reset()
	{
		MegaModifyObject modobj = (MegaModifyObject)gameObject.GetComponent<MegaModifyObject>();

		if ( modobj != null )
			modobj.ModReset(this);

		if ( GetComponent<Renderer>() != null )
		{
			Mesh ms = MegaUtils.GetSharedMesh(gameObject);

			if ( ms != null )
			{
				Bounds b = ms.bounds;
				Offset = -b.center;
				bbox.min = b.center - b.extents;
				bbox.max = b.center + b.extents;
			}
		}

		bsize = bbox.Size();
		bcenter = bbox.center;
		Init();
	}

	public override bool InitMod(MegaModifiers mc)
	{
		bsize = mc.bbox.size;
		bcenter = mc.bbox.center;
		Init();
		return true;
	}

	static MegaFFD Create(GameObject go, int type)
	{
		switch ( type )
		{
			case 0: return go.AddComponent<MegaFFD2x2x2>();
			case 1: return go.AddComponent<MegaFFD3x3x3>();
			case 2: return go.AddComponent<MegaFFD4x4x4>();
		}

		return null;
	}

	public override void DrawGizmo(MegaModContext context)
	{
		//Gizmos.DrawCube(transform.position, new Vector3(bsize.x * 0.1f, bsize.y * 0.1f, bsize.z * 0.1f));
	}
}