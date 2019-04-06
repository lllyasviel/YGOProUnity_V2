using System;
using UnityEngine;
using YGOSharp.OCGWrapper.Enums;

public class gameHiddenButton : OCGobject
{
    public CardLocation location;

    public int player;

    public TextMaster hintText;

    GPS ps;

    public gameHiddenButton(CardLocation l, int p)
    {
        ps = new GPS();
        ps.controller = (UInt32)p;
        ps.location = (UInt32)l;
        ps.position = 0;
        ps.sequence = 0;
        Program.I().ocgcore.AddUpdateAction_s(Update);
        player = p;
        location = l;
        gameObject = create(Program.I().mod_ocgcore_hidden_button);
    }

    public void dispose()
    {
        Program.I().ocgcore.RemoveUpdateAction_s(Update);
    }

    bool excited = false;

    public void Update()
    {
        if (gameObject != null)
        {
            gameObject.transform.position = Program.I().ocgcore.get_point_worldposition(ps);
            if (Program.pointedGameObject == gameObject)
            {
                if (excited == false)
                    excite();
                if (Program.InputGetMouseButtonUp_0)
                {
                    showAll();
                }
            }
            else
            {
                if (excited == true)
                {
                    excited = false;
                    calm();
                }
            }
        }
    }

    void showAll()
    {
        bool allShow = true;
        for (int i = 0; i < Program.I().ocgcore.cards.Count; i++) if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
            {
                if ((Program.I().ocgcore.cards[i].p.location & (UInt32)location) > 0)
                {
                    if (Program.I().ocgcore.cards[i].p.controller == player)
                    {
                        if (Program.I().ocgcore.cards[i].isShowed == false)
                        {
                            if (Program.I().ocgcore.cards[i].prefered == true)
                            {
                                allShow = false;
                            }
                        }
                    }
                }
            }
        for (int i = 0; i < Program.I().ocgcore.cards.Count; i++) if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
            {
                if ((Program.I().ocgcore.cards[i].p.location & (UInt32)location) > 0)
                {
                    if (Program.I().ocgcore.cards[i].p.controller == player)
                    {
                        if (allShow)
                        {
                            Program.I().ocgcore.cards[i].isShowed = true;
                        }
                        else
                        {
                            if (Program.I().ocgcore.cards[i].prefered == true)
                            {
                                Program.I().ocgcore.cards[i].isShowed = true;
                            }
                        }
                    }
                }
            }
        Program.I().ocgcore.realize();
        Program.I().ocgcore.toNearest();
        Program.I().audio.clip = Program.I().zhankai;
        Program.I().audio.Play();
    }

    void calm()
    {
        if (Program.I().ocgcore.condition == Ocgcore.Condition.duel && Program.I().ocgcore.InAI == false && Program.I().room.mode != 2)
        {
            if (player == 0)
            {
                if (location == CardLocation.Deck)
                {
                    if (Program.I().book.lab != null)
                    {
                        destroy(Program.I().book.lab.gameObject);
                        Program.I().book.lab = null;
                    }
                    return;
                }
            }
        }
        if (player == 1)
        {
            if (location == CardLocation.Deck)
            {
                if (Program.I().book.labop != null)
                {
                    destroy(Program.I().book.labop.gameObject);
                    Program.I().book.labop = null;
                }
                return;
            }
        }
        for (int i = 0; i < Program.I().ocgcore.cards.Count; i++) if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
            {
            if ((Program.I().ocgcore.cards[i].p.location & (UInt32)location) > 0)
            {
                if (Program.I().ocgcore.cards[i].p.controller == player && Program.I().ocgcore.cards[i].isShowed == false)
                {
                    Program.I().ocgcore.cards[i].ES_safe_card_move_to_original_place();
                }
            }
        }
        if (hintText != null)
        {
            hintText.dispose();
            hintText = null;
        }
    }

