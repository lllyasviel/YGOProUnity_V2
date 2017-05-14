using UnityEngine;
using System.Collections;

public class screenFader : MonoBehaviour
{
    Vector3 screen_point = Vector3.zero;
    public Vector3 from = Vector3.zero;
    public float time = 0.3f;
    // Use this for initialization
    void Start()
    {
        deltaTimeCloseUp = 0;
    }
    public float deltaTimeCloseUp = 0;
    // Update is called once per frame
    void Update()
    {

        screen_point = Program.camera_game_main.WorldToScreenPoint(Vector3.zero);
        screen_point.z = 8f;
        int bun = Screen.height / 3;
        if (screen_point.y > Screen.height / 2 + bun)
        {
            screen_point.y = Screen.height / 2 + bun;
        }
        if (screen_point.y < Screen.height / 2 - bun)
        {
            screen_point.y = Screen.height / 2 - bun;
        }
        Vector3 to = Program.camera_game_main.ScreenToWorldPoint(screen_point);





        deltaTimeCloseUp += Time.deltaTime;
        if (deltaTimeCloseUp > time)
        {
            deltaTimeCloseUp = time;
        }
        gameObject.transform.position = new Vector3
            (
            iTween.easeOutQuad(from.x, to.x, deltaTimeCloseUp / time),
            iTween.easeOutQuad(from.y, to.y, deltaTimeCloseUp / time),
            iTween.easeOutQuad(from.z, to.z, deltaTimeCloseUp / time)
            );
    }
}
