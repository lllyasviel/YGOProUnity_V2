// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;
using System;
using System.Collections;


namespace TMPro.EditorUtilities
{

    [CustomEditor(typeof(TextMeshPro)), CanEditMultipleObjects]
    public class TMPro_EditorPanel : Editor
    {
      

        private struct m_foldout
        { // Track Inspector foldout panel states, globally.
            public static bool textInput = true;
            public static bool fontSettings = true;
            public static bool extraSettings = false;
            public static bool shadowSetting = false;
            public static bool materialEditor = true;
        }

        private static int m_eventID;

        private static string[] uiStateLabel = new string[] { "\t- <i>Click to expand</i> -", "\t- <i>Click to collapse</i> -" };

        private const string k_UndoRedo = "UndoRedoPerformed";
        //private static string[] k_sortingLayerNames = 


        public int selAlignGrid_A = 0;
        public int selAlignGrid_B = 0;


        // Serialized Properties
        private SerializedProperty text_prop;   
        private SerializedProperty fontAsset_prop;

        private SerializedProperty fontStyle_prop;

		// Color Properties
		private SerializedProperty fontColor_prop;
		private SerializedProperty enableVertexGradient_prop;
		private SerializedProperty fontColorGradient_prop;
        private SerializedProperty overrideHtmlColor_prop;

        private SerializedProperty fontSize_prop;
        private SerializedProperty fontSizeBase_prop;

        private SerializedProperty autoSizing_prop;
        private SerializedProperty fontSizeMin_prop;
        private SerializedProperty fontSizeMax_prop;
        private SerializedProperty charSpacingMax_prop;
        private SerializedProperty lineSpacingMax_prop;

        private SerializedProperty characterSpacing_prop;     
        private SerializedProperty lineSpacing_prop;
        private SerializedProperty paragraphSpacing_prop;

        private SerializedProperty textAlignment_prop;
        //private SerializedProperty textAlignment_prop;
     
        private SerializedProperty horizontalMapping_prop;
        private SerializedProperty verticalMapping_prop;
        private SerializedProperty uvOffset_prop;
        private SerializedProperty uvLineOffset_prop;

        private SerializedProperty enableWordWrapping_prop;
        private SerializedProperty wordWrappingRatios_prop;
        private SerializedProperty textOverflowMode_prop;
        private SerializedProperty pageToDisplay_prop;
        
        private SerializedProperty enableKerning_prop;
                 
        private SerializedProperty inputSource_prop;
        private SerializedProperty havePropertiesChanged_prop;
        private SerializedProperty isInputPasingRequired_prop;
        //private SerializedProperty isAffectingWordWrapping_prop;
        private SerializedProperty isRichText_prop;

        private SerializedProperty hasFontAssetChanged_prop;

        private SerializedProperty enableExtraPadding_prop;
        private SerializedProperty checkPaddingRequired_prop;

        private SerializedProperty isOrthographic_prop;
        //private SerializedProperty isOverlay_prop;

        //private SerializedProperty textRectangle_prop;

        //private SerializedProperty isMaskUpdateRequired_prop;
        //private SerializedProperty mask_prop;
        //private SerializedProperty maskOffset_prop;
        //private SerializedProperty maskOffsetMode_prop;
        //private SerializedProperty maskSoftness_prop;

        private SerializedProperty vertexOffset_prop;


        private int m_sortingLayerID;
        private int m_sortingOrder;

     
        private bool havePropertiesChanged = false;


        private TextMeshPro m_textMeshProScript;
        //private Transform m_transform;
      
        private Renderer m_renderer;
        private int m_currentMaterialID;
        private int m_previousMaterialID;
        //private Material m_currentMaterial;
        //private TMPro_UpdateManager m_updateManager;

        //private Vector3[] handlePoints = new Vector3[4]; // { new Vector3(-10, -10, 0), new Vector3(-10, 10, 0), new Vector3(10, 10, 0), new Vector3(10, -10, 0) };
        private float prev_lineLenght;



