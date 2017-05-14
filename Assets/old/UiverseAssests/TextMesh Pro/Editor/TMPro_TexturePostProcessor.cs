using UnityEngine;
using UnityEditor;
using System.Collections;


namespace TMPro.EditorUtilities
{

    public class TMPro_TexturePostProcessor : AssetPostprocessor
    {

        void OnPostprocessTexture(Texture2D texture)
        {
            //var importer = assetImporter as TextureImporter;

            Texture2D tex = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D)) as Texture2D;

            // Send Event to InlineGraphics ...

            //Debug.Log(tex.GetInstanceID());
            if (tex != null)
                TMPro_EventManager.ON_SPRITE_ASSET_PROPERTY_CHANGED(true, tex);
        }

    }
}