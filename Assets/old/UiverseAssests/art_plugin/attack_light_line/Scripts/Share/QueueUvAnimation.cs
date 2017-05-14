using UnityEngine;
using System.Collections;

public class QueueUvAnimation : MonoBehaviour
{
  public int RowsFadeIn = 4;
  public int ColumnsFadeIn = 4;
  public int RowsLoop = 4;
  public int ColumnsLoop = 4;
  public float Fps = 20;
  public bool IsBump = false;
  public Material NextMaterial;

  private int index;
  private int count, allCount;
  private float deltaTime;
  private bool isVisible;
  private bool isFadeHandle;

  private void Start()
  {
    deltaTime = 1f / Fps;
    InitDefaultTex(RowsFadeIn, ColumnsFadeIn);
  }

  private void InitDefaultTex(int rows, int colums)
  {
    count = rows * colums;
    index += colums - 1;
    var size = new Vector2(1f / colums, 1f / rows);
    GetComponent<Renderer>().material.SetTextureScale("_MainTex", size);
    if (IsBump) GetComponent<Renderer>().material.SetTextureScale("_BumpMap", size);
  }

  private void OnBecameVisible()
  {
    isVisible = true;
    StartCoroutine(UpdateTiling());
  }

  private void OnBecameInvisible()
  {
    isVisible = false;
  }

  private IEnumerator UpdateTiling()
  {
    while (isVisible && allCount!=count) {
      allCount++;
      index++;
      if (index >= count)
        index = 0;
      var offset = !isFadeHandle
        ? new Vector2((float) index / ColumnsFadeIn - (index / ColumnsFadeIn), 1 - (index / ColumnsFadeIn) / (float) RowsFadeIn)
        : new Vector2((float) index / ColumnsLoop - (index / ColumnsLoop), 1 - (index / ColumnsLoop) / (float) RowsLoop);
      if (!isFadeHandle) {
        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
        if (IsBump) GetComponent<Renderer>().material.SetTextureOffset("_BumpMap", offset);
      }
      else {
        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
        if (IsBump) GetComponent<Renderer>().material.SetTextureOffset("_BumpMap", offset);
      }

      if (allCount==count) {
        isFadeHandle = true;
        GetComponent<Renderer>().material = NextMaterial;
        InitDefaultTex(RowsLoop, ColumnsLoop);
      }
      yield return new WaitForSeconds(deltaTime);
    }
  }
}

