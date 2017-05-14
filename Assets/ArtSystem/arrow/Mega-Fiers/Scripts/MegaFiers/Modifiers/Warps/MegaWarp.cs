
using UnityEngine;

// TODO: Add Enable value
// TODO: Range value
// TODO: DisplaceWarp
[ExecuteInEditMode]
public class MegaWarp : MonoBehaviour
{
	public float	Width = 1.0f;
	public float	Height = 1.0f;
	public float	Length = 1.0f;

	public float		Decay;
	public bool			Enabled = true;
	public bool			DisplayGizmo	 = true;
	public Color		GizCol1 = Color.yellow;
	public Color		GizCol2 = Color.green;

	[System.NonSerialized]
	public Matrix4x4			tm = new Matrix4x4();
	[System.NonSerialized]
	public Matrix4x4			invtm = new Matrix4x4();
	Vector3	Offset = Vector3.zero;

	int steps = 50;	// How many steps for the gizmo boxes

	[System.NonSerialized]
	public float	totaldecay;
	[HideInInspector]
	public Vector3[]	corners				= new Vector3[8];	// Make static

	public virtual string WarpName() { return "None"; }
	public virtual string GetHelpURL() { return "Warp.htm"; }

	public virtual Vector3 Map(int i, Vector3 p)	{ return p; }
	public virtual bool Prepare(float decay)	{ return true; }

	public virtual string GetIcon() { return "MegaWave icon.png"; }

	[ContextMenu("Help")]
	public void Help()
	{
		Application.OpenURL("http://www.west-racing.com/megadoc/" + GetHelpURL());
	}

	public virtual void SetAxis(Matrix4x4 tmAxis)
	{
		Matrix4x4 itm = tmAxis.inverse;
		tm = tmAxis * tm;
		invtm = invtm * itm;
	}

	public void DrawEdge(Vector3 p1, Vector3 p2)
	{
		Vector3 last = Map(-1, p1);
		Vector3 pos = Vector3.zero;
		for ( int i = 1; i <= steps; i++ )
		{
			pos = p1 + ((p2 - p1) * ((float)i / (float)steps));

			pos = Map(-1, pos);

			if ( (i & 4) == 0 )
				Gizmos.color = gCol1;	//GizCol1;
			else
				Gizmos.color = gCol2;	//GizCol2;

			Gizmos.DrawLine(last, pos);
			last = pos;
		}
		Gizmos.color = gCol1;	//GizCol1;
	}

	public void DrawEdgeCol(Vector3 p1, Vector3 p2)
	{
		Vector3 last = Map(-1, p1);
		Vector3 pos = Vector3.zero;
		for ( int i = 1; i <= steps; i++ )
		{
			pos = p1 + ((p2 - p1) * ((float)i / (float)steps));

			pos = Map(-1, pos);

			Gizmos.DrawLine(last, pos);
			last = pos;
		}
	}

	public static Color gCol1;
	public static Color gCol2;

	public void SetGizCols(float a)
	{
		gCol1 = GizCol1;
		gCol1.a *= a;
		gCol2 = GizCol2;
		gCol2.a *= a;
	}

