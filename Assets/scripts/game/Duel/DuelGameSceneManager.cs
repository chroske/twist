using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class DuelGameSceneManager : NetworkBehaviour {
	[SerializeField]
	GameObject mainCamera;
	[SerializeField]
	GameObject offlinePlayerManagerObj;
	[SerializeField]
	PullArrow pullArrow;
	[SerializeField]
	CommandPanelManager CommandAreaUnit_1;
	[SerializeField]
	CommandPanelManager CommandAreaUnit_2;
	[SerializeField]
	CommandPanelManager CommandAreaUnit_3;
	[SerializeField]
	CommandPanelManager CommandAreaUnit_4;

	[SerializeField]
	GameObject myUnit_1;
	[SerializeField]
	GameObject myUnit_2;
	[SerializeField]
	GameObject myUnit_3;
	[SerializeField]
	GameObject myUnit_4;

	[SerializeField]
	GameObject enemyUnit_1;
	[SerializeField]
	GameObject enemyUnit_2;
	[SerializeField]
	GameObject enemyUnit_3;
	[SerializeField]
	GameObject enemyUnit_4;

	[SerializeField]
	GameObject turnEndText;

	public bool myTurnFlag;
	public int myPlayerNetIdInt;
	public int turnPlayerId;
	public int beforeTurnPlayerId; //比較用
	public int firstTurnPlayerId;
	public bool turnEnd;
	public bool offlineGame;

	private GameObject NetworkManager;
	private List<GameObject> partyUnitList;
	private GameStateManager gameStateManager;
	private CustomNetworkLobbyManager networkLobbyManager;
	private DuelCustomNetworkLobbyManager duelNetworkLobbyManager;
	private OfflinePlayerManager offlinePlayerManager;

	void Awake(){
		Application.targetFrameRate = 60;

		//offlineGameがtrueならofflinePlayerManagerを作る
		GameObject gameStateManagerObj = GameObject.Find("/GameStateManager");
		gameStateManager = gameStateManagerObj.GetComponent<GameStateManager> ();
	}

	public IEnumerator DisplayTurnEndText(float displayTime){
		turnEndText.SetActive(true);
		yield return new WaitForSeconds(displayTime);
		turnEndText.SetActive(false);
		yield break;
	}

	public void TurnChange(int newTurnPlayerId){
		//全ユニットの移動を完全に止める(変更後即判定されないようにするため)
		myUnit_1.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		myUnit_2.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		myUnit_3.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		myUnit_4.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit_1.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit_2.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit_3.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit_4.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;

		myTurnFlag = true;
		pullArrow.myTurnFlag = true;

//		turnPlayerId = newTurnPlayerId;
//
//		if (myPlayerNetIdInt == newTurnPlayerId) {
//			myTurnFlag = true;
//			pullArrow.myTurnFlag = true;
//		} else {
//			myTurnFlag = false;
//			pullArrow.myTurnFlag = false;
//		}
	}

	public void ReversalGameField(){
		myUnit_1.transform.eulerAngles = new Vector3 (0, 0, 180);
		myUnit_2.transform.eulerAngles = new Vector3 (0, 0, 180);
		myUnit_3.transform.eulerAngles = new Vector3 (0, 0, 180);
		myUnit_4.transform.eulerAngles = new Vector3 (0, 0, 180);
		enemyUnit_1.transform.eulerAngles = new Vector3 (0, 0, 180);
		enemyUnit_2.transform.eulerAngles = new Vector3 (0, 0, 180);
		enemyUnit_3.transform.eulerAngles = new Vector3 (0, 0, 180);
		enemyUnit_4.transform.eulerAngles = new Vector3 (0, 0, 180);

		mainCamera.transform.eulerAngles = new Vector3 (0, 0, 180);
	}
		
	public void SetControllUnit(){
		GameObject controllUnit = new GameObject();
		if(myPlayerNetIdInt == 0){
			controllUnit = myUnit_1;
			partyUnitList = new List<GameObject> (){myUnit_2, myUnit_3, myUnit_4};
		} else if(myPlayerNetIdInt == 1){
			controllUnit = enemyUnit_1;
			partyUnitList = new List<GameObject> (){enemyUnit_2, enemyUnit_3, enemyUnit_4};

			ReversalGameField ();
		}

		if (controllUnit != null) {
			//controllUnit設定
			controllUnit.tag = "controllUnit";
			foreach (Transform child in controllUnit.transform){
				child.tag = "controllUnit";
			}
			controllUnit.transform.GetComponent<MyPartyUnitController>().enabled =false;
			controllUnit.transform.GetComponent<MyUnitController>().enabled = true;

			pullArrow.myUnit = controllUnit;
		}

		//partyUnit設定
		foreach (GameObject partyUnit in partyUnitList)
		{
			//タグを切り替え
			partyUnit.tag = "partyUnit";
			foreach (Transform child in partyUnit.transform){
				child.tag = "partyUnit";
			}

			partyUnit.transform.GetComponent<MyPartyUnitController>().enabled =true;
			partyUnit.transform.GetComponent<MyUnitController>().enabled = false;
		}
	}

	//全ユニットのComboNumを初期化
	public void ResetComboNumAllUnit(){
		myUnit_1.GetComponent<MyUnitController> ().ResetComboNum ();
		myUnit_1.GetComponent<MyPartyUnitController> ().ResetComboNum ();

		myUnit_2.GetComponent<MyUnitController> ().ResetComboNum ();
		myUnit_2.GetComponent<MyPartyUnitController> ().ResetComboNum ();

		myUnit_3.GetComponent<MyUnitController> ().ResetComboNum ();
		myUnit_3.GetComponent<MyPartyUnitController> ().ResetComboNum ();

		myUnit_4.GetComponent<MyUnitController> ().ResetComboNum ();
		myUnit_4.GetComponent<MyPartyUnitController> ().ResetComboNum ();

		enemyUnit_1.GetComponent<MyUnitController> ().ResetComboNum ();
		enemyUnit_1.GetComponent<MyPartyUnitController> ().ResetComboNum ();

		enemyUnit_2.GetComponent<MyUnitController> ().ResetComboNum ();
		enemyUnit_2.GetComponent<MyPartyUnitController> ().ResetComboNum ();

		enemyUnit_3.GetComponent<MyUnitController> ().ResetComboNum ();
		enemyUnit_3.GetComponent<MyPartyUnitController> ().ResetComboNum ();

		enemyUnit_4.GetComponent<MyUnitController> ().ResetComboNum ();
		enemyUnit_4.GetComponent<MyPartyUnitController> ().ResetComboNum ();
	}

	public void StopAllunitVelocity(){
		myUnit_1.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		myUnit_2.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		myUnit_3.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		myUnit_4.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;

		enemyUnit_1.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit_2.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit_3.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit_4.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
	}

	//ユニットのパラメータとそれに対応するComboEffectをセット
	public void SetUnitParamatorByNetId(int netId, List<OwnedUnitData> myUnitParam){
		if (netId == 0) {
			myUnit_1.GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[0]);
			myUnit_2.GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[1]);
			myUnit_3.GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[2]);
			myUnit_4.GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[3]);
			MyUnitController myUnitController_1 = myUnit_1.GetComponent<MyUnitController> ();
			myUnitController_1.SetUnitIcon ();
			myUnitController_1.SetComboEffect ();
			myUnit_1.GetComponent<MyPartyUnitController> ().SetComboEffect ();
			MyUnitController myUnitController_2 = myUnit_2.GetComponent<MyUnitController> ();
			myUnitController_2.SetUnitIcon ();
			myUnitController_2.SetComboEffect ();
			myUnit_2.GetComponent<MyPartyUnitController> ().SetComboEffect ();
			MyUnitController myUnitController_3 = myUnit_3.GetComponent<MyUnitController> ();
			myUnitController_3.SetUnitIcon ();
			myUnitController_3.SetComboEffect ();
			myUnit_3.GetComponent<MyPartyUnitController> ().SetComboEffect ();
			MyUnitController myUnitController_4 = myUnit_4.GetComponent<MyUnitController> ();
			myUnitController_4.SetUnitIcon ();
			myUnitController_4.SetComboEffect ();
			myUnit_4.GetComponent<MyPartyUnitController> ().SetComboEffect ();
		} else if(netId == 1){
			enemyUnit_1.GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[0]);
			enemyUnit_2.GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[1]);
			enemyUnit_3.GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[2]);
			enemyUnit_4.GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[3]);
			MyUnitController enemyUnitController_1 = enemyUnit_1.GetComponent<MyUnitController> ();
			enemyUnitController_1.SetUnitIcon ();
			enemyUnitController_1.SetComboEffect ();
			enemyUnit_1.GetComponent<MyPartyUnitController> ().SetComboEffect ();
			MyUnitController enemyUnitController_2 = enemyUnit_2.GetComponent<MyUnitController> ();
			enemyUnitController_2.SetUnitIcon ();
			enemyUnitController_2.SetComboEffect ();
			enemyUnit_2.GetComponent<MyPartyUnitController> ().SetComboEffect ();
			MyUnitController enemyUnitController_3 = enemyUnit_3.GetComponent<MyUnitController> ();
			enemyUnitController_3.SetUnitIcon ();
			enemyUnitController_3.SetComboEffect ();
			enemyUnit_3.GetComponent<MyPartyUnitController> ().SetComboEffect ();
			MyUnitController enemyUnitController_4 = enemyUnit_4.GetComponent<MyUnitController> ();
			enemyUnitController_4.SetUnitIcon ();
			enemyUnitController_4.SetComboEffect ();
			enemyUnit_4.GetComponent<MyPartyUnitController> ().SetComboEffect ();
		}

		if(netId == myPlayerNetIdInt){
			CommandAreaUnit_1.SetCommandPanelParam (myUnitParam[0]);
			CommandAreaUnit_2.SetCommandPanelParam (myUnitParam[1]);
			CommandAreaUnit_3.SetCommandPanelParam (myUnitParam[2]);
			CommandAreaUnit_4.SetCommandPanelParam (myUnitParam[3]);
		}
	}

	public void StopGame(){
		if (gameStateManager.offlineGame) {
			Destroy (offlinePlayerManager.gameObject);
			networkLobbyManager.SetUpOfflinePlayTop();
			gameStateManager.offlineGame = false;
			SceneManager.UnloadScene ("GameMain");
		} else if(gameStateManager.onlineGame) {
			if(gameStateManager.onlineGameMode == "online"){
				networkLobbyManager = GameObject.Find ("/MainCanvas/BattlePanel/BattlePanelOnline").GetComponent<CustomNetworkLobbyManager> ();

				gameStateManager.onlineGame = false;
				networkLobbyManager.SendReturnToLobby (); //なんかエラー出るけど問題ないっぽい
				if (networkLobbyManager.isHost) {
					networkLobbyManager.StopHost ();
				} else {
					networkLobbyManager.StopClient ();
				}
				networkLobbyManager.StopMatchMaker ();
			} else if(gameStateManager.onlineGameMode == "duel"){
				duelNetworkLobbyManager = GameObject.Find ("/MainCanvas/BattlePanel/BattlePanelOnlineDuel").GetComponent<DuelCustomNetworkLobbyManager> ();

				gameStateManager.onlineGame = false;
				duelNetworkLobbyManager.SendReturnToLobby (); //なんかエラー出るけど問題ないっぽい
				if (duelNetworkLobbyManager.isHost) {
					duelNetworkLobbyManager.StopHost ();
				} else {
					duelNetworkLobbyManager.StopClient ();
				}
				duelNetworkLobbyManager.StopMatchMaker ();
			}
		}
	}
}