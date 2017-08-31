/*
* playerop.cpp
*
*  Created on: 2010-12-22
*      Author: Argon
*/

#include "field.h"
#include "duel.h"
#include "effect.h"
#include "card.h"

#include <memory.h>
#include <algorithm>
#include <time.h>
#include <stack>
#include <vector>
#include <algorithm>
#include <iostream>
#include <string>

#include "interpreter.h"
#include "ocgapi.h"

#define USE_LUA

#ifdef USE_LUA
void field::set_card_to_lua_without_index(void* LL, card* pcard, duel* pduel, uint32 description, bool extraSetTableCall) {
	lua_State* L = (lua_State*)LL;
	lua_newtable(L);
	lua_pushstring(L, "id");
	lua_pushnumber(L, pcard->get_code());
	lua_settable(L, -3);
	//original id
	lua_pushstring(L, "original_id");
	if (pcard->data.alias) {
		int32 dif = pcard->data.code - pcard->data.alias;
		if (dif > -10 && dif < 10) {
			lua_pushinteger(L, pcard->data.alias);
		} else {
			lua_pushinteger(L, pcard->data.code);
		}
	} else {
		lua_pushinteger(L, pcard->data.code);
	}
	lua_settable(L, -3);

	//cardid, internal use only
	lua_pushstring(L, "cardid");
	lua_pushnumber(L, pcard->cardid);
	lua_settable(L, -3);
	lua_pushstring(L, "attack");
	lua_pushnumber(L, pcard->get_attack());
	lua_settable(L, -3);
	lua_pushstring(L, "defense");
	lua_pushnumber(L, pcard->get_defense());
	lua_settable(L, -3);
	lua_pushstring(L, "base_attack");
	lua_pushnumber(L, pcard->get_base_attack());
	lua_settable(L, -3);
	lua_pushstring(L, "base_defense");
	lua_pushnumber(L, pcard->get_base_defense());
	lua_settable(L, -3);
	lua_pushstring(L, "text_attack");
	lua_pushnumber(L, pcard->data.attack);
	lua_settable(L, -3);
	lua_pushstring(L, "text_defense");
	lua_pushnumber(L, pcard->data.defense);
	lua_settable(L, -3);
	lua_pushstring(L, "level");
	lua_pushnumber(L, pcard->get_level());
	lua_settable(L, -3);
	lua_pushstring(L, "base_level");
	lua_pushnumber(L, pcard->data.level);
	lua_settable(L, -3);
	lua_pushstring(L, "type");
	lua_pushnumber(L, pcard->get_type());
	lua_settable(L, -3);
	lua_pushstring(L, "race");
	lua_pushnumber(L, pcard->get_race());
	lua_settable(L, -3);
	lua_pushstring(L, "rank");
	lua_pushnumber(L, pcard->get_rank());
	lua_settable(L, -3);
	lua_pushstring(L, "attribute");
	lua_pushnumber(L, pcard->get_attribute());
	lua_settable(L, -3);
	lua_pushstring(L, "position");
	lua_pushnumber(L, pcard->current.position);
	lua_settable(L, -3);
	lua_pushstring(L, "setcode");
	lua_pushnumber(L, pcard->data.setcode);
	lua_settable(L, -3);
	lua_pushstring(L, "location");
	lua_pushnumber(L, pcard->current.location);
	lua_settable(L, -3);
	lua_pushstring(L, "owner");
	
	if (player[0].isAI) {
		lua_pushnumber(L, (pcard->owner == 0) ? 1 : 2);
	} else {
		lua_pushnumber(L, (pcard->owner == 0) ? 2 : 1);
	}
	lua_settable(L, -3);
	lua_pushstring(L, "status");
	lua_pushnumber(L, pcard->status);
	lua_settable(L, -3);

	lua_pushstring(L, "description");
	lua_pushnumber(L, description);
	lua_settable(L, -3);

	lua_pushstring(L, "previous_location");
	lua_pushnumber(L, pcard->previous.location);
	lua_settable(L, -3);

	lua_pushstring(L, "summon_type");
	lua_pushnumber(L, pcard->summon_info & 0xff00ffff);
	lua_settable(L, -3);

	lua_pushstring(L, "xyz_material_count");
	lua_pushnumber(L, pcard->xyz_materials.size());
	lua_settable(L, -3);

	lua_pushstring(L, "xyz_materials");
	lua_newtable(L);
	for (size_t w = 0; w<pcard->xyz_materials.size(); w++) {
		lua_pushnumber(L, w + 1);
		set_card_to_lua_without_index(L, pcard->xyz_materials[w], pduel);
		lua_settable(L, -3);
	}
	lua_settable(L, -3);

	lua_pushstring(L, "get_counter");
	lua_pushcfunction(L, interpreter::get_counter);
	lua_settable(L, -3);
	lua_pushstring(L, "is_affected_by");
	lua_pushcfunction(L, interpreter::is_affected_by);
	lua_settable(L, -3);

	lua_pushstring(L, "is_affectable_by_chain");
	lua_pushcfunction(L, interpreter::is_affectable_by_chain);
	lua_settable(L, -3);

	lua_pushstring(L, "can_be_targeted_by_chain");
	lua_pushcfunction(L, interpreter::can_be_targeted_by_chain);
	lua_settable(L, -3);
	/*
	lua_pushstring(L, "is_destructable_by_effect");
	lua_pushcfunction(L, interpreter::is_destructable_by_effect);
	lua_settable(L, -3);
	*/
	lua_pushstring(L, "get_equipped_cards");
	lua_pushcfunction(L, interpreter::get_equipped_cards);
	lua_settable(L, -3);

	lua_pushstring(L, "get_equip_target");
	lua_pushcfunction(L, interpreter::get_equip_target);
	lua_settable(L, -3);

	lua_pushstring(L, "is_public");
	lua_pushcfunction(L, interpreter::is_public_card);
	lua_settable(L, -3);

	lua_pushstring(L, "lscale");
	lua_pushnumber(L, pcard->get_lscale());
	lua_settable(L, -3);

	lua_pushstring(L, "rscale");
	lua_pushnumber(L, pcard->get_rscale());
	lua_settable(L, -3);

	lua_pushstring(L, "equip_count");
	lua_pushnumber(L, pcard->equiping_cards.size());
	lua_settable(L, -3);

	lua_pushstring(L, "turnid");
	lua_pushnumber(L, pcard->turnid);
	lua_settable(L, -3);

	uint32 extrac = 0;
	effect* peffect;
	if ((peffect = pcard->is_affected_by_effect(EFFECT_EXTRA_ATTACK)))
		extrac = peffect->get_value(pcard);

	lua_pushstring(L, "extra_attack_count");
	lua_pushnumber(L, extrac);
	lua_settable(L, -3);

	if (extraSetTableCall)
		lua_settable(L, -3);
}
void field::set_card_to_lua(void* LL, card* pcard, int i, uint32 description) {
	lua_State* L = (lua_State *)LL;
	lua_pushnumber(L, i);
	lua_newtable(L);
	lua_pushstring(L, "id");
	lua_pushnumber(L, pcard->get_code());
	lua_settable(L, -3);
	//original id
	lua_pushstring(L, "original_id");
	if (pcard->data.alias) {
		int32 dif = pcard->data.code - pcard->data.alias;
		if (dif > -10 && dif < 10) {
			lua_pushinteger(L, pcard->data.alias);
		} else {
			lua_pushinteger(L, pcard->data.code);
		}
	} else {
		lua_pushinteger(L, pcard->data.code);
	}
	lua_settable(L, -3);

	//cardid, internal use only
	lua_pushstring(L, "cardid");
	lua_pushnumber(L, pcard->cardid);
	lua_settable(L, -3);
	lua_pushstring(L, "attack");
	lua_pushnumber(L, pcard->get_attack());
	lua_settable(L, -3);
	lua_pushstring(L, "defense");
	lua_pushnumber(L, pcard->get_defense());
	lua_settable(L, -3);
	lua_pushstring(L, "base_attack");
	lua_pushnumber(L, pcard->get_base_attack());
	lua_settable(L, -3);
	lua_pushstring(L, "base_defense");
	lua_pushnumber(L, pcard->get_base_defense());
	lua_settable(L, -3);
	lua_pushstring(L, "text_attack");
	lua_pushnumber(L, pcard->data.attack);
	lua_settable(L, -3);
	lua_pushstring(L, "text_defense");
	lua_pushnumber(L, pcard->data.defense);
	lua_settable(L, -3);
	lua_pushstring(L, "level");
	lua_pushnumber(L, pcard->get_level());
	lua_settable(L, -3);
	lua_pushstring(L, "base_level");
	lua_pushnumber(L, pcard->data.level);
	lua_settable(L, -3);
	lua_pushstring(L, "type");
	lua_pushnumber(L, pcard->get_type());
	lua_settable(L, -3);
	lua_pushstring(L, "race");
	lua_pushnumber(L, pcard->get_race());
	lua_settable(L, -3);
	lua_pushstring(L, "rank");
	lua_pushnumber(L, pcard->get_rank());
	lua_settable(L, -3);
	lua_pushstring(L, "attribute");
	lua_pushnumber(L, pcard->get_attribute());
	lua_settable(L, -3);
	lua_pushstring(L, "position");
	lua_pushnumber(L, pcard->current.position);
	lua_settable(L, -3);
	lua_pushstring(L, "setcode");
	lua_pushnumber(L, pcard->data.setcode);
	lua_settable(L, -3);
	lua_pushstring(L, "location");
	lua_pushnumber(L, pcard->current.location);
	lua_settable(L, -3);
	lua_pushstring(L, "owner");
	//lua_pushnumber(L,(pcard->owner == 0) ? 2:1); //1 = ai, 2 = player	
	if (player[0].isAI) {
		lua_pushnumber(L, (pcard->owner == 0) ? 1 : 2);
	} else {
		lua_pushnumber(L, (pcard->owner == 0) ? 2 : 1);
	}
	lua_settable(L, -3);

	lua_pushstring(L, "status");
	lua_pushnumber(L, pcard->status);
	lua_settable(L, -3);

	lua_pushstring(L, "description");
	lua_pushnumber(L, description);
	lua_settable(L, -3);

	lua_pushstring(L, "previous_location");
	lua_pushnumber(L, pcard->previous.location);
	lua_settable(L, -3);

	lua_pushstring(L, "summon_type");
	lua_pushnumber(L, pcard->summon_info & 0xff00ffff);
	lua_settable(L, -3);

	lua_pushstring(L, "xyz_material_count");
	lua_pushnumber(L, pcard->xyz_materials.size());
	lua_settable(L, -3);

	lua_pushstring(L, "xyz_materials");
	lua_newtable(L);
	for (size_t w = 0; w<pcard->xyz_materials.size(); w++) {
		set_card_to_lua(L, pcard->xyz_materials[w], w + 1);
	}
	lua_settable(L, -3);

	lua_pushstring(L, "get_counter");
	lua_pushcfunction(L, interpreter::get_counter);
	lua_settable(L, -3);
	lua_pushstring(L, "is_affected_by");
	lua_pushcfunction(L, interpreter::is_affected_by);
	lua_settable(L, -3);

	lua_pushstring(L, "is_affectable_by_chain");
	lua_pushcfunction(L, interpreter::is_affectable_by_chain);
	lua_settable(L, -3);

	lua_pushstring(L, "can_be_targeted_by_chain");
	lua_pushcfunction(L, interpreter::can_be_targeted_by_chain);
	lua_settable(L, -3);
	/*
	lua_pushstring(L, "is_destructable_by_effect");
	lua_pushcfunction(L, interpreter::is_destructable_by_effect);
	lua_settable(L, -3);
	*/
	lua_pushstring(L, "get_equipped_cards");
	lua_pushcfunction(L, interpreter::get_equipped_cards);
	lua_settable(L, -3);

	lua_pushstring(L, "get_equip_target");
	lua_pushcfunction(L, interpreter::get_equip_target);
	lua_settable(L, -3);

	lua_pushstring(L, "is_public");
	lua_pushcfunction(L, interpreter::is_public_card);
	lua_settable(L, -3);

	lua_pushstring(L, "lscale");
	lua_pushnumber(L, pcard->get_lscale());
	lua_settable(L, -3);

	lua_pushstring(L, "rscale");
	lua_pushnumber(L, pcard->get_rscale());
	lua_settable(L, -3);

	lua_pushstring(L, "equip_count");
	lua_pushnumber(L, pcard->equiping_cards.size());
	lua_settable(L, -3);

	lua_pushstring(L, "turnid");
	lua_pushnumber(L, pcard->turnid);
	lua_settable(L, -3);

	uint32 extrac = 0;
	effect* peffect;
	if ((peffect = pcard->is_affected_by_effect(EFFECT_EXTRA_ATTACK)))
		extrac = peffect->get_value(pcard);

	lua_pushstring(L, "extra_attack_count");
	lua_pushnumber(L, extrac);
	lua_settable(L, -3);

	lua_settable(L, -3);
}
#endif

