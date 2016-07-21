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
	DuelPullArrow pullArrow;
	[SerializeField]
	CommandPanelManager CommandAreaUnit_1;
	[SerializeField]
	CommandPanelManager CommandAreaUnit_2;
	[SerializeField]
	CommandPanelManager CommandAreaUnit_3;
	[SerializeField]
	CommandPanelManager CommandAreaUnit_4;

	[SerializeField]
	GameObject turnEndText;

	[SerializeField]
	GameObject[] myUnit = new GameObject[4];
	[SerializeField]
	GameObject[] enemyUnit = new GameObject[4];

	public bool myTurnFlag;
	public int myPlayerNetIdInt;
	public int turnPlayerId;
	public int beforeTurnPlayerId; //比較用
	public int firstTurnPlayerId;
	public bool turnEnd;
	public bool offlineGame;
	public int controllUnitNum;

	public bool guestPlayerShotFlag;
	public bool hostPlayerShotFlag;
	public Vector2 guestShotVector = new Vector2();
	public Vector2 hostShotVector = new Vector2();

	private GameObject NetworkManager;
	private List<GameObject> partyUnitList;
	private List<GameObject> enemyUnitList;
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
		myUnit[0].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		myUnit[1].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		myUnit[2].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		myUnit[3].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit[0].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit[1].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit[2].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit[3].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;

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

	//ゲーム画面反転
	public void ReversalGameField(){
		myUnit[0].transform.eulerAngles = new Vector3 (0, 0, 180);
		myUnit[1].transform.eulerAngles = new Vector3 (0, 0, 180);
		myUnit[2].transform.eulerAngles = new Vector3 (0, 0, 180);
		myUnit[3].transform.eulerAngles = new Vector3 (0, 0, 180);
		enemyUnit[0].transform.eulerAngles = new Vector3 (0, 0, 180);
		enemyUnit[1].transform.eulerAngles = new Vector3 (0, 0, 180);
		enemyUnit[2].transform.eulerAngles = new Vector3 (0, 0, 180);
		enemyUnit[3].transform.eulerAngles = new Vector3 (0, 0, 180);

		mainCamera.transform.eulerAngles = new Vector3 (0, 0, 180);
	}
		
	public void SetControllUnit(){
		GameObject controllUnit = new GameObject();
		if(myPlayerNetIdInt == 0){
			controllUnit = myUnit[0];
			partyUnitList = new List<GameObject> (){myUnit[1], myUnit[2], myUnit[3]};
			enemyUnitList = new List<GameObject> (){enemyUnit[0], enemyUnit[1], enemyUnit[2], enemyUnit[3]};
			controllUnitNum = 0;
		} else if(myPlayerNetIdInt == 1){
			controllUnit = enemyUnit[0];
			partyUnitList = new List<GameObject> (){enemyUnit[1], enemyUnit[2], enemyUnit[3]};
			enemyUnitList = new List<GameObject> (){myUnit[0], myUnit[1], myUnit[2], myUnit[3]};
			controllUnitNum = 0;
			//ゲーム画面反転
			ReversalGameField ();
		}

		if (controllUnit != null) {
			//controllUnit設定
			controllUnit.tag = "controllUnit";
			foreach (Transform child in controllUnit.transform){
				child.tag = "controllUnit";
			}
			controllUnit.transform.GetComponent<MyPartyUnitController>().enabled = false;
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

			partyUnit.transform.GetComponent<MyPartyUnitController>().enabled = true;
			partyUnit.transform.GetComponent<MyUnitController>().enabled = false;
		}
		//enemyUnit設定
		foreach (GameObject enemyUnit in enemyUnitList)
		{
			//タグを切り替え
			enemyUnit.tag = "enemyUnit";
			foreach (Transform child in enemyUnit.transform){
				child.tag = "enemyUnit";
			}

			enemyUnit.transform.GetComponent<MyPartyUnitController>().enabled = false;
			enemyUnit.transform.GetComponent<MyUnitController>().enabled = false;
		}
	}

	//全ユニットのComboNumを初期化
	public void ResetComboNumAllUnit(){
		myUnit[0].GetComponent<MyUnitController> ().ResetComboNum ();
		myUnit[0].GetComponent<MyPartyUnitController> ().ResetComboNum ();

		myUnit[1].GetComponent<MyUnitController> ().ResetComboNum ();
		myUnit[1].GetComponent<MyPartyUnitController> ().ResetComboNum ();

		myUnit[2].GetComponent<MyUnitController> ().ResetComboNum ();
		myUnit[2].GetComponent<MyPartyUnitController> ().ResetComboNum ();

		myUnit[3].GetComponent<MyUnitController> ().ResetComboNum ();
		myUnit[3].GetComponent<MyPartyUnitController> ().ResetComboNum ();

		enemyUnit[0].GetComponent<MyUnitController> ().ResetComboNum ();
		enemyUnit[0].GetComponent<MyPartyUnitController> ().ResetComboNum ();

		enemyUnit[1].GetComponent<MyUnitController> ().ResetComboNum ();
		enemyUnit[1].GetComponent<MyPartyUnitController> ().ResetComboNum ();

		enemyUnit[2].GetComponent<MyUnitController> ().ResetComboNum ();
		enemyUnit[2].GetComponent<MyPartyUnitController> ().ResetComboNum ();

		enemyUnit[3].GetComponent<MyUnitController> ().ResetComboNum ();
		enemyUnit[3].GetComponent<MyPartyUnitController> ().ResetComboNum ();
	}

	public void StopAllunitVelocity(){
		myUnit[0].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		myUnit[1].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		myUnit[2].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		myUnit[3].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;

		enemyUnit[0].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit[1].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit[2].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		enemyUnit[3].GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
	}

	//ユニットのパラメータとそれに対応するComboEffectをセット
	public void SetUnitParamatorByNetId(int netId, List<OwnedUnitData> myUnitParam){
		if (netId == 0) {
			myUnit[0].GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[0]);
			myUnit[1].GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[1]);
			myUnit[2].GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[2]);
			myUnit[3].GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[3]);
			MyUnitController myUnitController_1 = myUnit[0].GetComponent<MyUnitController> ();
			myUnitController_1.SetUnitIcon ();
			myUnitController_1.SetComboEffect ();
			myUnit[0].GetComponent<MyPartyUnitController> ().SetComboEffect ();
			MyUnitController myUnitController_2 = myUnit[1].GetComponent<MyUnitController> ();
			myUnitController_2.SetUnitIcon ();
			myUnitController_2.SetComboEffect ();
			myUnit[1].GetComponent<MyPartyUnitController> ().SetComboEffect ();
			MyUnitController myUnitController_3 = myUnit[2].GetComponent<MyUnitController> ();
			myUnitController_3.SetUnitIcon ();
			myUnitController_3.SetComboEffect ();
			myUnit[2].GetComponent<MyPartyUnitController> ().SetComboEffect ();
			MyUnitController myUnitController_4 = myUnit[3].GetComponent<MyUnitController> ();
			myUnitController_4.SetUnitIcon ();
			myUnitController_4.SetComboEffect ();
			myUnit[3].GetComponent<MyPartyUnitController> ().SetComboEffect ();
		} else if(netId == 1){
			enemyUnit[0].GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[0]);
			enemyUnit[1].GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[1]);
			enemyUnit[2].GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[2]);
			enemyUnit[3].GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam[3]);
			MyUnitController enemyUnitController_1 = enemyUnit[0].GetComponent<MyUnitController> ();
			enemyUnitController_1.SetUnitIcon ();
			enemyUnitController_1.SetComboEffect ();
			enemyUnit[0].GetComponent<MyPartyUnitController> ().SetComboEffect ();
			MyUnitController enemyUnitController_2 = enemyUnit[1].GetComponent<MyUnitController> ();
			enemyUnitController_2.SetUnitIcon ();
			enemyUnitController_2.SetComboEffect ();
			enemyUnit[1].GetComponent<MyPartyUnitController> ().SetComboEffect ();
			MyUnitController enemyUnitController_3 = enemyUnit[2].GetComponent<MyUnitController> ();
			enemyUnitController_3.SetUnitIcon ();
			enemyUnitController_3.SetComboEffect ();
			enemyUnit[2].GetComponent<MyPartyUnitController> ().SetComboEffect ();
			MyUnitController enemyUnitController_4 = enemyUnit[3].GetComponent<MyUnitController> ();
			enemyUnitController_4.SetUnitIcon ();
			enemyUnitController_4.SetComboEffect ();
			enemyUnit[3].GetComponent<MyPartyUnitController> ().SetComboEffect ();
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

	public void RemoteShot(Vector2 hostShotVector, Vector2 guestShotVector){
		//タグを切り替え
		GameObject myShotUnit = myUnit [controllUnitNum];
		myShotUnit.tag = "controllUnit";
		foreach (Transform child in myShotUnit.transform){
			child.tag = "controllUnit";
		}
		Rigidbody2D myUnitRigidbody2D = myUnit [controllUnitNum].GetComponent<Rigidbody2D> ();
		myUnitRigidbody2D.AddForce(hostShotVector);

		//タグを切り替え
		GameObject enemyShotUnit = enemyUnit [controllUnitNum];
		enemyShotUnit.tag = "controllUnit";
		foreach (Transform child in enemyShotUnit.transform){
			child.tag = "controllUnit";
		}
		Rigidbody2D enemyUnitRigidbody2D = enemyUnit [controllUnitNum].GetComponent<Rigidbody2D> ();
		enemyUnitRigidbody2D.AddForce(guestShotVector);





//		GameObject shotUnit = new GameObject ();
//		if(isLocalPlayer){
//			if(myPlayerNetIdInt == 0){
//				shotUnit = myUnit [controllUnitNum];
//			} else if(myPlayerNetIdInt == 1){
//				shotUnit = enemyUnit [controllUnitNum];
//			}
//		} else {
//			if(myPlayerNetIdInt == 0){
//				shotUnit = enemyUnit [controllUnitNum];
//			} else if(myPlayerNetIdInt == 1){
//				shotUnit = myUnit [controllUnitNum];
//			}
//		}
//
//		//タグを切り替え
//		shotUnit.tag = "controllUnit";
//		foreach (Transform child in shotUnit.transform){
//			child.tag = "controllUnit";
//		}
//		shotUnit.transform.GetComponent<MyUnitController>().enabled = true;
//		Rigidbody2D myUnitRigidbody2D = shotUnit.GetComponent<Rigidbody2D> ();
//		myUnitRigidbody2D.AddForce(shotVector);
	}

	public Vector3[] GetAllMyUnitPosition(){
		Vector3[] myUnitPositionArray = new Vector3[myUnit.Length];
		int i = 0;
		foreach(GameObject myUnitData in myUnit){
			myUnitPositionArray [i] = myUnitData.transform.position;
			i++;
		}
		return myUnitPositionArray;
	}

	public Vector3[] GetAllEnemyUnitPosition(){
		Vector3[] enemyUnitPositionArray = new Vector3[enemyUnit.Length];
		int i = 0;
		foreach(GameObject enemyUnitData in enemyUnit){
			enemyUnitPositionArray [i] = enemyUnitData.transform.position;
			i++;
		}
		return enemyUnitPositionArray;
	}

	public void SyncAllUnitPosition(Vector3[] myUnitPositionList, Vector3[] enemyUnitPositionList){
		int i = 0;
		foreach(Vector3 myUnitPosition in myUnitPositionList){
			myUnit [i].transform.position = myUnitPosition;
			i++;
		}
		int j = 0;
		foreach(Vector3 enemyUnitPosition in enemyUnitPositionList){
			enemyUnit [j].transform.position = enemyUnitPosition;
			j++;
		}
	}
}