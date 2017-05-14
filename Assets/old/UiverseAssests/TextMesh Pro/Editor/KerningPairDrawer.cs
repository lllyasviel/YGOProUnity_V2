// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;
using System.Collections;


namespace TMPro.EditorUtilities
{

    [CustomPropertyDrawer(typeof(KerningPair))]
    public class KerningPairDrawer : PropertyDrawer
    {
        private bool isEditingEnabled = false;
        private string char_left;
        private string char_right;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty prop_ascii_left = property.FindPropertyRelative("AscII_Left");
            SerializedProperty prop_ascii_right = property.FindPropertyRelative("AscII_Right");
            SerializedProperty prop_xOffset = property.FindPropertyRelative("XadvanceOffset");

            position.yMin += 4;
            position.yMax += 4;
            position.height -= 2;

            float width = position.width / 3;
            float padding = 5.0f;

            Rect ascii_FieldPos;
            Rect char_FieldPos;

            isEditingEnabled = label == GUIContent.none ? false : true;


            GUILayout.BeginHorizontal();

            GUI.enabled = isEditingEnabled;

            // Left Character
            char_FieldPos = new Rect(position.x, position.y, 25f, position.height);


            char_left = EditorGUI.TextArea(char_FieldPos, "" + (char)prop_ascii_left.intValue);
            if (GUI.changed && char_left != "")
            {
                GUI.changed = false;
                prop_ascii_left.intValue = char_left[0];
            }

            ascii_FieldPos = new Rect(position.x + char_FieldPos.width + padding, position.y, width - char_FieldPos.width - 10f, position.height);
            EditorGUI.PropertyField(ascii_FieldPos, prop_ascii_left, GUIContent.none);


            // Right Character 
            char_FieldPos = new Rect(position.x + (width * 1), position.y, 25f, position.height);
            char_right = EditorGUI.TextArea(char_FieldPos, "" + (char)prop_ascii_right.intValue);
            if (GUI.changed && char_right != "")
            {
                GUI.changed = false;
                prop_ascii_right.intValue = char_right[0];
            }

            ascii_FieldPos = new Rect(position.x + (width * 1) + char_FieldPos.width + padding, position.y, width - char_FieldPos.width - 10f, position.height);
            EditorGUI.PropertyField(ascii_FieldPos, prop_ascii_right, GUIContent.none);

            // Kerning xOffset
            GUI.enabled = true;
            ascii_FieldPos = new Rect(position.x + (width * 2), position.y, width, position.height);
            EditorGUIUtility.LookLikeControls(45, 50);
            EditorGUI.PropertyField(ascii_FieldPos, prop_xOffset, new GUIContent("Offset"));

            GUILayout.EndHorizontal();
        }


    }
}