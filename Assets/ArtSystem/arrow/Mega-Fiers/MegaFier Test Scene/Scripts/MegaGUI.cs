
using UnityEngine;

public class MegaGUI : MonoBehaviour
{
	public GameObject	source;
	public GameObject	ground;
	bool				showcommon = false;
	bool				showgcommon = false;
	MegaModifier[]		mods;
	MegaModifier[]		gmods;
	bool[]				showmod;
	bool[]				showgmod;
	MegaModifiers		context = null;
	MegaModifiers		gcontext = null;
	public bool			showparams = true;
	public bool			showgparams = true;
	GUIContent[]		Axislist;
	public float		objsize = 2.0f;
	GUIContent[]		EAxislist;
	Vector2				pos = new Vector2();
	public float		sx = 232.0f;	//190.0f;
	Rect				windowRect = new Rect(10, 5, 200, 500);
	public Material		mat;
	public Material		gmat;

	void Start()
	{
		if ( source != null )
		{
			context = (MegaModifiers)source.GetComponent<MegaModifyObject>();

			if ( context != null )
			{
				mods = source.GetComponents<MegaModifier>();
				showmod = new bool[mods.Length];
			}

			gcontext = (MegaModifiers)ground.GetComponent<MegaModifyObject>();

			if ( gcontext != null )
			{
				gmods = ground.GetComponents<MegaModifier>();
				showgmod = new bool[gmods.Length];
			}
		}

		//if ( book )
		//{
			//pageturn = book.GetComponent<PageTurn>();
		//}
		//windowRect.yMax = Screen.height - dsize;
		//svh = windowRect.yMax * svd;	//(float)Screen.height * 0.5f;	// - dsize;	//25.0f;	//* 0.955f;
		SizeChange();
	}

	void InitAxis()
	{
		if ( Axislist == null )
		{
			// Make some content for the popup list
			Axislist = new GUIContent[3];
			Axislist[0] = new GUIContent("X");
			Axislist[1] = new GUIContent("Y");
			Axislist[2] = new GUIContent("Z");
		}
	}

	void InitEAxis()
	{
		if ( EAxislist == null )
		{
			// Make some content for the popup list
			EAxislist = new GUIContent[3];
			EAxislist[0] = new GUIContent("X");
			EAxislist[1] = new GUIContent("Y");
			EAxislist[2] = new GUIContent("XY");
		}
	}

	GUIContent[] MatList;

	void InitMatList()
	{
		if ( MatList == null )
		{
			// Make some content for the popup list
			MatList = new GUIContent[5];
			MatList[0] = new GUIContent("Ice");
			MatList[1] = new GUIContent("Glass");
			MatList[2] = new GUIContent("Jelly");
			MatList[3] = new GUIContent("Plastic");
			MatList[4] = new GUIContent("Custom");
		}
	}
	public int EAxisXYZ(string name, int val)
	{
		InitEAxis();
		GUILayout.Label(name);
		return GUILayout.SelectionGrid(val, EAxislist, 3, "toggle");
	}

	public int EditInt(string name, int val)
	{
		GUILayout.Label(name);
		string s = GUILayout.TextField(val.ToString());
#if !UNITY_FLASH
		int.TryParse(s, out val);
#endif

		return val;
	}

	public int XYZ(string name, int val)
	{
		InitAxis();
		GUILayout.Label(name);
		return GUILayout.SelectionGrid(val, Axislist, 3, "toggle");
	}

	// Slider pro to size of mesh
	public float ProSlider(string name, float val, float low, float high, float pro)
	{
		GUILayout.Label(name + " " + val.ToString("0.000"));
		return GUILayout.HorizontalSlider(val, low * pro * 0.5f, high * pro * 0.5f);
	}

	public float ProSlider(float val, float low, float high, float pro)
	{
		return GUILayout.HorizontalSlider(val, low * pro * 0.5f, high * pro * 0.5f);
	}

	public float Slider(string name, float val, float low, float high)
	{
		float a = (val - low) / (high - low);
		GUILayout.Label(name + " " + val.ToString("0.000"));
		float v = GUILayout.HorizontalSlider(a, 0.0f, 1.0f);	//low, high);
		float delta = v - a;

		if ( Input.GetKey(KeyCode.LeftShift) )
		{
			delta *= 0.001f;
		}
		a += delta;
		return low + (a * (high - low));
	}

	public float AngleSlider(string name, float val, float scl)
	{
		GUILayout.Label(name + " " + val.ToString("0.000"));
		return GUILayout.HorizontalSlider(val, -360.0f * scl, 360.0f * scl);
	}

