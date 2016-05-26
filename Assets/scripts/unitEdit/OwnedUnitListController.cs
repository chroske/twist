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

	public string fromPanelName;
	public GameObject fromPanel;

	// Use this for initialization
	void Start () {
		GameStateManager gameStateManager = gameStateManagerObj.GetComponent<GameStateManager> ();
		List<OwnedUnitData> ownedUnitList = gameStateManager.ownedUnitList;
		GenOwnedUnitList (ownedUnitList);
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void GotoUnitDetailPanel(int unitId){
		unitDetailController.unitId = unitId;
	}

	void GenOwnedUnitList(List<OwnedUnitData> ownedUnitList){
		//リストをクリア
		RemoveAllListViewItem ();

		foreach(OwnedUnitData ownedUnitData in ownedUnitList){
			var node = GameObject.Instantiate(scrollViewNode) as RectTransform;
			node.SetParent(content.transform, false);
			UnitListNodeController unitListNodeController = node.GetComponent<UnitListNodeController> ();
			unitListNodeController.unitId = ownedUnitData.unit_id;
			unitListNodeController.UnitDetailPanel = UnitDetailPanel;
			unitListNodeController.ownedUnitListController = this;

			unitListNodeController.GetComponent<uTools.uPlayTween> ().ChangeTweenTarget (UnitDetailPanel);

			var text = node.GetComponentInChildren<Text>();

			//rank番号とカッコを取り除く
			text.text = ownedUnitData.unit_name;
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
		} else {
			navigationBar.RollBackTweenPanel ();
		}
	}
}
