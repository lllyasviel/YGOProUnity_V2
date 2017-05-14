using UnityEngine;
using System.Collections;

public class RandomPosition : MonoBehaviour
{
  public int fps = 30;
  public float x = 1, y = 2, z = 3;

  private float rangeX, rangeY, rangeZ;
  // Use this for initialization
  private void Start()
  {
    rangeX = Random.Range(0, x);
    rangeY = Random.Range(0, y);
    rangeZ = Random.Range(0, z);
  }

  // Update is called once per frame
  private void Update()
  {
    var offset = new Vector3(Time.deltaTime * Mathf.Sin(Time.time + rangeX) * x,
      Time.deltaTime * Mathf.Sin(Time.time + rangeY) * y,
      Time.deltaTime * Mathf.Sin(Time.time + rangeZ) * z);
    transform.position += offset * Time.deltaTime;
  }
}