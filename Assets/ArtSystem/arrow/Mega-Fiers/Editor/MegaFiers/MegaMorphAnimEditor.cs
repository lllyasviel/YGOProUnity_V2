
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MegaMorphAnim))]
public class MegaMorphAnimEditor : Editor
{
	int GetIndex(string name, string[] channels)
	{
		int index = -1;
		for ( int i = 0; i < channels.Length; i++ )
		{
			if ( channels[i] == name )
			{
				index = i;
				break;
			}
		}
		return index;
	}

	// TODO: Need none in the popup to clear a channel
	public override void OnInspectorGUI()
	{
		MegaMorphAnim anim = (MegaMorphAnim)target;

		MegaMorph morph = anim.gameObject.GetComponent<MegaMorph>();

		if ( morph != null )
		{
			string[] channels = morph.GetChannelNames();

			int index = GetIndex(anim.SrcChannel, channels);
			index = EditorGUILayout.Popup("Source Channel", index, channels);

			if ( index != -1 )
			{
				anim.SrcChannel = channels[index];
				anim.SetChannel(morph, 0);
			}
			anim.Percent = EditorGUILayout.Slider("Percent", anim.Percent, 0.0f, 100.0f);

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel1, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel1 = channels[index];
					anim.SetChannel(morph, 1);
				}
				anim.Percent1 = EditorGUILayout.Slider("Percent", anim.Percent1, 0.0f, 100.0f);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel2, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel2 = channels[index];
					anim.SetChannel(morph, 2);
				}
				anim.Percent2 = EditorGUILayout.Slider("Percent", anim.Percent2, 0.0f, 100.0f);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel3, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel3 = channels[index];
					anim.SetChannel(morph, 3);
				}
				anim.Percent3 = EditorGUILayout.Slider("Percent", anim.Percent3, 0.0f, 100.0f);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel4, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel4 = channels[index];
					anim.SetChannel(morph, 4);
				}
				anim.Percent4 = EditorGUILayout.Slider("Percent", anim.Percent4, 0.0f, 100.0f);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel5, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel5 = channels[index];
					anim.SetChannel(morph, 5);
				}
				anim.Percent5 = EditorGUILayout.Slider("Percent", anim.Percent5, 0.0f, 100.0f);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel6, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel6 = channels[index];
					anim.SetChannel(morph, 6);
				}
				anim.Percent6 = EditorGUILayout.Slider("Percent", anim.Percent6, 0.0f, 100.0f);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel7, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel7 = channels[index];
					anim.SetChannel(morph, 7);
				}
				anim.Percent7 = EditorGUILayout.Slider("Percent", anim.Percent7, 0.0f, 100.0f);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel8, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel8 = channels[index];
					anim.SetChannel(morph, 8);
				}
				anim.Percent8 = EditorGUILayout.Slider("Percent", anim.Percent8, 0.0f, 100.0f);
			}

			if ( index != -1 )
			{
				index = GetIndex(anim.SrcChannel9, channels);
				index = EditorGUILayout.Popup("Source Channel", index, channels);
				if ( index != -1 )
				{
					anim.SrcChannel9 = channels[index];
					anim.SetChannel(morph, 9);
				}
				anim.Percent9 = EditorGUILayout.Slider("Percent", anim.Percent9, 0.0f, 100.0f);
			}

			if ( GUI.changed )
			{
				EditorUtility.SetDirty(target);
			}
		}
	}
}