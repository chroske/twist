using UnityEngine;
using System.Collections;

public class GameSceneManager : MonoBehaviour {

	public GameObject Unit_1;
	public GameObject Unit_2;
	public GameObject arrow;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void ChangeControllUnit(){
		PullArrow pullArrow = arrow.transform.GetComponent<PullArrow>();

		if(Unit_1.tag == "controllUnit" && Unit_2.tag == "partyUnit" ){
			//タグを切り替え
			Unit_1.tag = "partyUnit";
			foreach (Transform child in Unit_1.transform){
				child.tag = "partyUnit";
			}

			Unit_2.tag = "controllUnit";
			foreach (Transform child in Unit_2.transform){
				child.tag = "controllUnit";
			}

			//スクリプトをenabled&disabled
			Unit_1.transform.GetComponent<MyPartyUnitController>().enabled =true;
			Unit_1.transform.GetComponent<MyUnitController>().enabled = false;
			Unit_2.transform.GetComponent<MyPartyUnitController>().enabled =false;
			Unit_2.transform.GetComponent<MyUnitController>().enabled = true;

			//arrowにオブジェクトをアタッチ
			pullArrow.myUnit = Unit_2;
		} else if(Unit_2.tag == "controllUnit" && Unit_1.tag == "partyUnit"){
			Unit_1.tag = "controllUnit";
			foreach (Transform child in Unit_1.transform){
				child.tag = "controllUnit";
			}

			Unit_2.tag = "partyUnit";
			foreach (Transform child in Unit_2.transform){
				child.tag = "partyUnit";
			}

			//スクリプトをenabled&disabled
			Unit_1.transform.GetComponent<MyPartyUnitController>().enabled = false;
			Unit_1.transform.GetComponent<MyUnitController>().enabled = true;
			Unit_2.transform.GetComponent<MyPartyUnitController>().enabled = true;
			Unit_2.transform.GetComponent<MyUnitController>().enabled = false;



			pullArrow.myUnit = Unit_1;
		}
	}

	public void OnClickButton(){
		ChangeControllUnit();
	}
}
