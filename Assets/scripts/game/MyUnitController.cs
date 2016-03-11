using UnityEngine;
using System.Collections;

public class MyUnitController : MonoBehaviour {
	//unit game parameter
	public int attack;
	public int hitPoint;
	public float speed;
	public int type;

	public int Level;

	public int combo;
	public int ability_1;
	public int ability_2;
	public int ability_3;
	public int strikeShot;

	public Vector2 speedTest;


	//particle
	public GameObject enemyHitEffect;
	public GameObject wallHitEffect;

	// Use this for initialization
	void Start () {
	
	}

	void FixedUpdate() {

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
		} else if(collision2d.gameObject.tag == "enemy") {
			//effect発生
			GameObject effect = Instantiate(enemyHitEffect ,
				collision2d.contacts[0].point,
				Quaternion.identity
			) as GameObject;	

			Destroy(effect , 0.2f);

			//attack分ダメージを与える
			collision2d.gameObject.SendMessage("Damage",2);
		}
	}
}
