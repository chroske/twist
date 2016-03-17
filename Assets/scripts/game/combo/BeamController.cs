using UnityEngine;
using System.Collections;

public class BeamController : MonoBehaviour {

	private bool startSendDamageFlag;
	public float delayDamageTime;
	private int comboAttack;
	private bool triggerEnterFlag;
	public float destroyTime;


	// Use this for initialization
	void Start () {
		startSendDamageFlag = true;
		triggerEnterFlag = true;
		delayDamageTime = 0.1f;
		destroyTime = 1f;

		Destroy(this.gameObject,destroyTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//コンボダメージ値をセット
	public void SetParameter(int comboAttackParam){
		comboAttack = comboAttackParam;
	}

	//triggerにenterした段階でダメージ判定を開始する
	void OnTriggerEnter2D(Collider2D collider2d){
		if(triggerEnterFlag == true){
			if (collider2d.gameObject.tag == "enemy") {
				triggerEnterFlag = false;
				startSendDamageFlag = true;

				//次のダメージ発生を待たせる
				StartCoroutine (SendDamageLoop(collider2d));
			}
		}
	}

	//triggerがexitした段階でダメージ判定ループを抜ける
	void OnTriggerExit2D(Collider2D collider2d){
		if (collider2d.gameObject.tag == "enemy") {
			startSendDamageFlag = false;
			triggerEnterFlag = true;
		}
	}

	//ダメージ判定用ループ
	IEnumerator SendDamageLoop(Collider2D collider2d){
		while (startSendDamageFlag) {
			if (collider2d == null) {
				yield break;
			} else {
				//comboAttack分ダメージを与える
				collider2d.gameObject.SendMessage ("Damage", comboAttack);
				yield return new WaitForSeconds (delayDamageTime);
			}
		}
	}
}
