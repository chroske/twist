using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TimeLineNodeController : MonoBehaviour {

	[SerializeField]
	private Text tweetTextPrefab;
	[SerializeField]
	private Text screenNameText;
	[SerializeField]
	private Text nameText;
	[SerializeField]
	private Image icon;
	[SerializeField]
	private GameObject tweetTextUrlButtonPrefab;
	[SerializeField]
	private GameObject TweetTextBox;


	public void SetNodeDatas(string tweet, string name, string screenName, string profileImageUrl){
		string tweetTextStr = tweet;
		nameText.text = name;
		screenNameText.text = screenName;


		MatchCollection matchCollection = Regex.Matches (tweet, @"(https?://[-_.!~*\'()a-zA-Z0-9;/?:@&=+$,%#]+)");
		string[] matchUrl = new string[matchCollection.Count];
		int i = 0;
		foreach (Match urlStr in matchCollection) {
			matchUrl[i] = urlStr.Value;
			i++;
		}

		int j = 0;
		foreach(string splitTweet in tweet.Split(matchUrl,System.StringSplitOptions.None) ){
			if(splitTweet != ""){
				Text tweetText = GameObject.Instantiate(tweetTextPrefab) as Text;
				tweetText.transform.SetParent(TweetTextBox.transform, false);
				tweetText.text = splitTweet;
			}
			if(matchUrl.Length > j && matchUrl[j] != ""){
				GameObject tweetTextUrlButton = GameObject.Instantiate(tweetTextUrlButtonPrefab) as GameObject;
				tweetTextUrlButton.transform.SetParent(TweetTextBox.transform, false);
				Text tweetTextUrlButtonText = tweetTextUrlButton.GetComponent<Text>();

				tweetTextUrlButtonText.text = matchUrl [j];
				tweetTextUrlButtonText.color = Color.blue;

				tweetTextUrlButton.GetComponent<Button> ().onClick.AddListener (() => onClickUrlButton(tweetTextUrlButtonText.text));
			}
			j++;
		}
	}

	public void SetNodeDatasRT(string rtText, string rtName, string rtScreenName, string rtProfileImageUrl, string name, string screenName){
		string tweetTextStr = rtText;
		nameText.text = rtName;
		screenNameText.text = rtScreenName;

		MatchCollection matchCollection = Regex.Matches (rtText, @"(https?://[-_.!~*\'()a-zA-Z0-9;/?:@&=+$,%#]+)");
		string[] matchUrl = new string[matchCollection.Count];
		int i = 0;
		foreach (Match urlStr in matchCollection) {
			matchUrl[i] = urlStr.Value;
			i++;
		}

		int j = 0;
		foreach(string splitTweet in rtText.Split(matchUrl,System.StringSplitOptions.None) ){
			if(splitTweet != ""){
				Text tweetText = GameObject.Instantiate(tweetTextPrefab) as Text;
				tweetText.transform.SetParent(TweetTextBox.transform, false);
				tweetText.text = splitTweet;
			}
			if(matchUrl.Length > j && matchUrl[j] != ""){
				GameObject tweetTextUrlButton = GameObject.Instantiate(tweetTextUrlButtonPrefab) as GameObject;
				tweetTextUrlButton.transform.SetParent(TweetTextBox.transform, false);
				Text tweetTextUrlButtonText = tweetTextUrlButton.GetComponent<Text>();

				tweetTextUrlButtonText.text = matchUrl [j];
				tweetTextUrlButtonText.color = Color.blue;

				tweetTextUrlButton.GetComponent<Button> ().onClick.AddListener (() => onClickUrlButton(tweetTextUrlButtonText.text));
			}
			j++;
		}
	}

	public void onClickUrlButton(string url){
		Application.OpenURL(url);
	}
}
