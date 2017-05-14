using UnityEngine;
using System.Collections.Generic;

public class JointBreaker : MonoBehaviour
{
    Rope2 script;

    public void SetParentControl(Rope2 script)
    {
        this.script = script;
    }

    void OnJointBreak(float breakForce)
    {
        Debug.Log("A joint has just been broken!, force: " + breakForce);
    }
}
