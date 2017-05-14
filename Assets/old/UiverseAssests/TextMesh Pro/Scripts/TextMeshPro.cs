// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms
// Beta Release 0.1.5 Beta 1.5


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;



namespace TMPro
{
    public enum TextAlignmentOptions {  TopLeft = 0, Top = 1, TopRight = 2, TopJustified = 3,
                                        Left = 4, Center = 5, Right = 6, Justified = 7,
                                        BottomLeft = 8, Bottom = 9, BottomRight = 10, BottomJustified = 11,
                                        BaselineLeft = 12, Baseline = 13, BaselineRight = 14, BaselineJustified = 15,
                                        MidlineLeft = 16, Midline = 17, MidlineRight = 18, MidlineJustified = 19 };

    //public enum TextAlignmentOptions { Left = 0, Center = 1, Right = 2, Top = 4, Bottom = 8, Justified = 16, Baseline = 32, 
    //                                   TopLeft = Left + Top, BottomLeft = Left + Bottom, BaselineLeft = Left + Baseline,
    //                                   TopRight = Right + Top, BottomRight = Right + Bottom, BaselineRight = Right + Baseline,
    //                                   TopJustified = Justified + Top, BottomJustified = Justified + Bottom, BaselineJustified = Justified + Baseline };                         
    
          
    //public enum AlignmentTypes { Left, Center, Right, Justified };

    public enum TextRenderFlags { Render, DontRender, GetPreferredSizes };
    
    public enum MaskingTypes { MaskOff = 0, MaskHard = 1, MaskSoft = 2 };
    public enum TextOverflowModes { Overflow = 0, Ellipsis = 1, Masking = 2, Truncate = 3, ScrollRect = 4, Page = 5 };
    public enum MaskingOffsetMode {  Percentage = 0, Pixel = 1 };  
    public enum TextureMappingOptions { Character = 0, Line = 1, Paragraph = 2, MatchAspect = 3 };

