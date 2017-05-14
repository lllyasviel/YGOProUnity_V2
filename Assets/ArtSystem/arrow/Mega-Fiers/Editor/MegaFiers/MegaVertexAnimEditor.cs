
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Collections;

// Support anim uvs here as well
[CustomEditor(typeof(MegaVertexAnim))]
public class MegaVertexAnimEditor : MegaModifierEditor	//Editor
{
	//bool showmodparams = true;
	static string lastpath = " ";

	public delegate bool ParseBinCallbackType(BinaryReader br, string id);
	public delegate void ParseClassCallbackType(string classname, BinaryReader br);

	MegaModifiers mods;
	List<MegaAnimatedVert>	Verts = new List<MegaAnimatedVert>();

	void LoadVertexAnim()
	{
		MegaVertexAnim am = (MegaVertexAnim)target;
		mods = am.gameObject.GetComponent<MegaModifiers>();

		string filename = EditorUtility.OpenFilePanel("Vertex Animation File", lastpath, "mpc");

		if ( filename == null || filename.Length < 1 )
			return;

		lastpath = filename;

		// Clear what we have
		Verts.Clear();

		ParseFile(filename, AnimatedMeshCallback);
		am.Verts = Verts.ToArray();

		BitArray animated = new BitArray(mods.verts.Length);
		int count = 0;
		for ( int i = 0; i < Verts.Count; i++ )
		{
			for ( int v = 0; v < Verts[i].indices.Length; v++ )
			{
				if ( !animated[Verts[i].indices[v]] )
				{
					animated[Verts[i].indices[v]] = true;
					count++;
				}
			}
		}

		am.NoAnim = new int[mods.verts.Length - count];
		int index = 0;
		for ( int i = 0; i < animated.Count; i++ )
		{
			if ( !animated[i] )
				am.NoAnim[index++] = i;
		}

		am.maxtime = 0.0f;
		for ( int i = 0; i < Verts.Count; i++ )
		{
			float t = Verts[i].con.Times[Verts[i].con.Times.Length - 1];
			if ( t > am.maxtime )
					am.maxtime = t;
		}
	}

	void AnimatedMeshCallback(string classname, BinaryReader br)
	{
		switch ( classname )
		{
			case "AnimMesh": LoadAnimMesh(br); break;
		}
	}

	public void LoadAnimMesh(BinaryReader br)
	{
		MegaParse.Parse(br, ParseAnimMesh);
	}

	int[] FindVerts(Vector3 p)
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

	MegaAnimatedVert currentVert;
	float scl = 1.0f;

	public bool ParseAnimMesh(BinaryReader br, string id)
	{
		switch ( id )
		{
			case "Size":
				Vector3 sz = MegaParse.ReadP3(br);
				Vector3 min1,max1;

				Vector3 ex1 = Extents(mods.verts, out min1, out max1);

				int largest = 0;
				if ( sz.x > sz.y )
				{
					if ( sz.x > sz.z )
						largest = 0;
					else
						largest = 2;
				}
				else
				{
					if ( sz.y > sz.z )
						largest = 1;
					else
						largest = 2;
				}

				scl = ex1[largest] / sz[largest];
				break;

			case "V":
				Vector3 p = MegaParse.ReadP3(br) * scl;
				// Find all matching verts
				currentVert = new MegaAnimatedVert();
				currentVert.startVert = p;
				currentVert.indices = FindVerts(p);
				if ( currentVert.indices == null )
					Debug.Log("Error! No match found");

				Verts.Add(currentVert);
				break;

			case "Anim":
				//currentVert.con = MegaBezVector3KeyControl.LoadBezVector3KeyControl(br);
				currentVert.con = MegaParseBezVector3Control.LoadBezVector3KeyControl(br);

				currentVert.con.Scale(scl);
				break;

			default: return false;
		}

		return true;
	}

	public void ParseFile(string assetpath, ParseClassCallbackType cb)
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

	public override void OnInspectorGUI()
	{
		MegaVertexAnim am = (MegaVertexAnim)target;

		if ( GUILayout.Button("Import Vertex Anim File") )
		{
			LoadVertexAnim();
			EditorUtility.SetDirty(target);
		}

		// Basic mod stuff
		showmodparams = EditorGUILayout.Foldout(showmodparams, "Modifier Common Params");

		if ( showmodparams )
		{
			CommonModParamsBasic(am);
#if false
			am.ModEnabled	= EditorGUILayout.Toggle("Mod Enabled", am.ModEnabled);
			am.DisplayGizmo	= EditorGUILayout.Toggle("Display Gizmo", am.DisplayGizmo);
			am.Order		= EditorGUILayout.IntField("Order", am.Order);
			am.gizCol1 = EditorGUILayout.ColorField("Giz Col 1", am.gizCol1);
			am.gizCol2 = EditorGUILayout.ColorField("Giz Col 2", am.gizCol2);
#endif
		}

		am.time		= EditorGUILayout.FloatField("Time", am.time);
		am.maxtime	= EditorGUILayout.FloatField("Loop Time", am.maxtime);
		am.animated	= EditorGUILayout.Toggle("Animated", am.animated);
		am.speed	= EditorGUILayout.FloatField("Speed", am.speed);
		am.LoopMode	= (MegaRepeatMode)EditorGUILayout.EnumPopup("Loop Mode", am.LoopMode);

		am.blendMode = (MegaBlendAnimMode)EditorGUILayout.EnumPopup("Blend Mode", am.blendMode);
		if ( am.blendMode == MegaBlendAnimMode.Additive )
			am.weight = EditorGUILayout.FloatField("Weight", am.weight);

		if ( GUI.changed )
			EditorUtility.SetDirty(target);
	}
}