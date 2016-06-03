using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
	public bool offlineGame;
	public bool onlineGame;
	public Dictionary<int,OwnedUnitData> ownedUnitDic = new Dictionary<int,OwnedUnitData> ();//パーティユニットリスト
	public Dictionary<int,OwnedUnitData> partyUnitDic = new Dictionary<int,OwnedUnitData> ();//所持ユニットリスト

	void Awake(){
		DontDestroyOnLoad (this);
	}

	// Use this for initialization
	void Start () {
		//本来はサーバから取得して格納すべきだろうか
		//ローカルでやるならセーブメモリから取得
		Dictionary<string, object> data1 = new Dictionary<string, object> () {
			{ "unit_id", 1 },
			{ "unit_acount_id", "ninja01" },
			{ "unit_name", "ニンジャ1ビーム" },
			{ "party_id", 1 },
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
			{ "comboType", 0 },
			{ "comboAttack", test_uint_comboAttack },
			{ "maxComboNum", test_uint_maxComboNum }
		};
		Dictionary<string, object> data2 = new Dictionary<string, object> () {
			{ "unit_id", 2 },
			{ "unit_acount_id", "ninja02" },
			{ "unit_name", "ニンジャ2爆" },
			{ "party_id", 2 },
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
			{ "comboType", 1 },
			{ "comboAttack", test_uint_comboAttack },
			{ "maxComboNum", test_uint_maxComboNum }
		};
		Dictionary<string, object> data3 = new Dictionary<string, object> () {
			{ "unit_id", 3 },
			{ "unit_acount_id", "ninja03" },
			{ "unit_name", "ニンジャ3強ビーム" },
			{ "party_id", 0 },
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
			{ "comboType", 2 },
			{ "comboAttack", test_uint_comboAttack },
			{ "maxComboNum", test_uint_maxComboNum }
		};
		Dictionary<string, object> data4 = new Dictionary<string, object> () {
			{ "unit_id", 4 },
			{ "unit_acount_id", "ninja04" },
			{ "unit_name", "ニンジャ4ビーム" },
			{ "party_id", 4 },
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
			{ "comboType", 0 },
			{ "comboAttack", test_uint_comboAttack },
			{ "maxComboNum", test_uint_maxComboNum }
		};
		Dictionary<string, object> data5 = new Dictionary<string, object> () {
			{ "unit_id", 5 },
			{ "unit_acount_id", "ninja05" },
			{ "unit_name", "ニンジャ5爆" },
			{ "party_id", 0 },
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
			{ "comboType", 1 },
			{ "comboAttack", test_uint_comboAttack },
			{ "maxComboNum", test_uint_maxComboNum }
		};
		Dictionary<string, object> data6 = new Dictionary<string, object> () {
			{ "unit_id", 6 },
			{ "unit_acount_id", "ninja06" },
			{ "unit_name", "ニンジャ6ビーム" },
			{ "party_id", 0 },
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
			{ "comboType", 0 },
			{ "comboAttack", test_uint_comboAttack },
			{ "maxComboNum", test_uint_maxComboNum }
		};
		Dictionary<string, object> data7 = new Dictionary<string, object> () {
			{ "unit_id", 7 },
			{ "unit_acount_id", "ninja07" },
			{ "unit_name", "ニンジャ7爆" },
			{ "party_id", 0 },
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
			{ "comboType", 1 },
			{ "comboAttack", test_uint_comboAttack },
			{ "maxComboNum", test_uint_maxComboNum }
		};
		Dictionary<string, object> data8 = new Dictionary<string, object> () {
			{ "unit_id", 8 },
			{ "unit_acount_id", "ninja08" },
			{ "unit_name", "ニンジャ8強ビーム" },
			{ "party_id", 0 },
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
			{ "comboType", 2 },
			{ "comboAttack", test_uint_comboAttack },
			{ "maxComboNum", test_uint_maxComboNum }
		};

		ownedUnitDic.Add(new OwnedUnitData(data1).unit_id, new OwnedUnitData(data1));
		ownedUnitDic.Add(new OwnedUnitData(data2).unit_id, new OwnedUnitData(data2));
		ownedUnitDic.Add(new OwnedUnitData(data3).unit_id, new OwnedUnitData(data3));
		ownedUnitDic.Add(new OwnedUnitData(data4).unit_id, new OwnedUnitData(data4));
		ownedUnitDic.Add(new OwnedUnitData(data5).unit_id, new OwnedUnitData(data5));
		ownedUnitDic.Add(new OwnedUnitData(data6).unit_id, new OwnedUnitData(data6));
		ownedUnitDic.Add(new OwnedUnitData(data7).unit_id, new OwnedUnitData(data7));
		ownedUnitDic.Add(new OwnedUnitData(data8).unit_id, new OwnedUnitData(data8));

		foreach(KeyValuePair<int, OwnedUnitData> UnitDataPair in ownedUnitDic)
		{
			if(UnitDataPair.Value.party_id != 0){
				partyUnitDic.Add (UnitDataPair.Value.unit_id, UnitDataPair.Value);
			}
		}

		//party_idでソート
		partyUnitDic = SortPartyUnitDicByPartyId (partyUnitDic);
	}

	public void ResetPartyDic(){
		//初期化してから
		partyUnitDic = new Dictionary<int,OwnedUnitData> ();
		foreach(KeyValuePair<int, OwnedUnitData> UnitDataPair in ownedUnitDic)
		{
			if(UnitDataPair.Value.party_id != 0){
				partyUnitDic.Add (UnitDataPair.Value.party_id, UnitDataPair.Value);
			}
		}

		//party_idでソート
		partyUnitDic = SortPartyUnitDicByPartyId (partyUnitDic);
	}

	//party_idでソート
	private Dictionary<int,OwnedUnitData> SortPartyUnitDicByPartyId(Dictionary<int,OwnedUnitData> partyUnitDic_Before){
		Dictionary<int,OwnedUnitData> partyUnitDic_After = new Dictionary<int,OwnedUnitData> ();
		var vs2 = partyUnitDic_Before.OrderBy((x) => x.Value.party_id);
		foreach (var v in vs2)
		{
			partyUnitDic_After.Add (v.Key, v.Value);
		}

		return partyUnitDic_After;
	}
		
	// Update is called once per frame
	void Update () {
	
	}
}
