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
	protected ulong _currentMatchID;
	protected ulong _currentMatchNodeID;

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

	//修正必須
	[SerializeField]
	GameObject content;
	[SerializeField]
	GameObject contentDuel;

	void Start(){
		GameObject gameStateManagerObj = GameObject.Find("/GameStateManager");
		gameStateManager = gameStateManagerObj.GetComponent<GameStateManager> ();
	}
		
	//クライアントがロビーのシーンからゲームプレイヤーシーンに切り替えが終了したことを伝えられたときサーバー上で呼び出し
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
	{
		//lobbyPlayerオブジェクトのnetidをnetworkManagerControllerに送って以後ゲーム中idとして使う
		NetworkPlayerController networkPlayerController = gamePlayer.GetComponent<NetworkPlayerController>();
		int playerNetIdInt = lobbyPlayer.GetComponent<LobbyPlayerController>().playerUniqueId;
		networkPlayerController.syncPlayerNetID = playerNetIdInt;

		//ユニットのパラメータをlobbyPlayerからgamePlayerに渡す
		LobbyPlayerController lobbyPlayerController = lobbyPlayer.GetComponent<LobbyPlayerController>();

		//networkManagerControllerでsyncの構造体を定義すると何故か落ちるのでバラで渡す
		networkPlayerController.syncUnitId = lobbyPlayerController.unitParamsClass [0].unit_id;
		networkPlayerController.syncUnitAccountId = lobbyPlayerController.unitParamsClass [0].unit_account_id;
		networkPlayerController.syncUnitName = lobbyPlayerController.unitParamsClass [0].unit_name;
		networkPlayerController.syncUnitIconUrl = lobbyPlayerController.unitParamsClass [0].unit_icon_url;
		networkPlayerController.syncUnitAttack = lobbyPlayerController.unitParamsClass [0].attack;
		networkPlayerController.syncUnitHitpoint = lobbyPlayerController.unitParamsClass [0].hitPoint;
		networkPlayerController.syncUnitSpeed = lobbyPlayerController.unitParamsClass [0].speed;
		networkPlayerController.syncUnitType = lobbyPlayerController.unitParamsClass [0].type;
		networkPlayerController.syncUnitLevel = lobbyPlayerController.unitParamsClass [0].Level;
		networkPlayerController.syncUnitCombo = lobbyPlayerController.unitParamsClass [0].combo;
		networkPlayerController.syncUintAbbility_1 = lobbyPlayerController.unitParamsClass [0].ability_1;
		networkPlayerController.syncUintAbbility_2 = lobbyPlayerController.unitParamsClass [0].ability_2;
		networkPlayerController.syncUintAbbility_3 = lobbyPlayerController.unitParamsClass [0].ability_3;
		networkPlayerController.syncUintStrikeShot = lobbyPlayerController.unitParamsClass [0].strikeShot;
		networkPlayerController.syncUintComboType = lobbyPlayerController.unitParamsClass [0].comboType;
		networkPlayerController.syncUintComboAttack = lobbyPlayerController.unitParamsClass [0].comboAttack;
		networkPlayerController.syncUintMaxComboNum = lobbyPlayerController.unitParamsClass [0].maxComboNum;

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
			//DestroyMatchしないとマッチが残って邪魔
			this.matchMaker.DestroyMatch((NetworkID)_currentMatchID, OnMatchDestroyedHost);
		} else {
			this.matchMaker.DropConnection((NetworkID)_currentMatchID, (NodeID)_currentMatchNodeID , OnMatchDestroyedClient);
		}
	}

	public void OnMatchDestroyedHost(BasicResponse resp)
	{
		StopMatchMaker();
		StopHost();
	}

	public void OnMatchDestroyedClient(BasicResponse resp)
	{
		StopMatchMaker();
		StopClient();
	}

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
		if (desc.currentSize < desc.maxSize) {
			networkMatch.JoinMatch (desc.networkId, "", CustamOnMatchJoined);
		} else {
			Debug.Log ("player num limit over");
		}
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

			if (desc.currentSize < desc.maxSize) {
				networkMatch.JoinMatch (desc.networkId, "", CustamOnMatchJoined);
				battlePanelOnlineDuelController.ChangePanel (3);
			} else {
				Debug.Log ("player num limit over");
			}
		} else {
			Debug.Log("RoomCount=0");
			CreatelRandumMatchRoom();
		}
	}

	private void CreatelRandumMatchRoom(){
		string matchRoomName = "RANDOM MATCH ROOM";
		int rank = 1;

		string roomName = "{" + rank.ToString() + "}" + matchRoomName;
		networkManager.matchName = roomName;
		networkManager.matchSize = 4U;
		networkMatch.CreateMatch (networkManager.matchName, networkManager.matchSize, true, "", networkManager.OnMatchCreate);
		battlePanelOnlineDuelController.ChangePanel(3);
	}

	public override void OnMatchCreate(UnityEngine.Networking.Match.CreateMatchResponse matchInfo)
	{
		base.OnMatchCreate(matchInfo);

		_currentMatchID = (System.UInt64)matchInfo.networkId;
		_currentMatchNodeID = (System.UInt64)matchInfo.nodeId;
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

		if(gameStateManager.onlineGameMode == "online"){
			battlePanelOnlineController.ChangeGameScene ();
		} else if(gameStateManager.onlineGameMode == "duel"){
			battlePanelOnlineDuelController.ChangeGameScene ();
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
//		LobbyPlayerController lobbyPlayerController = newLobbyPlayer.GetComponent<LobbyPlayerController> ();
//
//		lobbyPlayerController.onlineGameMode = gameStateManager.onlineGameMode;
//		if(gameStateManager.onlineGameMode == "online"){
//			lobbyPlayerController.playerListInRoomController = playerListInRoomController;
//		} else if(gameStateManager.onlineGameMode == "duel"){
//			lobbyPlayerController.duelPlayerListInRoomController = duelPlayerListInRoomController;
//		}

		return newLobbyPlayer;
	}


	//ScrollViewにnodeを作る
	public GameObject CreateLobbyPlayerListPrefab(string lobbyPlayerName, string unitIconUrl){
		GameObject node = Instantiate (lobbyPlayerListNode);
		if(gameStateManager.onlineGameMode == "online"){
			node.transform.SetParent (content.transform,false);
		} else if(gameStateManager.onlineGameMode == "duel"){
			node.transform.SetParent (contentDuel.transform,false);
		}

		node.transform.GetComponent<LobbyPlayerListNodeController> ().SetNodeDatas (lobbyPlayerName, unitIconUrl);

		return node;
	}
}
