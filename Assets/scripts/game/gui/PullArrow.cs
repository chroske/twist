using UnityEngine;
using System;
using System.Collections;


public class PullArrow : MonoBehaviour {
	public float steerAngle { get; private set; }
	public float arrowDistance { get; private set; }

	public float maxDistance;
	public float minimumDistance;

	public float dx;
	public float dy;

	public float power;

	private float mouseDownPositionX;
	private float mouseDownPositionY;

	private float arrowUiDistance;

	//矢印が長すぎると感じたらここをいじってみよう
	public float arrowUiDistanceRate;

	public GameObject myUnit;
	public GameObject gameSceneManager;

	private RectTransform rectTrans;
	public bool shotFlag;
	public bool tapFlag;


	private Vector2 shotVector;


	// Use this for initialization
	void Start () {
		shotFlag = false;
	}

	// Update is called once per frame
	void Update () {
		if(gameSceneManager.GetComponent<GameSceneManager>().myTurnFlag){
			rectTrans = GetComponent <RectTransform>();

			//タップ
			if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began || Input.GetMouseButtonDown (0)) {
				tapFlag = true;
				shotFlag = false;
				
				mouseDownPositionX = Input.mousePosition.x;
				mouseDownPositionY = Input.mousePosition.y;

				rectTrans.position = new Vector3(myUnit.transform.position.x, myUnit.transform.position.y, 0);
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
				double addforceX = Math.Cos((90*Mathf.Deg2Rad)+rad) * power;
				double addforceY = Math.Sin((90*Mathf.Deg2Rad)+rad) * power;
				shotVector = new Vector2((float)addforceX, (float)addforceY);

				//矢印の長さ
				arrowDistance = Vector2.Distance (new Vector2 (dx, dy), new Vector2 (0, 0));

				rectTrans.position = new Vector3(myUnit.transform.position.x, myUnit.transform.position.y, 0);

			//アンタップ
			} else if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp (0)) {
				//矢印の長さがminimumに達していなければ飛ばさない
				if(arrowDistance > minimumDistance){
					tapFlag = false;
					shotFlag = true;

					Rigidbody2D myUnitRigidbody2D = myUnit.GetComponent<Rigidbody2D> ();

					myUnitRigidbody2D.AddForce(shotVector);

					arrowDistance = 0;
					arrowUiDistance = 0;
				}
			}

			//矢印をドラッグ距離にあわせて変形させる
			rectTrans.sizeDelta = new Vector2 (200, Mathf.Clamp (arrowUiDistance, 0, maxDistance) * arrowUiDistanceRate);
			transform.eulerAngles = new Vector3(0, 0, steerAngle);

			//矢印の長さがminimumに達していなければ矢印を表示しない
			if (arrowDistance > minimumDistance) {
				arrowUiDistance = arrowDistance;
			} else {
				arrowUiDistance = 0;
			}
		}
	}

	public void RemoteShot(Vector2 pullDistance){
		//floatに変換
		arrowDistance = Vector2.Distance (pullDistance, new Vector2 (0, 0));

		//発射用ベクトル作成
		float rad = Mathf.Atan2 (pullDistance.x, -pullDistance.y);
		double addforceX = Math.Cos((90*Mathf.Deg2Rad)+rad) * power;
		double addforceY = Math.Sin((90*Mathf.Deg2Rad)+rad) * power;
		shotVector = new Vector2((float)addforceX, (float)addforceY);

		if(arrowDistance > minimumDistance){
			Rigidbody2D myUnitRigidbody2D = myUnit.GetComponent<Rigidbody2D> ();
			myUnitRigidbody2D.AddForce(shotVector);

			arrowDistance = 0;
			arrowUiDistance = 0;
		}
	}
}
