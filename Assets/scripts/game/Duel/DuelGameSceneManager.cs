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
	CommandPanelController CommandAreaUnit_1;
	[SerializeField]
	CommandPanelController CommandAreaUnit_2;
	[SerializeField]
	CommandPanelController CommandAreaUnit_3;
	[SerializeField]
	CommandPanelController CommandAreaUnit_4;

	[SerializeField]
	GameObject turnEndText;

	[SerializeField]
	public GameObject[] myUnit = new GameObject[4];
	[SerializeField]
	public GameObject[] enemyUnit = new GameObject[4];

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
	public int guestShotUnitNum;
	public int hostShotUnitNum;

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

	public void SetArrowParameter(){
		if (myPlayerNetIdInt == 0) {
			pullArrow.isHost = true;
		} else {
			pullArrow.isHost = false;
		}
	}

	public void ChangeControllUnit(int unitNum){
		controllUnitNum = unitNum;
		if (myPlayerNetIdInt == 0) {
			pullArrow.myUnit = myUnit [controllUnitNum];
		} else if(myPlayerNetIdInt == 1) {
			pullArrow.myUnit = enemyUnit [controllUnitNum];
		}
	}

	public void ResetUnit(){
		//partyUnit設定
		foreach (GameObject myUnitObj in myUnit) {
			//タグを切り替え
			myUnitObj.tag = "partyUnit";
			foreach (Transform child in myUnitObj.transform){
				child.tag = "partyUnit";
			}
			myUnitObj.transform.GetComponent<MyPartyUnitController>().enabled = true;
			myUnitObj.transform.GetComponent<MyUnitController>().enabled = false;
			myUnitObj.transform.GetComponent<EnemyUnitController>().enabled = false;
			myUnitObj.transform.GetComponent<EnemyPartyUnitController>().enabled = false;
		}
		//enemyPartyUnit設定
		foreach (GameObject enemyUnitObj in enemyUnit){
			//タグを切り替え
			enemyUnitObj.tag = "enemyPartyUnit";
			foreach (Transform child in enemyUnitObj.transform){
				child.tag = "enemyPartyUnit";
			}
			enemyUnitObj.transform.GetComponent<MyPartyUnitController>().enabled = false;
			enemyUnitObj.transform.GetComponent<MyUnitController>().enabled = false;
			enemyUnitObj.transform.GetComponent<EnemyUnitController>().enabled = false;
			enemyUnitObj.transform.GetComponent<EnemyPartyUnitController>().enabled = true;
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
			EnemyUnitController enemyUnitController_1 = enemyUnit[0].GetComponent<EnemyUnitController> ();
			enemyUnitController_1.SetUnitIcon ();
			enemyUnitController_1.SetComboEffect ();
			enemyUnit[0].GetComponent<EnemyPartyUnitController> ().SetComboEffect ();
			EnemyUnitController enemyUnitController_2 = enemyUnit[1].GetComponent<EnemyUnitController> ();
			enemyUnitController_2.SetUnitIcon ();
			enemyUnitController_2.SetComboEffect ();
			enemyUnit[1].GetComponent<EnemyPartyUnitController> ().SetComboEffect ();
			EnemyUnitController enemyUnitController_3 = enemyUnit[2].GetComponent<EnemyUnitController> ();
			enemyUnitController_3.SetUnitIcon ();
			enemyUnitController_3.SetComboEffect ();
			enemyUnit[2].GetComponent<EnemyPartyUnitController> ().SetComboEffect ();
			EnemyUnitController enemyUnitController_4 = enemyUnit[3].GetComponent<EnemyUnitController> ();
			enemyUnitController_4.SetUnitIcon ();
			enemyUnitController_4.SetComboEffect ();
			enemyUnit[3].GetComponent<EnemyPartyUnitController> ().SetComboEffect ();
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

	public void RemoteShot(Vector2 hostShotVector, Vector2 guestShotVector, int hostShotNum,  int guestShotNum){
		int myControllUnitNum = 0;
		int enemyControllUnitNum = 0;
		if (myPlayerNetIdInt == 0) {
			myControllUnitNum = hostShotNum;
			enemyControllUnitNum = guestShotNum;
		} else if(myPlayerNetIdInt == 1) {
			myControllUnitNum = hostShotNum;
			enemyControllUnitNum = guestShotNum;
		}

		//タグを切り替え
		GameObject myShotUnit = myUnit [myControllUnitNum];
		myShotUnit.tag = "controllUnit";
		foreach (Transform child in myShotUnit.transform){
			child.tag = "controllUnit";
		}
		myShotUnit.transform.GetComponent<MyUnitController>().enabled = true;
		myShotUnit.transform.GetComponent<MyPartyUnitController>().enabled = false;
		Rigidbody2D myUnitRigidbody2D = myUnit [myControllUnitNum].GetComponent<Rigidbody2D> ();
		myUnitRigidbody2D.AddForce(hostShotVector);

		//タグを切り替え
		GameObject enemyShotUnit = enemyUnit [enemyControllUnitNum];
		enemyShotUnit.tag = "enemyUnit";
		foreach (Transform child in enemyShotUnit.transform){
			child.tag = "enemyUnit";
		}
		enemyShotUnit.transform.GetComponent<EnemyUnitController>().enabled = true;
		enemyShotUnit.transform.GetComponent<EnemyPartyUnitController>().enabled = false;
		Rigidbody2D enemyUnitRigidbody2D = enemyUnit [enemyControllUnitNum].GetComponent<Rigidbody2D> ();
		enemyUnitRigidbody2D.AddForce(guestShotVector);
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