        public void OnEnable()
        {
                      
            //Debug.Log("OnEnable() for Inspector ID " + this.GetInstanceID() + " has been called.");
            
            // Initialize the Event Listener for Undo Events.
            Undo.undoRedoPerformed += OnUndoRedo;
            //Undo.postprocessModifications += OnUndoRedoEvent;   
      
            text_prop = serializedObject.FindProperty("m_text");
            fontAsset_prop = serializedObject.FindProperty("m_fontAsset");

            fontStyle_prop = serializedObject.FindProperty("m_fontStyle");

            fontSize_prop = serializedObject.FindProperty("m_fontSize");
            fontSizeBase_prop = serializedObject.FindProperty("m_fontSizeBase");

            autoSizing_prop = serializedObject.FindProperty("m_enableAutoSizing");
            fontSizeMin_prop = serializedObject.FindProperty("m_fontSizeMin");
            fontSizeMax_prop = serializedObject.FindProperty("m_fontSizeMax");
            charSpacingMax_prop = serializedObject.FindProperty("m_charSpacingMax");
            lineSpacingMax_prop = serializedObject.FindProperty("m_lineSpacingMax");
            
            // Colors & Gradient
            fontColor_prop = serializedObject.FindProperty("m_fontColor");
            enableVertexGradient_prop = serializedObject.FindProperty ("m_enableVertexGradient");
            fontColorGradient_prop = serializedObject.FindProperty ("m_fontColorGradient");
            overrideHtmlColor_prop = serializedObject.FindProperty("m_overrideHtmlColors");

            characterSpacing_prop = serializedObject.FindProperty("m_characterSpacing");         
            lineSpacing_prop = serializedObject.FindProperty("m_lineSpacing");
            paragraphSpacing_prop = serializedObject.FindProperty("m_paragraphSpacing");
            textAlignment_prop = serializedObject.FindProperty("m_textAlignment");
          
            horizontalMapping_prop = serializedObject.FindProperty("m_horizontalMapping");
            verticalMapping_prop = serializedObject.FindProperty("m_verticalMapping");
            uvOffset_prop = serializedObject.FindProperty("m_uvOffset");
            uvLineOffset_prop = serializedObject.FindProperty("m_uvLineOffset");
                     
            enableWordWrapping_prop = serializedObject.FindProperty("m_enableWordWrapping");
            wordWrappingRatios_prop = serializedObject.FindProperty("m_wordWrappingRatios");
            textOverflowMode_prop = serializedObject.FindProperty("m_overflowMode");
            pageToDisplay_prop = serializedObject.FindProperty("m_pageToDisplay");

            enableKerning_prop = serializedObject.FindProperty("m_enableKerning");     
            isOrthographic_prop = serializedObject.FindProperty("m_isOrthographic");
            //isOverlay_prop = serializedObject.FindProperty("m_isOverlay");

            havePropertiesChanged_prop = serializedObject.FindProperty("havePropertiesChanged");
            inputSource_prop = serializedObject.FindProperty("m_inputSource");
            isInputPasingRequired_prop = serializedObject.FindProperty("isInputParsingRequired");
            //isAffectingWordWrapping_prop = serializedObject.FindProperty("isAffectingWordWrapping");
            enableExtraPadding_prop = serializedObject.FindProperty("m_enableExtraPadding");
            isRichText_prop = serializedObject.FindProperty("m_isRichText");
            checkPaddingRequired_prop = serializedObject.FindProperty("checkPaddingRequired");

            //isMaskUpdateRequired_prop = serializedObject.FindProperty("isMaskUpdateRequired");
            //mask_prop = serializedObject.FindProperty("m_mask");
            //maskOffset_prop= serializedObject.FindProperty("m_maskOffset");
            //maskOffsetMode_prop = serializedObject.FindProperty("m_maskOffsetMode");
            //maskSoftness_prop = serializedObject.FindProperty("m_maskSoftness");
            //vertexOffset_prop = serializedObject.FindProperty("m_vertexOffset");

            //m_sortingLayerID = serializedObject.FindProperty("m_sortingLayerID");
            //m_sortingOrder = serializedObject.FindProperty("m_sortingOrder");

            hasFontAssetChanged_prop = serializedObject.FindProperty("hasFontAssetChanged");

            //renderer_prop = serializedObject.FindProperty("m_renderer");

            // Get the UI Skin and Styles for the various Editors
            TMP_UIStyleManager.GetUIStyles();
            
            m_textMeshProScript = target as TextMeshPro;
            //m_transform = Selection.activeGameObject.transform;
            m_renderer = m_textMeshProScript.GetComponent<Renderer>();          
            //m_sortingLayerID = m_renderer.sortingLayerID;
            //m_sortingOrder = m_renderer.sortingOrder;
            //if (m_renderer.sharedMaterial != null)
            //    m_currentMaterial = m_renderer.sharedMaterial;

            //m_updateManager = Camera.main.gameObject.GetComponent<TMPro_UpdateManager>();
        }


        public void OnDisable()
        {
            //Debug.Log("OnDisable() for Inspector ID " + this.GetInstanceID() + " has been called.");
            
            Undo.undoRedoPerformed -= OnUndoRedo;          
            //Undo.postprocessModifications -= OnUndoRedoEvent;  
        }


