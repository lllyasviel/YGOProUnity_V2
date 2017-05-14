
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MegaMorphAnimClip
{
	public string			name;
	public float			start;
	public float			end;
	public MegaRepeatMode	loop;

	public MegaMorphAnimClip(string _name, float _start, float _end, MegaRepeatMode _loop)
	{
		name = _name;
		start = _start;
		end = _end;
		loop = _loop;
	}
}

//public delegate bool MegaMorphAnimClipEvent(int clip, int id);

[AddComponentMenu("Modifiers/Morph Animator")]
[ExecuteInEditMode]
public class MegaMorphAnimator : MonoBehaviour
{
	public MegaMorphBase	morph;

	public List<MegaMorphAnimClip>	clips = new List<MegaMorphAnimClip>();

	public int current = 0;
	public float t = -1.0f;	// Current clip time
	public float at = 0.0f;
	//MegaMorphAnimClipEvent	listener;

	//public void SetListener(MegaMorphAnimClipEvent listen)
	//{
	//	listener = listen;
	//}

	public bool IsPlaying()
	{
		if ( t >= 0.0f )
			return true;

		return false;
	}

	public void SetTime(float time)
	{
		t = time;
	}

	public float GetTime()
	{
		return at;
	}

	public void PlayClip(int i)
	{
		if ( i < clips.Count )
		{
			current = i;
			t = 0.0f;
		}
	}

	public void PlayClip(string name)
	{
		for ( int i = 0; i < clips.Count; i++ )
		{
			if ( clips[i].name == name )
			{
				current = i;
				t = 0.0f;
				return;
			}
		}
	}

	public void Stop()
	{
		t = -1.0f;
	}

	public int AddClip(string name, float start, float end, MegaRepeatMode loop)
	{
		MegaMorphAnimClip clip = new MegaMorphAnimClip(name, start, end, loop);
		clips.Add(clip);
		return clips.Count - 1;
	}

	public string[] GetClipNames()
	{
		string[] names = new string[clips.Count];

		for ( int i = 0; i < clips.Count; i++ )
		{
			names[i] = clips[i].name;
		}

		return names;
	}

	void Update()
	{
		if ( morph == null )
			morph = (MegaMorphBase)GetComponent<MegaMorphBase>();

		if ( morph && clips.Count > 0 && current < clips.Count )
		{
			if ( t >= 0.0f )
			{
				t += Time.deltaTime;
				float dt = clips[current].end - clips[current].start;

				switch ( clips[current].loop )
				{
					case MegaRepeatMode.Loop:		at = Mathf.Repeat(t, dt);		break;
					case MegaRepeatMode.PingPong:	at = Mathf.PingPong(t, dt);		break;
					case MegaRepeatMode.Clamp:		at = Mathf.Clamp(t, 0.0f, dt);	break;
				}

				at += clips[current].start;
				morph.SetAnim(at);
			}
		}
	}
}