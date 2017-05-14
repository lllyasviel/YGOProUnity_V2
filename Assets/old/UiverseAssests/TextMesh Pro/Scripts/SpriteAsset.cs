using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;



namespace TMPro
{
    
    //[System.Serializable]
    public class SpriteAsset : ScriptableObject
    {

        // The texture which contains the sprites.      
        public Texture spriteSheet;

        // The material used to render these sprites.
        public Material material;

        // Array which contains all the sprites contained in the sprite sheet.           
        public List<SpriteInfo> spriteInfoList;


        private List<Sprite> m_sprites;
        
        // Temporary for testing
        //public bool updateSprite;


        void OnEnable()
        {

#if UNITY_EDITOR
           
            //if (m_sprites == null)
            //    LoadSprites();
#endif 
        
        }


        public void AddSprites(string path)
        {

        }


        void OnValidate()
        {
            //Debug.Log("OnValidate called on SpriteAsset.");
            
            //if (updateSprite)
            //{
                //UpdateSpriteArray();
            //    updateSprite = false;
            //}

            TMPro_EventManager.ON_SPRITE_ASSET_PROPERTY_CHANGED(true, this);

        }


#if UNITY_EDITOR
        public void LoadSprites()
        {
            if (m_sprites != null && m_sprites.Count > 0)
                return;

            Debug.Log("Loading Sprite List");
            
            string filePath = UnityEditor.AssetDatabase.GetAssetPath(spriteSheet);

            Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(filePath);

            m_sprites = new List<Sprite>();

            foreach (Object obj in objects)
            {
                if (obj.GetType() == typeof(Sprite))
                {
                    Sprite sprite = obj as Sprite;
                    Debug.Log("Sprite # " + m_sprites.Count + " Rect: " + sprite.rect);
                    m_sprites.Add(sprite);
                }
            }
        }



        public List<Sprite> GetSprites()
        {
            if (m_sprites != null && m_sprites.Count > 0)
                return m_sprites;

            //Debug.Log("Loading Sprite List");

            string filePath = UnityEditor.AssetDatabase.GetAssetPath(spriteSheet);

            Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(filePath);

            m_sprites = new List<Sprite>();

            foreach (Object obj in objects)
            {
                if (obj.GetType() == typeof(Sprite))
                {
                    Sprite sprite = obj as Sprite;
                    //Debug.Log("Sprite # " + m_sprites.Count + " Rect: " + sprite.rect);
                    m_sprites.Add(sprite);
                }
            }

            return m_sprites;
        }
#endif
      
    }
}
