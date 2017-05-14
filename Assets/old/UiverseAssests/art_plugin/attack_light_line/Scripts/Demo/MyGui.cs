using System;
using UnityEngine;
using System.Collections;
public class MyGui : MonoBehaviour
{
  public enum GuiStat { Ball, Bottom, Middle, Top}

  public bool UseGui = true;
  public int CurrentPrefabNomber = 0; 
  public float UpdateInterval = 0.5F;
  public Light DirLight;
  public GameObject Target;
  public GameObject TopPosition;
  public GameObject MiddlePosition;
  public GameObject BottomPosition;
  public GameObject Plane1;
  public GameObject Plane2;
  public Material[] PlaneMaterials;
  public GuiStat[] GuiStats;
  public float[] Times;
  public GameObject[] Prefabs;

  private float oldLightIntensity;
  private Color oldAmbientColor;
  private GameObject currentGo, defaultBall;
  private bool isDay, isHomingMove, isDefaultPlaneTexture;
  private int current;
  private Animator anim;
  private float prefabSpeed = 4;
  private EffectSettings effectSettings, defaultBallEffectSettings;
  private bool isReadyEffect, isReadyDefaulBall;
 
  private float accum = 0; // FPS accumulated over the interval
  private int frames = 0; // Frames drawn over the interval
  private float timeleft; // Left time for current interval
  private float fps;

  private GUIStyle guiStyleHeader = new GUIStyle();

  void Start()
  {
    oldAmbientColor = RenderSettings.ambientLight;
    oldLightIntensity = DirLight.intensity;

    anim = Target.GetComponent<Animator>();
    guiStyleHeader.fontSize = 14;
    guiStyleHeader.normal.textColor = new Color(1,1,1);
    var prefabSett = Prefabs[current].GetComponent<EffectSettings>();
    if (prefabSett != null) prefabSpeed = prefabSett.MoveSpeed;
    current = CurrentPrefabNomber;
    InstanceCurrent(GuiStats[CurrentPrefabNomber]);
  }

  private void InstanceEffect(Vector3 pos)
  {
    currentGo = Instantiate(Prefabs[current], pos, Prefabs[current].transform.rotation) as GameObject;
    effectSettings = currentGo.GetComponent<EffectSettings>();
    effectSettings.Target = Target;
    if (isHomingMove) effectSettings.IsHomingMove = isHomingMove;
    prefabSpeed = effectSettings.MoveSpeed;
    effectSettings.EffectDeactivated+=effectSettings_EffectDeactivated;
    currentGo.transform.parent = transform;
    //effectSettings.CollisionEnter += (n, e) => { Debug.Log(e.Hit.transform.name); };
  }

  private void InstanceDefaulBall()
  {
    defaultBall = Instantiate(Prefabs[1], transform.position, Prefabs[1].transform.rotation) as GameObject;
    defaultBallEffectSettings = defaultBall.GetComponent<EffectSettings>();
    defaultBallEffectSettings.Target = Target;
    defaultBallEffectSettings.EffectDeactivated += defaultBall_EffectDeactivated;
    defaultBall.transform.parent = transform;
  }

  void defaultBall_EffectDeactivated(object sender, EventArgs e)
  {
    defaultBall.transform.position = transform.position;
    isReadyDefaulBall = true;
  }

  void effectSettings_EffectDeactivated(object sender, EventArgs e)
  {
    currentGo.transform.position = GetInstancePosition(GuiStats[current]);
    isReadyEffect = true;
  }

