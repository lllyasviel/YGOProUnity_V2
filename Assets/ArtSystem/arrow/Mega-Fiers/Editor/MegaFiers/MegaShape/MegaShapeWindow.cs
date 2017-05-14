
using UnityEditor;
using UnityEngine;

// TODO: Select axis for shapes
// TODO: Add new spline to shape
// TODO: Button to recalc lengths
// TEST: Build a simple scene in max then have a road, barrier, fence etc
// Import of simple text file for path
public class MegaShapeWindow : EditorWindow
{
	static bool		showcommon;
	//string name = "Shape";
	static MegaAxis	axis = MegaAxis.Y;
	static bool		drawknots = true;
	static bool		drawhandles = true;
	static float	stepdist = 0.5f;
	static float	knotsize = 2.0f;
	static Color	col1 = Color.white;
	static Color	col2 = Color.black;

	static bool		makemesh = false;

	// Add menu named "My Window" to the Window menu
	[MenuItem("GameObject/Mega Shapes")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		//MegaShapeWindow window = (MegaShapeWindow)EditorWindow.GetWindow(typeof(MegaShapeWindow));
		EditorWindow.GetWindow(typeof(MegaShapeWindow), false, "MegaShapes");
	}

	[MenuItem("GameObject/Create Other/MegaShape/Star Shape")]			static void CreateStarShape()	{ CreateShape("Star", typeof(MegaShapeStar)); }
	[MenuItem("GameObject/Create Other/MegaShape/Circle Shape")]		static void CreateCircleShape() { CreateShape("Circle", typeof(MegaShapeCircle)); }
	[MenuItem("GameObject/Create Other/MegaShape/NGon Shape")]			static void CreateNGonShape() { CreateShape("NGon", typeof(MegaShapeNGon)); }
	[MenuItem("GameObject/Create Other/MegaShape/Arc Shape")]			static void CreateArcShape() { CreateShape("Arc", typeof(MegaShapeArc)); }
	[MenuItem("GameObject/Create Other/MegaShape/Ellipse Shape")]		static void CreateEllipseShape() { CreateShape("Ellipse", typeof(MegaShapeEllipse)); }
	[MenuItem("GameObject/Create Other/MegaShape/Rectangle Shape")]		static void CreateRectangleShape() { CreateShape("Rectangle", typeof(MegaShapeRectangle)); }
	[MenuItem("GameObject/Create Other/MegaShape/Helix Shape")]			static void CreateHelixShape() { CreateShape("Helix", typeof(MegaShapeHelix)); }

	static Color butcol = new Color(0.75f, 0.75f, 1.0f);

	static ModBut[] mods = new ModBut[] {
		new ModBut("Arc",			"Create a Arc Shape",		typeof(MegaShapeArc), butcol),
		new ModBut("Circle",		"Create a Circle Shape",	typeof(MegaShapeCircle), butcol),
		new ModBut("Ellipse",		"Create a Ellipse Shape",	typeof(MegaShapeEllipse), butcol),
		new ModBut("Helix",			"Create a Helix Shape",		typeof(MegaShapeHelix), butcol),
		new ModBut("NGon",			"Create a NGon Shape",		typeof(MegaShapeNGon), butcol),
		new ModBut("Rectangle",		"Create a Rectangle Shape",	typeof(MegaShapeRectangle), butcol),
		new ModBut("Star",			"Create a Star Shape",		typeof(MegaShapeStar), butcol),
	};

	void DoButtons(ModBut[] buttons, float width, int bstep, bool modobj)
	{
		Color c = GUI.backgroundColor;
		int off = 0;
		GUI.backgroundColor = Color.blue;
		Color guicol = GUI.color;
		GUI.color = new Color(1, 1, 1, 1);
		GUI.backgroundColor = new Color(0, 0, 0, 0);
		GUI.contentColor = Color.white;

		for ( int i = 0; i < buttons.Length; i++ )
		{
			//GUI.backgroundColor = buttons[i].color;	//Color.blue;
			GUI.contentColor = buttons[i].color;	//Color.blue;
			GUI.backgroundColor = buttons[i].color * 0.08f;

			if ( off == 0 )
				EditorGUILayout.BeginHorizontal();

			if ( GUILayout.Button(buttons[i].content, GUILayout.Width(width)) )
			{
				CreateShape(buttons[i].name, buttons[i].classname);
			}

			off++;
			if ( off == bstep )
			{
				off = 0;
				EditorGUILayout.EndHorizontal();
			}
		}

		if ( off != 0 )
			EditorGUILayout.EndHorizontal();

		GUI.backgroundColor = c;
		GUI.color = guicol;
	}


