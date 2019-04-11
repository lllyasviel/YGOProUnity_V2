using System;
using System.Collections.Generic;
using UnityEngine;
public class SuperScrollView
{
    UIPanel panel;

    UIScrollBar scrollBar;

    Func<string[], GameObject> itemOnListProducer;

    public Action<GameObject> itemOnListHider;

    public Action<GameObject> itemOnSelect;

    public Action<GameObject> itemOnListShower;

    public Action<GameObject, bool> selectHandler;

    private GameObject mSelected;

    public GameObject getSelected()
    {
        return mSelected;
    }

    public void setSelected(GameObject obj)    
    {
        if (obj != mSelected)
        {
            if (mSelected != null)
            {
                if (mSelected.activeInHierarchy)
                {
                    if (selectHandler != null)
                    {
                        selectHandler(mSelected, false);
                    }
                }
            }
            mSelected = obj;
            if (mSelected != null)
            {
                if (mSelected.activeInHierarchy)
                {
                    if (selectHandler != null)
                    {
                        selectHandler(mSelected, true);
                    }
                }
            }
            if (itemOnSelect != null)
            {
                itemOnSelect(obj);
            }
        }
    }

    float heightOfEach;

    UIScrollView uIScrollView;

    public SuperScrollView
        (
        UIPanel panel,
        UIScrollBar scrollBar,
        Func<string[], GameObject> itemOnListProducer,
        float heightOfEach
        )
    {
        this.panel = panel;
        this.scrollBar = scrollBar;
        this.itemOnListProducer = itemOnListProducer;
        this.heightOfEach = heightOfEach;
        install();
    }

    void install()
    {
        uIScrollView = panel.gameObject.AddComponent<UIScrollView>();
        uIScrollView.can_be_draged = false;
        uIScrollView.movement = UIScrollView.Movement.Vertical;
        uIScrollView.contentPivot = UIWidget.Pivot.TopLeft;
        uIScrollView.dragEffect = UIScrollView.DragEffect.Momentum;
        scrollBar.value = 0;
        scrollBar.barSize = 0.1f;
        UIHelper.registEvent(uIScrollView, printSmall);
        UIHelper.registEvent(scrollBar, onScrollBarChange);
        float magicNumber = -(panel.GetViewSize().y) / 2 + heightOfEach;
        uIScrollView.transform.localPosition = new Vector3(
            uIScrollView.transform.localPosition.x,
            -magicNumber,
            uIScrollView.transform.localPosition.z
            );
        panel.clipOffset = new Vector2(0, magicNumber);
    }

    public void selectArg(string[] task)
    {
        int index = -1;
        for (int i = 0; i < Items.Count; i++)
        {
            if (task != null)
            {
                bool same = true;
                if (Items[i].Args.Length != task.Length)
                {
                    same = false;
                }
                else
                {
                    for (int x = 0; x < task.Length; x++)
                    {
                        if (Items[i].Args[x] != task[x])
                        {
                            same = false;
                        }
                    }
                }
                if (same)
                {
                    index = i;
                }
            }
        }
        if (index > -1)
        {
            selectIndex(index);
        }
    }

    public void print(List<string[]> tasks)
    {
        int index = -1;
        string[] selectedArgs = null;
        for (int i = 0; i < Items.Count; i++)   
        {
            if (Items[i].gameObject == mSelected && mSelected != null)
            {
                selectedArgs = Items[i].Args;
                index = i;
            }
        }
        panel.transform.DestroyChildren();
        Items.Clear();
        for (int i = 0; i < tasks.Count; i++)
        {
            Item it = new Item();
            it.Args = tasks[i];
            it.gameObject = null;
            Items.Add(it);
            if (selectedArgs != null)
            {
                bool same = true;
                if (selectedArgs.Length != it.Args.Length)
                {
                    same = false;
                }
                else
                {
                    for (int x = 0; x < selectedArgs.Length; x++)
                    {
                        if (selectedArgs[x] != it.Args[x])
                        {
                            same = false;
                        }
                    }
                }
                if (same)
                {
                    index = i;
                }
            }
        }
        if (index != -1)
        {
            selectIndex(index);
        }
        lastForce = true;
        printSmall();
        scrollBar.barSize = (float)(panel.GetViewSize().y / heightOfEach) / (float)(Items.Count);
        if (scrollBar.barSize < 0.1f)
        {
            scrollBar.barSize = 0.1f;
        }
    }

