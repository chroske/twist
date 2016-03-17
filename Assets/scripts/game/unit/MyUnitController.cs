using UnityEngine;
using System.Collections;

public class MyUnitController : MonoBehaviour {

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
	private float decelerateSpeedRate = 0.93f;

	private float effectDestroyTime = 0.8f;

	private int comboNum;
	private bool startEffectFlag;
	private GameObject comboEffect;

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

	void FixedUpdate() {
		Rigidbody2D rigidbody2d = transform.GetComponent<Rigidbody2D> ();

		//遅くなってきたらvelocityをさげて停止させる
		if(rigidbody2d.velocity.magnitude < decelerateLimitSpeed && rigidbody2d.velocity.magnitude > 0f){
			rigidbody2d.velocity = rigidbody2d.velocity * decelerateSpeedRate;
		}
	}

	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D collider2d){
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
		

	void OnCollisionEnter2D(Collision2D collision2d) {
		if (collision2d.gameObject.tag == "wall") {
			GameObject effect = Instantiate(wallHitEffect ,
				collision2d.contacts[0].point,
				Quaternion.identity
			) as GameObject;

			Destroy(effect , 0.2f);
		} else if(collision2d.gameObject.tag == "enemy") {
			//effect発生
			GameObject effect = Instantiate(enemyHitEffect ,
				collision2d.contacts[0].point,
				Quaternion.identity
			) as GameObject;	

			Destroy(effect , 0.2f);

			//attack分ダメージを与える
			collision2d.gameObject.SendMessage("Damage",unitParamManager.attack);
		}
	}

	IEnumerator DelayNextCombo()
	{
		yield return new WaitForSeconds(effectDestroyTime);
		startEffectFlag = true;
	}
}
