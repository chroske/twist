using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class OwnedUnitData {

	public int unit_id;
	public string unit_account_id;
	public string unit_name;
	public string unit_icon_url;
	public int party_id; //0=パーティ以外 1~4=パーティ
	public int attack;
	public int hitPoint;
	public float speed;
	public int type;
	public int Level;
	public int combo;
	public int ability_1;
	public int ability_2;
	public int ability_3;
	public int strikeShot;
	public int comboType;
	public int comboAttack;
	public int maxComboNum;

	public OwnedUnitData(Dictionary<string, object>data)
	{
		unit_id = (int)data["unit_id"];
		unit_account_id = data["unit_acount_id"].ToString();
		unit_name = data["unit_name"].ToString();
		unit_icon_url = data["unit_icon_url"].ToString();
		party_id = (int)data["party_id"];
		attack = (int)data["attack"];
		hitPoint = (int)data["hitPoint"];
		speed = (float)data["speed"];
		type = (int)data["type"];
		Level = (int)data["Level"];
		combo = (int)data["combo"];
		ability_1 = (int)data["ability_1"];
		ability_2 = (int)data["ability_2"];
		ability_3 = (int)data["ability_3"];
		strikeShot = (int)data["strikeShot"];
		comboType = (int)data["comboType"];
		comboAttack = (int)data["comboAttack"];
		maxComboNum = (int)data["maxComboNum"];
	}
}