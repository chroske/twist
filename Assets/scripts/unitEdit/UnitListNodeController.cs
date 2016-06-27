using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.IO;

//このクラスでUnitの情報を持つ
public class UnitListNodeController : MonoBehaviour {
	[SerializeField]
	uTools.uPlayTween playTween;
	[SerializeField]
	Image unitIcon;

	public OwnedUnitListController ownedUnitListController;
	public int unitId;
	public int partyId;
	public string iconImageUrl;

	void Start(){
		playTween = GetComponent<uTools.uPlayTween> ();

		StartCoroutine (SetUnitIcon (iconImageUrl));
	}

	public void OnClickUnitListNode(){
		playTween.ChangeTweenTarget(ownedUnitListController.nextPanelObj);
		ownedUnitListController.nextPanelObj.SetActive (true);

		if (ownedUnitListController.ownedUnitListMode == "PartyEdit") {
			//パーティパネルに情報送る(通信が必要なのでコールバックで実行かも
			ownedUnitListController.SetPartyUnit(unitId);
		} else if (ownedUnitListController.ownedUnitListMode == "Detail") {
			//ユニット詳細パネルに情報送る(情報をとってくる必要あり？
			ownedUnitListController.GotoUnitDetailPanel(unitId);
		}
		//画面遷移アニメーション開始
		playTween.Play ();
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