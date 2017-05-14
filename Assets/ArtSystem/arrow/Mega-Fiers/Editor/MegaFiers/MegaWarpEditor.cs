
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaWarp))]
public class MegaWarpEditor : Editor
{
	[DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
	static void RenderGizmo(MegaWarp warp, GizmoType gizmoType)
	{
		if ( MegaModifiers.GlobalDisplay && warp.DisplayGizmo )
		{
			if ( (gizmoType & GizmoType.NotInSelectionHierarchy) != 0 )
			{
				if ( (gizmoType & GizmoType.Active) != 0 )
				{
					if ( warp.Enabled )
						warp.DrawGizmo(Color.white);
					else
						warp.DrawGizmo(new Color(1.0f, 1.0f, 0.0f, 0.75f));
				}
				else
				{
					if ( warp.Enabled )
						warp.DrawGizmo(new Color(0.0f, 1.0f, 0.0f, 0.5f));
					else
						warp.DrawGizmo(new Color(1.0f, 0.0f, 0.0f, 0.25f));
				}
			}
			Gizmos.DrawIcon(warp.transform.position, warp.GetIcon());
		}
	}

	//[DrawGizmo(GizmoType.SelectedOrChild)]
	//static void RenderGizmoSelected(Warp warp, GizmoType gizmoType)
	//{
		//if ( Modifiers.GlobalDisplay && warp.DisplayGizmo )
		//{
			//if ( warp.Enabled )
				//warp.DrawGizmo(Color.white);
			//else
				//warp.DrawGizmo(new Color(1.0f, 1.0f, 0.0f, 0.75f));

			//Gizmos.DrawIcon(warp.transform.position, warp.GetIcon());
		//}
	//}
}
