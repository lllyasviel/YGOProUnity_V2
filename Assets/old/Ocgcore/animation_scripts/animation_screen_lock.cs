using UnityEngine;
using System.Collections;

public class animation_screen_lock : MonoBehaviour
{
    public Vector3 screen_point = Vector3.zero;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = Camera.main.ScreenToWorldPoint(screen_point);
    }
}

public class animation_screen_lock2 : MonoBehaviour 
{
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Program.I().ocgcore.centre();
    }
}
