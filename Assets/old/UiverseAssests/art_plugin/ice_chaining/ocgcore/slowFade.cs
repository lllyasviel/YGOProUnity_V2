using UnityEngine;
using System.Collections;

public class slowFade : MonoBehaviour
{
    public bool yse = true;
    // Use this for initialization
    //void Start () {
    //       if (yse)
    //       {
    //           Program.go(1000, () => {
    //               try
    //               {
    //                   gameObject.AddComponent<partical_alphaer>().scale = 0.3f;
    //               }
    //               catch (System.Exception)
    //               {
    //               }
    //           });
    //       }
    //   }
    bool fades = false;
    // Update is called once per frame
    void Update()
    {
        if (yse)
        {
            if (fades == false)
            {
                fades = true;
                gameObject.AddComponent<partical_alphaer>().scale = 0.3f;
            }
        }

    }
}