  private void OnGUI()
  {
    if (!UseGui)
      return;
    if (GUI.Button(new Rect(10, 15, 105, 30), "Previous Effect")) {
      ChangeCurrent(-1);
    }
    if (GUI.Button(new Rect(130, 15, 105, 30), "Next Effect"))
    {
      ChangeCurrent(+1);
    }
    if(Prefabs[current]!=null)GUI.Label(new Rect(300, 15, 100, 20), "Prefab name is \"" + Prefabs[current].name + "\"  \r\nHold any mouse button that would move the camera", guiStyleHeader);
    if (GUI.Button(new Rect(10, 60, 225, 30), "Day/Night")) {
      DirLight.intensity = !isDay ? 0.00f : oldLightIntensity;
      RenderSettings.ambientLight = !isDay ? new Color(0.1f, 0.1f, 0.1f) : oldAmbientColor;
      isDay = !isDay;
    }
    if (GUI.Button(new Rect(10, 105, 225, 30), "Change environment")) {
      if (isDefaultPlaneTexture) {
        Plane1.GetComponent<Renderer>().material = PlaneMaterials[0];
        Plane2.GetComponent<Renderer>().material = PlaneMaterials[0];
      }
      else {
        Plane1.GetComponent<Renderer>().material = PlaneMaterials[1];
        Plane2.GetComponent<Renderer>().material = PlaneMaterials[2];
      }
      isDefaultPlaneTexture = !isDefaultPlaneTexture;
    }
    if (current <= 15) {
      GUI.Label(new Rect(10, 152, 225, 30), "Ball Speed " + (int) prefabSpeed + "m", guiStyleHeader);
      prefabSpeed = GUI.HorizontalSlider(new Rect(115, 155, 120, 30), prefabSpeed, 1.0F, 30.0F);
      isHomingMove = GUI.Toggle(new Rect(10, 190, 150, 30), isHomingMove, " Is Homing Move");
      effectSettings.MoveSpeed = prefabSpeed;
    }

    GUI.Label(new Rect(1, 1, 30, 30), "" + (int)fps, guiStyleHeader);

  }

  void Update()
  {
    anim.enabled = isHomingMove;
    effectSettings.IsHomingMove = isHomingMove;

    timeleft -= Time.deltaTime;
    accum += Time.timeScale / Time.deltaTime;
    ++frames;

    if (timeleft <= 0.0)
    {
      fps = accum / frames;
      timeleft = UpdateInterval;
      accum = 0.0F;
      frames = 0;
    }
    if (isReadyEffect) {
      isReadyEffect = false;
      currentGo.SetActive(true);
    }
    if (isReadyDefaulBall)
    {
      isReadyDefaulBall = false;
      defaultBall.SetActive(true);
    }
  }

  private void InstanceCurrent(GuiStat stat)
  {
    switch (stat) {
    case GuiStat.Ball: {
      InstanceEffect(transform.position);
      break;
    }
    case GuiStat.Bottom: {
      InstanceEffect(BottomPosition.transform.position);
      break;
    }
    case GuiStat.Top: {
      InstanceEffect(TopPosition.transform.position);
      break;
    }
    case GuiStat.Middle: {
      MiddlePosition.SetActive(true);
      InstanceEffect(MiddlePosition.transform.position);
      break;
    }
    }
  }

  private Vector3 GetInstancePosition(GuiStat stat)
  {
    switch (stat) {
    case GuiStat.Ball: {
      return transform.position;
    }
    case GuiStat.Bottom: {
      return BottomPosition.transform.position;
    }
    case GuiStat.Top: {
      return TopPosition.transform.position;
    }
    case GuiStat.Middle: {
      return MiddlePosition.transform.position;
    }
    }
    return transform.position;
  }

  void ChangeCurrent(int delta)
  {
    Destroy(currentGo);
    Destroy(defaultBall);
    CancelInvoke("InstanceDefaulBall");
    current += delta;
    if (current> Prefabs.Length - 1)
      current = 0;
    else if (current < 0)
      current = Prefabs.Length - 1;
      
    if(effectSettings!=null) effectSettings.EffectDeactivated -= effectSettings_EffectDeactivated;
    if (defaultBallEffectSettings != null) defaultBallEffectSettings.EffectDeactivated -= effectSettings_EffectDeactivated;
    MiddlePosition.SetActive(GuiStats[current]==GuiStat.Middle);
    if (GuiStats[current] == GuiStat.Middle) Invoke("InstanceDefaulBall", 2);
    InstanceEffect(GetInstancePosition(GuiStats[current]));
  }
}