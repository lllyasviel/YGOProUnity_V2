using UnityEngine;
using System.Collections;

public class spining : MonoBehaviour {
	void Update () {
        transform.localEulerAngles+=(Vector3.forward * Program.deltaTime * 160);
    }
}