	// TODO: allow bind to have a decay value as well, and mult the two together
	public virtual void DrawGizmo(Color col)
	{
		SetGizCols(col.a);

		tm = Matrix4x4.identity;
		invtm = tm.inverse;

		if ( !Prepare(0.0f) )
			return;

		tm = tm * transform.localToWorldMatrix;	// * tm;
		invtm = tm.inverse;

		Vector3 min = new Vector3(-Width * 0.5f, 0.0f, -Length * 0.5f);
		Vector3 max = new Vector3(Width * 0.5f, Height, Length * 0.5f);

		Gizmos.matrix = transform.localToWorldMatrix;

		//Gizmos.color = col;	//Color.yellow;
		corners[0] = new Vector3(min.x, min.y, min.z);
		corners[1] = new Vector3(min.x, max.y, min.z);
		corners[2] = new Vector3(max.x, max.y, min.z);
		corners[3] = new Vector3(max.x, min.y, min.z);

		corners[4] = new Vector3(min.x, min.y, max.z);
		corners[5] = new Vector3(min.x, max.y, max.z);
		corners[6] = new Vector3(max.x, max.y, max.z);
		corners[7] = new Vector3(max.x, min.y, max.z);

		DrawEdge(corners[0], corners[1]);
		DrawEdge(corners[1], corners[2]);
		DrawEdge(corners[2], corners[3]);
		DrawEdge(corners[3], corners[0]);

		DrawEdge(corners[4], corners[5]);
		DrawEdge(corners[5], corners[6]);
		DrawEdge(corners[6], corners[7]);
		DrawEdge(corners[7], corners[4]);

		DrawEdge(corners[0], corners[4]);
		DrawEdge(corners[1], corners[5]);
		DrawEdge(corners[2], corners[6]);
		DrawEdge(corners[3], corners[7]);

		ExtraGizmo();
	}

	public virtual void ExtraGizmo()
	{
	}

	public void DrawFromTo(MegaAxis axis, float from, float to)
	{
		Vector3 min = new Vector3(-Width * 0.5f, 0.0f, -Length * 0.5f);
		Vector3 max = new Vector3(Width * 0.5f, Height, Length * 0.5f);

		switch ( axis )
		{
			case MegaAxis.X:
				corners[0] = new Vector3(-from, min.y, min.z);
				corners[1] = new Vector3(-from, max.y, min.z);
				corners[2] = new Vector3(-from, max.y, max.z);
				corners[3] = new Vector3(-from, min.y, max.z);

				corners[4] = new Vector3(-to, min.y, min.z);
				corners[5] = new Vector3(-to, max.y, min.z);
				corners[6] = new Vector3(-to, max.y, max.z);
				corners[7] = new Vector3(-to, min.y, max.z);
				break;

			case MegaAxis.Y:
				corners[0] = new Vector3(min.x, min.y, -from);
				corners[1] = new Vector3(min.x, max.y, -from);
				corners[2] = new Vector3(max.x, max.y, -from);
				corners[3] = new Vector3(max.x, min.y, -from);

				corners[4] = new Vector3(min.x, min.y, -to);
				corners[5] = new Vector3(min.x, max.y, -to);
				corners[6] = new Vector3(max.x, max.y, -to);
				corners[7] = new Vector3(max.x, min.y, -to);
				break;

			case MegaAxis.Z:
				corners[0] = new Vector3(min.x, from, min.z);
				corners[1] = new Vector3(min.x, from, max.z);
				corners[2] = new Vector3(max.x, from, max.z);
				corners[3] = new Vector3(max.x, from, min.z);

				corners[4] = new Vector3(min.x, to, min.z);
				corners[5] = new Vector3(min.x, to, max.z);
				corners[6] = new Vector3(max.x, to, max.z);
				corners[7] = new Vector3(max.x, to, min.z);
				break;
		}

		Color c = Color.red;
		c.a = gCol1.a;
		Gizmos.color = c;

		DrawEdgeCol(corners[0] - Offset, corners[1] - Offset);
		DrawEdgeCol(corners[1] - Offset, corners[2] - Offset);
		DrawEdgeCol(corners[2] - Offset, corners[3] - Offset);
		DrawEdgeCol(corners[3] - Offset, corners[0] - Offset);

		c = Color.green;
		c.a = gCol1.a;
		Gizmos.color = c;

		DrawEdgeCol(corners[4] - Offset, corners[5] - Offset);
		DrawEdgeCol(corners[5] - Offset, corners[6] - Offset);
		DrawEdgeCol(corners[6] - Offset, corners[7] - Offset);
		DrawEdgeCol(corners[7] - Offset, corners[4] - Offset);
	}
}
