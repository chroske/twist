using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UnitListNodeController : MonoBehaviour {

	public int unitId;
	public OwnedUnitListController ownedUnitListController;
	public GameObject UnitDetailPanel;

	public void OnClickUnitListNode(){
		if (ownedUnitListController.fromPanelName == "PartyEdit") {

		} else if (ownedUnitListController.fromPanelName == "Top") {
			ownedUnitListController.GotoUnitDetailPanel(unitId);
			UnitDetailPanel.SetActive (true);
		}
//		ownedUnitListController.GotoUnitDetailPanel(unitId);
//		UnitDetailPanel.SetActive (true);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
