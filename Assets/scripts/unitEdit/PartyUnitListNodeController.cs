using UnityEngine;
using System.Collections;

public class PartyUnitListNodeController : MonoBehaviour {

	public int unitId;
	public PartyUnitListController partyUnitListController;

	public void OnClickUnitListNode(){
		partyUnitListController.unitPanelList.SetActive (true);
	}
}
