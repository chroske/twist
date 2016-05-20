using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour {
	//test data
	public int test_uint_id;
	public int test_party_num;
	public int test_uint_attack;
	public int test_uint_hitPoint;
	public float test_uint_speed;
	public int test_uint_type;
	public int test_uint_Level;
	public int test_uint_combo;
	public int test_uint_ability_1;
	public int test_uint_ability_2;
	public int test_uint_ability_3;
	public int test_uint_strikeShot;
	public int test_uint_comboType;
	public int test_uint_comboAttack;
	public int test_uint_maxComboNum;

	public int AccountId;
	public string AccountName;
	public List<OwnedUnitData> partyUnitList1 = new List<OwnedUnitData>();//パーティユニットリスト
	public List<OwnedUnitData> ownedUnitList = new List<OwnedUnitData>();//所持ユニットリスト

	// Use this for initialization
	void Start () {
		//本来はサーバから取得して格納すべきだろうか
		//ローカルでやるならセーブメモリから取得
		Dictionary<string, object> data1 = new Dictionary<string, object> () {
			{ "unit_id", test_uint_id },
			{ "unit_acount_id", "ninja01" },
			{ "unit_name", "ニンジャ1" },
			{ "partyNum", 1 },
			{ "attack", test_uint_attack },
			{ "hitPoint", test_uint_hitPoint },
			{ "speed", test_uint_speed },
			{ "type", test_uint_type },
			{ "Level", test_uint_Level },
			{ "combo", test_uint_combo },
			{ "ability_1", test_uint_ability_1 },
			{ "ability_2", test_uint_ability_2 },
			{ "ability_3", test_uint_ability_3 },
			{ "strikeShot", test_uint_strikeShot },
			{ "comboType", test_uint_comboType },
			{ "comboAttack", test_uint_comboAttack },
			{ "maxComboNum", test_uint_maxComboNum }
		};
		Dictionary<string, object> data2 = new Dictionary<string, object> () {
			{ "unit_id", test_uint_id },
			{ "unit_acount_id", "ninja02" },
			{ "unit_name", "ニンジャ2" },
			{ "partyNum", 2 },
			{ "attack", test_uint_attack },
			{ "hitPoint", test_uint_hitPoint },
			{ "speed", test_uint_speed },
			{ "type", test_uint_type },
			{ "Level", test_uint_Level },
			{ "combo", test_uint_combo },
			{ "ability_1", test_uint_ability_1 },
			{ "ability_2", test_uint_ability_2 },
			{ "ability_3", test_uint_ability_3 },
			{ "strikeShot", test_uint_strikeShot },
			{ "comboType", test_uint_comboType },
			{ "comboAttack", test_uint_comboAttack },
			{ "maxComboNum", test_uint_maxComboNum }
		};
		Dictionary<string, object> data3 = new Dictionary<string, object> () {
			{ "unit_id", test_uint_id },
			{ "unit_acount_id", "ninja03" },
			{ "unit_name", "ニンジャ3" },
			{ "partyNum", 3 },
			{ "attack", test_uint_attack },
			{ "hitPoint", test_uint_hitPoint },
			{ "speed", test_uint_speed },
			{ "type", test_uint_type },
			{ "Level", test_uint_Level },
			{ "combo", test_uint_combo },
			{ "ability_1", test_uint_ability_1 },
			{ "ability_2", test_uint_ability_2 },
			{ "ability_3", test_uint_ability_3 },
			{ "strikeShot", test_uint_strikeShot },
			{ "comboType", test_uint_comboType },
			{ "comboAttack", test_uint_comboAttack },
			{ "maxComboNum", test_uint_maxComboNum }
		};
		Dictionary<string, object> data4 = new Dictionary<string, object> () {
			{ "unit_id", test_uint_id },
			{ "unit_acount_id", "ninja04" },
			{ "unit_name", "ニンジャ4" },
			{ "partyNum", 4 },
			{ "attack", test_uint_attack },
			{ "hitPoint", test_uint_hitPoint },
			{ "speed", test_uint_speed },
			{ "type", test_uint_type },
			{ "Level", test_uint_Level },
			{ "combo", test_uint_combo },
			{ "ability_1", test_uint_ability_1 },
			{ "ability_2", test_uint_ability_2 },
			{ "ability_3", test_uint_ability_3 },
			{ "strikeShot", test_uint_strikeShot },
			{ "comboType", test_uint_comboType },
			{ "comboAttack", test_uint_comboAttack },
			{ "maxComboNum", test_uint_maxComboNum }
		};
		Dictionary<string, object> data5 = new Dictionary<string, object> () {
			{ "unit_id", test_uint_id },
			{ "unit_acount_id", "ninja05" },
			{ "unit_name", "ニンジャ5" },
			{ "partyNum", 0 },
			{ "attack", test_uint_attack },
			{ "hitPoint", test_uint_hitPoint },
			{ "speed", test_uint_speed },
			{ "type", test_uint_type },
			{ "Level", test_uint_Level },
			{ "combo", test_uint_combo },
			{ "ability_1", test_uint_ability_1 },
			{ "ability_2", test_uint_ability_2 },
			{ "ability_3", test_uint_ability_3 },
			{ "strikeShot", test_uint_strikeShot },
			{ "comboType", test_uint_comboType },
			{ "comboAttack", test_uint_comboAttack },
			{ "maxComboNum", test_uint_maxComboNum }
		};
		ownedUnitList.Add(new OwnedUnitData(data1));
		ownedUnitList.Add(new OwnedUnitData(data2));
		ownedUnitList.Add(new OwnedUnitData(data3));
		ownedUnitList.Add(new OwnedUnitData(data4));
		ownedUnitList.Add(new OwnedUnitData(data5));

		foreach(OwnedUnitData UnitData in ownedUnitList)
		{
			if(UnitData.partyNum != 0){
				partyUnitList1.Add(UnitData);
			}
		}
	}
		
	// Update is called once per frame
	void Update () {
	
	}
}
