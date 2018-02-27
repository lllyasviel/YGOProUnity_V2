using System;
using UnityEngine;
public class WindowServantSP : Servant  
{
    public bool instanceHide = false;

    public override void hide()
    {
        base.hide();
        if (instanceHide)
        {
            if (gameObject != null)
            {
                var glass = gameObject.transform.Find("glass");
                UIPanel pan = gameObject.GetComponentInChildren<UIPanel>();
                if (pan != null)
                {
                    pan.alpha = 0;
                }
                if (glass != null)
                {
                    glass.gameObject.SetActive(false);
                }
                SetActiveFalse();
            }
        }
    }

    public override void applyHideArrangement()
    {
        base.applyHideArrangement();
        if (gameObject != null)
        {
            if (instanceHide)   
            {
                return;
            }
            var glass = gameObject.transform.Find("glass");
            var panelKIller = gameObject.GetComponent<panelKIller>();
            if (panelKIller == null)
            {
                panelKIller = gameObject.AddComponent<panelKIller>();
            }
            panelKIller.set(false);
            Program.go(1000, SetActiveFalse);
            if (glass != null)
            {
                glass.gameObject.SetActive(false);
            }
        }
        resize();
    }

    public void SetActiveFalse()   
    {
        gameObject.SetActive(false);
    }

    public void SetActiveTrue()
    {
        gameObject.SetActive(true);
    }

    public override void applyShowArrangement()
    {
        base.applyShowArrangement();
        if (gameObject != null)
        {
            Program.notGo(SetActiveFalse);
            SetActiveTrue();
            var panelKIller = gameObject.GetComponent<panelKIller>();
            if (panelKIller == null)
            {
                panelKIller = gameObject.AddComponent<panelKIller>();
            }
            panelKIller.set(true);
            var glass = gameObject.transform.Find("glass");
            if (glass != null)
            {
                glass.gameObject.SetActive(true);
            }
        }
        resize();
    }

    void resize()
    {
        if (gameObject != null)
        {
            if (Program.I().setting.setting.resize.value)
            {
                float f = Screen.height / 700f;
                gameObject.transform.localScale = new Vector3(f, f, f);
            }
            else
            {
                gameObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public void createWindow(GameObject mod)
    {
        gameObject = create
            (
            mod,
            Vector3.zero,
            Vector3.zero,
            false,
            Program.ui_windows_2d
            );
        UIHelper.InterGameObject(gameObject);
        Vector3 v=new Vector3();
        v.x = Mathf.Clamp(Config.getFloat("x_" + gameObject.name), -0.5f, 0.5f) * (float)Screen.width;
        v.y = Mathf.Clamp(Config.getFloat("y_" + gameObject.name), -0.5f, 0.5f) * (float)Screen.height;
        gameObject.transform.localPosition = v;
        var panelKIller = gameObject.GetComponent<panelKIller>();
        if (panelKIller == null)
        {
            panelKIller = gameObject.AddComponent<panelKIller>();
        }
        panelKIller.ini();
    }

    public override void ES_quit()
    {
        base.ES_quit();
        if (gameObject != null)
        {
            Config.setFloat("x_" + gameObject.name, gameObject.transform.localPosition.x / (float)Screen.width);
            Config.setFloat("y_" + gameObject.name, gameObject.transform.localPosition.y / (float)Screen.height);
        }
    }
}
