// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if UNITY_4_6 || UNITY_5

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.EventSystems;

#pragma warning disable 0414 // Disabled a few warnings related to serialized variables not used in this script but used in the editor.

namespace TMPro
{  

    public partial class TextMeshProUGUI
    {
                            
        [SerializeField]
        private string m_text;

        [SerializeField]
        private TextMeshProFont m_fontAsset;

        [SerializeField]
        private Material m_fontMaterial;

        [SerializeField]
        private Material m_sharedMaterial;

        [SerializeField]
        private FontStyles m_fontStyle = FontStyles.Normal;
        private FontStyles m_style = FontStyles.Normal;

        private bool m_isOverlay = false;

        [SerializeField]
        private Color32 m_fontColor32 = Color.white;

        [SerializeField]
        private Color m_fontColor = Color.white;

		[SerializeField]
		private bool m_enableVertexGradient;

		[SerializeField]
		private VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

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
		private float m_cSpacing = 0;
        private float m_monoSpacing = 0;

        //[SerializeField]
        //private float m_lineLength = 72;

        //[SerializeField]
        //private Vector4 m_textRectangle;

        [SerializeField]
        private float m_lineSpacing = 0;
        private float m_lineSpacingDelta = 0;
        private float m_lineHeight = 0; // Used with the <line-height=xx.x> tag.

        [SerializeField]
        private float m_paragraphSpacing = 0;
        
        //[SerializeField]
        //private AnchorPositions m_anchor = AnchorPositions.TopLeft;

        [SerializeField]
        [UnityEngine.Serialization.FormerlySerializedAs("m_lineJustification")]
        private TextAlignmentOptions m_textAlignment = TextAlignmentOptions.TopLeft;    
        private TextAlignmentOptions m_lineJustification;

        [SerializeField]
        private bool m_enableKerning = false;

        //private bool m_anchorDampening = false;
        //private float m_baseDampeningWidth;

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
        //private bool m_inputHasBeenReparsed = false;

        [SerializeField]
        private bool havePropertiesChanged;  // Used to track when properties of the text object have changed.

        [SerializeField]
        private bool hasFontAssetChanged = false; // Used to track when font properties have changed.
        //[SerializeField]
        //private bool hasMaterialChanged = false;

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
        //private float m_totalxAdvance;
        //private bool m_isCheckingTextLength = false;
        //private float m_textLength;
        //private int[] m_text_buffer = new int[8];

        //private float max_LineWrapLength = 999;

        private Vector3 m_anchorOffset; // The amount of offset to be applied to the vertices. 


        private TMP_TextInfo m_textInfo; // Class which holds information about the Text object such as characters, lines, mesh data as well as metrics.           
        private List<TMP_CharacterInfo> m_characterInfoList = new List<TMP_CharacterInfo>(256);

        private char[] m_htmlTag = new char[16]; // Maximum length of rich text tag. This is pre-allocated to avoid GC.


        //[SerializeField]
        private CanvasRenderer m_uiRenderer;

        private Canvas m_canvas;
        private RectTransform m_rectTransform;
        //private Mesh m_mesh;

        private Color32 m_htmlColor = new Color(255, 255, 255, 128);
        private Color32[] m_colorStack = new Color32[32];
        private int m_colorStackIndex = 0;
        
        private float m_tabSpacing = 0;
        private float m_spacing = 0;
        private Vector2[] m_spacePositions = new Vector2[8];

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


        //private Mesh_Extents[] m_lineExtents; // Struct that holds information about each line which is used for UV Mapping.

        //private IFormatProvider NumberFormat = System.Globalization.NumberFormatInfo.CurrentInfo; // Speeds up accessing this interface.
        private readonly float[] k_Power = { 5e-1f, 5e-2f, 5e-3f, 5e-4f, 5e-5f, 5e-6f, 5e-7f, 5e-8f, 5e-9f, 5e-10f }; // Used by FormatText to enable rounding and avoid using Mathf.Pow.

        private GlyphInfo m_cached_GlyphInfo; // Glyph / Character information is cached into this variable which is faster than having to fetch from the Dictionary multiple times.
        private GlyphInfo m_cached_Underline_GlyphInfo; // Same as above but for the underline character which is used for Underline.

        private WordWrapState m_SavedWordWrapState; // = new WordWrapState(); // Struct which holds various states / variables used in conjunction with word wrapping.
        private WordWrapState m_SavedLineState; // = new WordWrapState();
       
        private int m_characterCount;
        private int m_visibleCharacterCount;
        private int m_visibleSpriteCount;
        private int m_firstVisibleCharacterOfLine;
        private int m_lastVisibleCharacterOfLine;
        private int m_lineNumber;
        private int m_pageNumber;
        private float m_maxAscender;
        private float m_maxDescender;
        private float m_maxFontScale;
        //private float m_previousFontScale;
        private float m_lineOffset;      
        private Extents m_meshExtents;


        // Properties related to the Auto Layout System
        private bool m_isCalculateSizeRequired = false;
        //private bool m_isCalculatingLayout = false;
        private ILayoutController m_layoutController;

        // Mesh Declaration
        [SerializeField]
        private UIVertex[] m_uiVertices;
        //private Vector3[] m_vertices;
        //private Vector3[] m_normals;
        //private Vector4[] m_tangents;
        //private Vector2[] m_uvs;
        //private Vector2[] m_uv2s;
        //private Color32[] m_vertColors;
        //private int[] m_triangles;
        private Bounds m_bounds;

        //private Camera m_sceneCamera;
        //private Bounds m_default_bounds = new Bounds(Vector3.zero, new Vector3(1000, 1000, 0));
       

        [SerializeField]
        private bool m_ignoreCulling = true; // Not implemented yet.
        [SerializeField]
        private bool m_isOrthographic = false;

        [SerializeField]
        private bool m_isCullingEnabled = false;

        [SerializeField]
        private int m_sortingLayerID;
        [SerializeField]
        private int m_sortingOrder;


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
        private int[] m_meshAllocCount = new int[17];

        private GameObject[] subObjects = new GameObject[17];

        //[SerializeField]
        private List<Material> m_sharedMaterials = new List<Material>(16);
        private int m_selectedFontAsset;

        // MASKING RELATED PROPERTIES       
        private bool m_isMaskingEnabled;
        private bool m_isStencilUpdateRequired;

        [SerializeField]
        private Material m_baseMaterial;
        private Material m_lastBaseMaterial;
        [SerializeField]
        private bool m_isNewBaseMaterial;
        private Material m_maskingMaterial; // Instance of the material used for masking.

        private bool m_isScrollRegionSet;       
        //private Mask m_mask;
        private int m_stencilID = 0;
        /*
        [SerializeField]
        
        [SerializeField]
        private MaskingTypes m_mask;
        [SerializeField]
        private MaskingOffsetMode m_maskOffsetMode;
        */
        [SerializeField]
        private Vector4 m_maskOffset;
        /*
        [SerializeField]
        private Vector2 m_maskSoftness;
        [SerializeField]
        private Vector2 m_vertexOffset;
        */

        //
        private Matrix4x4 m_EnvMapMatrix = new Matrix4x4();


        // FLAGS
        //private bool DONT_RENDER_MESH = false;
        private TextRenderFlags m_renderMode = TextRenderFlags.Render;

        // ADVANCED LAYOUT COMPONENT ** Still work in progress
        private float m_maxXAdvance;
      

        // INLINE GRAPHIC COMPONENT
        private int m_spriteCount = 0;
        private bool m_isSprite = false;
        private int m_spriteIndex;
        private InlineGraphicManager m_inlineGraphics;



        // Text Container / RectTransform Component
        [SerializeField]
        private Vector4 m_margin = new Vector4(0, 0, 0, 0);
        private float m_marginWidth;
        private float m_marginHeight;
        private bool m_marginsHaveChanged;
        private bool IsRectTransformDriven = false;
        private float m_width;

        [SerializeField]
        //private bool m_isNewTextObject;
        //private TextContainer m_textContainer;
        private bool m_rectTransformDimensionsChanged;
        private Vector3[] m_rectCorners = new Vector3[4];

        [SerializeField]
        private bool m_enableAutoSizing;
        private float m_maxFontSize;
        private float m_minFontSize;

        private bool m_isAwake;
        private bool m_isEnabled;
      


        // ** Still needs to be implemented **
        //private Camera managerCamera;
        //private TMPro_UpdateManager m_updateManager;
        //private bool isAlreadyScheduled;

        // DEBUG Variables
        //private System.Diagnostics.Stopwatch m_StopWatch;
        //private int frame = 0;
        private int m_recursiveCount = 0;
        private int loopCountA = 0;
        //private int loopCountB = 0;
        //private int loopCountC = 0;
        //private int loopCountD = 0;
        //private int loopCountE = 0;

       
        protected override void Awake()
        {
            //base.Awake();
            //Debug.Log("***** Awake() *****"); // on Object ID:" + GetInstanceID());      

            m_isAwake = true;      
            // Cache Reference to the Canvas
            m_canvas = GetComponentInParent(typeof(Canvas)) as Canvas;          

            // Cache Reference to RectTransform.
            m_rectTransform = gameObject.GetComponent<RectTransform>();
            if (m_rectTransform == null)   
                m_rectTransform = gameObject.AddComponent<RectTransform>();
            

                          
            // Cache a reference to the UIRenderer.
            m_uiRenderer = GetComponent<CanvasRenderer>();
            if (m_uiRenderer == null) 
				m_uiRenderer = gameObject.AddComponent<CanvasRenderer> ();

			//m_uiRenderer.hideFlags = HideFlags.HideInInspector;

            // Determine if the RectTransform is Driven         
            m_layoutController = GetComponent(typeof(ILayoutController)) as ILayoutController ?? (transform.parent != null ? transform.parent.GetComponent(typeof(ILayoutController)) as ILayoutController : null);           
            if (m_layoutController != null) IsRectTransformDriven = true;

            // Cache reference to Mask Component if one is present         
            //m_stencilID = MaterialManager.GetStencilID(gameObject);
            //m_mask = GetComponentInParent<Mask>();
                       

            // Load the font asset and assign material to renderer.
            LoadFontAsset();

            // Allocate our initial buffers.          
            m_char_buffer = new int[m_max_characters];           
            m_cached_GlyphInfo = new GlyphInfo();
            m_uiVertices = new UIVertex[0]; // 
            m_isFirstAllocation = true;          
            
            m_textInfo = new TMP_TextInfo();
            //m_textInfo.wordInfo = new List<TMP_WordInfo>();
            //m_textInfo.lineInfo = new TMP_LineInfo[m_max_numberOfLines];
            //m_textInfo.pageInfo = new TMP_PageInfo[16];
            //m_textInfo.meshInfo = new TMP_MeshInfo();
            //m_textInfo.meshInfo.meshArrays = new UIVertex[17][];
          

            // TODO : Add support for inline sprites and other fonts.
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

            //// Set flags to ensure our text is parsed and text re-drawn. 
            isInputParsingRequired = true;
            havePropertiesChanged = true;
            m_rectTransformDimensionsChanged = true;
            //m_isCalculateSizeRequired = true;

            ForceMeshUpdate(); // Added to force OnWillRenderObject() to be called in case object is not visible so we get initial bounds for the mesh.
        }

           
        protected override void OnEnable()
        {
            //base.OnEnable();
            //Debug.Log("***** OnEnable() *****"); // + GetInstanceID());  // HavePropertiesChanged = " + havePropertiesChanged); // has been called on Object ID:" + gameObject.GetInstanceID());      
            m_isEnabled = true;
       
#if UNITY_EDITOR
            // Register Callbacks for various events.
            TMPro_EventManager.MATERIAL_PROPERTY_EVENT += ON_MATERIAL_PROPERTY_CHANGED;
            TMPro_EventManager.FONT_PROPERTY_EVENT += ON_FONT_PROPERTY_CHANGED;
            TMPro_EventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT += ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED;
            TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT += ON_DRAG_AND_DROP_MATERIAL;
            //TMPro_EventManager.BASE_MATERIAL_EVENT += ON_BASE_MATERIAL_CHANGED;
#endif
            // Register to get callback before Canvas is Rendered.
            Canvas.willRenderCanvases += OnPreRenderCanvas;

            // Cache Reference to the Canvas
            if (m_canvas == null) m_canvas = GetComponentInParent(typeof(Canvas)) as Canvas;

            // Re-assign the Material to the Canvas Renderer
            if (m_uiRenderer.GetMaterial() == null)
            {
                if (m_sharedMaterial != null)
                {
                    m_uiRenderer.SetMaterial(m_sharedMaterial, null);
                }
                else
                {
                    // We likely had a masking material assigned
                    m_isNewBaseMaterial = true;
                    fontSharedMaterial = m_baseMaterial;
                    ParentMaskStateChanged();
                }

                havePropertiesChanged = true;
                m_rectTransformDimensionsChanged = true;
            }
             
            //if (IsRectTransformDriven)
            LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
        }

      

        protected override void OnDisable()
        {
            //base.OnDisable();
            //Debug.Log("***** OnDisable() *****"); //for " + this.name + " with ID: " + this.GetInstanceID() + " has been called.");
            m_isEnabled = false;

#if UNITY_EDITOR
            // Un-register the event this object was listening to
            TMPro_EventManager.MATERIAL_PROPERTY_EVENT -= ON_MATERIAL_PROPERTY_CHANGED;
            TMPro_EventManager.FONT_PROPERTY_EVENT -= ON_FONT_PROPERTY_CHANGED;
            TMPro_EventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT -= ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED;
            TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT -= ON_DRAG_AND_DROP_MATERIAL;
            //TMPro_EventManager.BASE_MATERIAL_EVENT -= ON_BASE_MATERIAL_CHANGED;
#endif
            // Register to get callback before Canvas is Rendered.
            Canvas.willRenderCanvases -= OnPreRenderCanvas;
            
            m_uiRenderer.Clear();  // Might not want to clear since it wipes the Material in addition to the mesh geometry.

            //if (IsRectTransformDriven)
            LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);

        }


        protected override void OnDestroy()
        {
            //base.OnDestroy();
            //Debug.Log("***** OnDestroy() *****");
            
            if (m_maskingMaterial != null)
            {
                //Debug.Log("Trying to release Masking Material [" + m_maskingMaterial.name + "] with ID " + m_maskingMaterial.GetInstanceID());
                MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                m_maskingMaterial = null;
            }

            // Clean up any material instances we may have created.
            if (m_fontMaterial != null)
                DestroyImmediate(m_fontMaterial);
        }


        protected  void Reset()
        {
            //base.Reset();
            //Debug.Log("***** Reset() *****"); //has been called.");  
            //LoadFontAsset();
            isInputParsingRequired = true;
            havePropertiesChanged = true;
        }


        protected override void OnTransformParentChanged()
        {
            //Debug.Log("***** OnTransformParentChanged *****");
            // Check the Stencil ID        
            int currentID = m_stencilID;
            m_stencilID = MaterialManager.GetStencilID(gameObject);
            if (currentID != m_stencilID)
                ParentMaskStateChanged();

            LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
        }


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            //Debug.Log("isAwake = " + m_isAwake + "  isEnabled = " + m_isEnabled);
            // Additional Properties could be added to sync up Serialized Properties & Properties.

            //Debug.Log("***** OnValidate() *****"); // ID " + GetInstanceID()); // New Material [" + m_sharedMaterial.name + "] with ID " + m_sharedMaterial.GetInstanceID() + ". Base Material is [" + m_baseMaterial.name + "] with ID " + m_baseMaterial.GetInstanceID() + ". Previous Base Material is [" + (m_lastBaseMaterial == null ? "Null" : m_lastBaseMaterial.name) + "]."); 

            //if (!m_isAwake)
            //    return;
            
            //Debug.Log(m_fontAsset.atlas.GetInstanceID() + "  " + m_uiRenderer.GetMaterial().mainTexture.GetInstanceID());

            if (m_uiRenderer == null || m_uiRenderer.GetMaterial() == null || m_fontAsset.atlas.GetInstanceID() != m_uiRenderer.GetMaterial().mainTexture.GetInstanceID())
            { 
                LoadFontAsset();
                m_isCalculateSizeRequired = true;
                hasFontAssetChanged = false;
            }
            font = m_fontAsset;      
            
            text = m_text;
                  
            fontSharedMaterial = m_sharedMaterial;
            //checkPaddingRequired = true;
                
            margin = m_margin; // Getting called on assembly reloads.

            LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
        }


        // Event to Track Material Changed resulting from Drag-n-drop.
        void ON_DRAG_AND_DROP_MATERIAL(GameObject obj, Material currentMaterial, Material newMaterial)
        {                       
            
            //Debug.Log("Drag-n-Drop Event - Receiving Object ID " + GetInstanceID() + ". Sender ID " + obj.GetInstanceID()); // +  ". Prefab Parent is " + UnityEditor.PrefabUtility.GetPrefabParent(gameObject).GetInstanceID()); // + ". New Material is " + newMaterial.name + " with ID " + newMaterial.GetInstanceID() + ". Base Material is " + m_baseMaterial.name + " with ID " + m_baseMaterial.GetInstanceID());
            
            // Check if event applies to this current object
            if (obj == gameObject || UnityEditor.PrefabUtility.GetPrefabParent(gameObject) == obj)
            {
                //Debug.Log("Assigning new Base Material " + newMaterial.name + " to replace " + currentMaterial.name);

                UnityEditor.Undo.RecordObject(this, "Material Assignment");
                m_baseMaterial = newMaterial;
                m_isNewBaseMaterial = true;
                fontSharedMaterial = m_baseMaterial;            
            }
        }


        // Event received when custom material editor properties are changed.
        void ON_MATERIAL_PROPERTY_CHANGED(bool isChanged, Material mat)
        {
            //Debug.Log("ON_MATERIAL_PROPERTY_CHANGED event received by object with ID " + GetInstanceID()); // + " and Targeted Material is: " + mat.name + " with ID " + mat.GetInstanceID() + ". Base Material is " + m_baseMaterial.name + " with ID " + m_baseMaterial.GetInstanceID() + ". Masking Material is " + m_maskingMaterial.name + "  with ID " + m_maskingMaterial.GetInstanceID());         
            
            ShaderUtilities.GetShaderPropertyIDs(); // Initialize ShaderUtilities and get shader property IDs.

            int materialID = mat.GetInstanceID();

            if (m_uiRenderer.GetMaterial() == null)
            {
                if (m_fontAsset != null)
                {
                    m_uiRenderer.SetMaterial(m_fontAsset.material, null);
                    //Debug.LogWarning("No Material was assigned to " + name + ". " + m_fontAsset.material.name + " was assigned.");
                }
                else
                    Debug.LogWarning("No Font Asset assigned to " + name + ". Please assign a Font Asset.");
            }
           
          
            if (m_uiRenderer.GetMaterial() != m_sharedMaterial && m_fontAsset == null) //    || m_renderer.sharedMaterials.Contains(mat))
            {
                Debug.Log("ON_MATERIAL_PROPERTY_CHANGED Called on Target ID: " + GetInstanceID() + ". Previous Material:" + m_sharedMaterial + "  New Material:" + m_uiRenderer.GetMaterial()); // on Object ID:" + GetInstanceID() + ". m_sharedMaterial: " + m_sharedMaterial.name + "  m_renderer.sharedMaterial: " + m_renderer.sharedMaterial.name);         
                m_sharedMaterial = m_uiRenderer.GetMaterial();
            }

           
            // Is Material being modified my Base Material            
            if (m_stencilID > 0 && m_baseMaterial != null && m_maskingMaterial != null)
            {
                
                if (materialID == m_baseMaterial.GetInstanceID())
                {
                    //Debug.Log("Copying Material properties from Base Material [" + mat + "] to Masking Material [" + m_maskingMaterial + "].");
                    float stencilID = m_maskingMaterial.GetFloat(ShaderUtilities.ID_StencilID);
                    float stencilComp = m_maskingMaterial.GetFloat(ShaderUtilities.ID_StencilComp);
                    m_maskingMaterial.CopyPropertiesFromMaterial(mat);
                    m_maskingMaterial.shaderKeywords = mat.shaderKeywords;

                    m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
                    m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilComp, stencilComp);
                }
                else if (materialID == m_maskingMaterial.GetInstanceID())
                {                            
                    //Debug.Log("Copying Material properties from Masking Material [" + mat + "] to Base Material [" + m_baseMaterial + "].");
                    m_baseMaterial.CopyPropertiesFromMaterial(mat);
                    m_baseMaterial.shaderKeywords = mat.shaderKeywords;
                    m_baseMaterial.SetFloat(ShaderUtilities.ID_StencilID, 0);
                    m_baseMaterial.SetFloat(ShaderUtilities.ID_StencilComp, 8);
                }
                else if (MaterialManager.GetBaseMaterial(mat) != null && MaterialManager.GetBaseMaterial(mat).GetInstanceID() == m_baseMaterial.GetInstanceID())
                {
                    //Debug.Log("Copying Material properties from Masking Material [" + mat + "] to Masking Material [" + m_maskingMaterial + "].");
                    float stencilID = m_maskingMaterial.GetFloat(ShaderUtilities.ID_StencilID);
                    float stencilComp = m_maskingMaterial.GetFloat(ShaderUtilities.ID_StencilComp);
                    m_maskingMaterial.CopyPropertiesFromMaterial(mat);
                    m_maskingMaterial.shaderKeywords = mat.shaderKeywords;

                    m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
                    m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilComp, stencilComp);                
                }
            }
            
                       
            //Debug.Log("Assigned Material is " + m_sharedMaterial.name + " with ID " + m_sharedMaterial.GetInstanceID() +
            //          ". Target Mat is " + mat.name + " with ID " + mat.GetInstanceID() + 
            //          ". Base Material is " + m_baseMaterial.name + " with ID " + m_baseMaterial.GetInstanceID() + 
            //          ". Masking Material is " + m_maskingMaterial.name + " with ID " + m_maskingMaterial.GetInstanceID());

                                 
            m_padding = ShaderUtilities.GetPadding(m_uiRenderer.GetMaterial(), m_enableExtraPadding, m_isUsingBold);
            m_alignmentPadding = ShaderUtilities.GetFontExtent(m_uiRenderer.GetMaterial());
            havePropertiesChanged = true;
            /* ScheduleUpdate(); */
        }


        // Event received to handle base material changes related to masking
        void ON_BASE_MATERIAL_CHANGED(Material mat)
        {           
           
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
        void ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED(bool isChanged, TextMeshProUGUI obj)
        {
            Debug.Log("Event Received by " + obj);
            
            if (obj == this)
            {
                //Debug.Log("Undo / Redo Event Received by Object ID:" + GetInstanceID());
                havePropertiesChanged = true;
                isInputParsingRequired = true;
                /* ScheduleUpdate(); */
            }
        }
#endif


        // Function which loads either the default font or a newly assigned font asset. This function also assigned the appropriate material to the renderer.
        void LoadFontAsset()
        {
            //Debug.Log("***** LoadFontAsset() *****"); //TextMeshPro LoadFontAsset() has been called."); // Current Font Asset is " + (font != null ? font.name: "Null") );
            ShaderUtilities.GetShaderPropertyIDs();

            if (m_fontAsset == null) 
            {
                
                //Debug.LogWarning("No Font Asset has been assigned. Loading Default Arial SDF Font.");

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

                               
                //m_uiRenderer.SetMaterial(m_fontAsset.material, null);
                m_baseMaterial = m_fontAsset.material;
                m_sharedMaterial = m_baseMaterial;
                m_isNewBaseMaterial = true;
              
                //m_renderer.receiveShadows = false;
                //m_renderer.castShadows = false; // true;
                // Get a Reference to the Shader
            }
            else
            {
                if (m_fontAsset.characterDictionary == null)
                {
                    //Debug.Log("Reading Font Definition and Creating Character Dictionary.");
                    m_fontAsset.ReadFontDefinition();
                }


                // Force the use of the base material
                m_sharedMaterial = m_baseMaterial;
                m_isNewBaseMaterial = true;              


                // If font atlas texture doesn't match the assigned material font atlas, switch back to default material specified in the Font Asset.
                if (m_sharedMaterial == null || m_sharedMaterial.mainTexture == null || m_fontAsset.atlas.GetInstanceID() != m_sharedMaterial.mainTexture.GetInstanceID())
                {                                       
                    m_sharedMaterial = m_fontAsset.material;
                    m_baseMaterial = m_sharedMaterial;
                    m_isNewBaseMaterial = true;
                }                            
            }
            
            // Check & Assign Underline Character for use with the Underline tag.
            if (!m_fontAsset.characterDictionary.TryGetValue(95, out m_cached_Underline_GlyphInfo)) //95
                Debug.LogWarning("Underscore character wasn't found in the current Font Asset. No characters assigned for Underline.");

            
            m_stencilID = MaterialManager.GetStencilID(gameObject);
            if (m_stencilID == 0)
            {
                if (m_maskingMaterial != null)
                {
                    MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                    m_maskingMaterial = null;
                }

                m_sharedMaterial = m_baseMaterial;
            }
            else
            {
                if (m_maskingMaterial == null)
                    m_maskingMaterial = MaterialManager.GetStencilMaterial(m_baseMaterial, m_stencilID);
                else if (m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != m_stencilID || m_isNewBaseMaterial)
                {
                    MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                    m_maskingMaterial = MaterialManager.GetStencilMaterial(m_baseMaterial, m_stencilID);                    
                }

                m_sharedMaterial = m_maskingMaterial;                                       
            }

            m_isNewBaseMaterial = false;
            
            m_sharedMaterials.Add(m_sharedMaterial);
            SetShaderDepth(); // Set ZTestMode based on Canvas RenderMode.

            if (m_uiRenderer == null) m_uiRenderer = GetComponent<CanvasRenderer>();

            m_uiRenderer.SetMaterial(m_sharedMaterial, null);           
            m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
            m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
                                       
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

            Debug.Log("Updating Env Matrix...");
            Vector3 rotation = m_sharedMaterial.GetVector(ShaderUtilities.ID_EnvMatrixRotation);
            m_EnvMapMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rotation), Vector3.one);

            m_sharedMaterial.SetMatrix(ShaderUtilities.ID_EnvMatrix, m_EnvMapMatrix);
        }


        // Enable Masking in the Shader
        void EnableMasking()
        {
            if (m_fontMaterial == null)
            {
                m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
                m_uiRenderer.SetMaterial(m_fontMaterial, null);
            }

            m_sharedMaterial = m_fontMaterial;
            if (m_sharedMaterial.HasProperty(ShaderUtilities.ID_MaskCoord))
            {
                m_sharedMaterial.EnableKeyword("MASK_SOFT");
                m_sharedMaterial.DisableKeyword("MASK_HARD");
                m_sharedMaterial.DisableKeyword("MASK_OFF");
               
                UpdateMask(); // Update Masking Coordinates
            }
            
            m_isMaskingEnabled = true;

            //m_uiRenderer.SetMaterial(m_sharedMaterial, null);

            //m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
            //m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);

            /*
            Material mat = m_uiRenderer.GetMaterial();
            if (mat.HasProperty(ShaderUtilities.ID_MaskCoord))
            {
                mat.EnableKeyword("MASK_SOFT");
                mat.DisableKeyword("MASK_HARD");
                mat.DisableKeyword("MASK_OFF");

                m_isMaskingEnabled = true;
                UpdateMask();
            }
            */
        }


        // Enable Masking in the Shader
        void DisableMasking()
        {
            if (m_fontMaterial != null)
            {
                if (m_stencilID > 0)
                    m_sharedMaterial = m_maskingMaterial;
                else
                    m_sharedMaterial = m_baseMaterial;
                           
                m_uiRenderer.SetMaterial(m_sharedMaterial, null);

                DestroyImmediate(m_fontMaterial);
            }
              
            m_isMaskingEnabled = false;
            
            /*
            if (m_maskingMaterial != null && m_stencilID == 0)
            {                         
                m_sharedMaterial = m_baseMaterial;
                m_uiRenderer.SetMaterial(m_sharedMaterial, null);
            }
            else if (m_stencilID > 0)
            {
                m_sharedMaterial.EnableKeyword("MASK_OFF");
                m_sharedMaterial.DisableKeyword("MASK_HARD");
                m_sharedMaterial.DisableKeyword("MASK_SOFT");
            }
            */
             
          
            /*
            Material mat = m_uiRenderer.GetMaterial();
            if (mat.HasProperty(ShaderUtilities.ID_MaskCoord))
            {
                mat.EnableKeyword("MASK_OFF");
                mat.DisableKeyword("MASK_HARD");
                mat.DisableKeyword("MASK_SOFT");

                m_isMaskingEnabled = false;
                UpdateMask();
            }
            */
        }


        // Update & recompute Mask offset
        void UpdateMask()
        {
            if (m_rectTransform != null)
            {
                //Material mat = m_uiRenderer.GetMaterial();
                //if (mat == null || (m_overflowMode == TextOverflowModes.ScrollRect && m_isScrollRegionSet))
                //    return;

                if (!ShaderUtilities.isInitialized)
                    ShaderUtilities.GetShaderPropertyIDs();
                
                //Debug.Log("Setting Mask for the first time.");

                m_isScrollRegionSet = true;

                float softnessX = Mathf.Min(Mathf.Min(m_margin.x, m_margin.z), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
                float softnessY = Mathf.Min(Mathf.Min(m_margin.y, m_margin.w), m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessY));

                softnessX = softnessX > 0 ? softnessX : 0;
                softnessY = softnessY > 0 ? softnessY : 0;

                float width = (m_rectTransform.rect.width - Mathf.Max(m_margin.x, 0) - Mathf.Max(m_margin.z, 0)) / 2 + softnessX;
                float height = (m_rectTransform.rect.height - Mathf.Max(m_margin.y, 0) - Mathf.Max(m_margin.w, 0)) / 2 + softnessY;

                
                Vector2 center = m_rectTransform.localPosition + new Vector3((0.5f - m_rectTransform.pivot.x) * m_rectTransform.rect.width + (Mathf.Max(m_margin.x, 0) - Mathf.Max(m_margin.z, 0)) / 2, (0.5f - m_rectTransform.pivot.y) * m_rectTransform.rect.height + (-Mathf.Max(m_margin.y, 0) + Mathf.Max(m_margin.w, 0)) / 2);                           
        
                //Vector2 center = m_rectTransform.localPosition + new Vector3((0.5f - m_rectTransform.pivot.x) * m_rectTransform.rect.width + (margin.x - margin.z) / 2, (0.5f - m_rectTransform.pivot.y) * m_rectTransform.rect.height + (-margin.y + margin.w) / 2);
                Vector4 mask = new Vector4(center.x, center.y, width, height);
                //Debug.Log(mask);



                //Rect rect = new Rect(0, 0, m_rectTransform.rect.width + margin.x + margin.z, m_rectTransform.rect.height + margin.y + margin.w);
                //int softness = (int)m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX) / 2;
                m_sharedMaterial.SetVector(ShaderUtilities.ID_MaskCoord, mask);
            }
        }

     
   
        // Function called internally when a new material is assigned via the fontMaterial property.
        void SetFontMaterial(Material mat)
        {
            // Get Shader PropertyIDs if they haven't been cached already.
            ShaderUtilities.GetShaderPropertyIDs();
            
            // Check in case Object is disabled. If so, we don't have a valid reference to the Renderer.
            // This can occur when the Duplicate Material Context menu is used on an inactive object.
            if (m_uiRenderer == null)
                m_uiRenderer = GetComponent<CanvasRenderer>();

            // Destroy previous instance material.
            if (m_fontMaterial != null) DestroyImmediate(m_fontMaterial);

            // Release masking material
            if (m_maskingMaterial != null)
            {
                MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                m_maskingMaterial = null;
            } 

            // Get Masking ID
            m_stencilID = MaterialManager.GetStencilID(gameObject);
  
            // Create Instance Material
            m_fontMaterial = CreateMaterialInstance(mat);
            
            if (m_stencilID > 0)
                m_fontMaterial = MaterialManager.SetStencil(m_fontMaterial, m_stencilID);
                
                           
            m_sharedMaterial = m_fontMaterial;
            SetShaderDepth(); // Set ZTestMode based on Canvas RenderMode.
            
            m_uiRenderer.SetMaterial(m_sharedMaterial, null);
                   
            m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
            m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
        }


        // Function called internally when a new shared material is assigned via the fontSharedMaterial property.
        void SetSharedFontMaterial(Material mat) 
        {
            ShaderUtilities.GetShaderPropertyIDs();

            // Check in case Object is disabled. If so, we don't have a valid reference to the Renderer.
            // This can occur when the Duplicate Material Context menu is used on an inactive object. 
            if (m_uiRenderer == null)
                m_uiRenderer = GetComponent<CanvasRenderer>();

            if (mat == null) { mat = m_baseMaterial; m_isNewBaseMaterial = true; }

     
            // Handle UI Mask
            m_stencilID = MaterialManager.GetStencilID(gameObject);
            if (m_stencilID == 0)
            {
                if (m_maskingMaterial != null)
                {
                    MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                    m_maskingMaterial = null;
                }

                m_baseMaterial = mat; // Can Material be a Masking Material?
            }
            else
            {
                if (m_maskingMaterial == null)
                    m_maskingMaterial = MaterialManager.GetStencilMaterial(mat, m_stencilID);
                else if (m_maskingMaterial.GetInt(ShaderUtilities.ID_StencilID) != m_stencilID || m_isNewBaseMaterial)
                {
                    MaterialManager.ReleaseStencilMaterial(m_maskingMaterial);
                    m_maskingMaterial = MaterialManager.GetStencilMaterial(mat, m_stencilID);
                }

                mat = m_maskingMaterial;
            }

            m_isNewBaseMaterial = false;


            m_sharedMaterial = mat;
            SetShaderDepth(); // Set ZTestMode based on Canvas RenderMode.
            
            m_uiRenderer.SetMaterial(m_sharedMaterial, null);                     
            m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
            m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);

            //Debug.Log("New Material [" + mat.name + "] with ID " + mat.GetInstanceID() + " has been assigned. Base Material is [" + m_baseMaterial.name + "] with ID " + m_baseMaterial.GetInstanceID());
            
        }


        void SetFontBaseMaterial(Material mat)
        {
            Debug.Log("Changing Base Material from [" + (m_lastBaseMaterial == null ? "Null" : m_lastBaseMaterial.name) + "] to [" + mat.name + "].");
        
            // Remove reference to masking material for this base material if one exists.
            //if (m_maskingMaterial != null)           
            //    MaterialManager.ReleaseMaskingMaterial(m_lastBaseMaterial);
            
            // Assign new Base Material
            m_baseMaterial = mat;
            m_lastBaseMaterial = mat;

            // Check if Masking is enabled and if so assign the masking material.
            //if (m_mask != null && m_mask.enabled)
            //{
                //if (m_maskingMaterial == null)
            //    m_maskingMaterial = MaterialManager.GetMaskingMaterial(mat, 1);
            
            //    fontSharedMaterial = m_maskingMaterial;
            //}

            //m_isBaseMaterialChanged = false;
        }


      
        // This function will create an instance of the Font Material.
        void SetOutlineThickness(float thickness)
        {
            // Check if we need to create an instance of the material
            if (m_fontMaterial == null)
            {
                m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
                m_uiRenderer.SetMaterial(m_fontMaterial, null);
            }

            // Check to make sure we still have a Material assigned to the CanvasRenderer
            //if (m_uiRenderer.GetMaterial() == null)
            //    m_uiRenderer.SetMaterial(m_fontMaterial, null);
                       
            thickness = Mathf.Clamp01(thickness);
            m_uiRenderer.GetMaterial().SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);                   
        }


        // This function will create an instance of the Font Material.
        void SetFaceColor(Color32 color)
        {           
            // Check if we need to create an instance of the material
            if (m_fontMaterial == null)
            {
                m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
                m_uiRenderer.SetMaterial(m_fontMaterial, null);
            }
            
            // Check to make sure we still have a Material assigned to the CanvasRenderer
            //if (m_uiRenderer.GetMaterial() == null)
            //    m_uiRenderer.SetMaterial(m_fontMaterial, null);
            
            m_uiRenderer.GetMaterial().SetColor(ShaderUtilities.ID_FaceColor, color);                 
        }


        // This function will create an instance of the Font Material.
        void SetOutlineColor(Color32 color)
        {
            // Check if we need to create an instance of the material
            if (m_fontMaterial == null)
            {
                m_fontMaterial = CreateMaterialInstance(m_sharedMaterial);
                m_uiRenderer.SetMaterial(m_fontMaterial, null);
            }

            // Check to make sure we still have a Material assigned to the CanvasRenderer
            //if (m_uiRenderer.GetMaterial() == null)
            //    m_uiRenderer.SetMaterial(m_fontMaterial, null);
                  
            m_uiRenderer.GetMaterial().SetColor(ShaderUtilities.ID_OutlineColor, color);          
        }


        // Function used to create an instance of the material
        Material CreateMaterialInstance(Material source)
        {          
            Material mat = new Material(source);
            mat.shaderKeywords = source.shaderKeywords;
            
            mat.hideFlags = HideFlags.DontSave;
            mat.name += " (Instance)";
            //m_uiRenderer.SetMaterial(mat, null);
            //m_fontMaterial = mat;

            return mat;
        }


        // Sets the Render Queue and Ztest mode 
        void SetShaderDepth()
        {
            if (m_canvas == null)
                return;
            
            if (m_canvas.renderMode == RenderMode.ScreenSpaceOverlay || m_isOverlay)
            {
                m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 0);
                //m_renderer.material.SetFloat("_ZTestMode", 8);
                //m_renderer.material.renderQueue = 4000;

                //m_sharedMaterial = m_renderer.material;
            }
            else
            {   // TODO: This section needs to be tested.
                m_sharedMaterial.SetFloat(ShaderUtilities.ShaderTag_ZTestMode, 4);
            }
        }


        // Sets the Culling mode of the material
        void SetCulling()
        {
            if (m_isCullingEnabled)
            {                        
                m_uiRenderer.GetMaterial().SetFloat("_CullMode", 2);
            }
            else
            {
                m_uiRenderer.GetMaterial().SetFloat("_CullMode", 0);
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


        // Function to allocate the necessary buffers to render the text. This function is called whenever the buffer size needs to be increased.
        void SetMeshArrays(int size)
        {
            // Should add a check to make sure we don't try to create a mesh that contains more than 65535 vertices.

            int sizeX4 = size * 4;

            m_uiVertices = new UIVertex[sizeX4];

            // Setup Triangle Structure 
            for (int i = 0; i < size; i++)
            {
                int index_X4 = i * 4;
                //int index_X6 = i * 6;

                m_uiVertices[0 + index_X4].position = Vector3.zero;
                m_uiVertices[1 + index_X4].position = Vector3.zero;
                m_uiVertices[2 + index_X4].position = Vector3.zero;
                m_uiVertices[3 + index_X4].position = Vector3.zero;

                m_uiVertices[0 + index_X4].normal = new Vector3(0, 0, -1);
                m_uiVertices[1 + index_X4].normal = new Vector3(0, 0, -1);
                m_uiVertices[2 + index_X4].normal = new Vector3(0, 0, -1);
                m_uiVertices[3 + index_X4].normal = new Vector3(0, 0, -1);

                m_uiVertices[0 + index_X4].tangent = new Vector4(-1, 0, 0, 1);
                m_uiVertices[1 + index_X4].tangent = new Vector4(-1, 0, 0, 1);
                m_uiVertices[2 + index_X4].tangent = new Vector4(-1, 0, 0, 1);
                m_uiVertices[3 + index_X4].tangent = new Vector4(-1, 0, 0, 1);
            }

            //Debug.Log("Size:" + size + "  Vertices:" + m_vertices.Length + "  Triangles:" + m_triangles.Length + " Mesh - Vertices:" + m_mesh.vertices.Length + "  Triangles:" + m_mesh.triangles.Length);

            m_uiRenderer.SetVertices(m_uiVertices, sizeX4);
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
                        case 114: // \r
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
            int spriteCount = 0;
            m_isUsingBold = false;
            m_isSprite = false;
            m_fontIndex = 0;

            m_VisibleCharacters.Clear();
            Array.Clear(m_meshAllocCount, 0, 17);

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

                        if (m_isSprite) { spriteCount += 1; totalCount += 1; m_VisibleCharacters.Add((char)(57344 + m_spriteIndex)); m_isSprite = false; }

                        continue;
                    }
                }

                if (c != 32 && c != 9 && c != 10 && c != 13)
                {
                    visibleCount += 1;
                
                    // Track how many characters per mesh
                    m_meshAllocCount[m_fontIndex] += 1;                  

                }

                m_VisibleCharacters.Add((char)c);   
                totalCount += 1;
            }


            // Allocated secondary vertex buffers for InlineGraphic Component if present.
            if (spriteCount > 0)
            {
                if (m_inlineGraphics == null) m_inlineGraphics = GetComponent<InlineGraphicManager>() ?? gameObject.AddComponent<InlineGraphicManager>();

                m_inlineGraphics.AllocatedVertexBuffers(spriteCount);
            }
            else if (m_inlineGraphics != null)         
                m_inlineGraphics.ClearUIVertex();
            
           
            m_spriteCount = spriteCount;
                              

            if (m_textInfo.characterInfo == null || totalCount > m_textInfo.characterInfo.Length)
            {                
                m_textInfo.characterInfo = new TMP_CharacterInfo[totalCount > 1024 ? totalCount + 256 : Mathf.NextPowerOfTwo(totalCount)];              
            }

            // Make sure our Mesh Buffer Allocations can hold these new Quads.
            if (m_uiVertices == null) m_uiVertices = new UIVertex[0];
            if (visibleCount * 4 > m_uiVertices.Length)
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

            /*
            // Make sure our Mesh Array has enough capacity for the different fonts
            if (m_textInfo.meshInfo.meshArrays == null) m_textInfo.meshInfo.meshArrays = new UIVertex[17][];
            for (int i = 0; i < 17; i++ )
            {
                if (m_textInfo.meshInfo.meshArrays[i] == null || m_textInfo.meshInfo.meshArrays[i].Length < m_meshAllocCount[i])
                {
                    int arraySize = m_meshAllocCount[i] * 4;
                    m_textInfo.meshInfo.meshArrays[i] = new UIVertex[arraySize > 1024 ? arraySize + 256 : Mathf.NextPowerOfTwo(arraySize)];
                }

                if (i > 0 && m_meshAllocCount[i] > 0)
                {
                    if (subObjects[i] == null)
                    {
                        subObjects[i] = new GameObject("Font #" + i, typeof(CanvasRenderer));
                        RectTransform rectTransform = subObjects[i].AddComponent<RectTransform>();
                        rectTransform.SetParent(m_rectTransform);
                        rectTransform.localPosition = Vector3.zero;
                        rectTransform.sizeDelta = Vector2.zero;
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchorMax = Vector2.one;
                    }
                }
            }
            */

            return totalCount;
        }


        // Added to sort handle the potential issue with OnWillRenderObject() not getting called when objects are not visible by camera.
        //void OnBecameInvisible()
        //{
        //    if (m_mesh != null)
        //        m_mesh.bounds = new Bounds(transform.position, new Vector3(1000, 1000, 0));
        //}


        // Method used to mark layout for rebuild
        void MarkLayoutForRebuild()
        {
            //Debug.Log("MarkLayoutForRebuild() called.");
            
            if (m_rectTransform == null)
                m_rectTransform = GetComponent<RectTransform>();

            LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
        }


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

      
        void ComputeMarginSize()
        {                      
                      
            if (m_rectTransform != null)
            {              
                //Debug.Log("Computing new margins. Current RectTransform's Width is " + m_rectTransform.rect.width + " and Height is " + m_rectTransform.rect.height); // + "  Preferred Width: " + m_preferredWidth + " Height: " + m_preferredHeight);                            

                if (m_rectTransform.rect.width == 0) m_marginWidth = Mathf.Infinity;
                else
                    m_marginWidth = m_rectTransform.rect.width - m_margin.x - m_margin.z;
                
                if (m_rectTransform.rect.height == 0) m_marginHeight = Mathf.Infinity;                 
                else                   
                    m_marginHeight = m_rectTransform.rect.height - m_margin.y - m_margin.w;
                            
            }
        }


        protected override void OnDidApplyAnimationProperties()
        {           
            havePropertiesChanged = true;
            //Debug.Log("Animation Properties have changed.");
        }


        protected override void OnRectTransformDimensionsChange()
        {
            //Debug.Log("**** OnRectTransformDimensionsChange() **** isRebuildingLayout = " + (m_isRebuildingLayout ? "True" : "False") + "."); // Rect: " + m_rectTransform.rect); //  called on Object ID " + GetInstanceID());
            
            // Need to add code to figure out if the result of these changes should be processed immediately or the next time the frame is rendered.

            // Make sure object is active in Hierarachy
            if (!this.gameObject.activeInHierarchy)
                return;

            ComputeMarginSize();
         
            if (m_rectTransform != null)
                m_rectTransform.hasChanged = true;
            else
            {
                m_rectTransform = GetComponent<RectTransform>();
                m_rectTransform.hasChanged = true;
            }

            //m_rectTransformDimensionsChanged = true;
            
            //Debug.Log("OnRectTransformDimensionsChange() called. New Width: " + m_rectTransform.rect.width + "  Height: " + m_rectTransform.rect.height);

            if (m_isRebuildingLayout)
                m_isLayoutDirty = true;
            else
                havePropertiesChanged = true;
        } 

        

        // Called just before the Canvas is rendered.
        void OnPreRenderCanvas()
        {       
            loopCountA = 0;
            //loopCountB = 0;
            //loopCountC = 0;
            //loopCountD = 0;
            //loopCountE = 0;
          
            //Debug.Log("***** OnPreRenderCanvas() ***** Frame: " + Time.frameCount + "  Rect: " + m_rectTransform.rect); // Assigned Material is " + m_uiRenderer.GetMaterial().name); // isInputParsingRequired = " + isInputParsingRequired);                    

            //Debug.Log("Awake = " + m_isAwake + "  Enabled = " + m_isEnabled);
            // If Object is not enabled, simply return.
            //if (m_isEnabled == false)
            //    return;

            if (m_fontAsset == null)
                return;
            
            // Check if Transform has changed since last update.          
            if (m_rectTransform.hasChanged || m_marginsHaveChanged)
            {               
                //Debug.Log("RectTransform has changed."); // Current Width: " + m_rectTransform.rect.width + " and  Height: " + m_rectTransform.rect.height);

                // Update Pivot of Inline Graphic Component if Pivot has changed.
                // TODO : Should probably also update anchors
                if (m_inlineGraphics != null)               
                    m_inlineGraphics.UpdatePivot(m_rectTransform.pivot);
                   
                
				// If Dimension Changed or Margin (Regenerate the Mesh)              
                if (m_rectTransformDimensionsChanged || m_marginsHaveChanged)
                {                    
                    //Debug.Log("RectTransform Dimensions or Margins have changed.");
                    ComputeMarginSize();

                    if (m_marginsHaveChanged)                                                                                 
                       m_isScrollRegionSet = false;
                    

                    m_rectTransformDimensionsChanged = false;                 
                    m_marginsHaveChanged = false;
                    m_isCalculateSizeRequired = true;

                    havePropertiesChanged = true;                  
                }

                // Update Mask
                if (m_isMaskingEnabled)
                {
                    UpdateMask();
                }
            
                m_rectTransform.hasChanged = false;
                

                // We need to regenerate the mesh if the lossy scale has changed.
                Vector3 currentLossyScale = m_rectTransform.lossyScale;
                if (currentLossyScale != m_previousLossyScale)
                {
                    // Update UV2 Scale - only if we don't have to regenerate the text object.
                    if (havePropertiesChanged == false && m_previousLossyScale.z != 0 && m_text != string.Empty)
                        UpdateSDFScale(m_previousLossyScale.z, currentLossyScale.z);
                    else
                        havePropertiesChanged = true;

                    m_previousLossyScale = currentLossyScale;
                }               
            }
                     

            if (havePropertiesChanged || m_fontAsset.propertiesChanged || m_isLayoutDirty)
            {
                //Debug.Log("Properties have changed!"); // Assigned Material is:" + m_sharedMaterial); // New Text is: " + m_text + ".");                
                
                // Make sure Text Object is parented to a Canvas.
                if (m_canvas == null) m_canvas = GetComponentInParent<Canvas>();
                if (m_canvas == null) return;


                if (hasFontAssetChanged || m_fontAsset.propertiesChanged)
                {
                    //Debug.Log("Font Asset has changed. Loading new font asset."); 
                    
                    LoadFontAsset();

                    hasFontAssetChanged = false;

                    if (m_fontAsset == null || m_uiRenderer.GetMaterial() == null)
                        return;

                    m_fontAsset.propertiesChanged = false;
                }

                
                //if (m_isMaskingEnabled)
                //{                                                      
                //    UpdateMask();                  
                //}
                

                // Reparse the text if the input has changed.
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

               
                // Object regeneration will be handled by Layout Component if present
                //if (m_isCalculateSizeRequired) // && m_isEnabled)
                //{
                //    if (/* m_layoutController as UIBehaviour != null && (m_layoutController as UIBehaviour).enabled && */ m_layoutController as AspectRatioFitter == null)
                //    {
                        //LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
                        //Debug.Log("Calling MarkLayoutForRebuild() at Frame: " + Time.frameCount);
                        //return;
                //    }
                //}

                //if (m_isLayoutDirty) { Debug.Log("Regenerating Text since layout was dirty."); }
                m_isLayoutDirty = false;

                GenerateTextMesh();
                havePropertiesChanged = false;    
            }
        }

        
    
        /// <summary>
        /// This is the main function that is responsible for creating / displaying the text.
        /// </summary>
        void GenerateTextMesh()
        {
            //Debug.Log("***** GenerateTextMesh() ***** Frame: " + Time.frameCount + ". Point Size: " + m_fontSize + ". Margins are (W) " + m_marginWidth + "  (H) " + m_marginHeight); // ". Iteration Count: " + loopCountA + ".  Min: " + m_minFontSize + "  Max: " + m_maxFontSize + "  Delta: " + (m_maxFontSize - m_minFontSize) + "  Font size is " + m_fontSize); //called for Object with ID " + GetInstanceID()); // Assigned Material is " + m_uiRenderer.GetMaterial().name); // IncludeForMasking " + this.m_IncludeForMasking); // and text is " + m_text);
            //Debug.Log(this.defaultMaterial.GetInstanceID() + "  " + m_sharedMaterial.GetInstanceID() + "  " + m_uiRenderer.GetMaterial().GetInstanceID());
            
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
            if (m_char_buffer == null || m_char_buffer.Length == 0 || m_char_buffer[0] == (char)0)
            {
                //Debug.Log("Early Out! No Text has been set.");
                if (m_uiVertices != null)
                {
                    //m_uiRenderer.Clear();
                    m_uiRenderer.SetVertices(m_uiVertices, 0);
                    if (m_inlineGraphics != null) m_inlineGraphics.ClearUIVertex();
                }

                m_preferredWidth = 0;
                m_preferredHeight = 0;
                m_renderedWidth = 0;
                m_renderedHeight = 0;

                // This should only be called if there is a layout component attached
                LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
                return;
            }

            // Determine how many characters will be visible and make the necessary allocations (if needed).
            int totalCharacterCount = SetArraySizes(m_char_buffer);

            m_fontIndex = 0; // Will be used when support for using different font assets or sprites withint the same object will be added.
            m_fontAssetArray[m_fontIndex] = m_fontAsset;

            // Scale the font to approximately match the point size           
            m_fontScale = (m_fontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize);
            float baseScale = m_fontScale; // BaseScale keeps the character aligned vertically since <size=+000> results in font of different scale.
            m_maxFontScale = 0;
            float previousFontScale = 0;
            float spriteScale = 1;
            m_currentFontSize = m_fontSize;
            float fontSizeDelta = 0;

            int charCode = 0; // Holds the character code of the currently being processed character.
            //int prev_charCode = 0;
            bool isMissingCharacter; // Used to handle missing characters in the Font Atlas / Definition.

            //bool isLineTruncated = false;

            m_style = m_fontStyle; // Set the default style.
            m_lineJustification = m_textAlignment; // Sets the line justification mode to match editor alignment.

            // GetPadding to adjust the size of the mesh due to border thickness, softness, glow, etc...
            if (checkPaddingRequired)
            {
                checkPaddingRequired = false;
                m_padding = ShaderUtilities.GetPadding(m_uiRenderer.GetMaterial(), m_enableExtraPadding, m_isUsingBold);
                m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
                m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
            }

            //float base_padding = m_padding;
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
            m_cSpacing = 0; // Amount of space added between characters as a result of the use of the <cspace> tag.
            m_monoSpacing = 0;
            float lineOffsetDelta = 0;
            m_xAdvance = 0; // Used to track the position of each character.
            m_indent = 0; // Used for indentation of the text.
            m_maxXAdvance = 0;

            m_lineNumber = 0;
            m_pageNumber = 0;
            //int previousPageNumber = -1;
            m_characterCount = 0; // Total characters in the char[]
            m_visibleCharacterCount = 0; // # of visible characters.
            m_visibleSpriteCount = 0;

            // Limit Line Length to whatever size fits all characters on a single line.          
            m_firstVisibleCharacterOfLine = 0;
            m_lastVisibleCharacterOfLine = 0;

            int ellipsisIndex = 0;

            m_rectTransform.GetLocalCorners(m_rectCorners); // m_textContainer.corners;
            //Debug.Log (corners [0] + "  " + corners [2]);
            Vector4 margins = m_margin; // _textContainer.margins;
            float marginWidth = m_marginWidth; // m_rectTransform.rect.width - margins.z - margins.x;
            float marginHeight = m_marginHeight; // m_rectTransform.rect.height - margins.y - margins.w;            
            m_width = 0;
            
            // Used by Unity's Auto Layout system.
            m_renderedWidth = 0;
            m_renderedHeight = 0;

            // Initialize struct to track states of word wrapping
            bool isFirstWord = true;
            bool isLastBreakingChar = false;
            m_SavedLineState = new WordWrapState();
            m_SavedWordWrapState = new WordWrapState();   
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
            float pageAscender = 0;
            //float lineDescender = 0;
            m_isNewPage = false;

            //bool isLineOffsetAdjusted = false;
            loopCountA += 1;

            int endTagIndex = 0;
            // Parse through Character buffer to read html tags and begin creating mesh.
            for (int i = 0; m_char_buffer[i] != 0; i++)
            {
                //m_tabSpacing = -999;
                //m_spacing = -999;
                charCode = m_char_buffer[i];
                m_isSprite = false;
                spriteScale = 1;

                //Debug.Log("i:" + i + "  Character [" + (char)charCode + "] with ASCII of " + charCode);

                // Parse Rich Text Tag
                #region Parse Rich Text Tag
                if (m_isRichText && charCode == 60)  // '<'
                {
                    // Check if Tag is valid. If valid, skip to the end of the validated tag.
                    if (ValidateHtmlTag(m_char_buffer, i + 1, out endTagIndex))
                    {
                        i = endTagIndex;

                        if (m_isRecalculateScaleRequired)
                        {
                            m_fontScale = m_currentFontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize;
                            //isAffectingWordWrapping = true;
                            m_isRecalculateScaleRequired = false;
                        }

                        //if (m_tabSpacing != -999)
                        //{
                        //    // Move character to a fix position. Position expresses in characters (approximation).
                        //    m_xAdvance = m_tabSpacing * m_cached_Underline_GlyphInfo.width * m_fontScale;
                        //}

                        //if (m_spacing != -999)
                        //{
                        //    m_xAdvance += m_spacing * m_fontScale * m_cached_Underline_GlyphInfo.width;
                        //}

                        if (!m_isSprite)
                            continue;
                    }
                }
                #endregion End Parse Rich Text Tag

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
                //Debug.Log("Char [" + (char)charCode + "] is using FontIndex: " + m_fontIndex);



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
                        m_fontScale = m_currentFontSize * 0.8f / m_fontAssetArray[m_fontIndex].fontInfo.PointSize;
                        charCode -= 32;
                    }
                    else
                        m_fontScale = m_currentFontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize;

                }
                #endregion


                // Look up Character Data from Dictionary and cache it.
                #region Look up Character Data
                if (m_isSprite)
                {
                    SpriteInfo spriteInfo = m_inlineGraphics.GetSprite(m_spriteIndex);
                    if (spriteInfo == null) continue;

                    // Sprites are assigned in the E000 Private Area + sprite Index
                    charCode = 57344 + m_spriteIndex;

                    m_cached_GlyphInfo = new GlyphInfo(); // Generates 40 BYTE_HELPER

                    m_cached_GlyphInfo.x = spriteInfo.x;
                    m_cached_GlyphInfo.y = spriteInfo.y;
                    m_cached_GlyphInfo.width = spriteInfo.width;
                    m_cached_GlyphInfo.height = spriteInfo.height;
                    m_cached_GlyphInfo.xOffset = spriteInfo.pivot.x + spriteInfo.xOffset;
                    m_cached_GlyphInfo.yOffset = spriteInfo.pivot.y + spriteInfo.yOffset;                

                    spriteScale = m_fontAsset.fontInfo.Ascender / spriteInfo.height * spriteInfo.scale;
                   
                    m_cached_GlyphInfo.xAdvance = spriteInfo.xAdvance * spriteScale;
                   
                    m_visibleSpriteCount += 1;

                    m_textInfo.characterInfo[m_characterCount].type = TMP_CharacterType.Sprite;             
                }
                else
                {
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

                    m_textInfo.characterInfo[m_characterCount].type = TMP_CharacterType.Character;         
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


                // Set Padding based on selected font style
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


                // Set padding value if Character or Sprite                        
                float padding = m_isSprite ? m_enableExtraPadding ? 4 : 0 : m_padding;

                // Determine the position of the vertices of the Character or Sprite.   
                Vector3 top_left = new Vector3(0 + m_xAdvance + ((m_cached_GlyphInfo.xOffset - padding - style_padding) * m_fontScale * spriteScale), (m_cached_GlyphInfo.yOffset + padding) * m_fontScale * spriteScale - m_lineOffset + m_baselineOffset, 0);
                Vector3 bottom_left = new Vector3(top_left.x, top_left.y - ((m_cached_GlyphInfo.height + padding * 2) * m_fontScale * spriteScale), 0);
                Vector3 top_right = new Vector3(bottom_left.x + ((m_cached_GlyphInfo.width + padding * 2 + style_padding * 2) * m_fontScale * spriteScale), top_left.y, 0);
                Vector3 bottom_right = new Vector3(top_right.x, bottom_left.y, 0);

                // Check if we need to Shear the rectangles for Italic styles
                #region Handle Italic & Shearing
                if ((m_style & FontStyles.Italic) == FontStyles.Italic || (m_fontStyle & FontStyles.Italic) == FontStyles.Italic)
                {
                    // Shift Top vertices forward by half (Shear Value * height of character) and Bottom vertices back by same amount. 
                    float shear_value = m_fontAssetArray[m_fontIndex].ItalicStyle * 0.01f;
                    Vector3 topShear = new Vector3(shear_value * ((m_cached_GlyphInfo.yOffset + padding + style_padding) * m_fontScale * spriteScale), 0, 0);
                    Vector3 bottomShear = new Vector3(shear_value * (((m_cached_GlyphInfo.yOffset - m_cached_GlyphInfo.height - padding - style_padding)) * m_fontScale * spriteScale), 0, 0);

                    top_left = top_left + topShear;
                    bottom_left = bottom_left + bottomShear;
                    top_right = top_right + topShear;
                    bottom_right = bottom_right + bottomShear;
                }
                #endregion Handle Italics & Shearing


                // Store position of the vertices for the Character or Sprite.            
                m_textInfo.characterInfo[m_characterCount].bottomLeft = bottom_left;
                m_textInfo.characterInfo[m_characterCount].topLeft = top_left;
                m_textInfo.characterInfo[m_characterCount].topRight = top_right;
                m_textInfo.characterInfo[m_characterCount].bottomRight = bottom_right;


                // Compute MaxAscender & MaxDescender which is used for AutoScaling & other type layout options
                float ascender = (m_fontAsset.fontInfo.Ascender + m_alignmentPadding.y) * m_fontScale + m_baselineOffset;
                float descender = (m_fontAsset.fontInfo.Descender + m_alignmentPadding.w) * m_fontScale - m_lineOffset + m_baselineOffset;

                // Check if Sprite exceeds the Ascender and Descender of the font and if so make the adjustment.
                if (m_isSprite)
                {                  
                    ascender = Mathf.Max(ascender, top_left.y - padding * m_fontScale * spriteScale);
                    descender = Mathf.Min(descender, bottom_right.y - padding * m_fontScale * spriteScale);
                }

                if (m_lineNumber == 0) m_maxAscender = m_maxAscender > ascender ? m_maxAscender : ascender;
                if (m_lineOffset == 0) pageAscender = pageAscender > ascender ? pageAscender : ascender;


                // Set Characters to not visible by default.
                m_textInfo.characterInfo[m_characterCount].isVisible = false;


                // Setup Mesh for visible characters or sprites. ie. not a SPACE / LINEFEED / CARRIAGE RETURN.
                #region Handle Visible Characters
                if (charCode != 32 && charCode != 9 && charCode != 10 && charCode != 13 || m_isSprite)
                {

                    m_textInfo.characterInfo[m_characterCount].isVisible = true;

                    // Used to adjust line spacing when larger fonts or the size tag is used.
                    if (m_baselineOffset == 0)
                        m_maxFontScale = Mathf.Max(m_maxFontScale, m_fontScale);


                    // Check if Character exceeds the width of the Text Container or is first character of line              
                    #region Check for Characters Exceeding Width of Text Container
                    //float width = Mathf.Min(marginWidth + 0.0001f, m_width == 0 ? Mathf.Infinity : m_width);
                    float width = marginWidth + 0.0001f;
                    if (m_xAdvance + m_cached_GlyphInfo.xAdvance * m_fontScale > width)
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
                                    m_isCharacterWrappingEnabled = true;
                                    //Debug.Log("Enabling Character Wrapping.");                                                                  
                                }
                                else
                                    isLastBreakingChar = true;

                                
                                //Debug.Log("Warpping Index " + wrappingIndex + ". Recursive Count: " + m_recursiveCount);

                                m_recursiveCount += 1;
                                if (m_recursiveCount > 20)
                                {
                                    Debug.Log("Recursive count exceeded!");
                                    continue;
                                }

                                //Debug.Log("Line #" + m_lineNumber + " Character [" + (char)charCode + "] cannot be wrapped.  WrappingIndex: " + wrappingIndex + "  Saved Index: " + m_SavedWordWrapState.previous_WordBreak + ". Character Count is " + m_characterCount);                               
                            }


                            // Restore to previously stored state of last valid (space character or linefeed)
                            i = RestoreWordWrappingState(ref m_SavedWordWrapState);
                            wrappingIndex = i;  // Used to dectect when line length can no longer be reduced.

                            //Debug.Log("Last Visible Character of line # " + m_lineNumber + " is [" + m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].character + " Character Count: " + m_characterCount + " Last visible: " + m_lastVisibleCharacterOfLine);

                            // Check if we need to Adjust LineOffset & Restore State to the start of the line.                            
                            if (m_lineNumber > 0 && m_maxFontScale != 0 && m_lineHeight == 0 && m_maxFontScale != previousFontScale && !m_isNewPage)
                            {
                                // Compute Offset
                                float gap = m_fontAssetArray[m_fontIndex].fontInfo.LineHeight - (m_fontAssetArray[m_fontIndex].fontInfo.Ascender - m_fontAssetArray[m_fontIndex].fontInfo.Descender);
                                float offsetDelta = (m_fontAssetArray[m_fontIndex].fontInfo.Ascender + m_lineSpacing + m_paragraphSpacing + gap + m_lineSpacingDelta) * m_maxFontScale - (m_fontAssetArray[m_fontIndex].fontInfo.Descender - gap) * previousFontScale;
                                m_lineOffset += offsetDelta - lineOffsetDelta;
                                AdjustLineOffset(m_firstVisibleCharacterOfLine, m_characterCount - 1, offsetDelta - lineOffsetDelta);
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
                            m_textInfo.lineInfo[m_lineNumber].lineLength = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - padding * m_maxFontScale;
                            m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - m_characterSpacing * m_fontScale;


                            m_firstVisibleCharacterOfLine = m_characterCount; // Store first character for the next line.


                            // Compute Preferred Width & Height                                                     
                            m_renderedWidth += m_xAdvance; // m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex].topRight.x - m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex].bottomLeft.x;
                            if (m_enableWordWrapping)
                                m_renderedHeight = m_maxAscender - m_maxDescender;
                            else
                                m_renderedHeight = Mathf.Max(m_renderedHeight, lineAscender - lineDescender);

                            //Debug.Log("LineInfo for line # " + (m_lineNumber) + " First character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex +
                            //                                                    " Last character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex +
                            //                                                    " Last Visible character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex +
                            //                                                    " Character Count of " + m_textInfo.lineInfo[m_lineNumber].characterCount /* + " Line Lenght of " + m_textInfo.lineInfo[m_lineNumber].lineLength +
                            //                                                    "  MinX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.x + "  MinY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.y +
                            //                                                    "  MaxX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x + "  MaxY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.y +
                            //                                                    "  Line Ascender: " + lineAscender + "  Line Descender: " + lineDescender */ );

                            // Store the state of the line before starting on the new line.                        
                            SaveWordWrappingState(ref m_SavedLineState, i, m_characterCount - 1);


                            m_lineNumber += 1;
                            // Check to make sure Array is large enough to hold a new line.
                            if (m_lineNumber >= m_textInfo.lineInfo.Length)
                                ResizeLineExtents(m_lineNumber);

                            // Apply Line Spacing based on scale of the last character of the line.
                            if (m_lineHeight == 0)
                            {
                                lineOffsetDelta = (m_fontAssetArray[m_fontIndex].fontInfo.LineHeight + m_lineSpacing + m_lineSpacingDelta) * m_fontScale;
                                m_lineOffset += lineOffsetDelta;
                            }
                            else
                                m_lineOffset += (m_lineHeight + m_lineSpacing) * baseScale;


                            previousFontScale = m_fontScale;
                            m_xAdvance = 0 + m_indent;
                            spriteScale = 1;
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
                                //m_visibleCharacterCount -= 1;
                                break;
                        }
                        #endregion End Text Overflow

                    }
                    #endregion End Check for Characters Exceeding Width of Text Container


                    // Determine Vertex Color                                                    
                    if (isMissingCharacter)
                        vertexColor = Color.red;
                    else if (m_overrideHtmlColors)
                        vertexColor = m_fontColor32;
                    else
                        vertexColor = m_htmlColor;


                    // Store Character & Sprite Vertex Information
                    if (!m_isSprite)
                        SaveGlyphVertexInfo(style_padding, vertexColor);
                    else
                        SaveSpriteVertexInfo(vertexColor);


                    // Increase visible count for Characters.
                    if (!m_isSprite) m_visibleCharacterCount += 1;

                    if (m_textInfo.characterInfo[m_characterCount].isVisible || m_isSprite)
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
                if (m_maxAscender - descender + (m_alignmentPadding.w * 2 * m_fontScale) > marginHeight)
                {
                    //Debug.Log((m_maxAscender - m_maxDescender) + "  " + marginHeight);
                    //Debug.Log("Character [" + (char)charCode + "] at Index: " + m_characterCount + " has exceeded the Height of the text container. Max Ascender: " + m_maxAscender + "  Max Descender: " + m_maxDescender + "  Margin Height: " + marginHeight + " Bottom Left: " + bottom_left.y);                                              

                    // Handle Linespacing adjustments
                    #region Line Spacing Adjustments
                    if (m_enableAutoSizing && m_lineSpacingDelta > m_lineSpacingMax && m_lineNumber > 0)
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
                    
                            // Alternative Implementation                            
                            //if (m_lineNumber > 0)
                            //{                       
                            //    if (!m_isTextTruncated && m_textInfo.characterInfo[ellipsisIndex + 1].character != 10)
                            //    {
                            //        Debug.Log("Char [" + (char)charCode + "] on line " + m_lineNumber + " exceeds the vertical bounds. Last safe character was " + (int)m_textInfo.characterInfo[ellipsisIndex + 1].character);
                            //        i = RestoreWordWrappingState(ref m_SavedWordWrapState);
                            //        m_lineNumber -= 1;
                            //        m_isTextTruncated = true;
                            //        m_isCharacterWrappingEnabled = true;
                            //        continue;
                            //    }
                            //    else
                            //    {
                            //        //Debug.Log("Char [" + (char)charCode + "] on line " + m_lineNumber + " set to invisible.");
                            //        m_textInfo.characterInfo[m_characterCount].isVisible = false;
                            //    }
                            ////    m_char_buffer[m_textInfo.characterInfo[ellipsisIndex].index + 1] = (char)0;
                            ////    m_isTextTruncated = true;
                            ////    i = RestoreWordWrappingState(ref m_SavedLineState);
                            ////    m_lineNumber -= 1;
                            
                            ////    continue;
                            //}
                            //break;

                            
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

                            // Ignore Page Break, Linefeed or carriage return
                            if (charCode == 13 || charCode == 10)
                                break;

                            //Debug.Log("Character is [" + (char)charCode + "] with ASCII (" + charCode + ") on Page " + m_pageNumber + ". Ascender: " + m_textInfo.pageInfo[m_pageNumber].ascender + "  BaseLine: " + m_textInfo.pageInfo[m_pageNumber].baseLine + "  Descender: " + m_textInfo.pageInfo[m_pageNumber].descender);                          

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
                            m_pageNumber += 1;
                            m_lineNumber += 1;
                            continue;
                    }
                    #endregion End Text Overflow

                }
                #endregion Check Vertical Bounds


                // Handle xAdvance & Tabulation Stops. Tab stops at every 25% of Font Size.
                #region XAdvance, Tabulation & Stops
                if (charCode == 9)
                {
                    //m_xAdvance = (int)(m_xAdvance / (m_fontSize * 3.34f) + 1) * (m_fontSize * 3.34f);
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
                    m_maxXAdvance = Mathf.Max(m_maxXAdvance, m_renderedWidth + m_xAdvance + (m_alignmentPadding.z * m_fontScale));
                    m_renderedWidth = 0;
                    m_xAdvance = 0 + m_indent;
                }
                #endregion Carriage Return


                // Handle Line Spacing Adjustments + Word Wrapping & special case for last line.
                #region Check for Line Feed and Last Character
                if (charCode == 10 || m_characterCount == totalCharacterCount - 1)
                {
                    //Debug.Log("Line # " + m_lineNumber + "  Current Character is [" + (char)charCode + "] with ASC value of " + charCode);

                    // Handle Line Spacing Changes
                    if (m_lineNumber > 0 && m_maxFontScale != 0 && m_lineHeight == 0 && m_maxFontScale != previousFontScale && !m_isNewPage)
                    {
                        //Debug.Log("Adjusting Line Spacing");
                        float gap = m_fontAssetArray[m_fontIndex].fontInfo.LineHeight - (m_fontAssetArray[m_fontIndex].fontInfo.Ascender - m_fontAssetArray[m_fontIndex].fontInfo.Descender);
                        float offsetDelta = (m_fontAssetArray[m_fontIndex].fontInfo.Ascender + m_lineSpacing + m_paragraphSpacing + gap + m_lineSpacingDelta) * m_maxFontScale - (m_fontAssetArray[m_fontIndex].fontInfo.Descender - gap) * previousFontScale;
                        m_lineOffset += offsetDelta - lineOffsetDelta;
                        AdjustLineOffset(m_firstVisibleCharacterOfLine, m_characterCount, offsetDelta - lineOffsetDelta);
                    }
                    m_isNewPage = false;

                    // Calculate lineAscender & make sure if last character is superscript or subscript that we check that as well.
                    float lineAscender = (m_fontAsset.fontInfo.Ascender + m_alignmentPadding.y) * m_maxFontScale - m_lineOffset;
                    float lineAscender2 = (m_fontAsset.fontInfo.Ascender + m_alignmentPadding.y) * m_fontScale - m_lineOffset + m_baselineOffset;
                    lineAscender = lineAscender > lineAscender2 ? lineAscender : lineAscender2;
                    //Debug.Log("Line # " + m_lineNumber + " -- Current Page is " + m_pageNumber + ". Previous Page is " + previousPageNumber);

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
                    m_textInfo.lineInfo[m_lineNumber].lineLength = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - (padding * m_maxFontScale);
                    m_textInfo.lineInfo[m_lineNumber].maxAdvance = m_textInfo.characterInfo[m_lastVisibleCharacterOfLine].xAdvance - m_characterSpacing * m_fontScale;

                    m_firstVisibleCharacterOfLine = m_characterCount + 1;


                    // Store PreferredWidth paying attention to linefeed and last character of text.
                    if (charCode == 10 && m_characterCount != totalCharacterCount - 1)
                    {
                        m_maxXAdvance = Mathf.Max(m_maxXAdvance, m_renderedWidth + m_xAdvance + (m_alignmentPadding.z * m_fontScale));
                        m_renderedWidth = 0;
                    }
                    else
                        m_renderedWidth = Mathf.Max(m_maxXAdvance, m_renderedWidth + m_xAdvance + (m_alignmentPadding.z * m_fontScale));

                    //Debug.Log("Line # " + m_lineNumber + " XAdance is " +  (m_preferredWidth + m_xAdvance + (m_alignmentPadding.z * m_fontScale)) + "  Max XAdvance: " + m_maxXAdvance);
               
                    m_renderedHeight = m_maxAscender - m_maxDescender;
                   
                    //Debug.Log("LineInfo for line # " + (m_lineNumber) + " First character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex +
                    //                                                    " Last character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex +
                    //                                                    " Last Visible character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastVisibleCharacterIndex +
                    //                                                    " Character Count of " + m_textInfo.lineInfo[m_lineNumber].characterCount /* + " Line Lenght of " + m_textInfo.lineInfo[m_lineNumber].lineLength +
                    //                                                    "  MinX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.x + "  MinY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.y +
                    //                                                    "  MaxX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x + "  MaxY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.y +
                    //                                                    "  Line Ascender: " + lineAscender + "  Line Descender: " + lineDescender */ );


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
                            lineOffsetDelta = (m_fontAssetArray[m_fontIndex].fontInfo.LineHeight + m_paragraphSpacing + m_lineSpacing + m_lineSpacingDelta) * m_fontScale;
                            m_lineOffset += lineOffsetDelta;
                        }
                        else
                            m_lineOffset += (m_lineHeight + m_lineSpacing + m_paragraphSpacing) * baseScale;

                        previousFontScale = m_fontScale;
                        m_maxFontScale = 0;
                        spriteScale = 1;
                        m_xAdvance = 0 + m_indent;

                        ellipsisIndex = m_characterCount - 1;
                    }
                }
                #endregion Check for Linefeed or Last Character


                // Store Rectangle positions for each Character and Mesh Extents.
                #region Save CharacterInfo for the current character.
                m_textInfo.characterInfo[m_characterCount].baseLine = m_textInfo.characterInfo[m_characterCount].topRight.y - (m_cached_GlyphInfo.yOffset + padding) * m_fontScale;
                m_textInfo.characterInfo[m_characterCount].topLine = m_textInfo.characterInfo[m_characterCount].baseLine + (m_fontAssetArray[m_fontIndex].fontInfo.Ascender + m_alignmentPadding.y) * m_fontScale; // Ascender              
                m_textInfo.characterInfo[m_characterCount].bottomLine = m_textInfo.characterInfo[m_characterCount].baseLine + (m_fontAssetArray[m_fontIndex].fontInfo.Descender - m_alignmentPadding.w) * m_fontScale; // Descender          
                m_textInfo.characterInfo[m_characterCount].padding = padding * m_fontScale;
                m_textInfo.characterInfo[m_characterCount].aspectRatio = m_cached_GlyphInfo.width / m_cached_GlyphInfo.height;
                m_textInfo.characterInfo[m_characterCount].scale = m_fontScale;
                m_textInfo.characterInfo[m_characterCount].meshIndex = m_fontIndex;

                
                // Determine the bounds of the Mesh.                       
                if (m_textInfo.characterInfo[m_characterCount].isVisible)
                {
                    m_meshExtents.min = new Vector2(Mathf.Min(m_meshExtents.min.x, m_textInfo.characterInfo[m_characterCount].vertex_BL.position.x), Mathf.Min(m_meshExtents.min.y, m_textInfo.characterInfo[m_characterCount].vertex_BL.position.y));
                    m_meshExtents.max = new Vector2(Mathf.Max(m_meshExtents.max.x, m_textInfo.characterInfo[m_characterCount].vertex_TR.position.x), Mathf.Max(m_meshExtents.max.y, m_textInfo.characterInfo[m_characterCount].vertex_TL.position.y));
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


                // Save State of Mesh Creation for handling of Word Wrapping
                #region Save Word Wrapping State
                if (m_enableWordWrapping || m_overflowMode == TextOverflowModes.Truncate || m_overflowMode == TextOverflowModes.Ellipsis)
                {            
                    if (charCode == 9 || charCode == 32) // || char.IsPunctuation((char)charCode))
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
            if (!m_isCharacterWrappingEnabled && m_enableAutoSizing && fontSizeDelta > 0.051f && m_fontSize < m_fontSizeMax)
            {
                m_minFontSize = m_fontSize;
                m_fontSize += Mathf.Max((m_maxFontSize - m_fontSize) / 2, 0.05f);
                m_fontSize = (int)(Mathf.Min(m_fontSize, m_fontSizeMax) * 20 + 0.5f) / 20f;

                if (loopCountA > 20) return; // Added to debug    
                GenerateTextMesh();
                return;
            }
            #endregion End Auto-sizing Check


            m_isCharacterWrappingEnabled = false;

            // Adjust Preferred Height to account for Margins.
            m_renderedHeight += m_margin.y > 0 ? m_margin.y : 0;
            
            if (m_renderMode == TextRenderFlags.GetPreferredSizes)
                return;

            if (!IsRectTransformDriven) { m_preferredWidth = m_renderedWidth; m_preferredHeight = m_renderedHeight; }

            // DEBUG & PERFORMANCE CHECKS (0.006ms)                      
            //Debug.Log("Iteration Count: " + loopCountA + ". Final Point Size: " + m_fontSize); // + "  B: " + loopCountB + "  C: " + loopCountC + "  D: " + loopCountD);

            // If there are no visible characters... no need to continue
            if (m_visibleCharacterCount == 0 && m_visibleSpriteCount == 0)
            {
                if (m_uiVertices != null)
                {
                    m_uiRenderer.SetVertices(m_uiVertices, 0);
                }
                return;
            }


            int last_vert_index = m_visibleCharacterCount * 4;
            // Partial clear of the vertices array to mark unused vertices as degenerate.
            Array.Clear(m_uiVertices, last_vert_index, m_uiVertices.Length - last_vert_index);
            // Do we want to clear the sprite array?

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
                        m_anchorOffset = m_rectCorners[1] + new Vector3(0 + margins.x, 0 - m_maxAscender - margins.y, 0);
                    else
                    {
                        m_anchorOffset = m_rectCorners[1] + new Vector3(0 + margins.x, 0 - m_textInfo.pageInfo[m_pageToDisplay].ascender - margins.y, 0);
                        //Debug.Log("Page # " + m_pageToDisplay + " Ascender is " + m_textInfo.pageInfo[m_pageToDisplay].ascender);
                    }
                    break;

                // Middle Vertically
                case TextAlignmentOptions.Left:
                case TextAlignmentOptions.Right:
                case TextAlignmentOptions.Center:
                case TextAlignmentOptions.Justified:
                    if (m_overflowMode != TextOverflowModes.Page)
                        m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2 + new Vector3(0 + margins.x, 0 - (m_maxAscender + margins.y + m_maxDescender - margins.w) / 2, 0);
                    else
                        m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2 + new Vector3(0 + margins.x, 0 - (m_textInfo.pageInfo[m_pageToDisplay].ascender + margins.y + m_textInfo.pageInfo[m_pageToDisplay].descender - margins.w) / 2, 0);
                    break;

                // Bottom Vertically
                case TextAlignmentOptions.Bottom:
                case TextAlignmentOptions.BottomLeft:
                case TextAlignmentOptions.BottomRight:
                case TextAlignmentOptions.BottomJustified:
                    if (m_overflowMode != TextOverflowModes.Page)
                        m_anchorOffset = m_rectCorners[0] + new Vector3(0 + margins.x, 0 - m_maxDescender + margins.w, 0);
                    else
                        m_anchorOffset = m_rectCorners[0] + new Vector3(0 + margins.x, 0 - m_textInfo.pageInfo[m_pageToDisplay].descender + margins.w, 0);
                    break;

                // Baseline Vertically 
                case TextAlignmentOptions.BaselineLeft:
                case TextAlignmentOptions.BaselineRight:
                case TextAlignmentOptions.Baseline:
                    m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2 + new Vector3(0 + margins.x, 0, 0);
                    break;

                // Midline Vertically 
                case TextAlignmentOptions.MidlineLeft:
                case TextAlignmentOptions.Midline:
                case TextAlignmentOptions.MidlineRight:
                case TextAlignmentOptions.MidlineJustified:
                    m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2 + new Vector3(0 + margins.x, 0 - (m_meshExtents.max.y + margins.y + m_meshExtents.min.y - margins.w) / 2, 0);
                    break;
            }
            #endregion Text Alignment


            // Handling of Anchor Dampening. If mesh width changes by more than 1/3 of the underline character's wdith then adjust it.           
            //float currentMeshWidth = m_meshExtents.max.x - m_meshExtents.min.x;
            //if (m_anchorDampening)
            //{
            //    float delta = currentMeshWidth - m_baseDampeningWidth;
            //    if (m_baseDampeningWidth != 0 && Mathf.Abs(delta) < m_cached_Underline_GlyphInfo.width * m_fontScale * 0.6f)
            //        m_anchorOffset.x += delta / 2;
            //    else
            //        m_baseDampeningWidth = currentMeshWidth;
            //}


            // Initialization for Second Pass
            Vector3 justificationOffset = Vector3.zero;
            Vector3 offset = Vector3.zero;
            int vert_index_X4 = 0;
            int sprite_index_X4 = 0;
            Array.Clear(m_meshAllocCount, 0, 17);
            int underlineSegmentCount = 0;
            Color32 underlineColor = new Color32(255, 255, 255, 127);

            int wordCount = 0;
            int lineCount = 0;
            int lastLine = 0;

            bool isStartOfWord = false;
            int wordFirstChar = 0;
            int wordLastChar = 0;


            // Second Pass : Line Justification, UV Mapping, Character & Line Visibility & more.
            #region Handle Line Justification & UV Mapping & Character Visibility & More
            
            // Variables used to handle Canvas Render Modes and SDF Scaling
            float lossyScale = m_rectTransform.lossyScale.z;
            RenderMode canvasRenderMode = m_canvas.renderMode;
            float canvasScaleFactor = m_canvas.scaleFactor;       
            bool isCameraAssigned = m_canvas.worldCamera == null ? false : true;

            for (int i = 0; i < m_characterCount; i++)
            {
                TMP_CharacterInfo[] characterInfoArray = m_textInfo.characterInfo;
                int currentLine = characterInfoArray[i].lineNumber;
                char currentCharacter = characterInfoArray[i].character;
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
                        justificationOffset = new Vector3(marginWidth / 2 - lineInfo.maxAdvance / 2, 0, 0);
                        break;

                    case TextAlignmentOptions.TopRight:
                    case TextAlignmentOptions.Right:
                    case TextAlignmentOptions.BottomRight:
                    case TextAlignmentOptions.BaselineRight:
                    case TextAlignmentOptions.MidlineRight:
                        justificationOffset = new Vector3(marginWidth - lineInfo.maxAdvance, 0, 0);
                        break;

                    case TextAlignmentOptions.TopJustified:
                    case TextAlignmentOptions.Justified:
                    case TextAlignmentOptions.BottomJustified:
                    case TextAlignmentOptions.BaselineJustified:
                    case TextAlignmentOptions.MidlineJustified:
                        charCode = m_textInfo.characterInfo[i].character;
                        char lastCharOfCurrentLine = m_textInfo.characterInfo[lineInfo.lastCharacterIndex].character;

                        if (char.IsWhiteSpace(lastCharOfCurrentLine) && !char.IsControl(lastCharOfCurrentLine) && currentLine < m_lineNumber)
                        {   // All lines are justified accept the last one.
                            float gap = (m_rectCorners[3].x - margins.z) - (m_rectCorners[0].x + margins.x) - (lineInfo.maxAdvance);
                            if (currentLine != lastLine || i == 0)
                                justificationOffset = Vector3.zero;
                            else
                            {
                                if (charCode == 9 || charCode == 32)
                                {
                                    justificationOffset += new Vector3(gap * (1 - m_wordWrappingRatios) / (lineInfo.spaceCount - 1), 0, 0);
                                }
                                else
                                {
                                    //Debug.Log("LineInfo Character Count: " + lineInfo.characterCount);
                                    justificationOffset += new Vector3(gap * m_wordWrappingRatios / (lineInfo.characterCount - lineInfo.spaceCount - 1), 0, 0);
                                }
                            }
                        }
                        else
                            justificationOffset = Vector3.zero; // Keep last line left justified.

                        //Debug.Log("Char [" + (char)charCode + "] Code:" + charCode + "  Offset:" + justificationOffset + "  # Spaces:" + m_lineExtents[currentLine].NumberOfSpaces + "  # Characters:" + m_lineExtents[currentLine].NumberOfChars);                       
                        break;
                }
                #endregion End Text Justification

                offset = m_anchorOffset + justificationOffset;

                if (characterInfoArray[i].isVisible)
                {
                    TMP_CharacterType type = characterInfoArray[i].type;
                    switch (type)
                    {
                        // CHARACTERS
                        case TMP_CharacterType.Character:
                                                      
                            Extents lineExtents = lineInfo.lineExtents;
                            float uvOffset = (m_uvLineOffset * currentLine) % 1 + m_uvOffset.x;

                            // Setup UV2 based on Character Mapping Options Selected
                            #region Handle UV Mapping Options                            
                            switch (m_horizontalMapping)
                            {                               
                                case TextureMappingOptions.Character:
                                    characterInfoArray[i].vertex_BL.uv2.x = 0 + m_uvOffset.x;
                                    characterInfoArray[i].vertex_TL.uv2.x = 0 + m_uvOffset.x;
                                    characterInfoArray[i].vertex_TR.uv2.x = 1 + m_uvOffset.x;
                                    characterInfoArray[i].vertex_BR.uv2.x = 1 + m_uvOffset.x;
                                    break;

                                case TextureMappingOptions.Line:
                                    if (m_textAlignment != TextAlignmentOptions.Justified)
                                    {
                                        characterInfoArray[i].vertex_BL.uv2.x = (characterInfoArray[i].vertex_BL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                        characterInfoArray[i].vertex_TL.uv2.x = (characterInfoArray[i].vertex_TL.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                        characterInfoArray[i].vertex_TR.uv2.x = (characterInfoArray[i].vertex_TR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                        characterInfoArray[i].vertex_BR.uv2.x = (characterInfoArray[i].vertex_BR.position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                        break;
                                    }
                                    else // Special Case if Justified is used in Line Mode.
                                    {
                                        characterInfoArray[i].vertex_BL.uv2.x = (characterInfoArray[i].vertex_BL.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                        characterInfoArray[i].vertex_TL.uv2.x = (characterInfoArray[i].vertex_TL.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                        characterInfoArray[i].vertex_TR.uv2.x = (characterInfoArray[i].vertex_TR.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                        characterInfoArray[i].vertex_BR.uv2.x = (characterInfoArray[i].vertex_BR.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                        break;
                                    }

                                case TextureMappingOptions.Paragraph:
                                    characterInfoArray[i].vertex_BL.uv2.x = (characterInfoArray[i].vertex_BL.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                    characterInfoArray[i].vertex_TL.uv2.x = (characterInfoArray[i].vertex_TL.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                    characterInfoArray[i].vertex_TR.uv2.x = (characterInfoArray[i].vertex_TR.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                    characterInfoArray[i].vertex_BR.uv2.x = (characterInfoArray[i].vertex_BR.position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;

                                    Vector3 P0 = characterInfoArray[i].vertex_BL.position + justificationOffset;
                                    Vector3 P1 = characterInfoArray[i].vertex_TL.position + justificationOffset;
                                    Vector3 P2 = characterInfoArray[i].vertex_TR.position + justificationOffset;
                                    Vector3 P3 = characterInfoArray[i].vertex_BR.position + justificationOffset;
                                    Debug.DrawLine(P0, P1, Color.green, 60f);
                                    Debug.DrawLine(P1, P2, Color.green, 60f);
                                    Debug.DrawLine(P2, P3, Color.green, 60f);
                                    Debug.DrawLine(P3, P0, Color.green, 60f);

                                    P0 = m_meshExtents.min + new Vector2(justificationOffset.x * 0, justificationOffset.y);
                                    P1 = new Vector3(m_meshExtents.min.x, m_meshExtents.max.y, 0) + new Vector3 (justificationOffset.x * 0, justificationOffset.y, 0);
                                    P2 = m_meshExtents.max + new Vector2(justificationOffset.x * 0, justificationOffset.y);
                                    P3 = new Vector3(m_meshExtents.max.x, m_meshExtents.min.y, 0) + new Vector3(justificationOffset.x * 0, justificationOffset.y, 0);
                                    Debug.DrawLine(P0, P1, Color.red, 60f);
                                    Debug.DrawLine(P1, P2, Color.red, 60f);
                                    Debug.DrawLine(P2, P3, Color.red, 60f);
                                    Debug.DrawLine(P3, P0, Color.red, 60f);

                                    break;

                                case TextureMappingOptions.MatchAspect:

                                    switch (m_verticalMapping)
                                    {
                                        case TextureMappingOptions.Character:
                                            characterInfoArray[i].vertex_BL.uv2.y = 0 + m_uvOffset.y;
                                            characterInfoArray[i].vertex_TL.uv2.y = 1 + m_uvOffset.y;
                                            characterInfoArray[i].vertex_TR.uv2.y = 0 + m_uvOffset.y;
                                            characterInfoArray[i].vertex_BR.uv2.y = 1 + m_uvOffset.y;
                                            break;

                                        case TextureMappingOptions.Line:
                                            characterInfoArray[i].vertex_BL.uv2.y = (characterInfoArray[i].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + uvOffset;
                                            characterInfoArray[i].vertex_TL.uv2.y = (characterInfoArray[i].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + uvOffset;
                                            characterInfoArray[i].vertex_TR.uv2.y = characterInfoArray[i].vertex_BL.uv2.y;
                                            characterInfoArray[i].vertex_BR.uv2.y = characterInfoArray[i].vertex_TL.uv2.y;
                                            break;

                                        case TextureMappingOptions.Paragraph:
                                            characterInfoArray[i].vertex_BL.uv2.y = (characterInfoArray[i].vertex_BL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + uvOffset;
                                            characterInfoArray[i].vertex_TL.uv2.y = (characterInfoArray[i].vertex_TL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + uvOffset;
                                            characterInfoArray[i].vertex_TR.uv2.y = characterInfoArray[i].vertex_BL.uv2.y;
                                            characterInfoArray[i].vertex_BR.uv2.y = characterInfoArray[i].vertex_TL.uv2.y;
                                            break;

                                        case TextureMappingOptions.MatchAspect:
                                            Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
                                            break;
                                    }

                                    //float xDelta = 1 - (_uv2s[vert_index + 0].y * textMeshCharacterInfo[i].AspectRatio); // Left aligned
                                    float xDelta = (1 - ((characterInfoArray[i].vertex_BL.uv2.y +  characterInfoArray[i].vertex_TL.uv2.y) * characterInfoArray[i].aspectRatio)) / 2; // Center of Rectangle
                                    //float xDelta = 0;

                                    characterInfoArray[i].vertex_BL.uv2.x = (characterInfoArray[i].vertex_BL.uv2.y * characterInfoArray[i].aspectRatio) + xDelta + uvOffset;
                                    characterInfoArray[i].vertex_TL.uv2.x = characterInfoArray[i].vertex_BL.uv2.x;
                                    characterInfoArray[i].vertex_TR.uv2.x = (characterInfoArray[i].vertex_TL.uv2.y * characterInfoArray[i].aspectRatio) + xDelta + uvOffset;
                                    characterInfoArray[i].vertex_BR.uv2.x = characterInfoArray[i].vertex_TR.uv2.x;
                                    break;
                            }

                            switch (m_verticalMapping)
                            {
                                case TextureMappingOptions.Character:
                                    characterInfoArray[i].vertex_BL.uv2.y = 0 + m_uvOffset.y;
                                    characterInfoArray[i].vertex_TL.uv2.y = 1 + m_uvOffset.y;
                                    characterInfoArray[i].vertex_TR.uv2.y = 1 + m_uvOffset.y;
                                    characterInfoArray[i].vertex_BR.uv2.y = 0 + m_uvOffset.y;
                                    break;

                                case TextureMappingOptions.Line:
                                    characterInfoArray[i].vertex_BL.uv2.y = (characterInfoArray[i].vertex_BL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + m_uvOffset.y;
                                    characterInfoArray[i].vertex_TL.uv2.y = (characterInfoArray[i].vertex_TL.position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + m_uvOffset.y;
                                    characterInfoArray[i].vertex_BR.uv2.y = characterInfoArray[i].vertex_BL.uv2.y;
                                    characterInfoArray[i].vertex_TR.uv2.y = characterInfoArray[i].vertex_TL.uv2.y;                               
                                    break;

                                case TextureMappingOptions.Paragraph:
                                    characterInfoArray[i].vertex_BL.uv2.y = (characterInfoArray[i].vertex_BL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
                                    characterInfoArray[i].vertex_TL.uv2.y = (characterInfoArray[i].vertex_TL.position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
                                    characterInfoArray[i].vertex_BR.uv2.y = characterInfoArray[i].vertex_BL.uv2.y;
                                    characterInfoArray[i].vertex_TR.uv2.y = characterInfoArray[i].vertex_TL.uv2.y;
                                    break;

                                case TextureMappingOptions.MatchAspect:
                                    //float yDelta = 1 - (_uv2s[vert_index + 2].x / textMeshCharacterInfo[i].AspectRatio); // Top Corner                       
                                    float yDelta = (1 - ((characterInfoArray[i].vertex_BL.uv2.x + characterInfoArray[i].vertex_TR.uv2.x) / characterInfoArray[i].aspectRatio)) / 2; // Center of Rectangle
                                    //float yDelta = 0;

                                    characterInfoArray[i].vertex_BL.uv2.y = yDelta + (characterInfoArray[i].vertex_BL.uv2.x / characterInfoArray[i].aspectRatio) + m_uvOffset.y;
                                    characterInfoArray[i].vertex_TL.uv2.y = yDelta + (characterInfoArray[i].vertex_TR.uv2.x / characterInfoArray[i].aspectRatio) + m_uvOffset.y;
                                    characterInfoArray[i].vertex_BR.uv2.y = characterInfoArray[i].vertex_BL.uv2.y;
                                    characterInfoArray[i].vertex_TR.uv2.y = characterInfoArray[i].vertex_TL.uv2.y;
                                    break;
                            }
                            #endregion End UV Mapping Options


                            // Pack UV's so that we can pass Xscale needed for Shader to maintain 1:1 ratio.                                                                                                
                            #region Pack Scale into UV2
                            float xScale = characterInfoArray[i].scale;
                            if ((characterInfoArray[i].style & FontStyles.Bold) == FontStyles.Bold) xScale *= -1;
                        
                            switch (canvasRenderMode)
                            {
                                case RenderMode.ScreenSpaceOverlay:
                                    xScale *= lossyScale / canvasScaleFactor;
                                    break;
                                case RenderMode.ScreenSpaceCamera:
                                    xScale *= isCameraAssigned ? lossyScale : 1;
                                    break;
                                case RenderMode.WorldSpace:
                                    xScale *= lossyScale;
                                    break;
                            }
                        
                            //Debug.Log("Camera Assigned = " + (m_canvas.worldCamera != null) + ".  Character Scale: " + characterInfoArray[i].scale + ". LossyScale: " + m_rectTransform.lossyScale.z + ".  xScale: " + xScale + ". Canvas Scale: " + m_canvas.scaleFactor);

                            float x0 = characterInfoArray[i].vertex_BL.uv2.x;
                            float y0 = characterInfoArray[i].vertex_BL.uv2.y;
                            float x1 = characterInfoArray[i].vertex_TR.uv2.x;
                            float y1 = characterInfoArray[i].vertex_TR.uv2.y; 

                            float dx = Mathf.Floor(x0);
                            float dy = Mathf.Floor(y0);

                            x0 = x0 - dx;
                            x1 = x1 - dx;
                            y0 = y0 - dy;
                            y1 = y1 - dy;

                            characterInfoArray[i].vertex_BL.uv2 = PackUV(x0, y0, xScale);
                            characterInfoArray[i].vertex_TL.uv2 = PackUV(x0, y1, xScale);
                            characterInfoArray[i].vertex_TR.uv2 = PackUV(x1, y1, xScale);
                            characterInfoArray[i].vertex_BR.uv2 = PackUV(x1, y0, xScale);
                            #endregion                        
                            
                            break;
                        
                        // SPRITES
                        case TMP_CharacterType.Sprite:
                            // Nothing right now                     
                            break;
                    }


                    // Handle maxVisibleCharacters, maxVisibleLines and Overflow Page Mode.
                    #region Handle maxVisibleCharacters / maxVisibleLines / Page Mode

                    if (m_maxVisibleCharacters != -1 && i >= m_maxVisibleCharacters
                        || m_maxVisibleLines != -1 && currentLine >= m_maxVisibleLines
                        || m_overflowMode == TextOverflowModes.Page && characterInfoArray[i].pageNumber != m_pageToDisplay)
                    {
                        characterInfoArray[i].vertex_BL.position *= 0;
                        characterInfoArray[i].vertex_TL.position *= 0;
                        characterInfoArray[i].vertex_TR.position *= 0;
                        characterInfoArray[i].vertex_BR.position *= 0;
                    }
                    else
                    {
                        /* Vector3 p0 = */ characterInfoArray[i].vertex_BL.position += offset;
                        /* Vector3 p1 = */ characterInfoArray[i].vertex_TL.position += offset;
                        /* Vector3 p2 = */ characterInfoArray[i].vertex_TR.position += offset;
                        /* Vector3 p3 = */ characterInfoArray[i].vertex_BR.position += offset;

                        //Debug.DrawLine(p0, p1, Color.green, 60f);
                        //Debug.DrawLine(p1, p2, Color.green, 60f);
                        //Debug.DrawLine(p2, p3, Color.green, 60f);
                        //Debug.DrawLine(p3, p0, Color.green, 60f);
                    }
                    #endregion

                  
                    // Fill Vertex Buffers for the various types of element
                    if (type == TMP_CharacterType.Character)
                    {
                        FillCharacterVertexBuffers(i, vert_index_X4);
                        vert_index_X4 += 4;
                    }
                    else if (type == TMP_CharacterType.Sprite)
                    {
                        FillSpriteVertexBuffers(i, sprite_index_X4);
                        sprite_index_X4 += 4;
                    }           
                    #region
                }
                #endregion

                // Store modified characterInfo fields.
                //m_textInfo.characterInfo[i] = characterInfoArray[i];
                
                // Apply Alignment and Justification Offset
                m_textInfo.characterInfo[i].bottomLeft += offset;
                m_textInfo.characterInfo[i].topRight += offset;
                // Need to add top left and bottom right.
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


                // Track Word Count per line and for the object
                #region Track Word Count
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
                #endregion


                // Handle Underline
                #region Underline Tracking
                // TODO : Address underline and which font to use in the list
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
                        underlineSegmentCount += 1;
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
                        underlineSegmentCount += 1;
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
                        underlineSegmentCount += 1;
                    }
                }
                #endregion

                lastLine = currentLine;
            }
            #endregion

            // METRICS ABOUT THE TEXT OBJECT            
            m_textInfo.characterCount = (short)m_characterCount;
            m_textInfo.spriteCount = m_spriteCount;
            m_textInfo.lineCount = (short)lineCount;
            m_textInfo.wordCount = wordCount != 0 && m_characterCount > 0 ? (short)wordCount : (short)1;
            m_textInfo.pageCount = m_pageNumber;

            // Need to store UI Vertex
            m_textInfo.meshInfo.uiVertices = m_uiVertices;

            //if (m_textInfo.meshInfo.meshArrays == null)
            //    m_textInfo.meshInfo.meshArrays = new UIVertex[16][];            
            
            //m_textInfo.meshInfo.meshArrays[0] = m_uiVertices;
            //if (m_spriteCount > 0 && m_inlineGraphics != null) m_textInfo.meshInfo.meshArrays[17] = m_inlineGraphics.uiVertex;
            


            // If Advanced Layout Component is present, don't upload the mesh.
            if (m_renderMode == TextRenderFlags.Render) // m_isAdvanceLayoutComponentPresent == false || m_advancedLayoutComponent.isEnabled == false)
            {
                //Debug.Log("Uploading Mesh normally.");
                // Upload Mesh Data 
                m_uiRenderer.SetVertices(m_uiVertices, vert_index_X4 + underlineSegmentCount * 12);
                //m_uiRenderer.SetVertices(m_textInfo.meshInfo.meshArrays[0], m_meshAllocCount[0]);

                //for (int i = 0; i < subObjects.Length; i++ )
                //{
                //    if (subObjects[i] != null)
                //    {
                //        subObjects[i].GetComponent<CanvasRenderer>().SetVertices(m_textInfo.meshInfo.meshArrays[i], m_meshAllocCount[i]);
                //        Material mat = Resources.Load("Fonts & Materials/ARIAL SDF - Drop Shadow", typeof(Material)) as Material;
                //        subObjects[i].GetComponent<CanvasRenderer>().SetMaterial(mat, null);
                //    }
                //}


                if (m_spriteCount > 0 && m_inlineGraphics != null)
                    m_inlineGraphics.DrawSprite(m_inlineGraphics.uiVertex, m_spriteCount);


                //m_maskOffset = new Vector4(m_mesh.bounds.center.x, m_mesh.bounds.center.y, m_mesh.bounds.size.x, m_mesh.bounds.size.y);
            }

            // Compute Bounds for the mesh. Manual computation is more efficient then using Mesh.recalcualteBounds.
            m_bounds = new Bounds(new Vector3((m_meshExtents.max.x + m_meshExtents.min.x) / 2, (m_meshExtents.max.y + m_meshExtents.min.y) / 2, 0) + offset, new Vector3(m_meshExtents.max.x - m_meshExtents.min.x, m_meshExtents.max.y - m_meshExtents.min.y, 0));

            //Vector3 P0 = m_meshExtents.min + new Vector2(offset.x, offset.y);
            //Vector3 P1 = new Vector3(m_meshExtents.min.x, m_meshExtents.max.y, 0) + offset;
            //Vector3 P2 = m_meshExtents.max + new Vector2(offset.x, offset.y);
            //Vector3 P3 = new Vector3(m_meshExtents.max.x, m_meshExtents.min.y, 0) + offset;
            //Debug.DrawLine(P0, P1, Color.green, 60f);
            //Debug.DrawLine(P1, P2, Color.green, 60f);
            //Debug.DrawLine(P2, P3, Color.green, 60f);
            //Debug.DrawLine(P3, P0, Color.green, 60f);

            // Has Text Container's Width or Height been specified by the user?
            /*
            if (m_rectTransform.sizeDelta.x == 0 || m_rectTransform.sizeDelta.y == 0)
            {
                //Debug.Log("Auto-fitting Text. Default Width:" + m_textContainer.isDefaultWidth + "  Default Height:" + m_textContainer.isDefaultHeight);
                if (marginWidth == 0)
                    m_rectTransform.sizeDelta = new Vector2(m_preferredWidth + margins.x + margins.z, m_rectTransform.sizeDelta.y);

                if (marginHeight == 0)
                    m_rectTransform.sizeDelta = new Vector2(m_rectTransform.sizeDelta.x,  m_preferredHeight + margins.y + margins.w);

                
                Debug.Log("Auto-fitting Text. Default Width:" + m_preferredWidth + "  Default Height:" + m_preferredHeight);
                GenerateTextMesh();
                return;
            }
            */

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

            //Debug.Log("Done rendering text. Character Count is " + m_textInfo.characterCount);
            //Debug.Log("Done rendering text. Preferred Width:" + m_preferredWidth + "  Preferred Height:" + m_preferredHeight);

            //Debug.Log(m_minWidth);
            //Profiler.EndSample();
            //m_StopWatch.Stop();           
            //Debug.Log("Done Rendering Text.");

//#if UNITY_EDITOR
//            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
//#endif

            //Debug.Log("TimeElapsed is:" + (m_StopWatch.ElapsedTicks / 10000f).ToString("f4"));
            //m_StopWatch.Reset();     
        }


        // Store Vertex Information for each Character
        void SaveGlyphVertexInfo(float style_padding, Color32 vertexColor)
        {                              
            // Save the Vertex Position for the Character
            #region Setup Mesh Vertices
            /* Vector3 p0 = */ m_textInfo.characterInfo[m_characterCount].vertex_BL.position = m_textInfo.characterInfo[m_characterCount].bottomLeft;
            /* Vector3 p1 = */ m_textInfo.characterInfo[m_characterCount].vertex_TL.position = m_textInfo.characterInfo[m_characterCount].topLeft;
            /* Vector3 p2 = */ m_textInfo.characterInfo[m_characterCount].vertex_TR.position = m_textInfo.characterInfo[m_characterCount].topRight;
            /* Vector3 p3 = */ m_textInfo.characterInfo[m_characterCount].vertex_BR.position = m_textInfo.characterInfo[m_characterCount].bottomRight;

            //Debug.DrawLine(p0, p1, Color.green, 60f);
            //Debug.DrawLine(p1, p2, Color.green, 60f);
            //Debug.DrawLine(p2, p3, Color.green, 60f);
            //Debug.DrawLine(p3, p0, Color.green, 60f);
            #endregion

                       
            // Alpha is the lower of the vertex color or tag color alpha used.
            vertexColor.a = m_fontColor32.a < vertexColor.a ? (byte)(m_fontColor32.a) : (byte)(vertexColor.a);
            
            // Handle Vertex Colors & Vertex Color Gradient               
            if (!m_enableVertexGradient)
            {
                m_textInfo.characterInfo[m_characterCount].vertex_BL.color = vertexColor;
                m_textInfo.characterInfo[m_characterCount].vertex_TL.color = vertexColor;
                m_textInfo.characterInfo[m_characterCount].vertex_TR.color = vertexColor;
                m_textInfo.characterInfo[m_characterCount].vertex_BR.color = vertexColor;
            }
            else
            {
                if (!m_overrideHtmlColors && !m_htmlColor.CompareRGB(m_fontColor32))
                {
                    m_textInfo.characterInfo[m_characterCount].vertex_BL.color = vertexColor;
                    m_textInfo.characterInfo[m_characterCount].vertex_TL.color = vertexColor;
                    m_textInfo.characterInfo[m_characterCount].vertex_TR.color = vertexColor;
                    m_textInfo.characterInfo[m_characterCount].vertex_BR.color = vertexColor;
                }
                else
                {
                    m_textInfo.characterInfo[m_characterCount].vertex_BL.color = m_fontColorGradient.bottomLeft;
                    m_textInfo.characterInfo[m_characterCount].vertex_TL.color = m_fontColorGradient.topLeft;
                    m_textInfo.characterInfo[m_characterCount].vertex_TR.color = m_fontColorGradient.topRight;
                    m_textInfo.characterInfo[m_characterCount].vertex_BR.color = m_fontColorGradient.bottomRight;
                }

                m_textInfo.characterInfo[m_characterCount].vertex_BL.color.a = vertexColor.a;
                m_textInfo.characterInfo[m_characterCount].vertex_TL.color.a = vertexColor.a;
                m_textInfo.characterInfo[m_characterCount].vertex_TR.color.a = vertexColor.a;
                m_textInfo.characterInfo[m_characterCount].vertex_BR.color.a = vertexColor.a;
            }
          

            // Apply style_padding only if this is a SDF Shader.
            if (!m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal))
                style_padding = 0;


            // Setup UVs for the Character
            #region Setup UVs
            Vector2 uv0 = new Vector2((m_cached_GlyphInfo.x - m_padding - style_padding) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasWidth, 1 - (m_cached_GlyphInfo.y + m_padding + style_padding + m_cached_GlyphInfo.height) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasHeight);  // bottom left
            Vector2 uv1 = new Vector2(uv0.x, 1 - (m_cached_GlyphInfo.y - m_padding - style_padding) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasHeight);  // top left
            Vector2 uv2 = new Vector2((m_cached_GlyphInfo.x + m_padding + style_padding + m_cached_GlyphInfo.width) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasWidth, uv0.y); // bottom right
            Vector2 uv3 = new Vector2(uv2.x, uv1.y); // top right

            // Store UV Information
            m_textInfo.characterInfo[m_characterCount].vertex_BL.uv = uv0;
            m_textInfo.characterInfo[m_characterCount].vertex_TL.uv = uv1;
            m_textInfo.characterInfo[m_characterCount].vertex_TR.uv = uv3;
            m_textInfo.characterInfo[m_characterCount].vertex_BR.uv = uv2;   
            #endregion Setup UVs


            // Normal
            #region Setup Normals & Tangents
            Vector3 normal = new Vector3(0, 0, -1);
            m_textInfo.characterInfo[m_characterCount].vertex_BL.normal = normal;
            m_textInfo.characterInfo[m_characterCount].vertex_TL.normal = normal;
            m_textInfo.characterInfo[m_characterCount].vertex_TR.normal = normal;
            m_textInfo.characterInfo[m_characterCount].vertex_BR.normal = normal;

            // Tangents
            Vector4 tangent = new Vector4(-1, 0, 0, 1);
            m_textInfo.characterInfo[m_characterCount].vertex_BL.tangent = tangent;
            m_textInfo.characterInfo[m_characterCount].vertex_TL.tangent = tangent;
            m_textInfo.characterInfo[m_characterCount].vertex_TR.tangent = tangent;
            m_textInfo.characterInfo[m_characterCount].vertex_BR.tangent = tangent;
            #endregion end Normals & Tangents
        }


        // Store Vertex Information for each Sprite
        void SaveSpriteVertexInfo(Color32 vertexColor)
        {
            int padding = m_enableExtraPadding ? 4 : 0;
            // Determine UV for the Sprite
            Vector2 uv0 = new Vector2((m_cached_GlyphInfo.x - padding) / m_inlineGraphics.spriteAsset.spriteSheet.width, (m_cached_GlyphInfo.y - padding) / m_inlineGraphics.spriteAsset.spriteSheet.height);  // bottom left
            Vector2 uv1 = new Vector2(uv0.x, (m_cached_GlyphInfo.y + padding + m_cached_GlyphInfo.height) / m_inlineGraphics.spriteAsset.spriteSheet.height);  // top left
            Vector2 uv2 = new Vector2((m_cached_GlyphInfo.x + padding + m_cached_GlyphInfo.width) / m_inlineGraphics.spriteAsset.spriteSheet.width, uv0.y); // bottom right
            Vector2 uv3 = new Vector2(uv2.x, uv1.y); // top right


            // Vertex Color Alpha 
            vertexColor.a = m_fontColor32.a < vertexColor.a ? m_fontColor32.a : vertexColor.a;

            TMP_Vertex vertex = new TMP_Vertex();
            // Bottom Left Vertex
            vertex.position = m_textInfo.characterInfo[m_characterCount].bottomLeft;
            vertex.uv = uv0;
            vertex.color = vertexColor;
            m_textInfo.characterInfo[m_characterCount].vertex_BL = vertex;
            
            // Top Left Vertex
            vertex.position = m_textInfo.characterInfo[m_characterCount].topLeft;
            vertex.uv = uv1;
            vertex.color = vertexColor;
            m_textInfo.characterInfo[m_characterCount].vertex_TL = vertex;
            
            // Top Right Vertex
            vertex.position = m_textInfo.characterInfo[m_characterCount].topRight;
            vertex.uv = uv3;
            vertex.color = vertexColor;
            m_textInfo.characterInfo[m_characterCount].vertex_TR = vertex;
            
            // Bottom Right Vertex
            vertex.position = m_textInfo.characterInfo[m_characterCount].bottomRight;
            vertex.uv = uv2;
            vertex.color = vertexColor;
            m_textInfo.characterInfo[m_characterCount].vertex_BR = vertex;                
        }


        // Fill Vertex Buffers for Characters
        void FillCharacterVertexBuffers(int i, int index_X4)
        {
            //int meshIndex = m_textInfo.characterInfo[index].meshIndex;
            //int index2 = m_meshAllocCount[meshIndex];
            TMP_CharacterInfo[] characterInfoArray = m_textInfo.characterInfo;
            m_textInfo.characterInfo[i].vertexIndex = (short)(index_X4);

            // Setup Vertices for Characters or Sprites
            UIVertex bottomLeft = new UIVertex();
            bottomLeft.position = characterInfoArray[i].vertex_BL.position;
            bottomLeft.uv0 = characterInfoArray[i].vertex_BL.uv;
            bottomLeft.uv1 = characterInfoArray[i].vertex_BL.uv2;
            bottomLeft.color = characterInfoArray[i].vertex_BL.color;
            bottomLeft.normal = characterInfoArray[i].vertex_BL.normal;
            bottomLeft.tangent = characterInfoArray[i].vertex_BL.tangent;
            m_uiVertices[0 + index_X4] = bottomLeft;
            //m_textInfo.meshInfo.meshArrays[meshIndex][index2 + 0] = bottomLeft;
            

            UIVertex topLeft = new UIVertex();
            topLeft.position = characterInfoArray[i].vertex_TL.position;
            topLeft.uv0 = characterInfoArray[i].vertex_TL.uv;
            topLeft.uv1 = characterInfoArray[i].vertex_TL.uv2;
            topLeft.color = characterInfoArray[i].vertex_TL.color;
            topLeft.normal = characterInfoArray[i].vertex_TL.normal;
            topLeft.tangent = characterInfoArray[i].vertex_TL.tangent;
            m_uiVertices[1 + index_X4] = topLeft;
            //m_textInfo.meshInfo.meshArrays[meshIndex][index2 + 1] = topLeft;
            

            UIVertex topRight = new UIVertex();
            topRight.position = characterInfoArray[i].vertex_TR.position;
            topRight.uv0 = characterInfoArray[i].vertex_TR.uv;
            topRight.uv1 = characterInfoArray[i].vertex_TR.uv2;
            topRight.color = characterInfoArray[i].vertex_TR.color;
            topRight.normal = characterInfoArray[i].vertex_TR.normal;
            topRight.tangent = characterInfoArray[i].vertex_TR.tangent;
            m_uiVertices[2 + index_X4] = topRight;
            //m_textInfo.meshInfo.meshArrays[meshIndex][index2 + 2] = topRight;
            

            UIVertex bottomRight = new UIVertex();
            bottomRight.position = characterInfoArray[i].vertex_BR.position;
            bottomRight.uv0 = characterInfoArray[i].vertex_BR.uv;
            bottomRight.uv1 = characterInfoArray[i].vertex_BR.uv2;
            bottomRight.color = characterInfoArray[i].vertex_BR.color;
            bottomRight.normal = characterInfoArray[i].vertex_BR.normal;
            bottomRight.tangent = characterInfoArray[i].vertex_BR.tangent;
            m_uiVertices[3 + index_X4] = bottomRight;
            //m_textInfo.meshInfo.meshArrays[meshIndex][index2 + 3] = bottomRight;

            //m_meshAllocCount[meshIndex] += 4;
        }


        // Fill Vertex Buffers for Sprites
        void FillSpriteVertexBuffers(int i, int spriteIndex_X4)
        {
            //m_textInfo.characterInfo[index].meshIndex = 1;
            m_textInfo.characterInfo[i].vertexIndex = (short)(spriteIndex_X4);
            TMP_CharacterInfo[] characterInfoArray = m_textInfo.characterInfo;

            //Debug.Log(m_visibleSpriteCount);          
            UIVertex[] spriteVertices = m_inlineGraphics.uiVertex;

            UIVertex uiVertex = new UIVertex();


            uiVertex.position = characterInfoArray[i].vertex_BL.position;
            uiVertex.uv0 = characterInfoArray[i].vertex_BL.uv;
            uiVertex.color = characterInfoArray[i].vertex_BL.color;
            spriteVertices[spriteIndex_X4 + 0] = uiVertex;

            uiVertex.position = characterInfoArray[i].vertex_TL.position;
            uiVertex.uv0 = characterInfoArray[i].vertex_TL.uv;
            uiVertex.color = characterInfoArray[i].vertex_TL.color;
            spriteVertices[spriteIndex_X4 + 1] = uiVertex;

            uiVertex.position = characterInfoArray[i].vertex_TR.position;
            uiVertex.uv0 = characterInfoArray[i].vertex_TR.uv;
            uiVertex.color = characterInfoArray[i].vertex_TR.color;
            spriteVertices[spriteIndex_X4 + 2] = uiVertex;

            uiVertex.position = characterInfoArray[i].vertex_BR.position;
            uiVertex.uv0 = characterInfoArray[i].vertex_BR.uv;
            uiVertex.color = characterInfoArray[i].vertex_BR.color;
            spriteVertices[spriteIndex_X4 + 3] = uiVertex;  

            m_inlineGraphics.SetUIVertex(spriteVertices);                  
        }


        // Draws the Underline
        void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index, Color32 underlineColor)
        {

            int verticesCount = index + 12;          
            // Check to make sure our current mesh buffer allocations can hold these new Quads.  
            if (verticesCount > m_uiVertices.Length)
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
            //Debug.Log("Char H:" + cached_Underline_GlyphInfo.height);

            float underlineThickness = m_cached_Underline_GlyphInfo.height; // m_fontAsset.FontInfo.UnderlineThickness;
            // Front Part of the Underline
            m_uiVertices[index + 0].position = start + new Vector3(0, 0 - (underlineThickness + m_padding) * m_fontScale, 0); // BL
            m_uiVertices[index + 1].position = start + new Vector3(0, m_padding * m_fontScale, 0); // TL
            m_uiVertices[index + 2].position = start + new Vector3(segmentWidth, m_padding * m_fontScale, 0); // TR
            m_uiVertices[index + 3].position = m_uiVertices[index + 0].position + new Vector3(segmentWidth, 0, 0); // BR

            // Middle Part of the Underline
            m_uiVertices[index + 4].position = m_uiVertices[index + 3].position; // BL
            m_uiVertices[index + 5].position = m_uiVertices[index + 2].position; // TL
            m_uiVertices[index + 6].position = end + new Vector3(-segmentWidth, m_padding * m_fontScale, 0);  // TR
            m_uiVertices[index + 7].position = end + new Vector3(-segmentWidth, -(underlineThickness + m_padding) * m_fontScale, 0); // BR

            // End Part of the Underline
            m_uiVertices[index + 8].position = m_uiVertices[index + 7].position; // BL
            m_uiVertices[index + 9].position = m_uiVertices[index + 6].position; // TL
            m_uiVertices[index + 10].position = end + new Vector3(0, m_padding * m_fontScale, 0); // TR
            m_uiVertices[index + 11].position = end + new Vector3(0, -(underlineThickness + m_padding) * m_fontScale, 0); // BR


            // Calculate UV required to setup the 3 Quads for the Underline.
            Vector2 uv0 = new Vector2((m_cached_Underline_GlyphInfo.x - m_padding) / m_fontAsset.fontInfo.AtlasWidth, 1 - (m_cached_Underline_GlyphInfo.y + m_padding + m_cached_Underline_GlyphInfo.height) / m_fontAsset.fontInfo.AtlasHeight);  // bottom left
            Vector2 uv1 = new Vector2(uv0.x, 1 - (m_cached_Underline_GlyphInfo.y - m_padding) / m_fontAsset.fontInfo.AtlasHeight);  // top left
            Vector2 uv2 = new Vector2((m_cached_Underline_GlyphInfo.x + m_padding + m_cached_Underline_GlyphInfo.width / 2) / m_fontAsset.fontInfo.AtlasWidth, uv1.y); // Top Right
            Vector2 uv3 = new Vector2(uv2.x, uv0.y); // Bottom right
            Vector2 uv4 = new Vector2((m_cached_Underline_GlyphInfo.x + m_padding + m_cached_Underline_GlyphInfo.width) / m_fontAsset.fontInfo.AtlasWidth, uv1.y); // End Part - Bottom Right
            Vector2 uv5 = new Vector2(uv4.x, uv0.y); // End Part - Top Right

            // Left Part of the Underline
            m_uiVertices[0 + index].uv0 = uv0; // BL
            m_uiVertices[1 + index].uv0 = uv1; // TL   
            m_uiVertices[2 + index].uv0 = uv2; // TR   
            m_uiVertices[3 + index].uv0 = uv3; // BR

            // Middle Part of the Underline
            m_uiVertices[4 + index].uv0 = new Vector2(uv2.x - uv2.x * 0.001f, uv0.y);
            m_uiVertices[5 + index].uv0 = new Vector2(uv2.x - uv2.x * 0.001f, uv1.y);
            m_uiVertices[6 + index].uv0 = new Vector2(uv2.x + uv2.x * 0.001f, uv1.y);
            m_uiVertices[7 + index].uv0 = new Vector2(uv2.x + uv2.x * 0.001f, uv0.y);

            // Right Part of the Underline
            m_uiVertices[8 + index].uv0 = uv3;
            m_uiVertices[9 + index].uv0 = uv2;
            m_uiVertices[10 + index].uv0 = uv4;
            m_uiVertices[11 + index].uv0 = uv5;

          
            // UV1 contains Face / Border UV layout.
            float min_UvX = 0;
            float max_UvX = (m_uiVertices[index + 2].position.x - start.x) / (end.x - start.x);

            //Calculate the xScale or how much the UV's are getting stretched on the X axis for the middle section of the underline.
            float xScale = m_fontScale * m_rectTransform.lossyScale.z;
            float xScale2 = xScale;

            m_uiVertices[0 + index].uv1 = PackUV(0, 0, xScale);
            m_uiVertices[1 + index].uv1 = PackUV(0, 1, xScale);
            m_uiVertices[2 + index].uv1 = PackUV(max_UvX, 1, xScale);
            m_uiVertices[3 + index].uv1 = PackUV(max_UvX, 0, xScale);

            min_UvX = (m_uiVertices[index + 4].position.x - start.x) / (end.x - start.x);
            max_UvX = (m_uiVertices[index + 6].position.x - start.x) / (end.x - start.x);

            m_uiVertices[4 + index].uv1 = PackUV(min_UvX, 0, xScale2);
            m_uiVertices[5 + index].uv1 = PackUV(min_UvX, 1, xScale2);
            m_uiVertices[6 + index].uv1 = PackUV(max_UvX, 1, xScale2);
            m_uiVertices[7 + index].uv1 = PackUV(max_UvX, 0, xScale2);

            min_UvX = (m_uiVertices[index + 8].position.x - start.x) / (end.x - start.x);
            max_UvX = (m_uiVertices[index + 6].position.x - start.x) / (end.x - start.x);

            m_uiVertices[8 + index].uv1 = PackUV(min_UvX, 0, xScale);
            m_uiVertices[9 + index].uv1 = PackUV(min_UvX, 1, xScale);
            m_uiVertices[10 + index].uv1 = PackUV(1, 1, xScale);
            m_uiVertices[11 + index].uv1 = PackUV(1, 0, xScale);


            //underlineColor.a /= 2; // Since bold is encoded in the alpha, we need to adjust this.

            m_uiVertices[0 + index].color = underlineColor;
            m_uiVertices[1 + index].color = underlineColor;
            m_uiVertices[2 + index].color = underlineColor;
            m_uiVertices[3 + index].color = underlineColor;

            m_uiVertices[4 + index].color = underlineColor;
            m_uiVertices[5 + index].color = underlineColor;
            m_uiVertices[6 + index].color = underlineColor;
            m_uiVertices[7 + index].color = underlineColor;

            m_uiVertices[8 + index].color = underlineColor;
            m_uiVertices[9 + index].color = underlineColor;
            m_uiVertices[10 + index].color = underlineColor;
            m_uiVertices[11 + index].color = underlineColor;

            index += 12;
        }


        /// <summary>
        /// Method to Update Scale in UV2
        /// </summary>
        void UpdateSDFScale(float prevScale, float newScale)
        {
            for (int i = 0; i < m_uiVertices.Length; i++)
            {
                m_uiVertices[i].uv1.y = (m_uiVertices[i].uv1.y / prevScale) * newScale; 
            }

            m_uiRenderer.SetVertices(m_uiVertices, m_uiVertices.Length);
        }


        /// <summary>
        /// Function to Resize the Mesh Buffers
        /// </summary>
        /// <param name="size"></param>
        void ResizeMeshBuffers(int size)
        {
            int sizeX4 = size * 4;
            //int sizeX6 = size * 6;
            int previousSize = m_uiVertices.Length / 4;

            //Debug.Log("Resizing Mesh Buffers from " + previousSize + " to " + size + ".");

            Array.Resize(ref m_uiVertices, sizeX4);
            
            for (int i = previousSize; i < size; i++)
            {
                int index_X4 = i * 4;
                //int index_X6 = i * 6;

                m_uiVertices[0 + index_X4].normal = new Vector3(0, 0, -1);
                m_uiVertices[1 + index_X4].normal = new Vector3(0, 0, -1);
                m_uiVertices[2 + index_X4].normal = new Vector3(0, 0, -1);
                m_uiVertices[3 + index_X4].normal = new Vector3(0, 0, -1);

                m_uiVertices[0 + index_X4].tangent = new Vector4(-1, 0, 0, 1);
                m_uiVertices[1 + index_X4].tangent = new Vector4(-1, 0, 0, 1);
                m_uiVertices[2 + index_X4].tangent = new Vector4(-1, 0, 0, 1);
                m_uiVertices[3 + index_X4].tangent = new Vector4(-1, 0, 0, 1);
            }
        }



        // Used with Advanced Layout Component.
        void UpdateMeshData(TMP_CharacterInfo[] characterInfo, int characterCount, Mesh mesh, Vector3[] vertices, Vector2[] uv0s, Vector2[] uv2s, Color32[] vertexColors, Vector3[] normals, Vector4[] tangents)
        {
            m_textInfo.characterInfo = characterInfo;
            m_textInfo.characterCount = (short)characterCount;
            //m_meshInfo.mesh = mesh;
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
                    //int vertexIndex = m_textInfo.characterInfo[i].vertexIndex;
                    m_textInfo.characterInfo[i].vertex_BL.position = m_textInfo.characterInfo[i].bottomLeft -= vertexOffset;
                    m_textInfo.characterInfo[i].vertex_TL.position = m_textInfo.characterInfo[i].topLeft -= vertexOffset;
                    m_textInfo.characterInfo[i].vertex_TR.position = m_textInfo.characterInfo[i].topRight -= vertexOffset;
                    m_textInfo.characterInfo[i].vertex_BR.position = m_textInfo.characterInfo[i].bottomRight -= vertexOffset;
                    
                    m_textInfo.characterInfo[i].bottomLine -= vertexOffset.y;
                    m_textInfo.characterInfo[i].topLine -= vertexOffset.y;

                    //m_uiVertices[0 + vertexIndex].position -= vertexOffset;
                    //m_uiVertices[1 + vertexIndex].position -= vertexOffset;
                    //m_uiVertices[2 + vertexIndex].position -= vertexOffset;
                    //m_uiVertices[3 + vertexIndex].position -= vertexOffset;
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
            state.visible_SpriteCount = m_visibleSpriteCount;        
            state.xAdvance = m_xAdvance;
            state.maxAscender = m_maxAscender;
            state.maxDescender = m_maxDescender;
            state.preferredWidth = m_preferredWidth;
            state.preferredHeight = m_preferredHeight;
            state.fontScale = m_fontScale;
            state.maxFontScale = m_maxFontScale; 
            //state.previousLineScale = m_previousFontScale;
                    
            state.currentFontSize = m_currentFontSize;
            state.lineNumber = m_lineNumber; 
            state.lineOffset = m_lineOffset;
            state.baselineOffset = m_baselineOffset;
            state.fontStyle = m_style;
            //state.alignment = m_lineJustification;
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
            //m_lineJustification = state.alignment;
            m_htmlColor = state.vertexColor;
            m_colorStackIndex = state.colorStackIndex;

            m_characterCount = state.total_CharacterCount + 1;
            m_visibleCharacterCount = state.visible_CharacterCount;
            m_visibleSpriteCount = state.visible_SpriteCount;
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
            //m_previousFontScale = state.previousLineScale;
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
                        m_fontScale = m_currentFontSize / m_fontAsset.fontInfo.PointSize;
                        m_baselineOffset = m_fontAsset.fontInfo.SubscriptOffset * m_fontScale;
                        //m_isRecalculateScaleRequired = true;
                        return true;
                    case 679: // <pos=000.00>
                        float spacing = ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);
                        m_xAdvance = spacing * m_fontScale * m_fontAsset.fontInfo.TabWidth;
                        return true;
                    case 685: // <sup>
                        m_currentFontSize *= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1;
                        m_fontScale = m_currentFontSize / m_fontAsset.fontInfo.PointSize;
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
                        m_fontScale = m_currentFontSize / m_fontAsset.fontInfo.PointSize;
                        //m_isRecalculateScaleRequired = true;
                        return true;
                    case 1076: // </sup>
                        m_currentFontSize /= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1; //m_fontSize / m_fontAsset.FontInfo.PointSize * .1f;
                        m_baselineOffset = 0;
                        m_fontScale = m_currentFontSize / m_fontAsset.fontInfo.PointSize;
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
                        //Debug.Log("Font Tag used.");
                        m_fontIndex = (int)ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);

                        if(m_fontAssetArray[m_fontIndex] == null)
                        {
                            // Load new font asset into index
                            //Debug.Log("Loading Font Asset at Index " + m_fontIndex);
                            m_fontAssetArray[m_fontIndex] = m_fontAsset; // Resources.Load("Fonts & Materials/Bangers SDF", typeof(TextMeshProFont)) as TextMeshProFont; // Hard coded right now to a specific font
                        }
                        else
                        {
                            //Debug.Log("Font Asset at Index " + m_fontIndex + " has already been loaded.");
                        }


                        //m_fontScale = (m_fontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize * (m_isOrthographic ? 1 : 0.1f));
                       
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
                    case 1613: // <width=xx>
                        m_width = ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);                  
                        return true;     
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
                    case 2204: // </width>
                        m_width = 0;
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
                        m_spriteIndex = (int)ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);
                        //Debug.Log("Sprite Index is " + ((int)ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos)));
                        m_isSprite = true;
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

#endif