	public float AngleSlider(float val, float scl)
	{
		return GUILayout.HorizontalSlider(val, -360.0f * scl, 360.0f * scl);
	}

	Color gr = new Color(1.0f, 0.5f, 0.5f);
	Color gg = new Color(0.5f, 1.0f, 0.5f);
	Color gb = new Color(0.5f, 0.5f, 1.0f);

	public Vector3 AngleSlider(string name, ref Vector3 val, float scl)
	{
		GUILayout.Label(name + " " + val.x.ToString("0.0") + " " + val.y.ToString("0.0") + " " + val.z.ToString("0.0"));

		GUI.color = gr;
		val.x = AngleSlider(val.x, scl);
		GUI.color = gg;
		val.y = AngleSlider(val.y, scl);
		GUI.color = gb;
		val.z = AngleSlider(val.z, scl);

		GUI.color = Color.white;

		return val;
	}

	public Vector3 ProSlider(string name, ref Vector3 val, float low, float high, float pro)
	{
		GUILayout.Label(name + " " + val.x.ToString("0.0") + " " + val.y.ToString("0.0") + " " + val.z.ToString("0.0"));

		GUI.color = gr;
		val.x = ProSlider(val.x, low, high, pro);
		GUI.color = gg;
		val.y = ProSlider(val.y, low, high, pro);
		GUI.color = gb;
		val.z = ProSlider(val.z, low, high, pro);

		GUI.color = Color.white;

		return val;
	}

	public Color ColSlider(string name, ref Color val, float low, float high, float pro)
	{
		GUILayout.Label(name + " " + val.r.ToString("0.0") + " " + val.g.ToString("0.0") + " " + val.b.ToString("0.0"));

		GUI.color = gr;
		val.r = ProSlider(val.r, low, high, pro);
		GUI.color = gg;
		val.g = ProSlider(val.g, low, high, pro);
		GUI.color = gb;
		val.b = ProSlider(val.b, low, high, pro);

		GUI.color = Color.white;
		val.a = ProSlider(val.a, low, high, pro);

		return val;
	}

	public float butwidth = 166.0f;

	public Color bcol = new Color(245, 177, 17);	//190, 75);
	public Color bcol1 = new Color(96, 149, 255);

	void ShowCommon(MegaModifier md, int i)
	{
		if ( (i & 1) == 1 )
			GUI.color = bcol;	//Color.red;
		else
			GUI.color = bcol1;	//Color.red;

		if ( GUILayout.Button(md.ModName(), GUILayout.Width(butwidth)) )
			showmod[i] = showmod[i] ? false : true;

		if ( showmod[i] )
		{
			GUI.color = Color.white;

			md.ModEnabled = GUILayout.Toggle(md.ModEnabled, "Enabled");

			if ( showcommon )
			{
				ProSlider("Offset", ref md.Offset, -4.0f, 4.0f, objsize);
				ProSlider("Pos", ref md.gizmoPos, -4.0f, 4.0f, objsize);
				ProSlider("Rot", ref md.gizmoRot, -90.0f, 90.0f, objsize);

				int order = EditInt("Order", md.Order);

				if ( order != md.Order )
				{
					md.Order = order;
					context.Sort();
				}
			}
		}
	}

	void ShowGCommon(MegaModifier md, int i)
	{
		if ( (i & 1) == 1 )
			GUI.color = bcol;	//Color.red;
		else
			GUI.color = bcol1;	//Color.red;

		if ( GUILayout.Button(md.ModName(), GUILayout.Width(butwidth)) )
			showgmod[i] = showgmod[i] ? false : true;

		GUI.color = Color.white;	//Color.red;

		if ( showgmod[i] )
		{
			md.ModEnabled = GUILayout.Toggle(md.ModEnabled, "Enabled");

			if ( showgcommon )
			{
				ProSlider("Offset", ref md.Offset, -4.0f, 4.0f, objsize);
				ProSlider("Pos", ref md.gizmoPos, -4.0f, 4.0f, objsize);
				ProSlider("Rot", ref md.gizmoRot, -90.0f, 90.0f, objsize);

				int order = EditInt("Order", md.Order);

				if ( order != md.Order )
				{
					md.Order = order;
					gcontext.Sort();
				}
			}
		}
	}