        public override void OnInspectorGUI()
        {

            serializedObject.Update();

            Rect rect;
            float labelWidth = EditorGUIUtility.labelWidth = 130f;
            float fieldWidth = EditorGUIUtility.fieldWidth;

            // TEXT INPUT BOX SECTION
            if (GUILayout.Button("<b>TEXT INPUT BOX</b>" + (m_foldout.textInput ? uiStateLabel[1] : uiStateLabel[0]), TMP_UIStyleManager.Section_Label))
                m_foldout.textInput = !m_foldout.textInput;

            if (m_foldout.textInput)
            {
                EditorGUI.BeginChangeCheck();
                text_prop.stringValue = EditorGUILayout.TextArea(text_prop.stringValue, TMP_UIStyleManager.TextAreaBoxEditor, GUILayout.Height(125), GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {                    
                    inputSource_prop.enumValueIndex = 0;
                    isInputPasingRequired_prop.boolValue = true;
                    //isAffectingWordWrapping_prop.boolValue = true;
                    havePropertiesChanged = true;
                }
            }


            // FONT SETTINGS SECTION
            if (GUILayout.Button("<b>FONT SETTINGS</b>" + (m_foldout.fontSettings ? uiStateLabel[1] : uiStateLabel[0]), TMP_UIStyleManager.Section_Label))
                m_foldout.fontSettings = !m_foldout.fontSettings;

            if (m_foldout.fontSettings)
            {              
                // FONT ASSET
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(fontAsset_prop);
                if (EditorGUI.EndChangeCheck())
                {                   
                    Undo.RecordObject(m_renderer, "Asset & Material Change");
                    havePropertiesChanged = true;
                    hasFontAssetChanged_prop.boolValue = true;                 
                    //isAffectingWordWrapping_prop.boolValue = true;
                }


                // FONT STYLE
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Font Style");
                int styleValue = fontStyle_prop.intValue;

                int v1 = GUILayout.Toggle((styleValue & 1) == 1, "B", GUI.skin.button) ? 1 : 0; // Bold
                int v2 = GUILayout.Toggle((styleValue & 2) == 2, "I", GUI.skin.button) ? 2 : 0; // Italics
                int v3 = GUILayout.Toggle((styleValue & 4) == 4, "U", GUI.skin.button) ? 4 : 0; // Underline
                int v4 = GUILayout.Toggle((styleValue & 8) == 8, "ab", GUI.skin.button) ? 8 : 0; // Lowercase
                int v5 = GUILayout.Toggle((styleValue & 16) == 16, "AB", GUI.skin.button) ? 16 : 0; // Uppercase
                int v6 = GUILayout.Toggle((styleValue & 32) == 32, "S", GUI.skin.button) ? 32 : 0; // Smallcaps
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    fontStyle_prop.intValue = v1 + v2 + v3 + v4 + v5 + v6;
                    havePropertiesChanged = true;
                }


                // FACE VERTEX COLOR
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(fontColor_prop, new GUIContent("Color (Vertex)"));

                // VERTEX COLOR GRADIENT
                EditorGUILayout.BeginHorizontal();
                //EditorGUILayout.PrefixLabel("Color Gradient");               
                EditorGUILayout.PropertyField(enableVertexGradient_prop, new GUIContent("Color Gradient"), GUILayout.MinWidth(140), GUILayout.MaxWidth(200));
                EditorGUIUtility.labelWidth = 95;
                EditorGUILayout.PropertyField(overrideHtmlColor_prop, new GUIContent("Override Tags"));
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUILayout.EndHorizontal();

                if (enableVertexGradient_prop.boolValue)
                {
                    EditorGUILayout.PropertyField(fontColorGradient_prop.FindPropertyRelative("topLeft"), new GUIContent("Top Left"));
                    EditorGUILayout.PropertyField(fontColorGradient_prop.FindPropertyRelative("topRight"), new GUIContent("Top Right"));
                    EditorGUILayout.PropertyField(fontColorGradient_prop.FindPropertyRelative("bottomLeft"), new GUIContent("Bottom Left"));
                    EditorGUILayout.PropertyField(fontColorGradient_prop.FindPropertyRelative("bottomRight"), new GUIContent("Bottom Right"));
                }
                if (EditorGUI.EndChangeCheck())
                {
                    havePropertiesChanged = true;
                }


                
                // FONT SIZE
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(fontSize_prop, new GUIContent("Font Size"), GUILayout.MinWidth(168), GUILayout.MaxWidth(200));
                EditorGUIUtility.fieldWidth = fieldWidth;
                if (EditorGUI.EndChangeCheck())
                {
                    fontSizeBase_prop.floatValue = fontSize_prop.floatValue;
                    havePropertiesChanged = true;
                    //isAffectingWordWrapping_prop.boolValue = true;
                }

                EditorGUI.BeginChangeCheck();
                EditorGUIUtility.labelWidth = 70;
                EditorGUILayout.PropertyField(autoSizing_prop, new GUIContent("Auto Size"));
                EditorGUILayout.EndHorizontal();

                EditorGUIUtility.labelWidth = labelWidth;
                if (EditorGUI.EndChangeCheck())
                {
                    if (autoSizing_prop.boolValue == false)
                        fontSize_prop.floatValue = fontSizeBase_prop.floatValue;

                    havePropertiesChanged = true;
                    //isAffectingWordWrapping_prop.boolValue = true;
                }
                
                 
                
                // Show auto sizing options
                if (autoSizing_prop.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Auto Size Options");
                    EditorGUIUtility.labelWidth = 30;

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(fontSizeMin_prop, new GUIContent("Min"), GUILayout.MinWidth(50));
                    if (EditorGUI.EndChangeCheck())
                    {
                        fontSizeMin_prop.floatValue = Mathf.Min(fontSizeMin_prop.floatValue, fontSizeMax_prop.floatValue);
                        havePropertiesChanged = true;
                    }

                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(fontSizeMax_prop, new GUIContent("Max"), GUILayout.MinWidth(50));
                    if (EditorGUI.EndChangeCheck())
                    {
                        fontSizeMax_prop.floatValue = Mathf.Max(fontSizeMin_prop.floatValue, fontSizeMax_prop.floatValue);
                        havePropertiesChanged = true;
                    }

                    EditorGUI.BeginChangeCheck();
                    //EditorGUILayout.PropertyField(charSpacingMax_prop, new GUIContent("Char"), GUILayout.MinWidth(50));
                    EditorGUILayout.PropertyField(lineSpacingMax_prop, new GUIContent("Line"), GUILayout.MinWidth(50));

                    EditorGUIUtility.labelWidth = labelWidth;
                    EditorGUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        charSpacingMax_prop.floatValue = Mathf.Min(0, charSpacingMax_prop.floatValue);
                        lineSpacingMax_prop.floatValue = Mathf.Min(0, lineSpacingMax_prop.floatValue);
                        havePropertiesChanged = true;
                    }
                }
                
                 
                
                // CHARACTER, LINE & PARAGRAPH SPACING
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Spacing Options");
                EditorGUIUtility.labelWidth = 30;
                EditorGUILayout.PropertyField(characterSpacing_prop, new GUIContent("Char"), GUILayout.MinWidth(50)); //, GUILayout.MaxWidth(100));               
                EditorGUILayout.PropertyField(lineSpacing_prop, new GUIContent("Line"), GUILayout.MinWidth(50)); //, GUILayout.MaxWidth(100));                           
                EditorGUILayout.PropertyField(paragraphSpacing_prop, new GUIContent(" Par."), GUILayout.MinWidth(50)); //, GUILayout.MaxWidth(100));

                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    havePropertiesChanged = true;
                    //isAffectingWordWrapping_prop.boolValue = true;
                }
                
                
                // TEXT ALIGNMENT
                EditorGUI.BeginChangeCheck();
                rect = EditorGUILayout.GetControlRect(false, 17);
                GUIStyle btn = new GUIStyle(GUI.skin.button);
                btn.margin = new RectOffset(1, 1, 1, 1);
                btn.padding = new RectOffset(1, 1, 1, 0);

