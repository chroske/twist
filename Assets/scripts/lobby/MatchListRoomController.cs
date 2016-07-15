using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

public class MatchListRoomController : MonoBehaviour {
	public int listId;

	[SerializeField]
	Text roomNameText;
	[SerializeField]
	Text roomPlayerNumText;
	[SerializeField]
	Image roomIcon;

	private GameObject _parent;

	public void SetNodeDatas(int id, string roomName, int maxSize, int currentSize){
		listId = id;

		//rank番号とカッコを取り除く
		roomNameText.text = Regex.Replace(roomName, "{.+?}", "");
		//ルーム人数
		roomPlayerNumText.text = currentSize + "/" + maxSize;
	}

	public void OnClickButton(){
		_parent = transform.parent.gameObject;
		_parent.GetComponent<GenMatchListController> ().OnJoinMatchButton(listId);
	}
}
