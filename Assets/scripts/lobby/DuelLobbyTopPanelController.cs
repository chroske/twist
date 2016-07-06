using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class DuelLobbyTopPanelController : MonoBehaviour {

	private CustomNetworkLobbyManager customNetworkLobbyManager;

	[SerializeField]
	GameObject LobbyManager;

	void Start () {
		customNetworkLobbyManager = LobbyManager.GetComponent<CustomNetworkLobbyManager> ();
	}

	public void OnRundamMatchButton(int ListId){
		customNetworkLobbyManager.JoinDuelRundamMatch(0);
	}
}