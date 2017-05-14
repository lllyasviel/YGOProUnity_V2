using System.Collections;
using UnityEngine;

public class WaterUvAnimation : MonoBehaviour
{
  public bool IsReverse;
  public float Speed = 1;

  private Material mat;
  private float deltaFps;
  private bool isVisible;
  private bool isCorutineStarted;
  private float offset, delta;
  
  private void Awake()
  {
    mat = GetComponent<Renderer>().material;
  }

  private void Update()
  {
    if (IsReverse)
    {
      offset -= Time.deltaTime*Speed;
      if (offset < 0)
        offset = 1;
    }
    else
    {
      offset += Time.deltaTime * Speed;
      if (offset > 1)
        offset = 0;
    }
    var vec = new Vector2(0, offset);
    mat.SetTextureOffset("_BumpMap", vec);
    mat.SetFloat("_OffsetYHeightMap", offset);
  }
}