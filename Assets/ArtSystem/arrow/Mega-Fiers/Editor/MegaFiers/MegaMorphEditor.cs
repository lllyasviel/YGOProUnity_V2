
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class MegaTargetMesh
{
	public string name;
	public List<Vector3>	verts = new List<Vector3>();
	public List<int>		faces = new List<int>();

	static public List<MegaTargetMesh> LoadTargets(string path, float scale, bool flipzy, bool negx)
	{
		List<MegaTargetMesh>	targets = new List<MegaTargetMesh>();

		MegaTargetMesh current = null;

		StreamReader stream = File.OpenText(path);
		string entireText = stream.ReadToEnd();
		stream.Close();

		entireText = entireText.Replace("\n", "\r\n");

		List<Vector3>	verts = new List<Vector3>();

		using ( StringReader reader = new StringReader(entireText) )
		{
			string currentText = reader.ReadLine();

			char[] splitIdentifier = { ' ' };
			string[] brokenString;

			string name = "";

			Vector3 p = Vector3.zero;

			while ( currentText != null )
			{
				if ( !currentText.StartsWith("v ") && !currentText.StartsWith("g ") && !currentText.StartsWith("f ") )
				{
					currentText = reader.ReadLine();
					if ( currentText != null )
						currentText = currentText.Replace("  ", " ");
				}
				else
				{
					currentText = currentText.Trim();
					brokenString = currentText.Split(splitIdentifier, 50);
					switch ( brokenString[0] )
					{
						case "f":
							if ( verts.Count > 0 )
							{
								current = new MegaTargetMesh();
								current.name = name;
								current.verts = new List<Vector3>(verts);
								current.faces = new List<int>();
								targets.Add(current);

								verts.Clear();
							}
							break;

						case "g":
							name = brokenString[1];
							break;

						case "v":
							p.x = System.Convert.ToSingle(brokenString[1]) * scale;
							if ( negx )
							{
								p.x = -p.x;
							}

							if ( flipzy )
							{
								p.y = System.Convert.ToSingle(brokenString[3]) * scale;
								p.z = System.Convert.ToSingle(brokenString[2]) * scale;
							}
							else
							{
								p.y = System.Convert.ToSingle(brokenString[2]) * scale;
								p.z = System.Convert.ToSingle(brokenString[3]) * scale;
							}
							verts.Add(p);
							break;
					}

					currentText = reader.ReadLine();
					if ( currentText != null )
						currentText = currentText.Replace("  ", " ");
				}
			}
		}

		return targets;
	}
}

// Have a simple text format so can do a simple max exporter and others can do blender scripts etc
[CustomEditor(typeof(MegaMorph))]
public class MegaMorphEditor : Editor
{
	static string lastpath = " ";

	static public Color ChanCol1 = new Color(0.44f, 0.67f, 1.0f);
	static public Color ChanCol2 = new Color(1.0f, 0.67f, 0.44f);

	Stack<Color> bcol = new Stack<Color>();
	Stack<Color> ccol = new Stack<Color>();
	Stack<Color> col  = new Stack<Color>();

	bool extraparams = false;

	int FindVert(Vector3 vert, List<Vector3> verts, float tolerance)
	{
		float closest = Vector3.SqrMagnitude(verts[0] - vert);
		int find = 0;

		for ( int i = 0; i < verts.Count; i++ )
		{
			float dif = Vector3.SqrMagnitude(verts[i] - vert);

			if ( dif < closest )
			{
				closest = dif;
				find = i;
			}
		}

		if ( closest > tolerance )	//0.0001f )	// not exact
			return -1;

		return find;	//0;
	}

