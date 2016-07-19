using UnityEngine;
using System.Collections;
using UnityEngine.Networking.Match;


public class BattlePanelOnlineDuelController : MonoBehaviour {

	[SerializeField]
	GameObject matchPanel1;
	[SerializeField]
	GameObject matchPanel2;
	[SerializeField]
	GameObject matchPanel3;
	[SerializeField]
	DuelCustomNetworkLobbyManager duelCustomNetworkLobbyManager;
	[SerializeField]
	GameObject HeaderAndTabbar;
	[SerializeField]
	RoomListController roomListController;
	[SerializeField]
	DuelPlayerListInRoomController duelPlayerListInRoomController;

	private GameObject currentPanel;

	void Start(){
		//ロビーTOPパネルをcurrentに
		currentPanel = matchPanel2;
	}

	public void StartTheGame(){
		currentPanel.SetActive (false);
		this.gameObject.SetActive (false);
		duelCustomNetworkLobbyManager.StartTheGame();
	}

	public void ChangeGameScene(){
		currentPanel.SetActive (false);
		this.gameObject.SetActive (false);
		HeaderAndTabbar.SetActive (false);
	}

	public void ExitRoom(){
		ChangePanel(1);
		duelCustomNetworkLobbyManager.ExitRoom();
	}

	//GameSceneからLobbyに戻った時のPanel設定
	public void SetUpLobby(){
		ChangePanel (1);
		this.gameObject.SetActive (true);
		HeaderAndTabbar.SetActive (true);
	}

	public void GenerateMatchList(ListMatchResponse matchList){
		roomListController.GenerateMatchList (matchList);
	}

	public void SetActiveStartGameButtonInRoom(){
		duelPlayerListInRoomController.SetActiveStartGameButton ();
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