int32 field::select_battle_command(uint16 step, uint8 playerid) {
#ifdef DEBUG_PRINTS
	puts("\nselect_battle_command");
#endif
	if (step == 0) {
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
#ifdef USE_LUA
			lua_State* L = pduel->lua->lua_state;
			lua_getglobal(L, "OnSelectBattleCommand");

			lua_newtable(L);
			for (size_t i = 0; i<core.attackable_cards.size(); i++) {
				set_card_to_lua(L, core.attackable_cards[i], i + 1);
			}

			effect* peffect;
			lua_newtable(L);
			for (size_t i = 0; i<core.select_chains.size(); i++) {
				peffect = core.select_chains[i].triggering_effect;
				set_card_to_lua(L, core.select_chains[i].triggering_effect->handler, i + 1, peffect->description);
			}

			if (lua_pcall(L, 2, 2, 0) != 0) {
				sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
				handle_message(pduel, 1);
				returns.ivalue[0] = -1;
				return TRUE;
			}

			//lua_call(L, 1, 2);
			int attackerIndex = (int)lua_tointeger(L, -1);
			lua_pop(L, 1);
			int command = (int)lua_tointeger(L, -1);
			lua_pop(L, 1);

			if (command == 1) {
				returns.ivalue[0] = 1;
				returns.bvalue[0] = 1;
				returns.bvalue[1] = 0;
				returns.bvalue[2] = attackerIndex - 1;
			} else if (command == 2) {
				returns.ivalue[0] = 0;
				returns.bvalue[0] = 0;
				returns.bvalue[1] = 0;
				returns.bvalue[2] = attackerIndex - 1;
			} else {
				returns.ivalue[0] = -1;
			}
			return TRUE;
#endif
		} else {
			pduel->write_buffer8(MSG_SELECT_BATTLECMD);
			pduel->write_buffer8(playerid);
			uint32 i;
			card* pcard;
			effect* peffect;
			//Activatable
			pduel->write_buffer8(core.select_chains.size());
			std::sort(core.select_chains.begin(), core.select_chains.end(), chain::chain_operation_sort);
			for (i = 0; i < core.select_chains.size(); ++i) {
				peffect = core.select_chains[i].triggering_effect;
				pcard = peffect->get_handler();
				pduel->write_buffer32(pcard->data.code);
				pduel->write_buffer8(pcard->current.controler);
				pduel->write_buffer8(pcard->current.location);
				pduel->write_buffer8(pcard->current.sequence);
				pduel->write_buffer32(peffect->description);
			}
			//Attackable
			pduel->write_buffer8(core.attackable_cards.size());
			for (i = 0; i < core.attackable_cards.size(); ++i) {
				pcard = core.attackable_cards[i];
				pduel->write_buffer32(pcard->data.code);
				pduel->write_buffer8(pcard->current.controler);
				pduel->write_buffer8(pcard->current.location);
				pduel->write_buffer8(pcard->current.sequence);
				pduel->write_buffer8(pcard->direct_attackable);
			}
			//M2, EP
			if (core.to_m2)
				pduel->write_buffer8(1);
			else
				pduel->write_buffer8(0);
			if (core.to_ep)
				pduel->write_buffer8(1);
			else
				pduel->write_buffer8(0);
			return FALSE;
		}
	} else {
		uint32 t = returns.ivalue[0] & 0xffff;
		uint32 s = returns.ivalue[0] >> 16;
		if ((int)t < 0 || t > 3 || (int)s < 0
			|| (t == 0 && s >= core.select_chains.size())
			|| (t == 1 && s >= core.attackable_cards.size())
			|| (t == 2 && !core.to_m2)
			|| (t == 3 && !core.to_ep)) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		return TRUE;
	}
}

