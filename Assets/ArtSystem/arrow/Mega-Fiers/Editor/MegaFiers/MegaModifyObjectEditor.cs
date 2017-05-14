
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaModifyObject))]
public class MegaModifyObjectEditor : Editor
{
	Texture image;
	//bool showhelp = false;
	bool showorder = false;
	bool showmulti = false;

	bool showgroups = false;
	//[DrawGizmo(GizmoType.SelectedOrChild)]
	//static void RenderGizmoSelected(ModifyObject mod, GizmoType gizmoType)
	//{
		//mod.ColliderTest();
	//}

	public override void OnInspectorGUI()
	{
		MegaModifyObject mod = (MegaModifyObject)target;

		//showhelp = EditorGUILayout.Foldout(showhelp, "Help");

		//if ( showhelp )
		//{
			//if ( image == null )
				//image = (Texture)EditorGUIUtility.LoadRequired("mod_help.png");

			//if ( image != null )
			//{
				//float w = Screen.width - 12.0f;
				//float h = (w / image.width) * image.height;
				//GUILayout.Label((Texture)image, GUIStyle.none, GUILayout.Width(w), GUILayout.Height(h));
			//}
		//}

		EditorGUIUtility.LookLikeInspector();
		MegaModifiers.GlobalDisplay = EditorGUILayout.Toggle("GlobalDisplayGizmos", MegaModifiers.GlobalDisplay);
		mod.Enabled			= EditorGUILayout.Toggle("Enabled", mod.Enabled);
		mod.recalcnorms		= EditorGUILayout.Toggle("Recalc Normals", mod.recalcnorms);
		MegaNormalMethod method = mod.NormalMethod;
		mod.NormalMethod	= (MegaNormalMethod)EditorGUILayout.EnumPopup("Normal Method", mod.NormalMethod);
		mod.recalcbounds	= EditorGUILayout.Toggle("Recalc Bounds", mod.recalcbounds);
		mod.recalcCollider	= EditorGUILayout.Toggle("Recalc Collider", mod.recalcCollider);
		mod.recalcTangents	= EditorGUILayout.Toggle("Recalc Tangents", mod.recalcTangents);
		mod.DoLateUpdate	= EditorGUILayout.Toggle("Do Late Update", mod.DoLateUpdate);
		mod.GrabVerts		= EditorGUILayout.Toggle("Grab Verts", mod.GrabVerts);
		mod.DrawGizmos		= EditorGUILayout.Toggle("Draw Gizmos", mod.DrawGizmos);

		if ( mod.NormalMethod != method && mod.NormalMethod == MegaNormalMethod.Mega )
		{
			mod.BuildNormalMapping(mod.mesh, false);
		}

		//showmulti = EditorGUILayout.Foldout(showmulti, "Multi Core");

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

		// Group stuff
		if ( GUILayout.Button("Group Members") )
			showgroups = !showgroups;

		if ( showgroups )
		{
			//if ( GUILayout.Button("Add Object") )
			//{
				//MegaModifierTarget targ = new MegaModifierTarget();
			//	mod.group.Add(targ);
			//}

			for ( int i = 0; i < mod.group.Count; i++ )
			{
				EditorGUILayout.BeginHorizontal();
				mod.group[i] = (GameObject)EditorGUILayout.ObjectField("Obj " + i, mod.group[i], typeof(GameObject), true);
				if ( GUILayout.Button("Del") )
				{
					mod.group.Remove(mod.group[i]);
					i--;
				}
				EditorGUILayout.EndHorizontal();
			}

			GameObject newobj = (GameObject)EditorGUILayout.ObjectField("Add", null, typeof(GameObject), true);
			if ( newobj )
			{
				mod.group.Add(newobj);
			}

			if ( GUILayout.Button("Update") )
			{
				// for each group member check if it has a modify object comp, if not add one and copy values over
				// calculate box for all meshes and set, and set the Offset for each one
				// then for each modifier attached find or add and set instance value
				// in theory each gizmo should overlap the others

				// Have a method to update box and offsets if we allow moving in the group

			}
		}
	}
}
