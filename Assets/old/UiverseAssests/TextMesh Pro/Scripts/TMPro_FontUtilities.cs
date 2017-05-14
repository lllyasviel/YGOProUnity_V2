// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace TMPro
{
    // Class which contains the font also known as face information.
    [Serializable]
    public class FaceInfo
    {
        public string Name;
        public float PointSize;
        public float Padding;
        public float LineHeight;
        public float Baseline;
        public float Ascender;
        public float Descender;
        public float CenterLine;
        public float SuperscriptOffset;
        public float SubscriptOffset;
        public float SubSize;
        public float Underline;
        public float UnderlineThickness;
        public float TabWidth;
        public int CharacterCount;
        public float AtlasWidth;
        public float AtlasHeight;
    }


    // Class which contains the Glyph Info / Character definition for each character contained in the font asset.
    [Serializable]
    public class GlyphInfo
    {
        public int id;
        public float x;
        public float y;
        public float width;
        public float height;
        public float xOffset;
        public float yOffset;
        public float xAdvance;
    }


    // Structure which holds the font creation settings
    [Serializable]
    public struct FontCreationSetting
    {
        public string fontSourcePath;
        public int fontSizingMode;
        public int fontSize;
        public int fontPadding;
        public int fontPackingMode;
        public int fontAtlasWidth;
        public int fontAtlasHeight;
        public int fontCharacterSet;
        public int fontStyle;
        public float fontStlyeModifier;
        public int fontRenderMode;
        public bool fontKerning;
    }


    // Class which contains pre-defined mesh information for each character. This is not used at this time.
    [Serializable]
    public class Glyph2D
    {
        // Vertices aligned with pivot located at Midline / Baseline.
        public Vector3 bottomLeft;
        public Vector3 topLeft;
        public Vector3 bottomRight;
        public Vector3 topRight;

        public Vector2 uv0;
        public Vector2 uv1;
        public Vector2 uv2;
        public Vector2 uv3;
    }


    public struct KerningPairKey
    {
        public int ascii_Left;
        public int ascii_Right;
        public int key;

        public KerningPairKey(int ascii_left, int ascii_right)
        {
            ascii_Left = ascii_left;
            ascii_Right = ascii_right;
            key = (ascii_right << 16) + ascii_left;
        }
    }


    [Serializable]
    public class KerningPair
    {
        public int AscII_Left;
        public int AscII_Right;
        public float XadvanceOffset;

        public KerningPair(int left, int right, float offset)
        {
            AscII_Left = left;
            AscII_Right = right;
            XadvanceOffset = offset;
        }
    }


    [Serializable]
    public class KerningTable
    {
        public List<KerningPair> kerningPairs;


        public KerningTable()
        {
            kerningPairs = new List<KerningPair>();
        }


        public void AddKerningPair()
        {
            if (kerningPairs.Count == 0)
            {
                kerningPairs.Add(new KerningPair(0, 0, 0));
            }
            else
            {
                int left = kerningPairs.Last().AscII_Left;
                int right = kerningPairs.Last().AscII_Right;
                float xoffset = kerningPairs.Last().XadvanceOffset;

                kerningPairs.Add(new KerningPair(left, right, xoffset));
            }

        }


        public int AddKerningPair(int left, int right, float offset)
        {
            int index = kerningPairs.FindIndex(item => item.AscII_Left == left && item.AscII_Right == right);

            if (index == -1)
            {
                kerningPairs.Add(new KerningPair(left, right, offset));
                return 0;
            }

            // Return -1 if Kerning Pair already exists.
            return -1;
        }


        public void RemoveKerningPair(int left, int right)
        {
            int index = kerningPairs.FindIndex(item => item.AscII_Left == left && item.AscII_Right == right);

            if (index != -1)
                kerningPairs.RemoveAt(index);
        }


        public void RemoveKerningPair(int index)
        {
            kerningPairs.RemoveAt(index);
        }


        public void SortKerningPairs()
        {
            // Sort List of Kerning Info
            if (kerningPairs.Count > 0)
                kerningPairs = kerningPairs.OrderBy(s => s.AscII_Left).ThenBy(s => s.AscII_Right).ToList();
        }
    }

    
    [Serializable]
    public class LineBreakingTable
    {
        //public List<char> leadingCharacters;
        //public List<char> followingCharacters;
        public Dictionary<int, char> leadingCharacters;
        public Dictionary<int, char> followingCharacters;

        public LineBreakingTable()
        {
            leadingCharacters = new Dictionary<int, char>();
            followingCharacters = new Dictionary<int, char>();
        }
    }
    
}