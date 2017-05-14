using UnityEngine;
using System.Collections;

public class ShieldCollisionBehaviour : MonoBehaviour
{
  public GameObject effect, explosion;
  public Vector3 FixInctancePosition, FixInctanceAngle;
  public float FixInctanceScalePercent = 100;
  public bool IsDefaultCollisionPoint;

  private Vector3 pos;
	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider collider)
    {
      pos = transform.position;
      Vector3 hitPoint = Vector3.zero;
      if (!IsDefaultCollisionPoint)
      {
        RaycastHit hit;
        Physics.Raycast(transform.position, (collider.transform.position - pos).normalized, out hit);
        hitPoint = hit.point;
      }
      if (effect!=null) {
        var part = effect.GetComponent<ParticleSystem>();
        if (part!=null) {
          part.startSize = transform.lossyScale.x;
        }
        else {
          effect.transform.localScale = transform.lossyScale;
        }
        var inst1 = Instantiate(effect) as GameObject;
        inst1.transform.parent = gameObject.transform;
        inst1.transform.localPosition = transform.localPosition + FixInctancePosition;
        if (IsDefaultCollisionPoint) inst1.transform.localRotation = new Quaternion();
        else
          inst1.transform.LookAt(hitPoint);
        inst1.transform.Rotate(FixInctanceAngle);
        inst1.transform.localScale = transform.localScale * FixInctanceScalePercent / 100f;
      }
      if (explosion != null)
        {
          var inst2 = Instantiate(explosion, hitPoint, new Quaternion()) as GameObject;
            inst2.transform.parent = transform;
        }
    }
}