    void excite()
    {
        excited = true;
        YGOSharp.Card data = null;
        string tailString = "";
        uint con = 0;
        for (int i = 0; i < Program.I().ocgcore.cards.Count; i++) if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
            {
                if ((Program.I().ocgcore.cards[i].p.location & (UInt32)location) > 0)
                {
                    if (Program.I().ocgcore.cards[i].p.controller == player)
                    {
                        if (Program.I().ocgcore.cards[i].isShowed == false)
                        {
                            data = Program.I().ocgcore.cards[i].get_data();
                            tailString = Program.I().ocgcore.cards[i].tails.managedString;
                            con = Program.I().ocgcore.cards[i].p.controller;
                        }
                    }
                }
            }
        Program.I().cardDescription.setData(data, con == 0 ? GameTextureManager.myBack : GameTextureManager.opBack, tailString, data != null);
        if (Program.I().ocgcore.condition == Ocgcore.Condition.duel && Program.I().ocgcore.InAI == false && Program.I().room.mode != 2)
        {
            if (player == 0)
            {
                if (location == CardLocation.Deck)
                {
                    if (Program.I().book.lab != null)
                    {
                        destroy(Program.I().book.lab.gameObject);
                        Program.I().book.lab = null;
                    }


                    Program.I().book.lab = create(Program.I().New_decker, Vector3.zero, Vector3.zero, false, Program.ui_main_2d, true).GetComponent<UILabel>();
                    Program.I().book.realize();


                    Vector3 screenPosition = Input.mousePosition;
                    screenPosition.x -= 90;
                    screenPosition.y += Program.I().book.lab.height/4;
                    screenPosition.z = 0;
                    Vector3 worldPositin = Program.camera_main_2d.ScreenToWorldPoint(screenPosition);
                    Program.I().book.lab.transform.position = worldPositin;

                    return;
                }
            }
        }


        if (player == 1)
        {
            if (location == CardLocation.Deck)
            {
                if (Program.I().book.labop != null)
                {
                    destroy(Program.I().book.labop.gameObject);
                    Program.I().book.labop = null;
                }


                Program.I().book.labop = create(Program.I().New_decker, Vector3.zero, Vector3.zero, false, Program.ui_main_2d, true).GetComponent<UILabel>();
                Program.I().book.realize();


                Vector3 screenPosition = Input.mousePosition;
                screenPosition.x -= 90;
                screenPosition.y -= Program.I().book.labop.height / 4;
                screenPosition.z = 0;
                Vector3 worldPositin = Program.camera_main_2d.ScreenToWorldPoint(screenPosition);
                Program.I().book.labop.transform.position = worldPositin;

                return;
            }
        }

        int count = 0;
        for (int i = 0; i < Program.I().ocgcore.cards.Count; i++)if(Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
        {
            if ((Program.I().ocgcore.cards[i].p.location & (UInt32)location) > 0)
            {
                if (Program.I().ocgcore.cards[i].p.controller == player)
                {
                    count++;
                }
            }
        }
        int count_show = 0;
        for (int i = 0; i < Program.I().ocgcore.cards.Count; i++) if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
            {
            if ((Program.I().ocgcore.cards[i].p.location & (UInt32)location) > 0)
            {
                if (Program.I().ocgcore.cards[i].p.controller == player && Program.I().ocgcore.cards[i].isShowed == false)
                {
                    count_show++;
                }
            }
        }
        if (hintText != null)
        {
            hintText.dispose();
            hintText = null;
        }
        if (count > 0)
        {
            hintText = new TextMaster(count.ToString(), Input.mousePosition, false);
        }
        Vector3 qidian = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 12f);
        Vector3 zhongdian = new Vector3(2f * Program.I().ocgcore.getScreenCenter() - Input.mousePosition.x, Input.mousePosition.y, 19f);
        int i_real = 0;
        for (int i = 0; i < Program.I().ocgcore.cards.Count; i++) if (Program.I().ocgcore.cards[i].gameObject.activeInHierarchy)
            {
                if ((Program.I().ocgcore.cards[i].p.location & (UInt32)location) > 0)
                {
                    if (Program.I().ocgcore.cards[i].p.controller == player)
                    {
                        if (Program.I().ocgcore.cards[i].isShowed == false)
                        {
                            Vector3 screen_vector_to_move = Vector3.zero;
                            int gezi = 8;
                            if (count_show > 8)
                            {
                                gezi = count_show;
                            }
                            int index = count_show - 1 - i_real;
                            i_real++;
                            screen_vector_to_move =
                                                (new Vector3(0, 50f * (float)Math.Sin(((float)index / (float)count)  * 3.1415926f), 0))
                                                +
                                                qidian
                                                +
                                                ((float)index / (float)(gezi - 1)) * (zhongdian - qidian);
                            //iTween.MoveTo(Program.I().ocgcore.cards[i].gameObject, Camera.main.ScreenToWorldPoint(screen_vector_to_move), 0.5f);
                            //iTween.RotateTo(Program.I().ocgcore.cards[i].gameObject, new Vector3(-30, 0, 0), 0.1f);
                            Program.I().ocgcore.cards[i].TweenTo(Camera.main.ScreenToWorldPoint(screen_vector_to_move), new Vector3(-30, 0, 0),true);
                        }
                    }
                }
            }
        if (count_show > 0)
        {
            Program.I().audio.clip = Program.I().zhankai;
            Program.I().audio.Play();
        }
    }
}
