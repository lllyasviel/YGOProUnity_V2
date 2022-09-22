using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIselectableList : MonoBehaviour {
    public GameObject mod;
    public UIPanel panel;
    public UIScrollBar scrollBar;
    private string m_selectedString;
    public string selectedString
    {
        get
        {
            return m_selectedString;
        }
        set
        {
            if (m_selectedString != value)
            {
                m_selectedString = value;
                if (selectedAction != null)
                {
                    selectedAction();
                }
            }
        }
    }

    public int selectedIndex
    {
        get
        {
            for (int i = 0; i < selections.Count; i++)
            {
                if (selections[i].str == selectedString)
                {
                    return i;
                }
            }
            return -1;
        }
    }

    public Action selectedAction;
    UIScrollView uIScrollView;
    float heightOfEach=20;
    public float preHeight = 0;
    class selection
    {
        public string str = "";
        public UIselectableListItem obj = null;
        public bool selected = false;
    }
    List<selection> selections = new List<selection>(); 
    float width = 0;
    float height = 0;

    public void mark()
    {
        needRefresh = true;
    }

    public void install()
    {
        uIScrollView = panel.gameObject.AddComponent<UIScrollView>();
        uIScrollView.can_be_draged = false;
        uIScrollView.movement = UIScrollView.Movement.Vertical;
        uIScrollView.contentPivot = UIWidget.Pivot.TopLeft;
        uIScrollView.dragEffect = UIScrollView.DragEffect.Momentum;
        scrollBar.onDragged = onScrollBarChange;
        scrollBar.value = 0;
        scrollBar.barSize = 0.3f;
        width = panel.GetViewSize().x;
        height = panel.GetViewSize().y;
        clear();
        toTop();
        
    }

    void onScrollBarChange()
    {
        float beginY = (height - heightOfEach) / 2f;
        float endY = beginY + heightOfEach * selections.Count + preHeight - height;
        float allHeight = endY - beginY;
        float curHeight = allHeight * scrollBar.value;
        needRefresh = true;
        float Y = curHeight + beginY;
        uIScrollView.transform.localPosition = new Vector3(
            uIScrollView.transform.localPosition.x,
            Y,
            uIScrollView.transform.localPosition.z
            );
        panel.clipOffset = new Vector2(0, -Y);
    }

    bool needRefresh = false;

    public void clear()
    {
        for (int i = 0; i < selections.Count; i++)
        {
            if (selections[i].obj != null)
            {
                Destroy(selections[i].obj.gameObject);
                selections[i].obj = null;
            }
        }
        selections.Clear();
        needRefresh = true;
    }

    public bool Selected()  
    {
        bool returnValue = false;
        foreach (var item in selections)
        {
            if (selectedString == item.str)
            {
                returnValue = true;
            }
        }
        return returnValue;
    }

    public void toTop(bool select = false)
    {
        float Y = (height - heightOfEach) / 2f;
        uIScrollView.transform.localPosition = new Vector3(
            uIScrollView.transform.localPosition.x,
            Y,
            uIScrollView.transform.localPosition.z
            );
        panel.clipOffset = new Vector2(0, -Y);
        if (select)
        {
            selectTop();
        }
    }

    public void selectTop()
    {
        if (selections.Count > 0)
        {
            selectedString = selections[0].str;
        }
    }

    public void add(string item)
    {
        selections.Add(new selection
        {
            str = item,
            obj = null,
            selected = false
        });
        needRefresh = true;
    }

    float pre = 0;
    public void refresh()
    {
        float screenTop = panel.clipOffset.y + (height - heightOfEach) / 2f;
        if (needRefresh || pre != screenTop)
        {
            pre = screenTop;
            needRefresh = false;
            if (selections.Count > (int)(height / heightOfEach))
            {
                scrollBar.gameObject.SetActive(true);
                scrollBar.barSize = height / (heightOfEach * selections.Count);
                if (scrollBar.barSize < 0.1f)
                {
                    scrollBar.barSize = 0.1f;
                }
                float beginY = (height - heightOfEach) / 2f;
                float endY = beginY + heightOfEach * selections.Count+preHeight - height;
                float allHeight = endY - beginY;
                float curHeight = uIScrollView.transform.localPosition.y - beginY;
                scrollBar.value = curHeight / allHeight;
            }
            else
            {
                scrollBar.gameObject.SetActive(false);
            }
            float screenButtom = screenTop - height + heightOfEach;
            for (int i = 0; i < selections.Count; i++)  
            {
                var currentItem = selections[i];
                float currentY = -i * heightOfEach - preHeight;
                if (screenButtom - 100 < currentY && currentY < screenTop + 100)
                {
                    if (currentItem.obj != null)
                    {
                        currentItem.obj.gameObject.SetActive(true);
                    }
                    else
                    {
                        currentItem.obj = MonoBehaviour.Instantiate<GameObject>(mod).GetComponent<UIselectableListItem>();
                        currentItem.obj.List = this;
                        currentItem.obj.transform.SetParent(panel.transform, false);
                        currentItem.obj.lable.width = (int)width - 10;
                        currentItem.obj.transform.localPosition = new Vector3(0, currentY);
                        currentItem.obj.lable.text = currentItem.str;
                    }
                    if (currentItem.str == selectedString)
                    {
                        currentItem.selected = true;
                        currentItem.obj.selectedObject.SetActive(true);
                    }
                    else
                    {
                        if (currentItem.selected)
                        {
                            currentItem.selected = false;
                            currentItem.obj.selectedObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    if (currentItem.obj != null)
                    {
                        currentItem.obj.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

	void Update () {
        refresh();

    }

    public void refreshForOneFrame()
    {
        bool t = needRefresh;
        refresh();
        scrollBar.gameObject.SetActive(false);
        needRefresh = t;
    }
}
