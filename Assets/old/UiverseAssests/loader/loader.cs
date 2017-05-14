//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class loader : MonoBehaviour {
//    private CLIENT client;
//    public Camera main_camera;
//    public GameObject mod_ui_2d;
//    public GameObject mod_ui_3d;
//    public GameObject mod_setting;
//    public GameObject mod_ocgcore_button_mp;
//    public GameObject mod_ocgcore_button_bp;
//    public GameObject mod_ocgcore_button_ep;
//    public GameObject mod_ocgcore_button_ok;
//    public GameObject mod_ocgcore_button_no;
//    public GameObject mod_ocgcore_button_change;
//    public GameObject mod_ocgcore_button_see;
//    public GameObject mod_ocgcore_button_sum;
//    public GameObject mod_ocgcore_button_spsum;
//    public GameObject mod_ocgcore_button_atk;
//    public GameObject mod_ocgcore_button_act;
//    public GameObject mod_ocgcore_button_set;
//    public GameObject mod_ocgcore_button_wait_red;
//    public GameObject mod_ocgcore_button_wait_green;
//    public GameObject mod_ocgcore_button_wait_blue;
//    public GameObject mod_ocgcore_button_wait_black;
//    public GameObject mod_ocgcore_explode_mp;
//    public GameObject mod_ocgcore_explode_selected;
//    public GameObject mod_ocgcore_explode_bp;
//    public GameObject mod_ocgcore_explode_ep;
//    public GameObject mod_ocgcore_explode_ok;
//    public GameObject mod_ocgcore_explode_no;
//    public GameObject mod_ocgcore_explode_change;
//    public GameObject mod_ocgcore_explode_see;
//    public GameObject mod_ocgcore_explode_sum;
//    public GameObject mod_ocgcore_explode_spsum;
//    public GameObject mod_ocgcore_explode_atk;
//    public GameObject mod_ocgcore_explode_act;
//    public GameObject mod_ocgcore_explode_set;
//    public GameObject mod_audio_effect;
//    public GameObject mod_ocgcore_card;
//    public GameObject mod_ocgcore_card_cloude;
//    public GameObject mod_ocgcore_overlay_light;
//    public GameObject mod_ocgcore_card_number_shower;
//    public GameObject mod_ocgcore_card_figure_line;
//    public GameObject mod_ocgcore_hidden_button;
//    public GameObject mod_ocgcore_coin;
//    public GameObject mod_ocgcore_dice;
//    public GameObject mod_simple_quad;
//    public GameObject mod_simple_ngui_background_texture;
//    public GameObject mod_simple_ngui_text;
//    public GameObject mod_ocgcore_idle_container;
//    public GameObject mod_ocgcore_ui_card_hint;
//    public GameObject mod_ocgcore_ui_health;
//    public GameObject mod_ocgcore_ui_select_chain;
//    public GameObject mod_ocgcore_ui_time_effect;
//    public GameObject mod_ui_debbger;
//    public GameObject mod_ocgcore_number;
//    public GameObject mod_ocgcore_select_positions;
//    public GameObject mod_ocgcore_search_cards;
//    public GameObject mod_ocgcore_select_common;
//    public GameObject mod_ocgcore_select_button_option;
//    public GameObject mod_ocgcore_select_text_option;
//    public GameObject mod_ocgcore_decoration_chain_selecting;
//    public GameObject mod_ocgcore_decoration_card_selected;
//    public GameObject mod_ocgcore_decoration_card_selecting;
//    public GameObject mod_ocgcore_decoration_card_active;
//    public GameObject mod_ocgcore_decoration_spsummon;
//    public GameObject mod_ocgcore_decoration_thunder;
//    public GameObject mod_ocgcore_decoration_cage;
//    public GameObject mod_ocgcore_decoration_cage_of_field;
//    public GameObject mod_ocgcore_decoration_monster_activated;
//    public GameObject mod_ocgcore_decoration_trap_activated;
//    public GameObject mod_ocgcore_decoration_magic_activated;
//    public GameObject mod_ocgcore_decoration_magic_zhuangbei;
//    public GameObject mod_ocgcore_decoration_removed;
//    public GameObject mod_ocgcore_decoration_tograve;
//    public GameObject mod_ocgcore_decoration_card_setted;
//    public GameObject mod_ocgcore_blood;
//    public GameObject mod_ocgcore_blood_screen;
//    public Texture2D Texture2D_card_back;
//    public GameObject mod_deck_manager_card_on_list;
//    public GameObject mod_deck_manager_main_bed;
//    public GameObject mod_deck_manager_max_bed;
//    public GameObject mod_deck_manager_card;
//    public GameObject mod_deck_manager_lflist;
//    public GameObject mod_deck_manager_effect;
//    public GameObject mod_ocgcore_bs_atk_decoration;
//    public GameObject mod_ocgcore_bs_atk_sign;
//    public GameObject mod_ocgcore_bs_atk_line_earth;
//    public GameObject mod_ocgcore_bs_atk_line_water;
//    public GameObject mod_ocgcore_bs_atk_line_fire;
//    public GameObject mod_ocgcore_bs_atk_line_wind;
//    public GameObject mod_ocgcore_bs_atk_line_dark;
//    public GameObject mod_ocgcore_bs_atk_line_light;
//    public GameObject mod_ocgcore_cs_chaining;
//    public GameObject mod_ocgcore_cs_end;
//    public GameObject mod_ocgcore_cs_bomb;
//    public GameObject mod_ocgcore_cs_negated;
//    public GameObject mod_ocgcore_ss_summon_earth;
//    public GameObject mod_ocgcore_ss_summon_water;
//    public GameObject mod_ocgcore_ss_summon_fire;
//    public GameObject mod_ocgcore_ss_summon_wind;
//    public GameObject mod_ocgcore_ss_summon_dark;
//    public GameObject mod_ocgcore_ss_summon_light;
//    public GameObject mod_ocgcore_ss_spsummon_normal;
//    public GameObject mod_ocgcore_ss_spsummon_ronghe;
//    public GameObject mod_ocgcore_ss_spsummon_tongtiao;
//    public GameObject mod_ocgcore_ss_spsummon_yishi;
//    public GameObject mod_ocgcore_ss_p_idle_effect;
//    public GameObject mod_ocgcore_ss_p_sum_effect;
//    public GameObject mod_ocgcore_ss_dark_hole;

