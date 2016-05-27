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

	public GameObject ownedUnitPanelList;
	public GameObject fromPanel;

	private Dictionary<int, OwnedUnitData> partyUnitDic = new Dictionary<int, OwnedUnitData> ();

	// Use this for initialization
	void Start () {
		//GameStateManager gameStateManager = gameStateManagerObj.GetComponent<GameStateManager> ();
		//List<OwnedUnitData> partyUnitList = gameStateManager.partyUnitList1;
		GenPartyUnitList (gameStateManager.partyUnitDic);
	}

	void GenPartyUnitList(Dictionary<int, OwnedUnitData> partyUnitDic){
		//リストをクリア
		RemoveAllListViewItem ();

		//foreach(OwnedUnitData ownedUnitData in partyUnitList){
		foreach(KeyValuePair<int, OwnedUnitData> partyUnitDataPair in partyUnitDic){
			var node = GameObject.Instantiate(scrollViewNode) as RectTransform;
			node.SetParent(content.transform, false);
			//node.GetComponent<Button> ().onClick.AddListener (() => OnClickUnitListNode());

			PartyUnitListNodeController partyUnitListNodeController = node.GetComponent<PartyUnitListNodeController> ();
			partyUnitListNodeController.unitId = partyUnitDataPair.Value.unit_id;
			partyUnitListNodeController.partyId = partyUnitDataPair.Value.party_id;
			partyUnitListNodeController.ownedUnitPanelList = ownedUnitPanelList;

			uTools.uPlayTween uPlayTween = node.GetComponent<uTools.uPlayTween> ();
			uPlayTween.tweenTarget = ownedUnitPanelList;

			var text = node.GetComponentInChildren<Text>();

			//rank番号とカッコを取り除く
			text.text = partyUnitDataPair.Value.unit_name;
		}
	}

//	public void OnClickUnitListNode(){
//		ownedUnitPanelList.SetActive (true);
//		OwnedUnitListController ownedUnitListController = ownedUnitPanelList.GetComponent<OwnedUnitListController> ();
//		ownedUnitListController.ownedUnitListMode = "PartyEdit";
//		ownedUnitListController.selectPartyChangeUnitId ;
//	}

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
