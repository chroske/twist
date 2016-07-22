using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class CommandPanelController : MonoBehaviour {
	[SerializeField]
	RawImage iconImage;
	[SerializeField]
	DuelGameSceneManager gameSceneManager;

	public int commandPanelNum;

	public void SetCommandPanelParam(OwnedUnitData unitParam){
		StartCoroutine(SetCommandPanelIcon (unitParam.unit_icon_url));
	}

	IEnumerator SetCommandPanelIcon (string url) {
		Texture2D texture;

		string[] splitedUrl = url.Split('/');
		string imageFileName = splitedUrl [splitedUrl.Length - 1];
		string path = string.Format("{0}/{1}", Application.temporaryCachePath , imageFileName);
		if (!File.Exists (path)) {
			WWW www = new WWW(url);
			yield return www;

			if (www.error == null) {
				File.WriteAllBytes (path, www.bytes);

				iconImage.texture = www.texture;
			}
		} else {
			byte[] imageBytes = File.ReadAllBytes(path);
			Texture2D tex2D = new Texture2D(48, 48, TextureFormat.RGB24, false);
			bool isloadbmpSuccess =  tex2D.LoadImage(imageBytes);

			if( isloadbmpSuccess )
			{
				texture = tex2D;
				iconImage.texture = texture;
			} else {
				Debug.Log ("load bmp failed");
			}
		}
	}

	public void OnClickCommandPanelButton(){
		gameSceneManager.ChangeControllUnit (commandPanelNum);
	}
}