	void ShowGUI(MegaModifier mod)
	{
		switch ( mod.ModName() )
		{
			case "Bend":
				{
					MegaBend bmod = (MegaBend)mod;
					bmod.angle	= AngleSlider("Angle", bmod.angle, 2.0f);
					bmod.dir	= AngleSlider("Direction", bmod.dir, 1.0f);
					bmod.axis	= (MegaAxis)XYZ("Axis", (int)bmod.axis);
					bmod.doRegion = GUILayout.Toggle(bmod.doRegion, "DoRegion");
					if ( bmod.doRegion )
					{
						bmod.from = Slider("From", bmod.from, -40.0f, 0.0f);
						bmod.to = Slider("To", bmod.to, 0.0f, 40.0f);
					}
				}
				break;

			case "Hump":
				MegaHump hmod = (MegaHump)mod;
				Vector3 size = mod.bbox.Size();
				float sz = size.magnitude * 4.0f;
				hmod.amount = ProSlider("Amount", hmod.amount, -2.0f, 2.0f, sz);	//objsize);
				hmod.cycles = Slider("Cycles", hmod.cycles, 0.0f, 20.0f);
				hmod.phase = Slider("Phase", hmod.phase, 0.0f, 10.0f);
				hmod.axis = (MegaAxis)XYZ("Axis", (int)hmod.axis);
				hmod.animate = GUILayout.Toggle(hmod.animate, "Animate");
				if ( hmod.animate )
					hmod.speed = Slider("Speed", hmod.speed, -10.0f, 10.0f);
				break;

			case "Twist":
				{
					MegaTwist tmod = (MegaTwist)mod;
					tmod.angle = AngleSlider("Angle", tmod.angle, 2.0f);
					tmod.Bias = Slider("Bias", tmod.Bias, -40.0f, 40.0f);
					tmod.axis = (MegaAxis)XYZ("Axis", (int)tmod.axis);
					tmod.doRegion = GUILayout.Toggle(tmod.doRegion, "DoRegion");
					if ( tmod.doRegion )
					{
						tmod.from = Slider("From", tmod.from, -40.0f, 0.0f);
						tmod.to = Slider("To", tmod.to, 0.0f, 40.0f);
					}
				}
				break;

			case "Taper":
				{
					MegaTaper tmod = (MegaTaper)mod;

					tmod.amount = Slider("Amount", tmod.amount, -10.0f, 10.0f);
					tmod.axis = (MegaAxis)XYZ("Axis", (int)tmod.axis);
					tmod.EAxis = (MegaEffectAxis)EAxisXYZ("EffectAxis", (int)tmod.EAxis);
					tmod.dir = AngleSlider("Direction", tmod.dir, 1.0f);
					tmod.crv = Slider("Curve", tmod.crv, -10.0f, 10.0f);
					tmod.sym = GUILayout.Toggle(tmod.sym, "Symmetry");
					tmod.doRegion = GUILayout.Toggle(tmod.doRegion, "Limit Effect");

					if ( tmod.doRegion )
					{
						tmod.from = ProSlider("From", tmod.from, 0.0f, 1.0f, objsize);
						tmod.to = ProSlider("To", tmod.to, 0.0f, 1.0f, objsize);
					}
				}
				break;

			case "FFD3x3x3":
				MegaFFD fmod = (MegaFFD)mod;
				for ( int i = 0; i < 27; i++ )
				{
					string name = "p" + i;
					fmod.pt[i] = ProSlider(name, ref fmod.pt[i], -2.0f, 2.0f, objsize);
				}
				break;

			case "Noise":
				MegaNoise nmod = (MegaNoise)mod;
				nmod.Scale = Slider("Scale", nmod.Scale, 0.0f, 10.0f);
				nmod.Freq = Slider("Freq", nmod.Freq, 0.0f, 30.0f);
				nmod.Phase = Slider("Phase", nmod.Phase, 0.0f, 10.0f);
				nmod.Strength = ProSlider("Strength", ref nmod.Strength, 0.0f, 1.0f, objsize);
				nmod.Animate = GUILayout.Toggle(nmod.Animate, "Animate");
				nmod.Fractal = GUILayout.Toggle(nmod.Fractal, "Fractal");
				if ( nmod.Fractal )
				{
					nmod.Rough = Slider("Rough", nmod.Rough, 0.0f, 1.0f);
					nmod.Iterations = Slider("Iterations", nmod.Iterations, 0.0f, 10.0f);
				}
				break;

			case "Ripple":
				MegaRipple rmod = (MegaRipple)mod;
				rmod.animate = GUILayout.Toggle(rmod.animate, "Animate");
				if ( rmod.animate )
					rmod.Speed = Slider("Speed", rmod.Speed, -4.0f, 4.0f);

				rmod.amp = ProSlider("Amp", rmod.amp, -1.0f, 1.0f, objsize);
				rmod.amp2 = ProSlider("Amp2", rmod.amp2, -1.0f, 1.0f, objsize);
				rmod.flex = Slider("Flex", rmod.flex, -10.0f, 10.0f);
				rmod.wave = Slider("Wave", rmod.wave, -25.0f, 25.0f);
				rmod.phase = Slider("Phase", rmod.phase, -100.0f, 100.0f);
				rmod.Decay = Slider("decay", rmod.Decay, 0.0f, 500.0f);
				break;

			case "Wave":
				MegaWave wmod = (MegaWave)mod;
				wmod.animate = GUILayout.Toggle(wmod.animate, "Animate");
				if ( wmod.animate )
					wmod.Speed = Slider("Speed", wmod.Speed, -4.0f, 4.0f);

				wmod.amp = ProSlider("Amp", wmod.amp, -1.0f, 1.0f, objsize * 0.75f);
				wmod.amp2 = ProSlider("Amp2", wmod.amp2, -1.0f, 1.0f, objsize * 0.75f);
				wmod.flex = Slider("Flex", wmod.flex, -10.0f, 10.0f);
				wmod.wave = Slider("Wave", wmod.wave, -100.0f, 100.0f);
				wmod.phase = Slider("Phase", wmod.phase, -100.0f, 100.0f);
				wmod.Decay = Slider("decay", wmod.Decay, 0.0f, 50.0f);
				wmod.dir = Slider("Direction", wmod.dir, 0.0f, 90.0f);
				break;

			case "Stretch":
				{
					MegaStretch smod = (MegaStretch)mod;
					smod.amount	= Slider("Amount", smod.amount, -4.0f, 4.0f);
					smod.amplify	= Slider("Amplify", smod.amplify, -2.0f, 2.0f);
					smod.axis		= (MegaAxis)XYZ("Axis", (int)smod.axis);
				}
				break;

			case "Bubble":
				{
					MegaBubble bmod = (MegaBubble)mod;
					bmod.radius = ProSlider("Radius", bmod.radius, -1.0f, 4.0f, objsize);
					bmod.falloff = ProSlider("Falloff", bmod.falloff, -1.0f, 1.0f, objsize);
				}
				break;

			case "Spherify":
				{
					MegaSpherify smod = (MegaSpherify)mod;
					smod.percent = Slider("Percent", smod.percent, 0.0f, 100.0f);
				}
				break;

			case "Skew":
				{
					MegaSkew smod = (MegaSkew)mod;
					smod.amount = ProSlider("Amount", smod.amount, -2.0f, 2.0f, objsize);
					smod.dir = AngleSlider("Dir", smod.dir, 1.0f);
					smod.axis = (MegaAxis)XYZ("Axis", (int)smod.axis);
				}
				break;

			case "Melt":
				MegaMelt mmod = (MegaMelt)mod;
				mmod.Amount = Slider("Amount ", mmod.Amount, 0.0f, 100.0f);
				mmod.Spread = Slider("Spread", mmod.Spread, 0.0f, 100.0f);

				InitMatList();
				GUILayout.Label("Solidity");
				mmod.MaterialType = (MegaMeltMat)GUILayout.SelectionGrid((int)mmod.MaterialType, MatList, 2, "toggle");

				if ( mmod.MaterialType == MegaMeltMat.Custom )
					mmod.Solidity = Slider("Custom", mmod.Solidity, 0.0f, 10.0f);

				mmod.axis = (MegaAxis)XYZ("Axis", (int)mmod.axis);
				mmod.FlipAxis = GUILayout.Toggle(mmod.FlipAxis, "Flip Axis");
				break;
		}
	}

