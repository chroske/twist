using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class OwnedUnitListController : MonoBehaviour {

	[SerializeField]
	RectTransform scrollViewNode;

	[SerializeField]
	GameObject gameStateManagerObj;

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

	public GameObject nextPanelObj;
	public string ownedUnitListMode;
	public int selectPartyChangeUnitId;

	// Use this for initialization
	void Start () {
		//GameStateManager gameStateManager = gameStateManagerObj.GetComponent<GameStateManager> ();
		Dictionary<int, OwnedUnitData> ownedUnitDic = gameStateManager.ownedUnitDic;
		GenOwnedUnitList (ownedUnitDic);
	}

	public void GotoUnitDetailPanel(int unitId){
		unitDetailController.unitId = unitId;
	}

	public void SetPartyUnit(int unitId){
		gameStateManager.ownedUnitDic[unitId].party_id = gameStateManager.partyUnitDic[selectPartyChangeUnitId].party_id;
		gameStateManager.ownedUnitDic[selectPartyChangeUnitId].party_id = 0;
		//party_idを入れ替えたのでpartyDic再生成
		gameStateManager.ResetPartyDic ();

		partyUnitListController.ReloadPartyUnitList ();
	}

	void GenOwnedUnitList(Dictionary<int, OwnedUnitData> ownedUnitDic){
		//リストをクリア
		RemoveAllListViewItem ();

		//foreach(OwnedUnitData ownedUnitData in ownedUnitList){
		foreach(KeyValuePair<int, OwnedUnitData> ownedUnitDataPair in ownedUnitDic){
			var node = GameObject.Instantiate(scrollViewNode) as RectTransform;
			node.SetParent(content.transform, false);
			UnitListNodeController unitListNodeController = node.GetComponent<UnitListNodeController> ();
			//unitListNodeController.unitId = ownedUnitData.unit_id;
			unitListNodeController.unitId = ownedUnitDataPair.Value.unit_id;
			unitListNodeController.ownedUnitListController = this;
			SetNextPanelObj ();

			unitListNodeController.GetComponent<uTools.uPlayTween> ().ChangeTweenTarget (UnitDetailPanel);

			var text = node.GetComponentInChildren<Text>();

			//rank番号とカッコを取り除く
			text.text = ownedUnitDataPair.Value.unit_name;
		}
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
		} else {
			navigationBar.RollBackTweenPanel ();
		}
	}

	private void SetNextPanelObj(){
		if(ownedUnitListMode == "PartyEdit"){
			nextPanelObj = this.gameObject;
		} else if(ownedUnitListMode == "Detail"){
			nextPanelObj = UnitDetailPanel;
		}
	}
}
