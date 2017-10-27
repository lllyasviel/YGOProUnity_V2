using System;
using UnityEngine;
public class ServantWithCardDescription : Servant
{
    public override void show()
    {
        Program.I().cardDescription.show();
        base.show();
    }

    public override void hide()
    {
        Program.reMoveCam(Screen.width / 2);
        Program.I().cardDescription.hide();
        base.hide();
    }

    public override void fixScreenProblem()
    {
        base.fixScreenProblem();
        Program.I().cardDescription.fixScreenProblem();
    }

    public override void preFrameFunction()
    {
        var des = Program.I().cardDescription;
        if (Program.pointedGameObject!= Program.I().cardDescription.description.gameObject)    
        {
            des.description.OnScroll(Program.wheelValue / 50f);
        }
        des.onResized();
    }
}
