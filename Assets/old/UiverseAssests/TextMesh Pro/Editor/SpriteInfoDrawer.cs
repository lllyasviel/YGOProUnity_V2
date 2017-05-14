// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
using UnityEditor;
using System.Collections;


namespace TMPro.EditorUtilities
{

    [CustomPropertyDrawer(typeof(SpriteInfo))]
    public class SpriteInfoDrawer : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //SerializedProperty prop_fileID = property.FindPropertyRelative("fileID");
            SerializedProperty prop_id = property.FindPropertyRelative("id");
            SerializedProperty prop_name = property.FindPropertyRelative("name");
            SerializedProperty prop_x = property.FindPropertyRelative("x");
            SerializedProperty prop_y = property.FindPropertyRelative("y");
            SerializedProperty prop_width = property.FindPropertyRelative("width");
            SerializedProperty prop_height = property.FindPropertyRelative("height");
            SerializedProperty prop_xOffset = property.FindPropertyRelative("xOffset");
            SerializedProperty prop_yOffset = property.FindPropertyRelative("yOffset");
            SerializedProperty prop_xAdvance = property.FindPropertyRelative("xAdvance");
            SerializedProperty prop_scale = property.FindPropertyRelative("scale");
            //SerializedProperty prop_sprite = property.FindPropertyRelative("sprite");

            // Get a reference to the sprite texture
            Texture tex = (property.serializedObject.targetObject as SpriteAsset).spriteSheet;

            Vector2 spriteSize = new Vector2(65, 65);
            if (prop_width.floatValue >= prop_height.floatValue)
            {
                spriteSize.y = prop_height.floatValue * spriteSize.x / prop_width.floatValue;
                position.y += (spriteSize.x - spriteSize.y) / 2;
            }
            else
            {
                spriteSize.x = prop_width.floatValue * spriteSize.y / prop_height.floatValue;
                position.x += (spriteSize.y - spriteSize.x) / 2;
            }
            
            // Compute the normalized texture coordinates
            Rect texCoords = new Rect(prop_x.floatValue / tex.width, prop_y.floatValue / tex.height, prop_width.floatValue / tex.width, prop_height.floatValue / tex.height);       
            GUI.DrawTextureWithTexCoords(new Rect(position.x + 5, position.y, spriteSize.x,  spriteSize.y), tex, texCoords, true);

            // We get Rect since a valid position may not be provided by the caller.
            Rect rect = GUILayoutUtility.GetRect(position.width, 49);
            rect.x += 70;
            rect.y -= 15;

           
            EditorGUIUtility.LookLikeControls(40, 45);

            EditorGUI.BeginChangeCheck();
            EditorGUI.LabelField(new Rect(rect.x + 5f, rect.y, 50f, 18), new GUIContent("ID: " + prop_id.intValue));
            EditorGUI.LabelField(new Rect(rect.x + 60f, rect.y, 200f, 18), new GUIContent("Name: " + prop_name.stringValue));
            //EditorGUI.LabelField(new Rect(rect.x + 280f, rect.y, 150f, 18), new GUIContent("File ID: " + prop_fileID.intValue));
            if (EditorGUI.EndChangeCheck())
            {
                //string filepath = AssetDatabase.GetAssetPath(prop_sprite.objectReferenceValue);
                //TextureImporter texImporter = AssetImporter.GetAtPath(filepath) as TextureImporter;
                //texImporter
                //AssetDatabase.RenameAsset()
            }

            EditorGUIUtility.labelWidth = 30f;
            EditorGUIUtility.fieldWidth = 10f;

            GUI.enabled = false;
            float width = (rect.width - 75f) / 4;
            EditorGUI.PropertyField(new Rect(rect.x + 5f + width * 0, rect.y + 22, width - 5f, 18), prop_x, new GUIContent("X:"));
            EditorGUI.PropertyField(new Rect(rect.x + 5f + width * 1, rect.y + 22, width - 5f, 18), prop_y, new GUIContent("Y:"));
            EditorGUI.PropertyField(new Rect(rect.x + 5f + width * 2, rect.y + 22, width - 5f, 18), prop_width, new GUIContent("W:"));
            EditorGUI.PropertyField(new Rect(rect.x + 5f + width * 3, rect.y + 22, width - 5f, 18), prop_height, new GUIContent("H:"));

            GUI.enabled = true;

            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(new Rect(rect.x + 5f + width * 0, rect.y + 44, width - 5f, 18), prop_xOffset, new GUIContent("OX:"));
            EditorGUI.PropertyField(new Rect(rect.x + 5f + width * 1, rect.y + 44, width - 5f, 18), prop_yOffset, new GUIContent("OY:"));            
            EditorGUI.PropertyField(new Rect(rect.x + 5f + width * 2, rect.y + 44, width - 5f, 18), prop_xAdvance, new GUIContent("Adv."));
            EditorGUI.PropertyField(new Rect(rect.x + 5f + width * 3, rect.y + 44, width - 5f, 18), prop_scale, new GUIContent("SF."));
            if (EditorGUI.EndChangeCheck())
            {
                //Sprite sprite = prop_sprite.objectReferenceValue as Sprite;
                //sprite = Sprite.Create(sprite.texture, sprite.rect, new Vector2(0.1f, 0.8f));
                //prop_sprite.objectReferenceValue = sprite;
                //Debug.Log(sprite.bounds);
            }
        }

        

    }
}
