using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TwitterTimeLineData
{
	public string created_at;
	public string text;
	public string profile_image_url;
	public User user;

	[System.Serializable]
	public class User {
		public int id;
		public string name;
		public string screen_name;
	}
}