                selAlignGrid_A = textAlignment_prop.enumValueIndex & ~28;
                selAlignGrid_B = (textAlignment_prop.enumValueIndex & ~3) / 4;

                GUI.Label(new Rect(rect.x, rect.y, 100, rect.height), "Alignment");
                float columnB = EditorGUIUtility.labelWidth + 15;
                selAlignGrid_A = GUI.SelectionGrid(new Rect(columnB, rect.y, 23 * 4, rect.height), selAlignGrid_A, TMP_UIStyleManager.alignContent_A, 4, btn);
                selAlignGrid_B = GUI.SelectionGrid(new Rect(columnB + 23 * 4 + 10, rect.y, 23 * 5, rect.height), selAlignGrid_B, TMP_UIStyleManager.alignContent_B, 5, btn);

                if (EditorGUI.EndChangeCheck())
                {
                    textAlignment_prop.enumValueIndex = selAlignGrid_A + selAlignGrid_B * 4;
                    havePropertiesChanged = true;
                }

                // WRAPPING RATIOS shown if Justified mode is selected.
                EditorGUI.BeginChangeCheck();
                if (textAlignment_prop.enumValueIndex == 3 || textAlignment_prop.enumValueIndex == 7 || textAlignment_prop.enumValueIndex == 11 || textAlignment_prop.enumValueIndex == 19)
                    DrawPropertySlider("Wrap Mix (W <-> C)", wordWrappingRatios_prop);

                if (EditorGUI.EndChangeCheck())
                    havePropertiesChanged = true;
                


                // TEXT WRAPPING & OVERFLOW        
                EditorGUI.BeginChangeCheck();
                rect = EditorGUILayout.GetControlRect(false);
                EditorGUI.PrefixLabel(new Rect(rect.x, rect.y, 130, rect.height), new GUIContent("Wrapping & Overflow"));
                rect.width = (rect.width - 130) / 2f;
                rect.x += 130;
                int wrapSelection = EditorGUI.Popup(rect, enableWordWrapping_prop.boolValue ? 1 : 0, new string[] { "Disabled", "Enabled" });
                if (EditorGUI.EndChangeCheck())
                {
                    enableWordWrapping_prop.boolValue = wrapSelection == 1 ? true : false;
                    havePropertiesChanged = true;
                    isInputPasingRequired_prop.boolValue = true;
                }

