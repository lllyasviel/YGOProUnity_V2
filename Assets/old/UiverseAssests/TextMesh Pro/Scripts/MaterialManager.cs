// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_4_6 || UNITY_5
using UnityEngine.UI;


namespace TMPro
{

    public static class MaterialManager
    {

        private static List<MaskingMaterial> m_materialList = new List<MaskingMaterial>();
        private static Mask[] m_maskComponents = new Mask[0];
        
     
        /// <summary>
        /// Create a Masking Material Instance for the given ID
        /// </summary>
        /// <param name="baseMaterial"></param>
        /// <param name="stencilID"></param>
        /// <returns></returns>
        public static Material GetStencilMaterial(Material baseMaterial, int stencilID)
        {
            // Check if Material supports masking
            if (!baseMaterial.HasProperty(ShaderUtilities.ID_StencilID))
            {
                Debug.LogWarning("Selected Shader does not support Stencil Masking. Please select the Distance Field or Mobile Distance Field Shader.");
                return baseMaterial;
            }

            Material stencilMaterial = null;

            // Check if baseMaterial already has a masking material associated with it.                  
            int index = m_materialList.FindIndex(item => item.baseMaterial == baseMaterial && item.stencilID == stencilID);

            if (index == -1)
            {
                //Create new Masking Material Instance for this Base Material 
                stencilMaterial = new Material(baseMaterial);
                stencilMaterial.hideFlags = HideFlags.HideAndDontSave;
                stencilMaterial.name += " Masking ID:" + stencilID;
                stencilMaterial.shaderKeywords = baseMaterial.shaderKeywords;

                // Set Stencil Properties
                ShaderUtilities.GetShaderPropertyIDs();
                stencilMaterial.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
                stencilMaterial.SetFloat(ShaderUtilities.ID_StencilComp, 3);

                MaskingMaterial temp = new MaskingMaterial();
                temp.baseMaterial = baseMaterial;
                temp.stencilMaterial = stencilMaterial;
                temp.stencilID = stencilID;
                temp.count = 1;

                m_materialList.Add(temp);

                //Debug.Log("Masking material for " + baseMaterial.name + " DOES NOT exists. Creating new " + maskingMaterial.name + " with ID " + maskingMaterial.GetInstanceID() + " which is used " + temp.count + " time(s).");           

            }
            else
            {
                stencilMaterial = m_materialList[index].stencilMaterial;
                m_materialList[index].count += 1;

                //Debug.Log("Masking material for " + baseMaterial.name + " already exists. Passing reference to " + maskingMaterial.name + " with ID " + maskingMaterial.GetInstanceID() + " which is used " + m_materialList[index].count + " time(s).");           
            }

            // Used for Debug
            ListMaterials();
            
            return stencilMaterial;         
        }


        // Function which returns the base material associated with a Masking Material
        public static Material GetBaseMaterial(Material stencilMaterial)
        {
            // Check if maskingMaterial already has a base material associated with it.
            int index = m_materialList.FindIndex(item => item.stencilMaterial == stencilMaterial);

            if (index == -1)
                return null;
            else
                return m_materialList[index].baseMaterial;

        }


        /// <summary>
        /// Function to set the Material Stencil ID
        /// </summary>
        /// <param name="material"></param>
        /// <param name="stencilID"></param>
        /// <returns></returns>
        public static Material SetStencil(Material material, int stencilID)
        {
            material.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
            
            if (stencilID == 0)
                material.SetFloat(ShaderUtilities.ID_StencilComp, 8);
            else
                material.SetFloat(ShaderUtilities.ID_StencilComp, 3);

            return material;
        }



        public static void AddMaskingMaterial(Material baseMaterial, Material stencilMaterial, int stencilID)
        {
            // Check if maskingMaterial already has a base material associated with it.
            int index = m_materialList.FindIndex(item => item.stencilMaterial == stencilMaterial);

            if (index == -1)
            {
                MaskingMaterial temp = new MaskingMaterial();
                temp.baseMaterial = baseMaterial;
                temp.stencilMaterial = stencilMaterial;
                temp.stencilID = stencilID;
                temp.count = 1;

                m_materialList.Add(temp);
            }
            else
            {
                stencilMaterial = m_materialList[index].stencilMaterial;
                m_materialList[index].count += 1;
            }
        }



