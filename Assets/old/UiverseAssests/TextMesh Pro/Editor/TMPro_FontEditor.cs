// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;
using System.Collections;



namespace TMPro.EditorUtilities
{

    [CustomEditor(typeof(TextMeshProFont))]
    public class TMPro_FontEditor : Editor
    {
        private struct UI_PanelState
        {
            public static bool fontInfoPanel = true;
            public static bool glyphInfoPanel = false;
            public static bool kerningInfoPanel = true;
        }

        private int m_page = 0;


        private const string k_UndoRedo = "UndoRedoPerformed";

        private SerializedProperty font_atlas_prop;
        private SerializedProperty font_material_prop;

        private SerializedProperty font_normalStyle_prop;
        private SerializedProperty font_boldStyle_prop;

        private SerializedProperty font_italicStyle_prop;
        private SerializedProperty font_tabSize_prop;

        private SerializedProperty m_fontInfo_prop;
        private SerializedProperty m_glyphInfoList_prop;

        private SerializedProperty m_kerningInfo_prop;
        private KerningTable m_kerningTable;

        private SerializedProperty m_kerningPair_prop;


        private TextMeshProFont m_fontAsset;

        private bool isAssetDirty = false;

        private int errorCode;

        private System.DateTime timeStamp;

        private string[] uiStateLabel = new string[] { "<i>(Click to expand)</i>", "<i>(Click to collapse)</i>" };

        public void OnEnable()
        {
            font_atlas_prop = serializedObject.FindProperty("atlas");
            font_material_prop = serializedObject.FindProperty("material");
            font_normalStyle_prop = serializedObject.FindProperty("NormalStyle");
            font_boldStyle_prop = serializedObject.FindProperty("BoldStyle");
            font_italicStyle_prop = serializedObject.FindProperty("ItalicStyle");
            font_tabSize_prop = serializedObject.FindProperty("TabSize");

            m_fontInfo_prop = serializedObject.FindProperty("m_fontInfo");
            m_glyphInfoList_prop = serializedObject.FindProperty("m_glyphInfoList");
            m_kerningInfo_prop = serializedObject.FindProperty("m_kerningInfo");
            m_kerningPair_prop = serializedObject.FindProperty("m_kerningPair");

            //m_isGlyphInfoListExpanded_prop = serializedObject.FindProperty("isGlyphInfoListExpanded");
            //m_isKerningTableExpanded_prop = serializedObject.FindProperty("isKerningTableExpanded");

            m_fontAsset = target as TextMeshProFont;
            m_kerningTable = m_fontAsset.kerningInfo;

            // Get the UI Skin and Styles for the various Editors
            TMP_UIStyleManager.GetUIStyles();
        }

        public override void OnInspectorGUI()
        {

            //Debug.Log("OnInspectorGUI Called.");
            Event evt = Event.current;

            serializedObject.Update();

            GUILayout.Label("<b>TextMesh Pro! Font Asset</b>", TMP_UIStyleManager.Section_Label);

            // TextMeshPro Font Info Panel
            GUILayout.Label("Face Info", TMP_UIStyleManager.Section_Label);
            EditorGUI.indentLevel = 1;

            GUI.enabled = false; // Lock UI

            EditorGUIUtility.labelWidth = 135;
            //EditorGUIUtility.fieldWidth = 80;

            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Name"), new GUIContent("Font Source"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("PointSize"));

            GUI.enabled = true;
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("LineHeight"));

            //GUI.enabled = false;
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Baseline"));
            
