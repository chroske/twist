using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class CustomNetworkLobbyManager : NetworkLobbyManager
{
	private NetworkMatch networkMatch;
	private NetworkManager networkManager;

	void Start(){
		networkManager = GetComponent<NetworkManager>();
	}

	//クライアントがロビーのシーンからゲームプレイヤーシーンに切り替えが終了したことを伝えられたときサーバー上で呼び出し
	public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
	{
		//
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
	}

	public void OnStartMatchMakeButton(){
		networkManager.StartMatchMaker();
		networkManager.matchName = "TEEEEST";
		networkManager.matchSize = 4U;
		networkMatch = networkManager.matchMaker;
	}

	public void OnCreateMatchButton(){
		networkMatch.CreateMatch (networkManager.matchName, networkManager.matchSize, true, "", networkManager.OnMatchCreate);
	}

	public void OnListMatchButton(){
		networkMatch.ListMatches(0, 20, "", networkManager.OnMatchList);
	}

	public void OnJoinMatchButton(){
		var desc = networkManager.matches[0]; // join first room
		networkMatch.JoinMatch(desc.networkId, "", networkManager.OnMatchJoined);

	}
}