        public static void RemoveStencilMaterial(Material stencilMaterial)
        {
            // Check if maskingMaterial is already on the list.
            int index = m_materialList.FindIndex(item => item.stencilMaterial == stencilMaterial);

            if (index != -1)
            {
                m_materialList.RemoveAt(index);
            }

            ListMaterials();
        }



        public static void ReleaseBaseMaterial(Material baseMaterial)
        {
            // Check if baseMaterial already has a masking material associated with it.
            int index = m_materialList.FindIndex(item => item.baseMaterial == baseMaterial);

            if (index == -1)
            {
                Debug.Log("No Masking Material exists for " + baseMaterial.name);
            }
            else
            {
                if (m_materialList[index].count > 1)
                {
                    m_materialList[index].count -= 1;
                    Debug.Log("Removed (1) reference to " + m_materialList[index].stencilMaterial.name + ". There are " + m_materialList[index].count + " references left.");
                }
                else
                {
                    Debug.Log("Removed last reference to " + m_materialList[index].stencilMaterial.name + " with ID " + m_materialList[index].stencilMaterial.GetInstanceID());
                    Object.DestroyImmediate(m_materialList[index].stencilMaterial);
                    m_materialList.RemoveAt(index);
                }
            }

            ListMaterials();
        }



        public static void ReleaseStencilMaterial(Material stencilMaterial)
        {
            // Check if baseMaterial already has a masking material associated with it.
            int index = m_materialList.FindIndex(item => item.stencilMaterial == stencilMaterial);

            if (index == -1)
            {
                //Debug.Log("No Masking Material exists for " + maskingMaterial.name);
            }
            else
            {
                if (m_materialList[index].count > 1)
                {
                    m_materialList[index].count -= 1;
                    //Debug.Log("Removed (1) reference to " + m_materialList[index].maskingMaterial.name + ". There are " + m_materialList[index].count + " references left.");
                }
                else
                {
                    //Debug.Log("Removed last reference to " + m_materialList[index].maskingMaterial.name + " with ID " + m_materialList[index].maskingMaterial.GetInstanceID());
                    Object.DestroyImmediate(m_materialList[index].stencilMaterial);
                    m_materialList.RemoveAt(index);
                }
            }

            ListMaterials();
        }


        public static void ClearMaterials()
        {
            if (m_materialList.Count() == 0)
            {
                Debug.Log("Material List has already been cleared.");
                return;
            }

            for (int i = 0; i < m_materialList.Count(); i++)
            {
                //Material baseMaterial = m_materialList[i].baseMaterial;
                Material stencilMaterial = m_materialList[i].stencilMaterial;

                Object.DestroyImmediate(stencilMaterial);
                m_materialList.RemoveAt(i);
            }
        }


        public static void ListMaterials()
        {
            return;

            /*
            if (m_materialList.Count() == 0)
            {
                Debug.Log("Material List is empty.");
                return;
            }

            //Debug.Log("List contains " + m_materialList.Count() + " items.");

            for (int i = 0; i < m_materialList.Count(); i++)
            {
                Material baseMaterial = m_materialList[i].baseMaterial;
                Material stencilMaterial = m_materialList[i].stencilMaterial;

                Debug.Log("Item #" + (i + 1) + " - Base Material is [" + baseMaterial.name + "] with ID " + baseMaterial.GetInstanceID() + " is associated with [" + (stencilMaterial != null ? stencilMaterial.name : "Null") + "] Stencil ID " + m_materialList[i].stencilID + " with ID " + (stencilMaterial != null ? stencilMaterial.GetInstanceID() : 0) + " and is referenced " + m_materialList[i].count + " time(s).");
            }
            */
        }



        public static int GetStencilID(GameObject obj)
        {
            int count = 0;
          
            m_maskComponents = obj.GetComponentsInParent<Mask>();
            for (int i = 0; i < m_maskComponents.Length; i++ )
            {
                if (m_maskComponents[i].MaskEnabled())
                    count += 1;
            }

            switch (count)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
                case 2:
                    return 3;
                case 3:
                    return 11;
            }

            return 0;
        }



        private class MaskingMaterial
        {
            public Material baseMaterial;
            public Material stencilMaterial;
            public int count;
            public int stencilID;
        }

    }

}

#endif
