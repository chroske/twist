using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class NetworkManagerController : NetworkBehaviour {

	[SyncVar] private Quaternion syncPlayerRotation;
	[SyncVar] private Vector3 syncPlayerPosition;
	[SyncVar] private Vector2 syncPlayerSize;

	public GameObject arrow; 

	public bool isLocalPlayer;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		TransmitRotations();

		if(!isLocalPlayer){
			//Debug.Log (syncPlayerSize.y);
			Debug.Log (syncPlayerRotation.z);
			arrow.transform.rotation = syncPlayerRotation;
			arrow.transform.position = syncPlayerPosition;
			RectTransform rectTrans = arrow.transform.GetComponent <RectTransform>();
			rectTrans.sizeDelta = syncPlayerSize;
		}
	}

	[Command]
	void CmdProvideRotationsToServer (Quaternion playerRot, Vector3 playerPos, Vector2 playerSize)
	{
		syncPlayerRotation = playerRot;
		syncPlayerPosition = playerPos;
		syncPlayerSize = playerSize;
	}

	[Client]
	void TransmitRotations ()
	{
		if (isLocalPlayer) {
			RectTransform rectTrans = GetComponent <RectTransform>();
			CmdProvideRotationsToServer(transform.rotation, transform.position, rectTrans.sizeDelta);
		}
	}

}
