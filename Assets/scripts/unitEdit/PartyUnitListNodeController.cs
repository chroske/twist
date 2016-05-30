using UnityEngine;
using System.Collections;

public class PartyUnitListNodeController : MonoBehaviour {
	public int unitId;
	public int partyId;
	public GameObject ownedUnitListPanel;

	public void OnClickUnitListNode(){
		//unitIdがnullじゃなければユニット入れ替え
		ownedUnitListPanel.SetActive (true);
		OwnedUnitListController ownedUnitListController = ownedUnitListPanel.GetComponent<OwnedUnitListController> ();
		ownedUnitListController.ownedUnitListMode = "PartyEdit";
		ownedUnitListController.selectPartyChangeId = partyId;

		ownedUnitListController.CheckPartyInUnit ();
	}
}
