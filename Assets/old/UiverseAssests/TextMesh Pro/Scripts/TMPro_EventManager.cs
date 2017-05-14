using UnityEngine;
using System.Collections;



namespace TMPro
{
    public enum Compute_DistanceTransform_EventTypes { Processing, Completed };

    public static class TMPro_EventManager
    {
        //public delegate void PROGRESS_BAR_EVENT_HANDLER(object Sender, Progress_Bar_EventArgs e);
        //public static event PROGRESS_BAR_EVENT_HANDLER PROGRESS_BAR_EVENT;

        public delegate void COMPUTE_DT_EVENT_HANDLER(object Sender, Compute_DT_EventArgs e);
        public static event COMPUTE_DT_EVENT_HANDLER COMPUTE_DT_EVENT;


        // Event & Delegate used to notify TextMesh Pro objects that Material properties have been changed.
        public delegate void MaterialProperty_Event_Handler(bool isChanged, Material mat);
        public static event MaterialProperty_Event_Handler MATERIAL_PROPERTY_EVENT;

        public delegate void FontProperty_Event_Handler(bool isChanged, TextMeshProFont font);
        public static event FontProperty_Event_Handler FONT_PROPERTY_EVENT;

        public delegate void SpriteAssetProperty_Event_Handler(bool isChanged, Object obj);
        public static event SpriteAssetProperty_Event_Handler SPRITE_ASSET_PROPERTY_EVENT;

        public delegate void TextMeshProProperty_Event_Handler(bool isChanged, TextMeshPro obj);
        public static event TextMeshProProperty_Event_Handler TEXTMESHPRO_PROPERTY_EVENT;

        public delegate void DragAndDrop_Event_Handler(GameObject sender, Material currentMaterial, Material newMaterial);
        public static event DragAndDrop_Event_Handler DRAG_AND_DROP_MATERIAL_EVENT;

#if UNITY_4_6 || UNITY_5
        public delegate void TextMeshProUGUIProperty_Event_Handler(bool isChanged, TextMeshProUGUI obj);
        public static event TextMeshProUGUIProperty_Event_Handler TEXTMESHPRO_UGUI_PROPERTY_EVENT;

        public delegate void BaseMaterial_Event_Handler(Material mat);
        public static event BaseMaterial_Event_Handler BASE_MATERIAL_EVENT;
#endif

        public delegate void OnPreRenderObject_Event_Handler();
        public static event OnPreRenderObject_Event_Handler OnPreRenderObject_Event; 

        
        public static void ON_PRE_RENDER_OBJECT_CHANGED()
        {
            if (OnPreRenderObject_Event != null)
                OnPreRenderObject_Event();
        }


        public static void ON_MATERIAL_PROPERTY_CHANGED(bool isChanged, Material mat)
        {
            if (MATERIAL_PROPERTY_EVENT != null)
                MATERIAL_PROPERTY_EVENT(isChanged, mat);
        }

        public static void ON_FONT_PROPERTY_CHANGED(bool isChanged, TextMeshProFont font)
        {
            if (FONT_PROPERTY_EVENT != null)
                FONT_PROPERTY_EVENT(isChanged, font);
        }

        public static void ON_SPRITE_ASSET_PROPERTY_CHANGED(bool isChanged, Object obj)
        {
            if (SPRITE_ASSET_PROPERTY_EVENT != null)
                SPRITE_ASSET_PROPERTY_EVENT(isChanged, obj);
        }

        public static void ON_TEXTMESHPRO_PROPERTY_CHANGED(bool isChanged, TextMeshPro obj)
        {
            if (TEXTMESHPRO_PROPERTY_EVENT != null)
                TEXTMESHPRO_PROPERTY_EVENT(isChanged, obj);
        }

        public static void ON_DRAG_AND_DROP_MATERIAL_CHANGED(GameObject sender, Material currentMaterial, Material newMaterial)
        {
            if (DRAG_AND_DROP_MATERIAL_EVENT != null)
                DRAG_AND_DROP_MATERIAL_EVENT(sender, currentMaterial, newMaterial);
        }

#if UNITY_4_6 || UNITY_5
        public static void ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED(bool isChanged, TextMeshProUGUI obj)
        {
            if (TEXTMESHPRO_UGUI_PROPERTY_EVENT != null)
                TEXTMESHPRO_UGUI_PROPERTY_EVENT(isChanged, obj);
        }
      
        public static void ON_BASE_MATERIAL_CHANGED(Material mat)
        {
            if (BASE_MATERIAL_EVENT != null)
                BASE_MATERIAL_EVENT(mat);
        }
#endif

        //public static void ON_PROGRESSBAR_UPDATE(Progress_Bar_EventTypes event_type, Progress_Bar_EventArgs eventArgs)
        //{
        //    if (PROGRESS_BAR_EVENT != null)
        //        PROGRESS_BAR_EVENT(event_type, eventArgs);      
        //}

        public static void ON_COMPUTE_DT_EVENT(object Sender, Compute_DT_EventArgs e)
        {
            if (COMPUTE_DT_EVENT != null)
                COMPUTE_DT_EVENT(Sender, e);
        }
    }


    public class Compute_DT_EventArgs
    {
        public Compute_DistanceTransform_EventTypes EventType;
        public float ProgressPercentage;
        public Color[] Colors;


        public Compute_DT_EventArgs(Compute_DistanceTransform_EventTypes type, float progress)
        {
            EventType = type;
            ProgressPercentage = progress;
        }

        public Compute_DT_EventArgs(Compute_DistanceTransform_EventTypes type, Color[] colors)
        {
            EventType = type;
            Colors = colors;
        }

    }

}