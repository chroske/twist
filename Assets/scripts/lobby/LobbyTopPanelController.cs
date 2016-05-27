using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class LobbyTopPanelController : MonoBehaviour {

	private CustomNetworkLobbyManager customNetworkLobbyManager;

	[SerializeField]
	private GameObject LobbyManager;

	void Start () {
		customNetworkLobbyManager = LobbyManager.GetComponent<CustomNetworkLobbyManager> ();
	}

	public void OnCreateMatchButton(){
		customNetworkLobbyManager.CreateMatch ();
	}

	public void OnListMatchButton(int rank){
		customNetworkLobbyManager.GetListMatch (0);
	}

	public void OnJoinMatchButton(int ListId){
		customNetworkLobbyManager.ListMatchAndJoinMatch (0);
	}
}
