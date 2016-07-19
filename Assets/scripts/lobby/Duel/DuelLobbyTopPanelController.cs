using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class DuelLobbyTopPanelController : MonoBehaviour {

	private DuelCustomNetworkLobbyManager duelCustomNetworkLobbyManager;

	[SerializeField]
	GameObject LobbyManager;

	void Start () {
		duelCustomNetworkLobbyManager = LobbyManager.GetComponent<DuelCustomNetworkLobbyManager> ();
	}

	public void OnRundamMatchButton(int ListId){
		duelCustomNetworkLobbyManager.JoinDuelRundamMatch(0);
	}
}