/*=============={KEEP INFO INTACT}==================
===   Name: Rope Editor Version 2.1              ===
===   Company: Reverie Interactive               ===
===   ----------------------------------------   ===
===   Written By: Jacob Fletcher                 ===
===   Release: September, 26, 2010               ===
===   ----------------------------------------   ===
===   Copyright: Reverie Interactive             ===
===   License: Free Use if this box is left alone  =
==================================================*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class RopeEditor : EditorWindow
{
    string newName = "";
    Vector2 windowPosition = Vector2.zero;
    GameObject[] objectWithRope;
    GameObject bridgePlank;
    int ropeSelection = 0;
    Rope2 ropeObj;
    Rope2[] ropeObjects = new Rope2[0];
    List<string> ropeObjNames = new List<string>();
    string[] selections = new string[] { "Physics", "Joints", "Weak", "Attach", "Control" };
    int selected = 0;
    bool editorFocused = false;
    bool firstRun = true;
    bool linkScale = true;
    bool showPreviewSettings = false;

    // Add menu named "My Window" to the Window menu
    [MenuItem("RI Editors/Rope Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        RopeEditor window = (RopeEditor)EditorWindow.GetWindow(typeof(RopeEditor));
        window.minSize = new Vector2(350,50);
    }
    void Update()
    {
        ropeObjects = (Rope2[])GameObject.FindObjectsOfType(typeof(Rope2));

        if (firstRun == true)
        {
            firstRun = false;
            RefreshRopeList();
        }

        if (ropeObjects.Length != ropeObjNames.Count && ropeObjNames[0] != "None" || firstRun == true)
        {
            RefreshRopeList();
        }
        
        if(ropeObjects.Length > 0)
        {
            ropeObj = ropeObjects[ropeSelection];
        }

        Repaint();
    }
    void OnGUI()
    {
        Event e = Event.current;
        if (e.button == 0 && e.isMouse && editorFocused == false)
        {
            editorFocused = true;
        }

            ropeSelection = EditorGUILayout.Popup("Select Object", ropeSelection, ropeObjNames.ToArray());

            if (ropeObj != null)
            {
                newName = EditorGUILayout.TextField("Rename: ", ropeObj.gameObject.name);

                if (editorFocused)
                {
                    ropeObj.name = newName;
                    FocusOnObject(ropeSelection);
                }

                if (ropeObjNames[ropeSelection] != ropeObj.gameObject.name)
                    RefreshRopeList();

                windowPosition = EditorGUILayout.BeginScrollView(windowPosition);

                /*EDITOR IS ACTIVE*/
                GUI.contentColor = Color.white;
                EditorGUILayout.Separator();
                EditorGUILayout.BeginToggleGroup("In Edit Mode", !EditorApplication.isPlaying);

                ropeObj.PointA = (GameObject)EditorGUILayout.ObjectField("Rope End A", ropeObj.PointA, typeof(GameObject));
                ropeObj.PointB = (GameObject)EditorGUILayout.ObjectField("Rope End B", ropeObj.PointB, typeof(GameObject));

                if (ropeObj.PointA && ropeObj.PointB)
                {
                    ropeObj.isPointARestrained = EditorGUILayout.Toggle("A is restrained", ropeObj.isPointARestrained);
                    ropeObj.isPointBRestrained = EditorGUILayout.Toggle("B is restrained", ropeObj.isPointBRestrained);
                    //ropeObj.ParentRopeObjectTo = (Rope2.ParentObject)EditorGUILayout.EnumPopup("Parent Rope To", ropeObj.ParentRopeObjectTo);
                    ropeObj.ropeDetail = (int)Mathf.Clamp(EditorGUILayout.IntField("Rope Detail", ropeObj.ropeDetail), 1, Mathf.Infinity);
                    ropeObj.ropeWidth = Mathf.Clamp(EditorGUILayout.FloatField("Rope Width", ropeObj.ropeWidth), 0.0001f, Mathf.Infinity);
                    ropeObj.colliderType = (Rope2.ColliderType)EditorGUILayout.EnumPopup("Collider Type", ropeObj.colliderType);
                    if (ropeObj.colliderType == Rope2.ColliderType.Cube)
                    {
                        ropeObj.boxColliderSize = EditorGUILayout.Vector3Field("Collider Size", ropeObj.boxColliderSize);
                        if(ropeObj.chainLinkSettings.alternateChain)
                            ropeObj.altColliderSize = EditorGUILayout.Vector3Field("Alt. Collider Size", ropeObj.altColliderSize);
                    }

                    // Select Rope Mesh Setting
                    ropeObj.ropeMeshType = (Rope2.RopeMeshType)EditorGUILayout.EnumPopup("Rope Type", ropeObj.ropeMeshType);

                    //Which Menu Should We Draw
                    switch (ropeObj.ropeMeshType)
                    {
                        case Rope2.RopeMeshType.Prefab:
                            DrawPrefabSettings();
                            break;
                        case Rope2.RopeMeshType.LineRender:
                            DrawLineRenderSettings();
                            break;
                        case Rope2.RopeMeshType.NoSkin:
                            DrawNoneSettings();
                            break;
                    }
                    EditorGUILayout.Separator();

                    selected = GUILayout.Toolbar(selected, selections);

                    EditorGUILayout.Separator();

                    switch (selected)
                    {
                        case 0:
                            DrawPhysicsSettings();
                            break;
                        case 1:
                            DrawJointSettings();
                            break;
                        case 2:
                            DrawWeakLinkSettings();
                            break;
                        case 3:
                            DrawAttachPoint();
                            break;
                        case 4:
                            DrawControls();
                            break;
                    }

                    EditorGUILayout.Space();
                }
                else
                {
                    /*NO POINTA OR POINTB ASSIGNED*/
                    EditorGUILayout.Space();
                    GUI.contentColor = Color.yellow;
                    GUILayout.Label("PointA and PointB must be assigned to!");
                }

                GUILayout.FlexibleSpace();

                EditorGUILayout.EndToggleGroup();
                EditorGUILayout.EndScrollView();

                // Show 2 buttons if the editory isnt playing
                if (!EditorApplication.isPlaying)
                {
                    if (showPreviewSettings = EditorGUILayout.Foldout(showPreviewSettings, "Preview Settings:"))
                    {
                        ropeObj.allowPreview = EditorGUILayout.Toggle("Real Preview", ropeObj.allowPreview);
                        EditorGUILayout.BeginHorizontal();
                        ropeObj.showColliders = EditorGUILayout.Toggle("Show Colliders", ropeObj.showColliders);
                        ropeObj.showJointGizmos = EditorGUILayout.Toggle("Show Gizmos", ropeObj.showJointGizmos);
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.BeginHorizontal();
                    if (!ropeObj.allowPreview)
                    {
                        if (GUILayout.Button("Build Preview"))
                        {
                            ropeObj.CreateTempRopeJoints();
                        }
                        if (GUILayout.Button("Destroy Preview"))
                        {
                            ropeObj.DestroyRopePreview();
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space(10);
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Create New"))
                    {
                        CreateRope();
                    }
                    if (GUILayout.Button("Duplicate"))
                    {
                        DuplicateChain();
                    }
                    if (GUILayout.Button("Help"))
                    {
                        Application.OpenURL("http://reverieinteractive.com/cgi/ropehelpfile.php");
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Separator();
                }
            }
            else
            {
                GUI.contentColor = Color.yellow;
                EditorGUILayout.LabelField("Editing Rope: ", "NO ROPE OBJECTS IN SCENE");
                GUILayout.TextArea("To select a rope objec to edit, use the dropdown menu above. Otherwise just click on a gameObject that has the Rope2 component, and it will automatically focus on that object!");
                if (GUILayout.Button("Create new rope object"))
                {
                    selected = 0;
                    CreateRope();
                }
            }
    }
    void OnSelectionChange()
    {
        editorFocused = false;
    }

    void DrawAttachPoint()
    {

        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        GUILayout.BeginVertical();
        for (int i = 0; i < ropeObj.attachedObjects.Count; i++)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("[Attached Object " + (i + 1) + "] -------------------------");
            GUI.contentColor = Color.yellow;
            if (GUILayout.Button("<-- Delete", GUILayout.Width(70)))
            {
                ropeObj.attachedObjects.RemoveAt(i);
                return;
            }
            GUI.contentColor = Color.white;
            GUILayout.EndHorizontal();

            if (ropeObj.attachedObjects[i].attachedObject == null)
            {
                GUI.contentColor = Color.yellow;
                GUILayout.Label("ATTACHED OBJECT " + (i + 1) + " CANT BE NULL!");
            }
            else
                GUI.contentColor = Color.white;
            ropeObj.attachedObjects[i].attachedObject = (GameObject)EditorGUILayout.ObjectField("Object", ropeObj.attachedObjects[i].attachedObject, typeof(GameObject));
            ropeObj.attachedObjects[i].jointIndex = Mathf.Clamp(EditorGUILayout.IntField("Link Index", ropeObj.attachedObjects[i].jointIndex), 0, ropeObj.GetJointCount());
            GUI.contentColor = Color.white;
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUILayout.Button("Add Attached Object", GUILayout.Width(150)))
        {
        	Rope2.AttachPoint ap = new Rope2.AttachPoint();
        	ap.jointIndex = 0;
        	ap.attachedObject = null;
        	ropeObj.attachedObjects.Add(ap);
        }
    }
    void DrawWeakLinkSettings()
    {
        if (ropeObj.ropeMeshType == Rope2.RopeMeshType.Prefab)
        {
            ropeObj.chainLinkSettings.weakLinkBreakingForce = EditorGUILayout.FloatField("Break Force", ropeObj.chainLinkSettings.weakLinkBreakingForce);
            ropeObj.chainLinkSettings.makeAllWeak = EditorGUILayout.Toggle("All Joints Weak", ropeObj.chainLinkSettings.makeAllWeak);
            if (!ropeObj.chainLinkSettings.makeAllWeak)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginVertical();

                for (int i = 0; i < ropeObj.chainLinkSettings.weakLinkIndex.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(" [Breakable Link " + (i + 1) + "] ---------------------------");
                    GUI.contentColor = Color.yellow;
                    if (GUILayout.Button("<-- Delete", GUILayout.Width(70)))
                    {
                        ropeObj.chainLinkSettings.weakLinkIndex.RemoveAt(i);
                        return;
                    }
                    GUI.contentColor = Color.white;
                    GUILayout.EndHorizontal();

                    ropeObj.chainLinkSettings.weakLinkIndex[i] = Mathf.Clamp(EditorGUILayout.IntField("Link Index", ropeObj.chainLinkSettings.weakLinkIndex[i]), 0, ropeObj.GetJointCount());
                }

                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                EditorGUILayout.Space();

                if (GUILayout.Button("Add Breakable Link", GUILayout.Width(150)))
                {
                    ropeObj.chainLinkSettings.weakLinkIndex.Add(0);
                }
            }
        }
        else
        {
            GUI.contentColor = Color.yellow;
            GUILayout.Label("You can only have links when \"Rope Type\" = \"Prefab\"");
        }
        GUI.contentColor = Color.white;
    }
    void DrawLineRenderSettings()
    {
        EditorGUILayout.Space();
        ropeObj.material = (Material)EditorGUILayout.ObjectField("Rope Material", ropeObj.material, typeof(Material));
    }

    float s = 1.0f;
    void DrawPrefabSettings()
    {
        if (ropeObj.chainLinkSettings.linkPrefab)
        {
            GUI.contentColor = Color.white;
        }
        else
        {
            GUI.contentColor = Color.yellow;
            GUILayout.Label("WARNING: MISSING LINK PREFAB");
        }
        ropeObj.chainLinkSettings.linkPrefab = (GameObject)EditorGUILayout.ObjectField("Link Prefab", ropeObj.chainLinkSettings.linkPrefab, typeof(GameObject));
        ropeObj.chainLinkSettings.longestAxis = (Rope2.LongAxis)EditorGUILayout.EnumPopup("Long Axis", ropeObj.chainLinkSettings.longestAxis);
        if (!(linkScale = EditorGUILayout.Toggle("Link X,Y,Z", linkScale)))
        {
            ropeObj.chainLinkSettings.linkScale = EditorGUILayout.Vector3Field("",ropeObj.chainLinkSettings.linkScale);
        }
        else
        {
            s = EditorGUILayout.FloatField("Link Scale (Linked)", ropeObj.chainLinkSettings.linkScale.x);
            ropeObj.chainLinkSettings.linkScale = new Vector3(s,s,s);
        }
        ropeObj.chainLinkSettings.alternateChain = EditorGUILayout.BeginToggleGroup("Alternate Links", ropeObj.chainLinkSettings.alternateChain);
        ropeObj.chainLinkSettings.rotation = EditorGUILayout.Vector3Field("Link Rotation", ropeObj.chainLinkSettings.rotation);
        EditorGUILayout.EndToggleGroup();
        EditorGUILayout.Space();
    }
    void DrawNoneSettings()
    {
        GUI.contentColor = Color.yellow;
        EditorGUILayout.Space();
        GUILayout.Label("No Skin: Rope will not be visible.");
        GUI.contentColor = Color.white;
    }
    void DrawPhysicsSettings()
    {
            ropeObj.jointPhysicsSettings.physicsMaterial = (PhysicMaterial)EditorGUILayout.ObjectField("Physics Material", ropeObj.jointPhysicsSettings.physicsMaterial, typeof(PhysicMaterial));
            ropeObj.jointPhysicsSettings.JointsUseGravity = EditorGUILayout.Toggle("Use Gravity", ropeObj.jointPhysicsSettings.JointsUseGravity);
            ropeObj.jointPhysicsSettings.JointMass = Mathf.Clamp(EditorGUILayout.FloatField("Joint Mass", ropeObj.jointPhysicsSettings.JointMass), 0, Mathf.Infinity);
            ropeObj.jointPhysicsSettings.JointDrag = Mathf.Clamp(EditorGUILayout.FloatField("Joint Drag", ropeObj.jointPhysicsSettings.JointDrag), 0, Mathf.Infinity);
            ropeObj.jointPhysicsSettings.JointAngularDrag = Mathf.Clamp(EditorGUILayout.FloatField("Joint Angular Drag", ropeObj.jointPhysicsSettings.JointAngularDrag), 0, Mathf.Infinity);
            ropeObj.jointPhysicsSettings.interpolation = (RigidbodyInterpolation)EditorGUILayout.EnumPopup("Interpolation Mode", ropeObj.jointPhysicsSettings.interpolation);
            GUILayout.Space(10);
            GUILayout.Label("Physics Iteration [Affects ALL Scene Objects]");
            Physics.solverIterationCount = (int)EditorGUILayout.Slider(Physics.solverIterationCount, 5, 100);
            GUILayout.Space(10);
            GUILayout.Label("Collision Mode Used in Unity 3.0 ONLY");
            ropeObj.jointPhysicsSettings.collisionMode = (Rope2.CollisionDetectMode)EditorGUILayout.EnumPopup("Collision Mode", ropeObj.jointPhysicsSettings.collisionMode);
        //DrawJointSettings();
    }
    void DrawJointSettings()
    {
            ropeObj.jointSettings.CJoint_axis = EditorGUILayout.Vector3Field("Axis", ropeObj.jointSettings.CJoint_axis);
            ropeObj.jointSettings.CJoint_swingAxis = EditorGUILayout.Vector3Field("Swing Axis", ropeObj.jointSettings.CJoint_swingAxis);
            ropeObj.jointSettings.CJoint_lowTwist_Limit = Mathf.Clamp(EditorGUILayout.FloatField("LowTwist Limit", ropeObj.jointSettings.CJoint_lowTwist_Limit), -180, 180);
            ropeObj.jointSettings.CJoint_lowTwist_Spring = EditorGUILayout.FloatField("LowTwist Spring", ropeObj.jointSettings.CJoint_lowTwist_Spring);
            ropeObj.jointSettings.CJoint_lowTwist_Dampen = EditorGUILayout.FloatField("LowTwist Dampen", ropeObj.jointSettings.CJoint_lowTwist_Dampen);
            ropeObj.jointSettings.CJoint_lowTwist_Bounce = EditorGUILayout.FloatField("LowTwist Bounce", ropeObj.jointSettings.CJoint_lowTwist_Bounce);

            ropeObj.jointSettings.CJoint_highTwist_Limit = Mathf.Clamp(EditorGUILayout.FloatField("HighTwist Limit", ropeObj.jointSettings.CJoint_highTwist_Limit), -180, 180);
            ropeObj.jointSettings.CJoint_highTwist_Spring = EditorGUILayout.FloatField("HighTwist Spring", ropeObj.jointSettings.CJoint_highTwist_Spring);
            ropeObj.jointSettings.CJoint_highTwist_Dampen = EditorGUILayout.FloatField("HighTwist Dampen", ropeObj.jointSettings.CJoint_highTwist_Dampen);
            ropeObj.jointSettings.CJoint_highTwist_Bounce = EditorGUILayout.FloatField("HighTwist Bounce", ropeObj.jointSettings.CJoint_highTwist_Bounce);

            ropeObj.jointSettings.CJoint_swing1_Limit = Mathf.Clamp(EditorGUILayout.FloatField("Swing1 Limit", ropeObj.jointSettings.CJoint_swing1_Limit), -180, 180);
            ropeObj.jointSettings.CJoint_swing1_Spring = EditorGUILayout.FloatField("Swing1 Spring", ropeObj.jointSettings.CJoint_swing1_Spring);
            ropeObj.jointSettings.CJoint_swing1_Dampen = EditorGUILayout.FloatField("Swing1 Dampen", ropeObj.jointSettings.CJoint_swing1_Dampen);
            ropeObj.jointSettings.CJoint_swing1_Bounce = EditorGUILayout.FloatField("Swing1 Bounce", ropeObj.jointSettings.CJoint_swing1_Bounce);

            ropeObj.jointSettings.CJoint_swing2_Limit = Mathf.Clamp(EditorGUILayout.FloatField("Swing2 Limit", ropeObj.jointSettings.CJoint_swing2_Limit), -180, 180);
            ropeObj.jointSettings.CJoint_swing2_Spring = EditorGUILayout.FloatField("Swing2 Spring", ropeObj.jointSettings.CJoint_swing2_Spring);
            ropeObj.jointSettings.CJoint_swing2_Dampen = EditorGUILayout.FloatField("Swing2 Dampen", ropeObj.jointSettings.CJoint_swing2_Dampen);
            ropeObj.jointSettings.CJoint_swing2_Bounce = EditorGUILayout.FloatField("Swing2 Bounce", ropeObj.jointSettings.CJoint_swing2_Bounce);
    }
    
    void DrawControls()
    {
        ropeObj.ropeControlEnabled = EditorGUILayout.BeginToggleGroup("Enabled", ropeObj.ropeControlEnabled);
        ropeObj.ropeRaiseLowerSpeed = EditorGUILayout.FloatField("Motor Speed", ropeObj.ropeRaiseLowerSpeed);
        ropeObj.ropeControlPull = (KeyCode)EditorGUILayout.EnumPopup("Pull Control", ropeObj.ropeControlPull);
        ropeObj.ropeControlPush = (KeyCode)EditorGUILayout.EnumPopup("Push Control", ropeObj.ropeControlPush);
        ropeObj.windRopeAtStart = EditorGUILayout.Toggle("Start Raised", ropeObj.windRopeAtStart);
        EditorGUILayout.EndToggleGroup();
    }


    void CreateRope()
    {
        GameObject baseObj = new GameObject("NewRopeObject").AddComponent<Rope2>().gameObject;
        GameObject pA = new GameObject("Point A");
        GameObject pB = new GameObject("Point B");

        baseObj.transform.position = Vector3.zero;
        pA.transform.position = new Vector3(-10, 0, 0);
        pB.transform.position = new Vector3(10, 0, 0);

        baseObj.GetComponent<Rope2>().PointA = pA;
        baseObj.GetComponent<Rope2>().PointB = pB;

        pA.transform.parent = baseObj.transform;
        pB.transform.parent = baseObj.transform;

        RefreshRopeList();
    }
    void DuplicateChain()
    {
        Object newOb = Instantiate(ropeObj);
        newOb.name = ropeObj.name + "_Copy";
        RefreshRopeList();
    }

    void RefreshRopeList()
    {
        ropeObjNames.Clear();
        if (ropeObjects.Length > 0)
        {
            foreach (Rope2 s in ropeObjects)
            {
                ropeObjNames.Add(s.gameObject.name);
            }
        }
        else
        {
            ropeObjNames.Add("None");
        }
    }
    void FocusOnObject(int selection)
    {
        Selection.activeGameObject = ropeObjects[selection].gameObject;
    }
}