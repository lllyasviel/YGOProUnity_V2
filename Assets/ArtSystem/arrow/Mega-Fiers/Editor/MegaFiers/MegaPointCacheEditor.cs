
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

// Support anim uvs here as well
[CustomEditor(typeof(MegaPointCache))]
public class MegaPointCacheEditor : MegaModifierEditor
{
	static string lastpath = " ";

	public delegate bool ParseBinCallbackType(BinaryReader br, string id);
	public delegate void ParseClassCallbackType(string classname, BinaryReader br);

	MegaModifiers mods;
	List<MegaPCVert>	Verts = new List<MegaPCVert>();

	// Mapping values
	float	tolerance	= 0.0001f;
	float	scl			= 1.0f;
	bool	flipyz		= false;
	bool	negx		= false;
	bool	negz		= false;	// 8 cases now

	string Read(BinaryReader br, int count)
	{
		byte[] buf = br.ReadBytes(count);
		return System.Text.Encoding.ASCII.GetString(buf, 0, buf.Length);
	}

	struct MCCFrame
	{
		public Vector3[] points;
	}

	// Maya format
	void LoadMCC()
	{
		MegaPointCache am = (MegaPointCache)target;
		mods = am.gameObject.GetComponent<MegaModifiers>();

		string filename = EditorUtility.OpenFilePanel("Maya Cache File", lastpath, "mc");

		if ( filename == null || filename.Length < 1 )
			return;

		lastpath = filename;

		FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, System.IO.FileShare.Read);

		BinaryReader br = new BinaryReader(fs);

		string id = Read(br, 4);
		if ( id != "FOR4" )
		{
			Debug.Log("wrong file");
			return;
		}

		int offset  = MegaParse.ReadMotInt(br);

		br.ReadBytes(offset);

		List<MCCFrame> frames = new List<MCCFrame>();

		while ( true )
		{
			string btag = Read(br, 4);
			if ( btag == "" )
				break;

			if ( btag != "FOR4" )
			{
				Debug.Log("File format error");
				return;
			}

			int blocksize = MegaParse.ReadMotInt(br);

			int bytesread = 0;

			btag = Read(br, 4);
			if ( btag != "MYCH" )
			{
				Debug.Log("File format error");
				return;
			}
			bytesread += 4;

			btag = Read(br, 4);
			if ( btag != "TIME" )
			{
				Debug.Log("File format error");
				return;
			}
			bytesread += 4;

			br.ReadBytes(4);
			bytesread += 4;

			int time = MegaParse.ReadMotInt(br);
			bytesread += 4;

			am.maxtime = (float)time / 6000.0f;

			while ( bytesread < blocksize )
			{
				btag = Read(br, 4);
				if ( btag != "CHNM" )
				{
					Debug.Log("chm error");
					return;
				}
				bytesread += 4;

				int chnmsize = MegaParse.ReadMotInt(br);
				bytesread += 4;

				int mask = 3;
				int chnmsizetoread = (chnmsize + mask) & (~mask);
				//byte[] channelname = br.ReadBytes(chnmsize);
				br.ReadBytes(chnmsize);

				int paddingsize = chnmsizetoread - chnmsize;

				if ( paddingsize > 0 )
					br.ReadBytes(paddingsize);

				bytesread += chnmsizetoread;

				btag = Read(br, 4);

				if ( btag != "SIZE" )
				{
					Debug.Log("Size error");
					return;
				}
				bytesread += 4;

				br.ReadBytes(4);
				bytesread += 4;

				int arraylength = MegaParse.ReadMotInt(br);
				bytesread += 4;

				MCCFrame frame = new MCCFrame();
				frame.points = new Vector3[arraylength];

				string dataformattag = Read(br, 4);
				int bufferlength = MegaParse.ReadMotInt(br);
				bytesread += 8;

				if ( dataformattag == "FVCA" )
				{
					if ( bufferlength != arraylength * 3 * 4 )
					{
						Debug.Log("buffer len error");
						return;
					}

					for ( int i = 0; i < arraylength; i++ )
					{
						frame.points[i].x = MegaParse.ReadMotFloat(br);
						frame.points[i].y = MegaParse.ReadMotFloat(br);
						frame.points[i].z = MegaParse.ReadMotFloat(br);
					}

					bytesread += arraylength * 3 * 4;
				}
				else
				{
					if ( dataformattag == "DVCA" )
					{
						if ( bufferlength != arraylength * 3 * 8 )
						{
							Debug.Log("buffer len error");
							return;
						}

						for ( int i = 0; i < arraylength; i++ )
						{
							frame.points[i].x = (float)MegaParse.ReadMotDouble(br);
							frame.points[i].y = (float)MegaParse.ReadMotDouble(br);
							frame.points[i].z = (float)MegaParse.ReadMotDouble(br);
						}

						bytesread += arraylength * 3 * 8;
					}
					else
					{
						Debug.Log("Format Error");
						return;
					}
				}

				frames.Add(frame);
			}
		}

