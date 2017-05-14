
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

[CustomEditor(typeof(MegaMorphOMatic))]
public class MegaMorphOMaticEditor : Editor
{
	static string lastpath = " ";

	static public Color ChanCol1 = new Color(0.44f, 0.67f, 1.0f);
	static public Color ChanCol2 = new Color(1.0f, 0.67f, 0.44f);

	Stack<Color> bcol = new Stack<Color>();
	Stack<Color> ccol = new Stack<Color>();
	Stack<Color> col  = new Stack<Color>();

	bool extraparams	= false;
	bool showmodparams	= false;
	bool showchannels	= true;
	MegaMorphChan	currentChan;
	MegaMorphTarget	currentTarget;

	// Remove morph and pass tolerance then can morph to Utils
	// TODO: report error if target vert counts dont match base mapping
	bool DoMapping(MegaModifiers mod, MegaMorphOMatic morph, MegaTargetMesh tm, float scale, bool flipyz, bool negx)
	{
		for ( int i = 0; i < mod.verts.Length; i++ )
		{
			float a = (float)i / (float)mod.verts.Length;

			EditorUtility.DisplayProgressBar("Mapping", "Mapping vertex " + i, a);
			int map = MegaUtils.FindVert(mod.verts[i], tm.verts, morph.tolerance, scale, flipyz, negx, i);

			if ( map == -1 )
			{
				// Failed
				EditorUtility.ClearProgressBar();
				return false;
			}
		}

		EditorUtility.ClearProgressBar();
		return true;
	}

	void DisplayTarget(MegaMorphOMatic morph, MegaMorphChan channel, MegaMorphTarget mt, int num)
	{
		PushCols();
		EditorGUI.indentLevel = 1;
		mt.name = EditorGUILayout.TextField("Name", mt.name);
		//mt.percent = EditorGUILayout.Slider("Percent", mt.percent, 0.0f, 100.0f);
		mt.percent = EditorGUILayout.Slider("Percent", mt.percent, channel.mSpinmin, channel.mSpinmax);	//.0f, 100.0f);

		EditorGUILayout.BeginHorizontal();

		if ( mt.points == null || mt.points.Length != morph.oPoints.Length )
			GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
		else
			GUI.backgroundColor = new Color(0.0f, 1.0f, 0.0f);

		GUI.backgroundColor = new Color(1.0f, 0.5f, 0.5f);
		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel = 0;
		PopCols();
	}

	// These should be in EditorUtils
	void PushCols()
	{
		bcol.Push(GUI.backgroundColor);
		ccol.Push(GUI.contentColor);
		col.Push(GUI.color);
	}

	void PopCols()
	{
		GUI.backgroundColor = bcol.Pop();
		GUI.contentColor = ccol.Pop();
		GUI.color = col.Pop();
	}

