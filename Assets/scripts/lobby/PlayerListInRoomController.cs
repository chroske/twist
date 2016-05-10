using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PlayerListInRoomController : NetworkBehaviour {

	private CustomNetworkLobbyManager customNetworkLobbyManager;

	[SerializeField]
	private GameObject LobbyManager;

	[SerializeField]
	private GameObject StartGameButton;

	[SerializeField]
	private GameObject ExitButton;

	// Use this for initialization
	void Start () {
		customNetworkLobbyManager = LobbyManager.GetComponent<CustomNetworkLobbyManager> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//ゲームスタートボタン
	public void OnClickStartGameButton(){
		customNetworkLobbyManager.StartTheGame();
	}

	//ルーム退出ボタン
	public void OnClickExitButton(){
		customNetworkLobbyManager.ExitRoom();
	}

	public void SetActiveStartGameButton(){
		StartGameButton.SetActive (true);
	}
}
