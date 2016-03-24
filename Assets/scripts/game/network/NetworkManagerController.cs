using UnityEngine;
using System.Collections;
using UnityEngine.Networking;


public class NetworkManagerController : NetworkBehaviour {

	public GameObject arrow;

	public Vector3 arrowPos;
	public Quaternion arrowRot;
	public Vector2 arrowSize;
	public bool arrowShotFlag;
	public Vector2 arrowDistance;

	[SyncVar] public Vector3 syncArrowPos;
	[SyncVar] public Quaternion syncArrowRot;
	[SyncVar] public Vector2 syncArrowSize;
	[SyncVar] public bool syncArrowShotFlag;
	[SyncVar] public Vector2 syncArrowDistance;

	private PullArrow pullArrow;

	private bool remoteShotFlag;

	void Start(){
		arrow = GameObject.Find("GameCanvas/Arrow");
		pullArrow = arrow.GetComponent<PullArrow> ();
		remoteShotFlag = true;

	}

	void FixedUpdate(){
		//このスクリプトの付随するオブジェクトが別のネットワーク端末から作られたものでないことの確認
		if (isLocalPlayer) {
			TransmitArrowData ();
		}

		//自分のターン以外であれば受け手にまわる
		//どのユーザオブジェクトからもいじれる
		if (!pullArrow.MyTurn) {
			ReceveArrowData ();
		}
	}


	[Client]
	void TransmitArrowData()
	{
		if (pullArrow.tapFlag) {
			//Turnプレイヤーのタップが検出された時点からArrowのパラメータをサーバに送る
			RectTransform rectTrans = arrow.transform.GetComponent <RectTransform> ();

			arrowPos = arrow.transform.position;
			arrowRot = arrow.transform.rotation;
			arrowSize = rectTrans.sizeDelta;
			arrowShotFlag = false;
			//arrowShotFlag = pullArrow.shotFlag;
			arrowDistance = new Vector2 (pullArrow.dx, pullArrow.dy);

			//サーバにパラメータ送信
			CmdProvideArrowDataToServer (arrowPos, arrowRot, arrowSize, arrowShotFlag, arrowDistance);
		} else {
			//Turnプレイヤーのタップがはなれた時点でArrowのSizeを0にしてその時点のarrowDistanceをsyncに入れる
			if(arrowPos != Vector3.zero && arrowSize != Vector2.zero){
				arrowPos = Vector3.zero;
				arrowSize = Vector2.zero;
				arrowShotFlag = true;
				//arrowShotFlag = pullArrow.shotFlag;

				//サーバにパラメータ送信
				CmdProvideArrowDataToServer (arrowPos, arrowRot, arrowSize, arrowShotFlag, arrowDistance);
			}

		}
	}

	//クライアント側から受け取ったパラメータをサーバ側でsyncにつめる
	[Command]
	void CmdProvideArrowDataToServer (Vector3 arrowPos, Quaternion arrowRot, Vector2 arrowSize, bool arrowShotFlag, Vector2 arrowDistance){
		syncArrowPos = arrowPos;
		syncArrowRot = arrowRot;
		syncArrowSize = arrowSize;
		syncArrowShotFlag = arrowShotFlag;
		syncArrowDistance = arrowDistance;
	}
		

	void ReceveArrowData(){
		RectTransform rectTrans = arrow.transform.GetComponent <RectTransform>();

		arrow.transform.position = syncArrowPos;
		arrow.transform.rotation = syncArrowRot;
		rectTrans.sizeDelta = syncArrowSize;

		//remoteShotFlagは２回呼ばせないために設定
		if (syncArrowShotFlag) {
			if (remoteShotFlag) {
				remoteShotFlag = false;
				pullArrow.RemoteShot (syncArrowDistance);
			}
		} else {
			remoteShotFlag = true;
		}
	}
}
