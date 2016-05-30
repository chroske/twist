using UnityEngine;
using System.Collections;

public class UnitPanelTopController : MonoBehaviour {
	
	[SerializeField]
	private NavigationBarController navigationBar;

	public GameObject PartyEditPanel;
	public GameObject PoworUpPanel;
	public GameObject UnitListPanel;
	public GameObject UnitRemovePanel;
	public GameObject backPanelButton;

	private uTools.uTweenPosition uTweenPosition;
	public GameObject fromPanel;

	public void OnClickEditPanelNode(){
		PartyUnitListController partyUnitListController = PartyEditPanel.GetComponent<PartyUnitListController> ();
		partyUnitListController.fromPanel = this.gameObject;
		PartyEditPanel.SetActive (true);
	}

	public void OnClickPoworUpPanelNode(){
		//PoworUpPanel.SetActive (true);
	}

	public void OnClickUnitListPanelNode(){
		OwnedUnitListController ownedUnitListController = UnitListPanel.GetComponent<OwnedUnitListController> ();
		ownedUnitListController.ownedUnitListMode = "Detail";
		//パーティ用に設定されていた場合のリセット
		ownedUnitListController.CheckPartyInUnit ();
		UnitListPanel.SetActive (true);
	}

	public void OnClickUnitRemovePanelNode(){
		//UnitRemovePanel.SetActive (true);
	}
}