                // TEXT OVERFLOW
                EditorGUI.BeginChangeCheck();
                if (textOverflowMode_prop.enumValueIndex != 5)
                {
                    rect.x += rect.width + 5f;
                    rect.width -= 5;
                    EditorGUI.PropertyField(rect, textOverflowMode_prop, GUIContent.none);
                }
                else
                {
                    rect.x += rect.width + 5f;
                    rect.width /= 2;
                    EditorGUI.PropertyField(rect, textOverflowMode_prop, GUIContent.none);
                    rect.x += rect.width;
                    rect.width -= 5;
                    EditorGUI.PropertyField(rect, pageToDisplay_prop, GUIContent.none);
                }
                if (EditorGUI.EndChangeCheck())
                {
                    havePropertiesChanged = true;
                    isInputPasingRequired_prop.boolValue = true;
                }


                // TEXTURE MAPPING OPTIONS   
                EditorGUI.BeginChangeCheck();
                rect = EditorGUILayout.GetControlRect(false);
                EditorGUI.PrefixLabel(new Rect(rect.x, rect.y, 130, rect.height), new GUIContent("UV Mapping Options"));
                rect.width = (rect.width - 130) / 2f;
                rect.x += 130;
                EditorGUI.PropertyField(rect, horizontalMapping_prop, GUIContent.none);
                rect.x += rect.width + 5f;
                rect.width -= 5;
                EditorGUI.PropertyField(rect, verticalMapping_prop, GUIContent.none);
                if (EditorGUI.EndChangeCheck())
                {
                    havePropertiesChanged = true;
                }

                // UV OPTIONS
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("UV Offset");
                EditorGUILayout.PropertyField(uvOffset_prop, GUIContent.none, GUILayout.MinWidth(70f));
                EditorGUIUtility.labelWidth = 30;
                EditorGUILayout.PropertyField(uvLineOffset_prop, new GUIContent("Line"), GUILayout.MinWidth(70f));
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    havePropertiesChanged = true;
                }

                // KERNING
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(enableKerning_prop, new GUIContent("Enable Kerning?"));
                if (EditorGUI.EndChangeCheck())
                {                 
                    //isAffectingWordWrapping_prop.boolValue = true;
                    havePropertiesChanged = true;
                }

