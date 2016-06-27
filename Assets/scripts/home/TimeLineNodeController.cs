using UnityEngine;
using UnityEngine.UI;
using System.IO;
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
	private Text rtNameText;
	[SerializeField]
	private Image icon;
	[SerializeField]
	private GameObject tweetTextUrlButtonPrefab;
	[SerializeField]
	private GameObject TweetTextBox;


	public void SetNodeDatas(string tweet, string name, string screenName, string profileImageUrl){
		StartCoroutine (SetTweetIcon(profileImageUrl));

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
		StartCoroutine (SetTweetIcon(rtProfileImageUrl));

		nameText.text = rtName;
		screenNameText.text = rtScreenName;
		rtNameText.gameObject.SetActive (true);
		rtNameText.text = name+"さんのRT";

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

	IEnumerator SetTweetIcon (string url) {
		Texture2D texture;

		string[] splitedUrl = url.Split('/');
		string imageFileName = splitedUrl [splitedUrl.Length - 1];
		//string path = string.Format("{0}/{1}", Application.persistentDataPath , imageFileName);
		string path = string.Format("{0}/{1}", Application.temporaryCachePath , imageFileName);
		if (!File.Exists (path)) {
			WWW www = new WWW(url);
			yield return www;

			if( www.error == null){
				File.WriteAllBytes( path, www.bytes );
				texture = www.texture;
				icon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
			}
		} else {
			byte[] imageBytes = File.ReadAllBytes(path);
			Texture2D tex2D = new Texture2D(48, 48, TextureFormat.RGB24, false);

			bool isloadbmpSuccess =  tex2D.LoadImage(imageBytes);
			if( isloadbmpSuccess )
			{
				texture = tex2D;
				icon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
			} else {
				Debug.Log ("load bmp failed");
			}
		}
	}
}