/*
returns.ivalue[0]

-1 = change phase
0 = normal summon
1 = special summon (xyz)
2 = change to def
3 = set monster
4 = set s/t
5 = activate s/t
6 = change phase
7 = to end phase
8 = change phase
9 = change phase
10 = change phase
*/
int32 field::select_idle_command(uint16 step, uint8 playerid) {
#ifdef DEBUG_PRINTS
	puts("select_idle_command");
#endif
	if (step == 0) {
		uint32 i;
		card* pcard;
		effect* peffect;

		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
#ifdef USE_LUA
			lua_State* L = pduel->lua->lua_state;
			lua_getglobal(L, "OnSelectInitCommand");

			//1 start of mega table
			lua_newtable(L);
			lua_pushstring(L, "summonable_cards");
			lua_newtable(L);
			for (size_t i = 0; i<core.summonable_cards.size(); i++) {
				set_card_to_lua(L, core.summonable_cards[i], i + 1);
			}
			lua_settable(L, -3);

			lua_pushstring(L, "spsummonable_cards");
			lua_newtable(L);
			for (size_t i = 0; i<core.spsummonable_cards.size(); i++) {
				set_card_to_lua(L, core.spsummonable_cards[i], i + 1);
			}
			lua_settable(L, -3);

			lua_pushstring(L, "repositionable_cards");
			lua_newtable(L);
			for (size_t i = 0; i<core.repositionable_cards.size(); i++) {
				set_card_to_lua(L, core.repositionable_cards[i], i + 1);
			}
			lua_settable(L, -3);

			lua_pushstring(L, "monster_setable_cards");
			lua_newtable(L);
			for (size_t i = 0; i<core.msetable_cards.size(); i++) {
				set_card_to_lua(L, core.msetable_cards[i], i + 1);
			}
			lua_settable(L, -3);

			lua_pushstring(L, "st_setable_cards");
			lua_newtable(L);
			for (size_t i = 0; i<core.ssetable_cards.size(); i++) {
				set_card_to_lua(L, core.ssetable_cards[i], i + 1);
			}
			lua_settable(L, -3);

			lua_pushstring(L, "activatable_cards");
			lua_newtable(L);
			std::sort(core.select_chains.begin(), core.select_chains.end(), chain::chain_operation_sort);
			for (size_t i = 0; i<core.select_chains.size(); i++) {
				peffect = core.select_chains[i].triggering_effect;
				set_card_to_lua(L, core.select_chains[i].triggering_effect->handler, i + 1, peffect->description);
			}
			lua_settable(L, -3);

			//end of mega table

			//2
			bool tobp = false;
			if (infos.phase == PHASE_MAIN1 && core.to_bp) {
				tobp = true;
			}
			lua_pushboolean(L, tobp);
			//3
			bool toep = false;
			if (core.to_ep) {
				toep = true;
			}
			lua_pushboolean(L, toep);

			if (lua_pcall(L, 3, 2, 0) != 0) {
				sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
				handle_message(pduel, 1);

				//check if you can go to bp
				if (infos.phase == PHASE_MAIN1) {
					if (infos.turn_id <= 1 || core.to_bp == false) {
						//you cannot enter battle phase in turn 1, end turn
						returns.ivalue[0] = 7;
					} else {
						//go to next phase
						returns.ivalue[0] = 6;
					}
				} else {
					//go to next phase
					returns.ivalue[0] = 6;
				}
				returns.bvalue[2] = 0;
				return TRUE;
			}
			//lua_call(L, 3, 2);
			size_t index = (int)lua_tointeger(L, -1);
			lua_pop(L, 1);
			int command = (int)lua_tointeger(L, -1);
			lua_pop(L, 1);

			if (command > -1) {
				//error checking
				size_t index_range = 0;
				switch (command) {
				case 0://summon
					index_range = core.summonable_cards.size();
					break;
				case 1://special summon
					index_range = core.spsummonable_cards.size();
					break;
				case 2://change position
					index_range = core.repositionable_cards.size();
					break;
				case 3://set monster
					index_range = core.msetable_cards.size();
					break;
				case 4://set s/t
					index_range = core.ssetable_cards.size();
					break;
				case 5://activate
					index_range = core.select_chains.size();
					break;
				default://to next phase or to end phase
					index = 1;
					index_range = 1;
					break;
				}
				if (index_range == 0) {
#ifdef AI_DEBUG_BUILD
					sprintf(pduel->strbuffer, "\"[AI]OnSelectInitCommand\": cannot execute command: %d", command);
					handle_message(pduel, 1);
#endif
					command = 6; //selected a command that cannot be executed, change command to next phase
				}
				else if (index <= 0) {
#ifdef AI_DEBUG_BUILD
					sprintf(pduel->strbuffer, "\"[AI]OnSelectInitCommand\": invalid index (%d received) for command %d", index, command);
					handle_message(pduel, 1);
#endif
					index = 1;
				}
				else if (index > index_range) {
#ifdef AI_DEBUG_BUILD
					sprintf(pduel->strbuffer, "\"[AI]OnSelectInitCommand\": invalid index (max %d expected, %d received) for command %d", index_range, index, command);
					handle_message(pduel, 1);
#endif
					index = 1;
				}

				//check if to bp
				if (command == 6) {
					if (infos.phase == PHASE_MAIN1 && core.to_bp == false) {
						command = 7;
					}
					if (infos.turn_id <= 1) {
						//you cannot enter battle phase in turn 1
						command = 7;
					}
				}

				returns.ivalue[0] = command;
				returns.bvalue[2] = index - 1;
				
				return TRUE;
			} else {
				returns.ivalue[0] = -1;
				return TRUE;
			}
#endif
		} else {
			pduel->write_buffer8(MSG_SELECT_IDLECMD);
			pduel->write_buffer8(playerid);
			//idle summon
			pduel->write_buffer8(core.summonable_cards.size());
			for (i = 0; i < core.summonable_cards.size(); ++i) {
				pcard = core.summonable_cards[i];
				pduel->write_buffer32(pcard->data.code);
				pduel->write_buffer8(pcard->current.controler);
				pduel->write_buffer8(pcard->current.location);
				pduel->write_buffer8(pcard->current.sequence);
			}
			//idle spsummon
			pduel->write_buffer8(core.spsummonable_cards.size());
			for (i = 0; i < core.spsummonable_cards.size(); ++i) {
				pcard = core.spsummonable_cards[i];
				pduel->write_buffer32(pcard->data.code);
				pduel->write_buffer8(pcard->current.controler);
				pduel->write_buffer8(pcard->current.location);
				pduel->write_buffer8(pcard->current.sequence);
			}
			//idle pos change
			pduel->write_buffer8(core.repositionable_cards.size());
			for (i = 0; i < core.repositionable_cards.size(); ++i) {
				pcard = core.repositionable_cards[i];
				pduel->write_buffer32(pcard->data.code);
				pduel->write_buffer8(pcard->current.controler);
				pduel->write_buffer8(pcard->current.location);
				pduel->write_buffer8(pcard->current.sequence);
			}
			//idle mset
			pduel->write_buffer8(core.msetable_cards.size());
			for (i = 0; i < core.msetable_cards.size(); ++i) {
				pcard = core.msetable_cards[i];
				pduel->write_buffer32(pcard->data.code);
				pduel->write_buffer8(pcard->current.controler);
				pduel->write_buffer8(pcard->current.location);
				pduel->write_buffer8(pcard->current.sequence);
			}
			//idle sset
			pduel->write_buffer8(core.ssetable_cards.size());
			for (i = 0; i < core.ssetable_cards.size(); ++i) {
				pcard = core.ssetable_cards[i];
				pduel->write_buffer32(pcard->data.code);
				pduel->write_buffer8(pcard->current.controler);
				pduel->write_buffer8(pcard->current.location);
				pduel->write_buffer8(pcard->current.sequence);
			}
			//idle activate
			pduel->write_buffer8(core.select_chains.size());
			std::sort(core.select_chains.begin(), core.select_chains.end(), chain::chain_operation_sort);
			for (i = 0; i < core.select_chains.size(); ++i) {
				peffect = core.select_chains[i].triggering_effect;
				pcard = peffect->get_handler();
				pduel->write_buffer32(pcard->data.code);
				pduel->write_buffer8(pcard->current.controler);
				pduel->write_buffer8(pcard->current.location);
				pduel->write_buffer8(pcard->current.sequence);
				pduel->write_buffer32(peffect->description);
			}
			//To BP
			if (infos.phase == PHASE_MAIN1 && core.to_bp)
				pduel->write_buffer8(1);
			else
				pduel->write_buffer8(0);
			if (core.to_ep)
				pduel->write_buffer8(1);
			else
				pduel->write_buffer8(0);
			if (infos.can_shuffle && player[playerid].list_hand.size() > 1)
				pduel->write_buffer8(1);
			else
				pduel->write_buffer8(0);
			return FALSE;
		}
	} else {
		uint32 t = returns.ivalue[0] & 0xffff;
		uint32 s = returns.ivalue[0] >> 16;
		if ((int)t < 0 || t > 8 || (int)s < 0
			|| (t == 0 && s >= core.summonable_cards.size())
			|| (t == 1 && s >= core.spsummonable_cards.size())
			|| (t == 2 && s >= core.repositionable_cards.size())
			|| (t == 3 && s >= core.msetable_cards.size())
			|| (t == 4 && s >= core.ssetable_cards.size())
			|| (t == 5 && s >= core.select_chains.size())
			|| (t == 6 && (infos.phase != PHASE_MAIN1 || !core.to_bp))
			|| (t == 7 && !core.to_ep)
			|| (t == 8 && !(infos.can_shuffle && player[playerid].list_hand.size() > 1))) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		return TRUE;
	}
}
int32 field::select_effect_yes_no(uint16 step, uint8 playerid, uint32 description, card* pcard) {
#ifdef DEBUG_PRINTS
	puts("select_effect_yes_no");
#endif
	if (step == 0) {
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
#ifdef USE_LUA
			lua_State* L = pduel->lua->lua_state;
			lua_getglobal(L, "OnSelectEffectYesNo");

			/* Parameter 1: id */
			lua_pushnumber(L, pcard->data.code);

			/* Parameter 2: triggeringCard */
			bool hasDescription = false;
			if (core.new_ochain_s.size() > 0) {
				auto clit = core.new_ochain_s.begin();
				effect* peffect = clit->triggering_effect;
				if (peffect) {
					hasDescription = true;
					set_card_to_lua_without_index(L, pcard, interpreter::get_duel_info(L), peffect->description);
				}
			} else if (core.select_chains.size() > 0) {
				auto clit = core.select_chains.begin();
				effect* peffect = clit->triggering_effect;
				if (peffect) {
					hasDescription = true;
					set_card_to_lua_without_index(L, pcard, interpreter::get_duel_info(L), peffect->description);
				}
			}
			if (!hasDescription) {
				set_card_to_lua_without_index(L, pcard, interpreter::get_duel_info(L));
			}

			if (lua_pcall(L, 2, 1, 0) != 0) {
				sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
				handle_message(pduel, 1);
				returns.ivalue[0] = 0;
				return TRUE;
			}
			//lua_call(L, 1, 1);
			int result = (int)lua_tointeger(L, -1);
			lua_pop(L, 1);
			returns.ivalue[0] = result;

			return TRUE;
#endif
		} else if ((playerid == 1) && (core.duel_options & DUEL_SIMPLE_AI)) {
			returns.ivalue[0] = 1;
			return TRUE;
		}
		pduel->write_buffer8(MSG_SELECT_EFFECTYN);
		pduel->write_buffer8(playerid);
		pduel->write_buffer32(pcard->data.code);
		pduel->write_buffer32(pcard->get_info_location());
		pduel->write_buffer32(description);
		returns.ivalue[0] = -1;
		return FALSE;
	} else {
		if (returns.ivalue[0] != 0 && returns.ivalue[0] != 1) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		return TRUE;
	}
}
int32 field::select_yes_no(uint16 step, uint8 playerid, uint32 description) {
#ifdef DEBUG_PRINTS
	puts("select_yes_no");
#endif
	if (step == 0) {
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
#ifdef USE_LUA
			lua_State* L = pduel->lua->lua_state;
			lua_getglobal(L, "OnSelectYesNo");
			lua_pushnumber(L, description);

			if (lua_pcall(L, 1, 1, 0) != 0) {
				sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
				handle_message(pduel, 1);
				returns.ivalue[0] = 0;
				return TRUE;
			}
			//lua_call(L, 1, 1);
			int result = (int)lua_tointeger(L, -1);
			lua_pop(L, 1);
			returns.ivalue[0] = result;

			return TRUE;
#endif
		}
		else if ((playerid == 1) && (core.duel_options & DUEL_SIMPLE_AI)) {
			returns.ivalue[0] = 1;
			return TRUE;
		}
		pduel->write_buffer8(MSG_SELECT_YESNO);
		pduel->write_buffer8(playerid);
		pduel->write_buffer32(description);
		returns.ivalue[0] = -1;
		return FALSE;
	} else {
		if (returns.ivalue[0] != 0 && returns.ivalue[0] != 1) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		return TRUE;
	}
}
int32 field::select_option(uint16 step, uint8 playerid) {
#ifdef DEBUG_PRINTS
	puts("select_option");
#endif
	if (step == 0) {
		returns.ivalue[0] = -1;
		if (core.select_options.size() == 0)
			return TRUE;
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
#ifdef USE_LUA
			lua_State* L = pduel->lua->lua_state;
			lua_getglobal(L, "OnSelectOption");
			lua_newtable(L);
			std::vector<uint32> opt = core.select_options;

			for (size_t i = 0; i<opt.size(); i++) {
				lua_pushinteger(L, i + 1); //lua indices start at 1
				lua_pushinteger(L, opt[i]);
				lua_settable(L, -3);
			}
			if (lua_pcall(L, 1, 1, 0) != 0) {
				sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
				handle_message(pduel, 1);
				returns.ivalue[0] = 0;
				return TRUE;
			}
			//lua_call(L, 1, 1);
			int result = (int)lua_tointeger(L, -1);
			lua_pop(L, 1);
			returns.ivalue[0] = result - 1;
			return TRUE;
#endif
		} else if ((playerid == 1) && (core.duel_options & DUEL_SIMPLE_AI)) {
			returns.ivalue[0] = 0;
			return TRUE;
		}
		pduel->write_buffer8(MSG_SELECT_OPTION);
		pduel->write_buffer8(playerid);
		pduel->write_buffer8(core.select_options.size());
		for (uint32 i = 0; i < core.select_options.size(); ++i)
			pduel->write_buffer32(core.select_options[i]);
		return FALSE;
	} else {
		if (returns.ivalue[0] < 0 || returns.ivalue[0] >= (int32)core.select_options.size()) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		return TRUE;
	}
}
int32 field::select_card(uint16 step, uint8 playerid, uint8 cancelable, uint8 min, uint8 max) {
#ifdef DEBUG_PRINTS
	puts("select_card");
#endif
	if (step == 0) {
		returns.bvalue[0] = 0;
		if (max == 0 || core.select_cards.empty())
			return TRUE;
		if (max > 63)
			max = 63;
		if (max > core.select_cards.size())
			max = core.select_cards.size();
		if (min > max)
			min = max;
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
#ifdef USE_LUA
			lua_State* L = pduel->lua->lua_state;
			lua_getglobal(L, "OnSelectCard");

			lua_newtable(L);
			for (size_t i = 0; i<core.select_cards.size(); i++) {
				set_card_to_lua(L, core.select_cards[i], i + 1);
			}

			/* the 2nd argument */
			lua_pushnumber(L, min);
			/* the 3rd argument */
			lua_pushnumber(L, max);

			uint32 triggeringcode = 0;
			card* lastCardInChain = NULL;
			if (core.current_chain.size() > 0) {
				effect* e = core.current_chain[core.current_chain.size() - 1].triggering_effect;
				if (e) {
					lastCardInChain = e->owner;
					if (lastCardInChain) {
						triggeringcode = lastCardInChain->get_code();
					}
				}
			}

			/* the 4th argument */
			lua_pushnumber(L, triggeringcode);

			/* the 5th argument */
			if (triggeringcode > 0 && lastCardInChain != NULL) {
				if (lastCardInChain) {
					set_card_to_lua_without_index(L, lastCardInChain, interpreter::get_duel_info(L));
				} else {
					lua_pushboolean(L, false); //set a false
				}
			} else {
				lua_pushboolean(L, false); //set a false
			}

			if (lua_pcall(L, 5, 1, 0) != 0) {
				sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
				handle_message(pduel, 1);
				returns.ivalue[0] = 0;
				return TRUE;
			}
			//lua_call(L, 4, 1);

			//1st = table
			int len = luaL_len(L, -1);

			//First check if the returned target count is correct
			if (len < min || len > max) {
				sprintf(pduel->strbuffer, "\"[AI]OnSelectCard\": incorrect count (min %d, max %d expected, %d received)", min, max, len);
				handle_message(pduel, 1);
				//sprintf(pduel->strbuffer, "\"[AI]OnSelectCard\": enforcing correct targets: selecting %d card(s)", min);
				//handle_message(pduel, 1);

				//empty the return stack
				for (int j = 0; j < len; j++) {
					lua_pushinteger(L, j + 1);
					lua_gettable(L, -2);
					lua_pop(L, 1);
				}
				lua_pop(L, 1);//pop the table off the stack

				//enforce the correct min targets
				returns.bvalue[0] = min;
				for (int i = 0; i < min; i++) {
					returns.bvalue[i + 1] = i;
				}
			} else {
				if (len == 1) {
					returns.bvalue[0] = len; //set number of tagets
					lua_pushinteger(L, 1);
					lua_gettable(L, -2);
					size_t temp = lua_tonumber(L, -1);
					if (temp <= 0 || temp > core.select_cards.size()) {
						sprintf(pduel->strbuffer, "\"[AI]OnSelectCard\": incorrect target index (max index %d expected, %d received)", core.select_cards.size(), temp);
						handle_message(pduel, 1);
						temp = 1;
					}
					returns.bvalue[1] = temp - 1; //set target index
					lua_pop(L, 1);
					lua_pop(L, 1);//pop the table off the stack
				} else {
					returns.bvalue[0] = len; //set number of tagets
					for (int i = 0; i < len; i++) {
						lua_pushinteger(L, i + 1);
						lua_gettable(L, -2);
						int temp = lua_tonumber(L, -1);
						returns.bvalue[i + 1] = temp - 1; //set target index
						lua_pop(L, 1);
					}
					lua_pop(L, 1);//pop the table off the stack
				}
			}

			return TRUE;
#endif
		}
		else if ((playerid == 1) && (core.duel_options & DUEL_SIMPLE_AI)) {
			returns.bvalue[0] = min;
			for (uint8 i = 0; i < min; ++i)
				returns.bvalue[i + 1] = i;
			return TRUE;
		}
		core.units.begin()->arg2 = ((uint32)min) + (((uint32)max) << 16);
		pduel->write_buffer8(MSG_SELECT_CARD);
		pduel->write_buffer8(playerid);
		pduel->write_buffer8(cancelable);
		pduel->write_buffer8(min);
		pduel->write_buffer8(max);
		pduel->write_buffer8(core.select_cards.size());
		card* pcard;
		std::sort(core.select_cards.begin(), core.select_cards.end(), card::card_operation_sort);
		for (uint32 i = 0; i < core.select_cards.size(); ++i) {
			pcard = core.select_cards[i];
			pduel->write_buffer32(pcard->data.code);
			pduel->write_buffer32(pcard->get_info_location());
		}
		return FALSE;
	}
	else {
		if (cancelable && returns.ivalue[0] == -1)
			return TRUE;
		byte c[64];
		memset(c, 0, 64);
		if (returns.bvalue[0] < min || returns.bvalue[0] > max) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		uint8 m = core.select_cards.size(), v = 0;
		for (int32 i = 0; i < returns.bvalue[0]; ++i) {
			v = returns.bvalue[i + 1];
			if (v < 0 || v >= m || v >= 63 || c[v]) {
				pduel->write_buffer8(MSG_RETRY);
				return FALSE;
			}
			c[v] = 1;
		}
		return TRUE;
	}
}
int32 field::select_chain(uint16 step, uint8 playerid, uint8 spe_count, uint8 forced) {
#ifdef DEBUG_PRINTS	
	printf("select_chain player=%d, spe_count=%d, forced=%d\n", playerid, spe_count, forced);
#endif
	if (step == 0) {
		returns.ivalue[0] = -1;
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
#ifdef USE_LUA
			lua_State* L = pduel->lua->lua_state;
			lua_getglobal(L, "OnSelectChain");

			lua_newtable(L);
			size_t chainsize = core.select_chains.size();
			for (size_t i = 0; i<chainsize; i++) {
				effect* peffect = core.select_chains[i].triggering_effect;
				if (peffect) {
					set_card_to_lua(L, core.select_chains[i].triggering_effect->owner, i + 1, peffect->description);
				}
				else {
					set_card_to_lua(L, core.select_chains[i].triggering_effect->owner, i + 1);
				}
			}

			bool only_chains_by_player = true;
			//bool last_chain_by_player = core.current_chain.end()->triggering_player == 0;
			for (auto chit = core.current_chain.begin(); chit != core.current_chain.end(); ++chit)
				if (chit->triggering_player == playerid)
					only_chains_by_player = false;
			lua_pushboolean(L, (only_chains_by_player == true ? 1 : 0));

			/* the third argument */
			lua_pushnumber(L, forced);

			if (lua_pcall(L, 3, 2, 0) != 0) {
				sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
				handle_message(pduel, 1);
				return TRUE;
			}
			//lua_call(L, 2, 2);
			size_t chainIndex = (int)lua_tointeger(L, -1);
			lua_pop(L, 1);
			int shouldChain = (int)lua_tointeger(L, -1);
			lua_pop(L, 1);

			if (shouldChain > 0) {
				if (chainIndex > 0 && chainsize == 0) {
					//sprintf(pduel->strbuffer, "\"[AI]OnSelectChain\": received command to chain but there is no card to chain", chainsize, chainIndex);
					//handle_message(pduel, 1);
					chainIndex = 1;
				} else if (chainIndex <= 0 || chainIndex > chainsize) {
					sprintf(pduel->strbuffer, "\"[AI]OnSelectChain\": incorrect chain index (max index %d expected, %d received)", chainsize, chainIndex);
					handle_message(pduel, 1);
					chainIndex = 1;
				}
				returns.ivalue[0] = 0;
				returns.bvalue[0] = chainIndex - 1;
			} else {
				if (forced > 0) {
					returns.ivalue[0] = 0;
					returns.bvalue[0] = 0;
				} else {
					returns.ivalue[0] = -1;
				}
			}

			return TRUE;
#endif	
		}
		else if ((playerid == 1) && (core.duel_options & DUEL_SIMPLE_AI)) {
			if (core.select_chains.size() == 0)
				returns.ivalue[0] = -1;
			else {
				bool act = true;
				for (auto chit = core.current_chain.begin(); chit != core.current_chain.end(); ++chit)
					if (chit->triggering_player == 1)
						act = false;
				if (act)
					returns.ivalue[0] = 0;
				else
					returns.ivalue[0] = -1;
			}
			return TRUE;
		}
		pduel->write_buffer8(MSG_SELECT_CHAIN);
		pduel->write_buffer8(playerid);
		pduel->write_buffer8(core.select_chains.size());
		pduel->write_buffer8(spe_count);
		pduel->write_buffer8(forced);
		pduel->write_buffer32(pduel->game_field->core.hint_timing[playerid]);
		pduel->write_buffer32(pduel->game_field->core.hint_timing[1 - playerid]);
		std::sort(core.select_chains.begin(), core.select_chains.end(), chain::chain_operation_sort);
		for (uint32 i = 0; i < core.select_chains.size(); ++i) {
			effect* peffect = core.select_chains[i].triggering_effect;
			card* pcard = peffect->get_handler();
			if (peffect->is_flag(EFFECT_FLAG_FIELD_ONLY))
				pduel->write_buffer8(EDESC_OPERATION);
			else if (!(peffect->type & EFFECT_TYPE_ACTIONS))
				pduel->write_buffer8(EDESC_RESET);
			else
				pduel->write_buffer8(0);
			pduel->write_buffer32(pcard->data.code);
			pduel->write_buffer32(pcard->get_info_location());
			pduel->write_buffer32(peffect->description);
		}
		return FALSE;
	}
	else {
		if ((returns.ivalue[0] < 0 && forced) || returns.ivalue[0] >= (int32)core.select_chains.size()) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		return TRUE;
	}
}
int32 field::select_place(uint16 step, uint8 playerid, uint32 flag, uint8 count) {
#ifdef DEBUG_PRINTS
	puts("select_place");
#endif
	if (step == 0) {
		if (count == 0)
			return TRUE;
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
			flag = ~flag;
			int32 filter;
			int32 pzone = 0;
			if (flag & 0x7f) {
				returns.bvalue[0] = 1;
				returns.bvalue[1] = LOCATION_MZONE;
				filter = flag & 0x7f;
			}
			else if (flag & 0x1f00) {
				returns.bvalue[0] = 1;
				returns.bvalue[1] = LOCATION_SZONE;
				filter = (flag >> 8) & 0x1f;
			}
			else if (flag & 0xc000) {
				returns.bvalue[0] = 1;
				returns.bvalue[1] = LOCATION_SZONE;
				filter = (flag >> 14) & 0x3;
				pzone = 1;
			}
			else if (flag & 0x7f0000) {
				returns.bvalue[0] = 0;
				returns.bvalue[1] = LOCATION_MZONE;
				filter = (flag >> 16) & 0x7f;
			}
			else if (flag & 0x1f000000) {
				returns.bvalue[0] = 0;
				returns.bvalue[1] = LOCATION_SZONE;
				filter = (flag >> 24) & 0x1f;
			}
			else {
				returns.bvalue[0] = 0;
				returns.bvalue[1] = LOCATION_SZONE;
				filter = (flag >> 30) & 0x3;
				pzone = 1;
			}
			if (!pzone) {
				if (filter & 0x40) returns.bvalue[2] = 6;
				else if (filter & 0x20) returns.bvalue[2] = 5;
				else if (filter & 0x4) returns.bvalue[2] = 2;
				else if (filter & 0x2) returns.bvalue[2] = 1;
				else if (filter & 0x8) returns.bvalue[2] = 3;
				else if (filter & 0x1) returns.bvalue[2] = 0;
				else if (filter & 0x10) returns.bvalue[2] = 4;
			}
			else {
				if (filter & 0x1) returns.bvalue[2] = 6;
				else if (filter & 0x2) returns.bvalue[2] = 7;
			}
			return TRUE;
		} else if ((playerid == 1) && (core.duel_options & DUEL_SIMPLE_AI)) {
			flag = ~flag;
			int32 filter;
			int32 pzone = 0;
			if (flag & 0x7f) {
				returns.bvalue[0] = 1;
				returns.bvalue[1] = LOCATION_MZONE;
				filter = flag & 0x7f;
			}
			else if (flag & 0x1f00) {
				returns.bvalue[0] = 1;
				returns.bvalue[1] = LOCATION_SZONE;
				filter = (flag >> 8) & 0x1f;
			}
			else if (flag & 0xc000) {
				returns.bvalue[0] = 1;
				returns.bvalue[1] = LOCATION_SZONE;
				filter = (flag >> 14) & 0x3;
				pzone = 1;
			}
			else if (flag & 0x7f0000) {
				returns.bvalue[0] = 0;
				returns.bvalue[1] = LOCATION_MZONE;
				filter = (flag >> 16) & 0x7f;
			}
			else if (flag & 0x1f000000) {
				returns.bvalue[0] = 0;
				returns.bvalue[1] = LOCATION_SZONE;
				filter = (flag >> 24) & 0x1f;
			}
			else {
				returns.bvalue[0] = 0;
				returns.bvalue[1] = LOCATION_SZONE;
				filter = (flag >> 30) & 0x3;
				pzone = 1;
			}
			if (!pzone) {
				if (filter & 0x40) returns.bvalue[2] = 6;
				else if (filter & 0x20) returns.bvalue[2] = 5;
				else if (filter & 0x4) returns.bvalue[2] = 2;
				else if (filter & 0x2) returns.bvalue[2] = 1;
				else if (filter & 0x8) returns.bvalue[2] = 3;
				else if (filter & 0x1) returns.bvalue[2] = 0;
				else if (filter & 0x10) returns.bvalue[2] = 4;
			}
			else {
				if (filter & 0x1) returns.bvalue[2] = 6;
				else if (filter & 0x2) returns.bvalue[2] = 7;
			}
			return TRUE;
		}
		if (core.units.begin()->type == PROCESSOR_SELECT_PLACE)
			pduel->write_buffer8(MSG_SELECT_PLACE);
		else
			pduel->write_buffer8(MSG_SELECT_DISFIELD);
		pduel->write_buffer8(playerid);
		pduel->write_buffer8(count);
		pduel->write_buffer32(flag);
		returns.bvalue[0] = 0;
		return FALSE;
	} else {
		uint8 pt = 0, p, l, s;
		for (int8 i = 0; i < count; ++i) {
			p = returns.bvalue[pt];
			l = returns.bvalue[pt + 1];
			s = returns.bvalue[pt + 2];
			if ((p != 0 && p != 1)
				|| ((l != LOCATION_MZONE) && (l != LOCATION_SZONE))
				|| ((0x1u << s) & (flag >> (((p == playerid) ? 0 : 16) + ((l == LOCATION_MZONE) ? 0 : 8))))) {
				pduel->write_buffer8(MSG_RETRY);
				return FALSE;
			}
			pt += 3;
		}
		return TRUE;
	}
}
int32 field::select_position(uint16 step, uint8 playerid, uint32 code, uint8 positions) {
#ifdef DEBUG_PRINTS
	puts("select_position");
#endif
	if (step == 0) {
		if (positions == 0) {
			returns.ivalue[0] = POS_FACEUP_ATTACK;
			return TRUE;
		}
		positions &= 0xf;
		if (positions == 0x1 || positions == 0x2 || positions == 0x4 || positions == 0x8) {
			returns.ivalue[0] = positions;
			return TRUE;
		}
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
#ifdef USE_LUA
			lua_State* L = pduel->lua->lua_state;
			lua_getglobal(L, "OnSelectPosition");
			lua_pushnumber(L, code);
			lua_pushnumber(L, positions);

			if (lua_pcall(L, 2, 1, 0) != 0) {
				sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
				handle_message(pduel, 1);
				returns.ivalue[0] = POS_FACEUP_ATTACK;
				return TRUE;
			}
			int result = (int)lua_tointeger(L, -1);
			lua_pop(L, 1);
			returns.ivalue[0] = result;
#endif
			return TRUE;
		} else if ((playerid == 1) && (core.duel_options & DUEL_SIMPLE_AI)) {
			if (positions & 0x4)
				returns.ivalue[0] = 0x4;
			else if (positions & 0x1)
				returns.ivalue[0] = 0x1;
			else if (positions & 0x8)
				returns.ivalue[0] = 0x8;
			else
				returns.ivalue[0] = 0x2;
			return TRUE;
		}
		pduel->write_buffer8(MSG_SELECT_POSITION);
		pduel->write_buffer8(playerid);
		pduel->write_buffer32(code);
		pduel->write_buffer8(positions);
		returns.ivalue[0] = 0;
		return FALSE;
	} else {
		uint32 pos = returns.ivalue[0];
		if (pos != 0x1 && pos != 0x2 && pos != 0x4 && pos != 0x8 && !(pos & positions)) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		return TRUE;
	}
}
int32 field::select_tribute(uint16 step, uint8 playerid, uint8 cancelable, uint8 min, uint8 max) {
#ifdef DEBUG_PRINTS
	puts("select_tribute");
#endif
	if (step == 0) {
		returns.bvalue[0] = 0;
		if (max == 0 || core.select_cards.empty())
			return TRUE;
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
#ifdef USE_LUA
			returns.ivalue[0] = 1;

			lua_State* L = pduel->lua->lua_state;
			lua_getglobal(L, "OnSelectTribute");

			/* the first argument: cards */
			lua_newtable(L);
			for (size_t i = 0; i<core.select_cards.size(); i++) {
				set_card_to_lua(L, core.select_cards[i], i + 1);
			}

			/* the second argument: minTributes */
			lua_pushnumber(L, min);
			/* the second argument: maxTributes */
			lua_pushnumber(L, max);

			if (lua_pcall(L, 3, 1, 0) != 0) {
				sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
				handle_message(pduel, 1);
				returns.ivalue[0] = 0;
				return TRUE;
			}
			//lua_call(L, 3, 1);
			/* returned results */

			//1st = table
			int tribute_sum = 0;
			int len = luaL_len(L, -1);
			returns.bvalue[0] = len; //set number of tributes
			for (int i = 0; i < len; i++) {
				lua_pushinteger(L, i + 1);
				lua_gettable(L, -2);
				int temp = lua_tonumber(L, -1);
				returns.bvalue[i + 1] = temp - 1; //set tribute index
				tribute_sum = tribute_sum + core.select_cards[temp - 1]->release_param;
				lua_pop(L, 1);
			}
			lua_pop(L, 1);//pop the table off the stack
#else
			returns.ivalue[0] = 1;
			returns.bvalue[0] = min;
			for (int i = 1; i < min + 1; i++) {
				returns.bvalue[i] = i;
			}
#endif			
			return TRUE;
		}
		uint8 tm = 0;
		for (uint32 i = 0; i < core.select_cards.size(); ++i)
			tm += core.select_cards[i]->release_param;
		if (max > 5)
			max = 5;
		if (max > tm)
			max = tm;
		if (min > max)
			min = max;
		core.units.begin()->arg2 = ((uint32)min) + (((uint32)max) << 16);
		pduel->write_buffer8(MSG_SELECT_TRIBUTE);
		pduel->write_buffer8(playerid);
		pduel->write_buffer8(cancelable);
		pduel->write_buffer8(min);
		pduel->write_buffer8(max);
		pduel->write_buffer8(core.select_cards.size());
		card* pcard;
		std::sort(core.select_cards.begin(), core.select_cards.end(), card::card_operation_sort);
		for (uint32 i = 0; i < core.select_cards.size(); ++i) {
			pcard = core.select_cards[i];
			pduel->write_buffer32(pcard->data.code);
			pduel->write_buffer8(pcard->current.controler);
			pduel->write_buffer8(pcard->current.location);
			pduel->write_buffer8(pcard->current.sequence);
			pduel->write_buffer8(pcard->release_param);
		}
		return FALSE;
	} else {
		if (cancelable && returns.ivalue[0] == -1)
			return TRUE;
		byte c[64];
		memset(c, 0, 64);
		if (returns.bvalue[0] > max) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		uint8 m = core.select_cards.size(), v = 0, tt = 0;
		for (int32 i = 0; i < returns.bvalue[0]; ++i) {
			v = returns.bvalue[i + 1];
			if (v < 0 || v >= m || v >= 6 || c[v]) {
				pduel->write_buffer8(MSG_RETRY);
				return FALSE;
			}
			c[v] = 1;
			tt += core.select_cards[v]->release_param;
		}
		if (tt < min) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		return TRUE;
	}
}
int32 field::select_counter(uint16 step, uint8 playerid, uint16 countertype, uint16 count, uint8 s, uint8 o) {
	#ifdef DEBUG_PRINTS
		puts("select_counter");
	#endif
	if (step == 0) {
		if (count == 0)
			return TRUE;
		card* pcard;
		uint8 avail = s;
		uint8 fp = playerid;
		uint32 total = 0;
		core.select_cards.clear();
		for (int p = 0; p < 2; ++p) {
			if (avail) {
				for (int j = 0; j < 5; ++j) {
					pcard = player[fp].list_mzone[j];
					if (pcard && pcard->get_counter(countertype)) {
						core.select_cards.push_back(pcard);
						total += pcard->get_counter(countertype);
					}
				}
				for (int j = 0; j < 8; ++j) {
					pcard = player[fp].list_szone[j];
					if (pcard && pcard->get_counter(countertype)) {
						core.select_cards.push_back(pcard);
						total += pcard->get_counter(countertype);
					}
				}
			}
			fp = 1 - fp;
			avail = o;
		}
		if (count > total) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
			returns.ivalue[0] = 1;
			returns.bvalue[0] = count;
			return TRUE;
		}
		pduel->write_buffer8(MSG_SELECT_COUNTER);
		pduel->write_buffer8(playerid);
		pduel->write_buffer16(countertype);
		pduel->write_buffer16(count);
		pduel->write_buffer8(core.select_cards.size());
		std::sort(core.select_cards.begin(), core.select_cards.end(), card::card_operation_sort);
		for (uint32 i = 0; i < core.select_cards.size(); ++i) {
			pcard = core.select_cards[i];
			pduel->write_buffer32(pcard->data.code);
			pduel->write_buffer8(pcard->current.controler);
			pduel->write_buffer8(pcard->current.location);
			pduel->write_buffer8(pcard->current.sequence);
			pduel->write_buffer16(pcard->get_counter(countertype));
		}
		return FALSE;
	}
	else {
		uint16 ct = 0;
		for (uint32 i = 0; i < core.select_cards.size(); ++i) {
			if (core.select_cards[i]->get_counter(countertype) < returns.bvalue[i]) {
				pduel->write_buffer8(MSG_RETRY);
				return FALSE;
			}
			ct += returns.bvalue[i];
		}
		if (ct != count) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
	}
	return TRUE;
}
static int32 select_sum_check1(const int32* oparam, int32 size, int32 index, int32 acc) {
	if (acc == 0 || index == size)
		return FALSE;
	int32 o1 = oparam[index] & 0xffff;
	int32 o2 = oparam[index] >> 16;
	if (index == size - 1)
		return acc == o1 || acc == o2;
	return (acc > o1 && select_sum_check1(oparam, size, index + 1, acc - o1))
		|| (o2 > 0 && acc > o2 && select_sum_check1(oparam, size, index + 1, acc - o2));
}
static void Cij(int i, int j, std::vector<int> &r, int num, std::vector<std::vector<int> > & result)
{
	if (j == 1)
	{
		for (int k = 0; k < i; k++)
		{
			std::vector<int> temp(num);
			r[num - 1] = k;
			for (int i = 0; i < num; i++)
			{
				temp[i] = r[i];
			}
			result.push_back(temp);
		}
	}
	else if (j == 0)
	{
	}
	else
	{
		for (int k = i; k >= j; k--)
		{
			r[j - 2] = k - 1;
			Cij(k - 1, j - 1, r, num, result);
		}
	}
}
static void getCombine(std::vector<std::vector<int>>& result, int max) {
	result.clear();
	for (int i = 1; i <= max; i++)
	{
		std::vector<int> resulttemp(i);
		Cij(max,i,resulttemp,i,result);
	}
}
int32 field::select_with_sum_limit(int16 step, uint8 playerid, int32 acc, int32 min, int32 max) {
#ifdef DEBUG_PRINTS	
	puts("-select_with_sum_limit-\n");
#endif
	if (step == 0) {
		returns.bvalue[0] = 0;
		if (core.select_cards.empty())
			return TRUE;
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
			lua_State* L = pduel->lua->lua_state;
			lua_getglobal(L, "OnSelectSum");
			lua_newtable(L);
			for (size_t i = 0; i<core.select_cards.size(); i++) {
				set_card_to_lua(L, core.select_cards[i], i + 1);
			}
			lua_pushnumber(L, acc);
			uint32 triggeringcode = 0;
			card* lastCardInChain = NULL;
			if (core.current_chain.size() > 0) {
				effect* e = core.current_chain[core.current_chain.size() - 1].triggering_effect;
				if (e) {
					lastCardInChain = e->owner;
					if (lastCardInChain) {
						triggeringcode = lastCardInChain->get_code();
					}
				}
			}
			if (triggeringcode > 0 && lastCardInChain != NULL) {
				if (lastCardInChain) {
					set_card_to_lua_without_index(L, lastCardInChain, interpreter::get_duel_info(L));
				}
				else {
					lua_pushboolean(L, false); //set a false
				}
			}
			else {
				lua_pushboolean(L, false); //set a false
			}
			if (lua_pcall(L, 3, 1, 0) != 0) {
				sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
				handle_message(pduel, 1);
				returns.ivalue[0] = 0;
				return TRUE;
			}
			//1st = table
			int mustSize = core.must_select_cards.size();
			int len = luaL_len(L, -1);
			returns.ivalue[0] = 1;//return TRUE
			returns.bvalue[0] = (int8)(len+ mustSize); // size
			for (int z = 0; z < mustSize; z++) {
				returns.bvalue[z + 1] = z;
			}
			for (int z = 1; z < len + 1; z++) {
				lua_pushinteger(L, z);
				lua_gettable(L, -2);
				int8 temp = (int8)(lua_tonumber(L, -1) - 1);
				returns.bvalue[z + mustSize] = temp;
				lua_pop(L, 1);
			}
			lua_pop(L, 1);//pop the table off the stack
			return true;
			//lex




			//int fromSize = min;
			//int toSize = max;
			//if (fromSize<0)
			//{
			//	fromSize = 0;
			//}
			//if (fromSize>core.select_cards.size())
			//{
			//	fromSize = core.select_cards.size();
			//}
			//if (toSize<0)
			//{
			//	toSize = 0;
			//}
			//if (toSize>core.select_cards.size())
			//{
			//	toSize = core.select_cards.size();
			//}
			//if (fromSize>toSize)
			//{
			//	toSize = fromSize;
			//}
			//int mustSize = core.must_select_cards.size();
			//for (int CurrentSize = fromSize; CurrentSize <= toSize; CurrentSize++)
			//{
			//	returns.bvalue[0] = (int8)(mustSize + CurrentSize);
			//	for (int i = 0; i < mustSize; i++)
			//	{
			//		returns.bvalue[i + 1] = i;
			//	}

			//	//handleing

			//	std::vector<std::vector<int>> combination;

			//	getCombine(combination, CurrentSize);

			//	for (std::vector<std::vector<int>>::iterator item = combination.begin(); item != combination.end(); item++)
			//	{
			//		int i = 0;
			//		for (std::vector<int>::iterator number = item->begin(); number != item->end(); number++)
			//		{
			//			returns.bvalue[i + mustSize + 1] = i;
			//			i++;
			//		}
			//		if (sum_check(acc, min, max) == true)
			//		{
			//			return true;
			//		}
			//	}


			//	//end

			//}
			//returns.bvalue[0] = 0;
		}
		pduel->write_buffer8(MSG_SELECT_SUM);
		if (max)
			pduel->write_buffer8(0);
		else
			pduel->write_buffer8(1);
		if (max < min)
			max = min;
		pduel->write_buffer8(playerid);
		pduel->write_buffer32(acc & 0xffff);
		pduel->write_buffer8(min);
		pduel->write_buffer8(max);
		pduel->write_buffer8(core.must_select_cards.size());
		for (uint32 i = 0; i < core.must_select_cards.size(); ++i) {
			card* pcard = core.must_select_cards[i];
			pduel->write_buffer32(pcard->data.code);
			pduel->write_buffer8(pcard->current.controler);
			pduel->write_buffer8(pcard->current.location);
			pduel->write_buffer8(pcard->current.sequence);
			pduel->write_buffer32(pcard->sum_param);
		}
		pduel->write_buffer8(core.select_cards.size());
		std::sort(core.select_cards.begin(), core.select_cards.end(), card::card_operation_sort);
		for (uint32 i = 0; i < core.select_cards.size(); ++i) {
			card* pcard = core.select_cards[i];
			pduel->write_buffer32(pcard->data.code);
			pduel->write_buffer8(pcard->current.controler);
			pduel->write_buffer8(pcard->current.location);
			pduel->write_buffer8(pcard->current.sequence);
			pduel->write_buffer32(pcard->sum_param);
		}
		return FALSE;
	}
	else 
	{
		if (sum_check(acc, min, max) == false)
		{
			pduel->write_buffer8(MSG_RETRY);
			return false;
		}
	}
	return TRUE;
}
bool field::sum_check(int32 acc, int32 min, int32 max)
{
	byte c[64];
	memset(c, 0, 64);
	if (max) {
		int32 oparam[16];
		int32 mcount = core.must_select_cards.size();
		if (returns.bvalue[0] < min + mcount || returns.bvalue[0] > max + mcount) {
			return FALSE;
		}
		for (int32 i = 0; i < mcount; ++i)
			oparam[i] = core.must_select_cards[i]->sum_param;
		int32 m = core.select_cards.size();
		for (int32 i = mcount; i < returns.bvalue[0]; ++i) {
			int32 v = returns.bvalue[i + 1];
			if (v < 0 || v >= m || c[v]) {
				return FALSE;
			}
			c[v] = 1;
			oparam[i] = core.select_cards[v]->sum_param;
		}
		if (!select_sum_check1(oparam, returns.bvalue[0], 0, acc)) {
			return FALSE;
		}
		return TRUE;
	}
	else {
		int32 mcount = core.must_select_cards.size();
		int32 sum = 0, mx = 0, mn = 0x7fffffff;
		for (int32 i = 0; i < mcount; ++i) {
			int32 op = core.must_select_cards[i]->sum_param;
			int32 o1 = op & 0xffff;
			int32 o2 = op >> 16;
			int32 ms = (o2 && o2 < o1) ? o2 : o1;
			sum += ms;
			mx += (o2 > o1) ? o2 : o1;
			if (ms < mn)
				mn = ms;
		}
		int32 m = core.select_cards.size();
		for (int32 i = mcount; i < returns.bvalue[0]; ++i) {
			int32 v = returns.bvalue[i + 1];
			if (v < 0 || v >= m || c[v]) {
				return FALSE;
			}
			c[v] = 1;
			int32 op = core.select_cards[v]->sum_param;
			int32 o1 = op & 0xffff;
			int32 o2 = op >> 16;
			int32 ms = (o2 && o2 < o1) ? o2 : o1;
			sum += ms;
			mx += (o2 > o1) ? o2 : o1;
			if (ms < mn)
				mn = ms;
		}
		if (mx < acc || sum - mn >= acc) {
			return FALSE;
		}
		return TRUE;
	}
}
int32 field::sort_card(int16 step, uint8 playerid, uint8 is_chain) {
#ifdef DEBUG_PRINTS
	puts("sort_card");
#endif
	if (step == 0) {
		returns.bvalue[0] = 0;
		if ((playerid == 1) && (core.duel_options & DUEL_SIMPLE_AI)) {
			returns.ivalue[0] = -1;
			return TRUE;
		}
		else if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
			returns.ivalue[0] = 1;
			if (!core.select_cards.empty()) {
				size_t expectedCount = core.select_cards.size();
				lua_State* L = pduel->lua->lua_state;
				lua_getglobal(L, "OnSelectChainOrder");

				lua_newtable(L);
				for (size_t i = 0; i<expectedCount; i++) {
					set_card_to_lua(L, core.select_cards[i], i + 1);
				}

				if (lua_pcall(L, 1, 1, 0) != 0) {
#ifdef AI_DEBUG_BUILD
					sprintf(pduel->strbuffer, "Error in OnSelectChainOrder(): %s", lua_tostring(L, -1));
					handle_message(pduel, 1);
#endif
					//default implementation
					for (uint32 i = 0; i < core.select_cards.size(); ++i) {
						returns.bvalue[i] = i;
					}
					return TRUE;
				}

				//1st = table
				int len = luaL_len(L, -1);
				for (int i = 0; i < len; i++) {
					lua_pushinteger(L, i + 1);
					lua_gettable(L, -2);
					int temp = lua_tonumber(L, -1);
					returns.bvalue[i] = temp - 1; //set tribute index
					lua_pop(L, 1);
				}
				lua_pop(L, 1);//pop the table off the stack
				if (len != expectedCount) {
#ifdef AI_DEBUG_BUILD
					sprintf(pduel->strbuffer, "\"[AI]OnSelectChainOrder\": incorrect input (%d indices expected, %d indices received)", expectedCount, len);
					handle_message(pduel, 1);
#endif
					//default implementation
					for (uint32 i = 0; i < core.select_cards.size(); ++i) {
						returns.bvalue[i] = i;
					}
					return TRUE;
				}
			}
			return TRUE;
		}
		if (core.select_cards.empty())
			return TRUE;
		if (is_chain)
			pduel->write_buffer8(MSG_SORT_CHAIN);
		else
			pduel->write_buffer8(MSG_SORT_CARD);
		pduel->write_buffer8(playerid);
		pduel->write_buffer8(core.select_cards.size());
		card* pcard;
		for (uint32 i = 0; i < core.select_cards.size(); ++i) {
			pcard = core.select_cards[i];
			pduel->write_buffer32(pcard->data.code);
			pduel->write_buffer8(pcard->current.controler);
			pduel->write_buffer8(pcard->current.location);
			pduel->write_buffer8(pcard->current.sequence);
		}
		return FALSE;
	} else {
		if (returns.bvalue[0] == -1)
			return TRUE;
		uint8 seq[64];
		memset(seq, 0, 64);
		for (uint32 i = 0; i < core.select_cards.size(); ++i) {
			if (returns.bvalue[i] < 0 || returns.bvalue[i] >= (int32)core.select_cards.size() || seq[(int32)returns.bvalue[i]]) {
				pduel->write_buffer8(MSG_RETRY);
				return FALSE;
			}
			seq[(int32)returns.bvalue[i]] = 1;
		}
		return TRUE;
	}
	return TRUE;
}
int32 field::announce_race(int16 step, uint8 playerid, int32 count, int32 available) {
#ifdef DEBUG_PRINTS
	puts("announce_race");
#endif
	if (step == 0) {
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
#ifdef USE_LUA
			lua_State* L = pduel->lua->lua_state;
			lua_getglobal(L, "OnDeclareMonsterType");

			/* the first argument */
			lua_pushnumber(L, count);

			/* the second argument */
			lua_newtable(L);
			int counter = 1;
			for (int32 ft = 0x1; ft != 0x1000000; ft <<= 1) {
				if (ft & available) {
					lua_pushinteger(L, counter);
					lua_pushinteger(L, ft);
					lua_settable(L, -3);

					counter++;
				}
			}
			if (lua_pcall(L, 2, 1, 0) != 0) {
				sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
				handle_message(pduel, 1);
				returns.ivalue[0] = 0;
				return TRUE;
			}
			//lua_call(L, 2, 1); //2 arguments
			int result = (int)lua_tointeger(L, -1);
			lua_pop(L, 1);
			returns.ivalue[0] = result;
#endif
			pduel->write_buffer8(MSG_HINT);
			pduel->write_buffer8(HINT_RACE);
			pduel->write_buffer8(0);
			pduel->write_buffer32(returns.ivalue[0]);
			return TRUE;
		}
		int32 scount = 0;
		for (int32 ft = 0x1; ft != 0x1000000; ft <<= 1) {
			if (ft & available)
				scount++;
		}
		if (scount <= count) {
			count = scount;
			core.units.begin()->arg1 = (count << 16) + playerid;
		}
		pduel->write_buffer8(MSG_ANNOUNCE_RACE);
		pduel->write_buffer8(playerid);
		pduel->write_buffer8(count);
		pduel->write_buffer32(available);
		return FALSE;
	} else {
		int32 rc = returns.ivalue[0];
		int32 sel = 0;
		for (int32 ft = 0x1; ft != 0x1000000; ft <<= 1) {
			if (!(ft & rc)) continue;
			if (!(ft & available)) {
				pduel->write_buffer8(MSG_RETRY);
				return FALSE;
			}
			sel++;
		}
		if (sel != count) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		pduel->write_buffer8(MSG_HINT);
		pduel->write_buffer8(HINT_RACE);
		pduel->write_buffer8(playerid);
		pduel->write_buffer32(returns.ivalue[0]);
		return TRUE;
	}
	return TRUE;
}
int32 field::announce_attribute(int16 step, uint8 playerid, int32 count, int32 available) {
#ifdef DEBUG_PRINTS
	puts("announce_attribute");
#endif
	if (step == 0) {
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
#ifdef USE_LUA
			lua_State* L = pduel->lua->lua_state;
			lua_getglobal(L, "OnDeclareAttribute");

			/* the first argument */
			lua_pushnumber(L, count);

			/* the second argument */
			lua_newtable(L);
			int counter = 1;
			for (int32 ft = 0x1; ft != 0x80; ft <<= 1) {
				if (ft & available) {
					lua_pushinteger(L, counter);
					lua_pushinteger(L, ft);
					lua_settable(L, -3);

					counter++;
				}
			}
			if (lua_pcall(L, 2, 1, 0) != 0) {
				sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
				handle_message(pduel, 1);
				returns.ivalue[0] = 0;
				return TRUE;
			}
			//lua_call(L, 2, 1); //2 arguments
			int result = (int)lua_tointeger(L, -1);
			lua_pop(L, 1);
			returns.ivalue[0] = result;
#else
			int numChosen = 0;
			int32 result = 0;
			for (int32 ft = 0x1; ft != 0x80; ft <<= 1) {
				if (ft & available) {
					result = result + ft;
					returns.ivalue[0] = result;

					numChosen = numChosen + 1;
					if (numChosen >= count) {
						break;
					}
				}
			}
#endif
			pduel->write_buffer8(MSG_HINT);
			pduel->write_buffer8(HINT_ATTRIB);
			pduel->write_buffer8(0);
			pduel->write_buffer32(returns.ivalue[0]);

			return TRUE;
		}
		int32 scount = 0;
		for (int32 ft = 0x1; ft != 0x80; ft <<= 1) {
			if (ft & available)
				scount++;
		}
		if (scount <= count) {
			count = scount;
			core.units.begin()->arg1 = (count << 16) + playerid;
		}
		pduel->write_buffer8(MSG_ANNOUNCE_ATTRIB);
		pduel->write_buffer8(playerid);
		pduel->write_buffer8(count);
		pduel->write_buffer32(available);
		return FALSE;
	}
	else {
		int32 rc = returns.ivalue[0];
		int32 sel = 0;
		for (int32 ft = 0x1; ft != 0x80; ft <<= 1) {
			if (!(ft & rc)) continue;
			if (!(ft & available)) {
				pduel->write_buffer8(MSG_RETRY);
				return FALSE;
			}
			sel++;
		}
		if (sel != count) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		pduel->write_buffer8(MSG_HINT);
		pduel->write_buffer8(HINT_ATTRIB);
		pduel->write_buffer8(playerid);
		pduel->write_buffer32(returns.ivalue[0]);
		return TRUE;
	}
	return TRUE;
}
#define CARD_MARINE_DOLPHIN	78734254
#define CARD_TWINKLE_MOSS	13857930
static int32 is_declarable(card_data const& cd, const std::vector<uint32>& opcode) {
	std::stack<int32> stack;
	for (auto it = opcode.begin(); it != opcode.end(); ++it) {
		switch (*it) {
		case OPCODE_ADD: {
			if (stack.size() >= 2) {
				int32 rhs = stack.top();
				stack.pop();
				int32 lhs = stack.top();
				stack.pop();
				stack.push(lhs + rhs);
			}
			break;
		}
		case OPCODE_SUB: {
			if (stack.size() >= 2) {
				int32 rhs = stack.top();
				stack.pop();
				int32 lhs = stack.top();
				stack.pop();
				stack.push(lhs - rhs);
			}
			break;
		}
		case OPCODE_MUL: {
			if (stack.size() >= 2) {
				int32 rhs = stack.top();
				stack.pop();
				int32 lhs = stack.top();
				stack.pop();
				stack.push(lhs * rhs);
			}
			break;
		}
		case OPCODE_DIV: {
			if (stack.size() >= 2) {
				int32 rhs = stack.top();
				stack.pop();
				int32 lhs = stack.top();
				stack.pop();
				stack.push(lhs / rhs);
		}
			break;
		}
		case OPCODE_AND: {
			if (stack.size() >= 2) {
				int32 rhs = stack.top();
				stack.pop();
				int32 lhs = stack.top();
				stack.pop();
				stack.push(lhs && rhs);
			}
			break;
		}
		case OPCODE_OR: {
			if (stack.size() >= 2) {
				int32 rhs = stack.top();
				stack.pop();
				int32 lhs = stack.top();
				stack.pop();
				stack.push(lhs || rhs);
			}
			break;
		}
		case OPCODE_NEG: {
			if (stack.size() >= 1) {
				int32 val = stack.top();
				stack.pop();
				stack.push(-val);
			}
			break;
		}
		case OPCODE_NOT: {
			if (stack.size() >= 1) {
				int32 val = stack.top();
				stack.pop();
				stack.push(!val);
			}
			break;
		}
		case OPCODE_ISCODE: {
			if (stack.size() >= 1) {
				int32 code = stack.top();
				stack.pop();
				stack.push(cd.code == code);
			}
			break;
		}
		case OPCODE_ISSETCARD: {
			if (stack.size() >= 1) {
				int32 set_code = stack.top();
				stack.pop();
				uint64 sc = cd.setcode;
				bool res = false;
				uint32 settype = set_code & 0xfff;
				uint32 setsubtype = set_code & 0xf000;
				while (sc) {
					if ((sc & 0xfff) == settype && (sc & 0xf000 & setsubtype) == setsubtype)
						res = true;
					sc = sc >> 16;
				}
				stack.push(res);
			}
			break;
		}
		case OPCODE_ISTYPE: {
			if (stack.size() >= 1) {
				int32 val = stack.top();
				stack.pop();
				stack.push(cd.type & val);
			}
			break;
		}
		case OPCODE_ISRACE: {
			if (stack.size() >= 1) {
				int32 race = stack.top();
				stack.pop();
				stack.push(cd.race & race);
			}
			break;
		}
		case OPCODE_ISATTRIBUTE: {
			if (stack.size() >= 1) {
				int32 attribute = stack.top();
				stack.pop();
				stack.push(cd.attribute & attribute);
			}
			break;
		}
		default: {
			stack.push(*it);
			break;
		}
		}
	}
	if (stack.size() != 1 || stack.top() == 0)
		return FALSE;
	return cd.code == CARD_MARINE_DOLPHIN || cd.code == CARD_TWINKLE_MOSS
		|| (!cd.alias && (cd.type & (TYPE_MONSTER + TYPE_TOKEN)) != (TYPE_MONSTER + TYPE_TOKEN));
}
int32 field::announce_card(int16 step, uint8 playerid, uint32 ttype) {
	#ifdef DEBUG_PRINTS
		puts("announce_card");
	#endif
	if (step == 0) {
		if (core.select_options.size() == 0) {
			if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
#ifdef USE_LUA
				lua_State* L = pduel->lua->lua_state;
				lua_getglobal(L, "OnDeclareCard");
				if (lua_pcall(L, 0, 1, 0) != 0) {
					sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
					handle_message(pduel, 1);
					returns.ivalue[0] = 0;
					return TRUE;
				}
				//lua_call(L, 0, 1);
				uint32 result = (uint32)lua_tointeger(L, -1);
				lua_pop(L, 1);
				returns.ivalue[0] = result;
#else
				//random card?
				//use rescue rabbit for now
				returns.ivalue[0] = 85138716;
#endif
				//let the player know which card was chosen
				pduel->write_buffer8(MSG_HINT);
				pduel->write_buffer8(HINT_CODE);
				pduel->write_buffer8(0);
				pduel->write_buffer32(returns.ivalue[0]);
				return TRUE;
			}
			pduel->write_buffer8(MSG_ANNOUNCE_CARD);
			pduel->write_buffer8(playerid);
			pduel->write_buffer32(ttype);
		}
		else {
			if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
				returns.ivalue[0] = 86445415;//Red Gadget 
				//let the player know which card was chosen
				pduel->write_buffer8(MSG_HINT);
				pduel->write_buffer8(HINT_CODE);
				pduel->write_buffer8(0);
				pduel->write_buffer32(returns.ivalue[0]);
				return TRUE;
			}
			pduel->write_buffer8(MSG_ANNOUNCE_CARD_FILTER);
			pduel->write_buffer8(playerid);
			pduel->write_buffer8(core.select_options.size());
			for (uint32 i = 0; i < core.select_options.size(); ++i)
				pduel->write_buffer32(core.select_options[i]);
		}
		return FALSE;
	}
	else {
		int32 code = returns.ivalue[0];
		card_data data;
		read_card(code, &data);
		if (!data.code) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		if (core.select_options.size() == 0) {
			if (!(data.type & ttype)) {
				pduel->write_buffer8(MSG_RETRY);
				return FALSE;
			}
		}
		else {
			if (!is_declarable(data, core.select_options)) {
				pduel->write_buffer8(MSG_RETRY);
				return FALSE;
			}
		}
		pduel->write_buffer8(MSG_HINT);
		pduel->write_buffer8(HINT_CODE);
		pduel->write_buffer8(playerid);
		pduel->write_buffer32(code);
		return TRUE;
	}
	return TRUE;
}
int32 field::announce_number(int16 step, uint8 playerid) {
#ifdef DEBUG_PRINTS
	puts("announce_number");
#endif
	if (step == 0) {
		if ((player[playerid].isAI) && (core.duel_options & DUEL_SIMPLE_AI_MODE)) {
#ifdef USE_LUA
			lua_State* L = pduel->lua->lua_state;
			lua_getglobal(L, "OnSelectNumber");
			lua_newtable(L);
			std::vector<uint32> opt = core.select_options;

			for (size_t i = 0; i<opt.size(); i++) {
				lua_pushinteger(L, i + 1); //lua indices start at 1
				lua_pushinteger(L, opt[i]);
				lua_settable(L, -3);
			}
			if (lua_pcall(L, 1, 1, 0) != 0) {
				sprintf(pduel->strbuffer, "%s", lua_tostring(L, -1));
				handle_message(pduel, 1);
				returns.ivalue[0] = 0;
				return TRUE;
			}
			int result = (int)lua_tointeger(L, -1);
			lua_pop(L, 1);
			returns.ivalue[0] = result - 1;
#else
			//select random value from the the possibilities

			int size = core.select_options.size();
			srand(time(NULL));
			returns.ivalue[0] = rand() % size;

#endif
			//let the player know what number was chosen
			pduel->write_buffer8(MSG_HINT);
			pduel->write_buffer8(HINT_NUMBER);
			pduel->write_buffer8(0);
			pduel->write_buffer32(core.select_options[returns.ivalue[0]]);
			return TRUE;
		}
		pduel->write_buffer8(MSG_ANNOUNCE_NUMBER);
		pduel->write_buffer8(playerid);
		pduel->write_buffer8(core.select_options.size());
		for (uint32 i = 0; i < core.select_options.size(); ++i)
			pduel->write_buffer32(core.select_options[i]);
		return FALSE;
	} else {
		int32 ret = returns.ivalue[0];
		if (ret < 0 || ret >= (int32)core.select_options.size() || ret >= 63) {
			pduel->write_buffer8(MSG_RETRY);
			return FALSE;
		}
		pduel->write_buffer8(MSG_HINT);
		pduel->write_buffer8(HINT_NUMBER);
		pduel->write_buffer8(playerid);
		pduel->write_buffer32(core.select_options[returns.ivalue[0]]);
		return TRUE;
	}
}
