using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class UnitParamsData : SyncListStruct<UnitParams> {}

public struct UnitParams
{
	public int unit_id;
	public string unit_account_id;
	public string unit_name;
	public string unit_icon_url;
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
}
