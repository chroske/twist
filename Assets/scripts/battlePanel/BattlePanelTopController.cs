using UnityEngine;
using System.Collections;

public class BattlePanelTopController : MonoBehaviour {

	[SerializeField]
	GameStateManager gameStateManager;
	[SerializeField]
	CustomNetworkLobbyManager customNetworkLobbyManager;

	public void OnClickOnlineButton(){
		gameStateManager.onlineGameMode = "online";
		customNetworkLobbyManager.playScene = "GameMain";
	}

	public void OnClickDuelButton(){
		gameStateManager.onlineGameMode = "duel";
		customNetworkLobbyManager.playScene = "GameDuel";
	}
}
