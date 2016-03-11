using UnityEngine;
using System.Collections;

public class MyUnitAinmationController : MonoBehaviour {

	public GameObject pullArrow;
	private Animator animator;
	public float pullDistance;

	//animatorのconditionsのpullDistanceの値参照
	private float animationTrigerPullDistance = 300;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		pullDistance = pullArrow.GetComponent<RectTransform> ().sizeDelta.y;

		animator.SetFloat ("pullDistance", pullArrow.GetComponent<RectTransform> ().sizeDelta.y);
		if (pullDistance >= animationTrigerPullDistance) {
			animator.speed = pullDistance / 50;
		} else {
			animator.speed = 1;
		}
	}
}
