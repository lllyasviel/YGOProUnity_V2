using System;
using UnityEngine;
using System.Collections;

public class CollisionActiveBehaviour : MonoBehaviour
{
  public bool IsReverse;
  public float TimeDelay = 0;
  public bool IsLookAt;

  private EffectSettings effectSettings;

	// Use this for initialization
	void Start ()
	{
	  GetEffectSettingsComponent(transform);
	  if (IsReverse) {
	    effectSettings.RegistreInactiveElement(gameObject, TimeDelay);
	    gameObject.SetActive(false);
	  }
	  else
	    effectSettings.RegistreActiveElement(gameObject, TimeDelay);
    if (IsLookAt) effectSettings.CollisionEnter += effectSettings_CollisionEnter;
	}

  void effectSettings_CollisionEnter(object sender, CollisionInfo e)
  {
    transform.LookAt(effectSettings.transform.position + e.Hit.normal);
  }

  private void GetEffectSettingsComponent(Transform tr)
  {
    var parent = tr.parent;
    if (parent != null)
    {
      effectSettings = parent.GetComponentInChildren<EffectSettings>();
      if (effectSettings == null)
        GetEffectSettingsComponent(parent.transform);
    }
  }
}
