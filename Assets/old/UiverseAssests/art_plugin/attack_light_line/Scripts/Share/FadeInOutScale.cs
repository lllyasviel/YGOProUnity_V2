using System.Text;
using UnityEngine;
using System.Collections;

public class FadeInOutScale : MonoBehaviour
{
  public FadeInOutStatus FadeInOutStatus = FadeInOutStatus.In;
  public float Speed = 1;
  public float MaxScale = 2;

  private Vector3 oldScale;
  private float time, oldSin;
  private bool updateTime = true, canUpdate = true;
  private Transform t;
  private bool isInitialized;

  private void Start()
  {
    t = transform;
    oldScale = t.localScale;
    isInitialized = true;
  }

  void InitDefaultVariables()
  {
    t.localScale = Vector3.zero;
    time = 0;
    oldSin = 0;
    canUpdate = true;
    updateTime = true;
  }

  void OnEnable()
  {
    if(isInitialized) InitDefaultVariables();
  }

  private void Update()
  {
    if (!canUpdate)
      return;

    if (updateTime)
    {
      time = Time.time;
      updateTime = false;
    }
    var sin = Mathf.Sin((Time.time - time) / Speed);
    var scale = sin * MaxScale;

    if (FadeInOutStatus==FadeInOutStatus.In) {
      t.localScale = new Vector3(oldScale.x * scale, oldScale.y * scale, oldScale.z * scale);
    }
    if (FadeInOutStatus==FadeInOutStatus.Out) {
      t.localScale = new Vector3(
        MaxScale * oldScale.x - oldScale.x * scale, 
        MaxScale * oldScale.y - oldScale.y * scale, 
        MaxScale * oldScale.z - oldScale.z * scale);
    }
    if (oldSin > sin)
        canUpdate = false;
      oldSin = sin;
  }
}