using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeLineNodeController : MonoBehaviour {

	[SerializeField]
	private Text tweetText;
	[SerializeField]
	private Text screenNameText;
	[SerializeField]
	private Text nameText;
	[SerializeField]
	private Image icon;

	public void SetNodeDatas(string tweet, string name, string screenName, string profileImageUrl){
		tweetText.text = tweet;
		nameText.text = name;
		screenNameText.text = screenName;
	}
}
