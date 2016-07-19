using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

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
