using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

//このクラスでUnitの情報を持つ
public class UnitListNodeController : MonoBehaviour {
	[SerializeField]
	uTools.uPlayTween playTween;

	public OwnedUnitListController ownedUnitListController;
	public int unitId;
	public int partyId;

	void Start(){
		playTween = GetComponent<uTools.uPlayTween> ();
	}

	public void OnClickUnitListNode(){
		playTween.ChangeTweenTarget(ownedUnitListController.nextPanelObj);
		ownedUnitListController.nextPanelObj.SetActive (true);

		if (ownedUnitListController.ownedUnitListMode == "PartyEdit") {
			//パーティパネルに情報送る(通信が必要なのでコールバックで実行かも
			ownedUnitListController.SetPartyUnit(unitId);
		} else if (ownedUnitListController.ownedUnitListMode == "Detail") {
			//ユニット詳細パネルに情報送る(情報をとってくる必要あり？
			ownedUnitListController.GotoUnitDetailPanel(unitId);
		}
		//画面遷移アニメーション開始
		playTween.Play ();
	}
}