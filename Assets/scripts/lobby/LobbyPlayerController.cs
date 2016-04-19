using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class LobbyPlayerController : NetworkBehaviour {

	[SyncVar] public string syncLobbyPlayerName;
	private bool setLobbyPlayerNodeFlag;

	// Use this for initialization
	void awake (){
	}

	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//syncLobbyPlayerNameに値が入ったらNodeをprefabから作る
		if(syncLobbyPlayerName != ""){
			CreateLobbyPlayerListPrefab (syncLobbyPlayerName);
		}
	}

	//LobbyPlayerListPrefabの生成をlobbyManagerに依頼(１度だけ)
	void CreateLobbyPlayerListPrefab(string lobbyPlayerName){
		if(!setLobbyPlayerNodeFlag){
			setLobbyPlayerNodeFlag = true;

			GameObject lobbyManager = GameObject.Find("LobbyManager");
			lobbyManager.transform.GetComponent<CustomNetworkLobbyManager> ().CreateLobbyPlayerListPrefab(lobbyPlayerName);
		}
	}

	public void ProvideLobbyPlayerNameToServer (){
		GameObject acountDataManager = GameObject.Find("AccountDataManager");
		syncLobbyPlayerName = acountDataManager.GetComponent<AccountDataManager> ().AccountName;
		CmdProvideLobbyPlayerNameToServer(syncLobbyPlayerName);
	}

	[Command]
	void CmdProvideLobbyPlayerNameToServer (string lobbyPlayerName){
		syncLobbyPlayerName = lobbyPlayerName;
	}
}
