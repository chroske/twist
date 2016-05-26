using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class test : MonoBehaviour {
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
	private int scrollViewReloadHeight = -50; //ScrollViewがこのポジション以下まで下がったら更新開始 ReloadPanelの高さと同じでよい
	private int scrollViewDefaultHeight = 0; //ScrollViewのデフォルト位置
	private float offsetGroupMoveRate = 0.80f; //戻るはやさに関係する
	private float scrollRectDefaultElasticity;
	private float pullBackscrollRectElasticity = 0.01f;

	private ScrollRect scrollViewScrollRect;
	private RectTransform scrollViewRectTransform;
	private RectTransform contentRectTransform;
	private RectTransform offsetGroupRectTransform;

	void Start () {
		scrollViewScrollRect = GetComponent<ScrollRect> ();
		scrollViewRectTransform = GetComponent<RectTransform> ();
		contentRectTransform = content.GetComponent<RectTransform> ();
		offsetGroupRectTransform = offsetGroup.GetComponent<RectTransform> ();
		scrollRectDefaultElasticity = scrollViewScrollRect.elasticity;
	}

	void Update () {
		if (contentRectTransform.anchoredPosition.y < 0.0f && scrollViewRectTransform.anchoredPosition.y > scrollViewReloadHeight) {
			//ScrollViewのポジションが下がっているので戻る速度をあげてすばやく上にひっぱり戻す
			scrollViewScrollRect.elasticity = pullBackscrollRectElasticity;
			//ContentにScrollRectを追従させる
			scrollViewRectTransform.anchoredPosition = new Vector2 (0, contentRectTransform.anchoredPosition.y + scrollViewDefaultHeight);
			//offsetGroupをContentが移動した分だけ反対に移動させて子の追従を打ち消す
			offsetGroupRectTransform.anchoredPosition = new Vector2 (0, -contentRectTransform.anchoredPosition.y * offsetGroupMoveRate);
		} else if(scrollViewRectTransform.anchoredPosition.y <= scrollViewReloadHeight) {
			//reloadしてもいい状態か確認
			if (listReloadFlag) {
				listReloadEndFlag = false;
				listReloadFlag = false;

				//矢印を消してローディングを出す
				loadArrowIcon.SetActive(false);
				loadIcon.SetActive(true);

				//ScrollViewのポジションが下がらなくなるので戻る速度をデフォルトに戻す
				scrollViewScrollRect.elasticity = scrollRectDefaultElasticity;
				//ズレるので決め打ちで値を入れておく
				scrollViewRectTransform.anchoredPosition = new Vector2 (0, scrollViewReloadHeight);
				//リスト取得通信(デモなので実際の通信はしないよ)
				StartCoroutine (PullBackScrollView ());
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
		//loadIconを監視してactiveなら回す
		if(loadIcon.activeSelf){
			loadIcon.transform.eulerAngles += new Vector3 (0f, 0f, -8f);
		}
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
