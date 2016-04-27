using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitStatusTest {

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


	public void SetUnitStatusTest(int _unit_id, int _attack, int _hitPoint, float _speed, int _type, int _Level, int _combo, int _ability_1, int _ability_2, int _ability_3, int _strikeShot, int _comboType, int _comboAttack, int _maxComboNum)
	{
		unit_id = _unit_id;
		attack = _attack;
		hitPoint = _hitPoint;
		speed = _speed;
		type = _type;
		Level = _Level;
		combo = _combo;
		ability_1 = _ability_1;
		ability_2 = _ability_2;
		ability_3 = _ability_3;
		strikeShot = _strikeShot;
		comboType = _comboType;
		comboAttack = _comboAttack;
		maxComboNum = _maxComboNum;
	}
}