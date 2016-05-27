using UnityEngine;
using System.Collections;

public class PartyUnitListNodeController : MonoBehaviour {

	public int unitId;
	public int partyId;

	public GameObject ownedUnitPanelList;

	public void OnClickUnitListNode(){
		ownedUnitPanelList.SetActive (true);
		OwnedUnitListController ownedUnitListController = ownedUnitPanelList.GetComponent<OwnedUnitListController> ();
		ownedUnitListController.ownedUnitListMode = "PartyEdit";
		ownedUnitListController.selectPartyChangeUnitId = unitId;
	}
}
