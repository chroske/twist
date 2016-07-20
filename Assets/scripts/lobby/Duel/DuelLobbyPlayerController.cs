using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class DuelLobbyPlayerController : NetworkBehaviour {

	private GameObject LobbyPlayerListPrefab;
	private bool setLobbyPlayerNodeFlag;
	public int playerUniqueId; //ゲームのユニークID
	public PlayerListInRoomController playerListInRoomController;
	public DuelPlayerListInRoomController duelPlayerListInRoomController;
	public string onlineGameMode;

	[SyncVar] public string syncLobbyPlayerName;

	//public class UnitParamsClass : SyncListStruct<UnitParams> {}
	public UnitParamsData unitParamsData = new UnitParamsData();
//	public struct UnitParams
//	{
//		public int unit_id;
//		public string unit_account_id;
//		public string unit_name;
//		public string unit_icon_url;
//		public int attack;
//		public int hitPoint;
//		public float speed;
//		public int type;
//		public int Level;
//		public int combo;
//		public int ability_1;
//		public int ability_2;
//		public int ability_3;
//		public int strikeShot;
//		public int comboType;
//		public int comboAttack;
//		public int maxComboNum;
//	}

	void Update () {
		if(syncLobbyPlayerName != ""){
			CreateLobbyPlayerListPrefab (syncLobbyPlayerName + " " + unitParamsData[0].unit_name.ToString(), unitParamsData[0].unit_icon_url.ToString());
		}
	}

	//こいつが削除されるとき道連れでLobbyPlayerListPrefabも削除する
	void OnDestroy(){
		DestroyLobbyPlayerListPrefab ();
	}

	//syncLobbyPlayerNameに値が入ったらNodeをprefabから作る 今は使われてない
	void SyncLobbyPlayerNameValues(string playerName){
		if(playerName != ""){
			CreateLobbyPlayerListPrefab (playerName + unitParamsData[0].unit_id.ToString(), unitParamsData[0].unit_icon_url.ToString());
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
			DuelCustomNetworkLobbyManager duelCustomNetworkLobbyManager = GameObject.Find("/MainCanvas/BattlePanel/BattlePanelOnlineDuel").GetComponent<DuelCustomNetworkLobbyManager> ();
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

		foreach (KeyValuePair<int, OwnedUnitData> mainPartyUnitData in  mainPartyDic) {
			CmdProvideLobbyPlayerDataToServer(mainPartyUnitData.Value.unit_id, mainPartyUnitData.Value.unit_account_id, mainPartyUnitData.Value.unit_name, mainPartyUnitData.Value.unit_icon_url, mainPartyUnitData.Value.attack, mainPartyUnitData.Value.hitPoint, mainPartyUnitData.Value.speed, mainPartyUnitData.Value.type, mainPartyUnitData.Value.Level, mainPartyUnitData.Value.combo, mainPartyUnitData.Value.ability_1, mainPartyUnitData.Value.ability_2, mainPartyUnitData.Value.ability_3, mainPartyUnitData.Value.strikeShot, mainPartyUnitData.Value.comboType, mainPartyUnitData.Value.comboAttack, mainPartyUnitData.Value.maxComboNum);
		}

		CmdProvideLobbyPlayerNameToServer(gameStateManager.AccountName);
	}

	[Command]
	void CmdProvideLobbyPlayerDataToServer (int _unit_id, string _unit_account_id ,string _unit_name, string _unit_icon_url, int _attack, int _hitPoint, float _speed, int _type, int _Level, int _combo, int _ability_1, int _ability_2, int _ability_3, int _strikeShot, int _comboType, int _comboAttack, int _maxComboNum){
		UnitParams unitParams = new UnitParams ();
		unitParams.unit_id = _unit_id;
		unitParams.unit_account_id = _unit_account_id;
		unitParams.unit_name = _unit_name;
		unitParams.unit_icon_url = _unit_icon_url;
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

		unitParamsData.Add(unitParams);
	}

	[Command]
	void CmdProvideLobbyPlayerNameToServer(string lobbyPlayerName){
		syncLobbyPlayerName = lobbyPlayerName;
	}
}
