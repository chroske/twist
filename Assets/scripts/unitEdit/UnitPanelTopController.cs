using UnityEngine;
using System.Collections;

public class UnitPanelTopController : MonoBehaviour {

	public GameObject PartyEditPanel;
	public GameObject PoworUpPanel;
	public GameObject UnitListPanel;
	public GameObject UnitRemovePanel;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClickEditPanelNode(){
		PartyEditPanel.SetActive (true);
	}

	public void OnClickPoworUpPanelNode(){
		//PoworUpPanel.SetActive (true);
	}

	public void OnClickUnitListPanelNode(){
		UnitListPanel.SetActive (true);
		UnitListPanel.GetComponent<OwnedUnitListController>().fromPanelName = "Top";
	}

	public void OnClickUnitRemovePanelNode(){
		//UnitRemovePanel.SetActive (true);
	}
}
