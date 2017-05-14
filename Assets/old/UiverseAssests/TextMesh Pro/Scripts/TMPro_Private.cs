// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#pragma warning disable 0414 // Disabled a few warnings related to serialized variables not used in this script but used in the editor.

namespace TMPro
{  

    public partial class TextMeshPro
    {

        [SerializeField]
        private string m_text;

        [SerializeField]
        private TextMeshProFont m_fontAsset;

        private Material m_fontMaterial;

        private Material m_sharedMaterial;
 
        [SerializeField]
        private FontStyles m_fontStyle = FontStyles.Normal;
        private FontStyles m_style = FontStyles.Normal;
              
        [SerializeField]
        private bool m_isOverlay = false;

        // Convert from using Color32 to Color to make it possible to animate vertex colors using the Animation Editor.
#if UNITY_4_6 || UNITY_5
        [UnityEngine.Serialization.FormerlySerializedAs("m_fontColor")]
#endif
        [SerializeField]
        private Color32 m_fontColor32 = Color.white;
        
        [SerializeField]
        private Color m_fontColor = Color.white;

        [SerializeField]
        private VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

		[SerializeField]
		private bool m_enableVertexGradient;


        [SerializeField]
        private Color32 m_faceColor = Color.white;

        [SerializeField]
        private Color32 m_outlineColor = Color.black;

        private float m_outlineWidth = 0.0f;


        [SerializeField]
        private float m_fontSize = 36; // Font Size
        [SerializeField]
        private float m_fontSizeMin = 0; // Text Auto Sizing Min Font Size.
        [SerializeField]
        private float m_fontSizeMax = 0; // Text Auto Sizing Max Font Size.
        [SerializeField]
        private float m_fontSizeBase = 36;
        [SerializeField]
        private float m_charSpacingMax = 0; // Text Auto Sizing Max Character spacing reduction.
        [SerializeField]
        private float m_lineSpacingMax = 0; // Text Auto Sizing Max Line spacing reduction.
		

        private float m_currentFontSize; // Temporary Font Size affected by tags

        [SerializeField]
        private float m_characterSpacing = 0;
        private float m_cSpacing = 0; // Holds the additional spacing resulting from using the <cspace=xx.x> tag.
        private float m_monoSpacing = 0;

      

        [SerializeField]
        private Vector4 m_textRectangle;

        [SerializeField]
        private float m_lineSpacing = 0;
        private float m_lineSpacingDelta = 0; // Used with Text Auto Sizing feature
        private float m_lineHeight;      

        [SerializeField]
        private float m_paragraphSpacing = 0;

      
        // This is now obsolete as word wrappng is now controlled by the Text Container's Width and Margins.
        //[UnityEngine.Serialization.FormerlySerializedAs("m_lineLength")]
        [SerializeField]
        private float m_lineLength;

        // This is now obsolete as word wrappng is now controlled by the Text Container's Width and Margins.
        //[UnityEngine.Serialization.FormerlySerializedAs("m_anchor")]
        [SerializeField]
        private TMP_Compatibility.AnchorPositions m_anchor = TMP_Compatibility.AnchorPositions.TopLeft;

#if UNITY_4_6 || UNITY_5
        [UnityEngine.Serialization.FormerlySerializedAs("m_lineJustification")]
#endif
        [SerializeField]
        private TextAlignmentOptions m_textAlignment = TextAlignmentOptions.TopLeft;
        private TextAlignmentOptions m_lineJustification;

        [SerializeField]
        private bool m_enableKerning = false;

        private bool m_anchorDampening = false;
        private float m_baseDampeningWidth;

        [SerializeField]
        private bool m_overrideHtmlColors = false;

        [SerializeField]
        private bool m_enableExtraPadding = false;
        [SerializeField]
        private bool checkPaddingRequired;

        [SerializeField]
        private bool m_enableWordWrapping = false;
        private bool m_isCharacterWrappingEnabled = false;

        [SerializeField]
        private TextOverflowModes m_overflowMode = TextOverflowModes.Overflow;

        [SerializeField]
        private float m_wordWrappingRatios = 0.4f; // Controls word wrapping ratios between word or characters.

        [SerializeField]
        private TextureMappingOptions m_horizontalMapping = TextureMappingOptions.Character;

        [SerializeField]
        private TextureMappingOptions m_verticalMapping = TextureMappingOptions.Character;
        
        [SerializeField]
        private Vector2 m_uvOffset = Vector2.zero; // Used to offset UV on Texturing
        
        [SerializeField]
        private float m_uvLineOffset = 0.0f; // Used for UV line offset per line

        //[SerializeField]
        //private bool isAffectingWordWrapping = false; // Used internally to control which properties affect word wrapping.

        [SerializeField]
        private bool isInputParsingRequired = false; // Used to determine if the input text needs to be reparsed.

        [SerializeField]
        private bool havePropertiesChanged;  // Used to track when properties of the text object have changed.

        [SerializeField]
        private bool hasFontAssetChanged = false; // Used to track when font properties have changed.

        [SerializeField]
        private bool m_isRichText = true; // Used to enable or disable Rich Text.


        private enum TextInputSources { Text = 0, SetText = 1, SetCharArray = 2 };
        [SerializeField]
        private TextInputSources m_inputSource;
        private string old_text; // Used by SetText to determine if the text has changed.
        private float old_arg0, old_arg1, old_arg2; // Used by SetText to determine if the args have changed.

        private int m_fontIndex;

        private float m_fontScale; // Scaling of the font based on Atlas true Font Size and Rendered Font Size.  
        private bool m_isRecalculateScaleRequired = false;

        private Vector3 m_previousLossyScale; // Used for Tracking lossy scale changes in the transform;
        private float m_xAdvance; // Tracks x advancement from character to character.
        private float m_indent = 0;
        private float m_maxXAdvance; // Tracks the MaxXAdvance while considering linefeeds.
        //private bool m_isCheckingTextLength = false;
        //private float m_textLength;
        //private int[] m_text_buffer = new int[8];

        //private float max_LineWrapLength = 1250;

        private Vector3 m_anchorOffset; // The amount of offset to be applied to the vertices. 


        private TMP_TextInfo m_textInfo; // Class which holds information about the Text object such as characters, lines, mesh data as well as metrics.
        //private TMPro_CharacterInfo[] m_characters_Info; // Data structre that contains information about the text object and characters contained in it.
        //private TMPro_TextMetrics m_textMetrics;
     

        private char[] m_htmlTag = new char[128]; // Maximum length of rich text tag. This is pre-allocated to avoid GC.


        [SerializeField]
        private Renderer m_renderer;
        private MeshFilter m_meshFilter;
        private Mesh m_mesh;
        private Transform m_transform;

        // Fields used for vertex colors
        private Color32 m_htmlColor = new Color(255, 255, 255, 128);
        private Color32[] m_colorStack = new Color32[32];
        private int m_colorStackIndex = 0;
     
        private float m_tabSpacing = 0;
        private float m_spacing = 0;
        private Vector2[] m_spacePositions = new Vector2[8]; // Not fully implemented yet ... will be used to track all the location of inserted spaced.

        private float m_baselineOffset; // Used for superscript and subscript.
        private float m_padding = 0; // Holds the amount of padding required to display the mesh correctly as a result of dilation, outline thickness, softness and similar material properties.
        private Vector4 m_alignmentPadding; // Holds the amount of padding required to account for Outline Width and Dilation with regards to text alignment.
        private bool m_isUsingBold = false; // Used to ensure GetPadding & Ratios take into consideration bold characters.

        private Vector2 k_InfinityVector = new Vector2(Mathf.Infinity, Mathf.Infinity);

        private bool m_isFirstAllocation; // Flag to determine if this is the first allocation of the buffers.
        private int m_max_characters = 8; // Determines the initial allocation and size of the character array / buffer.
        private int m_max_numberOfLines = 4; // Determines the initial allocation and maximum number of lines of text. 

        private int[] m_char_buffer; // This array holds the characters to be processed by GenerateMesh();
        private char[] m_input_CharArray = new char[256]; // This array hold the characters from the SetText();
        private int m_charArray_Length = 0;
        private List<char> m_VisibleCharacters = new List<char>();

             
        private readonly float[] k_Power = { 5e-1f, 5e-2f, 5e-3f, 5e-4f, 5e-5f, 5e-6f, 5e-7f, 5e-8f, 5e-9f, 5e-10f }; // Used by FormatText to enable rounding and avoid using Mathf.Pow.

        private GlyphInfo m_cached_GlyphInfo; // Glyph / Character information is cached into this variable which is faster than having to fetch from the Dictionary multiple times.
        private GlyphInfo m_cached_Underline_GlyphInfo; // Same as above but for the underline character which is used for Underline.

        // Globale Variables used in conjunction with saving the state of words or lines.
        private WordWrapState m_SavedWordWrapState = new WordWrapState(); // Struct which holds various states / variables used in conjunction with word wrapping.
        private WordWrapState m_SavedLineState = new WordWrapState();

        

        private int m_characterCount;
        private int m_visibleCharacterCount;
        private int m_firstVisibleCharacterOfLine;
        private int m_lastVisibleCharacterOfLine;
        private int m_lineNumber;
        private int m_pageNumber;
        private float m_maxAscender;
        private float m_maxDescender;
        private float m_maxFontScale;
        private float m_lineOffset;
        private Extents m_meshExtents;

        private float m_preferredWidth;
        private float m_preferredHeight;

        // Mesh Declaration 
        private Vector3[] m_vertices;
        private Vector3[] m_normals;
        private Vector4[] m_tangents;
        private Vector2[] m_uvs;
        private Vector2[] m_uv2s;
        private Color32[] m_vertColors;
        private int[] m_triangles;

        //private Camera m_sceneCamera;
        private Bounds m_default_bounds = new Bounds(Vector3.zero, new Vector3(1000, 1000, 0));

        [SerializeField]
        private bool m_ignoreCulling = true; // Not implemented yet.
        [SerializeField]
        private bool m_isOrthographic = false;

        [SerializeField]
        private bool m_isCullingEnabled = false;

        //[SerializeField]
        private int m_sortingLayerID;
        //[SerializeField]
        //private int m_sortingOrder;


        //Special Cases
        private int m_maxVisibleCharacters = -1;
        private int m_maxVisibleLines = -1;
        [SerializeField]
        private int m_pageToDisplay = 0;
        private bool m_isNewPage = false;
        private bool m_isTextTruncated;

      
        // Multi Material & Font Handling
        // Forced to use a class until 4.5 as Structs do not serialize. 
        //private class TriangleList
        //{
        //    public int[] triangleIndex;
        //}

        //private TriangleList[] m_triangleListArray = new TriangleList[16];
        [SerializeField]
        private TextMeshProFont[] m_fontAssetArray;
        //[SerializeField]
        private List<Material> m_sharedMaterials = new List<Material>(16);
        private int m_selectedFontAsset;

        // MASKING RELATED PROPERTIES

        MaterialPropertyBlock m_maskingPropertyBlock;
        //[SerializeField]
        private bool m_isMaskingEnabled;
        private bool isMaskUpdateRequired;
        private bool m_isMaterialBlockSet;

        [SerializeField]
        private MaskingTypes m_maskType;

        /*
        [SerializeField]
        private MaskingOffsetMode m_maskOffsetMode;
        [SerializeField]
        private Vector4 m_maskOffset;
        [SerializeField]
        private Vector2 m_maskSoftness;
        [SerializeField]
        private Vector2 m_vertexOffset;
        */

        private Matrix4x4 m_EnvMapMatrix = new Matrix4x4();


        // FLAGS  
        private TextRenderFlags m_renderMode = TextRenderFlags.Render;

        // ADVANCED LAYOUT COMPONENT ** Still work in progress
        //private TMPro_AdvancedLayout m_advancedLayoutComponent;
        //private bool m_isAdvanceLayoutComponentPresent;        
        //private TMPro_MeshInfo m_meshInfo;


        // Text Container / RectTransform Component
        [SerializeField]
        private bool m_isNewTextObject;
        private TextContainer m_textContainer;
        private float m_marginWidth;
        [SerializeField]
        private bool m_enableAutoSizing;
        private float m_maxFontSize; // Used in conjunction with Auto-sizing
		private float m_minFontSize; // Used in conjunction with Auto-sizing

        // ** Still needs to be implemented **
        //private Camera managerCamera;
        //private TMPro_UpdateManager m_updateManager;
        //private bool isAlreadyScheduled;

        // DEBUG Variables
        private System.Diagnostics.Stopwatch m_StopWatch;
        private bool isDebugOutputDone;
        private int m_recursiveCount = 0;
        private int loopCountA;
        private int loopCountB;
        private int loopCountC;
        private int loopCountD;
        private int loopCountE;

        private GameObject m_prefabParent;

        void Awake()
        {
            //Debug.Log("Awake() called on Object ID " + GetInstanceID());          
            
            // Code to handle Compatibility related to the switch from Color32 to Color
            if (m_fontColor == Color.white && m_fontColor32 != Color.white)
            {
                Debug.LogWarning("Converting Vertex Colors from Color32 to Color.");
                m_fontColor = m_fontColor32;
            } 
         
            m_textContainer = GetComponent<TextContainer>();
            if (m_textContainer == null)
                m_textContainer = gameObject.AddComponent<TextContainer>();

           
            // Cache Reference to the Mesh Renderer.           
            m_renderer = GetComponent<Renderer>();
            if (m_renderer == null)
                m_renderer = gameObject.AddComponent<Renderer>();

           
            // Cache Reference to the transform;     
            m_transform = gameObject.transform;           

            // Cache a reference to the Mesh Filter.
            m_meshFilter = GetComponent<MeshFilter>();
            if (m_meshFilter == null)
                m_meshFilter = gameObject.AddComponent<MeshFilter>();

                   
            // Cache a reference to our mesh.
            if (m_mesh == null)
            {                              
                //Debug.Log("Creating new mesh.");
                m_mesh = new Mesh();
                m_mesh.hideFlags = HideFlags.HideAndDontSave;

                m_meshFilter.mesh = m_mesh;                
                //m_mesh.bounds = new Bounds(transform.position, new Vector3(1000, 1000, 0));               
            }           
            m_meshFilter.hideFlags = HideFlags.HideInInspector;           
         
            // Load the font asset and assign material to renderer.
            LoadFontAsset();

            // Allocate our initial buffers.          
            m_char_buffer = new int[m_max_characters];
            //m_parsedCharacters = new char[m_max_characters];
            //m_lineExtents = new Mesh_Extents[m_max_numberOfLines];
            m_cached_GlyphInfo = new GlyphInfo();
            m_vertices = new Vector3[0]; // 
            m_isFirstAllocation = true;          
            
            m_textInfo = new TMP_TextInfo();        
            m_textInfo.wordInfo = new List<TMP_WordInfo>();
            m_textInfo.lineInfo = new TMP_LineInfo[m_max_numberOfLines];
            m_textInfo.pageInfo = new TMP_PageInfo[16];
            m_textInfo.meshInfo = new TMP_MeshInfo();  
            
            m_fontAssetArray = new TextMeshProFont[16];
         

            // Check if we have a font asset assigned. Return if we don't because no one likes to see purple squares on screen.
            if (m_fontAsset == null)
            {
                Debug.LogWarning("Please assign a Font Asset to this " + transform.name + " gameobject."); 
                return;
            }

            // Set Defaults for Font Auto-sizing
            if (m_fontSizeMin == 0) m_fontSizeMin = m_fontSize / 2;
            if (m_fontSizeMax == 0) m_fontSizeMax = m_fontSize * 2;

            //// Set flags to cause ensure our text is parsed and text redrawn. 
            isInputParsingRequired = true;
            havePropertiesChanged = true;
        
            //m_isNewTextObject = true;
            ForceMeshUpdate(); // Added to force OnWillRenderObject() to be called in case object is not visible so we get initial bounds for the mesh.
            ///* ScheduleUpdate();         
        }


     
        void OnEnable()
        {
            //Debug.Log("***** OnEnable() called on object ID " + GetInstanceID() + ". *****"); // called. Renderer.MeshFilter ID " + m_renderer.GetComponent<MeshFilter>().sharedMesh.GetInstanceID() + "  Mesh ID " + m_mesh.GetInstanceID() + "  MeshFilter ID " + m_meshFilter.GetInstanceID()); //has been called. HavePropertiesChanged = " + havePropertiesChanged); // has been called on Object ID:" + gameObject.GetInstanceID());      
            
            if (m_meshFilter.sharedMesh == null)
                m_meshFilter.mesh = m_mesh;
            
#if UNITY_EDITOR
            // Register Callbacks for various events.
            TMPro_EventManager.MATERIAL_PROPERTY_EVENT += ON_MATERIAL_PROPERTY_CHANGED;
            TMPro_EventManager.FONT_PROPERTY_EVENT += ON_FONT_PROPERTY_CHANGED;
            TMPro_EventManager.TEXTMESHPRO_PROPERTY_EVENT += ON_TEXTMESHPRO_PROPERTY_CHANGED;
            TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT += ON_DRAG_AND_DROP_MATERIAL;
#endif

            TMPro_EventManager.OnPreRenderObject_Event += OnPreRenderObject;
            // Since Structures don't serialize in Unity 4.3 or below, we must re-allocate this array.          
            //m_textInfo.lineInfo = new LineInfo[m_max_numberOfLines];

            //if (m_meshInfo == null)
            //    Debug.Log("Mesh Info is null.");
        }


       void OnDisable()
        {
            //Debug.Log("***** OnDisable() called on object ID " + GetInstanceID() + ". *****"); //+ m_renderer.GetComponent<MeshFilter>().sharedMesh.GetInstanceID() + "  Mesh ID " + m_mesh.GetInstanceID() + "  MeshFilter ID " + m_meshFilter.GetInstanceID()); //has been called. HavePropertiesChanged = " + havePropertiesChanged); // has been called on Object ID:" + gameObject.GetInstanceID());      
            //if (m_meshFilter.sharedMesh != null)
            //    Debug.Log("OnDisable() called. We have a valid mesh with ID " + m_meshFilter.sharedMesh.GetInstanceID());
            //else
            //    Debug.Log("OnDisable() called. We DO NOT have a valid mesh");


#if UNITY_EDITOR
            // Un-register the event this object was listening to
            TMPro_EventManager.MATERIAL_PROPERTY_EVENT -= ON_MATERIAL_PROPERTY_CHANGED;
            TMPro_EventManager.FONT_PROPERTY_EVENT -= ON_FONT_PROPERTY_CHANGED;
            TMPro_EventManager.TEXTMESHPRO_PROPERTY_EVENT -= ON_TEXTMESHPRO_PROPERTY_CHANGED;
            TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT -= ON_DRAG_AND_DROP_MATERIAL;
#endif

            TMPro_EventManager.OnPreRenderObject_Event -= OnPreRenderObject;
        }


        void OnDestroy()
        {
            //Debug.Log("***** OnDestroy() called on object ID " + GetInstanceID() + ". *****");
            // Destroy the mesh if we have one.
            if (m_mesh != null)
            {
                DestroyImmediate(m_mesh);
            }
        }


        void Reset()
        {
            //Debug.Log("Reset() has been called.");
            
            if (m_mesh != null)
                DestroyImmediate(m_mesh);

            Awake();
            //LoadFontAsset();
            //isInputParsingRequired = true;
            //havePropertiesChanged = true;
        }


#if UNITY_EDITOR
        void OnValidate()
        {
            // Additional Properties could be added to sync up Serialized Properties & Properties.
            //Debug.Log("TextMeshPro OnValidate() called. Renderer.MeshFilter ID " + m_renderer.GetComponent<MeshFilter>().GetInstanceID() + "  Mesh ID " + m_mesh.GetInstanceID() + "  MeshFilter ID " + m_meshFilter.GetInstanceID()); //has been called. HavePropertiesChanged = " + havePropertiesChanged); // has been called on Object ID:" + gameObject.GetInstanceID());      
            //Debug.Log("TextMeshPro OnValidate() called. Mesh ID " + m_mesh.GetInstanceID() + "  MeshFilter ID " + m_meshFilter.GetInstanceID()); //has been called. HavePropertiesChanged = " + havePropertiesChanged); // has been called on Object ID:" + gameObject.GetInstanceID());      

            if (hasFontAssetChanged) { LoadFontAsset(); hasFontAssetChanged = false; }
            font = m_fontAsset;
            text = m_text;

            checkPaddingRequired = true;
        }



