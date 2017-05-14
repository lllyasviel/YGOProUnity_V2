using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

public class FadeInOutParticles : MonoBehaviour {

  private EffectSettings effectSettings;
  private ParticleSystem[] particles;
  private bool oldVisibleStat;

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
  
  void Start()
  {
    GetEffectSettingsComponent(transform);
    particles  = effectSettings.GetComponentsInChildren<ParticleSystem>();
    oldVisibleStat = effectSettings.IsVisible;
  }

  void Update()
  {
    if (effectSettings.IsVisible!=oldVisibleStat) {
      if (effectSettings.IsVisible)
        foreach (var particle in particles) {
          if (effectSettings.IsVisible) {
            particle.Play();
            particle.enableEmission = true;
          }
        }
      else
        foreach (var particle in particles) {
          if (!effectSettings.IsVisible) {
            particle.Stop();
            particle.enableEmission = false;
          }
        }
    }
    oldVisibleStat = effectSettings.IsVisible;
  }

}
