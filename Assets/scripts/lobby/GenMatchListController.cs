using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Text.RegularExpressions;

public class GenMatchListController: MonoBehaviour {

	[SerializeField]
	RectTransform scrollViewRoom;
	[SerializeField]
	GameObject lobbyManager;
	[SerializeField]
	BattlePanelOnlineController battlePanelOnlineController;

	public void GenMatchList(ListMatchResponse response){
		//リストをクリア
		RemoveAllListViewItem ();

		for (int i = 0; i < response.matches.Count; ++i)
		{
			//部屋がいっぱいじゃなければ表示する
			if(response.matches[i].currentSize <= response.matches[i].maxSize){
				Debug.Log (response.matches[i].name);

				var node = GameObject.Instantiate(scrollViewRoom) as RectTransform;
				node.SetParent(transform, false);
				node.GetComponent<MatchListRoomController>().SetNodeDatas(i, response.matches [i].name, response.matches[i].maxSize, response.matches[i].currentSize);
			}
		}
	}

	//リストクリア
	public void RemoveAllListViewItem() {
		foreach (Transform child in transform) {
			if(child.GetSiblingIndex() != 0){
				GameObject.Destroy(child.gameObject);
			}
		}
	}

	public void OnJoinMatchButton(int ListId){
		lobbyManager.GetComponent<CustomNetworkLobbyManager>().JoinListMatch(ListId);
		battlePanelOnlineController.ChangePanel(3);
	}
}
