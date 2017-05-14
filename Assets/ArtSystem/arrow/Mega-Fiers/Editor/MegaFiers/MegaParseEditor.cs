
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public delegate bool ParseBinCallbackType(BinaryReader br, string id);
public delegate void ParseClassCallbackType(string classname, BinaryReader br);

public class MegaParseBezFloatControl
{
	static MegaBezFloatKeyControl con;

	static public MegaBezFloatKeyControl LoadBezFloatKeyControl(BinaryReader br)
	{
		con = new MegaBezFloatKeyControl();

		MegaParse.Parse(br, Parse);
		return con;
	}

	static public bool Parse(BinaryReader br, string id)
	{
		switch ( id )
		{
			case "Num":
				int num = br.ReadInt32();
				con.Keys = new MegaBezFloatKey[num];
				con.Times = new float[num];
				break;

			case "Keys":
				for ( int i = 0; i < con.Keys.Length; i++ )
				{
					con.Keys[i] = new MegaBezFloatKey();
					con.Keys[i].val = br.ReadSingle();
					con.Keys[i].intan = br.ReadSingle();
					con.Keys[i].outtan = br.ReadSingle();
					con.Times[i] = br.ReadSingle();
				}
				con.InitKeys();
				break;

			case "BKeys":	// Blender keys
				Vector2 co = Vector2.zero;
				Vector2 left = Vector2.zero;
				Vector3 right = Vector2.zero;

				Vector2 pco = Vector2.zero;
				Vector2 pleft = Vector2.zero;
				Vector3 pright = Vector2.zero;

				for ( int i = 0; i < con.Keys.Length; i++ )
				{
					con.Keys[i] = new MegaBezFloatKey();

					co.x = br.ReadSingle();
					co.y = br.ReadSingle();

					left.x = br.ReadSingle();
					left.y = br.ReadSingle();

					right.x = br.ReadSingle();
					right.y = br.ReadSingle();

					if ( i > 0 )
						con.MakeKey(con.Keys[i - 1], pco, pleft, pright, co, left, right);

					pco = co;
					pleft = left;
					pright = right;
					con.Times[i] = co.x / 30.0f;
				}
				break;
		}

		return true;
	}
}

public class MegaParseBezVector3Control
{
	static MegaBezVector3KeyControl con;
 
	static public MegaBezVector3KeyControl LoadBezVector3KeyControl(BinaryReader br)
	{
		con = new MegaBezVector3KeyControl();

		MegaParse.Parse(br, Parse);
		return con;
	}

	static public bool Parse(BinaryReader br, string id)
	{
		switch ( id )
		{
			case "Num":
				int num = br.ReadInt32();
				con.Keys = new MegaBezVector3Key[num];
				con.Times = new float[num];
				break;

			case "Keys":
				for ( int i = 0; i < con.Keys.Length; i++ )
				{
					con.Keys[i] = new MegaBezVector3Key();
					con.Keys[i].val = MegaParse.ReadP3(br);
					con.Keys[i].intan = MegaParse.ReadP3(br);
					con.Keys[i].outtan = MegaParse.ReadP3(br);
					con.Times[i] = br.ReadSingle();
				}
				con.InitKeys();
				break;
		}

		return true;
	}
}


public class MegaParse
{

	static public Vector3 ReadP3(BinaryReader br)
	{
		Vector3 v = Vector3.zero;

		v.x = br.ReadSingle();
		v.y = br.ReadSingle();
		v.z = br.ReadSingle();

		return v;
	}

	static public string ReadString(BinaryReader br)
	{
		int len = br.ReadInt32();
		string str = new string(br.ReadChars(len - 1));
		br.ReadChar();
		return str;
	}

	static public string ReadStr(BinaryReader br)
	{
		string str = "";
		while ( true )
		{
			char c = br.ReadChar();
			if ( c == 0 )
				break;

			str += c;
		}
		return str;
	}

	static public Vector3[] ReadP3v(BinaryReader br)
	{
		int count = br.ReadInt32();

		Vector3[] tab = new Vector3[count];

		for ( int i = 0; i < count; i++ )
		{
			tab[i].x = br.ReadSingle();
			tab[i].y = br.ReadSingle();
			tab[i].z = br.ReadSingle();
		}

		return tab;
	}

	static public List<Vector3> ReadP3l(BinaryReader br)
	{
		int count = br.ReadInt32();

		List<Vector3> tab = new List<Vector3>(count);

		Vector3 p = Vector3.zero;

		for ( int i = 0; i < count; i++ )
		{
			p.x = br.ReadSingle();
			p.y = br.ReadSingle();
			p.z = br.ReadSingle();
			tab.Add(p);
		}

		return tab;
	}

	static public float ReadMotFloat(BinaryReader br)
	{
		byte[] floatBytes = br.ReadBytes(4);
		// swap the bytes
		Array.Reverse(floatBytes);
		// get the float from the byte array
		return BitConverter.ToSingle(floatBytes, 0);
	}

	static public double ReadMotDouble(BinaryReader br)
	{
		byte[] floatBytes = br.ReadBytes(8);
		// swap the bytes
		Array.Reverse(floatBytes);
		// get the float from the byte array
		return BitConverter.ToDouble(floatBytes, 0);
	}

	static public int ReadMotInt(BinaryReader br)
	{
		byte[] floatBytes = br.ReadBytes(4);
		// swap the bytes
		Array.Reverse(floatBytes);
		// get the float from the byte array
		return BitConverter.ToInt32(floatBytes, 0);
	}

	//public delegate bool ParseBinCallbackType(BinaryReader br, string id);
	//public delegate void ParseClassCallbackType(string classname, BinaryReader br);

	static public void Parse(BinaryReader br, ParseBinCallbackType cb)
	{
		bool readchunk = true;

		while ( readchunk )
		{
			string id = MegaParse.ReadString(br);

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
}