using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AccountDataManager : MonoBehaviour {

	public int AccountId;
	public string AccountName;
	public int test_uint_id;
	public int test_uint_attack;
	public int test_uint_hitPoint;
	public int test_uint_speed;
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


	public List<UnitStatus> partyUnitParamList1 = new List<UnitStatus>();


	// Use this for initialization
	void Start () {
		//本来はサーバから取得して格納すべきだろうか
		//ローカルでやるならセーブメモリから取得
		partyUnitParamList1.Add(new UnitStatus(
			test_uint_id,					//unit_id
			test_uint_attack,						//attack,
			test_uint_hitPoint,						//hitPoint,
			test_uint_speed,							//speed;
			test_uint_type,							//type;
			test_uint_Level,							//Level;
			test_uint_combo,							//combo;
			test_uint_ability_1,							//ability_1;
			test_uint_ability_2,
			test_uint_ability_3,
			test_uint_strikeShot,							//strikeShot;
			test_uint_comboType,							//comboType;
			test_uint_comboAttack,							//comboAttack;
			test_uint_maxComboNum							//maxComboNum;
		));

		partyUnitParamList1.Add(new UnitStatus(
			1,							//unit_id
			3,							//attack,
			100,						//hitPoint,
			10,							//speed;
			2,							//type;
			2,							//Level;
			1,							//combo;
			0,							//ability_1;
			0,
			0,
			0,							//strikeShot;
			1,							//comboType;
			2,							//comboAttack;
			1							//maxComboNum;
		));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
