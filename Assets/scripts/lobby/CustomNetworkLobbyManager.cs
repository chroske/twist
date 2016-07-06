using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public class CustomNetworkLobbyManager : NetworkLobbyManager
{
	private NetworkMatch networkMatch;
	private NetworkManager networkManager;
	private GameStateManager gameStateManager;
	private GameObject currentPanel;
	private short playerNetID;
	private int roomListId;
	private bool inTheGame; //ゲームシーンにいるかどうか

	public GameObject HeaderAndTabbar;

	public bool isHost = false; //ホストかどうか

	[SerializeField]
	GameObject lobbyContent;
	[SerializeField]
	GameObject lobbyPlayerListNode;
	[SerializeField]
	GameObject battlePanelOnline;
	[SerializeField]
	BattlePanelOnlineController battlePanelOnlineController;
	[SerializeField]
	GameObject battlePanelOnlineDuel;
	[SerializeField]
	BattlePanelOnlineDuelController battlePanelOnlineDuelController;
	[SerializeField]
	PlayerListInRoomController playerListInRoomController;
	[SerializeField]
	DuelPlayerListInRoomController duelPlayerListInRoomController;

	void Start(){
		GameObject gameStateManagerObj = GameObject.Find("/GameStateManager");
		gameStateManager = gameStateManagerObj.GetComponent<GameStateManager> ();
	}
		
	//クライアントがロビーのシーンからゲームプレイヤーシーンに切り替えが終了したことを伝えられたときサーバー上で呼び出し
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
	{
		//lobbyPlayerオブジェクトのnetidをnetworkManagerControllerに送って以後ゲーム中idとして使う
		NetworkPlayerManager networkManagerController = gamePlayer.GetComponent<NetworkPlayerManager>();
		int playerNetIdInt = lobbyPlayer.GetComponent<LobbyPlayerController>().playerUniqueId;
		networkManagerController.syncPlayerNetID = playerNetIdInt;

		//ユニットのパラメータをlobbyPlayerからgamePlayerに渡す
		LobbyPlayerController lobbyPlayerController = lobbyPlayer.GetComponent<LobbyPlayerController>();

		//networkManagerControllerでsyncの構造体を定義すると何故か落ちるのでバラで渡す
		networkManagerController.syncUnitId = lobbyPlayerController.unitParamsClass [0].unit_id;
		networkManagerController.syncUnitAccountId = lobbyPlayerController.unitParamsClass [0].unit_account_id;
		networkManagerController.syncUnitName = lobbyPlayerController.unitParamsClass [0].unit_name;
		networkManagerController.syncUnitIconUrl = lobbyPlayerController.unitParamsClass [0].unit_icon_url;
		networkManagerController.syncUnitAttack = lobbyPlayerController.unitParamsClass [0].attack;
		networkManagerController.syncUnitHitpoint = lobbyPlayerController.unitParamsClass [0].hitPoint;
		networkManagerController.syncUnitSpeed = lobbyPlayerController.unitParamsClass [0].speed;
		networkManagerController.syncUnitType = lobbyPlayerController.unitParamsClass [0].type;
		networkManagerController.syncUnitLevel = lobbyPlayerController.unitParamsClass [0].Level;
		networkManagerController.syncUnitCombo = lobbyPlayerController.unitParamsClass [0].combo;
		networkManagerController.syncUintAbbility_1 = lobbyPlayerController.unitParamsClass [0].ability_1;
		networkManagerController.syncUintAbbility_2 = lobbyPlayerController.unitParamsClass [0].ability_2;
		networkManagerController.syncUintAbbility_3 = lobbyPlayerController.unitParamsClass [0].ability_3;
		networkManagerController.syncUintStrikeShot = lobbyPlayerController.unitParamsClass [0].strikeShot;
		networkManagerController.syncUintComboType = lobbyPlayerController.unitParamsClass [0].comboType;
		networkManagerController.syncUintComboAttack = lobbyPlayerController.unitParamsClass [0].comboAttack;
		networkManagerController.syncUintMaxComboNum = lobbyPlayerController.unitParamsClass [0].maxComboNum;

		return true;
	}

	//マッチの一覧を ListMatches() から取得した場合に実行
	public override void OnMatchList(ListMatchResponse matchList)
	{
		matches = matchList.matches;
	}

	//ゲームシーンへ移行時にクライアントで呼び出される
	public override void OnClientSceneChanged(NetworkConnection conn){
		//currentPanel.SetActive (false);
		//HeaderAndTabbar.SetActive (false);
		base.OnClientSceneChanged(conn);
	}

	public void StartTheGame(){
		gameStateManager.onlineGame = true;
		ServerChangeScene (playScene);
	}

	public void ExitRoom(){
		if (isHost) {
			StopHost ();
		} else {
			StopClient();
		}
		StopMatchMaker();
	}
//
//	public void ExitRoomList(){
//		if(matchPanel3.activeSelf){
//			//ルーム一覧クリア
//			matchPanel3.GetComponent<RoomListController> ().ClearRoomList ();
//
//			battlePanelOnlineController.ChangePanel(1);
//		}
//	}

//	private void OnDisconnected(BasicResponse response)
//	{
//		if (response.success) {
//			networkManager.StopMatchMaker ();
//			battlePanelOnlineController.ChangePanel(1);
//		}
//	}

	//ホストとしてゲームを開始したときを含めサーバーを起動したとき、これはサーバー上で呼び出されます。
	public override void OnLobbyStartServer() {
		isHost = true;

		if (gameStateManager.onlineGameMode == "online") {
			battlePanelOnlineController.SetActiveStartGameButtonInRoom ();
		} else if(gameStateManager.onlineGameMode == "duel"){
			battlePanelOnlineDuelController.SetActiveStartGameButtonInRoom ();
		}

	}

	private void StartMatchMake(){
		networkManager = transform.GetComponent<NetworkManager> ();
		networkManager.StartMatchMaker();
		networkMatch = networkManager.matchMaker;
	}

	public void StopMatchMake(){
		networkManager.StopMatchMaker();
	}

	public void CreateMatch(){
		StartMatchMake ();

		string matchRoomName = "ONLINE ROOM";
		int rank = 1;

		string roomName = "{" + rank.ToString() + "}" + matchRoomName;
		networkManager.matchName = roomName;
		networkManager.matchSize = 4U;
		networkMatch.CreateMatch (networkManager.matchName, networkManager.matchSize, true, "", networkManager.OnMatchCreate);
	}

	//ルームリストパネルに遷移した時にデータ取ってくる処理
	public void GetListMatch(int rank){
		StartMatchMake ();
		networkMatch.ListMatches(0, 20, /*"{"+rank+"}"*/"", ListMatchCallBack);
	}

	public void JoinListMatch(int ListId){
		StartMatchMake ();

		var desc = networkManager.matches [ListId];
		networkMatch.JoinMatch (desc.networkId, "", CustamOnMatchJoined);
	}

	public void CustamOnMatchJoined(JoinMatchResponse matchJoin)
	{
		if (matchJoin.success)
		{
			Debug.Log("Join match succeeded");
			StartClient(new MatchInfo(matchJoin));
		} else {
			Debug.LogError("Join match failed");
		}
	}

	public void ListMatchAndJoinMatch(int ListId){
		StartMatchMake ();
		roomListId = ListId;
		networkMatch.ListMatches(0, 20, /*"{"+rank+"}"*/"", ListMatchAndJoinMatchCallBack);
	}

	public void JoinDuelRundamMatch(int ListId){
		StartMatchMake ();
		roomListId = ListId;
		networkMatch.ListMatches(0, 20, /*"{"+rank+"}"*/"", JoinDuelRundamMatchCallBack);
	}


	private void ListMatchCallBack(ListMatchResponse matchList){
		matches = matchList.matches;
		if (networkManager.matches.Count != 0) {
			battlePanelOnlineController.GenerateMatchList (matchList);
		} else {
			Debug.Log("RoomCount=0");
		}
		Debug.Log("ListMatchCallBack");

		battlePanelOnlineController.ChangePanel(2);
	}

	private void ListMatchAndJoinMatchCallBack(ListMatchResponse matchList){
		matches = matchList.matches;
		if (networkManager.matches.Count != 0) {
			var desc = matches [roomListId];
			networkMatch.JoinMatch (desc.networkId, "", CustamOnMatchJoined);
			battlePanelOnlineController.ChangePanel(3);
		} else {
			Debug.Log("RoomCount=0");
		}
	}

	private void JoinDuelRundamMatchCallBack(ListMatchResponse matchList){
		matches = matchList.matches;
		if (networkManager.matches.Count != 0) {
			var desc = matches [roomListId];
			networkMatch.JoinMatch (desc.networkId, "", CustamOnMatchJoined);
			battlePanelOnlineDuelController.ChangePanel(3);
		} else {
			Debug.Log("RoomCount=0");
			CreatelRundamMatchRoom();
		}
	}

	private void CreatelRundamMatchRoom(){
		string matchRoomName = "RUNDAM MATCH ROOM";
		int rank = 1;

		string roomName = "{" + rank.ToString() + "}" + matchRoomName;
		networkManager.matchName = roomName;
		networkManager.matchSize = 4U;
		networkMatch.CreateMatch (networkManager.matchName, networkManager.matchSize, true, "", networkManager.OnMatchCreate);
		battlePanelOnlineDuelController.ChangePanel(3);
	}

	//GameSceneからLobbyに戻った時のPanel設定
	public void SetUpOfflinePlayTop(){
		battlePanelOnline.SetActive (true);
		HeaderAndTabbar.SetActive (true);
	}

	public override void OnLobbyClientSceneChanged(NetworkConnection conn){
		Debug.Log ("OnLobbyClientSceneChanged");
		if(!inTheGame){
			inTheGame = true;
		}
	}

	public override void OnLobbyStopClient(){
		Debug.Log ("OnLobbyStopClient");
		if(inTheGame){
			inTheGame = false;				
			if(gameStateManager.onlineGameMode == "online"){
				battlePanelOnlineController.SetUpLobby ();
			} else if(gameStateManager.onlineGameMode == "duel"){
				battlePanelOnlineDuelController.SetUpLobby ();
			}
		}
	}

	public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControlID){
		GameObject newLobbyPlayer = Instantiate(lobbyPlayerPrefab.gameObject);
		LobbyPlayerController lobbyPlayerController = newLobbyPlayer.GetComponent<LobbyPlayerController> ();

		lobbyPlayerController.onlineGameMode = gameStateManager.onlineGameMode;
		if(gameStateManager.onlineGameMode == "online"){
			lobbyPlayerController.playerListInRoomController = playerListInRoomController;
		} else if(gameStateManager.onlineGameMode == "duel"){
			lobbyPlayerController.duelPlayerListInRoomController = duelPlayerListInRoomController;
		}

		return newLobbyPlayer;
	}
}
