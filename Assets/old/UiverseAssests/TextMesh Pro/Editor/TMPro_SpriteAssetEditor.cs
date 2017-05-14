// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;
using System.Collections;
//using System.Collections.Generic;



namespace TMPro.EditorUtilities
{

    [CustomEditor(typeof(SpriteAsset))]
    public class TMPro_SpriteAssetEditor : Editor
    {
        private struct UI_PanelState
        {
            public static bool spriteAssetInfoPanel = true;
            public static bool spriteInfoPanel = false;          
        }

        private int m_page = 0;


        private const string k_UndoRedo = "UndoRedoPerformed";

        private SerializedProperty m_spriteAtlas_prop;
        private SerializedProperty m_spriteInfoList_prop;

        private bool isAssetDirty = false;
      
        private string[] uiStateLabel = new string[] { "<i>(Click to expand)</i>", "<i>(Click to collapse)</i>" };

        private float m_xOffset;
        private float m_yOffset;
        private float m_xAdvance;
        private float m_scale;


        public void OnEnable()
        {
            m_spriteAtlas_prop = serializedObject.FindProperty("spriteSheet");
            m_spriteInfoList_prop = serializedObject.FindProperty("spriteInfoList");


            // Get the UI Skin and Styles for the various Editors
            TMP_UIStyleManager.GetUIStyles();
        
        }


        public override void OnInspectorGUI()
        {

            //Debug.Log("OnInspectorGUI Called.");
            Event evt = Event.current;
            string evt_cmd = evt.commandName; // Get Current Event CommandName to check for Undo Events

            serializedObject.Update();

            EditorGUIUtility.labelWidth = 135;

            // HEADER
            GUILayout.Label("<b>TextMeshPro - Sprite Asset</b>", TMP_UIStyleManager.Section_Label);


            // TEXTMESHPRO SPRITE INFO PANEL
            GUILayout.Label("Sprite Info", TMP_UIStyleManager.Section_Label);
            EditorGUI.indentLevel = 1;

            GUI.enabled = false; // Lock UI
      
            EditorGUILayout.PropertyField(m_spriteAtlas_prop , new GUIContent("Sprite Atlas"));

            // SPRITE LIST                                      
            GUI.enabled = true; // Unlock UI 
            GUILayout.Space(10);
            EditorGUI.indentLevel = 0;


            if (GUILayout.Button("Sprite List\t\t" + (UI_PanelState.spriteInfoPanel ? uiStateLabel[1] : uiStateLabel[0]), TMP_UIStyleManager.Section_Label))
                UI_PanelState.spriteInfoPanel = !UI_PanelState.spriteInfoPanel;

            if (UI_PanelState.spriteInfoPanel)
            {
                int arraySize = m_spriteInfoList_prop.arraySize;
                int itemsPerPage = (Screen.height - 283) / 80;              

                if (arraySize > 0)
                {                  
                    // Display each SpriteInfo entry using the SpriteInfo property drawer.                                                                                      
                    for (int i = itemsPerPage * m_page; i < arraySize && i < itemsPerPage * (m_page + 1); i++)
                    {
                        EditorGUILayout.BeginVertical(TMP_UIStyleManager.Group_Label, GUILayout.Height(60));
                        
                        SerializedProperty spriteInfo = m_spriteInfoList_prop.GetArrayElementAtIndex(i);
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(spriteInfo);
                        EditorGUILayout.EndVertical();                                    
                    }              
                }

                int shiftMultiplier = evt.shift ? 10 : 1; // Page + Shift goes 10 page forward
                
                Rect pagePos = EditorGUILayout.GetControlRect(false, 20);
                pagePos.width /= 3;

                // Previous Page
                if (m_page > 0) GUI.enabled = true;
                else GUI.enabled = false;

                if (GUI.Button(pagePos, "Previous"))
                    m_page -= 1 * shiftMultiplier;

                // PAGE COUNTER
                GUI.enabled = true;
                pagePos.x += pagePos.width;
                int totalPages = (int)(arraySize / (float)itemsPerPage + 0.999f);
                GUI.Label(pagePos, "Page " + (m_page + 1) + " / " + totalPages, GUI.skin.button);
                
                // Next Page
                pagePos.x += pagePos.width;
                if (itemsPerPage * (m_page + 1) < arraySize) GUI.enabled = true;
                else GUI.enabled = false;
                           
                if (GUI.Button(pagePos, "Next"))
                    m_page += 1 * shiftMultiplier;

                // Clamp page range
                m_page = Mathf.Clamp(m_page, 0, arraySize / itemsPerPage);


                // Global Settings
                
                EditorGUIUtility.labelWidth = 40f;
                EditorGUIUtility.fieldWidth = 20f;

                GUI.enabled = true;
                EditorGUILayout.BeginVertical(TMP_UIStyleManager.Group_Label);
                Rect rect = EditorGUILayout.GetControlRect(false, 40);
               
                float width = (rect.width - 75f) / 4;
                EditorGUI.LabelField(rect, "Global Offsets & Scale", EditorStyles.boldLabel);
                
                
                rect.x += 70;
                bool old_ChangedState = GUI.changed;

                GUI.changed = false;
                m_xOffset = EditorGUI.FloatField(new Rect(rect.x + 5f + width * 0, rect.y + 20, width - 5f, 18), new GUIContent("OX:"), m_xOffset);
                if (GUI.changed) UpdateGlobalProperty("xOffset", m_xOffset);
                
                m_yOffset = EditorGUI.FloatField(new Rect(rect.x + 5f + width * 1, rect.y + 20, width - 5f, 18), new GUIContent("OY:"), m_yOffset);
                if (GUI.changed) UpdateGlobalProperty("yOffset", m_yOffset);
                
                m_xAdvance = EditorGUI.FloatField(new Rect(rect.x + 5f + width * 2, rect.y + 20, width - 5f, 18), new GUIContent("ADV."), m_xAdvance);
                if (GUI.changed) UpdateGlobalProperty("xAdvance", m_xAdvance);
                
                m_scale = EditorGUI.FloatField(new Rect(rect.x + 5f + width * 3, rect.y + 20, width - 5f, 18), new GUIContent("SF."), m_scale);
                if (GUI.changed) UpdateGlobalProperty("scale", m_scale);

                EditorGUILayout.EndVertical();

                GUI.changed = old_ChangedState;
                
            }

            //Rect rect = EditorGUILayout.GetControlRect(false, 20);
            
           

            
            if (serializedObject.ApplyModifiedProperties() || evt_cmd == k_UndoRedo || isAssetDirty)
            {
                //Debug.Log("Serialized properties have changed.");
                //TMPro_EventManager.ON_FONT_PROPERTY_CHANGED(true, m_fontAsset);

                isAssetDirty = false;
                EditorUtility.SetDirty(target);
                //TMPro_EditorUtility.RepaintAll(); // Consider SetDirty
            }

        }


        /// <summary>
        /// Method to update the properties of all sprites
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        void UpdateGlobalProperty(string property, float value)
        {
            int arraySize = m_spriteInfoList_prop.arraySize;

            for (int i = 0; i < arraySize; i++)
            {
                SerializedProperty spriteInfo = m_spriteInfoList_prop.GetArrayElementAtIndex(i);
                spriteInfo.FindPropertyRelative(property).floatValue = value;
            }

            GUI.changed = false;
        }
    }
}