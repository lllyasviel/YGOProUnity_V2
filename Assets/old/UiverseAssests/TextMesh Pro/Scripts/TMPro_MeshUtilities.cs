// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using System;
using System.Collections.Generic;


namespace TMPro
{
    
  
    public struct TMP_MeshInfo
    {           
        public Vector3[] vertices;
        public Vector2[] uv0s;
        public Vector2[] uv2s;
        public Color32[] vertexColors;
        public Vector3[] normals;
        public Vector4[] tangents;
#if UNITY_4_6 || UNITY_5
        public UIVertex[] uiVertices;
        public UIVertex[][] meshArrays;      
#endif
    }


    public enum TMP_CharacterType { Character, Sprite };


    // Structure containing information about each Character & releated Mesh info for the text object.   
    public struct TMP_CharacterInfo
    {   
        public TMP_CharacterType type;
        public char character;
        //public short wordNumber;
        public short lineNumber;
        //public short charNumber;
        public short pageNumber;
        
        public short index; // Index of character in the input text.
               
        public int meshIndex;       
        public short vertexIndex;
        //public TMP_VertexInfo vertexInfo;
        public TMP_Vertex vertex_TL;
        public TMP_Vertex vertex_BL;
        public TMP_Vertex vertex_TR;
        public TMP_Vertex vertex_BR;
        
        public Vector3 topLeft;
        public Vector3 bottomLeft;
        public Vector3 topRight;
        public Vector3 bottomRight;      
        public float topLine;      
        public float baseLine;
        public float bottomLine;
        
        public float xAdvance;     
        public float aspectRatio;
        public float padding;
        public float scale;
        public Color32 color;
        public FontStyles style;       
        public bool isVisible;
    }


    public struct TMP_Vertex
    {      
        public Vector3 position;
        public Vector2 uv;
        public Vector2 uv2;
        public Color32 color;

        public Vector3 normal;
        public Vector4 tangent;
    }
    
    


    //public struct TMP_VertexInfo
    //{      
    //    public TMP_Vertex topLeft;
    //    public TMP_Vertex bottomLeft;
    //    public TMP_Vertex topRight;
    //    public TMP_Vertex bottomRight;
    //}


    [Serializable]
    public struct VertexGradient
    {
        public Color topLeft;
        public Color topRight;
        public Color bottomLeft;
        public Color bottomRight;

        public VertexGradient (Color color)
        {
            this.topLeft = color;
            this.topRight = color;
            this.bottomLeft = color;
            this.bottomRight = color;
        }

        public VertexGradient(Color color0, Color color1, Color color2, Color color3)
        {
            this.topLeft = color0;
            this.topRight = color1;
            this.bottomLeft = color2;
            this.bottomRight = color3;
        }
    }


    [Serializable]
    public class TMP_TextInfo
    {          
        public int characterCount;
        public int spriteCount;
        public int spaceCount;
        public int wordCount;
        public int lineCount;
        public int pageCount;

        //public List<TMP_CharacterInfo> characterInfoList;
        //public List<TMP_WordInfo> wordInfoList;
        //public List<TMP_LineInfo> lineInfoList;
        //public List<TMP_PageInfo> pageInfoList;

        public TMP_CharacterInfo[] characterInfo;
        public List<TMP_WordInfo> wordInfo;
        public TMP_LineInfo[] lineInfo;
        public TMP_PageInfo[] pageInfo;
        public TMP_MeshInfo meshInfo;
      

        // Default Constructor
        public TMP_TextInfo()
        {
            characterInfo = new TMP_CharacterInfo[0];
            wordInfo = new List<TMP_WordInfo>(32);
            lineInfo = new TMP_LineInfo[16];
            pageInfo = new TMP_PageInfo[16];

            meshInfo = new TMP_MeshInfo();
#if UNITY_4_6 || UNITY_5
            meshInfo.meshArrays = new UIVertex[17][];
#endif
            
            //characterInfoList = new List<TMP_CharacterInfo>(128);
            //wordInfoList = new List<TMP_WordInfo>(64);
            //lineInfoList = new List<TMP_LineInfo>(32);
            //pageInfoList = new List<TMP_PageInfo>(16);
        }


        // Method to clear the information of the text object;
        public void Clear()
        {
            characterCount = 0;
            spaceCount = 0;
            wordCount = 0;
            lineCount = 0;
            pageCount = 0;
            spriteCount = 0;
                 
            Array.Clear(characterInfo, 0, characterInfo.Length);
            wordInfo.Clear();
            Array.Clear(lineInfo, 0, lineInfo.Length);
            Array.Clear(pageInfo, 0, pageInfo.Length);
            

            //characterInfoList.Clear();
            //wordInfoList.Clear();
            //lineInfoList.Clear();
            //pageInfoList.Clear();
        }
    }


