using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GachaPanelSelectItemController : MonoBehaviour {
	[SerializeField]
	RectTransform scrollViewNode;
	[SerializeField]
	GameStateManager gameStateManager;
	[SerializeField]
	GameObject content;
	[SerializeField]
	Image BackGroundPanel;
	[SerializeField]
	GameObject MainCanvas;
	[SerializeField]
	GameObject ScrollView;

	public void GenOwnedUnitList(List<GachaTicketData> gachaTicketList){
		//リストをクリア
		RemoveAllListViewItem ();

		foreach(GachaTicketData gachaTicket in gachaTicketList){
			var node = GameObject.Instantiate(scrollViewNode) as RectTransform;
			node.SetParent(content.transform, false);
			GachaTicketListNodeController gachaTicketListNodeController = node.GetComponent<GachaTicketListNodeController> ();
			gachaTicketListNodeController.SetNodeParam("ツイチケ("+gachaTicket.languageNum + ")", gachaTicket.languageNum);
			gachaTicketListNodeController.gachaPanelSelectItemController = this;
		}
	}

	//リストクリア
	public void RemoveAllListViewItem() {
		foreach (Transform child in content.transform) {
			GameObject.Destroy(child.gameObject);
		}
	}

	//遷移アニメーション終了時に呼び出される
	public void AnimationEnd(){
		if(BackGroundPanel.color.a == 0){
			transform.gameObject.SetActive (false);
		}
	}
		
	public void GotoGachaScene(){
//		ScrollView.transform.localPosition = new Vector2 (5, -970);
//		BackGroundPanel.color = new Color(0, 0, 0, 0);
		this.gameObject.SetActive (false);

		ScrollView.GetComponent<uTools.uTweenPosition> ().ResetToBeginning ();
		BackGroundPanel.GetComponent<uTools.uTweenColor>().ResetToBeginning ();

		//ScrollView.SetActive (false);
		//BackGroundPanel.gameObject.SetActive (false);

		SceneManager.LoadScene("Gacha", LoadSceneMode.Additive);
		//SceneManager.LoadScene("Gacha");
	}
}
