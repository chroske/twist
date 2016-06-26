using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OwnedUnitListController : MonoBehaviour {
	[SerializeField]
	RectTransform scrollViewNode;
	[SerializeField]
	GameObject content;
	[SerializeField]
	GameObject backPanelButton;
	[SerializeField]
	NavigationBarController navigationBar;
	[SerializeField]
	GameObject UnitDetailPanel;
	[SerializeField]
	UnitDetailController unitDetailController;
	[SerializeField]
	GameStateManager gameStateManager;
	[SerializeField]
	PartyUnitListController partyUnitListController;
	[SerializeField]
	GameObject RemoveUnitOnPartyButton;

	public GameObject nextPanelObj;
	public string ownedUnitListMode;
	public int selectPartyChangeId;

	void Start () {
		GenOwnedUnitList (gameStateManager.ownedUnitDic);
	}

	public void GotoUnitDetailPanel(int unitId){
		unitDetailController.unitId = unitId;
	}

	//パーティセット＆入れ替え
	public void SetPartyUnit(int unitId){
		gameStateManager.ownedUnitDic[unitId].party_id = selectPartyChangeId;

		//partyUnitDicにkeyが存在してたら入れ替え元のparty_idを0にする
		if(gameStateManager.partyUnitDic.ContainsKey(selectPartyChangeId)){
			gameStateManager.ownedUnitDic[gameStateManager.partyUnitDic[selectPartyChangeId].unit_id].party_id = 0;
		}

		//party_idを入れ替えたのでpartyDic再生成
		gameStateManager.ResetPartyDic ();

		partyUnitListController.ReloadPartyUnitList ();
	}

	//パーティからユニットはずす
	public void RemovePartyUnit(){
		gameStateManager.ownedUnitDic[gameStateManager.partyUnitDic[selectPartyChangeId].unit_id].party_id = 0;
		//party_idを入れ替えたのでpartyDic再生成
		gameStateManager.ResetPartyDic ();

		partyUnitListController.ReloadPartyUnitList ();
	}

	void GenOwnedUnitList(Dictionary<int, OwnedUnitData> ownedUnitDic){
		//リストをクリア
		RemoveAllListViewItem ();

		foreach(KeyValuePair<int, OwnedUnitData> ownedUnitDataPair in ownedUnitDic){
			var node = GameObject.Instantiate(scrollViewNode) as RectTransform;
			node.SetParent(content.transform, false);
			UnitListNodeController unitListNodeController = node.GetComponent<UnitListNodeController> ();
			unitListNodeController.unitId = ownedUnitDataPair.Value.unit_id;
			unitListNodeController.partyId = ownedUnitDataPair.Value.party_id;
			unitListNodeController.iconImageUrl = ownedUnitDataPair.Value.unit_icon_url;
			unitListNodeController.ownedUnitListController = this;
			SetNextPanelObj ();
			unitListNodeController.GetComponent<uTools.uPlayTween> ().ChangeTweenTarget (UnitDetailPanel);
			var text = node.GetComponentInChildren<Text>();
			//rank番号とカッコを取り除く
			text.text = ownedUnitDataPair.Value.unit_name;
		}
		CheckPartyInUnit ();
	}
		
	//リストクリア
	public void RemoveAllListViewItem() {
		foreach (Transform child in content.transform) {
			if(child.GetSiblingIndex() != 0){
				GameObject.Destroy(child.gameObject);
			}
		}
	}

	//遷移アニメーション終了時に呼び出される
	public void AnimationEnd(){
		if (this.gameObject.GetComponent<RectTransform>().anchoredPosition.x == 0) {
			navigationBar.ChangeTweenPanel (this.gameObject);
			SetNextPanelObj ();
			GenOwnedUnitList (gameStateManager.ownedUnitDic);
		} else {
			navigationBar.RollBackTweenPanel ();
		}
	}

	//一覧パネルのモードに応じて次の遷移先を決める
	private void SetNextPanelObj(){
		if(ownedUnitListMode == "PartyEdit"){
			nextPanelObj = this.gameObject;
		} else if(ownedUnitListMode == "Detail"){
			nextPanelObj = UnitDetailPanel;
		}
	}

	//パーティにいるユニットはボタンを無効にする
	public void CheckPartyInUnit(){
		foreach (Transform child in content.transform) {
			if (ownedUnitListMode == "PartyEdit") {
				if(child.gameObject.name != "RemoveUnitOnPartyButton"){
					if (gameStateManager.ownedUnitDic [child.transform.GetComponent<UnitListNodeController> ().unitId].party_id != 0) {
						child.transform.GetComponent<Button> ().interactable = false;
					} else {
						child.transform.GetComponent<Button> ().interactable = true;
					}
				}
				RemoveUnitOnPartyButton.SetActive (true);
			} else {
				child.transform.GetComponent<Button> ().interactable = true;
				RemoveUnitOnPartyButton.SetActive (false);
			}
		}
	}
}
