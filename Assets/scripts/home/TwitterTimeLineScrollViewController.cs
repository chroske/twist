using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using System.Text.RegularExpressions;

public class TwitterTimeLineScrollViewController : MonoBehaviour {

	[SerializeField]
	private GameStateManager gameStateManager;
	[SerializeField]
	private GameObject scrollView;
	[SerializeField]
	private GameObject content;
	[SerializeField]
	private GameObject offsetGroup;
	[SerializeField]
	private GameObject loadIcon;
	[SerializeField]
	private GameObject loadArrowIcon;
	[SerializeField]
	private GameObject timeLineNode;


	private bool listReloadFlag = true; //リロード可能フラグ
	private bool listReloadEndFlag = false; //リロード終了確認用フラグ
	private int scrollViewReloadHeight = -150; //ScrollViewがこのポジション以下まで下がったら更新開始 ReloadPanelの高さに準ずる
	private int scrollViewDefaultHeight = 0; //ScrollViewのデフォルト位置
	private float offsetGroupMoveRate = 0.80f; //戻るはやさに関係する
	private float scrollRectDefaultElasticity; //デフォルトのelasticityを入れておく
	private float pullBackscrollRectElasticity = 0.01f; //リロード開始までの引っ張って戻る速さに関係する

	private string sinceId = "0";

	private ScrollRect scrollViewScrollRect;
	private RectTransform scrollViewRectTransform;
	private RectTransform contentRectTransform;
	private RectTransform offsetGroupRectTransform;

	void Awake(){
		//60fps
		Application.targetFrameRate = 60;
	}

	void Start () {
		scrollViewScrollRect = scrollView.GetComponent<ScrollRect> ();
		scrollViewRectTransform = scrollView.GetComponent<RectTransform> ();
		contentRectTransform = content.GetComponent<RectTransform> ();
		offsetGroupRectTransform = offsetGroup.GetComponent<RectTransform> ();
		scrollRectDefaultElasticity = scrollViewScrollRect.elasticity;
	}

	void Update () {
		//loadIconがactiveなら回す
		if(loadIcon.activeSelf){
			loadIcon.transform.eulerAngles += new Vector3 (0f, 0f, -8f);
		}

		if (contentRectTransform.anchoredPosition.y < 0.0f && scrollViewRectTransform.anchoredPosition.y > scrollViewReloadHeight) {
			//ScrollViewのポジションが下がっているので戻る速度をあげてすばやく上にひっぱり戻す
			scrollViewScrollRect.elasticity = pullBackscrollRectElasticity;
			//ContentにScrollRectを追従させる
			scrollViewRectTransform.anchoredPosition = new Vector2 (0, contentRectTransform.anchoredPosition.y + scrollViewDefaultHeight);
			//nodeをContentが移動した分だけ反対に移動させて子の追従を打ち消す
			offsetGroupRectTransform.anchoredPosition = new Vector2 (0, -contentRectTransform.anchoredPosition.y * offsetGroupMoveRate);
		} else if(scrollViewRectTransform.anchoredPosition.y <= scrollViewReloadHeight) {
			//reloadしてもいい状態か確認
			if (listReloadFlag) {
				//矢印とローディング
				loadArrowIcon.SetActive(false);
				loadIcon.SetActive(true);

				listReloadEndFlag = false;
				listReloadFlag = false;
				//ScrollViewのポジションが下がらなくなるので戻る速度をデフォルトに戻す
				scrollViewScrollRect.elasticity = scrollRectDefaultElasticity;
				//ズレるので決め打ちで値を入れておく
				scrollViewRectTransform.anchoredPosition = new Vector2 (0, scrollViewReloadHeight);
				//リスト取得通信
				StartCoroutine (Twitter.API.GetTimeLine (gameStateManager.CONSUMER_KEY, gameStateManager.CONSUMER_SECRET, gameStateManager.m_AccessTokenResponse, sinceId, new Twitter.TimeLineCallback(this.OnGetTimeLineCallback)));
			} else {
				//reload通信が終了していれば引き戻す処理
				if(listReloadEndFlag){
					scrollViewScrollRect.elasticity = pullBackscrollRectElasticity;
					scrollViewRectTransform.anchoredPosition = new Vector2 (0, contentRectTransform.anchoredPosition.y + scrollViewDefaultHeight);
					offsetGroupRectTransform.anchoredPosition = new Vector2 (0, -contentRectTransform.anchoredPosition.y * offsetGroupMoveRate);
				}
			}
		}

		//reloadした状態で上まで戻ったら再度reloadが行えるフラグをtrueにする
		if(!listReloadFlag){
			if(scrollViewRectTransform.anchoredPosition.y >=  scrollViewDefaultHeight-1){
				listReloadFlag = true;
			}
		}
	}

	void OnGetTimeLineCallback(bool success, Twitter.TimeLineResponse response)
	{
		if (success)
		{
			GenerateTimeLineNode (response.Json);
		}
		else
		{
			print("OnGetTimeLineCallback - failed.");
		}

		//ひっぱって出てくる矢印とローディング
		loadArrowIcon.SetActive(true);
		loadIcon.SetActive(false);

		listReloadEndFlag = true;
	}

	void GenerateTimeLineNode(string json){
		int tweetCounter = 0;
		IList TimeLineList = (IList)Json.Deserialize(json);

		foreach (IDictionary tweetData in TimeLineList) {
			if(tweetCounter == 0){
				sinceId = tweetData["id"].ToString();
			}
			IDictionary userDatas = (IDictionary)tweetData["user"];

			RectTransform node = GameObject.Instantiate(timeLineNode.transform) as RectTransform;
			node.SetParent(content.transform, false);
			node.SetSiblingIndex(tweetCounter+1); //引っ張り部分があるので+1

			TimeLineNodeController timeLineNodeController = node.GetComponentInChildren<TimeLineNodeController>();
			//リツイートなら
			if (tweetData ["retweeted_status"] != null) {
				IDictionary retweetDatas = (IDictionary)tweetData["retweeted_status"];
				IDictionary rtUserDatas = (IDictionary)retweetDatas["user"];
				timeLineNodeController.SetNodeDatasRT (retweetDatas["text"].ToString(),rtUserDatas["name"].ToString(), rtUserDatas["screen_name"].ToString(), rtUserDatas["profile_image_url"].ToString(), userDatas ["name"].ToString(), userDatas ["screen_name"].ToString());
			} else {
				timeLineNodeController.SetNodeDatas (tweetData ["text"].ToString (), userDatas ["name"].ToString (), userDatas ["screen_name"].ToString (), userDatas ["profile_image_url"].ToString ());
			}

			tweetCounter++;
		}
	}
}
