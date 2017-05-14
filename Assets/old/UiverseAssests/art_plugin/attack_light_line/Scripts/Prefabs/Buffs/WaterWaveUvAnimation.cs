using UnityEngine;
using System.Collections;

public class WaterWaveUvAnimation: MonoBehaviour
{
  public float speed = 1;
  public int fps = 30;
  public Color color;

  private Material mat;

  //private Material mat;
  private float offset, offsetHeight;
  private float delta;
	// Use this for initialization
	void Start ()
	{
	  mat = GetComponent<Renderer>().material;
    delta = 1f / fps * speed;
    StartCoroutine(updateTiling());
	}
	
	// Update is called once per frame
	IEnumerator updateTiling ()
	{
	  while (true) {
	    offset += delta;
	    offsetHeight += delta;
	    if (offset >= 1) {
        Destroy(gameObject);
	    }
	    var vec = new Vector2(0, offset);
	    mat.SetTextureOffset("_BumpMap", vec);
	    mat.SetFloat("_OffsetYHeightMap", offset);
      if (offset < 0.3f) mat.SetColor("_Color", new Color(color.r, color.g, color.b, offset / 0.3f));
      if (offset > 0.7f) mat.SetColor("_Color", new Color(color.r, color.g, color.b, (1 - offset) / 0.3f));
      yield return new WaitForSeconds(1f / fps);
	  }
	}
}
