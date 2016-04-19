using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AccountDataManager : MonoBehaviour {

	public int AccountId;
	public string AccountName;
	public List<UnitStatus> partyUnitParamList1 = new List<UnitStatus>();


	// Use this for initialization
	void Start () {
		partyUnitParamList1.Add(new UnitStatus(
			5,							//attack,
			200,						//hitPoint,
			30,							//speed;
			1,							//type;
			1,							//Level;
			0,							//combo;
			UnitStatus.Ability.Ability2,//ability_1;
			UnitStatus.Ability.Ability2,
			UnitStatus.Ability.Ability2,
			UnitStatus.StrikeShot.skill2,//strikeShot;
			UnitStatus.ComboType.Beam2,	//comboType;
			5,							//comboAttack;
			1							//maxComboNum;
		));

		partyUnitParamList1.Add(new UnitStatus(
			3,							//attack,
			100,						//hitPoint,
			10,							//speed;
			2,							//type;
			2,							//Level;
			1,							//combo;
			UnitStatus.Ability.Ability3,//ability_1;
			UnitStatus.Ability.Ability3,
			UnitStatus.Ability.Ability3,
			UnitStatus.StrikeShot.skill3,//strikeShot;
			UnitStatus.ComboType.Beam1,	//comboType;
			2,							//comboAttack;
			1							//maxComboNum;
		));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
