using UnityEngine;
using System.Collections;
using UnityEngine.Networking.Match;

public class BattlePanelOnlineController : MonoBehaviour {

	[SerializeField]
	GameObject matchPanel1;
	[SerializeField]
	GameObject matchPanel2;
	[SerializeField]
	GameObject matchPanel3;
	[SerializeField]
	CustomNetworkLobbyManager customNetworkLobbyManager;
	[SerializeField]
	GameObject HeaderAndTabbar;
	[SerializeField]
	RoomListController roomListController;
	[SerializeField]
	PlayerListInRoomController playerListInRoomController;

	private GameObject currentPanel;

	void Start(){
		//ロビーTOPパネルをcurrentに
		currentPanel = matchPanel2;
	}

	public void StartTheGame(){
		currentPanel.SetActive (false);
		this.gameObject.SetActive (false);
		customNetworkLobbyManager.StartTheGame();
	}

	public void ExitRoom(){
		ChangePanel(1);
		customNetworkLobbyManager.ExitRoom();
	}

	public void StopMatchMake(){
		ChangePanel(1);
		customNetworkLobbyManager.StopMatchMake ();
	}

	//GameSceneからLobbyに戻った時のPanel設定
	public void SetUpLobby(){
		ChangePanel (1);
		this.gameObject.SetActive (true);
		HeaderAndTabbar.SetActive (true);
	}

	public void CreateMatch(){
		ChangePanel(3);
		customNetworkLobbyManager.CreateMatch ();
	}

	public void GenerateMatchList(ListMatchResponse matchList){
		roomListController.GenerateMatchList (matchList);
	}

	public void SetActiveStartGameButtonInRoom(){
		playerListInRoomController.SetActiveStartGameButton ();
	}

	public void ChangePanel(int panelNum){
		if(currentPanel != null){
			currentPanel.SetActive (false);
		}

		switch (panelNum)
		{
		case 1:
			matchPanel1.SetActive (true);
			currentPanel = matchPanel1;
			break;
		case 2:
			matchPanel2.SetActive (true);
			currentPanel = matchPanel2;
			break;
		case 3:
			matchPanel3.SetActive (true);
			currentPanel = matchPanel3;
			break;
		}
	}
}
