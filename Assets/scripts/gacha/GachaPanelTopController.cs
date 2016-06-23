using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MiniJSON;

public class GachaPanelTopController : MonoBehaviour {

	[SerializeField]
	GameStateManager gameStateManager;
	[SerializeField]
	Text TextFieldInput;

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
			Debug.Log (response.Json);
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
		Debug.Log (gachaNum);
		IDictionary tweetData = (IDictionary)tweetDataList [gachaNum];

		IDictionary userDatas = (IDictionary)tweetData ["user"];
		GenerateUnitParameter (userDatas);
	}


	int id = 9;
	void GenerateUnitParameter(IDictionary userDatas){
		Debug.Log(userDatas ["screen_name"]);

		Dictionary<string, object> data = new Dictionary<string, object> () {
			{ "unit_id", id },
			{ "unit_acount_id", userDatas["name"] },
			{ "unit_name", userDatas["screen_name"] },
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
		id++;
		gameStateManager.ownedUnitDic.Add(new OwnedUnitData(data).unit_id, new OwnedUnitData(data));
	}
}
