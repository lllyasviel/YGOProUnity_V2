using UnityEngine;
using System.Collections;

public class placeSelector : MonoBehaviour {
    public Transform flag;
    public Collider col;
    public byte[] data;
    public bool selected = false;
    public Transform quad;

    public GameObject selectedGO;
    public GameObject selectableGO;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Program.pointedCollider == col)
        {
            if (flag.gameObject.activeInHierarchy == false)
            {
                flag.gameObject.SetActive(true);
            }
            Vector3 worldposition = Input.mousePosition;
            worldposition.z = 18;
            flag.transform.position = Camera.main.ScreenToWorldPoint(worldposition) - new Vector3(0, 0, 1);
            if (Program.InputGetMouseButtonDown_0)
            {
                Program.I().ocgcore.ES_placeSelected(this);
            }
        }
        else
        {
            if (flag.gameObject.activeInHierarchy == true)
            {
                flag.gameObject.SetActive(false);
            }
        }
        if (selectableGO!=null)
        {
            selectableGO.SetActive(!selected);

        }
        if (selectedGO != null)
        {
            selectedGO.SetActive(selected);
        }
    }
}