	int FindVert(Vector3 vert, List<Vector3> verts, float tolerance, float scl, bool flipyz, bool negx, int vn)
	{
		int find = 0;

		if ( negx )
			vert.x = -vert.x;

		if ( flipyz )
		{
			float z = vert.z;
			vert.z = vert.y;
			vert.y = z;
		}

		vert /= scl;

		float closest = Vector3.SqrMagnitude(verts[0] - vert);

		for ( int i = 0; i < verts.Count; i++ )
		{
			float dif = Vector3.SqrMagnitude(verts[i] - vert);

			if ( dif < closest )
			{
				closest = dif;
				find = i;
			}
		}

		if ( closest > tolerance )	//0.0001f )	// not exact
			return -1;

		return find;	//0;
	}

	bool FindMatch(Vector3 vert, Vector3[] verts, float tolerance)
	{
		float closest = Vector3.SqrMagnitude(verts[0] - vert);

		for ( int i = 0; i < verts.Length; i++ )
		{
			float dif = Vector3.SqrMagnitude(verts[i] - vert);

			if ( dif < closest )
				closest = dif;
		}

		if ( closest > tolerance )	//0.0001f )	// not exact
			return false;

		return true;
	}

	Vector3 Extents(Vector3[] verts, out Vector3 min, out Vector3 max)
	{
		Vector3 extent = Vector3.zero;

		min = Vector3.zero;
		max = Vector3.zero;

		if ( verts != null && verts.Length > 0 )
		{
			min = verts[0];
			max = verts[0];

			for ( int i = 1; i < verts.Length; i++ )
			{
				if ( verts[i].x < min.x )	min.x = verts[i].x;
				if ( verts[i].y < min.y ) min.y = verts[i].y;
				if ( verts[i].z < min.z ) min.z = verts[i].z;

				if ( verts[i].x > max.x ) max.x = verts[i].x;
				if ( verts[i].y > max.y ) max.y = verts[i].y;
				if ( verts[i].z > max.z ) max.z = verts[i].z;
			}

			extent = max - min;
		}

		return extent;
	}

	Vector3 Extents(List<Vector3> verts, out Vector3 min, out Vector3 max)
	{
		Vector3 extent = Vector3.zero;

		min = Vector3.zero;
		max = Vector3.zero;

		if ( verts != null && verts.Count > 0 )
		{
			min = verts[0];
			max = verts[0];

			for ( int i = 1; i < verts.Count; i++ )
			{
				if ( verts[i].x < min.x ) min.x = verts[i].x;
				if ( verts[i].y < min.y ) min.y = verts[i].y;
				if ( verts[i].z < min.z ) min.z = verts[i].z;

				if ( verts[i].x > max.x ) max.x = verts[i].x;
				if ( verts[i].y > max.y ) max.y = verts[i].y;
				if ( verts[i].z > max.z ) max.z = verts[i].z;
			}

			extent = max - min;
		}

		return extent;
	}

	// TODO: report error if target vert counts dont match base mapping
	bool DoMapping(MegaModifiers mod, MegaMorph morph, MegaTargetMesh tm, int[] mapping, float scale, bool flipyz, bool negx)
	{
		for ( int i = 0; i < mod.verts.Length; i++ )
		{
			float a = (float)i / (float)mod.verts.Length;

			EditorUtility.DisplayProgressBar("Mapping", "Mapping vertex " + i, a);
			mapping[i] = FindVert(mod.verts[i], tm.verts, morph.tolerance, scale, flipyz, negx, i);

			if ( mapping[i] == -1 )
			{
				// Failed
				EditorUtility.ClearProgressBar();
				return false;
			}
		}

		EditorUtility.ClearProgressBar();
		return true;
	}

