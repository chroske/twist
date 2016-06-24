using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameSceneManager : NetworkBehaviour {
	[SerializeField]
	GameObject offlinePlayerManagerObj;

	[SerializeField]
	PullArrow pullArrow;

	public GameObject Unit_1;
	public GameObject Unit_2;
	public GameObject Unit_3;
	public GameObject Unit_4;
	public GameObject turnEndText;


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
	private OfflinePlayerManager offlinePlayerManager;

	void Awake(){
		//offlineGameがtrueならofflinePlayerManagerを作る
		GameObject gameStateManagerObj = GameObject.Find("/GameStateManager");
		gameStateManager = gameStateManagerObj.GetComponent<GameStateManager> ();
		if (gameStateManager.offlineGame) {
			GameObject prefab = Instantiate (offlinePlayerManagerObj);
			offlinePlayerManager = prefab.GetComponent<OfflinePlayerManager> ();
			offlinePlayerManager.gameStateManager = gameStateManager;
			offlinePlayerManager.pullArrow = pullArrow;
			offlinePlayerManager.gameSceneManager = this;
		}
	}

	public IEnumerator DisplayTurnEndText(float displayTime){
		turnEndText.SetActive(true);
		yield return new WaitForSeconds(displayTime);
		turnEndText.SetActive(false);
		yield break;
	}

	public void TurnChange(int newTurnPlayerId){
		//全ユニットの移動を完全に止める(変更後即判定されないようにするため)
		Unit_1.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		Unit_2.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		Unit_3.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		Unit_4.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;

		turnPlayerId = newTurnPlayerId;

		if (myPlayerNetIdInt == newTurnPlayerId) {
			myTurnFlag = true;
		} else {
			myTurnFlag = false;
		}
	}


	public void ChangeControllUnit(int UnitControllPlayerId){
		GameObject controllUnit = new GameObject();

		if (UnitControllPlayerId == 0) {
			controllUnit = Unit_1;
			partyUnitList = new List<GameObject> (){Unit_2, Unit_3, Unit_4};
		} else if (UnitControllPlayerId == 1) {
			controllUnit = Unit_2;
			partyUnitList = new List<GameObject> (){Unit_1, Unit_3, Unit_4};
		} else if (UnitControllPlayerId == 2) {
			controllUnit = Unit_3;
			partyUnitList = new List<GameObject> (){Unit_1, Unit_2, Unit_4};
		} else if (UnitControllPlayerId == 3) {
			controllUnit = Unit_4;
			partyUnitList = new List<GameObject> (){Unit_1, Unit_2, Unit_3};
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
			
		ResetComboNumAllUnit();
	}

	//全ユニットのComboNumを初期化
	private void ResetComboNumAllUnit(){
		Unit_1.GetComponent<MyUnitController> ().ResetComboNum ();
		Unit_1.GetComponent<MyPartyUnitController> ().ResetComboNum ();

		Unit_2.GetComponent<MyUnitController> ().ResetComboNum ();
		Unit_2.GetComponent<MyPartyUnitController> ().ResetComboNum ();

		Unit_3.GetComponent<MyUnitController> ().ResetComboNum ();
		Unit_3.GetComponent<MyPartyUnitController> ().ResetComboNum ();

		Unit_4.GetComponent<MyUnitController> ().ResetComboNum ();
		Unit_4.GetComponent<MyPartyUnitController> ().ResetComboNum ();
	}

	public void SetAllUnitParamator(Dictionary<int,OwnedUnitData> unitParamDic){
		Unit_1.GetComponentInChildren<UnitParamManager> ().SetParameter (unitParamDic[1]);
		Unit_1.GetComponent<MyUnitController> ().SetComboEffect ();
		Unit_1.GetComponent<MyPartyUnitController> ().SetComboEffect ();

		Unit_2.GetComponentInChildren<UnitParamManager> ().SetParameter (unitParamDic[2]);
		Unit_2.GetComponent<MyUnitController> ().SetComboEffect ();
		Unit_2.GetComponent<MyPartyUnitController> ().SetComboEffect ();

		Unit_3.GetComponentInChildren<UnitParamManager> ().SetParameter (unitParamDic[3]);
		Unit_3.GetComponent<MyUnitController> ().SetComboEffect ();
		Unit_3.GetComponent<MyPartyUnitController> ().SetComboEffect ();

		Unit_4.GetComponentInChildren<UnitParamManager> ().SetParameter (unitParamDic[4]);
		Unit_4.GetComponent<MyUnitController> ().SetComboEffect ();
		Unit_4.GetComponent<MyPartyUnitController> ().SetComboEffect ();
	}

	public void StopAllunitVelocity(){
		Unit_1.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		Unit_2.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		Unit_3.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		Unit_4.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
	}

	//ユニットのパラメータとそれに対応するComboEffectをセット
	public void SetUnitParamatorByNetId(int netId, OwnedUnitData myUnitParam){
		GameObject controllUnit = new GameObject();
		if (netId == 0) {
			controllUnit = Unit_1;
		} else if (netId == 1) {
			controllUnit = Unit_2;
		} else if (netId == 2) {
			controllUnit = Unit_3;
		} else if (netId == 3) {
			controllUnit = Unit_4;
		}

		if(controllUnit != null){
			controllUnit.GetComponentInChildren<UnitParamManager> ().SetParameter (myUnitParam);
			controllUnit.GetComponent<MyUnitController> ().SetComboEffect ();
			controllUnit.GetComponent<MyPartyUnitController> ().SetComboEffect ();
		}
	}

	public void StopGame(){
		networkLobbyManager = GameObject.Find ("/MainCanvas").GetComponent<CustomNetworkLobbyManager> ();
		if (gameStateManager.offlineGame) {
			Destroy (offlinePlayerManager.gameObject);
			networkLobbyManager.SetUpOfflinePlayTop();
			gameStateManager.offlineGame = false;
//			Application.UnloadLevel ("GameMain");
			SceneManager.UnloadScene ("GameMain");
		} else if(gameStateManager.onlineGame) {
			gameStateManager.onlineGame = false;
			networkLobbyManager.SendReturnToLobby (); //なんかエラー出るけど問題ないっぽい
			if (networkLobbyManager.isHost) {
				networkLobbyManager.StopHost ();
			} else {
				networkLobbyManager.StopClient ();
			}
			networkLobbyManager.StopMatchMaker ();
			networkLobbyManager.SetUpLobby ();
		}
	}
}