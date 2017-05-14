
using UnityEngine;

[AddComponentMenu("Modifiers/Selection/Vert Color")]
public class MegaVertColSelect : MegaSelectionMod
{
	public override MegaModChannel ChannelsReq() { return MegaModChannel.Col; }
	public override string ModName() { return "Vert Color Select"; }
	public override string GetHelpURL() { return "?page_id=1305"; }

	public MegaChannel	channel = MegaChannel.Red;

	float[]	modselection;

	public float[] GetSel() { return modselection; }

	//public Color	gizCol = new Color(0.5f, 0.5f, 0.5f, 0.25f);
	public float	gizSize = 0.01f;
	public bool		displayWeights = true;
	public float	weight = 1.0f;
	public float	threshold = 0.0f;
	public bool		update = true;

	public override void GetSelection(MegaModifiers mc)
	{
		if ( ModEnabled )
		{
			if ( modselection == null || modselection.Length != mc.verts.Length )
				modselection = new float[mc.verts.Length];

			if ( update )
			{
				update = false;

				if ( mc.cols != null && mc.cols.Length > 0 )
				{
					int c = (int)channel;
					for ( int i = 0; i < mc.verts.Length; i++ )
						modselection[i] = ((mc.cols[i][c] - threshold) / (1.0f - threshold)) * weight;
				}
				else
				{
					for ( int i = 0; i < mc.verts.Length; i++ )
						modselection[i] = weight;
				}
			}

			mc.selection = modselection;
		}
	}
}
