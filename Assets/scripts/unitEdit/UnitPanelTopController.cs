using UnityEngine;
using System.Collections;

public class UnitPanelTopController : MonoBehaviour {
	
	[SerializeField]
	private NavigationBarController navigationBar;
	[SerializeField]
	private GameObject PartyEditPanel;
	[SerializeField]
	private GameObject PoworUpPanel;
	[SerializeField]
	private GameObject UnitListPanel;
	[SerializeField]
	private GameObject UnitRemovePanel;
	[SerializeField]
	private GameObject backPanelButton;

	private uTools.uTweenPosition uTweenPosition;

	public void OnClickEditPanelNode(){
		PartyUnitListController partyUnitListController = PartyEditPanel.GetComponent<PartyUnitListController> ();
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
