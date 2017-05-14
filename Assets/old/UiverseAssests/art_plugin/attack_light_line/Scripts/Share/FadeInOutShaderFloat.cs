using UnityEngine;
using System.Collections;

public class FadeInOutShaderFloat : MonoBehaviour
{
  public string PropertyName = "_CutOut";
  public float MaxFloat = 1;
  public float StartDelay = 0;
  public float FadeInSpeed = 0;
  public float FadeOutDelay = 0;
  public float FadeOutSpeed = 0;
  public bool FadeOutAfterCollision;
  public bool UseHideStatus;

  private Material mat;
  private float oldFloat, currentFloat, startFloat;
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
    if (effectSettings != null)
    effectSettings.CollisionEnter += prefabSettings_CollisionEnter;

    mat = GetComponent<Renderer>().material;
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
    oldFloat = 0;
    currentFloat = 0;

    if (isStartDelay)
      Invoke("SetupStartDelay", StartDelay);
    else
      canStart = true;
    if (!isIn) {
      if (!FadeOutAfterCollision)
        Invoke("SetupFadeOutDelay", FadeOutDelay);
      oldFloat = MaxFloat;
    }
  }

  private void prefabSettings_CollisionEnter(object sender, CollisionInfo e)
  {
    isCollisionEnter = true;
    if (!isIn && FadeOutAfterCollision) Invoke("SetupFadeOutDelay", FadeOutDelay);
  }


  void OnEnable()
  {
    if (isInitialized) InitDefaultVariables();
  }

  private void SetupStartDelay()
  {
    canStart = true;
  }

  private void SetupFadeOutDelay()
  {
    canStartFadeOut = true;
  }

  private void Update()
  {
    if (!canStart)
      return;

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
    if (effectSettings==null || (!UseHideStatus && !FadeOutAfterCollision))
      FadeOut();
    else if ((UseHideStatus && !effectSettings.IsVisible) || isCollisionEnter)
      FadeOut();
  }


  private void FadeIn()
  {
    currentFloat = oldFloat + Time.deltaTime / FadeInSpeed * MaxFloat;
    if (currentFloat >= MaxFloat)
    {
      fadeInComplited = true;
      if (!isOut) allComplited = true;
      currentFloat = MaxFloat;
      Invoke("SetupFadeOutDelay", FadeOutDelay);
    }

    mat.SetFloat(PropertyName, currentFloat);
    oldFloat = currentFloat;
  }

  private void FadeOut()
  {
    currentFloat = oldFloat - Time.deltaTime / FadeOutSpeed * MaxFloat;
    if (currentFloat <= 0)
    {
      currentFloat = 0;
      fadeOutComplited = true;
      allComplited = true;
    }

    mat.SetFloat(PropertyName, currentFloat);
    oldFloat = currentFloat;
  }

  #endregion
}