    public void clear()
    {
        panel.transform.DestroyChildren();
        Items.Clear();
    }

    public class Item
    {
        public string[] Args = null;
        public GameObject gameObject = null;
    }

    public List<Item> Items = new List<Item>();

    void onScrollBarChange()
    {
        //Program.notGo(changeHandler);
        //Program.go(10, changeHandler);
        changeHandler();
    }
    float moveFloat;
    float maxFloat;

    void caculateMoveFloat()
    {
        moveFloat = panel.baseClipRegion.y + (panel.GetViewSize().y - heightOfEach) / 2 - 10;
        maxFloat = -(heightOfEach * Items.Count - panel.GetViewSize().y + 20);
        // 1900x1000 resolution and 11 cards displayed caused this value to reach exactly 0 and crash the game. 
        if (maxFloat >= 0)
        {
            maxFloat = -0.001f;
        }
    }

    void changeHandler()
    {
        caculateMoveFloat();
        float now = scrollBar.value * maxFloat;
        panel.clipOffset = new Vector2(panel.clipOffset.x, now - moveFloat);
        uIScrollView.transform.localPosition = new Vector3(
           uIScrollView.transform.localPosition.x,
           -panel.clipOffset.y,
           0
           );
        printSmall();
    }

    float lastMin = 0;

    bool lastForce = false;

    void printSmall()
    {
        caculateMoveFloat();
        float now = panel.clipOffset.y + moveFloat;
        scrollBar.value = now / maxFloat;
        float min = -panel.clipOffset.y - (Screen.height / 2 + 100);
        if (Math.Abs(min - lastMin) > 40 || Items.Count<100||lastForce)
        {
            lastForce = false;
            lastMin = min;
            float max = -panel.clipOffset.y + panel.GetViewSize().y + (Screen.height / 2 + 100);
            for (int i = 0; i < Items.Count; i++)
            {
                if (i >= (int)(min / heightOfEach) && i <= (int)(max / heightOfEach))
                {
                    createItem(i);
                    Items[i].gameObject.SetActive(true);
                    if (selectHandler != null)
                    {
                        if (Items[i].gameObject != mSelected)
                        {
                            selectHandler(Items[i].gameObject, false);
                        }
                        else
                        {
                            selectHandler(Items[i].gameObject, true);
                        }
                    }
                    if (itemOnListShower != null)
                    {
                        itemOnListShower(Items[i].gameObject);
                    }
                }
                else
                {
                    if (Items[i].gameObject != null)
                    {
                        if (selectHandler != null)
                        {
                            if (Items[i].gameObject.activeInHierarchy)
                            {
                                if (Items[i].gameObject != mSelected)
                                {
                                    selectHandler(Items[i].gameObject, false);
                                }
                                else
                                {
                                    selectHandler(Items[i].gameObject, true);
                                }
                            }
                        }
                        if (itemOnListHider != null)
                        {
                            itemOnListHider(Items[i].gameObject);
                        }
                        Items[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    public void selectIndex(int i=0)
    {
        if (i >= 0)
        {
            if (Items.Count > i)
            {
                if (Items[i].gameObject == null)
                {
                    createItem(i);
                    Items[i].gameObject.SetActive(false);
                }
                setSelected(Items[i].gameObject);
            }
        }
    }

    private void createItem(int i)
    {
        if (Items[i].gameObject == null)
        {
            Items[i].gameObject = itemOnListProducer(Items[i].Args);
            Items[i].gameObject.transform.SetParent(panel.gameObject.transform, false);
            Items[i].gameObject.transform.localPosition = new Vector3(0, -i * heightOfEach, 0);
            BoxCollider boxCollider = Items[i].gameObject.transform.GetComponentInChildren<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.gameObject.AddComponent<UIDragScrollView>().scrollView = uIScrollView;
            }
        }
    }

    public void toTop()
    {
        scrollBar.value = 0;
        Program.go(50, onScrollBarChange);
        onScrollBarChange();
    }

}

