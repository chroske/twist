using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class RoomListController : MonoBehaviour {
	
	[SerializeField]
	private GameObject roomListContent;

	[SerializeField]
	private GameObject roomListScrollRect;

	[SerializeField]
	private GameObject nodeGroup;

	[SerializeField]
	private GameObject loadIcon;

	[SerializeField]
	private GameObject loadArrowIcon;

	[SerializeField]
	private GameObject LobbyManager;

	private NetworkMatch networkMatch;
	private NetworkManager networkManager;

	private bool contentScrollStop = true;
	private int defaultRoomListScrollViewHeight = -90;
	private bool pullBackScrollViewFlag = true;

	// Use this for initialization
	void Start () {
		networkManager = LobbyManager.GetComponent<NetworkManager> ();
		networkMatch = networkManager.matchMaker;
	}
	
	// Update is called once per frame
	void Update () {
		if(loadIcon.activeSelf){
			loadIcon.transform.eulerAngles += new Vector3 (0f, 0f, -1f);
		}
	}

	public void OnChangedScrollPosition(Vector2 position){
		if (roomListContent.GetComponent<RectTransform> ().anchoredPosition.y < 0.0f && roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition.y > -240) {
			roomListScrollRect.GetComponent<ScrollRect> ().elasticity = 0.01f;

			roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, roomListContent.GetComponent<RectTransform> ().anchoredPosition.y + defaultRoomListScrollViewHeight);
			nodeGroup.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, -roomListContent.GetComponent<RectTransform> ().anchoredPosition.y * 0.9f);
		} else if(roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition.y < -240 && contentScrollStop) {
			networkManager = LobbyManager.GetComponent<NetworkManager> ();
			networkMatch = networkManager.matchMaker;

			contentScrollStop = false;
			pullBackScrollViewFlag = true;
			roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (0, -240);
			roomListScrollRect.GetComponent<ScrollRect> ().elasticity = 0.1f;
			//StartCoroutine (PullBackScrollView ());

			//ひっぱって出てくる矢印とローディング
			loadArrowIcon.SetActive(false);
			loadIcon.SetActive(true);
			networkMatch.ListMatches(0, 20, /*"{"+rank+"}"*/"", networkManager.OnMatchList);
		}
	}



	public IEnumerator PullBackScrollView(ListMatchResponse matchList){
		yield return new WaitForSeconds(3);
		Debug.Log ("PullBackScrollView"+pullBackScrollViewFlag);

		//GUIで表示メソッド実行
		roomListContent.GetComponent<GenMatchListController>().GenMatchList (matchList);

		while (pullBackScrollViewFlag) {
			roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition = Vector2.Lerp (roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition, new Vector3 (0, defaultRoomListScrollViewHeight, 0), 0.1f);
			if(roomListScrollRect.GetComponent<RectTransform> ().anchoredPosition.y >= defaultRoomListScrollViewHeight-1){
				pullBackScrollViewFlag = false;
				contentScrollStop = true;
			}
			yield return null;
		}

		//ひっぱって出てくる矢印とローディング
		loadArrowIcon.SetActive(true);
		loadIcon.SetActive(false);

		yield break;
	}
}
