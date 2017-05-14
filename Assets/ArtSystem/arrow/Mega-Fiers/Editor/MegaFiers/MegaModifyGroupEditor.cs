
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaModifyGroup))]
public class MegaModifyGroupEditor : Editor
{
	Texture image;
	//bool showhelp = false;
	bool showorder = false;
	bool showmulti = false;

	//[DrawGizmo(GizmoType.SelectedOrChild)]
	//static void RenderGizmoSelected(ModifyObject mod, GizmoType gizmoType)
	//{
	//mod.ColliderTest();
	//}

	bool targets = false;

	public override void OnInspectorGUI()
	{
		MegaModifyGroup mod = (MegaModifyGroup)target;

		EditorGUIUtility.LookLikeInspector();
		MegaModifiers.GlobalDisplay = EditorGUILayout.Toggle("GlobalDisplayGizmos", MegaModifiers.GlobalDisplay);
		mod.Enabled = EditorGUILayout.Toggle("Enabled", mod.Enabled);
		mod.recalcnorms = EditorGUILayout.Toggle("Recalc Normals", mod.recalcnorms);
		MegaNormalMethod method = mod.NormalMethod;
		mod.NormalMethod = (MegaNormalMethod)EditorGUILayout.EnumPopup("Normal Method", mod.NormalMethod);
		mod.recalcbounds = EditorGUILayout.Toggle("Recalc Bounds", mod.recalcbounds);
		mod.recalcCollider = EditorGUILayout.Toggle("Recalc Collider", mod.recalcCollider);
		mod.recalcTangents = EditorGUILayout.Toggle("Recalc Tangents", mod.recalcTangents);
		mod.DoLateUpdate = EditorGUILayout.Toggle("Do Late Update", mod.DoLateUpdate);
		mod.GrabVerts = EditorGUILayout.Toggle("Grab Verts", mod.GrabVerts);
		mod.DrawGizmos = EditorGUILayout.Toggle("Draw Gizmos", mod.DrawGizmos);

		if ( mod.NormalMethod != method && mod.NormalMethod == MegaNormalMethod.Mega )
		{
			mod.BuildNormalMapping(mod.mesh, false);
		}

		if ( GUILayout.Button("Threading Options") )
			showmulti = !showmulti;

		if ( showmulti )
		{
			MegaModifiers.ThreadingOn = EditorGUILayout.Toggle("Threading Enabled", MegaModifiers.ThreadingOn);
			mod.UseThreading = EditorGUILayout.Toggle("Thread This Object", mod.UseThreading);
		}

		EditorGUIUtility.LookLikeControls();

		if ( GUI.changed )
			EditorUtility.SetDirty(target);

		showorder = EditorGUILayout.Foldout(showorder, "Modifier Order");

		if ( showorder && mod.mods != null )
		{
			for ( int i = 0; i < mod.mods.Length; i++ )
			{
				EditorGUILayout.LabelField("", i.ToString() + " - " + mod.mods[i].ModName() + " " + mod.mods[i].Order);
			}
		}

		if ( GUILayout.Button("Targets") )
			targets = !targets;

		if ( targets )
		{
			if ( GUILayout.Button("Add Target") )
			{
				MegaModifierTarget targ = new MegaModifierTarget();
				mod.targets.Add(targ);
			}

			for ( int i = 0; i < mod.targets.Count; i++ )
			{
				EditorGUILayout.BeginHorizontal();
				mod.targets[i].go = (GameObject)EditorGUILayout.ObjectField("Target " + i, mod.targets[i].go, typeof(GameObject), true);
				if ( GUILayout.Button("Del") )
				{
					mod.targets.Remove(mod.targets[i]);
					i--;
				}
				EditorGUILayout.EndHorizontal();
			}
		}
	}
}