
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaModifier))]
public class MegaModifierEditor : Editor
{
	public Texture		image;
	public bool			showhelp = false;
	public virtual Texture	LoadImage()			{ return null; }
	public virtual string	GetHelpString()	{ return "Modifer by Chris West"; }
	public virtual bool		Inspector()	{ return true; }
	public virtual bool		DisplayCommon() { return true; }

#if false
	[DrawGizmo(GizmoType.NotSelected | GizmoType.Pickable)]
	static void RenderGizmo(MegaModifier mod, GizmoType gizmoType)
	{
	}

	[DrawGizmo(GizmoType.SelectedOrChild | GizmoType.Pickable)]
	static void RenderGizmoSelected(MegaModifier mod, GizmoType gizmoType)
	{
		if ( GUI.changed )
		{
			Debug.Log("Editing " + mod.ModName());
		}
	}
#endif

	void OnDestroy()
	{
		MegaModifiers[] con = (MegaModifiers[])FindSceneObjectsOfType(typeof(MegaModifiers));

		for ( int i = 0; i < con.Length; i++ )
		{
			con[i].BuildList();
		}
	}

	public bool showmodparams = true;
	//bool showweight = true;

	public void CommonModParamsBasic(MegaModifier mod)
	{
		// Basic mod stuff
		//showmodparams = EditorGUILayout.Foldout(showmodparams, "Modifier Common Params");

		//if ( showmodparams )
		//{
			mod.ModEnabled = EditorGUILayout.Toggle("Mod Enabled", mod.ModEnabled);
			mod.DisplayGizmo = EditorGUILayout.Toggle("Display Gizmo", mod.DisplayGizmo);
			int order = EditorGUILayout.IntField("Order", mod.Order);

			if ( order != mod.Order )
			{
				mod.Order = order;

				MegaModifiers context = mod.GetComponent<MegaModifiers>();

				if ( context != null )
					context.BuildList();
			}

			mod.gizCol1 = EditorGUILayout.ColorField("Giz Col 1", mod.gizCol1);
			mod.gizCol2 = EditorGUILayout.ColorField("Giz Col 2", mod.gizCol2);
			mod.steps = EditorGUILayout.IntField("Gizmo Detail", mod.steps);
			if ( mod.steps < 1 )
				mod.steps = 1;
		//}

		//mod.useWeights = EditorGUILayout.Toggle("Use Weights", mod.useWeights);

		//if ( mod.useWeights )
		//	mod.weightChannel = (MegaWeightChannel)EditorGUILayout.EnumPopup("Weight Channel", mod.weightChannel);
	}

	public void CommonModParams(MegaModifier mod)
	{
		showmodparams = EditorGUILayout.Foldout(showmodparams, "Modifier Common Params");

		if ( showmodparams )
		{
			EditorGUILayout.BeginHorizontal();

			if ( GUILayout.Button("Rst Off") )
			{
				mod.Offset = Vector3.zero;
				EditorUtility.SetDirty(target);
			}

			if ( GUILayout.Button("Rst Pos") )
			{
				mod.gizmoPos = Vector3.zero;
				EditorUtility.SetDirty(target);
			}

			if ( GUILayout.Button("Rst Rot") )
			{
				mod.gizmoRot = Vector3.zero;
				EditorUtility.SetDirty(target);
			}

			if ( GUILayout.Button("Rst Scl") )
			{
				mod.gizmoScale = Vector3.one;
				EditorUtility.SetDirty(target);
			}
			EditorGUILayout.EndHorizontal();

			mod.Offset			= EditorGUILayout.Vector3Field("Offset", mod.Offset);
			mod.gizmoPos		= EditorGUILayout.Vector3Field("Gizmo Pos", mod.gizmoPos);
			mod.gizmoRot		= EditorGUILayout.Vector3Field("Gizmo Rot", mod.gizmoRot);
			mod.gizmoScale		= EditorGUILayout.Vector3Field("Gizmo Scale", mod.gizmoScale);
			CommonModParamsBasic(mod);
#if false
			mod.ModEnabled		= EditorGUILayout.Toggle("Mod Enabled", mod.ModEnabled);
			mod.DisplayGizmo	= EditorGUILayout.Toggle("Display Gizmo", mod.DisplayGizmo);

			int order = EditorGUILayout.IntField("Order", mod.Order);

			if ( order != mod.Order )
			{
				mod.Order = order;

				MegaModifiers context = mod.GetComponent<MegaModifiers>();

				if ( context != null )
					context.BuildList();
			}

			mod.gizCol1 = EditorGUILayout.ColorField("Giz Col 1", mod.gizCol1);
			mod.gizCol2 = EditorGUILayout.ColorField("Giz Col 2", mod.gizCol2);

			//showweight = EditorGUILayout.Foldout(showweight, "Modifier Weight Params");

			//if ( showweight )
			{
				//mod.useWeights = EditorGUILayout.Toggle("Use Weights", mod.useWeights);

				//if ( mod.useWeights )
				//{
					//mod.weightChannel = (MegaWeightChannel)EditorGUILayout.EnumPopup("Weight Channel", mod.weightChannel);
				//}
			}
#endif
		}
	}

