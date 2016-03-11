using UnityEngine;
using System.Collections;

public class MyPartyUnitController : MonoBehaviour {

	//particle
	public GameObject wallHitEffect;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D collision2d) {
		if (collision2d.gameObject.tag == "wall") {
			GameObject effect = Instantiate(wallHitEffect ,
				collision2d.contacts[0].point,
				Quaternion.identity
			) as GameObject;

			Destroy(effect , 0.2f);
		} else if(collision2d.gameObject.tag == "myUnit") {
			//transform
			//effect発生
//			GameObject effect = Instantiate(enemyHitEffect ,
//				collision2d.contacts[0].point,
//				Quaternion.identity
//			) as GameObject;	
//
//			Destroy(effect , 0.2f);
//
//			//attack分ダメージを与える
//			collision2d.gameObject.SendMessage("Damage",2);
		}
	}
}