                // EXTRA PADDING
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(enableExtraPadding_prop, new GUIContent("Extra Padding?"));
                if (EditorGUI.EndChangeCheck())
                {                 
                    havePropertiesChanged = true;
                    checkPaddingRequired_prop.boolValue = true;
                }
                EditorGUILayout.EndHorizontal();             
            }



            if (GUILayout.Button("<b>EXTRA SETTINGS</b>" + (m_foldout.extraSettings ? uiStateLabel[1] : uiStateLabel[0]), TMP_UIStyleManager.Section_Label))
                m_foldout.extraSettings = !m_foldout.extraSettings;

            if (m_foldout.extraSettings)
            {
                EditorGUI.indentLevel = 0;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Sorting Layer");
                EditorGUI.BeginChangeCheck();

                float old_LabelWidth = EditorGUIUtility.labelWidth;
                float old_fieldWidth = EditorGUIUtility.fieldWidth;

                
                // SORTING LAYERS
                var sortingLayerNames = SortingLayerHelper.sortingLayerNames;

                // Look up the layer name using the current layer ID
                string oldName = SortingLayerHelper.GetSortingLayerNameFromID(m_textMeshProScript.sortingLayerID);

                // Use the name to look up our array index into the names list
                int oldLayerIndex = Array.IndexOf(sortingLayerNames, oldName);
              
                // Show the popup for the names              
                EditorGUIUtility.fieldWidth = 0f;
                int newLayerIndex = EditorGUILayout.Popup(string.Empty, oldLayerIndex, sortingLayerNames, GUILayout.MinWidth(80f));

                // If the index changes, look up the ID for the new index to store as the new ID
                if (newLayerIndex != oldLayerIndex)
                {
                    //Undo.RecordObject(renderer, "Edit Sorting Layer");
                    m_textMeshProScript.sortingLayerID = SortingLayerHelper.GetSortingLayerIDForIndex(newLayerIndex);
                    //EditorUtility.SetDirty(renderer);
                }

                // Expose the manual sorting order              
                EditorGUIUtility.labelWidth = 40f;
                EditorGUIUtility.fieldWidth = 80f;
                int newSortingLayerOrder = EditorGUILayout.IntField("Order", m_textMeshProScript.sortingOrder);
                if (newSortingLayerOrder != m_textMeshProScript.sortingOrder)
                {
                    //Undo.RecordObject(renderer, "Edit Sorting Order");
                    m_textMeshProScript.sortingOrder = newSortingLayerOrder;                 
                }                            
                EditorGUILayout.EndHorizontal();
                EditorGUIUtility.labelWidth = old_LabelWidth;
                EditorGUIUtility.fieldWidth = old_fieldWidth;

                EditorGUI.BeginChangeCheck();
                //EditorGUILayout.PropertyField(isOverlay_prop, new GUIContent("Overlay Mode?"));
                EditorGUILayout.PropertyField(isOrthographic_prop, new GUIContent("Orthographic Mode?"));
                EditorGUILayout.PropertyField(isRichText_prop, new GUIContent("Enable Rich Text?"));
                //EditorGUILayout.PropertyField(textRectangle_prop, true);

                if (EditorGUI.EndChangeCheck())                                 
                    havePropertiesChanged = true;


                // EditorGUI.BeginChangeCheck();
                //EditorGUILayout.PropertyField(mask_prop);
                //EditorGUILayout.PropertyField(maskOffset_prop, true);
                //EditorGUILayout.PropertyField(maskSoftness_prop);
                //if (EditorGUI.EndChangeCheck())
                //{
                //    isMaskUpdateRequired_prop.boolValue = true;
                //    havePropertiesChanged = true;
                //}

                //EditorGUILayout.PropertyField(sortingLayerID_prop);
                //EditorGUILayout.PropertyField(sortingOrder_prop);

                // Mask Selection
            }

            /*
            if (Event.current.type == EventType.DragExited)
            {
                m_currentMaterialID = m_renderer.sharedMaterial.GetInstanceID();
                if (m_currentMaterialID != m_previousMaterialID)
                {
                    Debug.Log("Material has been changed to " + m_currentMaterialID + ". Previous Material was " + m_previousMaterialID);

                    //m_targetMaterial = m_renderer.sharedMaterial;
                    //TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, m_targetMaterial);
                    m_previousMaterialID = m_currentMaterialID;
                    havePropertiesChanged = true;
                }
            }
            */

            if (havePropertiesChanged)
            {             
                havePropertiesChanged_prop.boolValue = true;
                havePropertiesChanged = false;              
                //m_updateManager.ScheduleObjectForUpdate(m_textMeshProScript);
            }

            EditorGUILayout.Space();

            serializedObject.ApplyModifiedProperties();

          
            /*
            Editor materialEditor = Editor.CreateEditor(m_renderer.sharedMaterial);
            if (materialEditor != null)
            {
                if (GUILayout.Button("<b>MATERIAL SETTINGS</b>     - <i>Click to expand</i> -", Section_Label))
                    m_foldout.materialEditor= !m_foldout.materialEditor;

                if (m_foldout.materialEditor)
                {
                    materialEditor.OnInspectorGUI();
                }
            }
            */
        }

        /*
        public void OnSceneGUI()
        {
            
            if (enableWordWrapping_prop.boolValue)
            {                             
                // Show Handles to represent Line Lenght settings      
                Bounds meshExtents = m_textMeshProScript.bounds;
                Vector3 lossyScale = m_transform.lossyScale;
             
                handlePoints[0] = m_transform.TransformPoint(new Vector3(meshExtents.min.x * lossyScale.x, meshExtents.min.y, 0));
                handlePoints[1] = m_transform.TransformPoint(new Vector3(meshExtents.min.x * lossyScale.x, meshExtents.max.y, 0));
                handlePoints[2] = handlePoints[1] + m_transform.TransformDirection(new Vector3(m_textMeshProScript.lineLength * lossyScale.x, 0, 0));
                handlePoints[3] = handlePoints[0] + m_transform.TransformDirection(new Vector3(m_textMeshProScript.lineLength * lossyScale.x, 0, 0));

                Handles.DrawSolidRectangleWithOutline(handlePoints, new Color32(0, 0, 0, 0), new Color32(255, 255, 0, 255));

                Vector3 old_right = (handlePoints[2] + handlePoints[3]) * 0.5f;
                Vector3 new_right = Handles.FreeMoveHandle(old_right, Quaternion.identity, HandleUtility.GetHandleSize(m_transform.position) * 0.05f, Vector3.zero, Handles.DotCap);
                
                if (old_right != new_right)
                {
                    float delta = new_right.x - old_right.x;                   
                    m_textMeshProScript.lineLength += delta / lossyScale.x;
                }
            }
            */

            /* New Experimental Code
            // Margin Frame & Handles      
            Vector3 rectPos = m_transform.position;
            Vector4 textRect = m_textMeshProScript.textRectangle;

            handlePoints[0] = rectPos + m_transform.TransformDirection(new Vector3(- textRect.x, - textRect.w, 0)); // BL
            handlePoints[1] = rectPos + m_transform.TransformDirection(new Vector3(- textRect.x, + textRect.y, 0)); // TL
            handlePoints[2] = rectPos + m_transform.TransformDirection(new Vector3(+ textRect.z, + textRect.y, 0)); // TR
            handlePoints[3] = rectPos + m_transform.TransformDirection(new Vector3(+ textRect.z, - textRect.w, 0));   // BR

            Handles.DrawSolidRectangleWithOutline(handlePoints, new Color32(255, 255, 255, 0), new Color32(255, 255, 0, 255));

            // Draw & process FreeMoveHandles

            // LEFT HANDLE
            Vector3 old_left = (handlePoints[0] + handlePoints[1]) * 0.5f;
            Vector3 new_left = Handles.FreeMoveHandle(old_left, Quaternion.identity, HandleUtility.GetHandleSize(rectPos) * 0.05f, Vector3.zero, Handles.DotCap);
            bool hasChanged = false;
            if (old_left != new_left)
            {
                float delta = old_left.x - new_left.x;
                textRect.x += delta;              
                //Debug.Log("Left Margin H0:" + handlePoints[0] + "  H1:" + handlePoints[1]);
                hasChanged = true;
            }

            // TOP HANDLE
            Vector3 old_top = (handlePoints[1] + handlePoints[2]) * 0.5f;
            Vector3 new_top = Handles.FreeMoveHandle(old_top, Quaternion.identity, HandleUtility.GetHandleSize(rectPos) * 0.05f, Vector3.zero, Handles.DotCap);
            if (old_top != new_top)
            {
                float delta = old_top.y - new_top.y;             
                textRect.y -= delta;
                //Debug.Log("Top Margin H1:" + handlePoints[1] + "  H2:" + handlePoints[2]);   
                hasChanged = true;
            }

            // RIGHT HANDLE
            Vector3 old_right = (handlePoints[2] + handlePoints[3]) * 0.5f;
            Vector3 new_right = Handles.FreeMoveHandle(old_right, Quaternion.identity, HandleUtility.GetHandleSize(rectPos) * 0.05f, Vector3.zero, Handles.DotCap);
            if (old_right != new_right)
            {
                float delta = old_right.x - new_right.x;
                textRect.z -= delta;               
                hasChanged = true;
                //Debug.Log("Right Margin H2:" + handlePoints[2] + "  H3:" + handlePoints[3]);
            }

            // BOTTOM HANDLE
            Vector3 old_bottom = (handlePoints[3] + handlePoints[0]) * 0.5f;
            Vector3 new_bottom = Handles.FreeMoveHandle(old_bottom, Quaternion.identity, HandleUtility.GetHandleSize(rectPos) * 0.05f, Vector3.zero, Handles.DotCap);
            if (old_bottom != new_bottom)
            {
                float delta = old_bottom.y - new_bottom.y;
                textRect.w += delta;              
                hasChanged = true;
                //Debug.Log("Bottom Margin H0:" + handlePoints[0] + "  H3:" + handlePoints[3]);
            }

            if (hasChanged)
            {
                m_textMeshProScript.textRectangle = textRect;
                //m_textMeshProScript.ForceMeshUpdate();
            }
            
        }
        */


        void DrawPropertySlider(string label, SerializedProperty property)
        {
            float old_LabelWidth = EditorGUIUtility.labelWidth;
            float old_FieldWidth = EditorGUIUtility.fieldWidth;

            Rect rect = EditorGUILayout.GetControlRect(false, 17);

            //EditorGUIUtility.labelWidth = m_labelWidth;

            GUIContent content = label == "" ? GUIContent.none : new GUIContent(label);
            EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, rect.height), property, 0.0f, 1.0f, content);

            EditorGUIUtility.labelWidth = old_LabelWidth;
            EditorGUIUtility.fieldWidth = old_FieldWidth;
        }


        private void DrawDimensionProperty(SerializedProperty property, string label)
        {
            float old_LabelWidth = EditorGUIUtility.labelWidth;
            float old_FieldWidth = EditorGUIUtility.fieldWidth;

            Rect rect = EditorGUILayout.GetControlRect(false, 18);
            Rect pos0 = new Rect(rect.x, rect.y + 2, rect.width, 18);

            float width = rect.width + 3;
            pos0.width = old_LabelWidth;
            GUI.Label(pos0, label);

            Rect rectangle = property.rectValue;

            float width_B = width - old_LabelWidth;
            float fieldWidth = width_B / 4;
            pos0.width = fieldWidth - 5;

            pos0.x = old_LabelWidth + 15;
            GUI.Label(pos0, "Width");

            pos0.x += fieldWidth;
            rectangle.width = EditorGUI.FloatField(pos0, GUIContent.none, rectangle.width);

            pos0.x += fieldWidth;
            GUI.Label(pos0, "Height");

            pos0.x += fieldWidth;
            rectangle.height = EditorGUI.FloatField(pos0, GUIContent.none, rectangle.height);

            property.rectValue = rectangle;
            EditorGUIUtility.labelWidth = old_LabelWidth;
            EditorGUIUtility.fieldWidth = old_FieldWidth;
        }


        private void DrawDoubleEnumPopup(SerializedProperty property1, SerializedProperty property2, string label)
        {
            float old_LabelWidth = EditorGUIUtility.labelWidth;
            float old_FieldWidth = EditorGUIUtility.fieldWidth;

            Rect rect = EditorGUILayout.GetControlRect(false, 17);
            Rect pos0 = new Rect(rect.x, rect.y, EditorGUIUtility.labelWidth, rect.height);
            EditorGUI.PrefixLabel(pos0, new GUIContent(label));

            pos0.x += pos0.width;
            pos0.width = (rect.width - pos0.x) / 2 + 5;
            EditorGUI.PropertyField(pos0, property1, GUIContent.none);
            pos0.x += pos0.width + 5;
            EditorGUI.PropertyField(pos0, property2, GUIContent.none);

            EditorGUIUtility.labelWidth = old_LabelWidth;
            EditorGUIUtility.fieldWidth = old_FieldWidth;
        }


        void DrawPropertyBlock(string[] labels, SerializedProperty[] properties)
        {
            float old_LabelWidth = EditorGUIUtility.labelWidth;
            float old_FieldWidth = EditorGUIUtility.fieldWidth;
       
            Rect rect = EditorGUILayout.GetControlRect(false, 17);
            GUI.Label(new Rect(rect.x, rect.y, old_LabelWidth, rect.height), labels[0]);

            rect.x = old_LabelWidth + 15;
            rect.width = (rect.width + 20 - rect.x) / labels.Length;

            for (int i = 0; i < labels.Length; i++)
            {
                if (i == 0)
                {
                    EditorGUIUtility.labelWidth = 20;
                    GUI.enabled = properties[i] == fontSize_prop && autoSizing_prop.boolValue ? GUI.enabled = false : GUI.enabled = true; 
                    EditorGUI.PropertyField(new Rect(rect.x - 20, rect.y, 80, rect.height), properties[i], new GUIContent("  "));
                    rect.x += rect.width;
                    GUI.enabled = true;
                }
                else
                {
                    EditorGUIUtility.labelWidth = GUI.skin.textArea.CalcSize(new GUIContent(labels[i])).x;                                                              
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 5, rect.height), properties[i], new GUIContent(labels[i]));
                    rect.x += rect.width;  
                }          
                            
            }
           
            EditorGUIUtility.labelWidth = old_LabelWidth;
            EditorGUIUtility.fieldWidth = old_FieldWidth;
        }



        // Special Handling of Undo / Redo Events.
        private void OnUndoRedo()
        {
            int undoEventID = Undo.GetCurrentGroup();
            int LastUndoEventID = m_eventID;

            if (undoEventID != LastUndoEventID)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    //Debug.Log("Undo & Redo Performed detected in Editor Panel. Event ID:" + Undo.GetCurrentGroup());
                    TMPro_EventManager.ON_TEXTMESHPRO_PROPERTY_CHANGED(true, targets[i] as TextMeshPro);
                    m_eventID = undoEventID;
                }
            }
        }

        /*
        private UndoPropertyModification[] OnUndoRedoEvent(UndoPropertyModification[] modifications)
        {
            int eventID = Undo.GetCurrentGroup();
            PropertyModification modifiedProp = modifications[0].propertyModification;      
            System.Type targetType = modifiedProp.target.GetType();
              
            if (targetType == typeof(Material))
            {
                //Debug.Log("Undo / Redo Event Registered in Editor Panel on Target: " + targetObject);
           
                //TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, targetObject as Material);
                //EditorUtility.SetDirty(targetObject);        
            }
  
            //string propertyPath = modifications[0].propertyModification.propertyPath;  
            //if (propertyPath == "m_fontAsset")
            //{
                //int currentEvent = Undo.GetCurrentGroup();
                //Undo.RecordObject(Selection.activeGameObject.renderer.sharedMaterial, "Font Asset Changed");
                //Undo.CollapseUndoOperations(currentEvent);
                //Debug.Log("Undo / Redo Event: Font Asset changed. Event ID:" + Undo.GetCurrentGroup());
            
            //}

            //Debug.Log("Undo / Redo Event Registered in Editor Panel on Target: " + modifiedProp.propertyPath + "  Undo Event ID:" + eventID + "  Stored ID:" + TMPro_EditorUtility.UndoEventID);
            //TextMeshPro_EventManager.ON_TEXTMESHPRO_PROPERTY_CHANGED(true, target as TextMeshPro);
            return modifications;
        }
        */
    }
}