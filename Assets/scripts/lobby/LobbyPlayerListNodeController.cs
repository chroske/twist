using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class LobbyPlayerListNodeController : MonoBehaviour {

	[SerializeField]
	private Text userNameText;
	[SerializeField]
	private Image leaderUnitIcon;

	public void SetNodeDatas(string nodeText, string unitIconUrl){
		userNameText.text = nodeText;
		StartCoroutine (SetLeaderUnitIcon (unitIconUrl));
	}

	IEnumerator SetLeaderUnitIcon (string url) {
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
				leaderUnitIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
			}
		} else {
			byte[] imageBytes = File.ReadAllBytes(path);
			Texture2D tex2D = new Texture2D(48, 48, TextureFormat.RGB24, false);
			bool isloadbmpSuccess =  tex2D.LoadImage(imageBytes);

			if( isloadbmpSuccess )
			{
				texture = tex2D;
				leaderUnitIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
			} else {
				Debug.Log ("load bmp failed");
			}
		}
	}
}