    public struct TMP_PageInfo
    {
        public int firstCharacterIndex;
        public int lastCharacterIndex;
        public float ascender;
        public float baseLine;
        public float descender;
    }


    public struct TMP_WordInfo
    {
        public int firstCharacterIndex;
        public int lastCharacterIndex;
        public int characterCount;
        public float length;
        //public char[] word;
        //public string word;

        public string GetWord(TMP_CharacterInfo[] charInfo)
        {
            string word = string.Empty;
            
            for (int i = firstCharacterIndex; i < lastCharacterIndex + 1; i++)
            {
                word += charInfo[i].character;
            }

            return word;
        }
    }


    public struct TMP_SpriteInfo
    {
        public int spriteIndex; // Index of the sprite in the sprite atlas.
        public int characterIndex; // The characterInfo index which holds the key information about this sprite.
        public int vertexIndex;
    }


    public struct TMP_LineInfo
    {
        public int characterCount;
        public int spaceCount;
        public int wordCount;
        public int firstCharacterIndex;
        public int lastCharacterIndex;
        public int lastVisibleCharacterIndex;
        public float lineLength;
        public float lineHeight;
        public float ascender;
        public float descender;
        public float maxAdvance;

        public TextAlignmentOptions alignment;
        public Extents lineExtents;

    }


    //public struct SpriteInfo
    //{
    //    
    //}


    public struct Extents
    {
        public Vector2 min;
        public Vector2 max;

        public Extents(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public override string ToString()
        {
            string s = "Min (" + min.x.ToString("f2") + ", " + min.y.ToString("f2") + ")   Max (" + max.x.ToString("f2") + ", " + max.y.ToString("f2") + ")";           
            return s;
        }
    }


    [Serializable]
    public struct Mesh_Extents
    {
        public Vector2 min;
        public Vector2 max;
      
     
        public Mesh_Extents(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;           
        }

        public override string ToString()
        {
            string s = "Min (" + min.x.ToString("f2") + ", " + min.y.ToString("f2") + ")   Max (" + max.x.ToString("f2") + ", " + max.y.ToString("f2") + ")";
            //string s = "Center: (" + ")" + "  Extents: (" + ((max.x - min.x) / 2).ToString("f2") + "," + ((max.y - min.y) / 2).ToString("f2") + ").";
            return s;
        }
    }


    // Structure used for Word Wrapping which tracks the state of execution when the last space or carriage return character was encountered. 
    public struct WordWrapState
    {
        public int previous_WordBreak;     
        public int total_CharacterCount;
        public int visible_CharacterCount;
        public int visible_SpriteCount;
        public int firstVisibleCharacterIndex;
        public int lastVisibleCharIndex;
        public int lineNumber;
        public float maxAscender;
        public float maxDescender;
        public float xAdvance;
        public float preferredWidth;
        public float preferredHeight;
        public float maxFontScale;
        public float previousLineScale;
      
        public int wordCount;
        public FontStyles fontStyle;
        public float fontScale;
      
        public float currentFontSize;
        public float baselineOffset;
        public float lineOffset;

        public TMP_TextInfo textInfo;
        //public TMPro_CharacterInfo[] characterInfo;
        public TMP_LineInfo lineInfo;
        
        public Color32 vertexColor;
        public int colorStackIndex;
        public Extents meshExtents;
        //public Mesh_Extents lineExtents;    
    }


    // Structure used to track & restore state of previous line which is used to adjust linespacing.
    //public struct LineWrapState
    //{
    //    public int previous_LineBreak;
    //    public int total_CharacterCount;
    //    public int visible_CharacterCount;
    //    public int visible_SpriteCount;
    //    public float maxAscender;
    //    public float maxDescender;
    //    public float maxFontScale;

    //    //public float maxLineLength;
    //    public int wordCount;
    //    public FontStyles fontStyle;
    //    public float fontScale;
    //    public float xAdvance;
    //    public float currentFontSize;
    //    public float baselineOffset;
    //    public float lineOffset;

    //    public TMP_TextInfo textInfo;
    //    public TMP_LineInfo lineInfo;

    //    public Color32 vertexColor;
    //    public Extents meshExtents;
    //    //public Mesh_Extents lineExtents;    
    //}

}