	public void ShowGUI()
	{
		if ( context )
		{
			GUI.color = Color.white;

			if ( GUILayout.Button("Logo Params", GUILayout.Width(butwidth)) )
				showparams = showparams ? false : true;

			if ( showparams )
			{
				context.Enabled = GUILayout.Toggle(context.Enabled, "Enabled");
				context.recalcnorms = GUILayout.Toggle(context.recalcnorms, "Recalc Normals");
				showcommon = GUILayout.Toggle(showcommon, "Common Params");
				for ( int i = 0; i < mods.Length; i++ )
				{
					ShowCommon(mods[i], i);

					if ( showmod[i] )
						ShowGUI(mods[i]);
				}
			}
		}
	}

	void ShowGroundGUI()
	{
		if ( gcontext )
		{
			GUI.color = Color.white;

			if ( GUILayout.Button("Ground Params", GUILayout.Width(butwidth)) )
				showgparams = showgparams ? false : true;

			if ( showgparams )
			{
				gcontext.Enabled = GUILayout.Toggle(gcontext.Enabled, "Enabled");
				gcontext.recalcnorms = GUILayout.Toggle(gcontext.recalcnorms, "Recalc Normals");
				showgcommon = GUILayout.Toggle(showgcommon, "Common Params");
				for ( int i = 0; i < gmods.Length; i++ )
				{
					ShowGCommon(gmods[i], i);

					if ( showgmod[i] )
						ShowGUI(gmods[i]);
				}
			}
		}
	}

