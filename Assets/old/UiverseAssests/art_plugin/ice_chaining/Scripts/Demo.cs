using UnityEngine;
using System.Collections.Generic;

public class Demo : MonoBehaviour {

    public GameObject[] effects;

    int selected = 0;

    GameObject instance = null;

    bool loopedEffect = false;

    void Start()
    {
        OnSwitch();
    }

    void OnGUI()
    {
        List<string> effectNames = new List<string>();
        foreach (GameObject go in effects)
        {
            effectNames.Add(go.name);
        }

        int lastSelected = selected;

        selected = GUILayout.Toolbar(selected, effectNames.ToArray());

        if (selected != lastSelected)
            OnSwitch();
    }

    void OnSwitch()
    {
        if (loopedEffect && instance != null)
        {
            GameObject.Destroy(instance);
        }

        instance = (GameObject)GameObject.Instantiate(effects[selected], transform.position, Quaternion.identity);

        if (instance.GetComponent<ParticleAutoDestruction>() != null)
        {
            loopedEffect = false;
        }
        else
        {
            loopedEffect = true;
        }
    }
}
