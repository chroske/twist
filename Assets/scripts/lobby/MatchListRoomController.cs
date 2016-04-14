using UnityEngine;
using System.Collections;

public class MatchListRoomController : MonoBehaviour {

	public int ListId;
	private GameObject _parent;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClickButton(){
		_parent = transform.parent.gameObject;
		_parent.GetComponent<GenMatchListController> ().OnJoinMatchButton(ListId);
	}
}
