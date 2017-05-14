using UnityEngine;
using System.Collections;

public class partical_alphaer : MonoBehaviour {
    public float scale = 1;
    // Use this for initialization
    void Start()
    {
        var particles = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        foreach (ParticleSystem p in particles)
        {
            Color aaa = p.startColor;
            aaa.a *= scale;
            p.startColor = aaa;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
