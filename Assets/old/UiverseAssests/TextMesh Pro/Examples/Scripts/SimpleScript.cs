using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{
    
    public class SimpleScript : MonoBehaviour
    {

        private TextMeshPro m_textMeshPro;
        private TextContainer m_textContainer;
        private TextMeshProFont m_FontAsset;

        private const string label = "The <#0050FF>count is: </color>{0:2}";
        private float m_frame;
        private char[] m_chars;


        void Start()
        {
            // Add new TextMesh Pro Component
            m_textMeshPro = gameObject.AddComponent<TextMeshPro>();
            //m_textContainer = GetComponent<TextContainer>();

            // Load the Font Asset to be used.
            m_FontAsset = Resources.Load("Fonts & Materials/ARIAL SDF", typeof(TextMeshProFont)) as TextMeshProFont;
            m_textMeshPro.font = m_FontAsset;

            // Assign Material to TextMesh Pro Component
            //m_textMeshPro.fontSharedMaterial = Resources.Load("Fonts & Materials/ARIAL SDF Bevel", typeof(Material)) as Material;
            //m_textMeshPro.fontSharedMaterial.EnableKeyword("BEVEL_ON");
            // Set various font settings.
            m_textMeshPro.fontSize = 48;

            //m_textMeshPro.anchor = AnchorPositions.Center;
            m_textMeshPro.alignment = TextAlignmentOptions.Center;
            m_textMeshPro.anchorDampening = true;
            //m_textMeshPro.enableAutoSizing = true;
            //textMeshPro.lineJustification = LineJustificationTypes.Center; 
            //textMeshPro.characterSpacing = 0.2f;
            //m_textMeshPro.enableCulling = true;
            //textMeshPro.enableWordWrapping = true; 
            //textMeshPro.lineLength = 60;

            //textMeshPro.fontColor = new Color32(255, 255, 255, 255);      

            /*
            for (int i = 0; i <= 1000000; i++)
            {
        
                m_textMeshPro.SetText(label, i % 1000);
       
                // Example to test the .char function.
                //m_chars = (i % 100).ToString().ToCharArray();
                //textMeshPro.chars = m_chars;
                         
                yield return new WaitForSeconds(0.1f);
            }
            */
        }


        void Update()
        {
            m_textMeshPro.SetText(label, m_frame % 1000);
            m_frame += 1 * Time.deltaTime;
        }

    }
}