        // Event received when custom material editor properties are changed.
        void ON_MATERIAL_PROPERTY_CHANGED(bool isChanged, Material mat)
        {
            //Debug.Log("ON_MATERIAL_PROPERTY_CHANGED event received. Targeted Material is: " + mat.name + "  m_sharedMaterial: " + m_sharedMaterial.name + "  m_renderer.sharedMaterial: " + m_renderer.sharedMaterial);         

            if (m_renderer.sharedMaterial == null)
            {
                if (m_fontAsset != null)
                {
                    m_renderer.sharedMaterial = m_fontAsset.material;
                    Debug.LogWarning("No Material was assigned to " + name + ". " + m_fontAsset.material.name + " was assigned.");
                }
                else
                    Debug.LogWarning("No Font Asset assigned to " + name + ". Please assign a Font Asset.");
            }

            if (m_fontAsset.atlas.GetInstanceID() != m_renderer.sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
            {
                m_renderer.sharedMaterial = m_sharedMaterial;
                //m_renderer.sharedMaterial = m_fontAsset.material;
                Debug.LogWarning("Font Asset Atlas doesn't match the Atlas in the newly assigned material. Select a matching material or a different font asset.");
            }

            if (m_renderer.sharedMaterial != m_sharedMaterial) //    || m_renderer.sharedMaterials.Contains(mat))
            {
                //Debug.Log("ON_MATERIAL_PROPERTY_CHANGED Called on Target ID: " + GetInstanceID() + ". Previous Material:" + m_sharedMaterial + "  New Material:" + m_renderer.sharedMaterial); // on Object ID:" + GetInstanceID() + ". m_sharedMaterial: " + m_sharedMaterial.name + "  m_renderer.sharedMaterial: " + m_renderer.sharedMaterial.name);         
                m_sharedMaterial = m_renderer.sharedMaterial;
            }

            m_padding =  ShaderUtilities.GetPadding(m_renderer.sharedMaterials, m_enableExtraPadding, m_isUsingBold);
            m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
            m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
            UpdateMask();
            UpdateEnvMapMatrix();
            havePropertiesChanged = true;          
            /* ScheduleUpdate(); */
        }


        // Event received when font asset properties are changed in Font Inspector
        void ON_FONT_PROPERTY_CHANGED(bool isChanged, TextMeshProFont font)
        {
            if (font == m_fontAsset)
            {
                //Debug.Log("ON_FONT_PROPERTY_CHANGED event received.");
                havePropertiesChanged = true;
                hasFontAssetChanged = true;
                /* ScheduleUpdate(); */
            }
        }

     
        // Event received when UNDO / REDO Event alters the properties of the object.
        void ON_TEXTMESHPRO_PROPERTY_CHANGED(bool isChanged, TextMeshPro obj)
        {
            if (obj == this)
            {
                //Debug.Log("Undo / Redo Event Received by Object ID:" + GetInstanceID());
                havePropertiesChanged = true;
                isInputParsingRequired = true;
                /* ScheduleUpdate(); */
            }
        }


        // Event to Track Material Changed resulting from Drag-n-drop.
        void ON_DRAG_AND_DROP_MATERIAL(GameObject obj, Material currentMaterial, Material newMaterial)
        {

            //Debug.Log("Drag-n-Drop Event - Receiving Object ID " + GetInstanceID() + ". Target Object ID " + obj.GetInstanceID() + ".  New Material is " + mat.name + " with ID " + mat.GetInstanceID() + ". Base Material is " + m_baseMaterial.name + " with ID " + m_baseMaterial.GetInstanceID());


            // Check if event applies to this current object
            if (obj == gameObject || UnityEditor.PrefabUtility.GetPrefabParent(gameObject) == obj)
            {
                //Debug.Log("Assigning new Base Material " + newMaterial.name + " to replace " + currentMaterial.name);
                
                UnityEditor.Undo.RecordObject(this, "Material Assignment");
                UnityEditor.Undo.RecordObject(m_renderer, "Material Assignment");
                fontSharedMaterial = newMaterial;            
            }
        }

#endif


        //void OnPrefabUpdated(GameObject obj)
        //{
        //    Debug.Log("Prefab ID " + obj.name + " has been updated. Mesh ID " + m_mesh.GetInstanceID());
        //    //if (obj.GetInstanceID() == gameObject.GetInstanceID())
        //    //{
        //    //    if (m_meshFilter.sharedMesh.GetInstanceID() != m_mesh.GetInstanceID())
        //    //        m_mesh = m_meshFilter.sharedMesh; 
        //    //}
        //}


        // Function which loads either the default font or a newly assigned font asset. This function also assigned the appropriate material to the renderer.
        void LoadFontAsset()
        {          
            //Debug.Log("TextMeshPro LoadFontAsset() has been called."); // Current Font Asset is " + (font != null ? font.name: "Null") );
            
            ShaderUtilities.GetShaderPropertyIDs(); // Initialize & Get shader property IDs.

            if (m_fontAsset == null)  
            {
                
                //Debug.Log("Font Asset are Null. Trying to Load Default Arial SDF Font.");
                m_fontAsset = Resources.Load("Fonts & Materials/ARIAL SDF", typeof(TextMeshProFont)) as TextMeshProFont;
                if (m_fontAsset == null)
                {
                    Debug.LogWarning("The ARIAL SDF Font Asset was not found. There is no Font Asset assigned to " + gameObject.name + ".");
                    return;
                }

                if (m_fontAsset.characterDictionary == null)
                {
                    Debug.Log("Dictionary is Null!");
                }

                m_renderer.sharedMaterial = m_fontAsset.material;
                m_sharedMaterial = m_fontAsset.material;
                m_sharedMaterial.SetFloat("_CullMode", 0);
                m_sharedMaterial.SetFloat("_ZTestMode", 4);
                m_renderer.receiveShadows = false;
                m_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // true;
                // Get a Reference to the Shader
            }
            else
            {
                if (m_fontAsset.characterDictionary == null)
                {
                    //Debug.Log("Reading Font Definition and Creating Character Dictionary.");
                    m_fontAsset.ReadFontDefinition();
                }

                //Debug.Log("Font Asset name:" + font.material.name);

                // If font atlas texture doesn't match the assigned material font atlas, switch back to default material specified in the Font Asset.
                if (m_renderer.sharedMaterial == null || m_renderer.sharedMaterial.mainTexture == null || m_fontAsset.atlas.GetInstanceID() != m_renderer.sharedMaterial.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
                {
                    m_renderer.sharedMaterial = m_fontAsset.material;
                    m_sharedMaterial = m_fontAsset.material; 
                }
                else
                {
                    m_sharedMaterial = m_renderer.sharedMaterial;
                }

                //m_sharedMaterial.SetFloat("_CullMode", 0);
                m_sharedMaterial.SetFloat("_ZTestMode", 4);

                // Check if we are using the SDF Surface Shader
                if (m_sharedMaterial.passCount > 1)
                {
                    m_renderer.receiveShadows = false;
                    m_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                else
                {
                    m_renderer.receiveShadows = false;
                    m_renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; 
                }
            }

            m_padding = ShaderUtilities.GetPadding(m_renderer.sharedMaterials, m_enableExtraPadding, m_isUsingBold);
            m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
            m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
                      

            if (!m_fontAsset.characterDictionary.TryGetValue(95, out m_cached_Underline_GlyphInfo)) //95
                Debug.LogWarning("Underscore character wasn't found in the current Font Asset. No characters assigned for Underline.");

            

            m_sharedMaterials.Add(m_sharedMaterial);          
            // Hide Material Editor Component
            //m_renderer.sharedMaterial.hideFlags = HideFlags.None;
        }


        /// <summary>
        /// Function under development to utilize an Update Manager instead of normal event functions like LateUpdate() or OnWillRenderObject().
        /// </summary>
        void ScheduleUpdate()
        {
            return;
            /*
            if (!isAlreadyScheduled)
            {
                m_updateManager.ScheduleObjectForUpdate(this);
                isAlreadyScheduled = true;
            }
            */
        }


        void UpdateEnvMapMatrix()
        {
            if (!m_sharedMaterial.HasProperty(ShaderUtilities.ID_EnvMap) || m_sharedMaterial.GetTexture(ShaderUtilities.ID_EnvMap) == null)
                return;

            //Debug.Log("Updating Env Matrix...");
            Vector3 rotation = m_sharedMaterial.GetVector(ShaderUtilities.ID_EnvMatrixRotation);
            m_EnvMapMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rotation), Vector3.one);

            m_sharedMaterial.SetMatrix(ShaderUtilities.ID_EnvMatrix, m_EnvMapMatrix);
        }


        //
        void SetMask(MaskingTypes maskType)
        {
            switch(maskType)
            {
                case MaskingTypes.MaskOff:
                    m_sharedMaterial.EnableKeyword(ShaderUtilities.Keyword_MASK_OFF);
                    m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_SOFT);
                    m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_HARD);
                    break;
                case MaskingTypes.MaskSoft:
                    m_sharedMaterial.EnableKeyword(ShaderUtilities.Keyword_MASK_SOFT);
                    m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_HARD);
                    m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_OFF);
                    break;
                case MaskingTypes.MaskHard:
                    m_sharedMaterial.EnableKeyword(ShaderUtilities.Keyword_MASK_HARD);
                    m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_SOFT);
                    m_sharedMaterial.DisableKeyword(ShaderUtilities.Keyword_MASK_OFF);
                    break;
            }
        }


        // Method used to set the masking coordinates
        void SetMaskCoordinates(Vector4 coords)
        {
            m_sharedMaterial.SetVector(ShaderUtilities.ID_MaskCoord, coords);
        }

        // Method used to set the masking coordinates
        void SetMaskCoordinates(Vector4 coords, float softX, float softY)
        {
            m_sharedMaterial.SetVector(ShaderUtilities.ID_MaskCoord, coords);
            m_sharedMaterial.SetFloat(ShaderUtilities.ID_MaskSoftnessX, softX);
            m_sharedMaterial.SetFloat(ShaderUtilities.ID_MaskSoftnessY, softY);
        }



        // Enable Masking in the Shader
        void EnableMasking()
        {
            if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_MaskCoord))
            {
                m_sharedMaterial.EnableKeyword("MASK_SOFT");
                m_sharedMaterial.DisableKeyword("MASK_HARD");
                m_sharedMaterial.DisableKeyword("MASK_OFF");

                m_isMaskingEnabled = true;
                UpdateMask();
            }
        }


        // Enable Masking in the Shader
        void DisableMasking()
        {
            if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_MaskCoord))
            {
                m_sharedMaterial.EnableKeyword("MASK_OFF");
                m_sharedMaterial.DisableKeyword("MASK_HARD");
                m_sharedMaterial.DisableKeyword("MASK_SOFT");

                m_isMaskingEnabled = false;
                UpdateMask();
            }
        }


        void UpdateMask()
        {
            //Debug.Log("UpdateMask() called.");
            
            if (!m_isMaskingEnabled)
            {
                // Release Masking Material

                // Re-assign Base Material

                return;
            }
            
            if (m_isMaskingEnabled && m_fontMaterial == null)
            {
                CreateMaterialInstance();   
            }

            
            /*
            if (!m_isMaskingEnabled)
            {
                //Debug.Log("Masking is not enabled.");
                if (m_maskingPropertyBlock != null)
                {
                    m_renderer.SetPropertyBlock(null);
                    //havePropertiesChanged = true;
                }
                return;
            }
            //else
            //    Debug.Log("Updating Masking...");
            */
             
            // Compute Masking Coordinates & Softness
            float softnessX = Mathf.Min(Mathf.Min(m_textContainer.margins.x, m_textContainer.margins.z), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
            float softnessY = Mathf.Min(Mathf.Min(m_textContainer.margins.y, m_textContainer.margins.w), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessY));

            softnessX = softnessX > 0 ? softnessX : 0;
            softnessY = softnessY > 0 ? softnessY : 0;
           
            float width = (m_textContainer.width - Mathf.Max(m_textContainer.margins.x, 0) - Mathf.Max(m_textContainer.margins.z, 0)) / 2 + softnessX;
            float height =  (m_textContainer.height - Mathf.Max(m_textContainer.margins.y, 0) - Mathf.Max(m_textContainer.margins.w, 0)) / 2 + softnessY;
          
            Vector2 center = new Vector2((0.5f - m_textContainer.pivot.x) * m_textContainer.width + (Mathf.Max(m_textContainer.margins.x, 0) - Mathf.Max(m_textContainer.margins.z, 0)) / 2, (0.5f - m_textContainer.pivot.y) * m_textContainer.height + (- Mathf.Max(m_textContainer.margins.y, 0) + Mathf.Max(m_textContainer.margins.w, 0)) / 2);                           
            Vector4 mask = new Vector4(center.x, center.y, width, height);


            m_fontMaterial.SetVector(ShaderUtilities.ID_MaskCoord, mask);
            m_fontMaterial.SetFloat(ShaderUtilities.ID_MaskSoftnessX, softnessX);
            m_fontMaterial.SetFloat(ShaderUtilities.ID_MaskSoftnessY, softnessY);
            /*                     
            if(m_maskingPropertyBlock == null)
            {                
                m_maskingPropertyBlock = new MaterialPropertyBlock();
         
                //m_maskingPropertyBlock.AddFloat(ShaderUtilities.ID_VertexOffsetX,  m_sharedMaterial.GetFloat(ShaderUtilities.ID_VertexOffsetX));
                //m_maskingPropertyBlock.AddFloat(ShaderUtilities.ID_VertexOffsetY,  m_sharedMaterial.GetFloat(ShaderUtilities.ID_VertexOffsetY));
                //Debug.Log("Creating new MaterialPropertyBlock.");            
            }

            //Debug.Log("Updating Material Property Block.");
            //m_maskingPropertyBlock.Clear();
            m_maskingPropertyBlock.AddFloat(ShaderUtilities.ID_MaskID, m_renderer.GetInstanceID());       
            m_maskingPropertyBlock.AddVector(ShaderUtilities.ID_MaskCoord, mask);
            m_maskingPropertyBlock.AddFloat(ShaderUtilities.ID_MaskSoftnessX, softnessX);
            m_maskingPropertyBlock.AddFloat(ShaderUtilities.ID_MaskSoftnessY, softnessY);
           
            m_renderer.SetPropertyBlock(m_maskingPropertyBlock);
            */
        }


        // Function to allocate the necessary buffers to render the text. This function is called whenever the buffer size needs to be increased.
        void SetMeshArrays(int size)
        {
            // Should add a check to make sure we don't try to create a mesh that contains more than 65535 vertices.

            int sizeX4 = size * 4;
            int sizeX6 = size * 6;

            m_vertices = new Vector3[sizeX4];
            m_normals = new Vector3[sizeX4];
            m_tangents = new Vector4[sizeX4];

            m_uvs = new Vector2[sizeX4];
            m_uv2s = new Vector2[sizeX4];
            m_vertColors = new Color32[sizeX4];

            m_triangles = new int[sizeX6];

            // Setup Triangle Structure 
            for (int i = 0; i < size; i++)
            {
                int index_X4 = i * 4;
                int index_X6 = i * 6;

                m_vertices[0 + index_X4] = Vector3.zero;
                m_vertices[1 + index_X4] = Vector3.zero;
                m_vertices[2 + index_X4] = Vector3.zero;
                m_vertices[3 + index_X4] = Vector3.zero;

                m_uvs[0 + index_X4] = Vector2.zero;
                m_uvs[1 + index_X4] = Vector2.zero;
                m_uvs[2 + index_X4] = Vector2.zero;
                m_uvs[3 + index_X4] = Vector2.zero;

                m_normals[0 + index_X4] = new Vector3(0, 0, -1);
                m_normals[1 + index_X4] = new Vector3(0, 0, -1);
                m_normals[2 + index_X4] = new Vector3(0, 0, -1);
                m_normals[3 + index_X4] = new Vector3(0, 0, -1);

                m_tangents[0 + index_X4] = new Vector4(-1, 0, 0, 1);
                m_tangents[1 + index_X4] = new Vector4(-1, 0, 0, 1);
                m_tangents[2 + index_X4] = new Vector4(-1, 0, 0, 1);
                m_tangents[3 + index_X4] = new Vector4(-1, 0, 0, 1);

                // Setup Triangles based on whether or not Shadow Mode is Enabled.       
                m_triangles[0 + index_X6] = 0 + index_X4;
                m_triangles[1 + index_X6] = 1 + index_X4;
                m_triangles[2 + index_X6] = 2 + index_X4;
                m_triangles[3 + index_X6] = 3 + index_X4;
                m_triangles[4 + index_X6] = 2 + index_X4;
                m_triangles[5 + index_X6] = 1 + index_X4;
            }

            //Debug.Log("Size:" + size + "  Vertices:" + m_vertices.Length + "  Triangles:" + m_triangles.Length + " Mesh - Vertices:" + m_mesh.vertices.Length + "  Triangles:" + m_mesh.triangles.Length);

            m_mesh.vertices = m_vertices;
            m_mesh.uv = m_uvs;
            m_mesh.normals = m_normals;
            m_mesh.tangents = m_tangents;
            m_mesh.triangles = m_triangles;

            //Debug.Log("Bounds were updated.");
            m_mesh.bounds = m_default_bounds;
        }


        // Function called internally when a new material is assigned via the fontMaterial property.
        void SetFontMaterial(Material mat)
        {
            // Check in case Object is disabled. If so, we don't have a valid reference to the Renderer.
            // This can occur when the Duplicate Material Context menu is used on an inactive object.
            if (m_renderer == null)
                m_renderer = GetComponent<Renderer>();

            m_renderer.material = mat;
            m_fontMaterial = m_renderer.material;
            m_sharedMaterial = m_fontMaterial;
            m_padding = ShaderUtilities.GetPadding(m_renderer.sharedMaterials, m_enableExtraPadding, m_isUsingBold);
            m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
        }


        // Function called internally when a new shared material is assigned via the fontSharedMaterial property.
        void SetSharedFontMaterial(Material mat)
        {
            // Check in case Object is disabled. If so, we don't have a valid reference to the Renderer.
            // This can occur when the Duplicate Material Context menu is used on an inactive object.
            if (m_renderer == null)
                m_renderer = GetComponent<Renderer>();

            m_renderer.sharedMaterial = mat;
            m_sharedMaterial = m_renderer.sharedMaterial;
            m_padding = ShaderUtilities.GetPadding(m_renderer.sharedMaterials, m_enableExtraPadding, m_isUsingBold);
            m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
        }


        // This function will create an instance of the Font Material.
        void SetOutlineThickness(float thickness)
        {            
            thickness = Mathf.Clamp01(thickness);
            m_renderer.material.SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);

            if (m_fontMaterial == null)
                m_fontMaterial = m_renderer.material;

            m_fontMaterial = m_renderer.material;               
        }


        // This function will create an instance of the Font Material.
        void SetFaceColor(Color32 color)
        {
            m_renderer.material.SetColor(ShaderUtilities.ID_FaceColor, color);

            if (m_fontMaterial == null)
                m_fontMaterial = m_renderer.material;

            //Debug.Log("Material ID:" + m_fontMaterial.GetInstanceID());
            //m_faceColor = m_renderer.material;
        }


        // This function will create an instance of the Font Material.
        void SetOutlineColor(Color32 color)
        {
            m_renderer.material.SetColor(ShaderUtilities.ID_OutlineColor, color);

            if (m_fontMaterial == null)
                m_fontMaterial = m_renderer.material;

            //Debug.Log("Material ID:" + m_fontMaterial.GetInstanceID());
            //m_faceColor = m_renderer.material;
        }


        // Function used to create an instance of the material
        void CreateMaterialInstance()
        {
            Material mat = new Material(m_sharedMaterial);
            mat.shaderKeywords = m_sharedMaterial.shaderKeywords;

            //mat.hideFlags = HideFlags.DontSave;
            mat.name += " Instance";
            //m_uiRenderer.SetMaterial(mat, null);
            m_fontMaterial = mat;
        }



        // Sets the Render Queue and Ztest mode 
        void SetShaderType()
        {
            if (m_isOverlay)
            {
                // Changing these properties results in an instance of the material           
                m_renderer.material.SetFloat("_ZTestMode", 8);
                m_renderer.material.renderQueue = 4000;

                m_sharedMaterial = m_renderer.material;
                //Debug.Log("Text set to Overlay mode.");
            }
            else
            {   // TODO: This section needs to be tested.
                //m_renderer.material.SetFloat("_ZWriteMode", 0);
                m_renderer.material.SetFloat("_ZTestMode", 4);
                m_renderer.material.renderQueue = -1;
                m_sharedMaterial = m_renderer.material;
                //Debug.Log("Text set to Normal mode.");
            }
          
            //if (m_fontMaterial == null)
            //    m_fontMaterial = m_renderer.material;
        }      


        // Sets the Culling mode of the material
        void SetCulling()
        {
            if (m_isCullingEnabled)
            {                        
                m_renderer.material.SetFloat("_CullMode", 2);
            }
            else
            {
                m_renderer.material.SetFloat("_CullMode", 0);
            }
        }


        // Set Perspective Correction Mode based on whether Camera is Orthographic or Perspective
        void SetPerspectiveCorrection()
        {
            if (m_isOrthographic)
                m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0.0f);
            else
                m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0.875f);
        }


        // Function used in conjunection with SetText()
        void AddIntToCharArray(int number, ref int index, int precision)
        {
            if (number < 0)
            {
                m_input_CharArray[index++] = '-';
                number = -number;
            }

            int i = index;
            do
            {
                m_input_CharArray[i++] = (char)(number % 10 + 48);
                number /= 10;
            } while (number > 0);

            int lastIndex = i;

            // Reverse string
            while (index + 1 < i)
            {
                i -= 1;
                char t = m_input_CharArray[index];
                m_input_CharArray[index] = m_input_CharArray[i];
                m_input_CharArray[i] = t;
                index += 1;
            }
            index = lastIndex;
        }


        // Functions used in conjunction with SetText()
        void AddFloatToCharArray(float number, ref int index, int precision)
        {
            if (number < 0)
            {
                m_input_CharArray[index++] = '-';
                number = -number;
            }

            number += k_Power[Mathf.Min(9, precision)];

            int integer = (int)number;
            AddIntToCharArray(integer, ref index, precision);

            if (precision > 0)
            {
                // Add the decimal point
                m_input_CharArray[index++] = '.';

                number -= integer;
                for (int p = 0; p < precision; p++)
                {
                    number *= 10;
                    int d = (int)(number);

                    m_input_CharArray[index++] = (char)(d + 48);
                    number -= d;
                }
            }
        }


        // Converts a string to a Char[]
        void StringToCharArray(string text, ref int[] chars)
        {
            if (text == null)
                return;

            // Check to make sure chars_buffer is large enough to hold the content of the string.
            if (chars.Length <= text.Length)
            {
                int newSize = text.Length > 1024 ? text.Length + 256 : Mathf.NextPowerOfTwo(text.Length + 1);
                //Debug.Log("Resizing the chars_buffer[" + chars.Length + "] to chars_buffer[" + newSize + "].");
                chars = new int[newSize];
            }

            int index = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == 92 && i < text.Length - 1)
                {
                    switch ((int)text[i + 1])
                    {
                        case 110: // \n LineFeed
                            chars[index] = (char)10;
                            i += 1;
                            index += 1;
                            continue;
                        case 114: // \r LineFeed
                            chars[index] = (char)13;
                            i += 1;
                            index += 1;
                            continue;
                        case 116: // \t Tab
                            chars[index] = (char)9;
                            i += 1;
                            index += 1;
                            continue;
                    }
                }

                chars[index] = text[i];
                index += 1;
            }
            chars[index] = (char)0;
        }


        // Copies Content of formatted SetText() to charBuffer.
        void SetTextArrayToCharArray(char[] charArray, ref int[] charBuffer)
        {
            //Debug.Log("SetText Array to Char called.");
            if (charArray == null || m_charArray_Length == 0)
                return;

            // Check to make sure chars_buffer is large enough to hold the content of the string.
            if (charBuffer.Length <= m_charArray_Length)
            {
                int newSize = m_charArray_Length > 1024 ? m_charArray_Length + 256 : Mathf.NextPowerOfTwo(m_charArray_Length + 1);
                charBuffer = new int[newSize];
            }

            int index = 0;

            for (int i = 0; i < m_charArray_Length; i++)
            {
                if (charArray[i] == 92 && i < m_charArray_Length - 1)
                {
                    switch ((int)charArray[i + 1])
                    {
                        case 110: // \n LineFeed
                            charBuffer[index] = 10;
                            i += 1;
                            index += 1;
                            continue;
                        case 114: // \r LineFeed
                            charBuffer[index] = 13;
                            i += 1;
                            index += 1;
                            continue;
                        case 116: // \t Tab
                            charBuffer[index] = 9;
                            i += 1;
                            index += 1;
                            continue;
                    }
                }

                charBuffer[index] = charArray[i];
                index += 1;
            }
            charBuffer[index] = 0;
        }


        /// <summary>
        /// Function used in conjunction with GetTextInfo to figure out Array allocations.
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        int GetArraySizes(int[] chars)
        {
            //Debug.Log("Set Array Size called.");

            int visibleCount = 0;
            int totalCount = 0;
            int tagEnd = 0;
            m_isUsingBold = false;

            m_VisibleCharacters.Clear();

            for (int i = 0; chars[i] != 0; i++)
            {
                int c = chars[i];

                if (m_isRichText && c == 60) // if Char '<'
                {
                    // Check if Tag is Valid
                    if (ValidateHtmlTag(chars, i + 1, out tagEnd))
                    {
                        i = tagEnd;
                        if ((m_style & FontStyles.Underline) == FontStyles.Underline) visibleCount += 3;

                        if ((m_style & FontStyles.Bold) == FontStyles.Bold) m_isUsingBold = true;

                        continue;
                    }
                }

                if (c != 32 && c != 9 && c != 10 && c != 13)
                {
                    visibleCount += 1;
                }

                m_VisibleCharacters.Add((char)c);   
                totalCount += 1;
            }
                   
            return totalCount;
        }




        // This function parses through the Char[] to determine how many characters will be visible. It then makes sure the arrays are large enough for all those characters.
        int SetArraySizes(int[] chars)
        {
            //Debug.Log("Set Array Size called.");

            int visibleCount = 0;
            int totalCount = 0;
            int tagEnd = 0;
            m_isUsingBold = false;

            m_VisibleCharacters.Clear();

            for (int i = 0; chars[i] != 0; i++)
            {
                int c = chars[i];

                if (m_isRichText && c == 60) // if Char '<'
                {
                    // Check if Tag is Valid
                    if (ValidateHtmlTag(chars, i + 1, out tagEnd))
                    {
                        i = tagEnd;
                        if ((m_style & FontStyles.Underline) == FontStyles.Underline) visibleCount += 3;

                        if ((m_style & FontStyles.Bold) == FontStyles.Bold) m_isUsingBold = true;
            
                        continue;
                    }
                }

                if (c != 32 && c != 9 && c != 10 && c != 13)
                {
                    visibleCount += 1;
                }

                m_VisibleCharacters.Add((char)c);              
                totalCount += 1;               
            }


            if (m_textInfo.characterInfo == null || totalCount > m_textInfo.characterInfo.Length)
            {                
                m_textInfo.characterInfo = new TMP_CharacterInfo[totalCount > 1024 ? totalCount + 256 : Mathf.NextPowerOfTwo(totalCount)];              
            }

            // Make sure our Mesh Buffer Allocations can hold these new Quads.
            if (visibleCount * 4 > m_vertices.Length)
            {              
                // If this is the first allocation, we allocated exactly the number of Quads we need. Otherwise, we allocated more since this text object is dynamic.
                if (m_isFirstAllocation)
                {
                    SetMeshArrays(visibleCount);
                    m_isFirstAllocation = false;
                }
                else
                {
                    SetMeshArrays(visibleCount > 1024 ? visibleCount + 256 : Mathf.NextPowerOfTwo(visibleCount));
                }
            }

            return totalCount;
        }


        // Added to sort handle the potential issue with OnWillRenderObject() not getting called when objects are not visible by camera.
        //void OnBecameInvisible()
        //{
        //    if (m_mesh != null)
        //        m_mesh.bounds = new Bounds(transform.position, new Vector3(1000, 1000, 0));
        //}


        // Method to parse the input text based on its source
        void ParseInputText()
        {
            //Debug.Log("Reparsing Text.");
            isInputParsingRequired = false;

            switch (m_inputSource)
            {
                case TextInputSources.Text:
                    StringToCharArray(m_text, ref m_char_buffer);
                    //isTextChanged = false;
                    break;
                case TextInputSources.SetText:
                    SetTextArrayToCharArray(m_input_CharArray, ref m_char_buffer);
                    //isSetTextChanged = false;
                    break;
                case TextInputSources.SetCharArray:
                    break;
            }
        }


        void OnDidApplyAnimationProperties()
        {
            havePropertiesChanged = true;
            isMaskUpdateRequired = true;
            //Debug.Log("Animation Properties have changed.");
        }

        
        // Function still not implemented which will eventually replace OnWillRenderObject()
        void OnPreRenderObject()
        {
            //Debug.Log("OnPreRenderObject() called.");
            //Debug.Log(m_renderer.sortingLayerID);
        }

        // Called for every Camera able to see the Object. 
        void OnWillRenderObject()
        {                      
            // This will be called for each active camera and thus should be optimized as it is not necessary to update the mesh for each camera.
            //Debug.Log("OnWillRenderObject() called!");            
            
            //isDebugOutputDone = false;
            loopCountA = 0;
            //loopCountB = 0;
            //loopCountC = 0;
            //loopCountD = 0;
            //loopCountE = 0;

            // Return if no Font Asset has been assigned.
            if (m_fontAsset == null)
                return;

            // Check if Transform has changed since last update.          
            if (m_transform.hasChanged)
            {
                //Debug.Log("Transform has changed.");
                
                m_transform.hasChanged = false;
                
                if (m_textContainer != null && m_textContainer.hasChanged)
                {
                    //Debug.Log("Text Container has changed.");
                    
                    //Update Mask Coordinates     
                    isMaskUpdateRequired = true;
                    
                    m_textContainer.hasChanged = false;
                    havePropertiesChanged = true;
                }


                // We need to regenerate the mesh if the lossy scale has changed.
                Vector3 currentLossyScale = m_transform.lossyScale;
                if (currentLossyScale != m_previousLossyScale)
                {
                    // Update UV2 Scale - only if we don't have to regenerate the text object anyway.
                    if (havePropertiesChanged == false && m_previousLossyScale.z != 0 && m_text != string.Empty)
                        UpdateSDFScale(m_previousLossyScale.z, currentLossyScale.z);
                    else
                        havePropertiesChanged = true;

                    m_previousLossyScale = currentLossyScale;
                }
            }
                     

            if (havePropertiesChanged || m_fontAsset.propertiesChanged)
            {
                //Debug.Log("Properties have changed!"); // Assigned Material is:" + m_sharedMaterial); // New Text is: " + m_text + ".");                

                if (hasFontAssetChanged || m_fontAsset.propertiesChanged)
                {
                    //Debug.Log( m_fontAsset.name);
                    
                    LoadFontAsset();

                    hasFontAssetChanged = false;

                    if (m_fontAsset == null || m_renderer.sharedMaterial == null)
                        return;

                    m_fontAsset.propertiesChanged = false;
                }

                
                if (isMaskUpdateRequired)
                {                                                                       
                    UpdateMask();
                    isMaskUpdateRequired = false;
                }
                

                // Reparse the text if the input has changed or text was truncated.
                if (isInputParsingRequired || m_isTextTruncated)
                    ParseInputText();


				// Reset Font min / max used with Auto-sizing
                if (m_enableAutoSizing)
                    m_fontSize = Mathf.Clamp(m_fontSize, m_fontSizeMin, m_fontSizeMax);

                m_maxFontSize = m_fontSizeMax;
				m_minFontSize = m_fontSizeMin;
                m_lineSpacingDelta = 0;
                m_recursiveCount = 0;

                m_isCharacterWrappingEnabled = false;
                m_isTextTruncated = false;

                GenerateTextMesh();              
                havePropertiesChanged = false;
                //isAlreadyScheduled = false;
            }          
        }

        

        
        /// <summary>
        /// This is the main function that is responsible for creating / displaying the text.
        /// </summary>
        void GenerateTextMesh()
        {
            //Debug.Log("***** GenerateTextMesh() ***** Iteration Count: " + loopCountA + ".  Min: " + m_minFontSize + "  Max: " + m_maxFontSize + "  Font size is " + m_fontSize);
            
            // Early exit if no font asset was assigned. This should not be needed since Arial SDF will be assigned by default.
            if (m_fontAsset.characterDictionary == null)
            {
                Debug.Log("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + this.GetInstanceID());
                return;
            }

            // Reset TextInfo
            if (m_textInfo != null)
                m_textInfo.Clear();       


            // Early exit if we don't have any Text to generate.          
            if (m_char_buffer == null || m_char_buffer[0] == (char)0)
            {
                //Debug.Log("Early Out!");
                if (m_vertices != null)
                {
                    Array.Clear(m_vertices, 0, m_vertices.Length);
                    m_mesh.vertices = m_vertices;                   
                }

                m_preferredWidth = 0;
                m_preferredHeight = 0;

                return;
            }

            
            // Determine how many characters will be visible and make the necessary allocations (if needed).
            int totalCharacterCount = SetArraySizes(m_char_buffer);

            m_fontIndex = 0; // Will be used when support for using different font assets or sprites withint the same object will be added.
            m_fontAssetArray[m_fontIndex] = m_fontAsset;

            // Scale the font to approximately match the point size           
            m_fontScale = (m_fontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize * (m_isOrthographic ? 1 : 0.1f));
            float baseScale = m_fontScale; // BaseScale keeps the character aligned vertically since <size=+000> results in font of different scale.
            m_maxFontScale = 0;
            float previousFontScale = 0;
            m_currentFontSize = m_fontSize;
            float fontSizeDelta = 0;
           
            int charCode = 0; // Holds the character code of the currently being processed character.
            //int prev_charCode = 0;
            bool isMissingCharacter; // Used to handle missing characters in the Font Atlas / Definition.

            m_style = m_fontStyle; // Set the defaul style.
            m_lineJustification = m_textAlignment; // Sets the line justification mode to match editor alignment.

            // GetPadding to adjust the size of the mesh due to border thickness, softness, glow, etc...
            if (checkPaddingRequired)
            {
                checkPaddingRequired = false;
                m_padding = ShaderUtilities.GetPadding(m_renderer.sharedMaterials, m_enableExtraPadding, m_isUsingBold);
                m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
                m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
            }

            float style_padding = 0; // Extra padding required to accomodate Bold style.
            float xadvance_multiplier = 1; // Used to increase spacing between character when style is bold.         

            m_baselineOffset = 0; // Used by subscript characters.

            bool beginUnderline = false;
            Vector3 underline_start = Vector3.zero; // Used to track where underline starts & ends.
            Vector3 underline_end = Vector3.zero;

            m_fontColor32 = m_fontColor;
            Color32 vertexColor;
            m_htmlColor = m_fontColor32;
            m_colorStackIndex = 0;
            Array.Clear(m_colorStack, 0, m_colorStack.Length);

           
            m_lineOffset = 0; // Amount of space between lines (font line spacing + m_linespacing).
            m_lineHeight = 0;
            m_cSpacing = 0;
            m_monoSpacing = 0;
            float lineOffsetDelta = 0;
            m_xAdvance = 0; // Used to track the position of each character.
            m_indent = 0; // Used for indentation of text.
            m_maxXAdvance = 0; // Used to determine Preferred Width.

            m_lineNumber = 0;
            m_pageNumber = 0;
            //int previousPageNumber = -1;
            m_characterCount = 0; // Total characters in the char[]
            m_visibleCharacterCount = 0; // # of visible characters.

            // Limit Line Length to whatever size fits all characters on a single line.
            //m_lineLength = m_lineLength > max_LineWrapLength ? max_LineWrapLength : m_lineLength;
            m_firstVisibleCharacterOfLine = 0;
            m_lastVisibleCharacterOfLine = 0; 
            
            int ellipsisIndex = 0;
            //int truncateIndex = -1;     
            //m_isTextTruncated = false;
            //bool isLineTruncated = false;
            //int truncatedLine = 0;
            //m_isTextTruncated = false;

            Vector3[] corners = m_textContainer.corners;
            Vector4 margins = m_textContainer.margins;
            m_marginWidth = m_textContainer.rect.width - margins.z - margins.x;
            float marginHeight = m_textContainer.rect.height - margins.y - margins.w;

            float lossyScale = m_transform.lossyScale.z;
            // Used by Unity's Auto Layout system.
            m_preferredWidth = 0;
            m_preferredHeight = 0;

            // Initialize struct to track states of word wrapping
            bool isFirstWord = true;
            bool isLastBreakingChar = false;
            m_SavedWordWrapState = new WordWrapState();
            m_SavedLineState = new WordWrapState(); 

            int wrappingIndex = 0;

            // Need to initialize these Extents Structs
            m_meshExtents = new Extents(k_InfinityVector, -k_InfinityVector);

            // Initialize lineInfo
            if (m_textInfo.lineInfo == null) m_textInfo.lineInfo = new TMP_LineInfo[2];
            for (int i = 0; i < m_textInfo.lineInfo.Length; i++)
            {
                m_textInfo.lineInfo[i] = new TMP_LineInfo();
                m_textInfo.lineInfo[i].lineExtents = new Extents(k_InfinityVector, -k_InfinityVector);
                m_textInfo.lineInfo[i].ascender = -k_InfinityVector.x;
                m_textInfo.lineInfo[i].descender = k_InfinityVector.x;
            }

                           
            // Tracking of the highest Ascender
            m_maxAscender = 0;
            m_maxDescender = 0;
            m_isNewPage = false;
            float pageAscender = 0;
                                 
            //bool isLineOffsetAdjusted = false;
            loopCountA += 1;

            int endTagIndex = 0;
            // Parse through Character buffer to read html tags and begin creating mesh.
            for (int i = 0; m_char_buffer[i] != 0; i++)
            {
                //m_tabSpacing = -999;
                //m_spacing = -999;
                charCode = m_char_buffer[i];

				//Debug.Log("i:" + i + "  Character [" + (char)charCode + "]");
                loopCountE += 1;

                if (m_isRichText && charCode == 60)  // '<'
                {
                    // Check if Tag is valid. If valid, skip to the end of the validated tag.
                    if (ValidateHtmlTag(m_char_buffer, i + 1, out endTagIndex))
                    {
                        i = endTagIndex;

                        if (m_isRecalculateScaleRequired)
                        {                          
                            m_fontScale = (m_currentFontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize * (m_isOrthographic ? 1 : 0.1f));                            
                            //isAffectingWordWrapping = true;
                            m_isRecalculateScaleRequired = false;                         
                        }
                        
                        //if (m_tabSpacing != -999)
                        //{
                            // Move character to a fix position. Position expresses in characters (approximation).
                            //m_xAdvance = m_tabSpacing * m_cached_Underline_GlyphInfo.width * m_fontScale;
                        //}
                      
                        //if (m_spacing != -999)
                        //{                           
                        //    m_xAdvance += m_spacing * m_fontScale * m_cached_Underline_GlyphInfo.width;                          
                        //}

                        continue;
                    }
                }

                isMissingCharacter = false;
               
              
                // Check if we should be using a different font asset
                //if (m_fontIndex != 0)
                //{
                //    // Check if we need to load the new font asset
                //    if (m_fontAssetArray[m_fontIndex] == null)
                //    {
                //        Debug.Log("Loading secondary font asset.");
                //        m_fontAssetArray[m_fontIndex] = Resources.Load("Fonts & Materials/Bangers SDF", typeof(TextMeshProFont)) as TextMeshProFont;
                //        //m_sharedMaterials.Add(m_fontAssetArray[m_fontIndex].material);
                //        //m_renderer.sharedMaterials = new Material[] { m_sharedMaterial, m_fontAssetArray[m_fontIndex].material }; // m_sharedMaterials.ToArray();
                //    }
                //}              
                //Debug.Log("Char [" + (char)charCode + "] ASCII " + charCode); //is using FontIndex: " + m_fontIndex);


                // Handle Font Styles like LowerCase, UpperCase and SmallCaps.
                #region Handling of LowerCase, UpperCase and SmallCaps Font Styles
                if ((m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
                {
                    // If this character is lowercase, switch to uppercase.
                    if (char.IsLower((char)charCode))
                        charCode -= 32;

                }
                else if ((m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
                {
                    // If this character is uppercase, switch to lowercase.
                    if (char.IsUpper((char)charCode))
                        charCode += 32;
                }
                else if ((m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps)
                {
                    if (char.IsLower((char)charCode))
                    {                      
                        m_fontScale = m_currentFontSize * 0.8f / m_fontAssetArray[m_fontIndex].fontInfo.PointSize * (m_isOrthographic ? 1 : 0.1f);
                        charCode -= 32;
                    }
                    else
                        m_fontScale = m_currentFontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize * (m_isOrthographic ? 1 : 0.1f);

                }
                #endregion


                // Look up Character Data from Dictionary and cache it.
                #region Look up Character Data
                m_fontAssetArray[m_fontIndex].characterDictionary.TryGetValue(charCode, out m_cached_GlyphInfo);
                if (m_cached_GlyphInfo == null)
                {
                    // Character wasn't found in the Dictionary.
                    
                    // Check if Lowercase & Replace by Uppercase if possible                                   
                    if (char.IsLower((char)charCode))
                    {
                        if (m_fontAssetArray[m_fontIndex].characterDictionary.TryGetValue(charCode - 32, out m_cached_GlyphInfo))                        
                            charCode -= 32;                    
                    }
                    else if (char.IsUpper((char)charCode))
                    {
                        if (m_fontAssetArray[m_fontIndex].characterDictionary.TryGetValue(charCode + 32, out m_cached_GlyphInfo))                      
                            charCode += 32;                       
                    }
                    
                    // Still don't have a replacement?
                    if (m_cached_GlyphInfo == null)
                    {
                        m_fontAssetArray[m_fontIndex].characterDictionary.TryGetValue(88, out m_cached_GlyphInfo);
                        if (m_cached_GlyphInfo != null)
                        {
                            Debug.LogWarning("Character with ASCII value of " + charCode + " was not found in the Font Asset Glyph Table.");
                            // Replace the missing character by X (if it is found)
                            charCode = 88;
                            isMissingCharacter = true;
                        }
                        else
                        {  // At this point the character isn't in the Dictionary, the replacement X isn't either so ...                         
                            Debug.LogWarning("Character with ASCII value of " + charCode + " was not found in the Font Asset Glyph Table.");
                            continue;
                        }
                    }
                }
                #endregion


                // Store some of the text object's information                
                m_textInfo.characterInfo[m_characterCount].character = (char)charCode;
                m_textInfo.characterInfo[m_characterCount].color = m_htmlColor;
                m_textInfo.characterInfo[m_characterCount].style = m_style;
                m_textInfo.characterInfo[m_characterCount].index = (short)i;               


                // Handle Kerning if Enabled.                 
                #region Handle Kerning
                if (m_enableKerning && m_characterCount >= 1)
                {
                    int prev_charCode = m_textInfo.characterInfo[m_characterCount - 1].character;
                    KerningPairKey keyValue = new KerningPairKey(prev_charCode, charCode);

                    KerningPair pair;

                    m_fontAssetArray[m_fontIndex].kerningDictionary.TryGetValue(keyValue.key, out pair);
                    if (pair != null)
                    {
                        m_xAdvance += pair.XadvanceOffset * m_fontScale;
                    }
                }
                #endregion


                // Handle Mono Spacing
                #region Handle Mono Spacing
                if (m_monoSpacing != 0 && m_xAdvance != 0)              
                    m_xAdvance -= (m_cached_GlyphInfo.width / 2 + m_cached_GlyphInfo.xOffset) * m_fontScale;               
                #endregion

              
                // Set Padding based on selected font styles
                #region Handle Style Padding
                if ((m_style & FontStyles.Bold) == FontStyles.Bold || (m_fontStyle & FontStyles.Bold) == FontStyles.Bold) // Checks for any combination of Bold Style.
                {
                    style_padding = m_fontAssetArray[m_fontIndex].BoldStyle * 2;
                    xadvance_multiplier = 1.07f; // Increase xAdvance for bold characters.         
                }
                else
                {
                    style_padding = m_fontAssetArray[m_fontIndex].NormalStyle * 2;
                    xadvance_multiplier = 1.0f;
                }
                #endregion Handle Style Padding


                // Setup Vertices for each character.
                Vector3 top_left = new Vector3(0 + m_xAdvance + ((m_cached_GlyphInfo.xOffset - m_padding - style_padding) * m_fontScale), (m_cached_GlyphInfo.yOffset + m_padding) * m_fontScale - m_lineOffset + m_baselineOffset, 0);
                Vector3 bottom_left = new Vector3(top_left.x, top_left.y - ((m_cached_GlyphInfo.height + m_padding * 2) * m_fontScale), 0);
                Vector3 top_right = new Vector3(bottom_left.x + ((m_cached_GlyphInfo.width + m_padding * 2 + style_padding * 2) * m_fontScale), top_left.y, 0);
                Vector3 bottom_right = new Vector3(top_right.x, bottom_left.y, 0);

				
                // Check if we need to Shear the rectangles for Italic styles
                #region Handle Italic & Shearing
                if ((m_style & FontStyles.Italic) == FontStyles.Italic || (m_fontStyle & FontStyles.Italic) == FontStyles.Italic)
                {
                    // Shift Top vertices forward by half (Shear Value * height of character) and Bottom vertices back by same amount. 
                    float shear_value = m_fontAssetArray[m_fontIndex].ItalicStyle * 0.01f;
                    Vector3 topShear = new Vector3(shear_value * ((m_cached_GlyphInfo.yOffset + m_padding + style_padding) * m_fontScale), 0, 0);
                    Vector3 bottomShear = new Vector3(shear_value * (((m_cached_GlyphInfo.yOffset - m_cached_GlyphInfo.height - m_padding - style_padding)) * m_fontScale), 0, 0);

                    top_left = top_left + topShear;
                    bottom_left = bottom_left + bottomShear;
                    top_right = top_right + topShear;
                    bottom_right = bottom_right + bottomShear;
                }
                #endregion Handle Italics & Shearing


                // Store postion of vertices for each character
                m_textInfo.characterInfo[m_characterCount].topLeft = top_left;
                m_textInfo.characterInfo[m_characterCount].bottomLeft = bottom_left;
                m_textInfo.characterInfo[m_characterCount].topRight = top_right;
                m_textInfo.characterInfo[m_characterCount].bottomRight = bottom_right;

				// Compute MaxAscender & MaxDescender which is used for AutoScaling & other type layout options
                float ascender = (m_fontAsset.fontInfo.Ascender + m_alignmentPadding.y) * m_fontScale + m_baselineOffset;
				if (m_lineNumber == 0) m_maxAscender = m_maxAscender > ascender ? m_maxAscender : ascender;
                if (m_lineOffset == 0) pageAscender = pageAscender > ascender ? pageAscender : ascender;

                float descender = (m_fontAsset.fontInfo.Descender + m_alignmentPadding.w) * m_fontScale - m_lineOffset + m_baselineOffset;                 				

                //Debug.Log("Character [" + (char)charCode + "] at Index: " + m_characterCount + " with Ascender of " + ascender + " and Descender of " + descender + "  Max Ascender: " + m_maxAscender + "  Max Descender: " + m_maxDescender);
                

                // Set Characters to not visible by default.
                m_textInfo.characterInfo[m_characterCount].isVisible = false;


                // Setup Mesh for visible characters. ie. not a SPACE / LINEFEED / CARRIAGE RETURN.
                #region Handle Visible Characters
                if (charCode != 32 && charCode != 9 && charCode != 10 && charCode != 13)
                {
                    int index_X4 = m_visibleCharacterCount * 4;                   

                    m_textInfo.characterInfo[m_characterCount].isVisible = true;
                    m_textInfo.characterInfo[m_characterCount].vertexIndex = (short)(0 + index_X4);

                    m_vertices[0 + index_X4] = m_textInfo.characterInfo[m_characterCount].bottomLeft;
                    m_vertices[1 + index_X4] = m_textInfo.characterInfo[m_characterCount].topLeft;
                    m_vertices[2 + index_X4] = m_textInfo.characterInfo[m_characterCount].bottomRight;
                    m_vertices[3 + index_X4] = m_textInfo.characterInfo[m_characterCount].topRight;

                                     
                    //Debug.Log("Line Number: " + m_lineNumber + " Char [" + (char)charCode + "] Extents MinX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.x + "  MaxX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x); 

                    //m_textInfo.characterInfo[character_Count].charNumber = (short)m_lineExtents[lineNumber].characterCount;                                      

                    // Used to adjust line spacing when larger fonts or the size tag is used.
                    if (m_baselineOffset == 0)
                        m_maxFontScale = Mathf.Max(m_maxFontScale, m_fontScale);                       
                    

                   // Debug.Log("Line # " + m_lineNumber + " BaseScale: " + baseScale + "  MaxFontScale: " + m_maxFontScale);     

                    // Check if Character exceeds the width of the Text Container               
                    #region Check for Characters Exceeding Width of Text Container
                    if (m_xAdvance + m_cached_GlyphInfo.xAdvance * m_fontScale > m_marginWidth + 0.0001f && !m_textContainer.isDefaultWidth)
                    {

                        ellipsisIndex = m_characterCount - 1; // Last safely rendered character

                        // Word Wrapping
                        #region Handle Word Wrapping
                        if (enableWordWrapping && m_characterCount != m_firstVisibleCharacterOfLine)
                        {
                                                                                                         
                            if (wrappingIndex == m_SavedWordWrapState.previous_WordBreak || isFirstWord)
                            {
                                // Word wrapping is no longer possible. Shrink size of text if auto-sizing is enabled.
                                if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
                                {
                                    m_maxFontSize = m_fontSize;

                                    m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2, 0.05f);
                                    m_fontSize = (int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20 + 0.5f) / 20f;

                                    if (loopCountA > 20) return; // Added to debug                               
                                    GenerateTextMesh();
                                    return;
                                }

                                // Word wrapping is no longer possible, now breaking up individual words.
                                if (m_isCharacterWrappingEnabled == false)
                                {
                                    m_isCharacterWrappingEnabled = true; // Should add a check to make sure this mode is available.                                                                                                       

                                    //GenerateTextMesh();
                                    //return;
                                }
                                else
                                    isLastBreakingChar = true;

                                m_recursiveCount += 1;
                                if (m_recursiveCount > 20)
                                {
                                    //Debug.Log("Recursive count exceeded!");
                                    continue;
                                }                                                                
                            }
                            

                            // Restore to previously stored state of last valid (space character or linefeed)
                            i = RestoreWordWrappingState(ref m_SavedWordWrapState);                          
                            wrappingIndex = i;  // Used to dectect when line length can no longer be reduced.

                            //Debug.Log("Line # " + m_lineNumber + " Last Character of Line is [" + m_textInfo.characterInfo[m_characterCount - 1].character + "]. Last Visible Character is [" + m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].character + "].");
                        
                            // Check if we need to Adjust LineOffset & Restore State to the start of the line.                            
                            if (m_lineNumber > 0 && m_maxFontScale != 0 && m_lineHeight == 0 && m_maxFontScale != previousFontScale && !m_isNewPage)
                            {                         
                                //Debug.Log("Line # " + m_lineNumber + "'s lineOffset between Characters [" + m_textInfo.characterInfo[firstCharacterOfLine].character + "] and [" + m_textInfo.characterInfo[m_characterCount - 2].character + "] needs to be adjusted. BaseScale: " + baseScale + "  MaxFontScale: " + m_maxFontScale);
                                // Compute Offset
                                float gap = m_fontAssetArray[m_fontIndex].fontInfo.LineHeight - (m_fontAssetArray[m_fontIndex].fontInfo.Ascender - m_fontAssetArray[m_fontIndex].fontInfo.Descender);
                                float offsetDelta = (m_fontAssetArray[m_fontIndex].fontInfo.Ascender + gap + m_lineSpacing + m_lineSpacingDelta) * m_maxFontScale - (m_fontAssetArray[m_fontIndex].fontInfo.Descender - gap) * previousFontScale;
                                m_lineOffset += offsetDelta - lineOffsetDelta;
                                AdjustLineOffset(m_firstVisibleCharacterOfLine, m_characterCount - 1, offsetDelta - lineOffsetDelta);
                                // Need to add code to update characterInfo structure                               
                                //Debug.Log("Line #" + m_lineNumber + " Offset Adjustment: " + (offsetDelta - lineOffsetDelta));
                            }
                            m_isNewPage = false;

                            // Calculate lineAscender & make sure if last character is superscript or subscript that we check that as well.
                            float lineAscender = (m_fontAsset.fontInfo.Ascender + m_alignmentPadding.y) * m_maxFontScale - m_lineOffset;
                            float lineAscender2 = (m_fontAsset.fontInfo.Ascender + m_alignmentPadding.y) * m_fontScale - m_lineOffset + m_baselineOffset;
                            lineAscender = lineAscender > lineAscender2 ? lineAscender : lineAscender2;

                            // Calculate lineDescender & make sure if last character is superscript or subscript that we check that as well.
                            float lineDescender = (m_fontAsset.fontInfo.Descender + m_alignmentPadding.w) * m_maxFontScale - m_lineOffset;
                            float lineDescender2 = (m_fontAsset.fontInfo.Descender + m_alignmentPadding.w) * m_fontScale - m_lineOffset + m_baselineOffset;
                            lineDescender = lineDescender < lineDescender2 ? lineDescender : lineDescender2;

                            // Update maxDescender if first character of line is visible.
                            if (m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].isVisible)
                                m_maxDescender = m_maxDescender < lineDescender ? m_maxDescender : lineDescender;
                                                                        
                                      
                            // Track & Store lineInfo for the new line                           
                            m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = m_firstVisibleCharacterOfLine; // Need new variable to track this
                            m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = m_characterCount - 1 > 0 ? m_characterCount - 1 : 1;
                            m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex = m_lastVisibleCharacterOfLine;
                            m_textInfo.lineInfo[m_lineNumber].characterCount = m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex - m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex + 1;
                                                           
                            m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].bottomLeft.x, lineDescender);                           
                            m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].topRight.x, lineAscender);
                            m_textInfo.lineInfo[m_lineNumber].lineLength = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - m_padding * m_maxFontScale;
                            m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - m_characterSpacing * m_fontScale; 
                           
                            m_firstVisibleCharacterOfLine = m_characterCount; // Store first character for the next line.


                            // Compute Preferred Width & Height                                                     
                            m_preferredWidth += m_xAdvance; // m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex].topRight.x - m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex].bottomLeft.x;
                            if (m_enableWordWrapping)
                                m_preferredHeight = m_maxAscender - m_maxDescender;
                            else
                                m_preferredHeight = Mathf.Max(m_preferredHeight, lineAscender - lineDescender);


                            //Debug.Log("LineInfo for line # " + (m_lineNumber) + " First character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex +
                            //                                                    " Last character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex +
                            //                                                    " Last Visible character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex +
                            //                                                    " Character Count: " + m_textInfo.lineInfo[m_lineNumber].characterCount + " Line Lenght: " + m_textInfo.lineInfo[m_lineNumber].lineLength /* +
                            //                                                    "  MinX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.x + "  MinY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.y +
                            //                                                    "  MaxX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x + "  MaxY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.y +
                            //                                                    "  Line Ascender: " + lineAscender + "  Line Descender: " + lineDescender + "  FontScale: " + m_fontScale + "  MaxFontScale: " + m_maxFontScale +
                            //                                                    "  Line MaxX: " + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex].topRight.x */);

                            // Store the state of the line before starting on the new line.
                            SaveWordWrappingState(ref m_SavedLineState, i, m_characterCount - 1);

                            m_lineNumber += 1;
                            // Check to make sure Array is large enough to hold a new line.
                            if (m_lineNumber >= m_textInfo.lineInfo.Length)
                                ResizeLineExtents(m_lineNumber);
                            
                            // Apply Line Spacing based on scale of the last character of the line.
                            if (m_lineHeight == 0)
                            {
                                //Debug.Log("Line # " + m_lineHeight + " - Font Scale is " + m_fontScale + "  Max Font Scale is " + m_maxFontScale);
                                lineOffsetDelta = (m_fontAssetArray[m_fontIndex].fontInfo.LineHeight + m_lineSpacing + m_lineSpacingDelta) * m_fontScale;
                                m_lineOffset += lineOffsetDelta;
                            }
                            else
                                m_lineOffset += (m_lineHeight + m_lineSpacing) * baseScale; // Special handling when line-height tag is used.
                           
                            previousFontScale = m_fontScale;
                            m_xAdvance = 0 + m_indent;
                            m_maxFontScale = 0;

                            continue;
                        }
                        #endregion End Word Wrapping


                        // Text Auto-Sizing (text exceeding Width of container. 
                        #region Handle Text Auto-Sizing
                        if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
                        {
                            m_maxFontSize = m_fontSize;

                            m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2, 0.05f);
                            m_fontSize = (int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20 + 0.5f) / 20f;

                            m_recursiveCount = 0;
                            if (loopCountA > 20) return; // Added to debug 
                            GenerateTextMesh();
                            return;
                        }
                        #endregion End Text Auto-Sizing


                        // Handle Text Overflow
                        #region Handle Text Overflow
                        switch (m_overflowMode)
                        {
                            case TextOverflowModes.Overflow:
                                if (m_isMaskingEnabled)
                                    DisableMasking();

                                break;
                            case TextOverflowModes.Ellipsis:
                                if (m_isMaskingEnabled)
                                    DisableMasking();
                           
                                m_isTextTruncated = true;

                                if (m_characterCount < 1)
                                {
                                    m_textInfo.characterInfo[m_characterCount].isVisible = false;
                                    m_visibleCharacterCount -= 1;
                                    break;
                                }

                                m_char_buffer[i - 1] = 8230;
                                m_char_buffer[i] = (char)0;

                                GenerateTextMesh();                           
                                return;
                            case TextOverflowModes.Masking:
                                if (!m_isMaskingEnabled)
                                    EnableMasking();
                                break;
                            case TextOverflowModes.ScrollRect:
                                if (!m_isMaskingEnabled)
                                    EnableMasking();
                                break;
                            case TextOverflowModes.Truncate:
                                if (m_isMaskingEnabled)
                                    DisableMasking();

                                m_textInfo.characterInfo[m_characterCount].isVisible = false;                               
                                break;
                        }
                        #endregion End Text Overflow
                    
                    }
                    #endregion End Check for Characters Exceeding Width of Text Container                 
                          
                                                                                 
                    // Determine what color gets assigned to vertex.     
                    #region Handle Vertex Colors
                    if (isMissingCharacter)
                        vertexColor = Color.red;
                    else if (m_overrideHtmlColors)
                        vertexColor = m_fontColor32;
                    else
                        vertexColor = m_htmlColor;


                    // Set Alpha to the lesser of vertex color or color tag alpha).                 
                    vertexColor.a = m_fontColor32.a < vertexColor.a ? (byte)(m_fontColor32.a) : (byte)(vertexColor.a);
               

					if (!m_enableVertexGradient)
					{
                    	m_vertColors[0 + index_X4] = vertexColor;
                    	m_vertColors[1 + index_X4] = vertexColor;
                    	m_vertColors[2 + index_X4] = vertexColor;
						m_vertColors[3 + index_X4] = vertexColor;
					}
					else
					{

                        if (!m_overrideHtmlColors && !m_htmlColor.CompareRGB(m_fontColor32))
                        {
                            m_vertColors[0 + index_X4] = vertexColor;
                            m_vertColors[1 + index_X4] = vertexColor;
                            m_vertColors[2 + index_X4] = vertexColor;
                            m_vertColors[3 + index_X4] = vertexColor;
                        }
                        else
                        { 
                            m_vertColors[0 + index_X4] = m_fontColorGradient.bottomLeft;
                            m_vertColors[1 + index_X4] = m_fontColorGradient.topLeft;
                            m_vertColors[2 + index_X4] = m_fontColorGradient.bottomRight;
						    m_vertColors[3 + index_X4] = m_fontColorGradient.topRight;
                        }

						m_vertColors[0 + index_X4].a = vertexColor.a;
						m_vertColors[1 + index_X4].a = vertexColor.a;
						m_vertColors[2 + index_X4].a = vertexColor.a;
						m_vertColors[3 + index_X4].a = vertexColor.a;
                    }
                    #endregion Handle Vertex Colors


                    // Apply style_padding only if this is a SDF Shader.
                    if (!m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal))
                        style_padding = 0;


                    // Setup UVs for the Mesh
                    #region Setup UVs
                    Vector2 uv0 = new Vector2((m_cached_GlyphInfo.x - m_padding - style_padding) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasWidth, 1 - (m_cached_GlyphInfo.y + m_padding + style_padding + m_cached_GlyphInfo.height) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasHeight);  // bottom left
                    Vector2 uv1 = new Vector2(uv0.x, 1 - (m_cached_GlyphInfo.y - m_padding - style_padding) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasHeight);  // top left
                    Vector2 uv2 = new Vector2((m_cached_GlyphInfo.x + m_padding + style_padding + m_cached_GlyphInfo.width) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasWidth, uv0.y); // bottom right
                    Vector2 uv3 = new Vector2(uv2.x, uv1.y); // top right

                    m_uvs[0 + index_X4] = uv0;
                    m_uvs[1 + index_X4] = uv1;
                    m_uvs[2 + index_X4] = uv2;
                    m_uvs[3 + index_X4] = uv3;
                    #endregion Setup UVs


                    // Determine the bounds of the Mesh.                       
                    //m_meshExtents.min = new Vector2(Mathf.Min(m_meshExtents.min.x, bottom_left.x), Mathf.Min(m_meshExtents.min.y, bottom_left.y));
                    //m_meshExtents.max = new Vector2(Mathf.Max(m_meshExtents.max.x, top_right.x), Mathf.Max(m_meshExtents.max.y, top_left.y));


                    m_visibleCharacterCount += 1;
                    if (m_textInfo.characterInfo[m_characterCount].isVisible)
                        m_lastVisibleCharacterOfLine = m_characterCount;
                }
                else
                {   // This is a Space, Tab, LineFeed or Carriage Return              
                    
                    // Track # of spaces per line which is used for line justification.
                    if (charCode == 9 || charCode == 32)
                    {                      
                        m_textInfo.lineInfo[m_lineNumber].spaceCount += 1;
                        m_textInfo.spaceCount += 1;
                    }
                }
                #endregion Handle Visible Characters


                // Store Rectangle positions for each Character.                
                #region Store Character Data             
                m_textInfo.characterInfo[m_characterCount].lineNumber = (short)m_lineNumber;
                m_textInfo.characterInfo[m_characterCount].pageNumber = (short)m_pageNumber;
                //m_textInfo.lineInfo[m_lineNumber].characterCount += 1;
                if (charCode != 10 && charCode != 13 && charCode != 8230 || m_textInfo.lineInfo[m_lineNumber].characterCount == 1)
                    m_textInfo.lineInfo[m_lineNumber].alignment = m_lineJustification;                            
                #endregion Store Character Data


                // Check if text Exceeds the vertical bounds of the margin area.
                #region Check Vertical Bounds & Auto-Sizing
                if (m_maxAscender - descender + (m_alignmentPadding.w * 2 * m_fontScale) > marginHeight && !m_textContainer.isDefaultHeight)
                {
                    //Debug.Log((m_maxAscender - m_maxDescender) + "  " + marginHeight);
                    //Debug.Log("Character [" + (char)charCode + "] at Index: " + m_characterCount + " has exceeded the Height of the text container. Max Ascender: " + m_maxAscender + "  Max Descender: " + m_maxDescender + "  Margin Height: " + marginHeight + " Bottom Left: " + bottom_left.y);                                              

                    // Handle Linespacing adjustments
                    #region Line Spacing Adjustments
                    if (m_enableAutoSizing && m_lineSpacingDelta > m_lineSpacingMax)
                    {
                        m_lineSpacingDelta -= 1;
                        GenerateTextMesh();
                        return;
                    }
                    #endregion


                    // Handle Text Auto-sizing resulting from text exceeding vertical bounds.
                    #region Text Auto-Sizing (Text greater than verical bounds)
                    if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
                    {
                        m_maxFontSize = m_fontSize;

                        m_fontSize -= Mathf.Max((m_fontSize - m_minFontSize) / 2, 0.05f);
                        m_fontSize = (int)(Mathf.Max(m_fontSize, m_fontSizeMin) * 20 + 0.5f) / 20f;

                        m_recursiveCount = 0;
                        if (loopCountA > 20) return; // Added to debug 
                        GenerateTextMesh();
                        return;
                    }
                    #endregion Text Auto-Sizing

                    // Handle Text Overflow
                    #region Text Overflow
                    switch (m_overflowMode)
                    {
                        case TextOverflowModes.Overflow:
                            if (m_isMaskingEnabled)
                                DisableMasking();

                            break;
                        case TextOverflowModes.Ellipsis:
                            if (m_isMaskingEnabled)
                                DisableMasking();

                            if (m_lineNumber > 0)
                            {
                                m_char_buffer[m_textInfo.characterInfo[ellipsisIndex].index] = 8230;
                                m_char_buffer[m_textInfo.characterInfo[ellipsisIndex].index + 1] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }
                            else
                            {
                                m_char_buffer[0] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }
                        case TextOverflowModes.Masking:
                            if (!m_isMaskingEnabled)
                                EnableMasking();
                            break;
                        case TextOverflowModes.ScrollRect:
                            if (!m_isMaskingEnabled)
                                EnableMasking();
                            break;
                        case TextOverflowModes.Truncate:
                             if (m_isMaskingEnabled)
                                DisableMasking();

                            // TODO : Optimize 
                            if (m_lineNumber > 0)
                            {
                                m_char_buffer[m_textInfo.characterInfo[ellipsisIndex].index + 1] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }
                            else
                            {
                                m_char_buffer[0] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }    
                        case TextOverflowModes.Page:
                            if (m_isMaskingEnabled)
                                DisableMasking();

                            // Ignore Page Break on Linefeed or carriage return
                            if (charCode == 13 || charCode == 10)
                                break;

                            //Debug.Log("Character is [" + (char)charCode + "] with ASCII (" + charCode + ") on Page " + m_pageNumber);

                            // Go back to previous line and re-layout 
                            i = RestoreWordWrappingState(ref m_SavedLineState);
                            if (i == 0)
                            {
                                m_char_buffer[0] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }

                            m_isNewPage = true;
                            m_xAdvance = 0 + m_indent;
                            m_lineOffset = 0;
                            m_lineNumber += 1;
                            m_pageNumber += 1;
                            continue;
                    }
                    #endregion End Text Overflow

                }
                #endregion Check Vertical Bounds


                // Handle XAdvance, Tabulation Stops. Tab stops at every 25% of Font Size.
                #region XAdvance, Tabulation & Stops
                if (charCode == 9)
                {
                    m_xAdvance += m_fontAsset.fontInfo.TabWidth * m_fontScale * m_fontAsset.TabSize;                 
                }
                else if (m_monoSpacing != 0)                                   
                    m_xAdvance += ((m_monoSpacing + m_cached_GlyphInfo.width / 2 + m_cached_GlyphInfo.xOffset) + m_characterSpacing + m_cSpacing) * m_fontScale;                                
                else
                    m_xAdvance += (m_cached_GlyphInfo.xAdvance * xadvance_multiplier * m_fontScale) + (m_characterSpacing + m_cSpacing) * m_fontScale;

                // Store xAdvance information
                m_textInfo.characterInfo[m_characterCount].xAdvance = m_xAdvance;
               
                #endregion Tabulation & Stops
                           
                // Handle Carriage Return                          
                #region Carriage Return
                if (charCode == 13)
                {
                    m_maxXAdvance = Mathf.Max(m_maxXAdvance, m_preferredWidth + m_xAdvance + (m_alignmentPadding.z * m_fontScale));
                    m_preferredWidth = 0;
                    m_xAdvance = 0 + m_indent;
                }
                #endregion Carriage Return
                

                // Handle Line Spacing Adjustments + Word Wrapping & special case for last line.
                #region Check for Line Feed and Last Character
                if (charCode == 10 || m_characterCount == totalCharacterCount - 1)
                {
                    //Debug.Log("Line # " + m_lineNumber + " Last Character of Line is [" + m_textInfo.characterInfo[m_characterCount].character + "]. Last Visible Character is [" + m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].character + "].");

                    if (m_lineNumber > 0 && m_maxFontScale != 0 && m_lineHeight == 0 && m_maxFontScale != previousFontScale && !m_isNewPage) // && !isLineOffsetAdjusted)
                    {
                        float gap = m_fontAssetArray[m_fontIndex].fontInfo.LineHeight - (m_fontAssetArray[m_fontIndex].fontInfo.Ascender - m_fontAssetArray[m_fontIndex].fontInfo.Descender);
                        float offsetDelta = (m_fontAssetArray[m_fontIndex].fontInfo.Ascender + gap + m_lineSpacing + m_paragraphSpacing + m_lineSpacingDelta) * m_maxFontScale - (m_fontAssetArray[m_fontIndex].fontInfo.Descender - gap) * previousFontScale;                                                                                       
                        m_lineOffset += offsetDelta - lineOffsetDelta;                       
                        AdjustLineOffset(m_firstVisibleCharacterOfLine, m_characterCount, offsetDelta - lineOffsetDelta);                  
                    }
                    m_isNewPage = false;
                    
                    // Calculate lineAscender & make sure if last character is superscript or subscript that we check that as well.
                    float lineAscender = (m_fontAsset.fontInfo.Ascender + m_alignmentPadding.y) * m_maxFontScale - m_lineOffset;
                    float lineAscender2 = (m_fontAsset.fontInfo.Ascender + m_alignmentPadding.y) * m_fontScale - m_lineOffset + m_baselineOffset;
                    lineAscender = lineAscender > lineAscender2 ? lineAscender : lineAscender2;

                    // Calculate lineDescender & make sure if last character is superscript or subscript that we check that as well.
                    float lineDescender = (m_fontAsset.fontInfo.Descender + m_alignmentPadding.w) * m_maxFontScale - m_lineOffset;
                    float lineDescender2 = (m_fontAsset.fontInfo.Descender + m_alignmentPadding.w) * m_fontScale - m_lineOffset + m_baselineOffset;
                    lineDescender = lineDescender < lineDescender2 ? lineDescender : lineDescender2;

                    // Update maxDescender if first character of line is visible.
                    //if (m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].isVisible)
                    m_maxDescender = m_maxDescender < lineDescender ? m_maxDescender : lineDescender;

                                                                                                  
                    // Save Line Information
                    m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = m_firstVisibleCharacterOfLine; // Need new variable to track this
                    m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = m_characterCount;                                       
                    m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex = m_lastVisibleCharacterOfLine;
                    m_textInfo.lineInfo[m_lineNumber].characterCount = m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex - m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex + 1;
                  
                    m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_firstVisibleCharacterOfLine].bottomLeft.x, lineDescender);
                    m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].topRight.x, lineAscender);
                    m_textInfo.lineInfo[m_lineNumber].lineLength = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - (m_padding * m_maxFontScale);
                    m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - m_characterSpacing * m_fontScale;

                    m_firstVisibleCharacterOfLine = m_characterCount + 1;


                    // Store PreferredWidth paying attention to linefeed and last character of text.
                    if (charCode == 10 && m_characterCount != totalCharacterCount - 1)
                    {
                        m_maxXAdvance = Mathf.Max(m_maxXAdvance, m_preferredWidth + m_xAdvance + (m_alignmentPadding.z * m_fontScale));
                        m_preferredWidth = 0;
                    }
                    else
                        m_preferredWidth = Mathf.Max(m_maxXAdvance, m_preferredWidth + m_xAdvance + (m_alignmentPadding.z * m_fontScale));
                  
                    m_preferredHeight = m_maxAscender - m_maxDescender;
                   


                    //Debug.Log("LineInfo for line # " + (m_lineNumber) + " First character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex +
                    //                                                    " Last character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex +
                    //                                                    " Last Visible character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex +
                    //                                                    " Character Count of " + m_textInfo.lineInfo[m_lineNumber].characterCount + " Line Lenght of " + m_textInfo.lineInfo[m_lineNumber].lineLength +
                    //                                                    " Extent MinX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.x + "  MinY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.y +
                    //                                                    "  MaxX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x + "  MaxY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.y +
                    //                                                    "  Line Ascender: " + lineAscender + "  Line Descender: " + lineDescender +
                    //                                                    "  Line Max: " + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex].topRight.x );

                   
                    // Add new line if not last lines or character.
                    if (charCode == 10)
                    {
                        // Store the state of the line before starting on the new line.
                        SaveWordWrappingState(ref m_SavedLineState, i, m_characterCount);
                        // Store the state of the last Character before the new line.
                        SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
                        
                        m_lineNumber += 1;
                        // Check to make sure Array is large enough to hold a new line.
                        if (m_lineNumber >= m_textInfo.lineInfo.Length)
                            ResizeLineExtents(m_lineNumber);

                        // Apply Line Spacing based on scale of the last character of the line.                  
                        if (m_lineHeight == 0)
                        {
                            //Debug.Log("Line # " + m_lineHeight + " - Font Scale is " + m_fontScale + "  Max Font Scale is " + m_maxFontScale);
                            lineOffsetDelta = (m_fontAssetArray[m_fontIndex].fontInfo.LineHeight + m_paragraphSpacing + m_lineSpacing + m_lineSpacingDelta) * m_fontScale;
                            m_lineOffset += lineOffsetDelta;
                        }
                        else
                            m_lineOffset += (m_lineHeight + m_paragraphSpacing + m_lineSpacing) * baseScale;

                        previousFontScale = m_fontScale;
                        m_maxFontScale = 0;                       
                        m_xAdvance = 0 + m_indent;

                        ellipsisIndex = m_characterCount - 1;
                    }
                }
                #endregion Check for Linefeed or Last Character

            
                //// Store Rectangle positions for each Character. 
                #region Save CharacterInfo for the current character.
                m_textInfo.characterInfo[m_characterCount].baseLine = m_textInfo.characterInfo[m_characterCount].topRight.y - (m_cached_GlyphInfo.yOffset + m_padding) * m_fontScale;
                m_textInfo.characterInfo[m_characterCount].topLine = m_textInfo.characterInfo[m_characterCount].baseLine + (m_fontAssetArray[m_fontIndex].fontInfo.Ascender + m_alignmentPadding.y) * m_fontScale; // Ascender              
                m_textInfo.characterInfo[m_characterCount].bottomLine = m_textInfo.characterInfo[m_characterCount].baseLine + (m_fontAssetArray[m_fontIndex].fontInfo.Descender - m_alignmentPadding.w) * m_fontScale; // Descender          
                m_textInfo.characterInfo[m_characterCount].padding = m_padding * m_fontScale;
                m_textInfo.characterInfo[m_characterCount].aspectRatio = m_cached_GlyphInfo.width / m_cached_GlyphInfo.height;
                m_textInfo.characterInfo[m_characterCount].scale = m_fontScale;


                // Determine the bounds of the Mesh.                       
                if (m_textInfo.characterInfo[m_characterCount].isVisible)
                {
                    m_meshExtents.min = new Vector2(Mathf.Min(m_meshExtents.min.x, m_textInfo.characterInfo[m_characterCount].bottomLeft.x), Mathf.Min(m_meshExtents.min.y, m_textInfo.characterInfo[m_characterCount].bottomLeft.y));
                    m_meshExtents.max = new Vector2(Mathf.Max(m_meshExtents.max.x, m_textInfo.characterInfo[m_characterCount].topRight.x), Mathf.Max(m_meshExtents.max.y, m_textInfo.characterInfo[m_characterCount].topLeft.y));
                }
                
                // Save pageInfo Data                                                                       
                if (charCode != 13 && charCode != 10 && m_pageNumber < 16)
                {
                    m_textInfo.pageInfo[m_pageNumber].ascender = pageAscender;
                    m_textInfo.pageInfo[m_pageNumber].descender = descender < m_textInfo.pageInfo[m_pageNumber].descender ? descender : m_textInfo.pageInfo[m_pageNumber].descender;
                    //Debug.Log("Char [" + (char)charCode + "] with ASCII (" + charCode + ") on Page # " + m_pageNumber + " with Ascender: " + m_textInfo.pageInfo[m_pageNumber].ascender + ". Descender: " + m_textInfo.pageInfo[m_pageNumber].descender);                   
                    
                    if (m_pageNumber == 0 && m_characterCount == 0)
                        m_textInfo.pageInfo[m_pageNumber].firstCharacterIndex = m_characterCount;
                    else if (m_characterCount > 0 && m_pageNumber != m_textInfo.characterInfo[m_characterCount - 1].pageNumber)
                    {
                        m_textInfo.pageInfo[m_pageNumber - 1].lastCharacterIndex = m_characterCount - 1;
                        m_textInfo.pageInfo[m_pageNumber].firstCharacterIndex = m_characterCount;
                    }
                    else if (m_characterCount == totalCharacterCount - 1)
                        m_textInfo.pageInfo[m_pageNumber].lastCharacterIndex = m_characterCount;       
                }       
                #endregion Saving CharacterInfo

              
                // Save State of the last character
                #region Save Last Character State
                //if (m_overflowMode == TextOverflowModes.Ellipsis)
                //{
                    //Debug.Log("Char [" + (char)charCode + "] at Index " + (m_characterCount % 5));
                //    SaveWordWrappingState(ref m_SavedLastCharState, i);
                //    m_SavedCharacterStates[m_characterCount % 5] = m_SavedLastCharState;
                //}
                #endregion End Last Character State


                // Save State of Mesh Creation forhandling of Word Wrapping
                #region Save Word Wrapping State
                if (m_enableWordWrapping || m_overflowMode == TextOverflowModes.Truncate || m_overflowMode == TextOverflowModes.Ellipsis)
                {

                    if (charCode == 9 || charCode == 32)
                    {
                        // We store the state of numerous variables for the most recent Space, LineFeed or Carriage Return to enable them to be restored 
                        // for Word Wrapping.                      
                        SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
                        m_isCharacterWrappingEnabled = false;
                        isFirstWord = false;
                        //Debug.Log("Storing Word Wrapping Info at CharacterCount " + m_characterCount);             
                    }
                    else if ((isFirstWord || m_isCharacterWrappingEnabled == true) && m_characterCount < totalCharacterCount - 1
                        && m_fontAsset.lineBreakingInfo.leadingCharacters.ContainsKey(charCode) == false
                        && m_fontAsset.lineBreakingInfo.followingCharacters.ContainsKey(m_VisibleCharacters[m_characterCount + 1]) == false
                        || isLastBreakingChar)
                        //|| m_characterCount == m_firstVisibleCharacterOfLine)
                    {
                        //Debug.Log("Storing Character [" + (char)charCode + "] at Index: " + i);
                        SaveWordWrappingState(ref m_SavedWordWrapState, i, m_characterCount);
                    }
                }
                #endregion Save Word Wrapping State

                m_characterCount += 1;               
            }
          

            // Check Auto Sizing and increase font size to fill text container.
            #region Check Auto-Sizing (Upper Font Size Bounds)
            fontSizeDelta = m_maxFontSize - m_minFontSize;                                           
            if ((!m_textContainer.isDefaultWidth || !m_textContainer.isDefaultHeight) && !m_isCharacterWrappingEnabled && (m_enableAutoSizing && fontSizeDelta > 0.051f && m_fontSize < m_fontSizeMax))
            {
                m_minFontSize = m_fontSize;
                m_fontSize += Mathf.Max((m_maxFontSize - m_fontSize) / 2, 0.05f);
                m_fontSize = (int)(Mathf.Min(m_fontSize, m_fontSizeMax) * 20 + 0.5f) / 20f;

                if (loopCountA > 20) return; // Added to debug    
                GenerateTextMesh();
                return;
            }
            #endregion End Auto-sizing Check


          
            // Add Termination Character to textMeshCharacterInfo which is used by the Advanced Layout Component.     
            if (m_characterCount < m_textInfo.characterInfo.Length)
                m_textInfo.characterInfo[m_characterCount].character = (char)0;

            m_isCharacterWrappingEnabled = false;
                   
            if (m_renderMode == TextRenderFlags.GetPreferredSizes)
                return;
            
            // DEBUG & PERFORMANCE CHECKS (0.006ms)                    
            //Debug.Log("Iteration Count: " + loopCountA + ". Final Point Size: " + m_fontSize);
            //for (int i = 0; i < m_lineNumber + 1; i++ )
            //{
            //    Debug.Log("Line: " + (i + 1) + "  # Char: " + m_textInfo.lineInfo[i].characterCount
            //                                 + "  Word Count: " + m_textInfo.lineInfo[i].wordCount
            //                                 + "  Space: " + m_textInfo.lineInfo[i].spaceCount
            //                                 + "  First: [" + m_textInfo.characterInfo[m_textInfo.lineInfo[i].firstCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[i].firstCharacterIndex
            //                                 + "  Last [" + m_textInfo.characterInfo[m_textInfo.lineInfo[i].lastCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[i].lastCharacterIndex
            //                                 + "  Length: " + m_textInfo.lineInfo[i].lineLength
            //                                 + "  Line Extents: " + m_textInfo.lineInfo[i].lineExtents);         
            //}



                // If there are no visible characters... no need to continue
                if (m_visibleCharacterCount == 0)
                {
                    if (m_vertices != null)
                    {
                        Array.Clear(m_vertices, 0, m_vertices.Length);
                        m_mesh.vertices = m_vertices;
                    }
                    return;
                }

            
            int last_vert_index = m_visibleCharacterCount * 4;
            // Partial clear of the vertices array to mark unused vertices as degenerate.
            Array.Clear(m_vertices, last_vert_index, m_vertices.Length - last_vert_index);


            // Handle Text Alignment
            #region Text Alignment
            switch (m_textAlignment)
            {
                // Top Vertically
                case TextAlignmentOptions.Top:
                case TextAlignmentOptions.TopLeft:
                case TextAlignmentOptions.TopJustified:
                case TextAlignmentOptions.TopRight:
                    if (m_overflowMode != TextOverflowModes.Page)
                        m_anchorOffset = corners[1] + new Vector3(0 + margins.x, 0 - m_maxAscender - margins.y, 0);
                    else
                        m_anchorOffset = corners[1] + new Vector3(0 + margins.x, 0 - m_textInfo.pageInfo[m_pageToDisplay].ascender - margins.y, 0);                     
                    break;    
                          
                // Middle Vertically
                case TextAlignmentOptions.Left:
                case TextAlignmentOptions.Right:
                case TextAlignmentOptions.Center:               
                case TextAlignmentOptions.Justified:
                    if (m_overflowMode != TextOverflowModes.Page)
                        m_anchorOffset = (corners[0] + corners[1]) / 2 + new Vector3(0 + margins.x, 0 - (m_maxAscender + margins.y + m_maxDescender - margins.w) / 2, 0);
                    else                                         
                        m_anchorOffset = (corners[0] + corners[1]) / 2 + new Vector3(0 + margins.x, 0 - (m_textInfo.pageInfo[m_pageToDisplay].ascender + margins.y + m_textInfo.pageInfo[m_pageToDisplay].descender - margins.w) / 2, 0);                  
                    break;
                
                // Bottom Vertically
                case TextAlignmentOptions.Bottom:
                case TextAlignmentOptions.BottomLeft:
                case TextAlignmentOptions.BottomRight:
                case TextAlignmentOptions.BottomJustified:
                    if (m_overflowMode != TextOverflowModes.Page)    
                        m_anchorOffset = corners[0] + new Vector3(0 + margins.x, 0 - m_maxDescender + margins.w, 0);
                    else                 
                        m_anchorOffset = corners[0] + new Vector3(0 + margins.x, 0 - m_textInfo.pageInfo[m_pageToDisplay].descender + margins.w, 0);                
                    break;

                // Baseline Vertically 
                case TextAlignmentOptions.BaselineLeft:
                case TextAlignmentOptions.BaselineRight:
                case TextAlignmentOptions.Baseline:
                    m_anchorOffset = (corners[0] + corners[1]) / 2 + new Vector3(0 + margins.x, 0, 0);
                    break;

                // Midline Vertically 
                case TextAlignmentOptions.MidlineLeft:
                case TextAlignmentOptions.Midline:
                case TextAlignmentOptions.MidlineRight:
                case TextAlignmentOptions.MidlineJustified:
                    m_anchorOffset = (corners[0] + corners[1]) / 2 + new Vector3(0 + margins.x, 0 - (m_meshExtents.max.y + margins.y + m_meshExtents.min.y - margins.w) / 2, 0);
                    break;
            }
            #endregion


            // Handling of Anchor Dampening
            //float currentMeshWidth = m_meshExtents.max.x - m_meshExtents.min.x;         
            //if (m_anchorDampening)
            //{               
            //    float delta = currentMeshWidth - m_baseDampeningWidth;
            //    if (m_baseDampeningWidth != 0 && Mathf.Abs(delta) < m_cached_Underline_GlyphInfo.width * m_fontScale * 0.6f)                
            //        m_anchorOffset.x += delta / 2;                               
            //    else                                  
            //        m_baseDampeningWidth = currentMeshWidth;           
            //}

           
            Vector3 justificationOffset = Vector3.zero;
            Vector3 offset = Vector3.zero;
            int vert_index_X4 = 0;
            int wordCount = 0;
            int lineCount = 0;
            int lastLine = 0;
            Color32 underlineColor = new Color32(255, 255, 255, 127);

            bool isStartOfWord = false;          
            int wordFirstChar = 0;
            int wordLastChar = 0;


            // Second Pass : Line Justification, UV Mapping, Character & Line Visibility & more.
            #region Handle Line Justification & UV Mapping & Character Visibility & More
            for (int i = 0; i < m_characterCount; i++)
            {
                int currentLine = m_textInfo.characterInfo[i].lineNumber;

                char currentCharacter = m_textInfo.characterInfo[i].character;
                TMP_LineInfo lineInfo = m_textInfo.lineInfo[currentLine];
                TextAlignmentOptions lineAlignment = lineInfo.alignment;
                lineCount = currentLine + 1;              


                // Process Line Justification
                #region Handle Line Justification
                switch (lineAlignment)
                {
                    case TextAlignmentOptions.TopLeft:
                    case TextAlignmentOptions.Left:
                    case TextAlignmentOptions.BottomLeft:
                    case TextAlignmentOptions.BaselineLeft:
                    case TextAlignmentOptions.MidlineLeft:
                        justificationOffset = Vector3.zero;
                       
                        break;
                    case TextAlignmentOptions.Top:
                    case TextAlignmentOptions.Center:
                    case TextAlignmentOptions.Bottom:
                    case TextAlignmentOptions.Baseline:
                    case TextAlignmentOptions.Midline:
                        justificationOffset = new Vector3(m_marginWidth / 2 - lineInfo.maxAdvance / 2, 0, 0);
                       
                        break;
                    case TextAlignmentOptions.TopRight:
                    case TextAlignmentOptions.Right:
                    case TextAlignmentOptions.BottomRight:
                    case TextAlignmentOptions.BaselineRight: 
                    case TextAlignmentOptions.MidlineRight:
                        justificationOffset = new Vector3(m_marginWidth - lineInfo.maxAdvance, 0, 0);
                        break;
                    case TextAlignmentOptions.TopJustified:
                    case TextAlignmentOptions.Justified:
                    case TextAlignmentOptions.BottomJustified:
                    case TextAlignmentOptions.MidlineJustified:
                        charCode = m_textInfo.characterInfo[i].character;
                        char lastCharOfCurrentLine = m_textInfo.characterInfo[lineInfo.lastCharacterIndex].character;
                                  
                        if (char.IsWhiteSpace(lastCharOfCurrentLine) && !char.IsControl(lastCharOfCurrentLine) && currentLine < m_lineNumber)
                        {   // All lines are justified accept the last one.
                            float gap = (corners[3].x - margins.z) - (corners[0].x + margins.x) - (lineInfo.maxAdvance);
                          
                            if (currentLine != lastLine || i == 0)
                            {                              
                                justificationOffset = Vector3.zero;
                            }
                            else
                            {                                                          
                                if (charCode == 9 || charCode == 32)
                                {
                                    justificationOffset += new Vector3(gap * (1 - m_wordWrappingRatios) / (lineInfo.spaceCount - 1), 0, 0);
                                }
                                else
                                {                                   
                                    justificationOffset += new Vector3(gap * m_wordWrappingRatios / (lineInfo.characterCount - lineInfo.spaceCount - 1), 0, 0);
                                }                            
                            }
                        }
                        else
                            justificationOffset = Vector3.zero; // Keep last line left justified.

                        //Debug.Log("Char [" + (char)charCode + "] Code: " + charCode + "  Offset: " + justificationOffset + "  # Spaces: " + lineInfo.spaceCount + "  # Characters: " + lineInfo.characterCount + "  CurrentLine: " + currentLine + "  Last Line: " + lastLine + "  i: " + i);                      
                        break;
                }
                #endregion End Text Justification
              
                offset = m_anchorOffset + justificationOffset;

                if (m_textInfo.characterInfo[i].isVisible)
                {
                    Extents lineExtents = lineInfo.lineExtents;
                    float uvOffset = (m_uvLineOffset * currentLine) % 1 + m_uvOffset.x;
                    

                    // Setup UV2 based on Character Mapping Options Selected
                    #region Handle UV Mapping Options
                    switch (m_horizontalMapping) 
                    {
                        case TextureMappingOptions.Character:
                            m_uv2s[vert_index_X4 + 0].x = 0 + m_uvOffset.x;
                            m_uv2s[vert_index_X4 + 1].x = 0 + m_uvOffset.x;
                            m_uv2s[vert_index_X4 + 2].x = 1 + m_uvOffset.x;
                            m_uv2s[vert_index_X4 + 3].x = 1 + m_uvOffset.x;
                            break;

                        case TextureMappingOptions.Line:                          
                            if (m_textAlignment != TextAlignmentOptions.Justified)
                            {                               
                                m_uv2s[vert_index_X4 + 0].x = (m_vertices[vert_index_X4 + 0].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                m_uv2s[vert_index_X4 + 1].x = (m_vertices[vert_index_X4 + 1].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                m_uv2s[vert_index_X4 + 2].x = (m_vertices[vert_index_X4 + 2].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                m_uv2s[vert_index_X4 + 3].x = (m_vertices[vert_index_X4 + 3].x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                break;
                            }
                            else // Special Case if Justified is used in Line Mode.
                            {
                                m_uv2s[vert_index_X4 + 0].x = (m_vertices[vert_index_X4 + 0].x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                m_uv2s[vert_index_X4 + 1].x = (m_vertices[vert_index_X4 + 1].x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                m_uv2s[vert_index_X4 + 2].x = (m_vertices[vert_index_X4 + 2].x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                m_uv2s[vert_index_X4 + 3].x = (m_vertices[vert_index_X4 + 3].x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                break;
                            }

                        case TextureMappingOptions.Paragraph:                         
                            m_uv2s[vert_index_X4 + 0].x = (m_vertices[vert_index_X4 + 0].x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                            m_uv2s[vert_index_X4 + 1].x = (m_vertices[vert_index_X4 + 1].x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                            m_uv2s[vert_index_X4 + 2].x = (m_vertices[vert_index_X4 + 2].x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                            m_uv2s[vert_index_X4 + 3].x = (m_vertices[vert_index_X4 + 3].x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                            break;

                        case TextureMappingOptions.MatchAspect:

                            switch (m_verticalMapping)
                            {
                                case TextureMappingOptions.Character:
                                    m_uv2s[vert_index_X4 + 0].y = 0 + m_uvOffset.y;
                                    m_uv2s[vert_index_X4 + 1].y = 1 + m_uvOffset.y;
                                    m_uv2s[vert_index_X4 + 2].y = 0 + m_uvOffset.y;
                                    m_uv2s[vert_index_X4 + 3].y = 1 + m_uvOffset.y;
                                    break;

                                case TextureMappingOptions.Line:                                  
                                    m_uv2s[vert_index_X4 + 0].y = (m_vertices[vert_index_X4].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + uvOffset;
                                    m_uv2s[vert_index_X4 + 1].y = (m_vertices[vert_index_X4 + 1].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + uvOffset;
                                    m_uv2s[vert_index_X4 + 2].y = m_uv2s[vert_index_X4].y;
                                    m_uv2s[vert_index_X4 + 3].y = m_uv2s[vert_index_X4 + 1].y;
                                    break;

                                case TextureMappingOptions.Paragraph:                                   
                                    m_uv2s[vert_index_X4 + 0].y = (m_vertices[vert_index_X4].y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + uvOffset;
                                    m_uv2s[vert_index_X4 + 1].y = (m_vertices[vert_index_X4 + 1].y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + uvOffset;
                                    m_uv2s[vert_index_X4 + 2].y = m_uv2s[vert_index_X4].y;
                                    m_uv2s[vert_index_X4 + 3].y = m_uv2s[vert_index_X4 + 1].y;
                                    break;

                                case TextureMappingOptions.MatchAspect:
                                    Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
                                    break;
                            }

                            //float xDelta = 1 - (_uv2s[vert_index + 0].y * textMeshCharacterInfo[i].AspectRatio); // Left aligned
                            float xDelta = (1 - ((m_uv2s[vert_index_X4 + 0].y + m_uv2s[vert_index_X4 + 1].y) * m_textInfo.characterInfo[i].aspectRatio)) / 2; // Center of Rectangle
                            //float xDelta = 0;                           

                            m_uv2s[vert_index_X4 + 0].x = (m_uv2s[vert_index_X4 + 0].y * m_textInfo.characterInfo[i].aspectRatio) + xDelta + uvOffset;
                            m_uv2s[vert_index_X4 + 1].x = m_uv2s[vert_index_X4 + 0].x;
                            m_uv2s[vert_index_X4 + 2].x = (m_uv2s[vert_index_X4 + 1].y * m_textInfo.characterInfo[i].aspectRatio) + xDelta + uvOffset;
                            m_uv2s[vert_index_X4 + 3].x = m_uv2s[vert_index_X4 + 2].x;
                            break;
                    }

                    switch (m_verticalMapping)
                    {
                        case TextureMappingOptions.Character:
                            m_uv2s[vert_index_X4 + 0].y = 0 + m_uvOffset.y;
                            m_uv2s[vert_index_X4 + 1].y = 1 + m_uvOffset.y;
                            m_uv2s[vert_index_X4 + 2].y = 0 + m_uvOffset.y;
                            m_uv2s[vert_index_X4 + 3].y = 1 + m_uvOffset.y;
                            break;

                        case TextureMappingOptions.Line:
                            m_uv2s[vert_index_X4 + 0].y = (m_vertices[vert_index_X4].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + m_uvOffset.y;
                            m_uv2s[vert_index_X4 + 1].y = (m_vertices[vert_index_X4 + 1].y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + m_uvOffset.y;
                            m_uv2s[vert_index_X4 + 2].y = m_uv2s[vert_index_X4].y;
                            m_uv2s[vert_index_X4 + 3].y = m_uv2s[vert_index_X4 + 1].y;
                            break;

                        case TextureMappingOptions.Paragraph:
                            m_uv2s[vert_index_X4 + 0].y = (m_vertices[vert_index_X4].y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
                            m_uv2s[vert_index_X4 + 1].y = (m_vertices[vert_index_X4 + 1].y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
                            m_uv2s[vert_index_X4 + 2].y = m_uv2s[vert_index_X4].y;
                            m_uv2s[vert_index_X4 + 3].y = m_uv2s[vert_index_X4 + 1].y;
                            break;

                        case TextureMappingOptions.MatchAspect:
                            //float yDelta = 1 - (_uv2s[vert_index + 2].x / textMeshCharacterInfo[i].AspectRatio); // Top Corner                       
                            float yDelta = (1 - ((m_uv2s[vert_index_X4 + 0].x + m_uv2s[vert_index_X4 + 2].x) / m_textInfo.characterInfo[i].aspectRatio)) / 2; // Center of Rectangle
                            //float yDelta = 0;

                            m_uv2s[vert_index_X4 + 0].y = yDelta + (m_uv2s[vert_index_X4 + 0].x / m_textInfo.characterInfo[i].aspectRatio) + m_uvOffset.y;
                            m_uv2s[vert_index_X4 + 1].y = yDelta + (m_uv2s[vert_index_X4 + 2].x / m_textInfo.characterInfo[i].aspectRatio) + m_uvOffset.y;
                            m_uv2s[vert_index_X4 + 2].y = m_uv2s[vert_index_X4 + 0].y;
                            m_uv2s[vert_index_X4 + 3].y = m_uv2s[vert_index_X4 + 1].y;                           
                            break;
                    }
                    #endregion 


                    // Pack UV's so that we can pass Xscale needed for Shader to maintain 1:1 ratio.
                    float xScale = m_textInfo.characterInfo[i].scale * lossyScale;
                    if ((m_textInfo.characterInfo[i].style & FontStyles.Bold) == FontStyles.Bold) xScale *= -1;

                    float x0 = m_uv2s[vert_index_X4 + 0].x;
                    float y0 = m_uv2s[vert_index_X4 + 0].y;
                    float x1 = m_uv2s[vert_index_X4 + 3].x;
                    float y1 = m_uv2s[vert_index_X4 + 3].y;

                    float dx = Mathf.Floor(x0);
                    float dy = Mathf.Floor(y0);

                    x0 = x0 - dx;
                    x1 = x1 - dx;
                    y0 = y0 - dy;
                    y1 = y1 - dy;

                    m_uv2s[vert_index_X4 + 0] = PackUV(x0, y0, xScale);
                    m_uv2s[vert_index_X4 + 1] = PackUV(x0, y1, xScale);
                    m_uv2s[vert_index_X4 + 2] = PackUV(x1, y0, xScale);
                    m_uv2s[vert_index_X4 + 3] = PackUV(x1, y1, xScale);


                    // Enables control of the visibility of Characters, Lines and Pages.
                    if (m_maxVisibleCharacters != -1 && i >= m_maxVisibleCharacters
                        || m_maxVisibleLines != -1 && currentLine >= m_maxVisibleLines
                        || m_overflowMode == TextOverflowModes.Page && m_textInfo.characterInfo[i].pageNumber != m_pageToDisplay) 

                    {
                        m_vertices[vert_index_X4 + 0] *= 0;
                        m_vertices[vert_index_X4 + 1] *= 0;
                        m_vertices[vert_index_X4 + 2] *= 0;
                        m_vertices[vert_index_X4 + 3] *= 0;
                    }
                    else
                    {
                        m_vertices[vert_index_X4 + 0] += offset;
                        m_vertices[vert_index_X4 + 1] += offset;
                        m_vertices[vert_index_X4 + 2] += offset;
                        m_vertices[vert_index_X4 + 3] += offset;
                    }

                    vert_index_X4 += 4;
                }

                m_textInfo.characterInfo[i].bottomLeft += offset;
                m_textInfo.characterInfo[i].topRight += offset;             
                m_textInfo.characterInfo[i].topLine += offset.y;
                m_textInfo.characterInfo[i].bottomLine += offset.y;
                m_textInfo.characterInfo[i].baseLine += offset.y;


                 // Store Max Ascender & Descender
                m_textInfo.lineInfo[currentLine].ascender = m_textInfo.characterInfo[i].topLine > m_textInfo.lineInfo[currentLine].ascender ? m_textInfo.characterInfo[i].topLine : m_textInfo.lineInfo[currentLine].ascender;
                m_textInfo.lineInfo[currentLine].descender = m_textInfo.characterInfo[i].bottomLine < m_textInfo.lineInfo[currentLine].descender ? m_textInfo.characterInfo[i].bottomLine : m_textInfo.lineInfo[currentLine].descender;


                // Need to recompute lineExtent to account for the offset from justification.
                #region Adjust lineExtents resulting from alignment offset
                if (currentLine != lastLine || i == m_characterCount - 1)
                {
                    // Update the previous line's extents
                    if (currentLine != lastLine)
                    {
                        m_textInfo.lineInfo[lastLine].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lastLine].firstCharacterIndex].bottomLeft.x, m_textInfo.lineInfo[lastLine].descender);
                        m_textInfo.lineInfo[lastLine].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lastLine].lastVisibleCharacterIndex].topRight.x, m_textInfo.lineInfo[lastLine].ascender);
                    }

                    // Update the current line's extents
                    if (i == m_characterCount - 1)
                    {
                        m_textInfo.lineInfo[currentLine].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[currentLine].firstCharacterIndex].bottomLeft.x, m_textInfo.lineInfo[currentLine].descender);
                        m_textInfo.lineInfo[currentLine].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[currentLine].lastVisibleCharacterIndex].topRight.x, m_textInfo.lineInfo[currentLine].ascender);
                    }
                }
                #endregion      

               
                // Track word count and individual words.
                if (char.IsLetterOrDigit(currentCharacter) && i < m_characterCount - 1)
                {
                    if (isStartOfWord == false)
                    {
                        isStartOfWord = true;
                        wordFirstChar = i;
                    }
                }
                else if ((char.IsPunctuation(currentCharacter) || char.IsWhiteSpace(currentCharacter) || i == m_characterCount - 1) && isStartOfWord || i == 0)
                {
                    wordLastChar = i == m_characterCount - 1 && char.IsLetterOrDigit(currentCharacter) ? i : i - 1;
                    isStartOfWord = false;

                    wordCount += 1;
                    m_textInfo.lineInfo[currentLine].wordCount += 1;

                    TMP_WordInfo wordInfo = new TMP_WordInfo();
                    wordInfo.firstCharacterIndex = wordFirstChar;
                    wordInfo.lastCharacterIndex = wordLastChar;
                    wordInfo.characterCount = wordLastChar - wordFirstChar + 1;                                     
                    m_textInfo.wordInfo.Add(wordInfo);                     
                    //Debug.Log("Word #" + wordCount + " is [" + wordInfo.word + "] Start Index: " + wordInfo.firstCharacterIndex + "  End Index: " + wordInfo.lastCharacterIndex);
                }               


                // Setup & Handle Underline
                #region Underline
                // NOTE: Need to figure out how underline will be handled with multiple fonts and which font will be used for the underline.
                bool isUnderline = (m_textInfo.characterInfo[i].style & FontStyles.Underline) == FontStyles.Underline;
                if (isUnderline)
                {
                    if (beginUnderline == false && currentCharacter != 32 && currentCharacter != 10 && currentCharacter != 13)
                    {
                        beginUnderline = true;
                        underline_start = new Vector3(m_textInfo.characterInfo[i].bottomLeft.x, m_textInfo.characterInfo[i].baseLine + font.fontInfo.Underline * m_fontScale, 0);
                        underlineColor = m_textInfo.characterInfo[i].color;
                        //Debug.Log("Underline Start Char [" + m_textInfo.characterInfo[i].character + "].");
                    }

                    // End Underline if text only contains one character.
                    if (m_characterCount == 1)
                    {
                        beginUnderline = false;
                        underline_end = new Vector3(m_textInfo.characterInfo[i].topRight.x, m_textInfo.characterInfo[i].baseLine + font.fontInfo.Underline * m_fontScale, 0);

                        DrawUnderlineMesh(underline_start, underline_end, ref last_vert_index, underlineColor);
                    }
                    else if (i == lineInfo.lastCharacterIndex)
                    {
                        // Terminate underline at previous visible character if space or carriage return.
                        if (currentCharacter == 32 || currentCharacter == 10 || currentCharacter == 13)
                        {
                            int lastVisibleCharacterIndex = lineInfo.lastVisibleCharacterIndex;
                            underline_end = new Vector3(m_textInfo.characterInfo[lastVisibleCharacterIndex].topRight.x, m_textInfo.characterInfo[lastVisibleCharacterIndex].baseLine + font.fontInfo.Underline * m_fontScale, 0);
                        }
                        else
                        {
                            underline_end = new Vector3(m_textInfo.characterInfo[i].topRight.x, m_textInfo.characterInfo[i].baseLine + font.fontInfo.Underline * m_fontScale, 0);
                        }

                        beginUnderline = false;
                        DrawUnderlineMesh(underline_start, underline_end, ref last_vert_index, underlineColor);
                    }
                }
                else
                {
                    // End Underline
                    if (beginUnderline == true)
                    {
                        beginUnderline = false;
                        underline_end = new Vector3(m_textInfo.characterInfo[i - 1].topRight.x, m_textInfo.characterInfo[i - 1].baseLine + font.fontInfo.Underline * m_fontScale, 0);

                        DrawUnderlineMesh(underline_start, underline_end, ref last_vert_index, underlineColor);
                    }
                }
                #endregion


                lastLine = currentLine;
            }
            #endregion


            // METRICS ABOUT THE TEXT OBJECT            
            m_textInfo.characterCount = (short)m_characterCount;
            m_textInfo.lineCount = (short)lineCount;
            m_textInfo.wordCount = wordCount != 0 && m_characterCount > 0 ? (short)wordCount : (short)1;
            m_textInfo.pageCount = m_pageNumber;
            

            // Store Mesh information in MeshInfo
            m_textInfo.meshInfo.vertices = m_vertices;
            m_textInfo.meshInfo.uv0s = m_uvs;
            m_textInfo.meshInfo.uv2s = m_uv2s;
            m_textInfo.meshInfo.vertexColors = m_vertColors;

                   
            // If Advanced Layout Component is present, don't upload the mesh.
            if (m_renderMode == TextRenderFlags.Render)
            {
                //Debug.Log("Uploading Mesh normally.");
                // Upload Mesh Data 
                m_mesh.MarkDynamic();

                m_mesh.vertices = m_vertices;
                m_mesh.uv = m_uvs;
                m_mesh.uv2 = m_uv2s;
                m_mesh.colors32 = m_vertColors;
                                     
                //m_maskOffset = new Vector4(m_mesh.bounds.center.x, m_mesh.bounds.center.y, m_mesh.bounds.size.x, m_mesh.bounds.size.y);
            }
 
            // Setting Mesh Bounds manually is more efficient.   
            //m_mesh.bounds = new Bounds(new Vector3((m_meshExtents.max.x + m_meshExtents.min.x) / 2, (m_meshExtents.max.y + m_meshExtents.min.y) / 2, 0), new Vector3(m_meshExtents.max.x - m_meshExtents.min.x, m_meshExtents.max.y - m_meshExtents.min.y, 0));           
            m_mesh.RecalculateBounds();
            //Vector3 p0 = m_meshExtents.min; // +new Vector2(offset.x, offset.y);
            //Vector3 p1 = new Vector3(m_meshExtents.min.x, m_meshExtents.max.y, 0); // +offset; 
            //Vector3 p2 = m_meshExtents.max; // +new Vector2(offset.x, offset.y); 
            //Vector3 p3 = new Vector3(m_meshExtents.max.x, m_meshExtents.min.y, 0); // +offset; 
            //Debug.DrawLine(p0, p1, Color.green, 60f);
            //Debug.DrawLine(p1, p2, Color.green, 60f);
            //Debug.DrawLine(p2, p3, Color.green, 60f);
            //Debug.DrawLine(p3, p0, Color.green, 60f);

            //m_isCharacterWrappingEnabled = false;
            //Debug.Log("Corners [0] " + corners[0] + "  [1] " + corners[1] + " [2] " + corners[2] + " [3] " + corners[3]);
            //Debug.Log("Done rendering.");          


            // Option to re-size the Text Container to match the text.
            if ((m_textContainer.isDefaultWidth || m_textContainer.isDefaultHeight) && m_textContainer.isAutoFitting)
            {
                //Debug.Log("Auto-fitting Text. Default Width:" + m_textContainer.isDefaultWidth + "  Default Height:" + m_textContainer.isDefaultHeight);
                if (m_textContainer.isDefaultWidth)
                {
                    m_textContainer.width = m_preferredWidth + margins.x + margins.z;
                    //Debug.Log("Text Container's Width adjusted to fit text.");
                }

                if (m_textContainer.isDefaultHeight)
                {
                    //m_textContainer.height = m_maxAscender + margins.y - m_maxDescender + margins.w + m_alignmentPadding.y * m_fontScale * 2;
                    m_textContainer.height = m_preferredHeight + margins.y + margins.w;
                    //Debug.Log("Text Container's Height adjusted to fit text.");
                }
             
                //Debug.Log("Auto-fitting Text. Default Width:" + m_textContainer.width + "  Default Height:" + m_textContainer.height);
                if (m_isMaskingEnabled) isMaskUpdateRequired = true;
                
                // Since changing the size of the Text Container with set the .hasChanged flag, the text object with be regenerated.
                GenerateTextMesh();
                return;
            }



            //for (int i = 0; i < m_lineNumber + 1; i++)
            //{
            //    Debug.Log("Line: " + (i + 1) + "  # Char: " + m_textInfo.lineInfo[i].characterCount
            //                                 + "  Word Count: " + m_textInfo.lineInfo[i].wordCount
            //                                 + "  Space: " + m_textInfo.lineInfo[i].spaceCount
            //                                 + "  First: [" + m_textInfo.characterInfo[m_textInfo.lineInfo[i].firstCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[i].firstCharacterIndex
            //                                 + "  Last [" + m_textInfo.characterInfo[m_textInfo.lineInfo[i].lastCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[i].lastCharacterIndex
            //                                 + "  Last visible [" + m_textInfo.characterInfo[m_textInfo.lineInfo[i].lastVisibleCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[i].lastVisibleCharacterIndex
            //                                 + "  Length: " + m_textInfo.lineInfo[i].lineLength
            //                                 + "  Line Extents: " + m_textInfo.lineInfo[i].lineExtents);
            //}
            

           
            //Profiler.EndSample();
            //m_StopWatch.Stop(); 
            //Debug.Log("Preferred Width: " + m_preferredWidth + "  Height: " + m_preferredHeight); // + "  Margin Width: " + marginWidth + "  xAdvance Total: " + totalxAdvance);
            //Debug.Log("Done Rendering Text Object. Total Character Count is " + m_textInfo.characterCount + ".  Preferred Width: " + m_preferredWidth + "  Height: " + m_preferredHeight);
            //Debug.Log("TimeElapsed is:" + (m_StopWatch.ElapsedTicks / 10000f).ToString("f4"));
            //m_StopWatch.Reset();     
        }



        // Draws the Underline
        void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, Color32 underlineColor)
        {

            int verticesCount = index + 12;          
            // Check to make sure our current mesh buffer allocations can hold these new Quads.  
            if (verticesCount > m_vertices.Length)
            {
                // Resize Mesh Buffers            
                ResizeMeshBuffers(verticesCount / 4 + 12);
            }

            // Adjust the position of the underline based on the lowest character. This matters for subscript character.
            start.y = Mathf.Min(start.y, end.y);
            end.y = Mathf.Min(start.y, end.y);

            float segmentWidth = m_cached_Underline_GlyphInfo.width / 2 * m_fontScale;

            if (end.x - start.x < m_cached_Underline_GlyphInfo.width * m_fontScale)
            {
                segmentWidth = (end.x - start.x) / 2f;
            }

            float underlineThickness = m_cached_Underline_GlyphInfo.height; // m_fontAsset.FontInfo.UnderlineThickness;
            // Front Part of the Underline
            m_vertices[index + 0] = start + new Vector3(0, 0 - (underlineThickness + m_padding) * m_fontScale, 0); // BL
            m_vertices[index + 1] = start + new Vector3(0, m_padding * m_fontScale, 0); // TL
            m_vertices[index + 2] = m_vertices[index + 0] + new Vector3(segmentWidth, 0, 0); // BR
            m_vertices[index + 3] = start + new Vector3(segmentWidth, m_padding * m_fontScale, 0); // TR

            // Middle Part of the Underline
            m_vertices[index + 4] = m_vertices[index + 2]; // BL
            m_vertices[index + 5] = m_vertices[index + 3]; // TL
            m_vertices[index + 6] = end + new Vector3(-segmentWidth, -(underlineThickness + m_padding) * m_fontScale, 0); // BR
            m_vertices[index + 7] = end + new Vector3(-segmentWidth, m_padding * m_fontScale, 0); // TR

            // End Part of the Underline
            m_vertices[index + 8] = m_vertices[index + 6];
            m_vertices[index + 9] = m_vertices[index + 7];
            m_vertices[index + 10] = end + new Vector3(0, -(underlineThickness + m_padding) * m_fontScale, 0);
            m_vertices[index + 11] = end + new Vector3(0, m_padding * m_fontScale, 0);


            // Calculate UV required to setup the 3 Quads for the Underline.
            Vector2 uv0 = new Vector2((m_cached_Underline_GlyphInfo.x - m_padding) / m_fontAsset.fontInfo.AtlasWidth, 1 - (m_cached_Underline_GlyphInfo.y + m_padding + m_cached_Underline_GlyphInfo.height) / m_fontAsset.fontInfo.AtlasHeight);  // bottom left
            Vector2 uv1 = new Vector2(uv0.x, 1 - (m_cached_Underline_GlyphInfo.y - m_padding) / m_fontAsset.fontInfo.AtlasHeight);  // top left
            Vector2 uv2 = new Vector2((m_cached_Underline_GlyphInfo.x + m_padding + m_cached_Underline_GlyphInfo.width / 2) / m_fontAsset.fontInfo.AtlasWidth, uv0.y); // bottom right
            Vector2 uv3 = new Vector2(uv2.x, uv1.y); // top right
            Vector2 uv4 = new Vector2((m_cached_Underline_GlyphInfo.x + m_padding + m_cached_Underline_GlyphInfo.width) / m_fontAsset.fontInfo.AtlasWidth, uv0.y); // End Part - Bottom Right
            Vector2 uv5 = new Vector2(uv4.x, uv1.y); // End Part - Top Right

            // Left Part of the Underline
            m_uvs[0 + index] = uv0; // BL
            m_uvs[1 + index] = uv1; // TL   
            m_uvs[2 + index] = uv2; // BR   
            m_uvs[3 + index] = uv3; // TR

            // Middle Part of the Underline
            m_uvs[4 + index] = new Vector2(uv2.x - uv2.x * 0.001f, uv0.y);
            m_uvs[5 + index] = new Vector2(uv2.x - uv2.x * 0.001f, uv1.y);
            m_uvs[6 + index] = new Vector2(uv2.x + uv2.x * 0.001f, uv0.y);
            m_uvs[7 + index] = new Vector2(uv2.x + uv2.x * 0.001f, uv1.y);

            // Right Part of the Underline
            m_uvs[8 + index] = uv2;
            m_uvs[9 + index] = uv3;
            m_uvs[10 + index] = uv4;
            m_uvs[11 + index] = uv5;


            // UV1 contains Face / Border UV layout.
            float min_UvX = 0;
            float max_UvX = (m_vertices[index + 2].x - start.x) / (end.x - start.x);

            //Calculate the xScale or how much the UV's are getting stretched on the X axis for the middle section of the underline.
            float xScale = m_fontScale * m_transform.lossyScale.z;
            float xScale2 = xScale;

            m_uv2s[0 + index] = PackUV(0, 0, xScale);
            m_uv2s[1 + index] = PackUV(0, 1, xScale);
            m_uv2s[2 + index] = PackUV(max_UvX, 0, xScale);
            m_uv2s[3 + index] = PackUV(max_UvX, 1, xScale);

            min_UvX = (m_vertices[index + 4].x - start.x) / (end.x - start.x);
            max_UvX = (m_vertices[index + 6].x - start.x) / (end.x - start.x);

            m_uv2s[4 + index] = PackUV(min_UvX, 0, xScale2);
            m_uv2s[5 + index] = PackUV(min_UvX, 1, xScale2);
            m_uv2s[6 + index] = PackUV(max_UvX, 0, xScale2);
            m_uv2s[7 + index] = PackUV(max_UvX, 1, xScale2);

            min_UvX = (m_vertices[index + 8].x - start.x) / (end.x - start.x);
            max_UvX = (m_vertices[index + 6].x - start.x) / (end.x - start.x);

            m_uv2s[8 + index] = PackUV(min_UvX, 0, xScale);
            m_uv2s[9 + index] = PackUV(min_UvX, 1, xScale);
            m_uv2s[10 + index] = PackUV(1, 0, xScale);
            m_uv2s[11 + index] = PackUV(1, 1, xScale);


            //underlineColor.a /= 2; // Alpha value needs to be adjusted since bold is encoded in it.

            m_vertColors[0 + index] = underlineColor;
            m_vertColors[1 + index] = underlineColor;
            m_vertColors[2 + index] = underlineColor;
            m_vertColors[3 + index] = underlineColor;

            m_vertColors[4 + index] = underlineColor;
            m_vertColors[5 + index] = underlineColor;
            m_vertColors[6 + index] = underlineColor;
            m_vertColors[7 + index] = underlineColor;

            m_vertColors[8 + index] = underlineColor;
            m_vertColors[9 + index] = underlineColor;
            m_vertColors[10 + index] = underlineColor;
            m_vertColors[11 + index] = underlineColor;

            index += 12;
        }


        /// <summary>
        /// Method to Update Scale in UV2
        /// </summary>
        void UpdateSDFScale(float prevScale, float newScale)
        {
            for (int i = 0; i < m_uv2s.Length; i++)
            {
                m_uv2s[i].y = (m_uv2s[i].y / prevScale) * newScale;
            }

            m_mesh.uv2 = m_uv2s;
        }


        /// <summary>
        /// Function to Resize the Mesh Buffers
        /// </summary>
        /// <param name="size"></param>
        void ResizeMeshBuffers(int size)
        {
            int sizeX4 = size * 4;
            int sizeX6 = size * 6;
            int previousSize = m_vertices.Length / 4;

            //Debug.Log("Resizing Mesh Buffers from " + previousSize + " to " + size + ".");

            Array.Resize(ref m_vertices, sizeX4);
            Array.Resize(ref m_normals, sizeX4);
            Array.Resize(ref m_tangents, sizeX4);
            
            Array.Resize(ref m_vertColors, sizeX4);
            Array.Resize(ref m_uvs, sizeX4);
            Array.Resize(ref m_uv2s, sizeX4);

            Array.Resize(ref m_triangles, sizeX6);

            for (int i = previousSize; i < size; i++)
            {
                int index_X4 = i * 4;
                int index_X6 = i * 6;

                m_normals[0 + index_X4] = new Vector3(0, 0, -1);
                m_normals[1 + index_X4] = new Vector3(0, 0, -1);
                m_normals[2 + index_X4] = new Vector3(0, 0, -1);
                m_normals[3 + index_X4] = new Vector3(0, 0, -1);

                m_tangents[0 + index_X4] = new Vector4(-1, 0, 0, 1);
                m_tangents[1 + index_X4] = new Vector4(-1, 0, 0, 1);
                m_tangents[2 + index_X4] = new Vector4(-1, 0, 0, 1);
                m_tangents[3 + index_X4] = new Vector4(-1, 0, 0, 1);

                // Setup Triangles       
                m_triangles[0 + index_X6] = 0 + index_X4;
                m_triangles[1 + index_X6] = 1 + index_X4;
                m_triangles[2 + index_X6] = 2 + index_X4;
                m_triangles[3 + index_X6] = 3 + index_X4;
                m_triangles[4 + index_X6] = 2 + index_X4;
                m_triangles[5 + index_X6] = 1 + index_X4;
            }

            m_mesh.vertices = m_vertices;
            m_mesh.normals = m_normals;
            m_mesh.tangents = m_tangents;
            m_mesh.triangles = m_triangles;
        }



        // Used with Advanced Layout Component.
        void UpdateMeshData(TMP_CharacterInfo[] characterInfo, int characterCount, Mesh mesh, Vector3[] vertices, Vector2[] uv0s, Vector2[] uv2s, Color32[] vertexColors, Vector3[] normals, Vector4[] tangents)
        {
            //m_textInfo.characterInfo = characterInfo;
            //m_textInfo.characterCount = (short)characterCount;
            ////m_meshInfo.mesh = mesh;
            //m_meshInfo.vertices = vertices;
            //m_meshInfo.uv0s = uv0s;
            //m_meshInfo.uv2s = uv2s;
            //m_meshInfo.vertexColors = m_vertColors;
            //m_meshInfo.normals = normals;
            //m_meshInfo.tangents = tangents;
        }


        // Function to offset vertices position to account for line spacing changes.
        void AdjustLineOffset(int startIndex, int endIndex, float offset)
        {
            Vector3 vertexOffset = new Vector3(0, offset, 0);

            for (int i = startIndex; i <= endIndex; i++)
            {
                if (m_textInfo.characterInfo[i].isVisible)
                {
                    int vertexIndex = m_textInfo.characterInfo[i].vertexIndex;     
                    m_textInfo.characterInfo[i].bottomLeft -= vertexOffset;
                    m_textInfo.characterInfo[i].topRight -= vertexOffset;
                    m_textInfo.characterInfo[i].bottomLine -= vertexOffset.y;
                    m_textInfo.characterInfo[i].topLine -= vertexOffset.y;

                    m_vertices[0 + vertexIndex] -= vertexOffset;
                    m_vertices[1 + vertexIndex] -= vertexOffset;
                    m_vertices[2 + vertexIndex] -= vertexOffset;
                    m_vertices[3 + vertexIndex] -= vertexOffset;
                }

            }       
        }


        // Save the State of various variables used in the mesh creation loop in conjunction with Word Wrapping 
        void SaveWordWrappingState(ref WordWrapState state, int index, int count)
        {
            state.previous_WordBreak = index;
            state.total_CharacterCount = count;
            state.visible_CharacterCount = m_visibleCharacterCount;
            state.firstVisibleCharacterIndex = m_firstVisibleCharacterOfLine;
            state.lastVisibleCharIndex = m_lastVisibleCharacterOfLine;
            state.xAdvance = m_xAdvance;
            state.maxAscender = m_maxAscender;
            state.maxDescender = m_maxDescender;
            state.preferredWidth = m_preferredWidth;
            state.preferredHeight = m_preferredHeight;
            state.fontScale = m_fontScale;
            state.maxFontScale = m_maxFontScale;
            state.currentFontSize = m_currentFontSize;
            
            state.lineNumber = m_lineNumber; 
            state.lineOffset = m_lineOffset;
            state.baselineOffset = m_baselineOffset;
            state.fontStyle = m_style;
            state.vertexColor = m_htmlColor;
            state.colorStackIndex = m_colorStackIndex;
            state.meshExtents = m_meshExtents;
            state.lineInfo = m_textInfo.lineInfo[m_lineNumber];
            state.textInfo = m_textInfo;
        }


        // Restore the State of various variables used in the mesh creation loop.
        int RestoreWordWrappingState(ref WordWrapState state)
        {
            m_textInfo.lineInfo[m_lineNumber] = state.lineInfo;
            m_textInfo = state.textInfo != null ? state.textInfo : m_textInfo;
            m_currentFontSize = state.currentFontSize;
            m_fontScale = state.fontScale;
            m_baselineOffset = state.baselineOffset;
            m_style = state.fontStyle;
            m_htmlColor = state.vertexColor;
            m_colorStackIndex = state.colorStackIndex;

            m_characterCount = state.total_CharacterCount + 1;
            m_visibleCharacterCount = state.visible_CharacterCount;
            m_firstVisibleCharacterOfLine = state.firstVisibleCharacterIndex;
            m_lastVisibleCharacterOfLine = state.lastVisibleCharIndex;
            m_meshExtents = state.meshExtents;
            m_xAdvance = state.xAdvance;
            m_maxAscender = state.maxAscender;
            m_maxDescender = state.maxDescender;
            m_preferredWidth = state.preferredWidth;
            m_preferredHeight = state.preferredHeight;
            m_lineNumber = state.lineNumber;
            m_lineOffset = state.lineOffset;
            m_maxFontScale = state.maxFontScale;

            int index = state.previous_WordBreak;

            return index;
        }


        // Function to pack scale information in the UV2 Channel.
        Vector2 PackUV(float x, float y, float scale)
        {
            x = (x % 5) / 5;
            y = (y % 5) / 5;
          
            //return new Vector2((x * 4096) + y, scale);
            return new Vector2(Mathf.Round(x * 4096) + y, scale);
        }


        // Function to increase the size of the Line Extents Array.
        void ResizeLineExtents(int size)
        {
            size = size > 1024 ? size + 256 : Mathf.NextPowerOfTwo(size + 1);
           
            TMP_LineInfo[] temp_lineInfo = new TMP_LineInfo[size];
            for (int i = 0; i < size; i++)
            {
                if (i < m_textInfo.lineInfo.Length)
                    temp_lineInfo[i] = m_textInfo.lineInfo[i];
                else
                {
                    temp_lineInfo[i].lineExtents = new Extents(k_InfinityVector, -k_InfinityVector);
                    temp_lineInfo[i].ascender = -k_InfinityVector.x;
                    temp_lineInfo[i].descender = k_InfinityVector.x;
                }
            }
                    
            m_textInfo.lineInfo = temp_lineInfo;
        }


        // Convert HEX to INT
        int HexToInt(char hex)
        {
            switch (hex)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'A': return 10;
                case 'B': return 11;
                case 'C': return 12;
                case 'D': return 13;
                case 'E': return 14;
                case 'F': return 15;
                case 'a': return 10;
                case 'b': return 11;
                case 'c': return 12;
                case 'd': return 13;
                case 'e': return 14;
                case 'f': return 15;
            }
            return 15;
        }


        Color32 HexCharsToColor(char[] hexChars, int tagCount)
        {
            if (tagCount == 7)
            {
                byte r = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
                byte g = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
                byte b = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));

                return new Color32(r, g, b, 255);
            }
            else if (tagCount == 9)
            {
                byte r = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
                byte g = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
                byte b = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));
                byte a = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));

                return new Color32(r, g, b, a);
            }
            else if (tagCount == 13)
            {
                byte r = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
                byte g = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[10]));
                byte b = (byte)(HexToInt(hexChars[11]) * 16 + HexToInt(hexChars[12]));

                return new Color32(r, g, b, 255);
            }
            else if (tagCount == 15)
            {
                byte r = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));
                byte g = (byte)(HexToInt(hexChars[9]) * 16 + HexToInt(hexChars[10]));
                byte b = (byte)(HexToInt(hexChars[11]) * 16 + HexToInt(hexChars[12]));
                byte a = (byte)(HexToInt(hexChars[13]) * 16 + HexToInt(hexChars[14]));

                return new Color32(r, g, b, a);
            }

            return new Color32(255, 255, 255, 255);
        }


        /// <summary>
        /// Extracts a float value from char[] assuming we know the position of the start, end and decimal point.
        /// </summary>
        /// <param name="chars"></param> The Char[] containing the numerical sequence.
        /// <param name="startIndex"></param> The index of the start of the numerical sequence.
        /// <param name="endIndex"></param> The index of the last number in the numerical sequence.
        /// <param name="decimalPointIndex"></param> The index of the decimal point if any.
        /// <returns></returns>
        float ConvertToFloat(char[] chars, int startIndex, int endIndex, int decimalPointIndex)
        {
            float v = 0;
            float sign = 1;
            decimalPointIndex = decimalPointIndex > 0 ? decimalPointIndex : endIndex + 1; // Check in case we don't have any decimal point

            // Check if negative value
            if (chars[startIndex] == 45) // '-'
            {
                startIndex += 1;
                sign = -1;
            }

            if (chars[startIndex] == 43 || chars[startIndex] == 37) startIndex += 1; // '+'
              
              
            for (int i = startIndex; i < endIndex + 1; i++)
            {
                switch (decimalPointIndex - i)
                {
                    case 4:
                        v += (chars[i] - 48) * 1000;
                        break;
                    case 3:
                        v += (chars[i] - 48) * 100;
                        break;
                    case 2:
                        v += (chars[i] - 48) * 10;
                        break;
                    case 1:
                        v += (chars[i] - 48);
                        break;
                    case -1:
                        v += (chars[i] - 48) * 0.1f;
                        break;
                    case -2:
                        v += (chars[i] - 48) * 0.01f;
                        break;
                    case -3:
                        v += (chars[i] - 48) * 0.001f;
                        break;
                }
            }
            return v * sign;
        }


        // Function to identify and validate the rich tag. Returns the position of the > if the tag was valid.
        bool ValidateHtmlTag(int[] chars, int startIndex, out int endIndex)
        {
            Array.Clear(m_htmlTag, 0, m_htmlTag.Length);
            int tagCharCount = 0;
            int tagCode = 0;
            int colorCode = 0;
            int numSequenceStart = 0;
            int numSequenceEnd = 0;
            int numSequenceDecimalPos = 0;

            endIndex = startIndex;

            bool isValidHtmlTag = false;
            int equalSignValue = 1;

            for (int i = startIndex; chars[i] != 0 && tagCharCount < m_htmlTag.Length && chars[i] != 60; i++)
            {
                if (chars[i] == 62) // ASC Code of End Html tag '>'
                {
                    isValidHtmlTag = true;
                    endIndex = i;
                    m_htmlTag[tagCharCount] = (char)0;
                    break;
                }

                m_htmlTag[tagCharCount] = (char)chars[i];
                tagCharCount += 1;

                if (chars[i] == 61) equalSignValue = 0; // Once we encounter the equal sign, we stop adding the tagCode.

                tagCode += chars[i] * tagCharCount * equalSignValue;
                colorCode += chars[i] * tagCharCount * (1 - equalSignValue);
                
                // Get possible positions of numerical values 
                switch ((int)chars[i])
                {
                    case 61: // '='
                        numSequenceStart = tagCharCount;
                        break;
                    case 46: // '.'
                        numSequenceDecimalPos = tagCharCount - 1;
                        break;
                }
            }

            if (!isValidHtmlTag)
            {
                return false;
            }

            //Debug.Log("Tag is [" + m_htmlTag.ArrayToString() + "].  Tag Code:" + tagCode + "  Color Code: " + colorCode);

            if (m_htmlTag[0] == 35 && tagCharCount == 7) // if Tag begins with # and contains 7 characters. 
            {
                m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                m_colorStack[m_colorStackIndex] = m_htmlColor;
                m_colorStackIndex += 1;
                return true;
            }
            else if (m_htmlTag[0] == 35 && tagCharCount == 9) // if Tag begins with # and contains 9 characters. 
            {
                m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                m_colorStack[m_colorStackIndex] = m_htmlColor;
                m_colorStackIndex += 1;
                return true;
            }
            else
            {
                switch (tagCode)
                {
                    case 98: // <b>
                        m_style |= FontStyles.Bold;
                        return true;
                    case 105: // <i>
                        m_style |= FontStyles.Italic;
                        return true;
                    case 117: // <u>
                        m_style |= FontStyles.Underline;
                        return true;
                    case 241: // </a>
                        return true;
                    case 243: // </b>
                        m_style &= ~FontStyles.Bold;
                        return true;
                    case 257: // </i>
                        m_style &= ~FontStyles.Italic;
                        return true;
                    case 281: // </u>
                        m_style &= ~FontStyles.Underline;
                        return true;
                    case 643: // <sub>
                        m_currentFontSize *= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1; // Subscript characters are half size.
                        m_fontScale = (m_currentFontSize / m_fontAsset.fontInfo.PointSize * (m_isOrthographic ? 1 : 0.1f)); 
                        m_baselineOffset = m_fontAsset.fontInfo.SubscriptOffset * m_fontScale;
                        //m_isRecalculateScaleRequired = true;
                        return true;
                    case 679: // <pos=000.00>
                        float spacing = ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);
                        m_xAdvance = spacing * m_fontScale * m_fontAsset.fontInfo.TabWidth;
                        return true;
                    case 685: // <sup>
                        m_currentFontSize *= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1;
                        m_fontScale = (m_currentFontSize / m_fontAsset.fontInfo.PointSize * (m_isOrthographic ? 1 : 0.1f)); 
                        m_baselineOffset = m_fontAsset.fontInfo.SuperscriptOffset * m_fontScale;
                        //m_isRecalculateScaleRequired = true;
                        return true;
                    case 1019: // <page>
                        // This mode only works when page mode is used.
                        if (m_overflowMode == TextOverflowModes.Page)
                        {
                            m_xAdvance = 0 + m_indent;
                            m_lineOffset = 0;
                            m_pageNumber += 1;
                            m_isNewPage = true;
                            //Debug.Log("m_lineOffet is " + m_lineOffset);
                        }
                        return true;
                    case 1020: // </sub>
                        m_currentFontSize /= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1; //m_fontSize / m_fontAsset.FontInfo.PointSize * .1f;
                        m_baselineOffset = 0;
                        m_fontScale = (m_currentFontSize / m_fontAsset.fontInfo.PointSize * (m_isOrthographic ? 1 : 0.1f)); 
                        //m_isRecalculateScaleRequired = true;
                        return true;
                    case 1076: // </sup>
                        m_currentFontSize /= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1; //m_fontSize / m_fontAsset.FontInfo.PointSize * .1f;
                        m_baselineOffset = 0;
                        m_fontScale = (m_currentFontSize / m_fontAsset.fontInfo.PointSize * (m_isOrthographic ? 1 : 0.1f)); 
                        //m_isRecalculateScaleRequired = true;
                        return true;
                    case 1095: // <size=>
                        numSequenceEnd = tagCharCount - 1;
                        float val = 0;

                        if (m_htmlTag[5] == 37) // <size=%00>
                        {
                            val = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                            m_currentFontSize = m_fontSize * val / 100;
                            m_isRecalculateScaleRequired = true;
                            return true;
                        }
                        else if (m_htmlTag[5] == 43) // <size=+00>
                        {
                            val = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                            m_currentFontSize = m_fontSize + val;
                            m_isRecalculateScaleRequired = true;
                            return true;
                        }
                        else if (m_htmlTag[5] == 45) // <size=-00>
                        {
                            val = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                            m_currentFontSize = m_fontSize + val;
                            m_isRecalculateScaleRequired = true;
                            return true;
                        }
                        else // <size=0000.00>
                        {
                            val = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                            if (val == 73493) return false; // if tag is <size> with no values.
                            m_currentFontSize = val;
                            m_isRecalculateScaleRequired = true;
                            return true;
                        }
                    case 1118: // <font=xx>
                        Debug.Log("Font Tag used.");
                        //m_fontIndex = (int)ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);

                        //if(m_fontAssetArray[m_fontIndex] == null)
                        //{
                        //    // Load new font asset into index
                        //    m_fontAssetArray[m_fontIndex] = Resources.Load("Fonts & Materials/Bangers SDF", typeof(TextMeshProFont)) as TextMeshProFont; // Hard coded right now to a specific font
                        //}

                        //m_fontScale = (m_fontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize * (m_isOrthographic ? 1 : 0.1f));
                        //Debug.Log("Font Index = " + m_fontIndex);
                        return true;
                    case 1531: // <space=000.00>
                        spacing = ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);
                        m_xAdvance += spacing * m_fontScale * m_fontAsset.fontInfo.TabWidth;
                        return true;
                    case 1550: // <alpha=#FF>
                        m_htmlColor.a = (byte)(HexToInt(m_htmlTag[7]) * 16 + HexToInt(m_htmlTag[8]));
                        return true;
                    case 1585: // </size>
                        m_currentFontSize = m_fontSize;
                        m_isRecalculateScaleRequired = true;
                        //m_fontScale = m_fontSize / m_fontAsset.fontInfo.PointSize * .1f;
                        return true;
                    case 1590: // <align=>
                        //Debug.Log("Align " + colorCode);
                        switch (colorCode)
                        {
                            case 4008: // <align=left>
                                m_lineJustification = TextAlignmentOptions.Left;
                                return true;
                            case 5247: // <align=right>
                                m_lineJustification = TextAlignmentOptions.Right;
                                return true;
                            case 6496: // <align=center>
                                m_lineJustification = TextAlignmentOptions.Center;
                                return true;
                            case 10897: // <align=justified>
                                m_lineJustification = TextAlignmentOptions.Justified;
                                return true;
                        }
                        return false;
                    case 1659: // <color=>                       
                        // Handle <color=#FF00FF> or <color=#FF00FF00>
                        if (m_htmlTag[6] == 35 && tagCharCount == 13)
                        {
                            m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                            m_colorStack[m_colorStackIndex] = m_htmlColor;
                            m_colorStackIndex += 1;
                            return true;
                        }
                        else if (m_htmlTag[6] == 35 && tagCharCount == 15)
                        {
                            m_htmlColor = HexCharsToColor(m_htmlTag, tagCharCount);
                            m_colorStack[m_colorStackIndex] = m_htmlColor;
                            m_colorStackIndex += 1;
                            return true;
                        }

                        switch (colorCode)
                        {
                            case 2872: // <color=red>                            
                                m_htmlColor = Color.red;
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case 3979: // <color=blue>
                                m_htmlColor = Color.blue;
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case 4956: // <color=black>
                                m_htmlColor = Color.black;
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case 5128: // <color=green>
                                m_htmlColor = Color.green;
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case 5247: // <color=white>
                                m_htmlColor = Color.white;
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case 6373: // <color=orange>
                                m_htmlColor = new Color32(255, 128, 0, 255);
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case 6632: // <color=purple>
                                m_htmlColor = new Color32(160, 32, 240, 255);
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                            case 6722: // <color=yellow>
                                m_htmlColor = Color.yellow;
                                m_colorStack[m_colorStackIndex] = m_htmlColor;
                                m_colorStackIndex += 1;
                                return true;
                        }
                        return false;
                    case 2030: // <a name="">
                        return true;
                    case 2154: // <cspace=xx.x>
                        m_cSpacing = ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);
                        return true;
                    case 2160: // </align>
                        m_lineJustification = m_textAlignment;
                        return true;
                    case 2164: // <mspace=xx.x>
                        m_monoSpacing = ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);                        
                        return true;
                    case 2249: // </color>
                        m_colorStackIndex -= 1;

                        if (m_colorStackIndex <= 0)
                        {
                            m_htmlColor = m_fontColor32;
                            m_colorStackIndex = 0;
                        }
                        else
                        {
                            m_htmlColor = m_colorStack[m_colorStackIndex - 1];
                        }


                        //m_htmlColor = m_fontColor32;
                        return true;
                    case 2275: // <indent=00.0>
                        m_indent = ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos) * m_fontScale * m_fontAsset.fontInfo.TabWidth;
                        m_xAdvance = m_indent;                      
                        return true;                      
                    case 2287: // <sprite=x>
                        Debug.Log("Sprite Tag used.");
                        return true;
                    case 2844: // </mspace>
                        m_monoSpacing = 0;
                        return true;
                    case 2824: // </cspace>
                        m_cSpacing = 0;
                        return true;
                    case 2964: // </indent>
                        m_indent = 0;
                        return true;
                    case 2995: // <allcaps>
                        m_style |= FontStyles.UpperCase;
                        return true;
                    case 3778: // </allcaps>
                        m_style &= ~FontStyles.UpperCase;
                        return true;
                    case 4800: // <smallcaps>                      
                        m_style |= FontStyles.SmallCaps;
                        return true;
                    case 5807: // </smallcaps>
                        m_currentFontSize = m_fontSize;
                        m_style &= ~FontStyles.SmallCaps;
                        m_isRecalculateScaleRequired = true;
                        return true;
                    case 6691: // <line-height=xx.x>
                        m_lineHeight = ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);
                        return true;
                    case 7840: // </line-height>
                        m_lineHeight = 0;
                        return true;          
                }
            }

            return false;
        }
    }
}