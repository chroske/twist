using UnityEngine;
using System.Collections;

public class UnitParamManager : MonoBehaviour {

	//unit game parameter
	public int unit_id;
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


	public void SetParameter(OwnedUnitData myUnitParam){
		unit_id = myUnitParam.unit_id;
		attack = myUnitParam.attack;
		hitPoint = myUnitParam.hitPoint;
		speed = myUnitParam.speed;
		type = myUnitParam.type;
		Level = myUnitParam.Level;
		combo = myUnitParam.combo;
		ability_1 = myUnitParam.ability_1;
		ability_2 = myUnitParam.ability_2;
		ability_3 = myUnitParam.ability_3;
		strikeShot = myUnitParam.strikeShot;
		comboType = myUnitParam.comboType;
		comboAttack = myUnitParam.comboAttack;
		maxComboNum = myUnitParam.maxComboNum;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
