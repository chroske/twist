using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GachaTicketData {

	public int id;
	public int languageNum;

	public GachaTicketData(Dictionary<string, object>data)
	{
		id = (int)data["id"];
		languageNum = (int)data["languageNum"];
	}
}
