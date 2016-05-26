using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PartyUnitListController : MonoBehaviour {

	[SerializeField]
	RectTransform scrollViewNode;

	[SerializeField]
	GameObject gameStateManagerObj;

	[SerializeField]
	GameObject content;

	[SerializeField]
	NavigationBarController navigationBar;

	public GameObject unitPanelList;
	public GameObject fromPanel;

	// Use this for initialization
	void Start () {
		GameStateManager gameStateManager = gameStateManagerObj.GetComponent<GameStateManager> ();
		List<OwnedUnitData> partyUnitList = gameStateManager.partyUnitList1;
		GenPartyUnitList (partyUnitList);
	}

	// Update is called once per frame
	void Update () {
	}

	void GenPartyUnitList(List<OwnedUnitData> partyUnitList){
		//リストをクリア
		RemoveAllListViewItem ();

		foreach(OwnedUnitData ownedUnitData in partyUnitList){
			var node = GameObject.Instantiate(scrollViewNode) as RectTransform;
			node.SetParent(content.transform, false);
			PartyUnitListNodeController partyUnitListNodeController = node.GetComponent<PartyUnitListNodeController> ();
			partyUnitListNodeController.partyUnitListController = this;

			uTools.uPlayTween uPlayTween = node.GetComponent<uTools.uPlayTween> ();
			uPlayTween.tweenTarget = unitPanelList;

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
