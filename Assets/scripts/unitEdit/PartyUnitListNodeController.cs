using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

public class PartyUnitListNodeController : MonoBehaviour {
	[SerializeField]
	Image unitIcon;

	public int unitId;
	public int partyId;
	public string iconImageUrl;
	public GameObject ownedUnitListPanel;


	void Start(){
		StartCoroutine(SetUnitIcon(iconImageUrl));
	}

	public void OnClickUnitListNode(){
		//unitIdがnullじゃなければユニット入れ替え
		ownedUnitListPanel.SetActive (true);
		OwnedUnitListController ownedUnitListController = ownedUnitListPanel.GetComponent<OwnedUnitListController> ();
		ownedUnitListController.ownedUnitListMode = "PartyEdit";
		ownedUnitListController.selectPartyChangeId = partyId;

		ownedUnitListController.CheckPartyInUnit ();
	}

	IEnumerator SetUnitIcon (string url) {
		if(url != ""){
			Texture2D texture;

			string[] splitedUrl = url.Split('/');
			string imageFileName = splitedUrl [splitedUrl.Length - 1];
			string path = string.Format("{0}/{1}", Application.persistentDataPath , imageFileName);

			if (!File.Exists (path)) {
				WWW www = new WWW(url);
				yield return www;

				if( www.error == null){
					File.WriteAllBytes( path, www.bytes );
#if UNITY_IOS
					UnityEngine.iOS.Device.SetNoBackupFlag(path);//iCloudにバックアップさせない(リジェクト対策)
#endif

					texture = www.texture;
					unitIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
				}
			} else {
				byte[] imageBytes = File.ReadAllBytes(path);
				Texture2D tex2D = new Texture2D(48, 48, TextureFormat.RGB24, false);
				bool isloadbmpSuccess =  tex2D.LoadImage(imageBytes);

				if( isloadbmpSuccess )
				{
					texture = tex2D;
					unitIcon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
				} else {
					Debug.Log ("load bmp failed");
				}
			}
		}
	}
}
