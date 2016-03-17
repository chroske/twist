using UnityEngine;
using System.Collections;

public class MyPartyUnitController : MonoBehaviour {

	//combo
	public GameObject wallHitEffect;
	public GameObject explosion;
	public GameObject beam;
	public GameObject beamType2;

	private float effectDestroyTime = 0.8f;

	private int comboNum;

	private bool startEffectFlag;

	private GameObject comboEffect;

	public GameObject UnitParamManager;
	private UnitParamManager unitParamManager;

	// Use this for initialization
	void Start () {
		unitParamManager = UnitParamManager.GetComponent<UnitParamManager>();

		comboNum = 1;
		startEffectFlag = true;

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
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerStay2D(Collider2D collider2d){
		if (collider2d.gameObject.tag == "controllUnit") {
			//コントロール中ユニットから離れる
			Vector2 controllUnitPosition = collider2d.gameObject.transform.position;
			transform.GetComponent<Rigidbody2D> ().velocity = new Vector2 (controllUnitPosition.x, controllUnitPosition.y) / 5;
		}
	}

	//コンボ
	void OnTriggerEnter2D(Collider2D collider2d) {
		
		//コンボ発動
		if ((collider2d.gameObject.tag == "controllUnit" || collider2d.gameObject.tag == "explosion") && unitParamManager.maxComboNum >= comboNum) {
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

				//エフェクトを消す
				//Destroy(effect, effectDestroyTime);

				//コンボ２回
				comboNum++;
				startEffectFlag = false;

				//エフェクトが消えるまで次のコンボ発生を待たせる
				StartCoroutine (DelayNextCombo());
			}
		}
	}

	IEnumerator DelayNextCombo()
	{
		yield return new WaitForSeconds(effectDestroyTime);
		startEffectFlag = true;
	}

	void OnCollisionEnter2D(Collision2D collision2d) {
		if (collision2d.gameObject.tag == "wall") {
			GameObject effect = Instantiate(wallHitEffect ,
				collision2d.contacts[0].point,
				Quaternion.identity
			) as GameObject;

			Destroy(effect , 0.2f);
		}
	}
}
