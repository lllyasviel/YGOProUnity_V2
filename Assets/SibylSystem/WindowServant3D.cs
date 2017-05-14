//using System;
//using UnityEngine;
//public class WindowServant3D:Servant
//{
//    public int visiableDistance = 400;

//    public override void hide()
//    {
//        base.hide();
//        Program.shiftCameraPan(Program.camera_main_3d, false);
//    }

//    public override void show()
//    {
//        base.show();
//        Program.shiftCameraPan(Program.camera_main_3d, true);
//    }

//    public override void applyHideArrangement()
//    {
//        base.applyHideArrangement();
//        if (gameObject!=null)
//        {
//            var panelKIller = gameObject.GetComponent<panelKIller>();
//            if (panelKIller == null)
//            {
//                panelKIller= gameObject.AddComponent<panelKIller>();
//            }
//            panelKIller.set(false);
//            Program.go(1000, killani);
//        }
//    }

//    private void killani()
//    {
//        gameObject.transform.localPosition = Program.camera_main_3d.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height * 1.5f, 1000));
//    }

//    public override void applyShowArrangement()
//    {
//        base.applyShowArrangement();
//        if (gameObject != null)
//        {
//            var panelKIller = gameObject.GetComponent<panelKIller>();
//            if (panelKIller == null)
//            {
//                panelKIller = gameObject.AddComponent<panelKIller>();
//            }
//            panelKIller.set(true);
//            Program.notGo(killani);
//            gameObject.transform.localPosition = Program.camera_main_3d.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 500));
//        }
//    }

//    public static GameObject createWindow(Servant servant,GameObject mod)
//    {
//        GameObject re = servant.create
//            (
//            mod,
//            Program.camera_main_3d.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height * 1.5f, 600)),
//            new Vector3(60, 0, 0),
//            false,
//            Program.ui_main_3d
//            );
//        UIHelper.InterGameObject(re);
//        return re;
//    }
//}