	public virtual void DrawGUI()
	{
		//showhelp = EditorGUILayout.Foldout(showhelp, "Help");

		//if ( showhelp )
		//{
			//if ( image == null )
				//image = LoadImage();

			//if ( image != null )
			//{
				//float w = Screen.width - 12.0f;
				//float h = (w / image.width) * image.height;

				//if ( h > image.height )
					//h = image.height;

				//GUILayout.Label((Texture)image, GUIStyle.none, GUILayout.Width(w), GUILayout.Height(h));
			//}
		//}

		if ( DisplayCommon() )
			CommonModParams((MegaModifier)target);

		if ( GUI.changed )
			EditorUtility.SetDirty(target);

		if ( Inspector() )
			DrawDefaultInspector();

		//if ( showhelp )
			//GUILayout.TextArea(GetHelpString());
	}

	public virtual void DrawSceneGUI()
	{
		MegaModifier mod = (MegaModifier)target;

		if ( mod.ModEnabled && mod.DisplayGizmo && MegaModifiers.GlobalDisplay && showmodparams )
		{
			MegaModifiers context = mod.GetComponent<MegaModifiers>();

			if ( context != null && context.Enabled && context.DrawGizmos )
			{
				//mod.Offset = -Handles.PositionHandle(-mod.Offset, Quaternion.identity);
				float a = mod.gizCol1.a;
				Color col = Color.white;

				Quaternion rot = mod.transform.localRotation;

				Handles.matrix = Matrix4x4.identity;

				if ( mod.Offset != Vector3.zero )
				{
					Vector3 pos = mod.transform.localToWorldMatrix.MultiplyPoint(-mod.Offset);
					Handles.Label(pos, mod.ModName() + " Offset\n" + mod.Offset.ToString("0.000"));
					col = Color.blue;
					col.a = a;
					Handles.color = col;
					Handles.ArrowCap(0, pos, rot * Quaternion.Euler(180.0f, 0.0f, 0.0f), mod.GizmoSize());
					col = Color.green;
					col.a = a;
					Handles.color = col;
					Handles.ArrowCap(0, pos, rot * Quaternion.Euler(90.0f, 0.0f, 0.0f), mod.GizmoSize());
					col = Color.red;
					col.a = a;
					Handles.color = col;
					Handles.ArrowCap(0, pos, rot * Quaternion.Euler(0.0f, -90.0f, 0.0f), mod.GizmoSize());
				}

				// gizmopos
				if ( mod.gizmoPos != Vector3.zero )
				{
					Vector3 pos = mod.transform.localToWorldMatrix.MultiplyPoint(-mod.gizmoPos);
					Handles.Label(pos, mod.ModName() + " Pos\n" + mod.gizmoPos.ToString("0.000"));
					col = Color.blue;
					col.a = a;
					Handles.color = col;
					Handles.ArrowCap(0, pos, rot * Quaternion.Euler(180.0f, 0.0f, 0.0f), mod.GizmoSize());
					col = Color.green;
					col.a = a;
					Handles.color = col;
					Handles.ArrowCap(0, pos, rot * Quaternion.Euler(90.0f, 0.0f, 0.0f), mod.GizmoSize());
					col = Color.red;
					col.a = a;
					Handles.color = col;
					Handles.ArrowCap(0, pos, rot * Quaternion.Euler(0.0f, -90.0f, 0.0f), mod.GizmoSize());
				}

				Handles.matrix = Matrix4x4.identity;
			}
		}
	}

	public override void OnInspectorGUI()
	{
		DrawGUI();

		if ( GUI.changed )
			EditorUtility.SetDirty(target);
	}

	public void OnSceneGUI()
	{
		DrawSceneGUI();
	}
}