	public bool ShowGui = true;
	public float dsize = 10.0f;
	public float svd = 0.9f;
	float svh = 0.0f;

#if false
	bool showbparams = true;
	void BookGUI()
	{
		if ( pageturn )
		{
			if ( GUILayout.Button("Book Params", GUILayout.Width(butwidth)) )
				showbparams = showbparams ? false : true;

			if ( showbparams )
			{
				pageturn.turn = Slider("Turn", pageturn.turn, 0.0f, 100.0f);
				pageturn.flexer_max_angle = Slider("Flex Ang", pageturn.flexer_max_angle, 0.0f, 360.0f);
				pageturn.Flexer_crease_area = Slider("Flex Crease", pageturn.Flexer_crease_area, 0.0f, 5.0f);
				pageturn.Flexer_crease_center = Slider("Flex Cen", pageturn.Flexer_crease_center, 0.0f, 5.0f);

				pageturn.turner_max_angle = Slider("Turn Ang", pageturn.turner_max_angle, 0.0f, 360.0f);
				pageturn.turner_crease_area = Slider("Turn Crease", pageturn.turner_crease_area, 0.0f, 5.0f);
				pageturn.turner_crease_center = Slider("Turn Cen", pageturn.turner_crease_center, -5.0f, 5.0f);
				pageturn.turner_extra = Slider("Turn Rot", pageturn.turner_extra, 0.0f, 5.0f);
				pageturn.ext_rot = Slider("Ext Rot", pageturn.ext_rot, -90.0f, 90.0f);
				pageturn.lander_max_angle = Slider("Land Ang", pageturn.lander_max_angle, 0.0f, 360.0f);
				pageturn.lander_crease_area = Slider("Land Crease", pageturn.lander_crease_area, 0.0f, 5.0f);
				pageturn.lander_crease_center = Slider("Land Cen", pageturn.lander_crease_center, 0.0f, 5.0f);
			}
		}
	}
#endif

	void DoWindow(int windowID)
	{
		pos = GUILayout.BeginScrollView(pos, GUILayout.Width(sx), GUILayout.Height(svh));

		if ( mat != null )
		{
			Color col = mat.color;
			mat.color = ColSlider("Col", ref col, 0.0f, 1.0f, 2.0f);
		}

		if ( gmat != null )
		{
			Color col = gmat.color;
			gmat.color = ColSlider("Ground", ref col, 0.0f, 1.0f, 2.0f);
		}
		ShowGUI();
		ShowGroundGUI();
		//BookGUI();

		GUILayout.EndScrollView();
		GUI.DragWindow();
	}

	void Update()
	{
		if ( Input.GetKeyUp(KeyCode.Escape) )
		{
			ShowGui = ShowGui ? false : true;
		}
	}

	void SizeChange()
	{
		windowRect.yMax = windowRect.yMin + (Screen.height - dsize);
		svh = (windowRect.yMax - windowRect.yMin) - svd;	//(float)Screen.height * 0.5f;	// - dsize;	//25.0f;	//* 0.955f;
		lastscreenheight = Screen.height;
	}

	float lastscreenheight = 0.0f;
	public GUISkin	skin;
	void OnGUI()
	{
		if ( ShowGui && mods != null )
		{
			GUI.skin = skin;

			if ( Screen.height != lastscreenheight )
			{
				SizeChange();
			}

			windowRect = GUILayout.Window(0, windowRect, DoWindow, source.name + " - Modifiers", GUILayout.Width(100));	//, GUILayout.MinWidth(150), GUILayout.MaxWidth(400));	//BeginArea(new Rect(10, 5, 175, 700));
		}
		else
		{
			//float fps = 1.0f / Time.smoothDeltaTime;
			//GUI.Label(new Rect(0, 0, 100, 32), fps.ToString("0.0"));
		}
	}
}
// Vidoes
// General work flow, add object to scene, add modob and a couple of mods