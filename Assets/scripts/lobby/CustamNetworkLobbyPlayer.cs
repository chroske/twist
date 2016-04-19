using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CustamNetworkLobbyPlayer : NetworkLobbyPlayer
{
	private LobbyPlayerController lobbyPlayerController;

	public override void OnClientEnterLobby()
	{
	}

	//ローカルプレイヤーのinstanceが作られたら実行される
	public override void OnStartLocalPlayer(){
		
		LobbyPlayerController lobbyPlayerController = transform.GetComponent<LobbyPlayerController> ();
		lobbyPlayerController.ProvideLobbyPlayerNameToServer ();
	}
}