	bool TryMapping(List<MegaTargetMesh> targets, MegaMorph morph)
	{
		MegaModifiers mod = morph.GetComponent<MegaModifiers>();

		if ( mod == null )
		{
			//Debug.Log("No modifyobject found");
			EditorUtility.DisplayDialog("Missing ModifyObject!", "No ModifyObject script found on the object", "OK");
			return false;
		}

		int[] mapping = new int[mod.verts.Length];

		for ( int i = 0; i < 18; i++ )
		{
			Debug.Log("v[" + i + "] " + mod.verts[i].ToString("0.00000"));
		}

		for ( int i = 0; i < 18; i++ )
		{
			Debug.Log("t[" + i + "] " + targets[0].verts[i].ToString("0.00000"));
		}

		for ( int t = 0; t < targets.Count;	t++ )
		{
			MegaTargetMesh tm = targets[t];

			// Get extents for mod verts and for imported meshes, if not the same then scale
			Vector3 min1,max1;
			Vector3 min2,max2;

			Vector3 ex1 = Extents(mod.verts, out min1, out max1);
			Vector3 ex2 = Extents(tm.verts, out min2, out max2);

			Debug.Log("min1 " + min1.ToString("0.000"));
			Debug.Log("max1 " + max1.ToString("0.000"));
			Debug.Log("min2 " + min2.ToString("0.000"));
			Debug.Log("max2 " + max2.ToString("0.000"));
			// need min max on all axis so we can produce an offset to add
			float d1 = ex1.x;
			float d2 = ex2.x;

			float scl = d1 / d2;	//d2 / d1;
			bool flipyz = false;
			bool negx = false;

			//Vector3 offset = (min2 * scl) - min1;
			//Debug.Log("offset " + offset.ToString("0.0000"));
			Debug.Log("scl " + scl);

			// So try to match first vert using autoscale and no flip
			bool mapped = DoMapping(mod, morph, tm, mapping, scl, flipyz, negx);
			
			if ( !mapped )
			{
				flipyz = true;
				mapped = DoMapping(mod, morph, tm, mapping, scl, flipyz, negx);
				if ( !mapped )	//DoMapping(mod, morph, tm, mapping, scl, flipyz, negx) )
				{
					flipyz = false;
					negx = true;
					mapped = DoMapping(mod, morph, tm, mapping, scl, flipyz, negx);
					if ( !mapped )
					{
						flipyz = true;
						mapped = DoMapping(mod, morph, tm, mapping, scl, flipyz, negx);
					}
				}
			}

			if ( mapped )
			{
				morph.importScale = scl;
				morph.flipyz = flipyz;
				morph.negx = negx;

				morph.mapping = mapping;
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

				return true;
			}
		}

		return false;
	}

	void LoadBase(MegaMorph morph)
	{
		string filename = EditorUtility.OpenFilePanel("Morph Base", lastpath, "obj");

		if ( filename == null || filename.Length < 1 )
			return;

		lastpath = filename;
		List<MegaTargetMesh> targets = MegaTargetMesh.LoadTargets(filename, 1.0f, false, false);	//morph.importScale, morph.flipyz, morph.negx);

		// only use first
		if ( targets != null && targets.Count > 0 )
		{
			if ( !TryMapping(targets, morph) )
			{
				// No match found
				EditorUtility.DisplayDialog("Mapping Failed!", "Mapping of " + Path.GetFileNameWithoutExtension(filename) + " failed!", "OK");
				EditorUtility.ClearProgressBar();
				return;
			}
		}
	}