		// Build table
		am.Verts = new MegaPCVert[frames[0].points.Length];

		for ( int i = 0; i < am.Verts.Length; i++ )
		{
			am.Verts[i] = new MegaPCVert();
			am.Verts[i].points = new Vector3[frames.Count];

			for ( int p = 0; p < am.Verts[i].points.Length; p++ )
				am.Verts[i].points[p] = frames[p].points[i];
		}

		BuildData(mods, am, filename);
		br.Close();
	}

	void LoadMDD()
	{
		MegaPointCache am = (MegaPointCache)target;
		mods = am.gameObject.GetComponent<MegaModifiers>();

		string filename = EditorUtility.OpenFilePanel("Motion Designer File", lastpath, "mdd");

		if ( filename == null || filename.Length < 1 )
			return;

		lastpath = filename;

		// Clear what we have
		Verts.Clear();

		FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, System.IO.FileShare.Read);

		BinaryReader br = new BinaryReader(fs);

		int numSamples = MegaParse.ReadMotInt(br);
		int numPoints = MegaParse.ReadMotInt(br);

		float t = 0.0f;

		for ( int i = 0; i < numSamples; i++ )
			t = MegaParse.ReadMotFloat(br);

		am.maxtime = t;

		am.Verts = new MegaPCVert[numPoints];

		for ( int i = 0; i < am.Verts.Length; i++ )
		{
			am.Verts[i] = new MegaPCVert();
			am.Verts[i].points = new Vector3[numSamples];
		}

		for ( int i = 0; i < numSamples; i++ )
		{
			for ( int v = 0; v < numPoints; v++ )
			{
				am.Verts[v].points[i].x = MegaParse.ReadMotFloat(br);
				am.Verts[v].points[i].y = MegaParse.ReadMotFloat(br);
				am.Verts[v].points[i].z = MegaParse.ReadMotFloat(br);
			}
		}

		BuildData(mods, am, filename);
		br.Close();
	}

	void BuildData(MegaModifiers mods, MegaPointCache am, string filename)
	{
		// Build vector3[] of base verts
		Vector3[] baseverts = new Vector3[am.Verts.Length];

		for ( int i = 0; i < am.Verts.Length; i++ )
			baseverts[i] = am.Verts[i].points[0];

		if ( !TryMapping1(baseverts, mods.verts) )
		{
			EditorUtility.DisplayDialog("Mapping Failed!", "Mapping of " + Path.GetFileNameWithoutExtension(filename) + " failed!", "OK");
			EditorUtility.ClearProgressBar();
			return;
		}

		// Remap vertices
		for ( int i = 0; i < am.Verts.Length; i++ )
		{
			for ( int v = 0; v < am.Verts[i].points.Length; v++ )
			{
				if ( negx )
					am.Verts[i].points[v].x = -am.Verts[i].points[v].x;

				if ( flipyz )
				{
					float z = am.Verts[i].points[v].z;
					am.Verts[i].points[v].z = am.Verts[i].points[v].y;
					am.Verts[i].points[v].y = z;
				}

				if ( negz )
					am.Verts[i].points[v].z = -am.Verts[i].points[v].z;

				am.Verts[i].points[v] = am.Verts[i].points[v] * scl;
			}
		}

		for ( int i = 0; i < am.Verts.Length; i++ )
		{
			am.Verts[i].indices = FindVerts(am.Verts[i].points[0]);

			if ( am.Verts[i].indices.Length == 0 )
			{
				EditorUtility.DisplayDialog("Final Mapping Failed!", "Mapping of " + Path.GetFileNameWithoutExtension(filename) + " failed!", "OK");
				EditorUtility.ClearProgressBar();
				return;
			}
		}
	}

	void LoadPC2()
	{
		MegaPointCache am = (MegaPointCache)target;
		mods = am.gameObject.GetComponent<MegaModifiers>();

		string filename = EditorUtility.OpenFilePanel("Point Cache File", lastpath, "pc2");

		if ( filename == null || filename.Length < 1 )
			return;

		lastpath = filename;

		// Clear what we have
		Verts.Clear();

		FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, System.IO.FileShare.Read);

		BinaryReader br = new BinaryReader(fs);

		string sig = MegaParse.ReadStr(br);
		if ( sig != "POINTCACHE2" )
		{
			br.Close();
			return;
		}

		int fileVersion = br.ReadInt32();
		if ( fileVersion != 1 )
		{
			br.Close();
			return;
		}

		int numPoints = br.ReadInt32();
		br.ReadSingle();
		br.ReadSingle();
		int numSamples = br.ReadInt32();

		am.Verts = new MegaPCVert[numPoints];

		for ( int i = 0; i < am.Verts.Length; i++ )
		{
			am.Verts[i] = new MegaPCVert();
			am.Verts[i].points = new Vector3[numSamples];
		}

		for ( int i = 0; i < numSamples; i++ )
		{
			for ( int v = 0; v < numPoints; v++ )
				am.Verts[v].points[i] = MegaParse.ReadP3(br);
		}
		BuildData(mods, am, filename);
		br.Close();
	}

	// Utils methods
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

	Vector3 Extents(MegaPCVert[] verts, out Vector3 min, out Vector3 max)
	{
		Vector3 extent = Vector3.zero;

		min = Vector3.zero;
		max = Vector3.zero;

		if ( verts != null && verts.Length > 0 )
		{
			min = verts[0].points[0];
			max = verts[0].points[0];

			for ( int i = 1; i < verts.Length; i++ )
			{
				Vector3 p = verts[i].points[0];

				if ( p.x < min.x ) min.x = p.x;
				if ( p.y < min.y ) min.y = p.y;
				if ( p.z < min.z ) min.z = p.z;

				if ( p.x > max.x ) max.x = p.x;
				if ( p.y > max.y ) max.y = p.y;
				if ( p.z > max.z ) max.z = p.z;
			}

			extent = max - min;
		}

		return extent;
	}

	public override void OnInspectorGUI()
	{
		MegaPointCache am = (MegaPointCache)target;

		EditorGUILayout.BeginHorizontal();
		if ( GUILayout.Button("Import PC2") )
		{
			LoadPC2();
			EditorUtility.SetDirty(target);
		}

		if ( GUILayout.Button("Import MDD") )
		{
			LoadMDD();
			EditorUtility.SetDirty(target);
		}

		if ( GUILayout.Button("Import MC") )
		{
			LoadMCC();
			EditorUtility.SetDirty(target);
		}

		EditorGUILayout.EndHorizontal();

		// Basic mod stuff
		showmodparams = EditorGUILayout.Foldout(showmodparams, "Modifier Common Params");

		if ( showmodparams )
			CommonModParamsBasic(am);

		am.time = EditorGUILayout.FloatField("Time", am.time);
		am.maxtime = EditorGUILayout.FloatField("Loop Time", am.maxtime);
		am.animated = EditorGUILayout.Toggle("Animated", am.animated);
		am.speed = EditorGUILayout.FloatField("Speed", am.speed);
		am.LoopMode = (MegaRepeatMode)EditorGUILayout.EnumPopup("Loop Mode", am.LoopMode);
		am.interpMethod = (MegaInterpMethod)EditorGUILayout.EnumPopup("Interp Method", am.interpMethod);

		am.blendMode = (MegaBlendAnimMode)EditorGUILayout.EnumPopup("Blend Mode", am.blendMode);
		if ( am.blendMode == MegaBlendAnimMode.Additive )
			am.weight = EditorGUILayout.FloatField("Weight", am.weight);

		if ( am.Verts != null && am.Verts.Length > 0 )
		{
			int mem = am.Verts.Length * am.Verts[0].points.Length * 12;
			EditorGUILayout.LabelField("Memory: ", (mem / 1024) + "KB");
		}

		if ( GUI.changed )
			EditorUtility.SetDirty(target);
	}

	// From here down could move to util class
	int FindVert(Vector3 vert, Vector3[] tverts, float tolerance, float scl, bool flipyz, bool negx, bool negz, int vn)
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

		if ( negz )
			vert.z = -vert.z;

		vert /= scl;

		float closest = Vector3.SqrMagnitude(tverts[0] - vert);

		for ( int i = 0; i < tverts.Length; i++ )
		{
			float dif = Vector3.SqrMagnitude(tverts[i] - vert);

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

	bool DoMapping(Vector3[] verts, Vector3[] tverts, float scale, bool flipyz, bool negx, bool negz)
	{
		for ( int i = 0; i < verts.Length; i++ )
		{
			float a = (float)i / (float)verts.Length;

			//Debug.Log("map " + i);
			EditorUtility.DisplayProgressBar("Mapping", "Mapping vertex " + i, a);
			int mapping = FindVert(verts[i], tverts, tolerance, scale, flipyz, negx, negz, i);

			if ( mapping == -1 )
			{
				// Failed
				EditorUtility.ClearProgressBar();
				return false;
			}
		}

		EditorUtility.ClearProgressBar();
		return true;
	}

	// Out of this we need scl, negx, negz and flipyz
	bool TryMapping1(Vector3[] tverts, Vector3[] verts)
	{
		//for ( int i = 0; i < 4; i++ )
		//	Debug.Log("cache vert " + tverts[i].ToString("0.00000"));

		//for ( int i = 0; i < 4; i++ )
		//	Debug.Log("mesh vert " + verts[i].ToString("0.00000"));

		// Get extents for mod verts and for imported meshes, if not the same then scale
		Vector3 min1,max1;
		Vector3 min2,max2;

		Vector3 ex1 = Extents(verts, out min1, out max1);
		Vector3 ex2 = Extents(tverts, out min2, out max2);

		//Debug.Log("mesh extents " + ex1.ToString("0.00000"));
		//Debug.Log("cache extents " + ex2.ToString("0.00000"));

		//Debug.Log("mesh min " + min1.ToString("0.00000"));
		//Debug.Log("cache min " + min2.ToString("0.00000"));

		//Debug.Log("mesh max " + max1.ToString("0.00000"));
		//Debug.Log("cache max " + max2.ToString("0.00000"));

		int largest1 = MegaUtils.LargestComponent(ex1);
		int largest2 = MegaUtils.LargestComponent(ex2);

		//Debug.Log(largest1 + " " + largest2);
		scl = ex1[largest1] / ex2[largest2];
		//Debug.Log("scl " + scl);

		//Vector3 offset = verts[0] - (tverts[0] * scl);
		//Debug.Log("Offset " + offset.ToString("0.00000"));
		// need min max on all axis so we can produce an offset to add

		int map = 0;

		for ( map = 0; map < 8; map++ )
		{
			flipyz = ((map & 4) != 0);
			negx = ((map & 2) != 0);
			negz = ((map & 1) != 0);

			bool mapped = DoMapping(verts, tverts, scl, flipyz, negx, negz);
			if ( mapped )
				break;
		}

		if ( map == 8 )	// We couldnt find any mapping
			return false;

		//Debug.Log("scl " + scl + " negx " + negx + " negz " + negz + " flipyz " + flipyz);
		return true;
	}
}