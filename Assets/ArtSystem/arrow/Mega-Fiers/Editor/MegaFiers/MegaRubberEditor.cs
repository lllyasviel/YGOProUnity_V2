
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaRubber))]
public class MegaRubberEditor : MegaModifierEditor
{
	public override string GetHelpString() { return "Rubber Modifier by Chris West"; }
	public override Texture LoadImage() { return (Texture)EditorGUIUtility.LoadRequired("MegaFiers/bend_help.png"); }
	public override bool DisplayCommon()	{ return false; }

	public override bool Inspector()
	{
		MegaRubber mod = (MegaRubber)target;

		EditorGUIUtility.LookLikeControls();

		mod.ModEnabled = EditorGUILayout.Toggle("Enabled", mod.ModEnabled);

		MegaRubberType mattype = (MegaRubberType)EditorGUILayout.EnumPopup("Material", mod.Presets);

		if ( mattype != mod.Presets )
		{
			mod.Presets = mattype;
			mod.ChangeMaterial();
		}

		MegaWeightChannel channel = (MegaWeightChannel)EditorGUILayout.EnumPopup("Channel", mod.channel);

		if ( channel != mod.channel )
		{
			mod.channel = channel;
			mod.ChangeChannel();
		}

		channel = (MegaWeightChannel)EditorGUILayout.EnumPopup("Stiff Channel", mod.stiffchannel);

		if ( channel != mod.stiffchannel )
		{
			mod.stiffchannel = channel;
			mod.ChangeChannel();
		}

		mod.threshold = EditorGUILayout.Slider("Threshhold", mod.threshold, 0.0f, 1.0f);
		if ( GUILayout.Button("Apply Threshold") )
		{
			mod.ChangeChannel();
			EditorUtility.SetDirty(target);
		}

		mod.Intensity	= EditorGUILayout.Vector3Field("Intensity", mod.Intensity);
		mod.gravity		= EditorGUILayout.FloatField("Gravity", mod.gravity);
		mod.damping		= EditorGUILayout.Vector3Field("Damping", mod.damping);
		mod.mass		= EditorGUILayout.FloatField("Mass", mod.mass);
		mod.stiffness	= EditorGUILayout.Vector3Field("Stiffness", mod.stiffness);

		mod.showweights = EditorGUILayout.Toggle("Show Weights", mod.showweights);
		mod.size = EditorGUILayout.FloatField("Size", mod.size * 100.0f) * 0.01f;
		return false;
	}

#if false
	public override void DrawSceneGUI()
	{
		MegaRubber mod = (MegaRubber)target;
		if ( mod.showweights && mod.vr != null )
		{
			Color col = Color.black;

			Matrix4x4 tm = mod.gameObject.transform.localToWorldMatrix;
			Handles.matrix = Matrix4x4.identity;

			for ( int i = 0; i < mod.vr.Length; i++ )
			{
				float w = mod.vr[i].weight;
				if ( w > 0.6666f )
					col = Color.Lerp(Color.green, Color.red, (w - 0.6666f) * 3.0f);
				else
				{
					if ( w > 0.3333f )
						col = Color.Lerp(Color.blue, Color.green, (w - 0.3333f) * 3.0f);
					else
					{
						Color nocol = new Color(0.0f, 0.0f, 1.0f, 0.0f);
						col = Color.Lerp(nocol, Color.blue, w * 3.0f);
					}
				}
				Handles.color = col;

				Vector3 p = tm.MultiplyPoint(mod.vr[i].cpos);
				Handles.DotCap(i, p, Quaternion.identity, mod.size);
			}

			Handles.matrix = Matrix4x4.identity;
		}
	}
#else
	public override void DrawSceneGUI()
	{
		MegaRubber mod = (MegaRubber)target;
		if ( mod.showweights && mod.vr != null )
		{
			Color col = Color.black;

			Matrix4x4 tm = mod.gameObject.transform.localToWorldMatrix;
			Handles.matrix = Matrix4x4.identity;

			for ( int i = 0; i < mod.vr.Length; i++ )
			{
				float w = mod.vr[i].weight;
				if ( w > 0.5f )
					col = Color.Lerp(Color.green, Color.red, (w - 0.5f) * 2.0f);
				else
					col = Color.Lerp(Color.blue, Color.green, w * 2.0f);
				Handles.color = col;

				Vector3 p = tm.MultiplyPoint(mod.vr[i].cpos);
				Handles.DotCap(i, p, Quaternion.identity, mod.size);
			}

			Handles.matrix = Matrix4x4.identity;
		}
	}
#endif
}