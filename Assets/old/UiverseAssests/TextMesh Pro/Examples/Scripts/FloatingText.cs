using UnityEngine;
using System.Collections;


namespace TMPro.Examples
{
    
    public class FloatingText : MonoBehaviour
    {
        private GameObject m_floatingText;
        private TextMeshPro m_textMeshPro;
        private TextMesh m_textMesh;
        private NavMeshAgent m_navAgent;

        private Transform m_transform;
        private Transform m_floatingText_Transform;
        private Transform m_cameraTransform;

        Vector3 lastPOS = Vector3.zero;
        Quaternion lastRotation = Quaternion.identity;

        public int SpawnType;

        void Awake()
        {
            m_transform = transform;
            m_navAgent = GetComponent<NavMeshAgent>();
            m_floatingText = new GameObject(m_transform.name + " floating text");

            m_floatingText_Transform = m_floatingText.transform;
            m_floatingText_Transform.parent = m_transform;

            m_floatingText_Transform.localPosition = new Vector3(0, 1f, 0);

            m_cameraTransform = Camera.main.transform;

            //m_parentScript = GetComponent<TextMeshSpawner>();

            //Debug.Log(m_parentScript.NumberOfNPC);
        }

        void Start()
        {
            if (SpawnType == 0)
            {
                //Debug.Log("Spawning TextMesh Pro Objects.");
                // TextMesh Pro Implementation
                m_textMeshPro = m_floatingText.AddComponent<TextMeshPro>();
                //m_textMeshPro.FontAsset = Resources.Load("Fonts & Materials/JOKERMAN SDF", typeof(TextMeshProFont)) as TextMeshProFont; // User should only provide a string to the resource.
                //m_textMeshPro.anchor = AnchorPositions.Bottom;
                m_textMeshPro.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
                m_textMeshPro.fontSize = 16;
                m_textMeshPro.text = string.Empty;

                StartCoroutine(DisplayTextMeshProFloatingText());
            }
            else
            {
                //Debug.Log("Spawning TextMesh Objects.");

                m_textMesh = m_floatingText.AddComponent<TextMesh>();
                m_textMesh.font = Resources.Load("Fonts/ARIAL", typeof(Font)) as Font; // User should only provide a string to the resource.     
                m_textMesh.GetComponent<Renderer>().sharedMaterial = m_textMesh.font.material;
                m_textMesh.color = new Color32((byte)Random.Range(0, 255), (byte)Random.Range(0, 255), (byte)Random.Range(0, 255), 255);
                m_textMesh.anchor = TextAnchor.LowerCenter;
                m_textMesh.fontSize = 16;

                StartCoroutine(DisplayTextMeshFloatingText());
            }

        }


        public IEnumerator DisplayTextMeshProFloatingText()
        {
            while (true)
            {
                m_textMeshPro.text = m_navAgent.remainingDistance.ToString("f2");

                // Align floating text perpendicular to Camera.
                if (!lastPOS.Compare(m_cameraTransform.position, 1000) || !lastRotation.Compare(m_cameraTransform.rotation, 1000))
                {
                    lastPOS = m_cameraTransform.position;
                    lastRotation = m_cameraTransform.rotation;
                    m_floatingText_Transform.rotation = lastRotation;
                    //Vector3 dir = m_transform.position - lastPOS;
                    //m_transform.forward = new Vector3(dir.x, 0, dir.z);       
                }

                yield return new WaitForEndOfFrame();
            }
        }


        public IEnumerator DisplayTextMeshFloatingText()
        {
            while (true)
            {
                m_textMesh.text = m_navAgent.remainingDistance.ToString("f2");

                // Align floating text perpendicular to Camera.
                if (!lastPOS.Compare(m_cameraTransform.position, 1000) || !lastRotation.Compare(m_cameraTransform.rotation, 1000))
                {
                    lastPOS = m_cameraTransform.position;
                    lastRotation = m_cameraTransform.rotation;
                    m_floatingText_Transform.rotation = lastRotation;
                    //Vector3 dir = m_transform.position - lastPOS;
                    //m_transform.forward = new Vector3(dir.x, 0, dir.z);            
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
