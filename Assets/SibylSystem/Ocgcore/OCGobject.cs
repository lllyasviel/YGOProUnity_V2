using System;
using System.Collections.Generic;
using UnityEngine;

public class OCGobject
{
    public GameObject gameObject;

    public List<GameObject> allObjects=new List<GameObject>();

    public GameObject create(   
        GameObject mod,
        Vector3 position = default(Vector3),
        Vector3 rotation = default(Vector3),
        bool fade = false,
        GameObject father = null,
        bool allParamsInWorld = true,
        Vector3 wantScale = default(Vector3)
        )
    {
        GameObject g = Program.I().ocgcore.create_s(mod, position, rotation, fade, father, allParamsInWorld, wantScale);
        allObjects.Add(g);
        return g;
    }

    public void destroy(GameObject obj, float time = 0, bool fade = false, bool instantNull = false)
    {
        allObjects.Remove(obj);
        Program.I().ocgcore.destroy(obj, time, fade, instantNull);
    }

}