	// remove mPoints from channel, just use target list, if targets.Count == 1 then use delta
	// first target goes into mPoints
	// guess we should update any targets who we have already, ie use name
	void LoadTargets(MegaMorphChan channel)
	{
		MegaMorph mr = (MegaMorph)target;

		string filename = EditorUtility.OpenFilePanel("Morph Targets", lastpath, "obj");
		if ( filename == null || filename.Length < 1 )
			return;

		lastpath = filename;
		List<MegaTargetMesh> targets = MegaTargetMesh.LoadTargets(filename, mr.importScale, mr.flipyz, mr.negx);

		if ( targets != null )
		{
			if ( channel.mName == "Empty" )
				channel.mName = Path.GetFileNameWithoutExtension(filename);

			// Now need to check that each target has correct num verts and face list matches
			for ( int i = 0; i < targets.Count; i++ )
			{
				MegaTargetMesh tm = targets[i];

				if ( tm.verts.Count != mr.oPoints.Length )
					EditorUtility.DisplayDialog("Target Vertex count mismatch!", "Target " + tm.name + " has wrong number of verts", "OK");
				else
				{
					// See if we have a target with this name, if so update that
					MegaMorphTarget mt = channel.GetTarget(tm.name);

					if ( mt == null )	// add a new target
					{
						mt = new MegaMorphTarget();
						mt.name = tm.name;
						channel.mTargetCache.Add(mt);
					}

					mt.points = tm.verts.ToArray();

					//for ( int v = 0; v < mt.points.Length; v++ )
					//{
						//if ( mt.points[v] == mr.oPoints[v] )
							//Debug.Log("Vert " + v + " isnt morphed");
					//}
				}
			}

			channel.ResetPercent();
			channel.Rebuild(mr);	// rebuild delta for 1st channel
		}

		mr.BuildCompress();
	}

	void LoadTarget(MegaMorphTarget mt)
	{
		MegaMorph mr = (MegaMorph)target;
		string filename = EditorUtility.OpenFilePanel("Morph Target", lastpath, "obj");
		if ( filename == null || filename.Length < 1 )
			return;

		lastpath = filename;
		List<MegaTargetMesh> targets = MegaTargetMesh.LoadTargets(filename, mr.importScale, mr.flipyz, mr.negx);

		if ( targets != null && targets.Count > 0 )
		{
			MegaTargetMesh tm = targets[0];

			if ( tm.verts.Count != mr.oPoints.Length )
			{
				EditorUtility.DisplayDialog("Target Vertex count mismatch!", "Target " + tm.name + " has wrong number of verts", "OK");
			}
			else
			{
				mt.points = tm.verts.ToArray();
				mt.name = tm.name;
			}
		}
	}

	void SwapTargets(MegaMorphChan chan, int t1, int t2)
	{
		if ( t1 >= 0 && t1 < chan.mTargetCache.Count && t2 >= 0 && t2 < chan.mTargetCache.Count && t1 != t2 )
		{
			MegaMorphTarget mt1 = chan.mTargetCache[t1];
			MegaMorphTarget mt2 = chan.mTargetCache[t2];
			float per = mt1.percent;
			mt1.percent = mt2.percent;
			mt2.percent = per;
			chan.mTargetCache.RemoveAt(t1);
			chan.mTargetCache.Insert(t2, mt1);
			EditorUtility.SetDirty(target);
		}
	}

	// Still need to be able to add in unity meshes
	void DisplayTarget(MegaMorph morph, MegaMorphChan channel, MegaMorphTarget mt, int num)
	{
		PushCols();
		EditorGUI.indentLevel = 1;
		mt.name = EditorGUILayout.TextField("Name", mt.name);
		mt.percent = EditorGUILayout.Slider("Percent", mt.percent, 0.0f, 100.0f);

		EditorGUILayout.BeginHorizontal();

		if ( mt.points == null || mt.points.Length != morph.oPoints.Length)
			GUI.backgroundColor = new Color(0.5f, 0.5f, 0.5f);
		else
			GUI.backgroundColor = new Color(0.0f, 1.0f, 0.0f);

		if ( GUILayout.Button("Load") )
		{
			LoadTarget(mt);
		}

		GUI.backgroundColor = new Color(1.0f, 0.5f, 0.5f);
		if ( GUILayout.Button("Delete") )
		{
			MegaMorphTarget mt0 = channel.mTargetCache[0];

			channel.mTargetCache.Remove(mt);
			channel.ResetPercent();

			if ( channel.mTargetCache.Count > 0 && channel.mTargetCache[0] != mt0 )
				channel.Rebuild(morph);
		}

		GUI.backgroundColor = new Color(1.0f, 1.0f, 0.5f);
		if ( GUILayout.Button("Up") )
		{
			if ( num > 0 )
			{
				SwapTargets(channel, num, num - 1);

				if ( num == 1 )
					channel.Rebuild(morph);
			}
		}

		GUI.backgroundColor = new Color(0.5f, 1.0f, 1.0f);
		if ( GUILayout.Button("Dn") )
		{
			if ( num < channel.mTargetCache.Count - 1 )
			{
				SwapTargets(channel, num, num + 1);

				if ( num == 0 )
					channel.Rebuild(morph);
			}
		}

		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel = 0;
		PopCols();
	}

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

