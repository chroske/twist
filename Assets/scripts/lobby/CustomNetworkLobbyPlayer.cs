﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CustomNetworkLobbyPlayer : NetworkLobbyPlayer
{
	private LobbyPlayerController lobbyPlayerController;

//	public override void OnClientEnterLobby()
//	{
//		Debug.Log ("OnClientEnterLobby");
//	}
//
//	public override void OnClientExitLobby()
//	{
//		Debug.Log ("OnClientExitLobby");
//	}

	//ローカルプレイヤーのinstanceが作られたら実行される
	public override void OnStartLocalPlayer(){
		LobbyPlayerController lobbyPlayerController = transform.GetComponent<LobbyPlayerController> ();
		lobbyPlayerController.ProvideLobbyPlayerNameToServer ();
	}
}
