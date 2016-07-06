﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PlayerListInRoomController : NetworkBehaviour {

	[SerializeField]
	CustomNetworkLobbyManager customNetworkLobbyManager;
	[SerializeField]
	GameObject StartGameButton;
	[SerializeField]
	GameObject ExitButton;
	[SerializeField]
	BattlePanelOnlineController battlePanelOnlineController;
	[SerializeField]
	GameObject content;
	[SerializeField]
	GameObject lobbyPlayerListNode;

	//ScrollViewにnodeを作る
	public GameObject CreateLobbyPlayerListPrefab(string lobbyPlayerName, string unitIconUrl){
		GameObject node = Instantiate (lobbyPlayerListNode);
		node.transform.SetParent (content.transform,false);
		node.transform.GetComponent<LobbyPlayerListNodeController> ().SetNodeDatas (lobbyPlayerName, unitIconUrl);

		return node;
	}


	//ゲームスタートボタン
	public void OnClickStartGameButton(){
		battlePanelOnlineController.StartTheGame();
	}

	//ルーム退出ボタン
	public void OnClickExitButton(){
		battlePanelOnlineController.ExitRoom();
	}

	public void SetActiveStartGameButton(){
		StartGameButton.SetActive (true);
	}
}
