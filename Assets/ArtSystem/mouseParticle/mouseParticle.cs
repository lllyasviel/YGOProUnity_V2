using UnityEngine;
using System.Collections;

public class mouseParticle : MonoBehaviour {
    public Camera camera;
    public EllipsoidParticleEmitter e1;
    public EllipsoidParticleEmitter e2;
    public Transform trans;
    // Use this for initialization
    void Start () {
        camera.depth = 99999;
    }
    float time = 0;
	// Update is called once per frame
	void Update () {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 10;
        trans.position = camera.ScreenToWorldPoint(screenPoint);

        if (Input.GetMouseButton(0))    
        {
            if (Input.GetMouseButtonDown(0))
            {
                time = 0;
            }
            time += Time.deltaTime;
            if (time > 0.49)
            {
                time = 0.49f;
            }
            e1.maxEmission = (0.5f - time) * 60f;
            e1.minEmission = (0.5f - time) * 60f;
            e2.maxEmission = e1.maxEmission / 3f;
            e2.minEmission = e1.minEmission / 3f;
            e1.emit = true;
            e2.emit = true;
        }
        else
        {
            e1.emit = false;
            e2.emit = false;
        }
    }
}
