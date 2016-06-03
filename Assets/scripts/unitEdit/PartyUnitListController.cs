using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PartyUnitListController : MonoBehaviour {
	[SerializeField]
	RectTransform scrollViewNode;
	[SerializeField]
	GameStateManager gameStateManager;
	[SerializeField]
	GameObject content;
	[SerializeField]
	NavigationBarController navigationBar;

	public GameObject ownedUnitListPanel;

	void Start () {
		GenPartyUnitList (gameStateManager.partyUnitDic);
	}

	void GenPartyUnitList(Dictionary<int, OwnedUnitData> partyUnitDic){
		//リストをクリア
		RemoveAllListViewItem ();

		for(int i = 1; i <= 4; i++){
			if (partyUnitDic.ContainsKey (i)) {
				OwnedUnitData partyUnitData = partyUnitDic [i];
				var node = GameObject.Instantiate(scrollViewNode) as RectTransform;
				node.SetParent(content.transform, false);
				//node.GetComponent<Button> ().onClick.AddListener (() => OnClickUnitListNode());

				PartyUnitListNodeController partyUnitListNodeController = node.GetComponent<PartyUnitListNodeController> ();
				partyUnitListNodeController.unitId = partyUnitData.unit_id;
				partyUnitListNodeController.partyId = partyUnitData.party_id;
				partyUnitListNodeController.ownedUnitListPanel = ownedUnitListPanel;

				uTools.uPlayTween uPlayTween = node.GetComponent<uTools.uPlayTween> ();
				uPlayTween.tweenTarget = ownedUnitListPanel;

				var text = node.GetComponentInChildren<Text>();

				//rank番号とカッコを取り除く
				text.text = partyUnitData.unit_name;
			} else {
				var node = GameObject.Instantiate(scrollViewNode) as RectTransform;
				node.SetParent(content.transform, false);

				PartyUnitListNodeController partyUnitListNodeController = node.GetComponent<PartyUnitListNodeController> ();
				partyUnitListNodeController.partyId = i;
				partyUnitListNodeController.ownedUnitListPanel = ownedUnitListPanel;

				uTools.uPlayTween uPlayTween = node.GetComponent<uTools.uPlayTween> ();
				uPlayTween.tweenTarget = ownedUnitListPanel;

				var text = node.GetComponentInChildren<Text>();

				//rank番号とカッコを取り除く
				text.text = "空きスロット";
			}
		}
	}

	//リストクリア
	public void RemoveAllListViewItem() {
		foreach (Transform child in content.transform) {
			GameObject.Destroy(child.gameObject);
		}
	}

	//リスト更新
	public void ReloadPartyUnitList(){
		//変更があったらリスト再生成
		GenPartyUnitList (gameStateManager.partyUnitDic);
	}

	//遷移アニメーション終了時に呼び出される
	public void AnimationEnd(){
		if (this.gameObject.GetComponent<RectTransform>().anchoredPosition.x == 0) { //戻った時
			navigationBar.ChangeTweenPanel (this.gameObject);
		} else { //画面外に移動した時
			navigationBar.RollBackTweenPanel ();
		}
	}
}
