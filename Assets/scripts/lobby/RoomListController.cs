using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.EventSystems;

public class RoomListController : MonoBehaviour {
	
	[SerializeField]
	private GameObject roomListContent;

	[SerializeField]
	private GameObject roomListScrollView;

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

	public bool contentScrollStop = false; //true=止まってる　false=初期状態
	private int defaultRoomListScrollViewHeight = -90;
	public bool startListMatcheFlag = true; //ListMatcheの通信してもよいフラグ

	//プロパティ群
	RectTransform _roomListContentRect;
	RectTransform roomListContentRect
	{
		get {
			_roomListContentRect = _roomListContentRect ?? (_roomListContentRect = roomListContent.GetComponent<RectTransform>());
			return _roomListContentRect;
		}
	}

	ScrollRect _roomListScrollRect;
	ScrollRect roomListScrollRect
	{
		get {
			_roomListScrollRect = _roomListScrollRect ?? (_roomListScrollRect = roomListScrollView.GetComponent<ScrollRect> ());
			return _roomListScrollRect;
		}
	}

	RectTransform _roomListScrollViewRectTransform;
	RectTransform roomListScrollViewRectTransform
	{
		get {
			_roomListScrollViewRectTransform = _roomListScrollViewRectTransform ?? (_roomListScrollViewRectTransform = roomListScrollView.GetComponent<RectTransform> ());
			return _roomListScrollViewRectTransform;
		}
	}

	RectTransform _nodeGroupRectTransform;
	RectTransform nodeGroupRectTransform
	{
		get {
			_nodeGroupRectTransform = _nodeGroupRectTransform ?? (_nodeGroupRectTransform = nodeGroup.GetComponent<RectTransform> ());
			return _nodeGroupRectTransform;
		}
	}

	// Use this for initialization
	void Start () {
		networkManager = LobbyManager.GetComponent<NetworkManager> ();
		networkMatch = networkManager.matchMaker;
	}

	// Update is called once per frame
	bool ListReloadFlag = true;
	void Update () {
		if(loadIcon.activeSelf){
			loadIcon.transform.eulerAngles += new Vector3 (0f, 0f, -3f);
		}

		if (roomListContentRect.anchoredPosition.y < 0.0f && roomListScrollViewRectTransform.anchoredPosition.y > -240) {
			//if (ListReloadFlag) {
				//これなんだっけ
				roomListScrollRect.elasticity = 0.01f;
				//ContentにScrollRectを追従させる
				roomListScrollViewRectTransform.anchoredPosition = new Vector2 (0, roomListContentRect.anchoredPosition.y + defaultRoomListScrollViewHeight);
				//nodeをContentが移動した分だけ反対に移動させて子の追従を打ち消す
				nodeGroupRectTransform.anchoredPosition = new Vector2 (0, -roomListContentRect.anchoredPosition.y * 0.9f);
			//}
		} else if(roomListScrollViewRectTransform.anchoredPosition.y < -240) {
			if (ListReloadFlag) {
				//矢印とローディング
				loadArrowIcon.SetActive(false);
				loadIcon.SetActive(true);

				ListReloadFlag = false;
				roomListScrollRect.elasticity = 0.07f;
				roomListScrollViewRectTransform.anchoredPosition = new Vector2 (0, -240);
				//StartCoroutine (PullBackScrollView (null));
				networkMatch.ListMatches(0, 20, /*"{"+rank+"}"*/"", ListMatcheCallBack);
			} else {
				roomListScrollRect.elasticity = 0.0001f;
				//ContentにScrollRectを追従させる
				roomListScrollViewRectTransform.anchoredPosition = new Vector2 (0, roomListContentRect.anchoredPosition.y + defaultRoomListScrollViewHeight);
				//nodeをContentが移動した分だけ反対に移動させて子の追従を打ち消す
				nodeGroupRectTransform.anchoredPosition = new Vector2 (0, -roomListContentRect.anchoredPosition.y * 0.95f);
			}
		}

		if(!ListReloadFlag){
			if(roomListScrollViewRectTransform.anchoredPosition.y >=  -91){
				Debug.Log ("End");
				ListReloadFlag = true;
			}
		}
	}
		
	IEnumerator PullBackScrollView(ListMatchResponse matchList){
		yield return new WaitForSeconds(1);
		Debug.Log ("PullBackScrollView2");
		roomListScrollViewRectTransform.anchoredPosition = new Vector2 (roomListScrollViewRectTransform.anchoredPosition.x,-239.0f);
		roomListScrollRect.StopMovement();

		//GUIで表示メソッド実行
		if(matchList != null){
			roomListContent.GetComponent<GenMatchListController>().GenMatchList (matchList);
		}

		//ひっぱって出てくる矢印とローディング
		loadArrowIcon.SetActive(true);
		loadIcon.SetActive(false);

		yield break;
	}

	private void ListMatcheCallBack(ListMatchResponse matchList){
		Debug.Log ("ListMatcheCallBack");
		StartCoroutine(PullBackScrollView(matchList));
	}


	private bool pullBackScrollAnimationFlag = false;
	public IEnumerator PullBackScrollView2(ListMatchResponse matchList){
		startListMatcheFlag = false;
		Debug.Log ("XXXXXXXXXXXXXXXX");
		yield return new WaitForSeconds(1);
		Debug.Log ("PullBackScrollView"+startListMatcheFlag);

		//GUIで表示メソッド実行
		if(matchList != null){
			roomListContent.GetComponent<GenMatchListController>().GenMatchList (matchList);
		}

		//戻るアニメーション
		pullBackScrollAnimationFlag = true;
		while (pullBackScrollAnimationFlag) {
			//タップ中ならアニメーションさせない
			if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved) || Input.GetMouseButton (0)) {
				yield return null;
			} else {
				float rate = 0.1f;
				contentScrollStop = false;
				roomListScrollViewRectTransform.anchoredPosition = Vector2.Lerp (roomListScrollViewRectTransform.anchoredPosition, new Vector2 (0, defaultRoomListScrollViewHeight), rate);
				if (roomListScrollViewRectTransform.anchoredPosition.y >= defaultRoomListScrollViewHeight - 1) {
					pullBackScrollAnimationFlag = false;
					contentScrollStop = false;
					startListMatcheFlag = true;
					Debug.Log ("Animation end");
				}
				Debug.Log ("BBBB");
				yield return null;
			}
		}

		//ひっぱって出てくる矢印とローディング
		loadArrowIcon.SetActive(true);
		loadIcon.SetActive(false);

		yield break;
	}
}
