using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitStatus
{
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


	public UnitStatus(int _unit_id, int _attack, int _hitPoint, float _speed, int _type, int _Level, int _combo, int _ability_1, int _ability_2, int _ability_3, int _strikeShot, int _comboType, int _comboAttack, int _maxComboNum)
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
		
	private UnitStatus(){
	}

//	public int attack;
//	public int hitPoint;
//	public float speed;
//	public int type;
//	public int Level;
//	public int combo;
//	public Ability ability_1;
//	public Ability ability_2;
//	public Ability ability_3;
//	public StrikeShot strikeShot;
//	public ComboType comboType;
//	public int comboAttack;
//	public int maxComboNum;

//	public enum ComboType 
//	{
//		Beam1,
//		Beam2,
//		Exprosion
//	}
//
//	public enum Ability
//	{
//		Ability1,
//		Ability2,
//		Ability3
//	}
//
//	public enum StrikeShot
//	{
//		skill1,
//		skill2,
//		skill3
//	}
//
//	public UnitStatus(int _attack, int _hitPoint, float _speed, int _type, int _Level, int _combo, Ability _ability_1, Ability _ability_2, Ability _ability_3, StrikeShot _strikeShot, ComboType _comboType, int _comboAttack, int _maxComboNum)
//	{
//		attack = _attack;
//		hitPoint = _hitPoint;
//		speed = _speed;
//		type = _type;
//		Level = _Level;
//		combo = _combo;
//		ability_1 = _ability_1;
//		ability_2 = _ability_2;
//		ability_3 = _ability_3;
//		strikeShot = _strikeShot;
//		comboType = _comboType;
//		comboAttack = _comboAttack;
//		maxComboNum = _maxComboNum;
//	}
}

