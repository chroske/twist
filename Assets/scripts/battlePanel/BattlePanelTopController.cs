using UnityEngine;
using System.Collections;

public class BattlePanelTopController : MonoBehaviour {

	[SerializeField]
	GameStateManager gameStateManager;
	[SerializeField]
	CustomNetworkLobbyManager customNetworkLobbyManager;
	[SerializeField]
	CustomNetworkLobbyPlayer lobbyPlayer;
	[SerializeField]
	DuelCustomNetworkLobbyPlayer duelLobbyPlayer;
	[SerializeField]
	GameObject networkPlayerManager;
	[SerializeField]
	GameObject duelNetworkPlayerManager;

	public void OnClickOnlineButton(){
		gameStateManager.onlineGameMode = "online";
//		customNetworkLobbyManager.playScene = "GameMain";
//		customNetworkLobbyManager.lobbyPlayerPrefab = lobbyPlayer;
//		customNetworkLobbyManager.gamePlayerPrefab = networkPlayerManager;
	}

	public void OnClickDuelButton(){
		gameStateManager.onlineGameMode = "duel";
//		customNetworkLobbyManager.playScene = "GameDuel";
//		customNetworkLobbyManager.lobbyPlayerPrefab = duelLobbyPlayer;
//		customNetworkLobbyManager.gamePlayerPrefab = duelNetworkPlayerManager;
	}
}
