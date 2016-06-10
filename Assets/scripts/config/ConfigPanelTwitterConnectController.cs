using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConfigPanelTwitterConnectController : MonoBehaviour {

	[SerializeField]
	NavigationBarController navigationBar;
	[SerializeField]
	Text TextFieldInput;
	[SerializeField]
	GameStateManager gameStateManager;

	const string PLAYER_PREFS_TWITTER_USER_ID           = "TwitterUserID";
	const string PLAYER_PREFS_TWITTER_USER_SCREEN_NAME  = "TwitterUserScreenName";
	const string PLAYER_PREFS_TWITTER_USER_TOKEN        = "TwitterUserToken";
	const string PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET = "TwitterUserTokenSecret";


	public void OnClickTwitterConnectButton(){
		StartCoroutine(Twitter.API.GetRequestToken(gameStateManager.CONSUMER_KEY, gameStateManager.CONSUMER_SECRET,
		new Twitter.RequestTokenCallback(this.OnRequestTokenCallback)));
	}

	public void OnClickSendPinButton(){
		StartCoroutine(Twitter.API.GetAccessToken(gameStateManager.CONSUMER_KEY, gameStateManager.CONSUMER_SECRET, gameStateManager.m_RequestTokenResponse.Token, TextFieldInput.text.ToString(),
			new Twitter.AccessTokenCallback(this.OnAccessTokenCallback)));
	}

	public void OnClickTestTweetButton(){
		string m_Tweet = "test_ha_pants";
		StartCoroutine(Twitter.API.PostTweet(m_Tweet, gameStateManager.CONSUMER_KEY, gameStateManager.CONSUMER_SECRET, gameStateManager.m_AccessTokenResponse,
			new Twitter.PostTweetCallback(this.OnPostTweet)));
	}

	void OnRequestTokenCallback(bool success, Twitter.RequestTokenResponse response)
	{
		if (success)
		{
			string log = "OnRequestTokenCallback - succeeded";
			log += "\n    Token : " + response.Token;
			log += "\n    TokenSecret : " + response.TokenSecret;
			print(log);

			gameStateManager.m_RequestTokenResponse = response;

			Twitter.API.OpenAuthorizationPage(response.Token);
		}
		else
		{
			print("OnRequestTokenCallback - failed.");
		}
	}

	void OnAccessTokenCallback(bool success, Twitter.AccessTokenResponse response)
	{
		if (success)
		{
			string log = "OnAccessTokenCallback - succeeded";
			log += "\n    UserId : " + response.UserId;
			log += "\n    ScreenName : " + response.ScreenName;
			log += "\n    Token : " + response.Token;
			log += "\n    TokenSecret : " + response.TokenSecret;
			print(log);

			gameStateManager.m_AccessTokenResponse = response;

			PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_ID, response.UserId);
			PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_SCREEN_NAME, response.ScreenName);
			PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN, response.Token);
			PlayerPrefs.SetString(PLAYER_PREFS_TWITTER_USER_TOKEN_SECRET, response.TokenSecret);
		}
		else
		{
			print("OnAccessTokenCallback - failed.");
		}
	}

	void OnPostTweet(bool success)
	{
		print("OnPostTweet - " + (success ? "succedded." : "failed."));
	}

	//遷移アニメーション終了時に呼び出される
	public void AnimationEnd(){
		if (this.gameObject.GetComponent<RectTransform>().anchoredPosition.x == 0) { //戻った時
			navigationBar.ChangeTweenPanel (this.gameObject);
		} else { //画面外に移動した時
			navigationBar.RollBackTweenPanel ();
		}
	}
}
