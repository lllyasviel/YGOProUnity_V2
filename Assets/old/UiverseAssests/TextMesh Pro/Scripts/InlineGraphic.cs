// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if UNITY_4_6 || UNITY_5


using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;



namespace TMPro
{
    
    public class InlineGraphic : MaskableGraphic
    {


        public Texture texture;


        public override Texture mainTexture
        {
            get
            {
                if ((Object)this.texture == (Object)null)
                    return (Texture)Graphic.s_WhiteTexture;
                else
                    return this.texture;
            }
        }


        private InlineGraphicManager m_manager;
        //private CanvasRenderer m_canvasRenderer;
       
        //private List<UIVertex> m_uiVertices;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_manager = GetComponentInParent<InlineGraphicManager>();
            //m_canvasRenderer = GetComponent<CanvasRenderer>();

            if (m_manager != null && m_manager.spriteAsset != null)
                texture = m_manager.spriteAsset.spriteSheet;      
        }


        protected  void OnValidate()
        {
            //Debug.Log("Texture ID is " + this.texture.GetInstanceID());
        }

        
        public new void UpdateMaterial()
        {
            base.UpdateMaterial();
        }

        
        protected override void UpdateGeometry()
        {
            //Debug.Log("UpdateGeometry called.");
            //base.UpdateGeometry();
        }

        

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            base.OnPopulateMesh(vh);
        }
    }
}

#endif