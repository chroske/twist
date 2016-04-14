using UnityEngine;
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
	private GameObject ScrollViewContent;
	[SerializeField]
	private GameObject lobbyPlayerListNode;

	//protected LobbyHook _lobbyHooks;

	void Start(){
		networkManager = GetComponent<NetworkManager> ();
		//_lobbyHooks = GetComponent<UnityStandardAssets.Network.LobbyHook>();
	}


	//クライアントがロビーのシーンからゲームプレイヤーシーンに切り替えが終了したことを伝えられたときサーバー上で呼び出し
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
	{
		NetworkManagerController networkManagerController = gamePlayer.GetComponent<NetworkManagerController>();
		NetworkInstanceId playerNetID = lobbyPlayer.GetComponent<NetworkIdentity>().netId;
		int playerNetIdInt =  int.Parse(playerNetID.ToString());

		networkManagerController.syncPlayerNetID = playerNetIdInt;
		return true;
	}

	//マッチの一覧を ListMatches() から取得した場合に実行
	public override void OnMatchList(ListMatchResponse matchList)
	{
		matches = matchList.matches;

		//GUIで表示メソッド実行
		ScrollViewContent.GetComponent<GenMatchListController>().GenMatchList (matchList);
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

	public void CreateLobbyPlayerListPrefab(){
		GameObject prefab = Instantiate (lobbyPlayerListNode);
		prefab.transform.SetParent (lobbyContent.transform,false);


//		lobbyPlayer.transform.SetParent (lobbyContent.transform,false);
//		lobbyPlayer.transform.GetComponent<CustamNetworkLobbyPlayer> ().SetLobbyPlayerName("あーてすてす");
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


}
