
using UnityEngine;

[AddComponentMenu("Modifiers/Selection/Volume")]
public class MegaVolSelect : MegaSelectionMod
{
	public override MegaModChannel ChannelsReq() { return MegaModChannel.Col | MegaModChannel.Verts; }

	public override string ModName() { return "Vol Select"; }
	public override string GetHelpURL() { return "?page_id=1307"; }

	//public MegaChannel	channel = MegaChannel.Red;

	float[]	modselection;

	public float[] GetSel() { return modselection; }

	public Vector3	origin = Vector3.zero;
	public float	falloff = 1.0f;
	public float	radius = 1.0f;
	public Color	gizCol = new Color(0.5f, 0.5f, 0.5f, 0.25f);
	public float	gizSize = 0.01f;
	public bool		useCurrentVerts = true;
	public bool		displayWeights = true;

	public override void GetSelection(MegaModifiers mc)
	{
		if ( modselection == null || modselection.Length != mc.verts.Length )
		{
			modselection = new float[mc.verts.Length];
		}

		// we dont need to update if nothing changes
		if ( useCurrentVerts )
		{
			for ( int i = 0; i < verts.Length; i++ )
			{
				float d = Vector3.Distance(origin, verts[i]) - radius;

				if ( d < 0.0f )
					modselection[i] = 1.0f;
				else
				{
					float w = Mathf.Exp(-falloff * Mathf.Abs(d));
					modselection[i] = w;	//mc.cols[i][c];
				}
			}
		}
		else
		{
			for ( int i = 0; i < verts.Length; i++ )
			{
				float d = Vector3.Distance(origin, verts[i]) - radius;

				if ( d < 0.0f )
					modselection[i] = 1.0f;
				else
				{
					float w = Mathf.Exp(-falloff * Mathf.Abs(d));
					modselection[i] = w;	//mc.cols[i][c];
				}
			}
		}
		//if ( weight == 1.0f )
		//	mc.selection = null;	// Normal system

		// We only need the copy if we are first mod
		if ( (mc.dirtyChannels & MegaModChannel.Verts) == 0 )
		{
			mc.InitVertSource();
			//verts.CopyTo(sverts, 0);
			//mc.UpdateMesh = 1;
		}

		//Debug.Log("sel " + modselection.Length);
		mc.selection = modselection;
	}
}
