using UnityEngine;
using System.Collections;
using System;

public class MonoCardInDeckManager : MonoBehaviour {
    int loadedPicCode = 0;
    YGOSharp.Banlist loaded_banlist = null;
    public bool dying = false;
    bool died = false;
    public YGOSharp.Card cardData=new YGOSharp.Card();

	void Update () 
    {
        if (loadedPicCode != cardData.Id)
        {
            Texture2D pic = GameTextureManager.get(cardData.Id,GameTextureType.card_picture);
            if (pic != null)
            {
                loadedPicCode = cardData.Id;
                gameObject.transform.Find("face").GetComponent<Renderer>().material.mainTexture = pic;
            }
        }
        if (Program.I().deckManager.currentBanlist != loaded_banlist)
        {
            ban_icon ico = GetComponentInChildren<ban_icon>();
            loaded_banlist = Program.I().deckManager.currentBanlist;
            if (loaded_banlist != null)
            {
                ico.show(loaded_banlist.GetQuantity(cardData.Id));
            }
            else
            {
                ico.show(3);
            }
        }
        if (isDraging)
        {
            gameObject.transform.position += (getGoodPosition(4) - gameObject.transform.position) * 0.3f;
        }
        if (Vector3.Distance(Vector3.zero, gameObject.transform.position) > 50 && bool_physicalON)
        {
            killIt();
        }
    }

    public void killIt()
    {
        if (Program.I().deckManager.condition == DeckManager.Condition.changeSide)
        {
            gameObject.transform.position = new Vector3(0, 5, 0);
            endDrag();
            if (Program.I().deckManager.cardInDragging==this)   
            {
                Program.I().deckManager.cardInDragging = null;
            }
            Rigidbody rigidbody = GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
            }
            rigidbody.Sleep();
        }
        else
        {
            dying = true;
            died = true;
            gameObject.SetActive(false);
        }
    }

    public Vector3 getGoodPosition(float height)
    {
        float x = Input.mousePosition.x;
        float y = Input.mousePosition.y;
        Vector3 to_ltemp = Program.camera_game_main.ScreenToWorldPoint(new Vector3(x, y, 1));
        Vector3 dv = to_ltemp - Program.camera_game_main.transform.position;
        if (dv.y == 0) dv.y = 0.01f;
        to_ltemp.x = ((height - Program.camera_game_main.transform.position.y)
            * (dv.x) / dv.y + Program.camera_game_main.transform.position.x);
        to_ltemp.y = ((height - Program.camera_game_main.transform.position.y)
            * (dv.y) / dv.y + Program.camera_game_main.transform.position.y);
        to_ltemp.z = ((height - Program.camera_game_main.transform.position.y)
            * (dv.z) / dv.y + Program.camera_game_main.transform.position.z);
        return to_ltemp;
    }

    bool isDraging = false;

    public void beginDrag()
    {
        physicalOFF();
        physicalHalfON();
        isDraging = true;
        Program.go(1, () => { iTween.RotateTo(gameObject, new Vector3(90, 0, 0), 0.6f); });
    }

    public void endDrag()
    {
        physicalON();
        isDraging = false;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || getIfAlive() == false)
        {
            Vector3 form_position = getGoodPosition(4);
            Vector3 to_position = getGoodPosition(0);
            Vector3 delta_position = to_position - form_position;
            GetComponent<Rigidbody>().AddForce(delta_position * 1000);
            dying = true;
        }
    }

    public void tweenToVectorAndFall(Vector3 position,Vector3 rotation,float delay=0)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.Sleep();
        }
        iTween.MoveTo(gameObject, iTween.Hash(
                            "delay", delay,
                            "x", position.x,
                            "y", position.y,
                            "z", position.z,
                            "time", 0.2f,
                            "oncomplete", (Action)physicalON
                            ));
        iTween.RotateTo(gameObject, iTween.Hash(
                          "delay", delay,
                          "x", rotation.x,
                          "y", rotation.y,
                          "z", rotation.z,
                          "time", 0.15f
                          ));
        physicalOFF();
    }

    bool bool_physicalON = false;

    private void physicalON()
    {
        bool_physicalON = true;
        GetComponent<BoxCollider>().enabled = true;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
        }
        rigidbody.Sleep();
        rigidbody.useGravity = true;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void physicalHalfON()
    {
        bool_physicalON = false;
        GetComponent<BoxCollider>().enabled = true;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null)
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
        }
        rigidbody.useGravity = false;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void physicalOFF()
    {
        bool_physicalON = false;
        GetComponent<BoxCollider>().enabled = false;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.useGravity = false;
        }
    }

    public bool getIfAlive()
    {
        bool ret = true;
        if (died == true)
        {
            ret = false;
        }
        if (gameObject.transform.position.y < -0.5f)
        {
            ret = false;
        }
        Vector3 to_ltemp = refLectPosition(gameObject.transform.position);
        if (to_ltemp.x < -15.2f) ret = false;
        if (to_ltemp.x > 15.2f) ret = false;

        if (Program.I().deckManager.condition == DeckManager.Condition.changeSide)
        {
            ret = true;
        }
        return ret;
    }

    public static Vector3 refLectPosition(Vector3 pos)
    {
        Vector3 to_ltemp = pos;
        Vector3 dv = to_ltemp - Program.camera_game_main.transform.position;
        if (dv.y == 0) dv.y = 0.01f;
        to_ltemp.x = ((0 - Program.camera_game_main.transform.position.y)
            * (dv.x) / dv.y + Program.camera_game_main.transform.position.x);
        to_ltemp.y = ((0 - Program.camera_game_main.transform.position.y)
            * (dv.y) / dv.y + Program.camera_game_main.transform.position.y);
        to_ltemp.z = ((0 - Program.camera_game_main.transform.position.y)
            * (dv.z) / dv.y + Program.camera_game_main.transform.position.z);
        return to_ltemp;
    }
}
