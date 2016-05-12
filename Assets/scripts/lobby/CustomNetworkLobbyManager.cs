﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;

public class CustomNetworkLobbyManager : NetworkLobbyManager
{
	private NetworkMatch networkMatch;
	private NetworkManager networkManager;
	//private NetworkInstanceId playerNetID;
	private short playerNetID;
	private GameObject currentPanel;
	private bool isHost = false;
	private int roomListId;

	public GameObject matchPanel1;
	public GameObject matchPanel2;
	public GameObject matchPanel3;
	public GameObject matchPanel4;

	[SerializeField]
	private GameObject lobbyContent;

	[SerializeField]
	private GameObject lobbyPlayerListNode;

	void Start(){
		//ロビーTOPパネルをcurrentに
		currentPanel = matchPanel2;
	}
		
	//クライアントがロビーのシーンからゲームプレイヤーシーンに切り替えが終了したことを伝えられたときサーバー上で呼び出し
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
	{
		//lobbyPlayerオブジェクトのnetidをnetworkManagerControllerに送って以後ゲーム中idとして使う
		NetworkManagerController networkManagerController = gamePlayer.GetComponent<NetworkManagerController>();
		//playerNetID = lobbyPlayer.GetComponent<NetworkIdentity>().netId;
		//int playerNetIdInt =  int.Parse(playerNetID.ToString());
		int playerNetIdInt = lobbyPlayer.GetComponent<LobbyPlayerController>().playerUniqueId;
		networkManagerController.syncPlayerNetID = playerNetIdInt;

		//ユニットのパラメータをlobbyPlayerからgamePlayerに渡す
		LobbyPlayerController lobbyPlayerController = lobbyPlayer.GetComponent<LobbyPlayerController>();

		//networkManagerControllerでsyncの構造体を定義すると何故か落ちるのでバラで渡す
		networkManagerController.syncUnitId = lobbyPlayerController.unitParamsClass [0].unit_id;
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

		StartCoroutine (matchPanel3.GetComponent<RoomListController>().PullBackScrollView (matchList));
	}

	//ゲームシーンへ移行時にクライアントで呼び出される
	public override void OnClientSceneChanged(NetworkConnection conn){
		currentPanel.SetActive (false);
		base.OnClientSceneChanged(conn);
	}

	public void StartTheGame(){
		currentPanel.SetActive (false);
		ServerChangeScene (playScene);
	}

	public void ExitRoom(){
		if (isHost) {
			StopHost ();
		} else {
			StopClient();
		}
		StopMatchMaker();

		currentPanel.SetActive (false);
		matchPanel2.SetActive (true);

		currentPanel = matchPanel2;
	}

	private void OnDisconnected(BasicResponse response)
	{
		if (response.success) {
			networkManager.StopMatchMaker ();

			currentPanel.SetActive (false);
			matchPanel2.SetActive (true);

			currentPanel = matchPanel2;
		}
	}

	//ホストとしてゲームを開始したときを含めサーバーを起動したとき、これはサーバー上で呼び出されます。
	public override void OnLobbyStartServer() {
		isHost = true;
		matchPanel4.GetComponent<PlayerListInRoomController> ().SetActiveStartGameButton ();
	}

	public GameObject CreateLobbyPlayerListPrefab(string lobbyPlayerName){
		GameObject prefab = Instantiate (lobbyPlayerListNode);
		prefab.transform.SetParent (lobbyContent.transform,false);
		prefab.transform.GetComponentInChildren<Text>().text = lobbyPlayerName;

		return prefab;
	}

	private void StartMatchMake(){
		networkManager = transform.GetComponent<NetworkManager> ();
		networkManager.StartMatchMaker();
		networkMatch = networkManager.matchMaker;
	}

	public void StopMatchMake(){
		networkManager.StopMatchMaker();

		currentPanel.SetActive (false);
		matchPanel1.SetActive (true);

		currentPanel = matchPanel1;
	}

	public void CreateMatch(){
		StartMatchMake ();

		string matchRoomName = "TESTTESTBB";
		int rank = 1;

		string roomName = "{" + rank.ToString() + "}" + matchRoomName;
		networkManager.matchName = roomName;
		networkManager.matchSize = 4U;
		networkMatch.CreateMatch (networkManager.matchName, networkManager.matchSize, true, "", networkManager.OnMatchCreate);

		currentPanel.SetActive (false);
		matchPanel4.SetActive (true);

		currentPanel = matchPanel4;
	}

	public void GetListMatch(int rank){
		StartMatchMake ();

		networkMatch.ListMatches(0, 20, /*"{"+rank+"}"*/"", networkManager.OnMatchList);

		currentPanel.SetActive (false);
		matchPanel3.SetActive (true);

		currentPanel = matchPanel3;
	}

	public void JoinListMatch(int ListId){
		StartMatchMake ();

		var desc = networkManager.matches [ListId];
		networkMatch.JoinMatch (desc.networkId, "", CustamOnMatchJoined);

		currentPanel.SetActive (false);
		matchPanel4.SetActive (true);

		currentPanel = matchPanel4;
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

	private void ListMatchAndJoinMatchCallBack(ListMatchResponse matchList){
		matches = matchList.matches;
		if (networkManager.matches.Count != 0) {
			var desc = matches [roomListId];
			networkMatch.JoinMatch (desc.networkId, "", CustamOnMatchJoined);
			currentPanel.SetActive (false);
			matchPanel4.SetActive (true);

			currentPanel = matchPanel4;
		} else {
			Debug.Log("RoomCount=0");
		}
	}
}
