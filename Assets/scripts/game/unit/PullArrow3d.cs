using UnityEngine;
using System;
using System.Collections;

public class PullArrow3d : MonoBehaviour {
	[SerializeField]
	GameObject gameSceneManager;
	[SerializeField]
	float power;
	[SerializeField]
	float maxDistance;
	[SerializeField]
	float minimumDistance;

	public float steerAngle { get; private set; }
	public float arrowDistance { get; private set; }
	public bool shotFlag;
	public bool tapFlag;
	public float dx;
	public float dy;
	public GameObject myUnit;
	public Vector3 shotVector;
	//矢印が長すぎると感じたらここをいじってみよう
	public float arrowUiDistanceRate = 0.3f;

	private float mouseDownPositionX;
	private float mouseDownPositionY;
	public float arrowUiDistance;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (gameSceneManager.GetComponent<GameSceneManager> ().myTurnFlag && shotFlag == false) {
			//タップ
			if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began || Input.GetMouseButtonDown (0)) {
				tapFlag = true;
				shotFlag = false;

				mouseDownPositionX = Input.mousePosition.x;
				mouseDownPositionY = Input.mousePosition.y;
				transform.position = new Vector3(myUnit.transform.position.x, myUnit.transform.position.y, myUnit.transform.position.z);
				//rectTrans.position = new Vector3(myUnit.transform.position.x, myUnit.transform.position.y, 0);
			}
			//ドラッグ
			if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved) || Input.GetMouseButton (0)) {
				tapFlag = true;
				shotFlag = false;

				dx = Input.mousePosition.x - mouseDownPositionX;
				dy = Input.mousePosition.y - mouseDownPositionY;

				float rad = Mathf.Atan2 (dx, -dy);
				steerAngle = rad * Mathf.Rad2Deg;

				//rad(角度)から発射用ベクトルを作成
				double addforceX = Math.Sin(rad) * power;
				double addforceY = Math.Cos(rad) * power;
				shotVector = new Vector3(-(float)addforceX,0 ,(float)addforceY);

				//矢印の長さ
				arrowDistance = Vector2.Distance (new Vector2 (dx, -dy), new Vector2 (0, 0));
				//rectTrans.position = new Vector3(myUnit.transform.position.x, myUnit.transform.position.y, 0);

				//アンタップ
			} else if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp (0)) {
				//矢印の長さがminimumに達していなければ飛ばさない
				if(arrowDistance > minimumDistance){
					tapFlag = false;
					shotFlag = true;

					Rigidbody myUnitRigidbody = myUnit.GetComponent<Rigidbody> ();
					myUnitRigidbody.AddForce(shotVector);

					arrowDistance = 0;
					arrowUiDistance = 0;
				}
			}


			//rectTrans.sizeDelta = new Vector2 (200, Mathf.Clamp (arrowUiDistance, 0, maxDistance) * arrowUiDistanceRate);
			if(arrowUiDistance == 0){
				transform.localScale = new Vector3 (0, 0, 0);
			} else {
				transform.localScale = new Vector3 (0.5f, 0.5f, arrowUiDistance * arrowUiDistanceRate);
			}

			transform.eulerAngles = new Vector3(0, -steerAngle, 0);
			//矢印の長さがminimumに達していなければ矢印を表示しない
			if (arrowDistance > minimumDistance) {
				arrowUiDistance = arrowDistance;
			} else {
				arrowUiDistance = 0;
			}
		}
	}
}
