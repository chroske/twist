using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class DuelCustomNetworkLobbyPlayer : NetworkLobbyPlayer
{
	private LobbyPlayerController duelLobbyPlayerController;

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
		DuelLobbyPlayerController duelLobbyPlayerController = transform.GetComponent<DuelLobbyPlayerController> ();
		duelLobbyPlayerController.ProvideLobbyPlayerNameToServer ();
	}
}
