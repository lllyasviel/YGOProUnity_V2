using UnityEngine;
using System.Collections;

public class WaterCollisionBehaviour : MonoBehaviour
{
  public GameObject WaterWave;
  public float scaleWave = 0.97f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

  void OnTriggerEnter(Collider myTrigger)
  {
    if(WaterWave!=null)
    {
      var go = Instantiate(WaterWave) as GameObject;
      var t = go.transform;
      t.parent = transform;
      var scale = transform.localScale.x * scaleWave;
      t.localScale = new Vector3(scale, scale, scale);
      t.localPosition = new Vector3(0, 0.001f, 0);
      t.LookAt(myTrigger.transform.position);
    }
  }
}