	Vector2 scroll = Vector2.zero;
	int toolbarInt = 0;
	string[] toolbarStrings = { "Shapes", "Params" };

	// Put common params in, and each shape has its sections
	void OnGUI()
	{
		scroll = EditorGUILayout.BeginScrollView(scroll);

		//name = EditorGUILayout.TextField("Name", name);

		toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings, GUILayout.MaxWidth(150.0f));

		float butwidth = 80.0f;
		float width = this.position.width;	// / 2.0f;

		int bstep = (int)(width / butwidth);
		if ( bstep == 0 )
			bstep = 1;

		if ( toolbarInt == 0 )
		{
			DoButtons(mods, (width / bstep) - 6.0f, bstep, true);
		}
		else
		{
			//showcommon = EditorGUILayout.Foldout(showcommon, "Common");

			//if ( showcommon )
			{
				axis		= (MegaAxis)EditorGUILayout.EnumPopup("Axis", axis);
				stepdist	= EditorGUILayout.FloatField("Step Dist", stepdist);
				knotsize	= EditorGUILayout.FloatField("Knot Size", knotsize);
				drawknots	= EditorGUILayout.Toggle("Draw Knots", drawknots);
				drawhandles	= EditorGUILayout.Toggle("Draw Handles", drawhandles);
				col1		= EditorGUILayout.ColorField("Color 1", col1);
				col2		= EditorGUILayout.ColorField("Color 2", col2);
				makemesh	= EditorGUILayout.Toggle("Make Mesh", makemesh);
			}
		}
		EditorGUILayout.EndScrollView();
	}

#if false
	static void CreateShape(string type)
	{
		Vector3 pos = UnityEditor.SceneView.lastActiveSceneView.pivot;

		MegaShape ms = null;
		GameObject go = new GameObject(type + " Shape");
		
		switch ( type )
		{
			case "Circle":		ms = go.AddComponent<MegaShapeCircle>(); break;
			case "Star":		ms = go.AddComponent<MegaShapeStar>(); break;
			case "NGon":		ms = go.AddComponent<MegaShapeNGon>(); break;
			case "Arc":			ms = go.AddComponent<MegaShapeArc>(); break;
			case "Ellipse":		ms = go.AddComponent<MegaShapeEllipse>(); break;
			case "Rectangle":	ms = go.AddComponent<MegaShapeRectangle>(); break;
			case "Helix":		ms = go.AddComponent<MegaShapeHelix>(); break;
		}

		go.transform.position = pos;
		Selection.activeObject = go;

		if ( ms != null )
		{
			ms.axis			= axis;
			ms.drawHandles	= drawhandles;
			ms.drawKnots	= drawknots;
			ms.col1			= col1;
			ms.col2			= col2;
			ms.KnotSize		= knotsize;
			ms.stepdist		= stepdist;
		}
	}
#endif
	static void CreateShape(string type, System.Type classtype)
	{
		Vector3 pos = UnityEditor.SceneView.lastActiveSceneView.pivot;

		GameObject go = new GameObject(type + " Shape");

		MegaShape ms = (MegaShape)go.AddComponent(classtype);

		go.transform.position = pos;
		Selection.activeObject = go;

		if ( ms != null )
		{
			ms.axis			= axis;
			ms.drawHandles	= drawhandles;
			ms.drawKnots	= drawknots;
			ms.col1			= col1;
			ms.col2			= col2;
			ms.KnotSize		= knotsize;
			ms.stepdist		= stepdist;
			ms.makeMesh		= makemesh;
		}
	}
}