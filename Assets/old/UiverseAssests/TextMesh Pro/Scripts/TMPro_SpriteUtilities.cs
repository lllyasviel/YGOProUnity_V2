// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace TMPro
{

    // Class which contains the Sprite Info for each sprite contained in the sprite asset.
    [Serializable]
    public class SpriteInfo
    {
        //public int fileID;
        public int id;
        public string name;
        public float x;
        public float y;
        public float width;
        public float height;
        public Vector2 pivot;
        public float xOffset; // Pivot X
        public float yOffset; // Pivot Y
        public float xAdvance;
        public float scale;

        public Sprite sprite;
    }
}