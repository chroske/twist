using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class LobbyPlayerController : NetworkBehaviour {

	[SyncVar] public string syncLobbyPlayerName;
	private bool setLobbyPlayerNodeFlag;

	public class UnitParamsClass : SyncListStruct<UnitParams> {}
	public UnitParamsClass unitParamsClass = new UnitParamsClass();

	public struct UnitParams
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
	};
	
	// Update is called once per frame
	void Update () {
		//syncLobbyPlayerNameに値が入ったらNodeをprefabから作る
		if(syncLobbyPlayerName != ""){
			CreateLobbyPlayerListPrefab (syncLobbyPlayerName + unitParamsClass[0].unit_id.ToString());
			//CreateLobbyPlayerListPrefab (m_bufs[0].name);
		}
	}

	//LobbyPlayerListPrefabの生成をlobbyManagerに依頼(１度だけ)
	void CreateLobbyPlayerListPrefab(string lobbyPlayerName){
		if(!setLobbyPlayerNodeFlag){
			setLobbyPlayerNodeFlag = true;

			GameObject lobbyManager = GameObject.Find("LobbyManager");
			lobbyManager.transform.GetComponent<CustomNetworkLobbyManager> ().CreateLobbyPlayerListPrefab(lobbyPlayerName);
		}
	}

	public void ProvideLobbyPlayerNameToServer (){
		GameObject acountDataManagerObj = GameObject.Find("AccountDataManager");
		AccountDataManager acountDataManager = acountDataManagerObj.GetComponent<AccountDataManager> ();
		List<UnitStatus> mainPartyList = acountDataManager.partyUnitParamList1;
		UnitStatus LeaderUnitParam = mainPartyList [0];

		CmdProvideLobbyPlayerDataToServer(acountDataManager.AccountName, LeaderUnitParam.unit_id, LeaderUnitParam.attack, LeaderUnitParam.hitPoint, LeaderUnitParam.speed, LeaderUnitParam.type, LeaderUnitParam.Level, LeaderUnitParam.combo, LeaderUnitParam.ability_1, LeaderUnitParam.ability_2, LeaderUnitParam.ability_3, LeaderUnitParam.strikeShot, LeaderUnitParam.comboType, LeaderUnitParam.comboAttack, LeaderUnitParam.maxComboNum);
	}

	[Command]
	void CmdProvideLobbyPlayerDataToServer (string lobbyPlayerName, int _unit_id, int _attack, int _hitPoint, float _speed, int _type, int _Level, int _combo, int _ability_1, int _ability_2, int _ability_3, int _strikeShot, int _comboType, int _comboAttack, int _maxComboNum){
		syncLobbyPlayerName = lobbyPlayerName;

		UnitParams unitParams = new UnitParams ();
		unitParams.unit_id = _unit_id;
		unitParams.attack = _attack;
		unitParams.hitPoint = _hitPoint;
		unitParams.speed = _speed;
		unitParams.type = _type;
		unitParams.Level = _Level;
		unitParams.combo = _combo;
		unitParams.ability_1 = _ability_1;
		unitParams.ability_2 = _ability_2;
		unitParams.ability_3 = _ability_3;
		unitParams.strikeShot = _strikeShot;
		unitParams.comboType = _comboType;
		unitParams.comboAttack = _comboAttack;
		unitParams.maxComboNum = _maxComboNum;

		unitParamsClass.Add(unitParams);
	}















}
