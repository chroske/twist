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

	public GameObject detailPanel;
	public string fromPanelName;

	// Use this for initialization
	void Start () {
		GameStateManager gameStateManager = gameStateManagerObj.GetComponent<GameStateManager> ();
		List<OwnedUnitData> ownedUnitList = gameStateManager.ownedUnitList;
		GenOwnedUnitList (ownedUnitList);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void GenOwnedUnitList(List<OwnedUnitData> ownedUnitList){
		//リストをクリア
		RemoveAllListViewItem ();

		foreach(OwnedUnitData ownedUnitData in ownedUnitList){
			var node = GameObject.Instantiate(scrollViewNode) as RectTransform;
			node.SetParent(content.transform, false);
			UnitListNodeController unitListNodeController = node.GetComponent<UnitListNodeController> ();
			unitListNodeController.unitId = ownedUnitData.unit_id;
			unitListNodeController.ownedUnitListController = this;

			uTools.uPlayTween uPlayTween = node.GetComponent<uTools.uPlayTween> ();
			uPlayTween.tweenTarget = detailPanel;

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
}
