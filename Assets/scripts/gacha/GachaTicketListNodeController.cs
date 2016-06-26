using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GachaTicketListNodeController : MonoBehaviour {
	[SerializeField]
	uTools.uPlayTween playTween;
	[SerializeField]
	Text text;

	public GameObject selectGachaItemPanel;
	public GachaPanelSelectItemController gachaPanelSelectItemController;

	private int langageLimitNum;

	void Start(){
		playTween = GetComponent<uTools.uPlayTween> ();
	}

	public void SetNodeParam(string ticketName, int langageNumInt){
		langageLimitNum = langageNumInt;
		text.text = ticketName;
	}

	public void OnClickUnitListNode(){
		gachaPanelSelectItemController.GotoGachaScene(langageLimitNum);
	}
}