	// This is common to other morpher
	void DisplayChannel(MegaMorphOMatic morph, MegaMorphChan channel)
	{
		if ( GUILayout.Button(channel.mName) )
			channel.showparams = !channel.showparams;

		GUI.backgroundColor = new Color(1, 1, 1);
		if ( channel.showparams )
		{
			channel.mName = EditorGUILayout.TextField("Name", channel.mName);

			if ( channel.mTargetCache != null && channel.mTargetCache.Count > 0 )
			{
				channel.mActiveOverride = EditorGUILayout.Toggle("Active", channel.mActiveOverride);
				channel.Percent = EditorGUILayout.Slider("Percent", channel.Percent, channel.mSpinmin, channel.mSpinmax);	//.0f, 100.0f);
				channel.mCurvature = EditorGUILayout.FloatField("Tension", channel.mCurvature);
			}

			channel.mUseLimit = EditorGUILayout.Toggle("Use Limit", channel.mUseLimit);

			if ( channel.mUseLimit )
			{
				channel.mSpinmin = EditorGUILayout.FloatField("Min", channel.mSpinmin);
				channel.mSpinmax = EditorGUILayout.FloatField("Max", channel.mSpinmax);
			}

			EditorGUILayout.BeginHorizontal();
			PushCols();
			GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f);

			GUI.backgroundColor = new Color(1.5f, 0.5f, 0.5f);
			if ( GUILayout.Button("Delete Channel") )
				morph.chanBank.Remove(channel);

			EditorGUILayout.EndHorizontal();

			PopCols();

			if ( channel.mTargetCache != null && channel.mTargetCache.Count > 0 )
			{
				channel.showtargets = EditorGUILayout.Foldout(channel.showtargets, "Targets");

				if ( channel.showtargets )
				{
					if ( channel.mTargetCache != null )
					{
						for ( int i = 0; i < channel.mTargetCache.Count; i++ )
							DisplayTarget(morph, channel, channel.mTargetCache[i], i);
					}
				}
			}
		}
		else
		{
			if ( channel.mActiveOverride && channel.mTargetCache != null && channel.mTargetCache.Count > 0 )
			{
				channel.Percent = EditorGUILayout.Slider("Percent", channel.Percent, channel.mSpinmin, channel.mSpinmax);	//.0f, 100.0f);
			}
		}
	}

	public override void OnInspectorGUI()
	{
		MegaMorphOMatic morph = (MegaMorphOMatic)target;

		PushCols();

		if ( GUILayout.Button("Import MorphOMatic File") )
		{
			LoadMorph();
			EditorUtility.SetDirty(target);
		}

		// Basic mod stuff
		showmodparams = EditorGUILayout.Foldout(showmodparams, "Modifier Common Params");

		if ( showmodparams )
		{
			morph.ModEnabled = EditorGUILayout.Toggle("Mod Enabled", morph.ModEnabled);
			morph.DisplayGizmo = EditorGUILayout.Toggle("Display Gizmo", morph.DisplayGizmo);
			morph.Order = EditorGUILayout.IntField("Order", morph.Order);
			morph.gizCol1 = EditorGUILayout.ColorField("Giz Col 1", morph.gizCol1);
			morph.gizCol2 = EditorGUILayout.ColorField("Giz Col 2", morph.gizCol2);
		}

		morph.animate = EditorGUILayout.Toggle("Animate", morph.animate);

		if ( morph.animate )
		{
			morph.animtime = EditorGUILayout.FloatField("AnimTime", morph.animtime);
			morph.looptime = EditorGUILayout.FloatField("LoopTime", morph.looptime);
			morph.speed = EditorGUILayout.FloatField("Speed", morph.speed);
			morph.repeatMode = (MegaRepeatMode)EditorGUILayout.EnumPopup("RepeatMode", morph.repeatMode);
		}

		EditorGUILayout.BeginHorizontal();
		PushCols();
		if ( morph.mapping == null || morph.mapping.Length == 0 )
			GUI.backgroundColor = Color.red;
		else
			GUI.backgroundColor = Color.green;

		PopCols();

		if ( GUILayout.Button("Add Channel") )
		{
			if ( morph.chanBank == null )
				morph.chanBank = new List<MegaMorphChan>();

			MegaMorphChan nc = new MegaMorphChan();
			nc.mName = "Empty";
			morph.chanBank.Add(nc);
		}

		EditorGUILayout.EndHorizontal();

		string bname = "Hide Channels";

		if ( !showchannels )
			bname = "Show Channels";

		if ( GUILayout.Button(bname) )
			showchannels = !showchannels;

		if ( showchannels && morph.chanBank != null )
		{
			for ( int i = 0; i < morph.chanBank.Count; i++ )
			{
				PushCols();

				if ( (i & 1) == 0 )
					GUI.backgroundColor = ChanCol1;
				else
					GUI.backgroundColor = ChanCol2;

				DisplayChannel(morph, morph.chanBank[i]);
				PopCols();
			}
		}

		extraparams = EditorGUILayout.Foldout(extraparams, "Extra Params");

		if ( extraparams )
		{
			ChanCol1 = EditorGUILayout.ColorField("Channel Col 1", ChanCol1);
			ChanCol2 = EditorGUILayout.ColorField("Channel Col 2", ChanCol2);
		}

		PopCols();

		if ( GUI.changed )
			EditorUtility.SetDirty(target);
	}

	public void ParseFile(String assetpath, ParseClassCallbackType cb)
	{
		FileStream fs = new FileStream(assetpath, FileMode.Open, FileAccess.Read, System.IO.FileShare.Read);

		BinaryReader br = new BinaryReader(fs);

		bool processing = true;

		while ( processing )
		{
			string classname = MegaParse.ReadString(br);

			if ( classname == "Done" )
				break;

			int	chunkoff = br.ReadInt32();
			long fpos = fs.Position;

			cb(classname, br);

			fs.Position = fpos + chunkoff;
		}

		br.Close();
	}

	void MorphCallback(string classname, BinaryReader br)
	{
		switch ( classname )
		{
			case "Morph": LoadMorph(br); break;
		}
	}

	void LoadMorph()
	{
		MegaMorphOMatic mr = (MegaMorphOMatic)target;

		string filename = EditorUtility.OpenFilePanel("Morph-O-Matic Morph File", lastpath, "mmf");

		if ( filename == null || filename.Length < 1 )
			return;

		lastpath = filename;

		// Clear what we have
		mr.chanBank.Clear();

		ParseFile(filename, MorphCallback);

		mr.animate = false;
		float looptime = 0.0f;
		// Set Looptime and animate if there is an anim
		for ( int i = 0; i < mr.chanBank.Count; i++ )
		{
			MegaMorphChan mc = mr.chanBank[i];

			if ( mc.control != null )
			{
				mr.animate = true;
				float t = mc.control.Times[mc.control.Times.Length - 1];
				if ( t > looptime )
					looptime = t;
			}
		}

		if ( mr.animate )
			mr.looptime = looptime;

		BuildData();
	}

	public void LoadMorph(BinaryReader br)
	{
		MegaParse.Parse(br, ParseMorph);
	}

	bool AnimCallback(BinaryReader br, string id)
	{
		MegaMorphOMatic mr = (MegaMorphOMatic)target;

		switch ( id )
		{
			case "Chan":
				int cn = br.ReadInt32();
				currentChan = mr.chanBank[cn]; break;
			case "Anim": currentChan.control = LoadAnim(br); break;
			default: return false;
		}

		return true;
	}

	void LoadAnimation(MegaMorphOMatic mr, BinaryReader br)
	{
		MegaParse.Parse(br, AnimCallback);
	}

	// Fbx could have been exported any which way so still need to do try all mappings to find correct
	public bool ParseMorph(BinaryReader br, string id)
	{
		MegaMorphOMatic mr = (MegaMorphOMatic)target;

		switch ( id )
		{
			case "Max": mr.Max = br.ReadSingle(); break;
			case "Min": mr.Min = br.ReadSingle(); break;
			case "UseLim": mr.UseLimit = (br.ReadInt32() == 1); break;

				// This will only be changed points, but we need scale
			case "StartPoints":	// Mapping
				MegaTargetMesh tm = new MegaTargetMesh();
				tm.verts = MegaParse.ReadP3l(br);	// make a vector
				if ( !TryMapping1(tm, mr) )
				{
					EditorUtility.DisplayDialog("Mapping Failed!", "Mapping failed!", "OK");
					EditorUtility.ClearProgressBar();
					return false;
				}
				break;

			case "Channel":		mr.chanBank.Add(LoadChan(br));	break;
			case "Animation":	LoadAnimation(mr, br);			break;
			default: return false;
		}

		return true;
	}

	public MegaMorphChan LoadChan(BinaryReader br)
	{
		MegaMorphChan chan = new MegaMorphChan();

		chan.control = null;
		chan.showparams = false;
		chan.mTargetCache = new List<MegaMorphTarget>();
		currentChan = chan;

		MegaParse.Parse(br, ParseChan);

		return chan;
	}

	public static MegaBezFloatKeyControl LoadAnim(BinaryReader br)
	{
		//MegaBezFloatKeyControl con = new MegaBezFloatKeyControl();

		//MegaParse.Parse(br, con.Parse);
		//return con;
		return MegaParseBezFloatControl.LoadBezFloatKeyControl(br);
	}

	public bool ParseChan(BinaryReader br, string id)
	{
		switch ( id )
		{
			case "Target":		currentChan.mTargetCache.Add(LoadTarget(br)); break;
			case "Name":		currentChan.mName = MegaParse.ReadString(br); break;
			case "Percent":		currentChan.Percent = br.ReadSingle(); break;
			case "SpinMax":		currentChan.mSpinmax = br.ReadSingle(); break;
			case "SpinMin":		currentChan.mSpinmin = br.ReadSingle(); break;
			case "UseLim":		currentChan.mUseLimit = (br.ReadInt32() == 1); break;
			case "Override":	currentChan.mActiveOverride = (br.ReadInt32() == 1); break;
			case "Curve":		currentChan.mCurvature = br.ReadSingle(); break;
		}

		return true;
	}

	Vector3 ConvertPoint(Vector3 v)
	{
		MegaMorphOMatic mr = (MegaMorphOMatic)target;

		Vector3 p = v * mr.importScale;

		if ( mr.negx )
			p.x = -p.x;

		if ( mr.flipyz )
		{
			float y = p.y;
			p.y = p.z;
			p.z = y;
		}

		return p;
	}

	public bool ParseTarget(BinaryReader br, string id)
	{
		switch ( id )
		{
			case "Name":	currentTarget.name		= MegaParse.ReadString(br); break;
			case "Percent": currentTarget.percent	= br.ReadSingle(); break;
			case "MoPoints":
				int count = br.ReadInt32();

				if ( count > 0 )
				{
					currentTarget.loadpoints = new MOPoint[count];

					for ( int i = 0; i < count; i++ )
					{
						MOPoint p = new MOPoint();
						p.id = br.ReadInt32();	// we need to find the ids for this point (could be more than one)
						p.p = ConvertPoint(MegaParse.ReadP3(br));

						p.w = br.ReadSingle();
						currentTarget.loadpoints[i] = p;
					}
				}
				break;
		}

		return true;
	}

	public MegaMorphTarget LoadTarget(BinaryReader br)
	{
		MegaMorphTarget target = new MegaMorphTarget();
		currentTarget = target;

		MegaParse.Parse(br, ParseTarget);
		return target;
	}

	// We should know the mapping
	// remove morph pass tolerance instead
	bool TryMapping1(MegaTargetMesh tm, MegaMorphOMatic morph)
	{
		MegaModifiers mod = morph.GetComponent<MegaModifiers>();

		if ( mod == null )
		{
			EditorUtility.DisplayDialog("Missing ModifyObject!", "No ModifyObject script found on the object", "OK");
			return false;
		}

		// Get extents for mod verts and for imported meshes, if not the same then scale
		Vector3 min1,max1;
		Vector3 min2,max2;

		Vector3 ex1 = MegaUtils.Extents(mod.verts, out min1, out max1);
		Vector3 ex2 = MegaUtils.Extents(tm.verts, out min2, out max2);

		// need min max on all axis so we can produce an offset to add
		float d1 = ex1.x;
		float d2 = ex2.x;

		float scl = d1 / d2;	//d2 / d1;
		bool flipyz = false;
		bool negx = false;

		// So try to match first vert using autoscale and no flip
		bool mapped = DoMapping(mod, morph, tm, scl, flipyz, negx);

		if ( !mapped )
		{
			flipyz = true;
			mapped = DoMapping(mod, morph, tm, scl, flipyz, negx);
			if ( !mapped )	//DoMapping(mod, morph, tm, mapping, scl, flipyz, negx) )
			{
				flipyz = false;
				negx = true;
				mapped = DoMapping(mod, morph, tm, scl, flipyz, negx);
				if ( !mapped )
				{
					flipyz = true;
					mapped = DoMapping(mod, morph, tm, scl, flipyz, negx);
				}
			}
		}

		if ( mapped )
		{
			morph.importScale = scl;
			morph.flipyz = flipyz;
			morph.negx = negx;

			// if mapping was ok set opoints
			morph.oPoints = tm.verts.ToArray();

			for ( int i = 0; i < morph.oPoints.Length; i++ )
			{
				Vector3 p = morph.oPoints[i];

				if ( negx )
					p.x = -p.x;

				if ( flipyz )
				{
					float z = p.z;
					p.z = p.y;
					p.y = z;
				}

				morph.oPoints[i] = p * morph.importScale;
			}

			morph.mapping = new MegaMomVertMap[morph.oPoints.Length];

			for ( int i = 0; i < morph.oPoints.Length; i++ )
			{
				//int[] indices = morph.FindVerts(morph.oPoints[i], mod);
				int[] indices = FindVerts(morph.oPoints[i], mod);

				morph.mapping[i] = new MegaMomVertMap();
				morph.mapping[i].indices = indices;	//morph.FindVerts(morph.oPoints[i], mod);
			}

			return true;
		}

		return false;
	}

	bool GetDelta(MegaMorphTarget targ, int v, out Vector3 delta, out float w)
	{
		MegaMorphOMatic mod = (MegaMorphOMatic)target;

		if ( targ.loadpoints != null )
		{
			for ( int i = 0; i < targ.loadpoints.Length; i++ )
			{
				int id = targ.loadpoints[i].id;
				if ( id == v )
				{
					delta = targ.loadpoints[i].p - mod.oPoints[id];
					w = targ.loadpoints[i].w;
					return true;
				}
			}
		}

		delta = Vector3.zero;
		w = 0.0f;
		return false;
	}

	public int[] FindVerts(Vector3 p, MegaModifiers mods)
	{
		List<int>	indices = new List<int>();
		for ( int i = 0; i < mods.verts.Length; i++ )
		{
			float dist = Vector3.Distance(p, mods.verts[i]);
			if ( dist < 0.0001f )	//mods.verts[i].Equals(p)  )
				indices.Add(i);
		}
		return indices.ToArray();
	}

	// Build the morphing data
	// each target holds only the points that differe from the base so need to build table showing for each
	// target the points that differ
	public void BuildData()
	{
		MegaMorphOMatic mod = (MegaMorphOMatic)target;

		List<MOMVert>	verts = new List<MOMVert>();

		for ( int c = 0; c < mod.chanBank.Count; c++ )
		{
			MegaMorphChan chan = mod.chanBank[c];

			int maxverts = 0;

			for ( int t = 0; t < chan.mTargetCache.Count - 1; t++ )
			{
				MegaMorphTarget targ = chan.mTargetCache[t];
				MegaMorphTarget targ1 = chan.mTargetCache[t + 1];

				// if t is 0 then just use the points
				Vector3 delta = Vector3.zero;
				Vector3 delta1 = Vector3.zero;

				float w = 1.0f;

				verts.Clear();

				for ( int v = 0; v < mod.oPoints.Length; v++ )
				{
					bool t1 = GetDelta(targ, v, out delta, out w);
					bool t2 = GetDelta(targ1, v, out delta1, out w);

					if ( t1 || t2 )	//GetDelta(targ, v, out delta, out w) || GetDelta(targ1, v, out delta1, out w) )
					{
						MOMVert vert = new MOMVert();

						vert.id = v;
						vert.w = w;
						vert.start = delta;
						vert.delta = delta1 - delta;

						verts.Add(vert);
					}
				}

				if ( verts.Count > maxverts )
					maxverts = verts.Count;

				if ( verts.Count > 0 )
					targ.mompoints = verts.ToArray();
			}

			for ( int t = 0; t < chan.mTargetCache.Count; t++ )
				chan.mTargetCache[t].loadpoints = null;

			chan.diff = new Vector3[maxverts];
		}
	}
}