	void DisplayChannel(MegaMorph morph, MegaMorphChan channel, int num)
	{
		if ( GUILayout.Button(num + " - " + channel.mName) )
			channel.showparams = !channel.showparams;

		GUI.backgroundColor = new Color(1, 1, 1);
		if ( channel.showparams )
		{
			channel.mName = EditorGUILayout.TextField("Name", channel.mName);

			if ( channel.mTargetCache != null && channel.mTargetCache.Count > 0 )
			{
				channel.mActiveOverride = EditorGUILayout.Toggle("Active", channel.mActiveOverride);
				channel.Percent = EditorGUILayout.Slider("Percent", channel.Percent, 0.0f, 100.0f);
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
			if ( GUILayout.Button("Load Targets") )
				LoadTargets(channel);

			GUI.backgroundColor = new Color(0.5f, 1.0f, 0.5f);
			if ( GUILayout.Button("Add Target") )
			{
				if ( channel.mTargetCache == null )
					channel.mTargetCache = new List<MegaMorphTarget>();

				MegaMorphTarget mt = new MegaMorphTarget();
				channel.mTargetCache.Add(mt);
				channel.ResetPercent();
			}

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
				channel.Percent = EditorGUILayout.Slider("Percent", channel.Percent, 0.0f, 100.0f);
		}
	}

	bool showmodparams = false;
	bool showimport = false;
	bool showchannels = true;

	void ImportParams(MegaMorph morph)
	{
		showimport = EditorGUILayout.Foldout(showimport, "Import Params");

		if ( showimport )
		{
			morph.importScale = EditorGUILayout.FloatField("Import Scale", morph.importScale);
			morph.flipyz = EditorGUILayout.Toggle("FlipYZ", morph.flipyz);
			morph.negx = EditorGUILayout.Toggle("Negate X", morph.negx);
			morph.tolerance = EditorGUILayout.FloatField("Tolerance", morph.tolerance);
		}
	}

	public override void OnInspectorGUI()
	{
		MegaMorph morph = (MegaMorph)target;

		PushCols();

		if ( GUILayout.Button("Import Morph File") )
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

		//ImportParams(morph);

		EditorGUILayout.BeginHorizontal();
		PushCols();
		if ( morph.mapping == null || morph.mapping.Length == 0 )
			GUI.backgroundColor = Color.red;
		else
			GUI.backgroundColor = Color.green;

		if ( GUILayout.Button("Load Mapping") )
			LoadBase(morph);

		PopCols();

		if ( GUILayout.Button("Add Channel") )
		{
			if ( morph.chanBank == null )
				morph.chanBank = new List<MegaMorphChan>();

			MegaMorphChan nc = new MegaMorphChan();
			nc.mName = "Empty";
			morph.chanBank.Add(nc);
			//ChannelMapping(morph, nc);	// Create 1 to 1 mapping
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

				DisplayChannel(morph, morph.chanBank[i], i);
				PopCols();
			}
		}

		extraparams = EditorGUILayout.Foldout(extraparams, "Extra Params");

		if ( extraparams )
		{
			ChanCol1 = EditorGUILayout.ColorField("Channel Col 1", ChanCol1);
			ChanCol2 = EditorGUILayout.ColorField("Channel Col 2", ChanCol2);

			//int mem = CalcMemoryUsage(morph) / 1024;
			if ( morph.compressedmem == 0 )
			{
				morph.memuse = CalcMemoryUsage(morph);
				morph.Compress();
			}
			EditorGUILayout.LabelField("Memory: ", (morph.memuse / 1024) + "KB");
			EditorGUILayout.LabelField("Channel Compressed: ", (morph.compressedmem / 1024) + "KB");
		}

		PopCols();

		if ( GUI.changed )
			EditorUtility.SetDirty(target);
	}

