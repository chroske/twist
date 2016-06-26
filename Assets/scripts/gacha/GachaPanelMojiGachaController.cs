using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;
using UnityEngine.SceneManagement;
using System.IO;

public class GachaPanelMojiGachaController : MonoBehaviour {
	[SerializeField]
	Text TextFieldInput;
	[SerializeField]
	InputField MojigachaInputField;
	[SerializeField]
	GameObject ResultPanel;
	[SerializeField]
	RawImage ResultPanelIconImage;
	[SerializeField]
	Text ResultPanelUnitNameText;

	private GameStateManager gameStateManager;
	private GameObject mainCanvasObj;

	void Start(){
		gameStateManager = GameObject.Find ("/GameStateManager").GetComponent<GameStateManager>();
		mainCanvasObj = GameObject.Find ("/MainCanvas");
		mainCanvasObj.SetActive (false);

		//文字数制限をセット
		SetCharacterLimit (gameStateManager.gachaLangageLimite);
	}

	public void OnClickGachaButton(){
		Debug.Log (TextFieldInput.text.ToString());
		int getTweetCount = 10;
		StartCoroutine (Twitter.API.GetSearchTweet (gameStateManager.CONSUMER_KEY, gameStateManager.CONSUMER_SECRET, gameStateManager.m_AccessTokenResponse, TextFieldInput.text.ToString(), getTweetCount.ToString(), new Twitter.SearchTweetCallback(this.OnGetSearchTweetCallback)));
	}

	void OnGetSearchTweetCallback(bool success, Twitter.SearchTweetResponse response)
	{
		if (success)
		{
			GenerateUnitFromSearchTweet (response.Json);
			//Debug.Log (response.Json);
		}
		else
		{
			print("OnGetSearchTweetCallback - failed.");
		}
	}

	void GenerateUnitFromSearchTweet(string json){
		IDictionary TweetListStatus = (IDictionary)Json.Deserialize(json);
		IList tweetDataList = (IList)TweetListStatus["statuses"];

		//乱数生成
		int gachaNum = Random.Range (0, tweetDataList.Count);
		Debug.Log(gachaNum);
		IDictionary tweetData = (IDictionary)tweetDataList[gachaNum];

		IDictionary userDatas = (IDictionary)tweetData["user"];
		OwnedUnitData unitData = GenerateUnitParameter(userDatas);

		IndicateResultPanel(unitData, userDatas["profile_image_url"].ToString());

		//所持ユニットにセット
		gameStateManager.ownedUnitDic.Add(unitData.unit_id, unitData);
	}

	private void IndicateResultPanel(OwnedUnitData unitData, string profileImageUrl){
		ResultPanel.SetActive (true);
		ResultPanelUnitNameText.text = unitData.unit_name;

		StartCoroutine(SetResultIcon(profileImageUrl, unitData.unit_account_id));
	}

	IEnumerator SetResultIcon (string url, string accountId) {
		string[] splitedUrl = url.Split('/');
		string imageFileName = splitedUrl [splitedUrl.Length - 1];
		string path = string.Format("{0}/{1}", Application.persistentDataPath , imageFileName);

		WWW texturewww = new WWW (url);
		yield return texturewww;

		if (texturewww.error == null) {
			File.WriteAllBytes (path, texturewww.bytes);
			ResultPanelIconImage.texture = texturewww.texture;
		}
	}


	private OwnedUnitData GenerateUnitParameter(IDictionary userDatas){
		Debug.Log(userDatas ["screen_name"]);

		int id = gameStateManager.ownedUnitDic.Count + 1;

		Dictionary<string, object> data = new Dictionary<string, object> () {
			{ "unit_id", id },
			{ "unit_acount_id", userDatas["screen_name"] },
			{ "unit_name", userDatas["name"] },
			{ "unit_icon_url", userDatas["profile_image_url"] },
			{ "party_id", 0},
			{ "attack", 2},
			{ "hitPoint", int.Parse(userDatas["statuses_count"].ToString())},
			{ "speed", 1.5f },
			{ "type", 1 },
			{ "Level", 1 },
			{ "combo", 1 },
			{ "ability_1", 1 },
			{ "ability_2", 2 },
			{ "ability_3", 3 },
			{ "strikeShot", 1 },
			{ "comboType", 1 },
			{ "comboAttack", 10 },
			{ "maxComboNum", 1 }
		};

		return new OwnedUnitData(data);
	}

	public void SetCharacterLimit(int limit){
		MojigachaInputField.characterLimit = limit;
	}

	public void OnClickBackButton(){
		mainCanvasObj.SetActive (true);
		SceneManager.UnloadScene ("Gacha");
	}
}
