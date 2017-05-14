// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;

#if UNITY_4_6 || UNITY_5
using UnityEngine.UI;
using UnityEngine.EventSystems;
#endif

namespace TMPro.EditorUtilities
{
    public static class TMPro_CreateObjectMenu
    {
#if UNITY_4_6 || UNITY_5      
        [MenuItem("GameObject/3D Object/TextMeshPro Text", false, 30)]
#else
        [MenuItem("GameObject/Create Other/TextMeshPro Text", false, -1)]
#endif
        static void CreateTextMeshProObjectPerform(MenuCommand command)
        {
            GameObject go = new GameObject("TextMeshPro");
            TextMeshPro textMeshPro = go.AddComponent<TextMeshPro>();
            textMeshPro.text = "Sample text";
            textMeshPro.alignment = TextAlignmentOptions.TopLeft;

#if UNITY_4_6 || UNITY_5
            if (command.context != null)
                GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);
#else
            if (command.context != null)
                go.transform.parent =  (command.context as GameObject).transform;
#endif

        }

#if UNITY_4_6 || UNITY_5
        [MenuItem("GameObject/UI/TextMeshPro Text", false, 2003)]
        static void CreateTextMeshProGuiObjectPerform(MenuCommand command)
        {                   
                                                  
            // Check if there is a Canvas in the scene            
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                // Create new Canvas since none exists in the scene.
                canvas = new GameObject("Canvas").AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                // Add a Graphic Raycaster Component as well
                canvas.gameObject.AddComponent<GraphicRaycaster>();
            }


            // Create the TextMeshProUGUI Object
            GameObject go = new GameObject("TextMeshPro Text");
            RectTransform goRectTransform = go.AddComponent<RectTransform>();  
            
                      
            // Check if object is being create with left or right click
            if (command.context == null)
            {
                goRectTransform.sizeDelta = new Vector2(200f, 50f);
                GameObjectUtility.SetParentAndAlign(go, canvas.gameObject);

                TextMeshProUGUI textMeshPro = go.AddComponent<TextMeshProUGUI>();
                textMeshPro.text = "New Text";
                textMeshPro.alignment = TextAlignmentOptions.TopLeft;
            }
            else
            {
                GameObject parent = command.context as GameObject;
                
                if (parent.GetComponent<Button>() != null)
                {
                    goRectTransform.sizeDelta = Vector2.zero;
                    goRectTransform.anchorMin = Vector2.zero;
                    goRectTransform.anchorMax = Vector2.one;

                    GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);    

                    TextMeshProUGUI textMeshPro = go.AddComponent<TextMeshProUGUI>();
                    textMeshPro.text = "Button";
                    textMeshPro.fontSize = 24;
                    textMeshPro.alignment = TextAlignmentOptions.Center;
                }
                else
                {
                    goRectTransform.sizeDelta = new Vector2(200f, 50f);

                    GameObjectUtility.SetParentAndAlign(go, command.context as GameObject);    

                    TextMeshProUGUI textMeshPro = go.AddComponent<TextMeshProUGUI>();
                    textMeshPro.text = "New Text";
                    textMeshPro.alignment = TextAlignmentOptions.TopLeft;
                }
                                                     
            }

         
            // Check if an event system already exists in the scene
            if (!Object.FindObjectOfType<EventSystem>())
            {
                GameObject eventObject = new GameObject("EventSystem", typeof(EventSystem));
                eventObject.AddComponent<StandaloneInputModule>();
                eventObject.AddComponent<TouchInputModule>();
            }

        }        
#endif

    }
}
