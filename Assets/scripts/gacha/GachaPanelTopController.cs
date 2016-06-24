using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class GachaPanelTopController : MonoBehaviour {
	[SerializeField]
	GameStateManager gameStateManager;
	[SerializeField]
	GachaPanelSelectItemController GachaPanelSelectItemController;

	public void OnClickMojiGachaButton(){
		GachaPanelSelectItemController.GenOwnedUnitList (gameStateManager.gachaTicketList);
	}
}