//    public GameObject mod_room_window;
//    public GameObject mod_room_ui_button;

//    public GameObject mod_login_window;
//    public GameObject mod_regist_window;

//    public GameObject mod_dating_window;
//    public GameObject mod_room_in_dating_window;
//    public GameObject mod_dating_chat;

//    public GameObject mod_shangcheng;
//    public GameObject mod_shangcheng_queren;
//    public GameObject mod_shangping;
//    //最后做一下preload
//	void Start () {
//        client = new CLIENT(this);
//        ini_mods();
//	}
//    void ini_mods()
//    {
//        preload(mod_ocgcore_button_mp);
//        preload(mod_ocgcore_button_bp);
//        preload(mod_ocgcore_button_ep);
//        preload(mod_ocgcore_button_ok);
//        preload(mod_ocgcore_button_no);
//        preload(mod_ocgcore_button_change);
//        preload(mod_ocgcore_button_see);
//        preload(mod_ocgcore_button_sum);
//        preload(mod_ocgcore_button_spsum);
//        preload(mod_ocgcore_button_atk);
//        preload(mod_ocgcore_button_act);
//        preload(mod_ocgcore_button_set);
//        preload(mod_ocgcore_button_wait_red);
//        preload(mod_ocgcore_button_wait_green);
//        preload(mod_ocgcore_button_wait_blue);
//        preload(mod_ocgcore_button_wait_black);
//        preload(mod_ocgcore_explode_mp);
//        preload(mod_ocgcore_explode_selected);
//        preload(mod_ocgcore_explode_bp);
//        preload(mod_ocgcore_explode_ep);
//        preload(mod_ocgcore_explode_ok);
//        preload(mod_ocgcore_explode_no);
//        preload(mod_ocgcore_explode_change);
//        preload(mod_ocgcore_explode_see);
//        preload(mod_ocgcore_explode_sum);
//        preload(mod_ocgcore_explode_spsum);
//        preload(mod_ocgcore_explode_atk);
//        preload(mod_ocgcore_explode_act);
//        preload(mod_ocgcore_explode_set);
//        preload(mod_ocgcore_card);
//        preload(mod_ocgcore_card_cloude);
//        preload(mod_ocgcore_overlay_light);
//        preload(mod_ocgcore_card_number_shower);
//        preload(mod_ocgcore_card_figure_line);
//        preload(mod_ocgcore_hidden_button);
//        preload(mod_ocgcore_coin);
//        preload(mod_ocgcore_dice);
//        preload(mod_simple_quad);
//        preload(mod_simple_ngui_background_texture);
//        preload(mod_simple_ngui_text);
//        preload(mod_ocgcore_idle_container);
//        preload(mod_ocgcore_ui_card_hint);
//        preload(mod_ocgcore_ui_health);
//        preload(mod_ocgcore_ui_select_chain);
//        preload(mod_ocgcore_ui_time_effect);
//        preload(mod_ocgcore_number);
//        preload(mod_ocgcore_select_positions);
//        preload(mod_ocgcore_search_cards);
//        preload(mod_ocgcore_select_common);
//        preload(mod_ocgcore_select_button_option);
//        preload(mod_ocgcore_select_text_option);
//        preload(mod_ocgcore_decoration_chain_selecting);
//        preload(mod_ocgcore_decoration_card_selected);
//        preload(mod_ocgcore_decoration_card_selecting);
//        preload(mod_ocgcore_decoration_card_active);
//        preload(mod_ocgcore_decoration_spsummon);
//        preload(mod_ocgcore_decoration_thunder);
//        preload(mod_ocgcore_decoration_cage);
//        preload(mod_ocgcore_decoration_cage_of_field);
//        preload(mod_ocgcore_decoration_monster_activated);
//        preload(mod_ocgcore_decoration_trap_activated);
//        preload(mod_ocgcore_decoration_magic_activated);
//        preload(mod_ocgcore_decoration_magic_zhuangbei);
//        preload(mod_ocgcore_decoration_removed);
//        preload(mod_ocgcore_decoration_tograve);
//        preload(mod_ocgcore_decoration_card_setted);
//        preload(mod_ocgcore_blood);
//        preload(mod_ocgcore_blood_screen);
//        preload(mod_deck_manager_card_on_list);
//        preload(mod_deck_manager_main_bed);
//        preload(mod_deck_manager_max_bed);
//        preload(mod_deck_manager_card);
//        preload(mod_deck_manager_lflist);
//        preload(mod_deck_manager_effect);
//        preload(mod_ocgcore_bs_atk_decoration);
//        preload(mod_ocgcore_bs_atk_sign);
//        preload(mod_ocgcore_bs_atk_line_earth);
//        preload(mod_ocgcore_bs_atk_line_water);
//        preload(mod_ocgcore_bs_atk_line_fire);
//        preload(mod_ocgcore_bs_atk_line_wind);
//        preload(mod_ocgcore_bs_atk_line_dark);
//        preload(mod_ocgcore_bs_atk_line_light);
//        preload(mod_ocgcore_cs_chaining);
//        preload(mod_ocgcore_cs_end);
//        preload(mod_ocgcore_cs_bomb);
//        preload(mod_ocgcore_cs_negated);
//        preload(mod_ocgcore_ss_summon_earth);
//        preload(mod_ocgcore_ss_summon_water);
//        preload(mod_ocgcore_ss_summon_fire);
//        preload(mod_ocgcore_ss_summon_wind);
//        preload(mod_ocgcore_ss_summon_dark);
//        preload(mod_ocgcore_ss_summon_light);
//        preload(mod_ocgcore_ss_spsummon_normal);
//        preload(mod_ocgcore_ss_spsummon_ronghe);
//        preload(mod_ocgcore_ss_spsummon_tongtiao);
//        preload(mod_ocgcore_ss_spsummon_yishi);
//        preload(mod_ocgcore_ss_p_idle_effect);
//        preload(mod_ocgcore_ss_p_sum_effect);
//        preload(mod_ocgcore_ss_dark_hole);
//    }
//    private List<GameObject> GameObjects = new List<GameObject>();
//    void preload(GameObject g)
//    {
//        GameObject obj = GameObject.Instantiate(g) as GameObject;
//        obj.SetActive(false);
//        GameObjects.Add(obj);
//    }
//    bool first = true;
//	void Update () 
//    {
//        if (first)
//        {
//            first = false;
//            //QualitySettings.SetQualityLevel(5);
//        }
//        client.update();
//	}
//    void OnApplicationQuit()
//    {
//        Debug.Log("OnApplicationQuit");
//        client.is_running = false;
//        client.networkStream.Flush();
//        client.networkStream.Close();
//        client.networkStream.Dispose();
//        client.networkStream = null;
//        client.tcpClient.Close();
//        client.tcpClient = null;
        
//    }
//}