	int MorphedVerts(MegaMorph mr, MegaMorphChan channel)
	{
		int count = 0;

		for ( int v = 0; v < mr.oPoints.Length; v++ )
		{
			Vector3 p = mr.oPoints[v];

			bool morphed = false;

			for ( int i = 0; i < channel.mTargetCache.Count; i++ )
			{
				MegaMorphTarget mt = channel.mTargetCache[i];

				if ( !p.Equals(mt.points[v]) )
				{
					morphed = true;
					break;
				}
			}

			if ( morphed )
				count++;
		}

		return count;
	}

	void ChannelMapping(MegaMorph mr, MegaMorphChan mc)
	{
		mc.mapping = new int[mr.oPoints.Length];

		for ( int i = 0; i < mr.oPoints.Length; i++ )
		{
			mc.mapping[i] = i;
		}
	}

	void CompressChannel(MegaMorph mr, MegaMorphChan mc)
	{
		// for now change system to work off mapping, just have 1 to 1 mapping to test its working

		mc.mapping = new int[mr.oPoints.Length];

		for ( int i = 0; i < mr.oPoints.Length; i++ )
		{
			mc.mapping[i] = i;
		}
#if false
		BitArray modded = new BitArray(mr.oPoints.Length);

		modded.SetAll(false);

		for ( int t = 0; t < mc.mTargetCache.Count; t++ )
		{
			MegaMorphTarget mt = mc.mTargetCache[t];

			for ( int i = 0; i < mr.oPoints.Length; i++ )
			{
				if ( mt.points[i] != mr.oPoints[i] )	// Have a threshold for this
				{
					modded[i] = true;
					break;
				}
			}
		}

		List<int>	points = new List<int>();

		for ( int i = 0; i < modded.Count; i++ )
		{
			if ( modded[i] )
				points.Add(i);
		}

		// points now holds indexes of morphed verts for the channel, so now need to collapse points
		Vector3[] pts = new Vector3[points.Count];

		for ( int t = 0; t < mc.mTargetCache.Count; t++ )
		{
			MegaMorphTarget mt = mc.mTargetCache[t];

			for ( int i = 0; i < points.Count; i++ )
			{
				pts[i] = mt.points[points[i]];
			}

			pts.CopyTo(mt.points, 0);
		}

		// If one target deal with deltas
#endif
	}

	int CalcCompressedMemory(MegaMorph mr)
	{
		int mem = 0;

		for ( int i = 0; i < mr.chanBank.Count; i++ )
		{
			MegaMorphChan mc = mr.chanBank[i];

			//mem += MorphedVerts(mr, mc) * 12 * mc.mTargetCache.Count;

			int mv = MorphedVerts(mr, mc);
			int m = mv * 12 * mc.mTargetCache.Count;
			mem += m;
			EditorGUILayout.LabelField(mc.mName, "Verts: " + mv + " mem: " + m);
		}

		EditorGUILayout.LabelField("Total: ", (mem / 1024) + "KB");

		return mem;
	}

	int CalcMemoryUsage(MegaMorph mr)
	{
		int mem = 0;

		for ( int i = 0; i < mr.chanBank.Count; i++ )
		{
			MegaMorphChan mc = mr.chanBank[i];

			mem += mc.mTargetCache.Count * 12 * mr.oPoints.Length;
		}

		return mem;
	}

