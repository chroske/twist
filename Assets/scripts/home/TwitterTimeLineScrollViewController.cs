using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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


	private bool listReloadFlag = true; //リロード可能フラグ
	private bool listReloadEndFlag = false; //リロード終了確認用フラグ
	private int scrollViewReloadHeight = -150; //ScrollViewがこのポジション以下まで下がったら更新開始 ReloadPanelの高さに準ずる
	private int scrollViewDefaultHeight = 0; //ScrollViewのデフォルト位置
	private float offsetGroupMoveRate = 0.80f; //戻るはやさに関係する
	private float scrollRectDefaultElasticity; //デフォルトのelasticityを入れておく
	private float pullBackscrollRectElasticity = 0.01f; //リロード開始までの引っ張って戻る速さに関係する

	private ScrollRect scrollViewScrollRect;
	private RectTransform scrollViewRectTransform;
	private RectTransform contentRectTransform;
	private RectTransform offsetGroupRectTransform;

	void Start () {
		scrollViewScrollRect = scrollView.GetComponent<ScrollRect> ();
		scrollViewRectTransform = scrollView.GetComponent<RectTransform> ();
		contentRectTransform = content.GetComponent<RectTransform> ();
		offsetGroupRectTransform = offsetGroup.GetComponent<RectTransform> ();
		scrollRectDefaultElasticity = scrollViewScrollRect.elasticity;

		//json配列に対応させるためにはどうしたらいいか考える
		string json = "[{\"created_at\":\"Wed Jun 08 08:02:25 +0000 2016\",\"id\":740453916056428544,\"id_str\":\"740453916056428544\",\"text\":\"\\u304a\\u3063\\u3057\\u3083\\uff15\\u6642\\u3060\",\"truncated\":false,\"entities\":{\"hashtags\":[],\"symbols\":[],\"user_mentions\":[],\"urls\":[]},\"source\":\"\\u003ca href=\\\"http:\\/\\/twitter.com\\/kiyozamurai\\\" rel=\\\"nofollow\\\"\\u003e\\u9b54\\u5973\\u3063\\u5b50\\u30de\\u30b8\\u30ab\\u30eb\\u6e05\\u9ebb\\u5442\\u30af\\u30e9\\u30a4\\u30a2\\u30f3\\u30c8\\u003c\\/a\\u003e\",\"in_reply_to_status_id\":null,\"in_reply_to_status_id_str\":null,\"in_reply_to_user_id\":null,\"in_reply_to_user_id_str\":null,\"in_reply_to_screen_name\":null,\"user\":{\"id\":91616437,\"id_str\":\"91616437\",\"name\":\"\\u6e05\\u9ebb\\u5442\",\"screen_name\":\"kiyozamurai\",\"location\":\"\\u9759\\u5ca1\\u770c\\u5468\\u667a\\u90e1\\u68ee\\u753a\",\"description\":\"\\u8da3\\u5473\\u30cd\\u30c8\\u30b2\\uff06\\u30ab\\u30e1\\u30e9\\u3002FEZ\\u306fJ\\u9bd6\\u300c\\u6e05\\u9ebf\\u300d\\u300c\\u6e05\\u4e4b\\u4e1e\\u300d\\u3002 \\u3078\\u30fc\\u30d3\\u30fc\\u30b9\\u30e2\\u30fc\\u30ab\\u30fc\\u3002\\u304a\\u9152\\u3082\\u597d\\u304d\\u3002 \\u4e3b\\u88c5\\u5099\\uff1aX100\\/E-PL1\\/F3\\/LC-A+ \\u526f\\u88c5\\u5099\\uff1aXQ2 \\u732b\\u30d1\\u30f3\\u30c1\\uff1aPOCKET DIGITAL CAMERA SQ30m\",\"url\":null,\"entities\":{\"description\":{\"urls\":[]}},\"protected\":false,\"followers_count\":271,\"friends_count\":260,\"listed_count\":21,\"created_at\":\"Sat Nov 21 17:36:12 +0000 2009\",\"favourites_count\":6,\"utc_offset\":-36000,\"time_zone\":\"Hawaii\",\"geo_enabled\":true,\"verified\":false,\"statuses_count\":19259,\"lang\":\"ja\",\"contributors_enabled\":false,\"is_translator\":false,\"is_translation_enabled\":false,\"profile_background_color\":\"B2DFDA\",\"profile_background_image_url\":\"http:\\/\\/abs.twimg.com\\/images\\/themes\\/theme13\\/bg.gif\",\"profile_background_image_url_https\":\"https:\\/\\/abs.twimg.com\\/images\\/themes\\/theme13\\/bg.gif\",\"profile_background_tile\":false,\"profile_image_url\":\"http:\\/\\/pbs.twimg.com\\/profile_images\\/2591291484\\/ldj8k0ztqxxobys9uhyq_normal.jpeg\",\"profile_image_url_https\":\"https:\\/\\/pbs.twimg.com\\/profile_images\\/2591291484\\/ldj8k0ztqxxobys9uhyq_normal.jpeg\",\"profile_banner_url\":\"https:\\/\\/pbs.twimg.com\\/profile_banners\\/91616437\\/1396686909\",\"profile_link_color\":\"93A644\",\"profile_sidebar_border_color\":\"EEEEEE\",\"profile_sidebar_fill_color\":\"FFFFFF\",\"profile_text_color\":\"333333\",\"profile_use_background_image\":true,\"has_extended_profile\":false,\"default_profile\":false,\"default_profile_image\":false,\"following\":true,\"follow_request_sent\":false,\"notifications\":false},\"geo\":null,\"coordinates\":null,\"place\":null,\"contributors\":null,\"is_quote_status\":false,\"retweet_count\":0,\"favorite_count\":0,\"favorited\":false,\"retweeted\":false,\"lang\":\"ja\"},\n{\"created_at\":\"Wed Jun 08 08:01:21 +0000 2016\",\"id\":740453650892492800,\"id_str\":\"740453650892492800\",\"text\":\"\\u30b5\\u30a4\\u30aa\\u30f3\\u304b\\u306a\\u30fb\\u30fb\\u30fb\\uff1f\",\"truncated\":false,\"entities\":{\"hashtags\":[],\"symbols\":[],\"user_mentions\":[],\"urls\":[]},\"source\":\"\\u003ca href=\\\"https:\\/\\/sites.google.com\\/site\\/tweentwitterclient\\/\\\" rel=\\\"nofollow\\\"\\u003eTween\\u003c\\/a\\u003e\",\"in_reply_to_status_id\":null,\"in_reply_to_status_id_str\":null,\"in_reply_to_user_id\":null,\"in_reply_to_user_id_str\":null,\"in_reply_to_screen_name\":null,\"user\":{\"id\":71341787,\"id_str\":\"71341787\",\"name\":\"\\u30ea\\u30ea\\u30c6\\u30a3\\u30a2\",\"screen_name\":\"LiLiTeA\",\"location\":\"\\u683c\\u7d0d\\u5eab\",\"description\":\"('\\u03c9')\\uff01\\u3000\\u8272\\u3005\\u3068\\u30c0\\u30e1\\u306a\\u4eba\\u9593\\u3067\\u3059\\u304c\\u3001\\u7d75\\u3092\\u63cf\\u304f\\u3053\\u3068\\u304c\\u3067\\u304d\\u307e\\u3059\\u3002  \\u6700\\u8fd1\\u30d7\\u30ec\\u30a4\\u4e2d\\u306e\\u30b2\\u30fc\\u30e0\\u2192\\u745e\\u9cf3in\\u6a2a\\u93ae\\u3000\\u30a8\\u30aa\\u30eb\\u30bc\\u30a2\\uff1aBahamut\\u3000PSO2\\uff1aship4\\u3000\\u30b0\\u30e9\\u30d6\\u30eb\\uff1a\\u30bc\\u30a8\\u30f3\\u6559\\u5f92\",\"url\":\"https:\\/\\/t.co\\/5FR8jTEvTg\",\"entities\":{\"url\":{\"urls\":[{\"url\":\"https:\\/\\/t.co\\/5FR8jTEvTg\",\"expanded_url\":\"http:\\/\\/pixiv.me\\/lilitea\",\"display_url\":\"pixiv.me\\/lilitea\",\"indices\":[0,23]}]},\"description\":{\"urls\":[]}},\"protected\":false,\"followers_count\":2049,\"friends_count\":523,\"listed_count\":153,\"created_at\":\"Thu Sep 03 19:50:18 +0000 2009\",\"favourites_count\":3022,\"utc_offset\":32400,\"time_zone\":\"Tokyo\",\"geo_enabled\":false,\"verified\":false,\"statuses_count\":128255,\"lang\":\"ja\",\"contributors_enabled\":false,\"is_translator\":false,\"is_translation_enabled\":false,\"profile_background_color\":\"EDECE9\",\"profile_background_image_url\":\"http:\\/\\/abs.twimg.com\\/images\\/themes\\/theme3\\/bg.gif\",\"profile_background_image_url_https\":\"https:\\/\\/abs.twimg.com\\/images\\/themes\\/theme3\\/bg.gif\",\"profile_background_tile\":false,\"profile_image_url\":\"http:\\/\\/pbs.twimg.com\\/profile_images\\/715914597413335040\\/pYrmSMZZ_normal.jpg\",\"profile_image_url_https\":\"https:\\/\\/pbs.twimg.com\\/profile_images\\/715914597413335040\\/pYrmSMZZ_normal.jpg\",\"profile_banner_url\":\"https:\\/\\/pbs.twimg.com\\/profile_banners\\/71341787\\/1459522222\",\"profile_link_color\":\"088253\",\"profile_sidebar_border_color\":\"D3D2CF\",\"profile_sidebar_fill_color\":\"E3E2DE\",\"profile_text_color\":\"634047\",\"profile_use_background_image\":true,\"has_extended_profile\":false,\"default_profile\":false,\"default_profile_image\":false,\"following\":true,\"follow_request_sent\":false,\"notifications\":false},\"geo\":null,\"coordinates\":null,\"place\":null,\"contributors\":null,\"is_quote_status\":false,\"retweet_count\":0,\"favorite_count\":0,\"favorited\":false,\"retweeted\":false,\"lang\":\"ja\"}]";
//		TwitterTimeLineData twitterTimeLineData = new TwitterTimeLineData();
//		var obj = JsonUtility.FromJson<TwitterTimeLineData> (json);
		//Debug.Log (obj[0].user.name);

		//var objects = getJsonArray<TwitterTimeLineScrollViewController> (json);
	}

