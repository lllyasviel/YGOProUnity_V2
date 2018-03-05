// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;
using TMPro;
using TMPro.EditorUtilities;



public class TMPro_SDFMaterialEditor : MaterialEditor
{
    private struct m_foldout
    { // Track Inspector foldout panel states, globally.
        public static bool face = true;
        public static bool outline = true;
        public static bool underlay = false;
        public static bool bevel = false;
        public static bool light = false;
        public static bool bump = false;
        public static bool env = false;
        public static bool glow = false;
        public static bool debug = false;
    }

    private enum FoldoutType { face, outline, underlay, bevel, light, bump, env, glow, debug };

    //private static PropertyModification m_modifiedProperties;
    private static int m_eventID;
	//private Material m_targetMaterial;


    // Face Properties
    private MaterialProperty m_faceColor;
    private MaterialProperty m_faceTex;
    private MaterialProperty m_faceUVSpeedX;
    private MaterialProperty m_faceUVSpeedY;
    private MaterialProperty m_faceDilate;
    private MaterialProperty m_faceShininess;
    //private MaterialProperty m_faceSoftness;

    // Outline Properties
    private MaterialProperty m_outlineColor;
    private MaterialProperty m_outlineTex;
    private MaterialProperty m_outlineUVSpeedX;
    private MaterialProperty m_outlineUVSpeedY;
    private MaterialProperty m_outlineThickness;
    private MaterialProperty m_outlineSoftness;
    private MaterialProperty m_outlineShininess;

    // Properties Related to Bevel Options
    private MaterialProperty m_bevel;
    private MaterialProperty m_bevelOffset;
    private MaterialProperty m_bevelWidth;
    private MaterialProperty m_bevelClamp;
    private MaterialProperty m_bevelRoundness;

    // Properties for the Underlay Options
    private MaterialProperty m_underlayColor;
    private MaterialProperty m_underlayOffsetX;
    private MaterialProperty m_underlayOffsetY;
    private MaterialProperty m_underlayDilate;
    private MaterialProperty m_underlaySoftness;

    // Properties for Simulated Lighting
    private MaterialProperty m_lightAngle;
    private MaterialProperty m_specularColor;
    private MaterialProperty m_specularPower;
    private MaterialProperty m_reflectivity;
    private MaterialProperty m_diffuse;
    private MaterialProperty m_ambientLight;


    // Bump Mapping Options
    private MaterialProperty m_bumpMap;
    private MaterialProperty m_bumpFace;
    private MaterialProperty m_bumpOutline;

    // Properties for Environmental Mapping 
    private MaterialProperty m_reflectFaceColor;
    private MaterialProperty m_reflectOutlineColor;
    private MaterialProperty m_reflectTex;
    private MaterialProperty m_reflectRotation;
    //private MaterialProperty m_envTiltX;
    //private MaterialProperty m_envTiltY;
    //private MaterialProperty m_envMatrix;

    private MaterialProperty m_specColor;

    // Properties for Glow Options
    private MaterialProperty m_glowColor;
    private MaterialProperty m_glowInner;
    private MaterialProperty m_glowOffset;
    private MaterialProperty m_glowPower;
    private MaterialProperty m_glowOuter;

    // Hidden properties used for debug
    private MaterialProperty m_mainTex;
    private MaterialProperty m_texSampleWidth;
    private MaterialProperty m_texSampleHeight;
    private MaterialProperty m_gradientScale;

    private MaterialProperty m_scaleX;
    private MaterialProperty m_scaleY;

    private MaterialProperty m_PerspectiveFilter;
    
    private MaterialProperty m_vertexOffsetX;
    private MaterialProperty m_vertexOffsetY;
    private MaterialProperty m_maskCoord;
    private MaterialProperty m_maskSoftnessX;
    private MaterialProperty m_maskSoftnessY;

    // Stencil Properties
    private MaterialProperty m_stencilID;
    private MaterialProperty m_stencilOp;
    private MaterialProperty m_stencilComp;
     
    //private MaterialProperty m_weightNormal;
    //private MaterialProperty m_weightBold;
     
   

    private MaterialProperty m_shaderFlags; // _ShaderFlag useed to determine bevel type.
    private MaterialProperty m_scaleRatio_A;
    private MaterialProperty m_scaleRatio_B;
    private MaterialProperty m_scaleRatio_C;



    // Private Fields  
    private enum Bevel_Types { OuterBevel = 0, InnerBevel = 1 };
    private enum Mask_Type { MaskOff = 0, MaskHard = 1, MaskSoft = 2 };

    private string[] m_bevelOptions = { "Outer Bevel", "Inner Bevel", "--" };
    private int m_bevelSelection;
    private Mask_Type m_mask;

    private enum Underlay_Types { Normal = 0, Inner = 1};
    private Underlay_Types m_underlaySelection = Underlay_Types.Normal;

    private string[] m_Keywords;

    private Vector3 m_matrixRotation;

    private bool isRatiosEnabled;
    private bool isBevelEnabled;
    private bool isGlowEnabled;
    private bool isBumpEnabled;
    private bool isEnvEnabled;
    private bool isUnderlayEnabled;
    private bool havePropertiesChanged = false;


