using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class CustomNetworkLobbyManager : NetworkLobbyManager
{
	private NetworkMatch networkMatch;
	private NetworkManager networkManager;

	public GameObject matchPanel1;
	public GameObject matchPanel2;
	public GameObject matchPanel3;
	public GameObject matchPanel4;

	[SerializeField]
	private GameObject lobbyContent;
	[SerializeField]
	private GameObject roomListContent;
	[SerializeField]
	private GameObject lobbyPlayerListNode;

	private string lobbyPlayerName;

	[SerializeField]
	private GameObject lobbyScrollRect;

	[SerializeField]
	private GameObject roomListScrollRect;

	[SerializeField]
	private GameObject nodeGroup;


	void Start(){
		networkManager = GetComponent<NetworkManager> ();
		//_lobbyHooks = GetComponent<UnityStandardAssets.Network.LobbyHook>();
	}


	//クライアントがロビーのシーンからゲームプレイヤーシーンに切り替えが終了したことを伝えられたときサーバー上で呼び出し
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
	{
		//lobbyPlayerオブジェクトのnetidをnetworkManagerControllerに送って以後ゲーム中idとして使う
		NetworkManagerController networkManagerController = gamePlayer.GetComponent<NetworkManagerController>();
		NetworkInstanceId playerNetID = lobbyPlayer.GetComponent<NetworkIdentity>().netId;
		int playerNetIdInt =  int.Parse(playerNetID.ToString());
		networkManagerController.syncPlayerNetID = playerNetIdInt;

		//ユニットのパラメータをlobbyPlayerからgamePlayerに渡す
		LobbyPlayerController lobbyPlayerController = lobbyPlayer.GetComponent<LobbyPlayerController>();
//		UnitStatus unitStatus = new UnitStatus (
//			lobbyPlayerController.unitParamsClass[0].unit_id,
//			lobbyPlayerController.unitParamsClass[0].attack,
//			lobbyPlayerController.unitParamsClass[0].hitPoint,
//			lobbyPlayerController.unitParamsClass[0].speed,
//			lobbyPlayerController.unitParamsClass[0].type,
//			lobbyPlayerController.unitParamsClass[0].Level,
//			lobbyPlayerController.unitParamsClass[0].combo,
//			lobbyPlayerController.unitParamsClass[0].ability_1,
//			lobbyPlayerController.unitParamsClass[0].ability_2,
//			lobbyPlayerController.unitParamsClass[0].ability_3,
//			lobbyPlayerController.unitParamsClass[0].strikeShot,
//			lobbyPlayerController.unitParamsClass[0].comboType,
//			lobbyPlayerController.unitParamsClass[0].comboAttack,
//			lobbyPlayerController.unitParamsClass[0].maxComboNum
//		);

		//networkManagerControllerでsyncの構造体を定義すると何故か落ちるのでバラで渡す
		networkManagerController.syncUnitId = lobbyPlayerController.unitParamsClass [0].unit_id;
		networkManagerController.syncUnitAttack = lobbyPlayerController.unitParamsClass [0].attack;
		networkManagerController.syncUnitHitpoint = lobbyPlayerController.unitParamsClass [0].hitPoint;
		networkManagerController.syncUnitSpeed = lobbyPlayerController.unitParamsClass [0].speed;
		networkManagerController.syncUnitType = lobbyPlayerController.unitParamsClass [0].type;
		networkManagerController.syncUnitLevel = lobbyPlayerController.unitParamsClass [0].Level;
		networkManagerController.syncUnitCombo = lobbyPlayerController.unitParamsClass [0].combo;
		networkManagerController.syncUintAbbility_1 = lobbyPlayerController.unitParamsClass [0].ability_1;
		networkManagerController.syncUintAbbility_2 = lobbyPlayerController.unitParamsClass [0].ability_2;
		networkManagerController.syncUintAbbility_3 = lobbyPlayerController.unitParamsClass [0].ability_3;
		networkManagerController.syncUintStrikeShot = lobbyPlayerController.unitParamsClass [0].strikeShot;
		networkManagerController.syncUintComboType = lobbyPlayerController.unitParamsClass [0].comboType;
		networkManagerController.syncUintComboAttack = lobbyPlayerController.unitParamsClass [0].comboAttack;
		networkManagerController.syncUintMaxComboNum = lobbyPlayerController.unitParamsClass [0].maxComboNum;

		return true;
	}

	//マッチの一覧を ListMatches() から取得した場合に実行
	public override void OnMatchList(ListMatchResponse matchList)
	{
		matches = matchList.matches;

		//GUIで表示メソッド実行
		roomListContent.GetComponent<GenMatchListController>().GenMatchList (matchList);

		//flag = true;
		StartCoroutine (PullBackScrollView ());
	}

	public override void OnLobbyClientEnter(){
		Debug.Log("OnLobbyClientEnter");
	}

	public void StartTheGame(){
		ServerChangeScene (playScene);
	}

	//LobbyPlayerがprefabから作成された時に呼び出される(サーバ上)
//	public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
//	{
//		GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;
//
//		obj.transform.SetParent (lobbyContent.transform,false);
//		obj.transform.GetComponent<CustamNetworkLobbyPlayer> ().SetLobbyPlayerName("あーてすてす");
//
//		return obj;
//	}

	public GameObject CreateLobbyPlayerListPrefab(string lobbyPlayerName){
		GameObject prefab = Instantiate (lobbyPlayerListNode);
		prefab.transform.SetParent (lobbyContent.transform,false);
		prefab.transform.GetComponentInChildren<Text>().text = lobbyPlayerName;

		return prefab;
	}

	public void OnStartMatchMakeButton(){
		networkManager.StartMatchMaker();
		networkMatch = networkManager.matchMaker;

		matchPanel1.SetActive (false);
		matchPanel2.SetActive (true);
	}

	public void OnStopMatchMakeButton(){
		networkManager.StopMatchMaker();

		matchPanel1.SetActive (true);
		matchPanel2.SetActive (false);
	}

	public void OnCreateMatchButton(){
		string matchRoomName = "TESTTEST";
		int rank = 1;

		string roomName = "{" + rank.ToString() + "}" + matchRoomName;
		networkManager.matchName = roomName;
		networkManager.matchSize = 4U;
		networkMatch.CreateMatch (networkManager.matchName, networkManager.matchSize, true, "", networkManager.OnMatchCreate);

		matchPanel2.SetActive (false);
		matchPanel4.SetActive (true);
	}

	public void OnListMatchButton(int rank){
		networkMatch.ListMatches(0, 20, /*"{"+rank+"}"*/"", networkManager.OnMatchList);
		matchPanel2.SetActive (false);
		matchPanel3.SetActive (true);
	}

	public void OnJoinMatchButton(int ListId){
		var desc = networkManager.matches[ListId];
		networkMatch.JoinMatch(desc.networkId, "", networkManager.OnMatchJoined);

		matchPanel3.SetActive (false);
		matchPanel4.SetActive (true);
	}

	private bool flag = true;
	private int defaultRoomListScrollViewHeight = -135;

	public void OnChangedScrollPosition(Vector2 position){
		if (roomListContent.GetComponent<RectTransform> ().anchoredPosition.y < 0.0f && roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition.y > -270) {
			roomListScrollRect.GetComponent<ScrollRect> ().elasticity = 0.01f;

			roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, roomListContent.GetComponent<RectTransform> ().anchoredPosition.y + defaultRoomListScrollViewHeight);
			nodeGroup.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, -roomListContent.GetComponent<RectTransform> ().anchoredPosition.y * 0.9f);
		} else if(roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition.y < -270 && flag) {
			flag = false;
			moveFlag = true;
			roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, -270);
			roomListScrollRect.GetComponent<ScrollRect> ().elasticity = 0.1f;
			StartCoroutine (PullBackScrollView ());
			//networkMatch.ListMatches(0, 20, /*"{"+rank+"}"*/"", networkManager.OnMatchList);
		}
	}


	bool moveFlag = true;

	IEnumerator PullBackScrollView(){
		
		yield return new WaitForSeconds(2);
		//flag = true;
		Debug.Log ("PullBackScrollView"+moveFlag);
		//networkMatch.ListMatches(0, 20, /*"{"+rank+"}"*/"", networkManager.OnMatchList);
		while (moveFlag) {
			roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition = Vector2.Lerp (roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition, new Vector3 (0, defaultRoomListScrollViewHeight, 0), 0.1f);
			if(roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition.y >= defaultRoomListScrollViewHeight-1){
				moveFlag = false;
				flag = true;
			}
			yield return null;
		}
		yield break;
	}
}