    public enum FontStyles { Normal = 0, Bold = 1, Italic = 2, Underline = 4, LowerCase = 8, UpperCase = 16, SmallCaps = 32,
                            BoldItalic = Bold + Italic, BoldUnderline = Bold + Underline, BoldItalicUnderline = BoldUnderline + Italic, 
                            ItalicUnderline = Italic + Underline, 
                            LowerCaseBold = LowerCase + Bold, LowerCaseItalic = LowerCase + Italic, LowerCaseUnderline = LowerCase + Underline, LowerCaseBoldItalic = LowerCase + BoldItalic, LowerCaseBoldUnderline = LowerCase + BoldUnderline, LowerCaseBoldItalicUnderline = LowerCase + BoldItalicUnderline,
                            UpperCaseBold = UpperCase + Bold, UpperCaseItalic = UpperCase + Italic, UpperCaseUnderline = UpperCase + Underline, UpperCaseBoldItalic = UpperCase + BoldItalic, UpperCaseBoldUnderline = UpperCase + BoldUnderline, UpperCaseBoltItalicUnderline = UpperCase + BoldItalicUnderline,
                            SmallCapsBold = SmallCaps + Bold, SmallCapsItalic = SmallCaps + Italic, SmallCapsUnderline = SmallCaps + Underline, SmallCapsBoldItalic = SmallCaps + BoldItalic, SmallCapsBoldUnderline = SmallCaps + BoldUnderline, SmallCapsBoldItalicUnderline = SmallCaps + BoldItalicUnderline
                            };

   
    [ExecuteInEditMode]
    [RequireComponent(typeof(TextContainer))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))] 
    [AddComponentMenu("Mesh/TextMesh Pro")]
    public partial class TextMeshPro : MonoBehaviour
    {
        // Public Properties & Serializable Properties  
        
        /// <summary>
        /// A string containing the text to be displayed.
        /// </summary>
        public string text
        {
            get { return m_text; }
            set { m_inputSource = TextInputSources.Text; havePropertiesChanged = true; isInputParsingRequired = true; m_text = value; /* ScheduleUpdate(); */ }
        }


        /// <summary>
        /// The TextMeshPro font asset to be assigned to this text object.
        /// </summary>
        public TextMeshProFont font
        {
            get { return m_fontAsset; }
            set { if (m_fontAsset != value) { m_fontAsset = value; LoadFontAsset(); havePropertiesChanged = true; /* hasFontAssetChanged = true;*/ /* ScheduleUpdate(); */} }
        }


        /// <summary>
        /// The material to be assigned to this text object. An instance of the material will be assigned to the object's renderer.
        /// </summary>
        public Material fontMaterial
        {           
            // Return a new Instance of the Material if none exists. Otherwise return the current Material Instance.
            get 
            {           
                if (m_fontMaterial == null)
                {
                    SetFontMaterial(m_sharedMaterial);
                    return m_sharedMaterial;
                }
                return m_sharedMaterial;
            }

            // Assigning fontMaterial always returns an instance of the material.
            set { SetFontMaterial(value); havePropertiesChanged = true; /* ScheduleUpdate(); */  }
        }


        /// <summary>
        /// The material to be assigned to this text object.
        /// </summary>
        public Material fontSharedMaterial
        {
            get { return m_renderer.sharedMaterial; }
            set { if (m_sharedMaterial != value) { SetSharedFontMaterial(value); havePropertiesChanged = true; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Sets the RenderQueue along with Ztest to force the text to be drawn last and on top of scene elements.
        /// </summary>
        public bool isOverlay
        {
            get { return m_isOverlay; }
            set { m_isOverlay = value; SetShaderType(); havePropertiesChanged = true; /* ScheduleUpdate(); */  }
        }


        /// <summary>
        /// This is the default vertex color assigned to each vertices. Color tags will override vertex colors unless the overrideColorTags is set.
        /// </summary>      
        public Color color
        {
            get { return m_fontColor; }
            set { if (!m_fontColor.Compare(value)) { havePropertiesChanged = true; m_fontColor = value; /* ScheduleUpdate(); */ } }
        }

		/// <summary>
		/// Sets the vertex colors for each of the 4 vertices of the character quads.
		/// </summary>
		/// <value>The color gradient.</value>
		public VertexGradient colorGradient
		{
			get { return m_fontColorGradient;}
			set { havePropertiesChanged = true; m_fontColorGradient = value; }
		}

		/// <summary>
		/// Determines if Vertex Color Gradient should be used
		/// </summary>
		/// <value><c>true</c> if enable vertex gradient; otherwise, <c>false</c>.</value>
		public bool enableVertexGradient
		{
			get { return m_enableVertexGradient; }
			set { havePropertiesChanged = true; m_enableVertexGradient = value; }
		}


        /// <summary>
        /// Sets the color of the _FaceColor property of the assigned material. Changing face color will result in an instance of the material.
        /// </summary>
        public Color32 faceColor
        {
            get { return m_faceColor; }
            set { if (m_faceColor.Compare(value) == false) { SetFaceColor(value); havePropertiesChanged = true; m_faceColor = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Sets the color of the _OutlineColor property of the assigned material. Changing outline color will result in an instance of the material.
        /// </summary>
        public Color32 outlineColor
        {
            get { return m_outlineColor; }
            set { if (m_outlineColor.Compare(value) == false) { SetOutlineColor(value); havePropertiesChanged = true; m_outlineColor = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Sets the thickness of the outline of the font. Setting this value will result in an instance of the material.
        /// </summary>
        public float outlineWidth
        {
            get { return m_outlineWidth; }
            set { SetOutlineThickness(value); havePropertiesChanged = true; checkPaddingRequired = true; m_outlineWidth = value; /* ScheduleUpdate(); */ }
        }


        /// <summary>
        /// The size of the font.
        /// </summary>
        public float fontSize
        {
            get { return m_fontSize; }
            set { if (m_fontSize != value) { havePropertiesChanged = true; /* hasFontScaleChanged = true; */ m_fontSize = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// The scale of the current text.
        /// </summary>
        public float fontScale
        {
            get { return m_fontScale; }
        }


        /// <summary>
        /// The style of the text
        /// </summary>
        public FontStyles fontStyle
        {
            get { return m_fontStyle; }
            set { m_fontStyle = value; havePropertiesChanged = true; checkPaddingRequired = true; }
        }


        /// <summary>
        /// The amount of additional spacing between characters.
        /// </summary>
        public float characterSpacing
        {
            get { return m_characterSpacing; }
            set { if (m_characterSpacing != value) { havePropertiesChanged = true; m_characterSpacing = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Enables or Disables Rich Text Tags
        /// </summary>
        public bool richText
        {
            get { return m_isRichText; }
            set { m_isRichText = value; havePropertiesChanged = true; isInputParsingRequired = true; }
        }

      
        /// <summary>
        /// Determines where word wrap will occur.
        /// </summary>
        [Obsolete("The length of the line is now controlled by the size of the text container and margins.")]
        public float lineLength
        {
            get { return m_lineLength; }
            set { Debug.Log("lineLength set called.");  }
        }


        /// <summary>
        /// Controls the Text Overflow Mode
        /// </summary>
        public TextOverflowModes OverflowMode
        {
            get { return m_overflowMode;  }
            set { m_overflowMode = value; havePropertiesChanged = true; }
        }


        /// <summary>
        /// Contains the bounds of the text object.
        /// </summary>
        public Bounds bounds
        {
            get { if (m_mesh != null) return m_mesh.bounds; return new Bounds(); }
            //set { if (_meshExtents != value) havePropertiesChanged = true; _meshExtents = value; }
        }

        /// <summary>
        /// The amount of additional spacing to add between each lines of text.
        /// </summary>
        public float lineSpacing
        {
            get { return m_lineSpacing; }
            set { if (m_lineSpacing != value) { havePropertiesChanged = true; m_lineSpacing = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// The amount of additional spacing to add between each lines of text.
        /// </summary>
        public float paragraphSpacing
        {
            get { return m_paragraphSpacing; }
            set { if (m_paragraphSpacing != value) { havePropertiesChanged = true; m_paragraphSpacing = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Determines the anchor position of the text object.  
        /// </summary>       
        [Obsolete("The length of the line is now controlled by the size of the text container and margins.")]
        public TMP_Compatibility.AnchorPositions anchor
        {
            get { return m_anchor; }
        }
        
              
        /// <summary>
        /// Text alignment options
        /// </summary>
        public TextAlignmentOptions alignment
        {
            get { return m_textAlignment; }
            set { if (m_textAlignment != value) { havePropertiesChanged = true; m_textAlignment = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Determines if kerning is enabled or disabled.
        /// </summary>
        public bool enableKerning
        {
            get { return m_enableKerning; }
            set { if (m_enableKerning != value) { havePropertiesChanged = true; m_enableKerning = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Anchor dampening prevents the anchor position from being adjusted unless the positional change exceeds about 40% of the width of the underline character. This essentially stabilizes the anchor position.
        /// </summary>
        public bool anchorDampening
        {
            get { return m_anchorDampening; }
            set { if (m_anchorDampening != value) { havePropertiesChanged = true; m_anchorDampening = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// This overrides the color tags forcing the vertex colors to be the default font color.
        /// </summary>
        public bool overrideColorTags
        {
            get { return m_overrideHtmlColors; }
            set { if (m_overrideHtmlColors != value) { havePropertiesChanged = true; m_overrideHtmlColors = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Adds extra padding around each character. This may be necessary when the displayed text is very small to prevent clipping.
        /// </summary>
        public bool extraPadding
        {
            get { return m_enableExtraPadding; }
            set { if (m_enableExtraPadding != value) { havePropertiesChanged = true; checkPaddingRequired = true; m_enableExtraPadding = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Controls whether or not word wrapping is applied. When disabled, the text will be displayed on a single line.
        /// </summary>
        public bool enableWordWrapping
        {
            get { return m_enableWordWrapping; }
            set { if (m_enableWordWrapping != value) { havePropertiesChanged = true; isInputParsingRequired = true; m_enableWordWrapping = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Controls how the face and outline textures will be applied to the text object.
        /// </summary>
        public TextureMappingOptions horizontalMapping
        {
            get { return m_horizontalMapping; }
            set { if (m_horizontalMapping != value) { havePropertiesChanged = true; m_horizontalMapping = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Controls how the face and outline textures will be applied to the text object.
        /// </summary>
        public TextureMappingOptions verticalMapping
        {
            get { return m_verticalMapping; }
            set { if (m_verticalMapping != value) { havePropertiesChanged = true; m_verticalMapping = value; /* ScheduleUpdate(); */ } }
        }

        /// <summary>
        /// Forces objects that are not visible to get refreshed.
        /// </summary>
        public bool ignoreVisibility
        {
            get { return m_ignoreCulling; }
            set { if (m_ignoreCulling != value) { havePropertiesChanged = true; m_ignoreCulling = value; /* ScheduleUpdate(); */ } }
        }


        /// <summary>
        /// Sets Perspective Correction to Zero for Orthographic Camera mode and 0.875f for Perspective Camera mode.
        /// </summary>
        public bool isOrthographic
        {
            get { return m_isOrthographic; }
            set { havePropertiesChanged = true; m_isOrthographic = value; /* ScheduleUpdate(); */ }
        }


        /// <summary>
        /// Sets the culling on the shaders. Note changing this value will result in an instance of the material.
        /// </summary>
        public bool enableCulling
        {
            get { return m_isCullingEnabled; }
            set { m_isCullingEnabled = value; SetCulling(); havePropertiesChanged = true; }
        }


        /// <summary>
        /// Sets the Renderer's sorting Layer ID
        /// </summary>
        public int sortingLayerID
        {
            get { return m_renderer.sortingLayerID; }
            set { m_renderer.sortingLayerID = value; }
        }


        /// <summary>
        /// Sets the Renderer's sorting order within the assigned layer.
        /// </summary>
        public int sortingOrder
        {
            get { return m_renderer.sortingOrder; }
            set { m_renderer.sortingOrder = value; }
        }



        public bool hasChanged
        {
            get { return havePropertiesChanged; }
            set { havePropertiesChanged = value; }
        }


        /// <summary>
        /// Determines if the Mesh will be uploaded.
        /// </summary>
        public TextRenderFlags renderMode
        {
            get { return m_renderMode; }
            set { m_renderMode = value; havePropertiesChanged = true; }
        }


        /*
        public bool isAdvancedLayoutComponentPresent
        {
            //get { return m_isAdvanceLayoutComponentPresent; }
            set
            {
                if (m_isAdvanceLayoutComponentPresent != value)
                {
                    m_advancedLayoutComponent = value == true ? GetComponent<TMPro_AdvancedLayout>() : null;
                    havePropertiesChanged = true;
                    m_isAdvanceLayoutComponentPresent = value;
                }
            }
        }
        */

        /// <summary>
        /// Returns a reference to the Text Container
        /// </summary>
        public TextContainer textContainer
        {
            get
            {
                if (m_textContainer == null)
                    m_textContainer = GetComponent<TextContainer>();
                
                return m_textContainer; }
        }


        /// <summary>
        /// Returns a reference to the Transform
        /// </summary>
        //public Transform transform
        //{
        //    get { return m_transform; }
        //}


        /// <summary>
        /// Allows to control how many characters are visible from the input. Non-visible character are set to fully transparent.
        /// </summary>
        public int maxVisibleCharacters
        {
            get { return m_maxVisibleCharacters; }
            set { if (m_maxVisibleCharacters != value) { havePropertiesChanged = true; m_maxVisibleCharacters = value; } }
        }

        /// <summary>
        /// Allows control over how many lines of text are displayed.
        /// </summary>
        public int maxVisibleLines
        {
            get { return m_maxVisibleLines; }
            set { if (m_maxVisibleLines != value) { havePropertiesChanged = true; isInputParsingRequired = true; m_maxVisibleLines = value; } }
        }


        /// <summary>
        /// Controls which page of text is shown
        /// </summary>
        public int pageToDisplay
        {
            get { return m_pageToDisplay; }
            set { havePropertiesChanged = true; m_pageToDisplay = value; }
        }


        // Width of the text object if layout on a single line
        public float preferredWidth
        {
            get { return m_preferredWidth; }
           
        }


        //public TMPro_TextMetrics metrics
        //{
        //    get { return m_textMetrics; }
        //}


        //public int characterCount
        //{
        //    get { return m_textInfo.characterCount; }
        //}

        //public int lineCount
        //{
        //    get { return m_textInfo.lineCount; }
        //}


        public Vector2[] spacePositions
        {
            get { return m_spacePositions; }
        }


        public bool enableAutoSizing
        {
            get { return m_enableAutoSizing; }
            set { m_enableAutoSizing = value; }
        }

        public float fontSizeMin
        {
            get { return m_fontSizeMin; }
            set { m_fontSizeMin = value; }
        }

        public float fontSizeMax
        {
            get { return m_fontSizeMax; }
            set { m_fontSizeMax = value; }
        }

        
        // MASKING RELATED PROPERTIES
        /// <summary>
        /// Sets the mask type 
        /// </summary>
        public MaskingTypes maskType
        {
            get { return m_maskType; }
            set { m_maskType = value; SetMask(m_maskType); }
        }

        /// <summary>
        /// Function used to set the mask type and coordinates in World Space
        /// </summary>
        /// <param name="type"></param>
        /// <param name="maskCoords"></param>
        public void SetMask(MaskingTypes type, Vector4 maskCoords)
        {
            SetMask(type);

            SetMaskCoordinates(maskCoords);
        }

        /// <summary>
        /// Function used to set the mask type, coordinates and softness
        /// </summary>
        /// <param name="type"></param>
        /// <param name="maskCoords"></param>
        /// <param name="softnessX"></param>
        /// <param name="softnessY"></param>
        public void SetMask(MaskingTypes type, Vector4 maskCoords, float softnessX, float softnessY)
        {
            SetMask(type);

            SetMaskCoordinates(maskCoords, softnessX, softnessY);
        }


        /*
        /// <summary>
        /// Set the masking offset mode (as percentage or pixels)
        /// </summary>
        public MaskingOffsetMode maskOffsetMode
        {
            get { return m_maskOffsetMode; }
            set { m_maskOffsetMode = value; havePropertiesChanged = true; isMaskUpdateRequired = true; }
        }

        /// <summary>
        /// Sets the masking offset from the bounds of the object
        /// </summary>
        public Vector4 maskOffset
        {
            get { return m_maskOffset; }
            set { m_maskOffset = value;  havePropertiesChanged = true; isMaskUpdateRequired = true; }
        }

        /// <summary>
        /// Sets the softness of the mask
        /// </summary>
        public Vector2 maskSoftness
        {
            get { return m_maskSoftness; }
            set { m_maskSoftness = value; havePropertiesChanged = true; isMaskUpdateRequired = true; }
        }

        /// <summary>
        /// Allows to move / offset the mesh vertices by a set amount
        /// </summary>
        public Vector2 vertexOffset
        {
            get { return m_vertexOffset; }
            set { m_vertexOffset = value; havePropertiesChanged = true; isMaskUpdateRequired = true; }
        }
        */

        public TMP_TextInfo textInfo
        {
            get { return m_textInfo; }
        }

        //public TMPro_MeshInfo meshInfo
        //{
        //    get { return m_meshInfo; }
        //}


        public Mesh mesh
        {
            get { return m_mesh; }
        }


        //public TMPro_CharacterInfo[] characterInfo
        //{
        //    get { return m_textInfo.characterInfo; }
        //
        //}

  

        /// <summary>
        /// Function to be used to force recomputing of character padding when Shader / Material properties have been changed via script.
        /// </summary>
        public void UpdateMeshPadding()
        {
            m_padding = ShaderUtilities.GetPadding(m_renderer.sharedMaterials, m_enableExtraPadding, m_isUsingBold);
            havePropertiesChanged = true;
            /* ScheduleUpdate(); */
        }


        /// <summary>
        /// Function to force regeneration of the mesh before its normal process time. This is useful when changes to the text object properties need to be applied immediately.
        /// </summary>
        public void ForceMeshUpdate()
        {
            //Debug.Log("ForceMeshUpdate() called.");
            havePropertiesChanged = true;
            OnWillRenderObject();            
        }


      
        public void UpdateFontAsset()
        {           
            LoadFontAsset();
        }


        /// <summary>
        /// Function used to evaluate the length of a text string.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public TMP_TextInfo GetTextInfo(string text)
        {
            //TextInfo temp_textInfo = new TextInfo();

            StringToCharArray(text, ref m_char_buffer);
            m_renderMode = TextRenderFlags.DontRender;
            
            GenerateTextMesh();

            m_renderMode = TextRenderFlags.Render;            

            return this.textInfo;          
        }

        //public Vector2[] SetTextWithSpaces(string text, int numPositions)
        //{
        //    m_spacePositions = new Vector2[numPositions];

        //    this.text = text;

        //    return m_spacePositions;
        //}


        /// <summary>
        /// <para>Formatted string containing a pattern and a value representing the text to be rendered.</para>
        /// <para>ex. TextMeshPro.SetText ("Number is {0:1}.", 5.56f);</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">String containing the pattern."</param>
        /// <param name="arg0">Value is a float.</param>
        public void SetText (string text, float arg0)
        {
            SetText(text, arg0, 255, 255);
        }

        /// <summary>
        /// <para>Formatted string containing a pattern and a value representing the text to be rendered.</para>
        /// <para>ex. TextMeshPro.SetText ("First number is {0} and second is {1:2}.", 10, 5.756f);</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">String containing the pattern."</param>
        /// <param name="arg0">Value is a float.</param>
        /// <param name="arg1">Value is a float.</param>
        public void SetText (string text, float arg0, float arg1)            
        {
            SetText(text, arg0, arg1, 255);
        }

        /// <summary>
        /// <para>Formatted string containing a pattern and a value representing the text to be rendered.</para>
        /// <para>ex. TextMeshPro.SetText ("A = {0}, B = {1} and C = {2}.", 2, 5, 7);</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="text">String containing the pattern."</param>
        /// <param name="arg0">Value is a float.</param>
        /// <param name="arg1">Value is a float.</param>
        /// <param name="arg2">Value is a float.</param>
        public void SetText (string text, float arg0, float arg1, float arg2)        
        {
            // Early out if nothing has been changed from previous invocation.
            if (text == old_text && arg0 == old_arg0 && arg1 == old_arg1 && arg2 == old_arg2)
            {
                return;
            }

            // Make sure Char[] can hold the input string
            if (m_input_CharArray.Length < text.Length)
                m_input_CharArray = new char[Mathf.NextPowerOfTwo(text.Length + 1)];

            old_text = text;
            old_arg1 = 255;
            old_arg2 = 255;

            int decimalPrecision = 0;
            int index = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                if (c == 123) // '{'
                {
                    // Check if user is requesting some decimal precision. Format is {0:2}
                    if (text[i + 2] == 58) // ':'
                    {
                        decimalPrecision = text[i + 3] - 48;
                    }

                    switch (text[i + 1] - 48)
                    {
                        case 0: // 1st Arg                        
                            old_arg0 = arg0;
                            AddFloatToCharArray(arg0, ref index, decimalPrecision);
                            break;                        
                        case 1: // 2nd Arg
                            old_arg1 = arg1;
                            AddFloatToCharArray(arg1, ref index, decimalPrecision);
                            break;                       
                        case 2: // 3rd Arg
                            old_arg2 = arg2;
                            AddFloatToCharArray(arg2, ref index, decimalPrecision);
                            break;                       
                    }

                    if (text[i + 2] == 58)
                        i += 4;
                    else
                        i += 2;

                    continue;
                }
                m_input_CharArray[index] = c;
                index += 1;
            }

            m_input_CharArray[index] = (char)0;
            m_charArray_Length = index; // Set the length to where this '0' termination is.

#if UNITY_EDITOR
            // Create new string to be displayed in the Input Text Box of the Editor Panel.
            m_text = new string(m_input_CharArray, 0, index);           
#endif

            m_inputSource = TextInputSources.SetText;
            isInputParsingRequired = true;
            havePropertiesChanged = true;
            /* ScheduleUpdate(); */
        }




        /// <summary>
        /// Character array containing the text to be displayed.
        /// </summary>
        /// <param name="charArray"></param>
        public void SetCharArray(char[] charArray)
        {
            if (charArray == null || charArray.Length == 0)
                return;

            // Check to make sure chars_buffer is large enough to hold the content of the string.
            if (m_char_buffer.Length <= charArray.Length)
            {
                int newSize = Mathf.NextPowerOfTwo(charArray.Length + 1);
                m_char_buffer = new int[newSize];
            }

            int index = 0;

            for (int i = 0; i < charArray.Length; i++)
            {
                if (charArray[i] == 92 && i < charArray.Length - 1)
                {
                    switch ((int)charArray[i + 1])
                    {
                        case 110: // \n LineFeed
                            m_char_buffer[index] = (char)10;
                            i += 1;
                            index += 1;
                            continue;
                        case 114: // \r LineFeed
                            m_char_buffer[index] = (char)13;
                            i += 1;
                            index += 1;
                            continue;
                        case 116: // \t Tab
                            m_char_buffer[index] = (char)9;
                            i += 1;
                            index += 1;
                            continue;
                    }
                }

                m_char_buffer[index] = charArray[i];
                index += 1;
            }
            m_char_buffer[index] = (char)0;

            m_inputSource = TextInputSources.SetCharArray;
            havePropertiesChanged = true;
            isInputParsingRequired = true;
        }

    }
}