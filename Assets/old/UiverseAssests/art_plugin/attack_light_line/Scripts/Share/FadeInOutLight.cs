using UnityEngine;

public class FadeInOutLight : MonoBehaviour
{
  public float StartDelay = 0;
  public float FadeInSpeed = 0;
  public float FadeOutDelay = 0;
  public float FadeOutSpeed = 0;
  public bool FadeOutAfterCollision;
  public bool UseHideStatus;

  private Light goLight;
  private float oldIntensity, currentIntensity, startIntensity;
  private bool canStart, canStartFadeOut, fadeInComplited, fadeOutComplited;
  private bool isCollisionEnter, allComplited;
  private bool isStartDelay, isIn, isOut;
  private EffectSettings effectSettings;
  private bool isInitialized;

  #region Non-public methods

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

  private void Start()
  {
    GetEffectSettingsComponent(transform);
    if (effectSettings!=null)
      effectSettings.CollisionEnter += prefabSettings_CollisionEnter;

    goLight = GetComponent<Light>();
    startIntensity = goLight.intensity;
    isStartDelay = StartDelay > 0.001f;
    isIn = FadeInSpeed > 0.001f;
    isOut = FadeOutSpeed > 0.001f;
    InitDefaultVariables();
    isInitialized = true;
  }

  private void InitDefaultVariables()
  {
    fadeInComplited = false;
    fadeOutComplited = false;
    allComplited = false;
    canStartFadeOut = false;
    isCollisionEnter = false;
    oldIntensity = 0;
    currentIntensity = 0;
    canStart = false;

    goLight.intensity = isIn ? 0 : startIntensity;

    if (isStartDelay)
      Invoke("SetupStartDelay", StartDelay);
    else
      canStart = true;
    if (!isIn) {
      if (!FadeOutAfterCollision)
        Invoke("SetupFadeOutDelay", FadeOutDelay);
      oldIntensity = startIntensity;
    }
  }

  void prefabSettings_CollisionEnter(object sender, CollisionInfo e)
  {
    isCollisionEnter = true;
    if (!isIn && FadeOutAfterCollision) Invoke("SetupFadeOutDelay", FadeOutDelay);
  }

  void OnEnable()
  {
    if(isInitialized) InitDefaultVariables();
  }

  void SetupStartDelay()
  {
    canStart = true;
  }

  void SetupFadeOutDelay()
  {
    canStartFadeOut = true;
  }

  private void Update()
  {
    if (!canStart) return;

    if (effectSettings != null && UseHideStatus && allComplited && effectSettings.IsVisible)
    {
      allComplited = false;
      fadeInComplited = false;
      fadeOutComplited = false;
      InitDefaultVariables();
    }

    if (isIn && !fadeInComplited)
    {
      if (effectSettings == null) FadeIn();
      else if ((UseHideStatus && effectSettings.IsVisible) || !UseHideStatus) FadeIn();
    }
    
    if (!isOut || fadeOutComplited || !canStartFadeOut)
      return;
    if (effectSettings == null || (!UseHideStatus && !FadeOutAfterCollision))
      FadeOut();
    else if ((UseHideStatus && !effectSettings.IsVisible) || isCollisionEnter)
      FadeOut();
  }


  private void FadeIn()
  {
    currentIntensity = oldIntensity + Time.deltaTime / FadeInSpeed * startIntensity;
    if (currentIntensity >= startIntensity)
    {
      fadeInComplited = true;
      if (!isOut) allComplited = true;
      currentIntensity = startIntensity;
      Invoke("SetupFadeOutDelay", FadeOutDelay);
    }
    goLight.intensity = currentIntensity;
    oldIntensity = currentIntensity;
  }

  private void FadeOut()
  {
    currentIntensity = oldIntensity - Time.deltaTime / FadeOutSpeed * startIntensity;
    if (currentIntensity <= 0)
    {
      currentIntensity = 0;
      fadeOutComplited = true;
      allComplited = true;
    }
    goLight.intensity = currentIntensity;
    oldIntensity = currentIntensity;
  }

  #endregion
}
