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
	private bool listReloadFlag = true; //リロード可能フラグ
	private bool listReloadEndFlag = false; //リロード終了確認用フラグ
	private int scrollViewReloadHeight = -150; //ScrollViewがこのポジション以下まで下がったら更新開始
	private int scrollViewDefaultHeight = 0; //ScrollViewのデフォルト位置
	private float nodeGroupMoveRate = 0.80f; //戻るはやさに関係する
	private float scrollRectDefaultElasticity;
	private float pullBackscrollRectElasticity = 0.01f;

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

	void Awake(){
		//60fps
		Application.targetFrameRate = 60;
	}
		
	//メソッド
	void Start () {
		networkManager = LobbyManager.GetComponent<NetworkManager> ();
		networkMatch = networkManager.matchMaker;
		scrollRectDefaultElasticity = roomListScrollRect.elasticity;
	}

	void Update () {
		//loadIconがactiveなら回す
		if(loadIcon.activeSelf){
			loadIcon.transform.eulerAngles += new Vector3 (0f, 0f, -8f);
		}

		if (roomListContentRect.anchoredPosition.y < 0.0f && roomListScrollViewRectTransform.anchoredPosition.y > scrollViewReloadHeight) {
			//ScrollViewのポジションが下がっているので戻る速度をあげてすばやく上にひっぱり戻す
			roomListScrollRect.elasticity = pullBackscrollRectElasticity;
			//ContentにScrollRectを追従させる
			roomListScrollViewRectTransform.anchoredPosition = new Vector2 (0, roomListContentRect.anchoredPosition.y + scrollViewDefaultHeight);
			//nodeをContentが移動した分だけ反対に移動させて子の追従を打ち消す
			nodeGroupRectTransform.anchoredPosition = new Vector2 (0, -roomListContentRect.anchoredPosition.y * nodeGroupMoveRate);
		} else if(roomListScrollViewRectTransform.anchoredPosition.y <= scrollViewReloadHeight) {
			//reloadしてもいい状態か確認
			if (listReloadFlag) {
				//矢印とローディング
				loadArrowIcon.SetActive(false);
				loadIcon.SetActive(true);

				listReloadEndFlag = false;
				listReloadFlag = false;
				//ScrollViewのポジションが下がらなくなるので戻る速度をデフォルトに戻す
				roomListScrollRect.elasticity = scrollRectDefaultElasticity;
				//ズレるので決め打ちで値を入れておく
				roomListScrollViewRectTransform.anchoredPosition = new Vector2 (0, scrollViewReloadHeight);
				//リスト取得通信
				//StartCoroutine (PullBackScrollView (null));

				if(networkMatch == null){
					networkManager = LobbyManager.GetComponent<NetworkManager> ();
					networkMatch = networkManager.matchMaker;
				}
				networkMatch.ListMatches(0, 20, /*"{"+rank+"}"*/"", ListMatcheCallBack);
			} else {
				//reload通信が終了していれば引き戻す処理
				if(listReloadEndFlag){
					roomListScrollRect.elasticity = pullBackscrollRectElasticity;
					roomListScrollViewRectTransform.anchoredPosition = new Vector2 (0, roomListContentRect.anchoredPosition.y + scrollViewDefaultHeight);
					nodeGroupRectTransform.anchoredPosition = new Vector2 (0, -roomListContentRect.anchoredPosition.y * nodeGroupMoveRate);
				}
			}
		}

		//reloadした状態で上まで戻ったら再度reloadが行えるフラグをtrueにする
		if(!listReloadFlag){
			if(roomListScrollViewRectTransform.anchoredPosition.y >=  scrollViewDefaultHeight-1){
				listReloadFlag = true;
			}
		}
	}
				
	IEnumerator PullBackScrollView(ListMatchResponse matchList){
		yield return new WaitForSeconds(2);

		//GUIで表示メソッド実行
		if(matchList != null){
			roomListContent.GetComponent<GenMatchListController>().GenMatchList (matchList);
		}

		//ひっぱって出てくる矢印とローディング
		loadArrowIcon.SetActive(true);
		loadIcon.SetActive(false);

		listReloadEndFlag = true;

		yield break;
	}

	private void ListMatcheCallBack(ListMatchResponse matchList){
		StartCoroutine(PullBackScrollView(matchList));
	}

	public void GenerateMatchList(ListMatchResponse matchList){
		if(matchList != null){
			roomListContent.GetComponent<GenMatchListController>().GenMatchList (matchList);
		}
	}

	public void ClearRoomList(){
		roomListContent.GetComponent<GenMatchListController>().RemoveAllListViewItem ();
	}
}
