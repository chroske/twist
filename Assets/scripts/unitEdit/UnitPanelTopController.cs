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

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

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
		ownedUnitListController.fromPanelName = "Top";
		ownedUnitListController.fromPanel = this.gameObject;
		UnitListPanel.SetActive (true);
	}

	public void OnClickUnitRemovePanelNode(){
		//UnitRemovePanel.SetActive (true);
	}
}
