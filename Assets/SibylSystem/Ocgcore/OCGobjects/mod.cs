using System;
using UnityEngine;

public class mod : OCGobject    
{
    public mod()    
    {
        Program.I().ocgcore.AddUpdateAction_s(Update);
    }

    public void dispose()
    {
        Program.I().ocgcore.RemoveUpdateAction_s(Update);
    }

    public void Update()
    {

    }
}
