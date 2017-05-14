
using UnityEngine;

[AddComponentMenu("Modifiers/UV/Tiles")]
public class MegaUVTiles : MegaModifier
{
	public int		Frame		= 0;
	public int		TileWidth	= 16;
	public int		TileHeight	= 16;
	public Vector2	off			= Vector2.zero;
	public Vector2	scale		= Vector2.one;
	public bool		Animate		= false;
	public int		EndFrame	= 0;
	public float	fps			= 1.0f;
	public float	AnimTime	= 0.0f;
	public bool		flipy		= false;
	public bool		flipx		= false;
	public MegaRepeatMode	loopMode = MegaRepeatMode.Loop;

	[HideInInspector]
	public int		twidth;
	[HideInInspector]
	public int		theight;
	[HideInInspector]
	public float	frm = 0.0f;

	Material	mat;

	float		tuvw;
	float		tuvh;
	int			xtiles;
	int			ytiles;
	int			maxframe;

	// 3 channels of UV
	public override MegaModChannel ChannelsReq()		{ return MegaModChannel.UV; }
	public override MegaModChannel ChannelsChanged()	{ return MegaModChannel.UV; }
	public override string ModName()	{ return "UVTiles"; }
	public override string GetHelpURL() { return "?page_id=354"; }

	// Going to have to allow submesh support so need to say which material we are effecting
	// Same for all uv mods really, see how max does it, prob mat id, max 32 mats possibly
	void Init()
	{
		MeshRenderer mr = GetComponent<MeshRenderer>();

		mat = mr.sharedMaterial;

		if ( mat != null )
		{
			Texture tex = mat.GetTexture("_MainTex");
			if ( tex != null )
			{
				twidth = tex.width;
				theight = tex.height;
			}
		}
	}

	// TODO: Show deform
	public override Vector3 Map(int i, Vector3 p)
	{
		return p;
	}

	//public override void Modify(ref Vector3[] sverts, ref Vector3[] verts)
	public override void Modify(MegaModifiers mc)
	{
		Vector2[]	uvs = mc.GetSourceUvs();
		Vector2[]	newuvs = mc.GetDestUvs();

		if ( mat == null || twidth == 0.0f )
			Init();

		if ( uvs.Length > 0 )
		{
			//Debug.Log("twidth " + twidth);
			xtiles = twidth / TileWidth;
			ytiles = theight / TileHeight;

			tuvw = (float)TileWidth / (float)twidth;
			tuvh = (float)TileHeight / (float)theight;

			maxframe = xtiles * ytiles;

			Frame = Frame % maxframe;

			int x = Frame % xtiles;
			int y = Frame / xtiles;

			float su = (x * tuvw);
			float sv = (y * tuvh);

			for ( int i = 0; i < uvs.Length; i++ )
			{
				Vector2 uv = Vector2.Scale(uvs[i] + off, scale);

				if ( flipy )	uv.y = 1.0f - uv.y;
				if ( flipx )	uv.x = 1.0f - uv.x;

				uv.x = su + (tuvw * uv.x);
				uv.y = 1.0f - (sv + (tuvh * uv.y));
				newuvs[i] = uv;
			}
		}
	}

	//public override bool ModLateUpdate(Modifiers mc)
	public override bool ModLateUpdate(MegaModContext mc)
	{
		if ( Animate )
		{
			AnimTime += Time.deltaTime;

			float l = (float)EndFrame / fps;
			switch ( loopMode )
			{
				case MegaRepeatMode.Loop:		frm = Mathf.Repeat(AnimTime, l);		break;
				case MegaRepeatMode.PingPong:	frm = Mathf.PingPong(AnimTime, l);		break;
				case MegaRepeatMode.Clamp:		frm = Mathf.Clamp(AnimTime, 0.0f, l);	break;
			}

			//frm += Time.deltaTime * fps;
			Frame = (int)((frm / l) * EndFrame);
		}
		return Prepare(mc);
	}

	public override bool Prepare(MegaModContext mc)
	{
		return true;
	}
}