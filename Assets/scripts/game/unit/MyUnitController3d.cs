using UnityEngine;
using System.Collections;
using System.IO;

public class MyUnitController3d : MonoBehaviour {
	public int unitNum;

	[SerializeField]
	private SpriteRenderer unitIconSprite;
	[SerializeField]
	private SpriteRenderer mask;

	//particle
	public GameObject enemyHitEffect;
	public GameObject wallHitEffect;

	//combo
	public GameObject explosion;
	public GameObject beam;
	public GameObject beamType2;

	public GameObject UnitParamManager;
	private UnitParamManager unitParamManager;

	//減速を始める速度(velocity.magnitude)
	private float decelerateLimitSpeed = 1.5f;
	private float decelerateSpeedRate = 0.90f;

	private float effectDestroyTime = 0.8f;

	private int comboNum;
	private bool startEffectFlag;
	private GameObject comboEffect;

	void Awake(){
		unitParamManager = UnitParamManager.GetComponent<UnitParamManager>();
//		mask.material.SetInt ("_Stencil", unitNum);
//		unitIconSprite.material.SetInt ("_Stencil", unitNum);
	}

	void Start () {
		comboNum = 1;
		startEffectFlag = true;
	}

	void FixedUpdate() {
//		Rigidbody2D rigidbody2d = transform.GetComponent<Rigidbody2D> ();
//
//		//遅くなってきたらvelocityをさげて停止させる
//		if(rigidbody2d.velocity.magnitude < decelerateLimitSpeed && rigidbody2d.velocity.magnitude > 0f){
//			rigidbody2d.velocity = rigidbody2d.velocity * decelerateSpeedRate;
//		}
	}

	void OnTriggerEnter2D(Collider2D collider2d){
		if(this.tag == "controllUnit"){
			//爆発コンボに当たったらコンボ発動
			if(collider2d.gameObject.tag == "explosion" && unitParamManager.maxComboNum >= comboNum){
				//コンボエフェクト
				if(startEffectFlag){
					GameObject effect = Instantiate(comboEffect ,
						transform.position,
						Quaternion.identity
					) as GameObject;

					//エフェクトと判定をユニットに追従させるため子にする
					effect.transform.parent = transform;

					//コンボのダメージ値をセット
					effect.transform.SendMessage ("SetParameter", unitParamManager.comboAttack);

					//コンボ２回
					comboNum++;
					startEffectFlag = false;

					//エフェクトが消えるまで次のコンボ発生を待たせる
					StartCoroutine (DelayNextCombo());
				}
			}
		}
	}


	void OnCollisionEnter2D(Collision2D collision2d) {
		if (this.tag == "controllUnit") {
			if (collision2d.gameObject.tag == "wall") {
				GameObject effect = Instantiate (wallHitEffect,
					collision2d.contacts [0].point,
					Quaternion.identity
				) as GameObject;

				Destroy (effect, 0.2f);
			} else if (collision2d.gameObject.tag == "enemy") {
				//effect発生
				GameObject effect = Instantiate (enemyHitEffect,
					collision2d.contacts [0].point,
					Quaternion.identity
				) as GameObject;	

				Destroy (effect, 0.2f);

				//attack分ダメージを与える
				collision2d.gameObject.SendMessage ("Damage", unitParamManager.attack);
			}
		}
	}

	IEnumerator DelayNextCombo()
	{
		yield return new WaitForSeconds(effectDestroyTime);
		startEffectFlag = true;
	}

	//初期値1に戻す
	public void ResetComboNum(){
		comboNum = 1;
	}

	public void SetUnitIcon(){
		StartCoroutine(SetLeaderUnitIcon(unitParamManager.unitIconUrl));
	}

	public void SetComboEffect(){
		//コンボタイプをセット
		if (unitParamManager.comboType == 0) {
			//ビーム
			comboEffect = beam;
		} else if (unitParamManager.comboType == 1) {
			//爆発
			comboEffect = explosion;
		} else if (unitParamManager.comboType == 2) {
			//上下ビーム
			comboEffect = beamType2;
		} else {
			comboEffect = null;
		}
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

			if (www.error == null) {
				File.WriteAllBytes (path, www.bytes);
				texture = www.texture;

				unitIconSprite.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f), 11.0f);
			}
		} else {
			byte[] imageBytes = File.ReadAllBytes(path);
			Texture2D tex2D = new Texture2D(48, 48, TextureFormat.RGB24, false);
			bool isloadbmpSuccess =  tex2D.LoadImage(imageBytes);

			if( isloadbmpSuccess )
			{
				texture = tex2D;
				unitIconSprite.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f), 11.0f);
			} else {
				Debug.Log ("load bmp failed");
			}
		}
	}
}