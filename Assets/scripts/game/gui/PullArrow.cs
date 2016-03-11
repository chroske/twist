using UnityEngine;
using System.Collections;


public class PullArrow : MonoBehaviour {
	public float steerAngle { get; private set; }
	public float arrowDistance { get; private set; }

	public float maxDistance;
	public float minimumDistance;

	public float dx;
	public float dy;

	public float powor;

	private float mouseDownPositionX;
	private float mouseDownPositionY;

	private float arrowUiDistance;

	public GameObject myUnit;

	private RectTransform rectTrans;




	// Use this for initialization
	void Start () {
		
	}

	// Update is called once per frame
	void Update () {

		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began || Input.GetMouseButtonDown (0)) {
			mouseDownPositionX = Input.mousePosition.x;
			mouseDownPositionY = Input.mousePosition.y;

			rectTrans.position = new Vector3(myUnit.transform.position.x, myUnit.transform.position.y, 0);
		}

		if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Moved) || Input.GetMouseButton (0)) {
			
			dx = Input.mousePosition.x - mouseDownPositionX;
			dy = Input.mousePosition.y - mouseDownPositionY;

			float rad = Mathf.Atan2 (dx, -dy);
			steerAngle = rad * Mathf.Rad2Deg;

			arrowDistance = Vector2.Distance (new Vector2 (dx, dy), new Vector2 (0, 0));

		} else if((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) || Input.GetMouseButtonUp (0)) {
			
			Rigidbody2D myUnitRigidbody2D = myUnit.GetComponent<Rigidbody2D> ();
			myUnitRigidbody2D.AddForce(new Vector2 (-Mathf.Clamp (dx,-maxDistance,maxDistance), -Mathf.Clamp (dy,-maxDistance,maxDistance)) * powor);

			arrowDistance = 0;
			arrowUiDistance = 0;

		}
			
		if(arrowDistance > minimumDistance){
			arrowUiDistance = arrowDistance;
		}

		rectTrans = GetComponent <RectTransform>();
		rectTrans.sizeDelta = new Vector2 (200, Mathf.Clamp (arrowUiDistance, 0, maxDistance) * 5);

		transform.eulerAngles = new Vector3(0, 0, steerAngle);
	}
}