    //private TextMeshPro m_textMeshPro;
    //private TextMeshProUGUI m_textMeshProUGUI;
    private Rect m_inspectorStartRegion;
    private Rect m_inspectorEndRegion;



    public override void OnEnable()
    {
        base.OnEnable();

        //Debug.Log("New Instance of SDF Material Editor with ID " + this.GetInstanceID());

        // Get the UI Skin and Styles for the various Editors
        TMP_UIStyleManager.GetUIStyles();

        // Get a reference to the TextMeshPro or TextMeshProUGUI object if possible
        //if (Selection.activeGameObject != null)
        //{
        //    if (Selection.activeGameObject.GetComponent<TextMeshPro>() != null)
        //    {
        //        m_textMeshPro = Selection.activeGameObject.GetComponent<TextMeshPro>();
        //    }
        //    else
        //    {
        //        m_textMeshProUGUI = Selection.activeGameObject.GetComponent<TextMeshProUGUI>();
        //    }
        //}
        
      
        // Initialize the Event Listener for Undo Events.     
        Undo.undoRedoPerformed += OnUndoRedo;
        //Undo.postprocessModifications += OnUndoRedoEvent;
    }


    public override void OnDisable()
    {
        //Debug.Log("OnDisable() called.");
        
        // Remove Undo / Redo Event Listeners.
        base.OnDisable();
      
        Undo.undoRedoPerformed -= OnUndoRedo;
        //Undo.postprocessModifications -= OnUndoRedoEvent;
    }


