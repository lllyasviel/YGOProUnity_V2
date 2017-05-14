
using UnityEngine;

// Going to need a clear selection
public class MegaSelectionMod : MegaModifier
{
	//public override MegaModChannel ChannelsReq()		{ return MegaModChannel.Col; }
	public override MegaModChannel ChannelsChanged()	{ return MegaModChannel.Selection; }

	public virtual	void	GetSelection(MegaModifiers mc)	{ }
	public override bool	ModLateUpdate(MegaModContext mc)
	{
		GetSelection(mc.mod);
		return false;		// Dont need to do any mapping
	}

	public override void DrawGizmo(MegaModContext context)
	{
	}

	// TEST this is needed
	public override void DoWork(MegaModifiers mc, int index, int start, int end, int cores)
	{
		for ( int i = start; i < end; i++ )
			sverts[i] = verts[i];
	}

}

// This is purely a selection set, so a list of vert indices and weights for that
// to start with rgba channel info, but will also have painted weights
#if false
[AddComponentMenu("Modifiers/Selection")]
public class MegaSelection : MegaSelectionMod
{
	public override string ModName()	{ return "Selection"; }
	public override string GetHelpURL() { return "?page_id=1302 e"; }

	public MegaChannel	channel = MegaChannel.Red;

	public float weight = 1.0f;

	float[]	modselection;

	public float[] GetSel()	{ return modselection; }

	public override void GetSelection(MegaModifiers mc)
	{
		if ( modselection == null || modselection.Length != mc.verts.Length )
		{
			modselection = new float[mc.verts.Length];
		}

		//int c = (int)channel;

		// we dont need to update if nothing changes
		for ( int i = 0; i < mc.verts.Length; i++ )
		{
			modselection[i] = weight;	//mc.cols[i][c];
		}

		if ( weight == 1.0f )
			mc.selection = null;	// Normal system

		mc.selection = modselection;
	}
}
#endif
