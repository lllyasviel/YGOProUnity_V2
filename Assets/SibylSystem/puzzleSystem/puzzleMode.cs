using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class puzzleMode : WindowServantSP
{

    UIselectableList superScrollView = null;

    public override void initialize()
    {
        createWindow(Program.I().remaster_puzzleManager);
        UIHelper.registEvent(gameObject, "exit_", onClickExit);
        superScrollView = gameObject.GetComponentInChildren<UIselectableList>();
        superScrollView.selectedAction = onSelected;
        superScrollView.install();
        SetActiveFalse();
    }


    string selectedString = "miaomiaomiao";
    void onSelected()
    {
        if (!isShowed)
        {
            return;
        }
        if (selectedString == superScrollView.selectedString)
        {
            KF_puzzle(superScrollView.selectedString);
        }
        selectedString = superScrollView.selectedString;
    }

    public override void preFrameFunction()
    {
        base.preFrameFunction();
        Menu.checkCommend();
    }

    public void KF_puzzle(string name)
    {
        launch("puzzle/" + name + ".lua");
    }

    public override void show()
    {
        base.show();
        printFile();
    }

    void printFile()
    {
        superScrollView.clear();
        List<string[]> args = new List<string[]>();
        FileInfo[] fileInfos = (new DirectoryInfo("puzzle")).GetFiles();
        Array.Sort(fileInfos, UIHelper.CompareName);
        for (int i = 0; i < fileInfos.Length; i++)
        {
            if (fileInfos[i].Name.Length > 4)
            {
                if (fileInfos[i].Name.Substring(fileInfos[i].Name.Length - 4, 4) == ".lua")
                {
                    superScrollView.add(fileInfos[i].Name.Substring(0, fileInfos[i].Name.Length - 4));
                }
            }
        }
    }

    void onClickExit()
    {
        if (Program.exitOnReturn)
            Program.I().menu.onClickExit();
        else
            Program.I().shiftToServant(Program.I().menu);
    }

    PrecyOcg precy;

    public void launch(string path)
    {
        if (precy != null)
        {
            precy.dispose();
        }
        precy = new PrecyOcg();
        precy.startPuzzle(path);
    }
}