	//public delegate bool ParseBinCallbackType(BinaryReader br, string id);
	//public delegate void ParseClassCallbackType(string classname, BinaryReader br);

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

#if false
	static public void Parse(BinaryReader br, ParseBinCallbackType cb)
	{
		bool readchunk = true;

		while ( readchunk )
		{
			string id = MegaUtils.ReadString(br);

			if ( id == "eoc" )
				break;

			int skip = br.ReadInt32();

			long fpos = br.BaseStream.Position;

			if ( !cb(br, id) )
			{
				Debug.Log("Error Loading chunk id " + id);
				readchunk = false;	// done
				break;
			}

			br.BaseStream.Position = fpos + skip;
		}
	}
#endif
	void MorphCallback(string classname, BinaryReader br)
	{
		switch ( classname )
		{
			case "Morph":	LoadMorph(br);	break;
		}
	}

	void LoadMorph()
	{
		MegaMorph mr = (MegaMorph)target;
		//Modifiers mod = mr.GetComponent<Modifiers>();	// Do this at start and store

		string filename = EditorUtility.OpenFilePanel("Morph File", lastpath, "mor");

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
		{
			mr.looptime = looptime;
		}
		mr.compressedmem = 0;
		mr.BuildCompress();
	}

	public void LoadMorph(BinaryReader br)
	{
		//Parse(br, ParseMorph);
		MegaParse.Parse(br, ParseMorph);
	}

	bool AnimCallback(BinaryReader br, string id)
	{
		MegaMorph mr = (MegaMorph)target;

		switch ( id )
		{
			case "Chan":
				int cn = br.ReadInt32();
				currentChan = mr.chanBank[cn]; break;
			case "Anim": currentChan.control = LoadAnim(br);	break;
			default: return false;
		}

		return true;
	}

	void LoadAnimation(MegaMorph mr, BinaryReader br)
	{
		//Parse(br, AnimCallback);
		MegaParse.Parse(br, AnimCallback);
	}

	// Fbx could have been exported any which way so still need to do try all mappings to find correct
	public bool ParseMorph(BinaryReader br, string id)
	{
		MegaMorph mr = (MegaMorph)target;

		//Debug.Log("ParseMorph " + id);
		switch ( id )
		{
			case "Max": mr.Max = br.ReadSingle(); break;
			case "Min": mr.Min = br.ReadSingle(); break;
			case "UseLim": mr.UseLimit = (br.ReadInt32() == 1); break;

			case "StartPoints":	// Mapping
				MegaTargetMesh tm = new MegaTargetMesh();
				tm.verts = MegaParse.ReadP3l(br);	// make a vector
				if ( !TryMapping1(tm, mr) )
				{
					EditorUtility.DisplayDialog("Mapping Failed!", "Mapping failed!", "OK");
					EditorUtility.ClearProgressBar();
					return false;
				}
				//Debug.Log("StartPoints " + tm.verts.Count);
				break;

			case "Channel":
				mr.chanBank.Add(LoadChan(br));
				break;

			case "Animation":
				LoadAnimation(mr, br);
				break;
			default:	return false;
		}

		return true;
	}

	MegaMorphChan currentChan;

	public MegaMorphChan LoadChan(BinaryReader br)
	{
		MegaMorphChan chan = new MegaMorphChan();

		chan.control = null;
		chan.showparams = false;
		chan.mTargetCache = new List<MegaMorphTarget>();
		currentChan = chan;

		//Parse(br, ParseChan);
		MegaParse.Parse(br, ParseChan);

		MegaMorph mr = (MegaMorph)target;
		chan.Rebuild(mr);
		return chan;
	}

	public static MegaBezFloatKeyControl LoadAnim(BinaryReader br)
	{
		//MegaBezFloatKeyControl con = new MegaBezFloatKeyControl();

		//Parse(br, con.Parse);
		//MegaParse.Parse(br, con.Parse);
		//return con;
		return MegaParseBezFloatControl.LoadBezFloatKeyControl(br);
	}

	public bool ParseChan(BinaryReader br, string id)
	{
		//Debug.Log("ParseChan " + id);

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
			//case "Anim":		currentChan.control = LoadAnim(br);	break;
		}