//	public static T[] getJsonArray<T>(string json){
//		TwitterTimeLineData<T> wrapper = JsonUtility.FromJson<TwitterTimeLineData<T>> (json);
//		return wrapper.array;
//	}

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
				StartCoroutine (PullBackScrollView ());

				//StartCoroutine (Twitter.API.GetTimeLine (gameStateManager.CONSUMER_KEY, gameStateManager.CONSUMER_SECRET, gameStateManager.m_AccessTokenResponse, new Twitter.TimeLineCallback(this.OnGetTimeLineCallback)));

//				if(networkMatch == null){
//					networkManager = LobbyManager.GetComponent<NetworkManager> ();
//					networkMatch = networkManager.matchMaker;
//				}
//				networkMatch.ListMatches(0, 20, /*"{"+rank+"}"*/"", ListMatcheCallBack);
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
			Debug.Log (response.Json);

			GenerateTimeLineNode (response.Json);
			//GUIで表示メソッド実行
//			if(matchList != null){
//				roomListContent.GetComponent<GenMatchListController>().GenMatchList (matchList);
//			}

			//ひっぱって出てくる矢印とローディング
			loadArrowIcon.SetActive(true);
			loadIcon.SetActive(false);

			listReloadEndFlag = true;
		}
		else
		{
			print("OnGetTimeLineCallback - failed.");
		}
	}
		

	void GenerateTimeLineNode(string json){
//		TwitterTimeLineData twitterTimeLineData = new TwitterTimeLineData();
//		var obj = JsonUtility.FromJson<TwitterTimeLineData> (json);
//
//		//TwitterTimeLineData timeLineJsonData = twitterTimeLineData.CreateFromJSON (json);
//		Debug.Log (obj.created_at);
	}

	IEnumerator PullBackScrollView(){
		//とりあえず２秒クルクルさせる
		yield return new WaitForSeconds(2);

		/* ここらへんで通信してデータ取って来てリストに追加する */

		//ローディングから矢印に切り替え
		loadArrowIcon.SetActive(true);
		loadIcon.SetActive(false);
		listReloadEndFlag = true;

		yield break;
	}
}