    public override void OnInspectorGUI()
    {        
        // if we are not visible... return
        //if (!isVisible)
        //    return;

        ReadMaterialProperties();
			
        Material targetMaterial = target as Material;

        // If multiple materials have been selected and are not using the same shader, we simply return.
        if (targets.Length > 1)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                Material mat = targets[i] as Material;

                if (targetMaterial.shader != mat.shader)
                {
                    return;
                }
            }
        }


        // Retrieve Shader Multi_Compile Keywords
        m_Keywords = targetMaterial.shaderKeywords;
        isBevelEnabled = m_Keywords.Contains("BEVEL_ON");
        isGlowEnabled = m_Keywords.Contains("GLOW_ON");
        //isUnderlayEnabled = m_Keywords.Contains("UNDERLAY_ON") | m_Keywords.Contains("UNDERLAY_INNER");
        isRatiosEnabled = !m_Keywords.Contains("RATIOS_OFF");

        if (m_Keywords.Contains("UNDERLAY_ON"))
        {
            isUnderlayEnabled = true;
            m_underlaySelection = Underlay_Types.Normal;
        }
        else if (m_Keywords.Contains("UNDERLAY_INNER"))
        {
            isUnderlayEnabled = true;
            m_underlaySelection = Underlay_Types.Inner;
        }
        else
            isUnderlayEnabled = false;


        if (m_Keywords.Contains("MASK_HARD")) m_mask = Mask_Type.MaskHard;
        else if (m_Keywords.Contains("MASK_SOFT")) m_mask = Mask_Type.MaskSoft;
        else m_mask = Mask_Type.MaskOff;


        if (m_shaderFlags.hasMixedValue)
            m_bevelSelection = 2;
        else
            m_bevelSelection = (int)m_shaderFlags.floatValue & 1;


        // Define the Drag-n-Drop Region (Start)
        m_inspectorStartRegion = GUILayoutUtility.GetRect(0f, 0f, GUILayout.ExpandWidth(true));

        // Check if Shader selection is compatible with Font Asset
        // TODO


        EditorGUIUtility.LookLikeControls(130, 50);
        

        // FACE PANEL
        EditorGUI.indentLevel = 0;
        if (GUILayout.Button("<b>Face</b> - <i>Settings</i> -", TMP_UIStyleManager.Group_Label))
            m_foldout.face = !m_foldout.face;

        if (m_foldout.face)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.indentLevel = 1;
            ColorProperty(m_faceColor, "Color");

            if (targetMaterial.HasProperty("_FaceTex"))
            {
                DrawTextureProperty(m_faceTex, "Texture");
                DrawUVProperty(new MaterialProperty[] { m_faceUVSpeedX, m_faceUVSpeedY }, "UV Speed");
            }

            DrawRangeProperty(m_outlineSoftness, "Softness");

            DrawRangeProperty(m_faceDilate, "Dilate");
            if (targetMaterial.HasProperty("_FaceShininess")) DrawRangeProperty(m_faceShininess, "Gloss");

            if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
        }


        // OUTLINE PANEL
        EditorGUI.indentLevel = 0;
        if (GUILayout.Button("<b>Outline</b> - <i>Settings</i> -", TMP_UIStyleManager.Group_Label))
            m_foldout.outline = !m_foldout.outline;

        if (m_foldout.outline)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.indentLevel = 1;
            ColorProperty(m_outlineColor, "Color");

            if (targetMaterial.HasProperty("_OutlineTex"))
            { 
                DrawTextureProperty(m_outlineTex, "Texture");
                DrawUVProperty(new MaterialProperty[] { m_outlineUVSpeedX, m_outlineUVSpeedY }, "UV Speed");
            }
            DrawRangeProperty(m_outlineThickness, "Thickness");

            if (targetMaterial.HasProperty("_OutlineShininess")) DrawRangeProperty(m_outlineShininess, "Gloss");

            if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
        }


        // UNDERLAY PANEL
        if (targetMaterial.HasProperty("_UnderlayColor"))
        {
            string underlayKeyword = m_underlaySelection == Underlay_Types.Normal ? "UNDERLAY_ON" : "UNDERLAY_INNER";
            isUnderlayEnabled = DrawTogglePanel(FoldoutType.underlay, "<b>Underlay</b> - <i>Settings</i> -", isUnderlayEnabled, underlayKeyword);


            if (m_foldout.underlay)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUI.indentLevel = 1;

                m_underlaySelection = (Underlay_Types)EditorGUILayout.EnumPopup("Underlay Type", m_underlaySelection);
                if (GUI.changed) SetUnderlayKeywords();

                ColorProperty(m_underlayColor, "Color");
                DrawRangeProperty(m_underlayOffsetX, "OffsetX");
                DrawRangeProperty(m_underlayOffsetY, "OffsetY");
                DrawRangeProperty(m_underlayDilate, "Dilate");
                DrawRangeProperty(m_underlaySoftness, "Softness");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
            }
        }

        // BEVEL PANEL
        if (targetMaterial.HasProperty("_Bevel"))
        {
            isBevelEnabled = DrawTogglePanel(FoldoutType.bevel, "<b>Bevel</b> - <i>Settings</i> -", isBevelEnabled, "BEVEL_ON");

            if (m_foldout.bevel)
            {
                EditorGUI.indentLevel = 1;
                GUI.changed = false;
                m_bevelSelection = EditorGUILayout.Popup("Type", m_bevelSelection, m_bevelOptions) & 1;
                if (GUI.changed)
                {
                    havePropertiesChanged = true;
                    m_shaderFlags.floatValue = m_bevelSelection;
                }

                EditorGUI.BeginChangeCheck();

                DrawRangeProperty(m_bevel, "Amount");
                DrawRangeProperty(m_bevelOffset, "Offset");
                DrawRangeProperty(m_bevelWidth, "Width");
                DrawRangeProperty(m_bevelRoundness, "Roundness");
                DrawRangeProperty(m_bevelClamp, "Clamp");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
            }
        }


        // LIGHTING PANEL
        if (targetMaterial.HasProperty("_SpecularColor") || targetMaterial.HasProperty("_SpecColor"))
        {
            isBevelEnabled = DrawTogglePanel(FoldoutType.light, "<b>Lighting</b> - <i>Settings</i> -", isBevelEnabled, "BEVEL_ON");

            if (m_foldout.light)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUI.indentLevel = 1;
                if (targetMaterial.HasProperty("_LightAngle"))
                { // Non Surface Shader
                    DrawRangeProperty(m_lightAngle, "Light Angle");
                    ColorProperty(m_specularColor, "Specular Color");
                    DrawRangeProperty(m_specularPower, "Specular Power");
                    DrawRangeProperty(m_reflectivity, "Reflectivity Power");
                    DrawRangeProperty(m_diffuse, "Diffuse Shadow");
                    DrawRangeProperty(m_ambientLight, "Ambient Shadow");
                }
                else
                    ColorProperty(m_specColor, "Specular Color");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
            }
        }


        // BUMPMAP PANEL
        if (targetMaterial.HasProperty("_BumpMap"))
        {
            isBevelEnabled = DrawTogglePanel(FoldoutType.bump, "<b>BumpMap</b> - <i>Settings</i> -", isBevelEnabled, "BEVEL_ON");

            if (m_foldout.bump)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel = 1;
                DrawTextureProperty(m_bumpMap, "Texture");
                DrawRangeProperty(m_bumpFace, "Face");
                DrawRangeProperty(m_bumpOutline, "Outline");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
            }
        }


        // ENVMAP PANEL
        if (targetMaterial.HasProperty("_Cube"))
        {
            isBevelEnabled = DrawTogglePanel(FoldoutType.env, "<b>EnvMap</b> - <i>Settings</i> -", isBevelEnabled, "BEVEL_ON");

            if (m_foldout.env)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUI.indentLevel = 1;
                ColorProperty(m_reflectFaceColor, "Face Color");
                ColorProperty(m_reflectOutlineColor, "Outline Color");
                DrawTextureProperty(m_reflectTex, "Texture");
                if (targetMaterial.HasProperty("_Cube"))
                {
                    DrawVectorProperty(m_reflectRotation, "EnvMap Rotation");
                    //var matrix = targetMaterial.GetMatrix("_EnvMatrix");
                  
                }

                if (EditorGUI.EndChangeCheck())
                {                   
                    //var m = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(m_matrixRotation), new Vector3(1f, 1f, 1f));
                    //targetMaterial.SetMatrix("_EnvMatrix", m);
                    havePropertiesChanged = true;
                }
            }

           
        }


        // GLOW PANEL
        if (targetMaterial.HasProperty("_GlowColor"))
        {
            isGlowEnabled = DrawTogglePanel(FoldoutType.glow, "<b>Glow</b> - <i>Settings</i> -", isGlowEnabled, "GLOW_ON");

            if (m_foldout.glow)
            {
                EditorGUI.BeginChangeCheck();

                EditorGUI.indentLevel = 1;
                ColorProperty(m_glowColor, "Color");
                DrawRangeProperty(m_glowOffset, "Offset");
                DrawRangeProperty(m_glowInner, "Inner");
                DrawRangeProperty(m_glowOuter, "Outer");
                DrawRangeProperty(m_glowPower, "Power");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
            }
        }


        // DEBUG PANEL        
        if (targetMaterial.HasProperty("_GradientScale"))
        {
            EditorGUI.indentLevel = 0;
            if (GUILayout.Button("<b>Debug</b> - <i>Settings</i> -", TMP_UIStyleManager.Group_Label))
                m_foldout.debug = !m_foldout.debug;

            if (m_foldout.debug)
            {
                EditorGUI.indentLevel = 1;

                EditorGUI.BeginChangeCheck();
       
                DrawTextureProperty(m_mainTex, "Font Atlas");
                DrawFloatProperty(m_gradientScale, "Gradient Scale");
                DrawFloatProperty(m_texSampleWidth, "Texture Width");
                DrawFloatProperty(m_texSampleHeight, "Texture Height");
                GUILayout.Space(20);

                DrawFloatProperty(m_scaleX, "Scale X");
                DrawFloatProperty(m_scaleY, "Scale Y");
                DrawRangeProperty(m_PerspectiveFilter, "Perspective Filter");

                GUILayout.Space(20);

                DrawFloatProperty(m_vertexOffsetX, "Offset X");
                DrawFloatProperty(m_vertexOffsetY, "Offset Y");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;

                // Mask                              
                if (targetMaterial.HasProperty("_MaskCoord"))
                {
                    GUILayout.Space(15);
                    m_mask = (Mask_Type)EditorGUILayout.EnumPopup("Mask", m_mask);
                    if (GUI.changed)
                    {
                        havePropertiesChanged = true;
                        SetMaskKeywords(m_mask);                     
                    }

                    
                    if (m_mask != Mask_Type.MaskOff)
                    {
                        EditorGUI.BeginChangeCheck();

                        Draw2DBoundsProperty(m_maskCoord, "Mask Bounds");
                        DrawFloatProperty(m_maskSoftnessX, "Softness X");
                        DrawFloatProperty(m_maskSoftnessY, "Softness Y");

                        if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
                            
                       
                    }
                    
                    GUILayout.Space(15);
                }

                GUILayout.Space(20);

                // Stencil
                if (targetMaterial.HasProperty("_Stencil"))
                {
                    FloatProperty(m_stencilID, "Stencil ID");
                    FloatProperty(m_stencilComp, "Stencil Comp");
                    FloatProperty(m_stencilOp, "Stencil Op");
                }
                
                GUILayout.Space(20);

                
                // Ratios
                GUI.changed = false;
                isRatiosEnabled = EditorGUILayout.Toggle("Enable Ratios?", isRatiosEnabled);
                if (GUI.changed)
                {
                    SetKeyword(!isRatiosEnabled, "RATIOS_OFF");
                }

                EditorGUI.BeginChangeCheck();

                DrawFloatProperty(m_scaleRatio_A, "Scale Ratio A");
                DrawFloatProperty(m_scaleRatio_B, "Scale Ratio B");
                DrawFloatProperty(m_scaleRatio_C, "Scale Ratio C");

                if (EditorGUI.EndChangeCheck()) havePropertiesChanged = true;
                
            }
        }

        // Define the Drag-n-Drop Region (End)
        m_inspectorEndRegion = GUILayoutUtility.GetRect(0f, 0f, GUILayout.ExpandWidth(true));


        // Handle Material Drag-n-Drop
        DragAndDropGUI();


        if (havePropertiesChanged)
        {
            //Debug.Log("Material Editor properties have changed. Target is " + target.name); 
            havePropertiesChanged = false;

            PropertiesChanged();
            EditorUtility.SetDirty(target);
            //TMPro_EditorUtility.RepaintAll(); // Look into using SetDirty.          
            TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, target as Material);
            
        }
    }



    private void DragAndDropGUI()
    {
        Event evt = Event.current;

        Rect dropArea = new Rect(m_inspectorStartRegion.x, m_inspectorStartRegion.y, m_inspectorEndRegion.width, m_inspectorEndRegion.y - m_inspectorStartRegion.y);

        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                    break;

                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    // Do something 
                    Material currentMaterial = target as Material;

                    Material newMaterial = DragAndDrop.objectReferences[0] as Material;
                    //Debug.Log("Drag-n-Drop Material is " + newMaterial + ". Target Material is " + currentMaterial); // + ".  Canvas Material is " + m_uiRenderer.GetMaterial()  );
               
                    // Check to make sure we have a valid material and that the font atlases match.
                    if (!newMaterial || newMaterial == currentMaterial || newMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID() != currentMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
                    {
                        if (newMaterial && newMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID() != currentMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
                            Debug.LogWarning("Drag-n-Drop Material [" + newMaterial.name + "]'s Atlas does not match the Atlas of the currently assigned Font Asset's Atlas.");
                        break;
                    }

                    // Check if this material is assigned to an object and active. 
                    GameObject go = Selection.activeGameObject;
                    if (go != null && !go.activeInHierarchy)
                    {
                        
                        if (go.GetComponent<TextMeshPro>() != null)
                        {
                            Undo.RecordObject(go.GetComponent<TextMeshPro>(), "Material Assignment");
                            go.GetComponent<TextMeshPro>().fontSharedMaterial = newMaterial;                        
                        }

#if UNITY_4_6 || UNITY_5
                        if (go.GetComponent<TextMeshProUGUI>() != null)
                        {
                            Undo.RecordObject(go.GetComponent<TextMeshProUGUI>(), "Material Assignment");                          
                            go.GetComponent<TextMeshProUGUI>().fontSharedMaterial = newMaterial;                          
                        }
#endif
                    }
                                                      
                    TMPro_EventManager.ON_DRAG_AND_DROP_MATERIAL_CHANGED(go, currentMaterial, newMaterial);
                    //SceneView.RepaintAll();   
                    EditorUtility.SetDirty(go);                                    
                }

                evt.Use();
                break;
        }
    }




    // Special Handling of Undo / Redo Events.
    private void OnUndoRedo()
    {
        //Debug.Log("Undo / Redo Event ID (" + Undo.GetCurrentGroup() + ") occured.");

        int UndoEventID = Undo.GetCurrentGroup();
        int LastUndoEventID = m_eventID;

        if (UndoEventID != LastUndoEventID)
        {
            //Debug.Log("Undo Redo Event processed by Material Editor. Affected Material is " + target + " with ID " + target.GetInstanceID()); // Event ID:" + UndoEventID + ".  Target ID: " + m_modifiedProperties.target.GetInstanceID() + "  Current Material: " + m_modifiedProperties.objectReference + "  New Material: " + (m_modifiedProperties.target as Renderer).sharedMaterial);
            TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, target as Material);
            //TMPro_EventManager.ON_BASE_MATERIAL_CHANGED(target as Material);
            m_eventID = UndoEventID;
        }
    }

    private UndoPropertyModification[] OnUndoRedoEvent(UndoPropertyModification[] modifications)
    {
        /*
        //Debug.Log("Undo Event Registered in SDF Material Editor. # of Properties affected is " + modifications.Length);
        
        PropertyModification modifiedProperties = modifications[0].propertyModification;
        System.Type objType = modifiedProperties.target.GetType();

        if (objType == typeof(MeshRenderer) || objType == typeof(Material)) // && UndoEventID != LastUndoEventID)
        {
            
            //Debug.Log("OnUndoRedoEvent() received in Material Editor."); // Event ID:" + UndoEventID + ".  Target ID: " + m_modifiedProperties.target.GetInstanceID() + "  Current Material: " + m_modifiedProperties.objectReference + "  New Material: " + (m_modifiedProperties.target as Renderer).sharedMaterial);
            TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, target as Material);
        }

        if (target != null)
            EditorUtility.SetDirty(target);

        */
        return modifications;
    }

   

    // Function to draw title + enable toggle options as well as setting keyword.
    private bool DrawTogglePanel(FoldoutType type, string label, bool toggle, string keyword)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        EditorGUI.indentLevel = 0;
        Rect rect = EditorGUILayout.GetControlRect(false, 22);
        GUI.Label(rect, GUIContent.none, TMP_UIStyleManager.Group_Label);
        if (GUI.Button(new Rect(rect.x, rect.y, 250, rect.height), label, TMP_UIStyleManager.Group_Label_Left))
        {
            switch (type)
            {
                case FoldoutType.underlay:
                    m_foldout.underlay = !m_foldout.underlay;
                    break;
                case FoldoutType.bevel:
                    m_foldout.bevel = !m_foldout.bevel;
                    break;
                case FoldoutType.light:
                    m_foldout.light = !m_foldout.light;
                    break;
                case FoldoutType.bump:
                    m_foldout.bump = !m_foldout.bump;
                    break;
                case FoldoutType.env:
                    m_foldout.env = !m_foldout.env;
                    break;
                case FoldoutType.glow:
                    m_foldout.glow = !m_foldout.glow;
                    break;
            }
        }

        EditorGUIUtility.labelWidth = 70;
      
        EditorGUI.BeginChangeCheck();

        Material mat = target as Material;

        if (mat.HasProperty("_FaceShininess") == false || keyword != "BEVEL_ON") // Show Enable Toggle only if material is not Surface Shader.
        {
            toggle = EditorGUI.Toggle(new Rect(rect.width - 90, rect.y + 3, 90, 22), new GUIContent("Enable ->"), toggle);
            if (EditorGUI.EndChangeCheck())
            {               
                SetKeyword(toggle, keyword);
                havePropertiesChanged = true;
            }
        }      

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;

        return toggle;
    }


    // Function to Draw UV Speed Property
    private void DrawUVProperty(MaterialProperty[] properties, string label)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        Rect rect = EditorGUILayout.GetControlRect(false, 20);
        Rect pos0 = new Rect(rect.x + 15, rect.y, rect.width - 55, 20);
        Rect pos1 = new Rect(130, rect.y, 80, 18);
        
        GUI.Label(pos0, label);
        
        EditorGUIUtility.labelWidth = 35;       
        FloatProperty(pos1, properties[0], "X");

        EditorGUIUtility.labelWidth = 35;
        FloatProperty(new Rect(pos1.x + 70, pos1.y, pos1.width, pos1.height), properties[1], "Y");

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;
    }


    // Function to Draw Material Property and make it look like a Slider with numericalf field.
    private void DrawSliderProperty(MaterialProperty property, string label)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        // Draw Slider
        //EditorGUIUtility.labelWidth = 160;
        Rect rect = EditorGUILayout.GetControlRect(false, 20);
        Rect pos0 = new Rect(rect.x, rect.y, rect.width - 55, 20);
        Rect pos1 = new Rect(rect.width - 46, rect.y, 60, 18);

        // Draw Numerical Field
        //EditorGUIUtility.labelWidth = 160;
        RangeProperty(pos0, property, label);
        EditorGUIUtility.labelWidth = 10;
        FloatProperty(new Rect(pos1), property, null);
        if (!property.hasMixedValue)
            property.floatValue = Mathf.Round(property.floatValue * 1000) / 1000;

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;
    }


    private void DrawTextureProperty(MaterialProperty property, string label)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        EditorGUIUtility.fieldWidth = 70;
        Rect rect = EditorGUILayout.GetControlRect(false, 75);
        GUI.Label(new Rect(rect.x + 15, rect.y + 5, 100, rect.height), label);
        TextureProperty(new Rect(rect.x, rect.y + 5, 200, rect.height), property, string.Empty, false);

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;
    }


    private void DrawFloatProperty(MaterialProperty property, string label)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        //EditorGUIUtility.labelWidth = 160;
        Rect rect = EditorGUILayout.GetControlRect(false, 20);
        Rect pos0 = new Rect(rect.x, rect.y, 225, 18);

        //EditorGUIUtility.fieldWidth = 60;
        FloatProperty(pos0, property, label);

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;
    }


    private void DrawRangeProperty(MaterialProperty property, string label)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        Rect rect = EditorGUILayout.GetControlRect(false, 16);
        Rect pos0 = new Rect(rect.x + 15, rect.y, rect.width, rect.height);

        GUI.Label(pos0, label);
        pos0.x += 100;
        pos0.width -= 115;
        RangeProperty(pos0, property, string.Empty);

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;
    }


    private void DrawVectorProperty(MaterialProperty property, string label)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        EditorGUIUtility.labelWidth = 160;
        Rect rect = EditorGUILayout.GetControlRect(false, 20);
        Rect pos0 = new Rect(rect.x + 15, rect.y + 2, rect.width - 120, 18);
        Rect pos1 = new Rect(175, rect.y - 14, rect.width - 160, 18);

        GUI.Label(pos0, label);     
        VectorProperty(pos1, property, "");

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;
    }


    // Need to finish implementing this function
    private void DrawVectorProperty(MaterialProperty property, string label, int floatCount)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        EditorGUIUtility.labelWidth = 160;
        Rect rect = EditorGUILayout.GetControlRect(false, 20);
        Rect pos0 = new Rect(rect.x + 15, rect.y + 2, rect.width - 120, 18);
        Rect pos1 = new Rect(175, rect.y - 14, rect.width - 160, 18);

        GUI.Label(pos0, label);
        VectorProperty(pos1, property, "");

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;
    }


    private void Draw2DBoundsProperty(MaterialProperty property, string label)
    {
        float old_LabelWidth = EditorGUIUtility.labelWidth;
        float old_FieldWidth = EditorGUIUtility.fieldWidth;

        //EditorGUIUtility.labelWidth = 100;
        Rect rect = EditorGUILayout.GetControlRect(false, 22);
        Rect pos0 = new Rect(rect.x + 15, rect.y + 2, rect.width - 15, 18);
        //Rect pos1 = new Rect(175, rect.y - 14, rect.width - 160, 18);

        GUI.Label(pos0, label);
        EditorGUIUtility.labelWidth = 30;

        float width = (pos0.width - 15) / 5;      
        pos0.x += old_LabelWidth - 30;
        
        Vector4 vec = property.vectorValue;
        pos0.width = width;
        vec.x = EditorGUI.FloatField(pos0, "X", vec.x);
        
        pos0.x += width - 14;
        vec.y = EditorGUI.FloatField(pos0, "Y", vec.y);

        pos0.x += width - 14;
        vec.z = EditorGUI.FloatField(pos0, "W", vec.z);
        
        pos0.x += width - 14;
        vec.w = EditorGUI.FloatField(pos0, "H", vec.w);

        pos0.x = rect.width - 11;
        pos0.width = 25;

        property.vectorValue = vec;

        if (GUI.Button(pos0, "X"))
        {
            Renderer _renderer = Selection.activeGameObject.GetComponent<Renderer>();
            if (_renderer != null)
            {
                property.vectorValue = new Vector4(0, 0, Mathf.Round(_renderer.bounds.extents.x * 1000) / 1000, Mathf.Round(_renderer.bounds.extents.y * 1000) / 1000);
            }
        }

        EditorGUIUtility.labelWidth = old_LabelWidth;
        EditorGUIUtility.fieldWidth = old_FieldWidth;
    }


    // Function to set keyword for each selected material.
    private void SetKeyword(bool state, string keyword)
    {
        Undo.RecordObjects(targets, "Keyword State Change");

        for (int i = 0; i < targets.Length; i++)
        {            
            Material mat = targets[i] as Material;

            if (state)
            {                         
                switch (keyword)
                {
                    case "UNDERLAY_ON":
                        mat.EnableKeyword("UNDERLAY_ON");
                        mat.DisableKeyword("UNDERLAY_INNER");
                        break;
                    case "UNDERLAY_INNER":
                        mat.EnableKeyword("UNDERLAY_INNER");
                        mat.DisableKeyword("UNDERLAY_ON");
                        break;
                    default:
                        mat.EnableKeyword(keyword);
                        break;
                }              
            }
            else
            {
                switch (keyword)
                {
                    case "UNDERLAY_ON":
                        mat.DisableKeyword("UNDERLAY_ON");
                        mat.DisableKeyword("UNDERLAY_INNER");
                        break;
                    case "UNDERLAY_INNER":
                        mat.DisableKeyword("UNDERLAY_INNER");
                        mat.DisableKeyword("UNDERLAY_ON");
                        break;
                    default:
                        mat.DisableKeyword(keyword);
                        break;
                }                                   
            }             
        }
    }


    private void SetUnderlayKeywords()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            Material mat = targets[i] as Material;

            if (m_underlaySelection == Underlay_Types.Normal)
            {
                mat.EnableKeyword("UNDERLAY_ON");
                mat.DisableKeyword("UNDERLAY_INNER");
            }
            else if (m_underlaySelection == Underlay_Types.Inner)
            {
                mat.EnableKeyword("UNDERLAY_INNER");
                mat.DisableKeyword("UNDERLAY_ON");
            }
        }
    }


    private void SetMaskKeywords(Mask_Type mask)
    {
        for (int i = 0; i < targets.Length; i++)
        {
            Material mat = targets[i] as Material;

            switch (mask)
            {
                case Mask_Type.MaskHard:
                    mat.EnableKeyword("MASK_HARD");
                    mat.DisableKeyword("MASK_SOFT");
                    mat.DisableKeyword("MASK_OFF");
                    
                    //if (m_textMeshPro != null)
                    //    m_textMeshPro.maskType = MaskingTypes.MaskHard;
                    //else if (m_textMeshProUGUI != null)
                        //m_textMeshProUGUI.maskType = MaskingTypes.MaskHard;
                    
                        break;
                case Mask_Type.MaskSoft:
                    mat.EnableKeyword("MASK_SOFT");
                    mat.DisableKeyword("MASK_HARD");
                    mat.DisableKeyword("MASK_OFF");

                    //if (m_textMeshPro != null)
                    //    m_textMeshPro.maskType = MaskingTypes.MaskSoft;
                    //else if (m_textMeshProUGUI != null)
                    //    m_textMeshProUGUI.maskType = MaskingTypes.MaskSoft;

                    break;
                case Mask_Type.MaskOff:
                    mat.EnableKeyword("MASK_OFF");
                    mat.DisableKeyword("MASK_HARD");
                    mat.DisableKeyword("MASK_SOFT");

                    //if (m_textMeshPro != null)
                    //    m_textMeshPro.maskType = MaskingTypes.MaskOff;
                    //else if (m_textMeshProUGUI != null)
                    //    m_textMeshProUGUI.maskType = MaskingTypes.MaskOff;

                    break;
            }
        }
    }


    // Need to get material properties every update.
    void ReadMaterialProperties()
    {
        Object[] target_Materials = this.targets;


        m_faceColor = GetMaterialProperty(target_Materials, "_FaceColor");
        m_faceTex = GetMaterialProperty(target_Materials, "_FaceTex");
        m_faceUVSpeedX = GetMaterialProperty(target_Materials, "_FaceUVSpeedX");
        m_faceUVSpeedY = GetMaterialProperty(target_Materials, "_FaceUVSpeedY");
        m_faceDilate = GetMaterialProperty(target_Materials, "_FaceDilate");
        m_faceShininess = GetMaterialProperty(target_Materials, "_FaceShininess");

        // Border Properties
        m_outlineColor = GetMaterialProperty(target_Materials, "_OutlineColor");
        m_outlineThickness = GetMaterialProperty(target_Materials, "_OutlineWidth");
        m_outlineSoftness = GetMaterialProperty(target_Materials, "_OutlineSoftness");
        m_outlineTex = GetMaterialProperty(target_Materials, "_OutlineTex");
        m_outlineUVSpeedX = GetMaterialProperty(target_Materials, "_OutlineUVSpeedX");
        m_outlineUVSpeedY = GetMaterialProperty(target_Materials, "_OutlineUVSpeedY");
        m_outlineShininess = GetMaterialProperty(target_Materials, "_OutlineShininess");


        // Underlay Properties
        m_underlayColor = GetMaterialProperty(target_Materials, "_UnderlayColor");
        m_underlayOffsetX = GetMaterialProperty(target_Materials, "_UnderlayOffsetX");
        m_underlayOffsetY = GetMaterialProperty(target_Materials, "_UnderlayOffsetY");
        m_underlayDilate = GetMaterialProperty(target_Materials, "_UnderlayDilate");
        m_underlaySoftness = GetMaterialProperty(target_Materials, "_UnderlaySoftness");


        // Normal Map Options
        m_bumpMap = GetMaterialProperty(target_Materials, "_BumpMap");
        m_bumpFace = GetMaterialProperty(target_Materials, "_BumpFace");
        m_bumpOutline = GetMaterialProperty(target_Materials, "_BumpOutline");

        // Used by Unlit SDF Shader 
        //m_edgeSharpness = GetMaterialProperty(target_Materials, "_Edge");

        // Material Properties for Beveling Options
        m_bevel = GetMaterialProperty(target_Materials, "_Bevel");
        m_bevelOffset = GetMaterialProperty(target_Materials, "_BevelOffset");
        m_bevelWidth = GetMaterialProperty(target_Materials, "_BevelWidth");
        m_bevelClamp = GetMaterialProperty(target_Materials, "_BevelClamp");
        m_bevelRoundness = GetMaterialProperty(target_Materials, "_BevelRoundness");

        m_specColor = GetMaterialProperty(target_Materials, "_SpecColor"); // Used by Surface Shader
        
        // Bevel properties for Basic Shader & Hidden for Surface Shader
        m_lightAngle = GetMaterialProperty(target_Materials, "_LightAngle");
        m_specularColor = GetMaterialProperty(target_Materials, "_SpecularColor");
        m_specularPower = GetMaterialProperty(target_Materials, "_SpecularPower");
        m_reflectivity = GetMaterialProperty(target_Materials, "_Reflectivity");
        m_diffuse = GetMaterialProperty(target_Materials, "_Diffuse");
        m_ambientLight = GetMaterialProperty(target_Materials, "_Ambient");



        // Material Properties for Glow Options
        m_glowColor = GetMaterialProperty(target_Materials, "_GlowColor");
        m_glowOffset = GetMaterialProperty(target_Materials, "_GlowOffset");
        m_glowInner = GetMaterialProperty(target_Materials, "_GlowInner");
        m_glowOuter = GetMaterialProperty(target_Materials, "_GlowOuter");
        m_glowPower = GetMaterialProperty(target_Materials, "_GlowPower");

        // Cube Map Options
        m_reflectFaceColor = GetMaterialProperty(target_Materials, "_ReflectFaceColor");
        m_reflectOutlineColor = GetMaterialProperty(target_Materials, "_ReflectOutlineColor");
        m_reflectTex = GetMaterialProperty(target_Materials, "_Cube");
        m_reflectRotation = GetMaterialProperty(target_Materials, "_EnvMatrixRotation");
        //m_envTiltX = GetMaterialProperty(target_Materials, "_EnvTiltX");
        //m_envTiltY = GetMaterialProperty(target_Materials, "_EnvTiltY");
        //m_envMatrix = GetMaterialProperty(target_Materials, "_EnvMatrix");
        // Properties specific to Surface Shader
        //m_shininess = GetMaterialProperty(target_Materials, "_Shininess");


        // Hidden Properties
        m_mainTex = GetMaterialProperty(target_Materials, "_MainTex");
        m_texSampleWidth = GetMaterialProperty(target_Materials, "_TextureWidth");
        m_texSampleHeight = GetMaterialProperty(target_Materials, "_TextureHeight");
        m_gradientScale = GetMaterialProperty(target_Materials, "_GradientScale");
        m_PerspectiveFilter = GetMaterialProperty(target_Materials, "_PerspectiveFilter");
        m_scaleX = GetMaterialProperty(target_Materials, "_ScaleX");
        m_scaleY = GetMaterialProperty(target_Materials, "_ScaleY");


        m_vertexOffsetX = GetMaterialProperty(target_Materials, "_VertexOffsetX");
        m_vertexOffsetY = GetMaterialProperty(target_Materials, "_VertexOffsetY");
        m_maskCoord = GetMaterialProperty(target_Materials, "_MaskCoord");
        m_maskSoftnessX = GetMaterialProperty(target_Materials, "_MaskSoftnessX");
        m_maskSoftnessY = GetMaterialProperty(target_Materials, "_MaskSoftnessY");

        // Stencil
        m_stencilID = GetMaterialProperty(target_Materials, "_Stencil");
        m_stencilComp = GetMaterialProperty(target_Materials, "_StencilComp");
        m_stencilOp = GetMaterialProperty(target_Materials, "_StencilOp");

        
        //m_weightNormal = GetMaterialProperty(target_Materials, "_WeightNormal");
        //m_weightBold = GetMaterialProperty(target_Materials, "_WeightBold");
      
        m_shaderFlags = GetMaterialProperty(target_Materials, "_ShaderFlags");
        m_scaleRatio_A = GetMaterialProperty(target_Materials, "_ScaleRatioA");
        m_scaleRatio_B = GetMaterialProperty(target_Materials, "_ScaleRatioB");
        m_scaleRatio_C = GetMaterialProperty(target_Materials, "_ScaleRatioC");
        //m_fadeOut = GetMaterialProperty(target_Materials, "_Fadeout");
       
    }
}
