using UnityEngine;
using System.Collections;

public class speed_scale_per : MonoBehaviour {
    public float scale = 1f;
	// Use this for initialization
	void Start () {
        var particles = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        foreach (ParticleSystem p in particles)
        {
            p.startSize *= scale;
            p.startSpeed *= scale;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
