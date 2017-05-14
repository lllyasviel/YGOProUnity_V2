using UnityEngine;
using System.Collections;

public class handShower : MonoBehaviour
{
    public Texture2D[] pics = new Texture2D[3];
    public int op = 0;
    public int me = 0;
    public UITexture texture_0;
    public UITexture texture_1;
    public GameObject GameObject_0;
    public GameObject GameObject_1;
    void Start()
    {
        pics[0] = GameTextureManager.get("jiandao");
        pics[1] = GameTextureManager.get("shitou");
        pics[2] = GameTextureManager.get("bu");
        texture_0.mainTexture = pics[me];
        texture_1.mainTexture = pics[op];
        GameObject_0.transform.position = Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width / 2, -Screen.height * 1.5f, 0));
        iTween.MoveToAction(GameObject_0, Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2 - 80, 0)), 1f, () => {
            Destroy(GameObject_0,0.3f);
        }, 0);
        GameObject_1.transform.position = Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height * 1.5f, 0));
        iTween.MoveToAction(GameObject_1, Program.camera_main_2d.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2 + 80, 0)), 1f, () => {
            Destroy(GameObject_1, 0.3f);
        }, 0);
    }

}