		return true;
	}

	MegaMorphTarget currentTarget;

	void ConvertPoints(Vector3[] verts)
	{
		MegaMorph mr = (MegaMorph)target;

		for ( int i = 0; i < verts.Length; i++ )
		{
			Vector3 p = verts[i] * mr.importScale;

			if ( mr.negx )
			{
				p.x = -p.x;
			}

			if ( mr.flipyz )
			{
				float y = p.y;
				p.y = p.z;
				p.z = y;
			}

			verts[i] = p;
		}
	}

	public bool ParseTarget(BinaryReader br, string id)
	{
		//Debug.Log("ParseTarget " + id);
		switch ( id )
		{
			case "Name":	currentTarget.name = MegaParse.ReadString(br);	break;
			case "Percent": currentTarget.percent = br.ReadSingle(); break;
			case "TPoints": currentTarget.points = MegaParse.ReadP3v(br); ConvertPoints(currentTarget.points); break;
			case "MoPoints":
				Debug.Log("Got morpho points");
				break;
		}

		return true;
	}

	public MegaMorphTarget LoadTarget(BinaryReader br)
	{
		MegaMorphTarget target = new MegaMorphTarget();
		currentTarget = target;

		//Parse(br, ParseTarget);
		MegaParse.Parse(br, ParseTarget);
		return target;
	}

	bool TryMapping1(MegaTargetMesh tm, MegaMorph morph)
	{
		MegaModifiers mod = morph.GetComponent<MegaModifiers>();

		if ( mod == null )
		{
			EditorUtility.DisplayDialog("Missing ModifyObject!", "No ModifyObject script found on the object", "OK");
			return false;
		}

		int[] mapping = new int[mod.verts.Length];

		//for ( int i = 0; i < 8; i++ )
		//{
			//Debug.Log("target vert " + tm.verts[i].ToString("0.000"));
		//}

		//for ( int i = 0; i < 8; i++ )
		//{
			//Debug.Log("base vert " + mod.verts[i].ToString("0.000"));
		//}

		// Get extents for mod verts and for imported meshes, if not the same then scale
		Vector3 min1,max1;
		Vector3 min2,max2;

		Vector3 ex1 = Extents(mod.verts, out min1, out max1);
		Vector3 ex2 = Extents(tm.verts, out min2, out max2);

		// need min max on all axis so we can produce an offset to add
		float d1 = ex1.x;
		float d2 = ex2.x;

		float scl = d1 / d2;	//d2 / d1;
		bool flipyz = false;
		bool negx = false;

		//Debug.Log("scale " + scl.ToString("0.0000"));
		//Vector3 offset = (min2 * scl) - min1;
		//Debug.Log("offset " + offset.ToString("0.0000"));

		//for ( int i = 0; i < 8; i++ )
		//{
			//Vector3 p = tm.verts[i];
			//p = (p * scl) + offset;
			//Debug.Log("adj vert " + p.ToString("0.000"));
		//}

		// So try to match first vert using autoscale and no flip
		bool mapped = DoMapping(mod, morph, tm, mapping, scl, flipyz, negx);

		if ( !mapped )
		{
			flipyz = true;
			mapped = DoMapping(mod, morph, tm, mapping, scl, flipyz, negx);
			if ( !mapped )	//DoMapping(mod, morph, tm, mapping, scl, flipyz, negx) )
			{
				flipyz = false;
				negx = true;
				mapped = DoMapping(mod, morph, tm, mapping, scl, flipyz, negx);
				if ( !mapped )
				{
					flipyz = true;
					mapped = DoMapping(mod, morph, tm, mapping, scl, flipyz, negx);
				}
			}
		}

		if ( mapped )
		{
			morph.importScale = scl;
			morph.flipyz = flipyz;
			morph.negx = negx;

			morph.mapping = mapping;
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

			return true;
		}

		return false;
	}
}
// 1497