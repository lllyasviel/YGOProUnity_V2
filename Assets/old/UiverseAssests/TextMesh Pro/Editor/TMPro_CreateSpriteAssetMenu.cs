using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;




namespace TMPro.EditorUtilities
{

    public static class TMPro_CreateSpriteAssetMenu
    {

        [MenuItem("Assets/Create/TextMeshPro - Sprite Asset", false, 100)]
        public static void CreateTextMeshProObjectPerform()
        {
            Object target = Selection.activeObject;

            // Make sure the selection is a texture.
            if (target == null || target.GetType() != typeof(Texture2D))
                return;

            Texture2D sourceTex = target as Texture2D;

            // Get the path to the selected texture.       
            string filePathWithName = AssetDatabase.GetAssetPath(sourceTex);
            string fileNameWithExtension = Path.GetFileName(filePathWithName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePathWithName);
            string filePath = filePathWithName.Replace(fileNameWithExtension, "");
             
            // Check if Sprite Asset already exists                    
            SpriteAsset spriteAsset = AssetDatabase.LoadAssetAtPath(filePath + fileNameWithoutExtension + ".asset", typeof(SpriteAsset)) as SpriteAsset;
            bool isNewAsset = spriteAsset == null ? true : false;

            if (isNewAsset)
            {
                // Create new Sprite Asset using this texture
                spriteAsset = ScriptableObject.CreateInstance<SpriteAsset>();
                AssetDatabase.CreateAsset(spriteAsset, filePath + fileNameWithoutExtension + ".asset");

                // Assign new Sprite Sheet texture to the Sprite Asset.         
                spriteAsset.spriteSheet = sourceTex;


                spriteAsset.spriteInfoList = GetSpriteInfo(sourceTex);

                //spriteAsset.UpdateSpriteArray(sourceTex);         
                //spriteSheet.hideFlags = HideFlags.HideInHierarchy;
                //AssetDatabase.AddObjectToAsset(spriteSheet, spriteAsset);

                //spriteAsset.spriteSheetWidth = sourceTex.width;
                //spriteAsset.spriteSheetHeight = sourceTex.height;
            }
            else
            {
                spriteAsset.spriteInfoList = UpdateSpriteInfo(spriteAsset);
            }

            // Get the Sprites contained in the Sprite Sheet
            EditorUtility.SetDirty(spriteAsset);
            
            //spriteAsset.sprites = sprites;

            // Set source texture back to Not Readable.
            //texImporter.isReadable = false;


            AssetDatabase.SaveAssets();

            //AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(spriteAsset));  // Re-import font asset to get the new updated version.

            //AssetDatabase.Refresh();
        }


        private static List<SpriteInfo> GetSpriteInfo(Texture source)
        {
            //Debug.Log("Creating new Sprite Asset.");
            
            string filePath = UnityEditor.AssetDatabase.GetAssetPath(source);

            // Get all the Sprites sorted by Index
            Sprite[] sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(filePath).Select(x => x as Sprite).Where(x => x != null).OrderByDescending(x => x.rect.y).ThenBy(x => x.rect.x).ToArray();
            
            List<SpriteInfo> spriteInfoList = new List<SpriteInfo>();

            for (int i = 0; i < sprites.Length; i++)   
            {
                SpriteInfo spriteInfo = new SpriteInfo();
                Sprite sprite = sprites[i];

                //spriteInfo.fileID = UnityEditor.Unsupported.GetLocalIdentifierInFile(sprite.GetInstanceID());
                spriteInfo.id = i;
                spriteInfo.name = sprite.name;

                Rect spriteRect = sprite.rect;
                spriteInfo.x = spriteRect.x;
                spriteInfo.y = spriteRect.y;
                spriteInfo.width = spriteRect.width;
                spriteInfo.height = spriteRect.height;

                // Compute Sprite pivot position
                Vector2 pivot = new Vector2(0 - (sprite.bounds.min.x) / (sprite.bounds.extents.x * 2), 0 - (sprite.bounds.min.y) / (sprite.bounds.extents.y * 2));               
                spriteInfo.pivot = new Vector2(0 - pivot.x * spriteRect.width, spriteRect.height - pivot.y * spriteRect.height);

                //spriteInfo.sprite = sprite;

                // Properties the can be modified
                spriteInfo.xAdvance = spriteRect.width;
                spriteInfo.scale = 1.0f;
                spriteInfo.xOffset = 0;
                spriteInfo.yOffset = 0;

                spriteInfoList.Add(spriteInfo);

            }

            return spriteInfoList;
        }


        // Update exsisting SpriteInfo
        private static List<SpriteInfo> UpdateSpriteInfo(SpriteAsset spriteAsset)
        {
            //Debug.Log("Updating Sprite Asset.");
            
            string filePath = UnityEditor.AssetDatabase.GetAssetPath(spriteAsset.spriteSheet);

            // Get all the Sprites sorted Left to Right / Top to Bottom
            Sprite[] sprites = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(filePath).Select(x => x as Sprite).Where(x => x != null).OrderByDescending(x => x.rect.y).ThenBy(x => x.rect.x).ToArray();

            for (int i = 0; i < sprites.Length; i++)
            {
                Sprite sprite = sprites[i];

                // Check if sprite already exists in the SpriteInfoList
                int index = spriteAsset.spriteInfoList.FindIndex(item => item.name == sprite.name);
                //Debug.Log("")

                // Use existing SpriteInfo is it already exists
                SpriteInfo spriteInfo = index == -1 ? new SpriteInfo() : spriteAsset.spriteInfoList[index];

                Rect spriteRect = sprite.rect;
                spriteInfo.name = sprite.name;
                spriteInfo.x = spriteRect.x;
                spriteInfo.y = spriteRect.y;
                spriteInfo.width = spriteRect.width;
                spriteInfo.height = spriteRect.height;

                // Get Sprite Pivot
                Vector2 pivot = new Vector2(0 - (sprite.bounds.min.x) / (sprite.bounds.extents.x * 2), 0 - (sprite.bounds.min.y) / (sprite.bounds.extents.y * 2));

                // The position of the pivot influences the Offset position.                
                spriteInfo.pivot = new Vector2(0 - pivot.x * spriteRect.width, spriteRect.height - pivot.y * spriteRect.height);

                if (index == -1)
                {
                    // Find the next available index for this Sprite
                    int[] ids = spriteAsset.spriteInfoList.Select(item => item.id).ToArray();

                    int id = 0;
                    for (int j = 0; j < ids.Length; j++ )
                    {
                        if (ids[0] != 0) break;
 
                        if (j > 0 && (ids[j] - ids[j - 1]) > 1)
                        {
                            id = ids[j - 1] + 1;
                            break;
                        }

                        id = j + 1;
                    }

                    //spriteInfo.fileID = fileID;
                    spriteInfo.id = id;
                    spriteInfo.xAdvance = spriteRect.width;
                    spriteInfo.scale = 1.0f;

                    spriteInfo.xOffset = 0;
                    spriteInfo.yOffset = 0;

                    spriteAsset.spriteInfoList.Add(spriteInfo);
                }
                else
                {
                    spriteAsset.spriteInfoList[index] = spriteInfo;
                }
            }

            return spriteAsset.spriteInfoList;
        }

       
    }
}