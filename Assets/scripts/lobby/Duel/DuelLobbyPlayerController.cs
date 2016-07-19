﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class DuelLobbyPlayerController : NetworkBehaviour {

	public class UnitParamsClass : SyncListStruct<UnitParams> {}
	public UnitParamsClass unitParamsClass = new UnitParamsClass();

	private GameObject LobbyPlayerListPrefab;
	private bool setLobbyPlayerNodeFlag;
	public int playerUniqueId; //ゲームのユニークID
	public PlayerListInRoomController playerListInRoomController;
	public DuelPlayerListInRoomController duelPlayerListInRoomController;
	public string onlineGameMode;
	public DuelCustomNetworkLobbyManager duelCustomNetworkLobbyManager;

	[SyncVar] public string syncLobbyPlayerName;

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

	void Start(){
		duelCustomNetworkLobbyManager = GameObject.Find("/MainCanvas/BattlePanel/BattlePanelOnlineDuel").GetComponent<DuelCustomNetworkLobbyManager> ();
	}

	// Update is called once per frame
	void Update () {
		if(syncLobbyPlayerName != ""){
			CreateLobbyPlayerListPrefab (syncLobbyPlayerName + " " + unitParamsClass[0].unit_name.ToString(), unitParamsClass[0].unit_icon_url.ToString());
		}
	}

	//こいつが削除されるとき道連れでLobbyPlayerListPrefabも削除する
	void OnDestroy(){
		DestroyLobbyPlayerListPrefab ();
	}

	//syncLobbyPlayerNameに値が入ったらNodeをprefabから作る 今は使われてない
	void SyncLobbyPlayerNameValues(string playerName){
		if(playerName != ""){
			CreateLobbyPlayerListPrefab (playerName + unitParamsClass[0].unit_id.ToString(), unitParamsClass[0].unit_icon_url.ToString());
		}
	}

	//LobbyPlayerListPrefabの生成をlobbyManagerに依頼(１度だけ)
	void CreateLobbyPlayerListPrefab(string lobbyPlayerName, string unitIconUrl){
		if(!setLobbyPlayerNodeFlag){
			setLobbyPlayerNodeFlag = true;

			//			if(onlineGameMode == "online"){
			//				LobbyPlayerListPrefab = playerListInRoomController.CreateLobbyPlayerListPrefab(lobbyPlayerName, unitIconUrl);
			//			} else if(onlineGameMode == "duel"){
			//				LobbyPlayerListPrefab = duelPlayerListInRoomController.CreateLobbyPlayerListPrefab(lobbyPlayerName, unitIconUrl);
			//			}
			LobbyPlayerListPrefab = duelCustomNetworkLobbyManager.CreateLobbyPlayerListPrefab(lobbyPlayerName, unitIconUrl);

			//ホストのスクロールビューの順番をゲームのユニークIDとして使う
			playerUniqueId = LobbyPlayerListPrefab.transform.GetSiblingIndex ();
		}
	}

	public void DestroyLobbyPlayerListPrefab(){
		Destroy(LobbyPlayerListPrefab);
	}

	public void ProvideLobbyPlayerNameToServer (){
		GameStateManager gameStateManager = GameObject.Find("/GameStateManager").GetComponent<GameStateManager> ();
		Dictionary<int,OwnedUnitData> mainPartyDic = gameStateManager.partyUnitDic;
		//PartyDicの一番上をリーダーとする
		//OwnedUnitData LeaderUnitParam = mainPartyDic.First().Value;

		//CmdProvideLobbyPlayerDataToServer(gameStateManager.AccountName, LeaderUnitParam.unit_id, LeaderUnitParam.unit_account_id, LeaderUnitParam.unit_name, LeaderUnitParam.unit_icon_url, LeaderUnitParam.attack, LeaderUnitParam.hitPoint, LeaderUnitParam.speed, LeaderUnitParam.type, LeaderUnitParam.Level, LeaderUnitParam.combo, LeaderUnitParam.ability_1, LeaderUnitParam.ability_2, LeaderUnitParam.ability_3, LeaderUnitParam.strikeShot, LeaderUnitParam.comboType, LeaderUnitParam.comboAttack, LeaderUnitParam.maxComboNum);
		CmdProvideLobbyPlayerDataToServer(gameStateManager.AccountName, mainPartyDic);
	}

	[Command]
	void CmdProvideLobbyPlayerDataToServer (string lobbyPlayerName, Dictionary<int, OwnedUnitData>  mainPartyDic){
		foreach(KeyValuePair<int, OwnedUnitData> mainPartyUnitData in  mainPartyDic){
			UnitParams unitParams = new UnitParams ();
			unitParams.unit_id = mainPartyUnitData.Value.unit_id;
			unitParams.unit_account_id = mainPartyUnitData.Value.unit_account_id;
			unitParams.unit_name = mainPartyUnitData.Value.unit_name;
			unitParams.unit_icon_url = mainPartyUnitData.Value.unit_icon_url;
			unitParams.attack = mainPartyUnitData.Value.attack;
			unitParams.hitPoint = mainPartyUnitData.Value.hitPoint;
			unitParams.speed = mainPartyUnitData.Value.speed;
			unitParams.type = mainPartyUnitData.Value.type;
			unitParams.Level = mainPartyUnitData.Value.Level;
			unitParams.combo = mainPartyUnitData.Value.combo;
			unitParams.ability_1 = mainPartyUnitData.Value.ability_1;
			unitParams.ability_2 = mainPartyUnitData.Value.ability_2;
			unitParams.ability_3 = mainPartyUnitData.Value.ability_3;
			unitParams.strikeShot = mainPartyUnitData.Value.strikeShot;
			unitParams.comboType = mainPartyUnitData.Value.comboType;
			unitParams.comboAttack = mainPartyUnitData.Value.comboAttack;
			unitParams.maxComboNum = mainPartyUnitData.Value.maxComboNum;
	
			unitParamsClass.Add(unitParams);
		}

		syncLobbyPlayerName = lobbyPlayerName;
	}

//	[Command]
//	void CmdProvideLobbyPlayerDataToServer (string lobbyPlayerName, int _unit_id, string _unit_account_id ,string _unit_name, string _unit_icon_url, int _attack, int _hitPoint, float _speed, int _type, int _Level, int _combo, int _ability_1, int _ability_2, int _ability_3, int _strikeShot, int _comboType, int _comboAttack, int _maxComboNum){
//		UnitParams unitParams = new UnitParams ();
//		unitParams.unit_id = _unit_id;
//		unitParams.unit_account_id = _unit_account_id;
//		unitParams.unit_name = _unit_name;
//		unitParams.unit_icon_url = _unit_icon_url;
//		unitParams.attack = _attack;
//		unitParams.hitPoint = _hitPoint;
//		unitParams.speed = _speed;
//		unitParams.type = _type;
//		unitParams.Level = _Level;
//		unitParams.combo = _combo;
//		unitParams.ability_1 = _ability_1;
//		unitParams.ability_2 = _ability_2;
//		unitParams.ability_3 = _ability_3;
//		unitParams.strikeShot = _strikeShot;
//		unitParams.comboType = _comboType;
//		unitParams.comboAttack = _comboAttack;
//		unitParams.maxComboNum = _maxComboNum;
//
//		unitParamsClass.Add(unitParams);
//
//		//ここでSyncLobbyPlayerNameValues()が発火するので必ずunitParamsClass.Addした後に行うこと ※SyncLobbyPlayerNameValuesは現在未実装
//		syncLobbyPlayerName = lobbyPlayerName;
//	}
}
