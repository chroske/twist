using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour {

	private int comboAttack;
	public float destroyTime;

	// Use this for initialization
	void Start () {
		destroyTime = 0.7f;
		Destroy(this.gameObject,destroyTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//コンボダメージ値をセット
	public void SetParameter(int comboAttackParam){
		comboAttack = comboAttackParam;
	}

	void OnTriggerEnter2D(Collider2D collider2d){
		if (collider2d.gameObject.tag == "enemy") {
			collider2d.gameObject.SendMessage ("Damage", comboAttack);
		}
	}
}
