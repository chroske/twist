using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CustamNetworkLobbyPlayer : NetworkLobbyPlayer
{
	public override void OnClientEnterLobby()
	{
		if(isLocalPlayer){
			
		}

		GameObject lobbyManager = GameObject.Find("LobbyManager");
		lobbyManager.transform.GetComponent<CustomNetworkLobbyManager> ().CreateLobbyPlayerListPrefab();
	}

	//命名
	public void SetLobbyPlayerName(string PlayerName){
		//LobbyPlayerNameText.GetComponentInChildren<Text>().text = PlayerName;
	}
}
