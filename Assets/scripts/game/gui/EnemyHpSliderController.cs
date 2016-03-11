using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyHpSliderController : MonoBehaviour {

	public GameObject enemy1;
	private float enemyMaxHp;
	private float enemyHp;

	Slider slider;

	// Use this for initialization
	void Start () {
		enemyMaxHp = enemy1.GetComponent<EnemyController> ().hitPoint;
		slider = transform.GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update () {
		if (enemy1 != null) {
			enemyHp = enemy1.GetComponent<EnemyController> ().hitPoint;
			slider.value = enemyHp / enemyMaxHp;
		} else {
			slider.value = 0;
		}
	}
}