            GUI.enabled = true;
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Ascender"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Descender"));        
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Underline"));
            //EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("UnderlineThickness"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("SuperscriptOffset"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("SubscriptOffset"));
            
            SerializedProperty subSize_prop = m_fontInfo_prop.FindPropertyRelative("SubSize");
            EditorGUILayout.PropertyField(subSize_prop);
            subSize_prop.floatValue = Mathf.Clamp(subSize_prop.floatValue, 0.25f, 1f);
            

            GUI.enabled = false;
            //EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("Padding"));

            //GUILayout.Label("Atlas Size");
            EditorGUI.indentLevel = 1;
            GUILayout.Space(18);
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("AtlasWidth"), new GUIContent("Width"));
            EditorGUILayout.PropertyField(m_fontInfo_prop.FindPropertyRelative("AtlasHeight"), new GUIContent("Height"));

            GUI.enabled = true;
            EditorGUI.indentLevel = 0;
            GUILayout.Space(20);
            GUILayout.Label("Font Sub-Assets", TMP_UIStyleManager.Section_Label);
                  
            GUI.enabled = false;
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(font_atlas_prop, new GUIContent("Font Atlas:"));
            EditorGUILayout.PropertyField(font_material_prop, new GUIContent("Font Material:"));

            GUI.enabled = true;

            // Font SETTINGS
            GUILayout.Space(10);
            GUILayout.Label("Face Style", TMP_UIStyleManager.Section_Label);          

            string evt_cmd = Event.current.commandName; // Get Current Event CommandName to check for Undo Events

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(font_normalStyle_prop, new GUIContent("Normal weight"));
            font_normalStyle_prop.floatValue = Mathf.Clamp(font_normalStyle_prop.floatValue, -3.0f, 3.0f);
            if (GUI.changed || evt_cmd == k_UndoRedo)
            {
                GUI.changed = false;
                Material mat = font_material_prop.objectReferenceValue as Material;
                mat.SetFloat("_WeightNormal", font_normalStyle_prop.floatValue);
            }

            //Rect rect = EditorGUILayout.GetControlRect();
            EditorGUILayout.PropertyField(font_boldStyle_prop, new GUIContent("Bold weight"));
            font_boldStyle_prop.floatValue = Mathf.Clamp(font_boldStyle_prop.floatValue, -3.0f, 3.0f);
            if (GUI.changed || evt_cmd == k_UndoRedo)
            {
                GUI.changed = false;
                Material mat = font_material_prop.objectReferenceValue as Material;
                mat.SetFloat("_WeightBold", font_boldStyle_prop.floatValue);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(font_italicStyle_prop, new GUIContent("Italic Style: "));
            font_italicStyle_prop.intValue = Mathf.Clamp(font_italicStyle_prop.intValue, 15, 60);

            EditorGUILayout.PropertyField(font_tabSize_prop, new GUIContent("Tab Multiple: "));

            EditorGUILayout.EndHorizontal();


            GUILayout.Space(10);
            EditorGUI.indentLevel = 0;
            if (GUILayout.Button("Glyph Info            \t\t\t" + (UI_PanelState.glyphInfoPanel ? uiStateLabel[1] : uiStateLabel[0]), TMP_UIStyleManager.Section_Label))
                UI_PanelState.glyphInfoPanel = !UI_PanelState.glyphInfoPanel;


            if (UI_PanelState.glyphInfoPanel)
            {              
                //Rect lastRect = GUILayoutUtility.GetLastRect();

                int arraySize = m_glyphInfoList_prop.arraySize;
                int itemsPerPage = 15;       


                if (arraySize > 0)
                {
                    // Display each GlyphInfo entry using the GlyphInfo property drawer.
                    for (int i = itemsPerPage * m_page; i < arraySize && i < itemsPerPage * (m_page + 1); i++)
                    {
                        SerializedProperty glyphInfo = m_glyphInfoList_prop.GetArrayElementAtIndex(i);

                        EditorGUILayout.BeginVertical(TMP_UIStyleManager.Group_Label);

                        EditorGUILayout.PropertyField(glyphInfo);

                        EditorGUILayout.EndVertical();
                    }
                }

                Rect pagePos = EditorGUILayout.GetControlRect(false, 20);
                pagePos.width /= 2;

                int shiftMultiplier = evt.shift ? 10 : 1;

                if (m_page > 0) GUI.enabled = true;
                else GUI.enabled = false;

                if (GUI.Button(pagePos, "Previous Page"))
                    m_page -= 1 * shiftMultiplier;

                pagePos.x += pagePos.width;
                if (itemsPerPage * (m_page + 1) < arraySize) GUI.enabled = true;
                else GUI.enabled = false;

                if (GUI.Button(pagePos, "Next Page"))
                    m_page += 1 * shiftMultiplier;

                m_page = Mathf.Clamp(m_page, 0, arraySize / itemsPerPage);           
            }


            // KERNING TABLE PANEL                     

            if (GUILayout.Button("Kerning Table Info\t\t\t" + (UI_PanelState.kerningInfoPanel ? uiStateLabel[1] : uiStateLabel[0]), TMP_UIStyleManager.Section_Label))
                UI_PanelState.kerningInfoPanel = !UI_PanelState.kerningInfoPanel;


            if (UI_PanelState.kerningInfoPanel)
            {
                
                Rect pos;

                SerializedProperty kerningPairs_prop = m_kerningInfo_prop.FindPropertyRelative("kerningPairs");
                int pairCount = kerningPairs_prop.arraySize;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Left Char", TMP_UIStyleManager.TMP_GUISkin.label);
                GUILayout.Label("Right Char", TMP_UIStyleManager.TMP_GUISkin.label);
                GUILayout.Label("Offset Value", TMP_UIStyleManager.TMP_GUISkin.label);
                GUILayout.Label(GUIContent.none, GUILayout.Width(20));
                EditorGUILayout.EndHorizontal();

                GUILayout.BeginVertical(TMP_UIStyleManager.TMP_GUISkin.label);

                for (int i = 0; i < pairCount; i++)
                {
                    SerializedProperty kerningPair_prop = kerningPairs_prop.GetArrayElementAtIndex(i);

                    pos = EditorGUILayout.BeginHorizontal();

                    EditorGUI.PropertyField(new Rect(pos.x, pos.y, pos.width - 20f, pos.height), kerningPair_prop, GUIContent.none);

                    // Button to Delete Kerning Pair                     
                    if (GUILayout.Button("-", GUILayout.ExpandWidth(false)))
                    {                       
                        m_kerningTable.RemoveKerningPair(i);
                        m_fontAsset.ReadFontDefinition(); // Reload Font Definition.  
                        serializedObject.Update(); // Get an updated version of the SerializedObject.
                        isAssetDirty = true;
                        break;
                    }

                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();

                GUILayout.Space(10);


                // Add New Kerning Pair Section
                GUILayout.BeginVertical(TMP_UIStyleManager.SquareAreaBox85G);

                pos = EditorGUILayout.BeginHorizontal();

                // Draw Empty Kerning Pair 
                EditorGUI.PropertyField(new Rect(pos.x, pos.y, pos.width - 20f, pos.height), m_kerningPair_prop);
                GUILayout.Label(GUIContent.none, GUILayout.Height(19));

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);

                if (GUILayout.Button("Add New Kerning Pair"))
                {
                    int asci_left = m_kerningPair_prop.FindPropertyRelative("AscII_Left").intValue;
                    int asci_right = m_kerningPair_prop.FindPropertyRelative("AscII_Right").intValue;
                    float xOffset = m_kerningPair_prop.FindPropertyRelative("XadvanceOffset").floatValue;

                    errorCode = m_kerningTable.AddKerningPair(asci_left, asci_right, xOffset);

                    // Sort Kerning Pairs & Reload Font Asset if new kerpair was added.
                    if (errorCode != -1)
                    {
                        m_kerningTable.SortKerningPairs();
                        m_fontAsset.ReadFontDefinition(); // Reload Font Definition.        
                        serializedObject.Update(); // Get an updated version of the SerializedObject.
                        isAssetDirty = true;
                    }
                    else
                    {
                        timeStamp = System.DateTime.Now.AddSeconds(5);
                    }
                }

                if (errorCode == -1)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Kerning Pair already <color=#ffff00>exists!</color>", TMP_UIStyleManager.TMP_GUISkin.label);
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();

                    if (System.DateTime.Now > timeStamp)
                        errorCode = 0;
                }

                GUILayout.EndVertical();
            }


            if (serializedObject.ApplyModifiedProperties() || evt_cmd == k_UndoRedo || isAssetDirty)
            {
                //Debug.Log("Serialized properties have changed.");
                TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, m_fontAsset);

                isAssetDirty = false;
                EditorUtility.SetDirty(target);
                //TMPro_EditorUtility.RepaintAll(); // Consider SetDirty
            }

        }
    }
}