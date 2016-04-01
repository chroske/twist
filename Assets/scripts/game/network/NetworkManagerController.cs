using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;


public class NetworkManagerController : NetworkBehaviour {
	[SyncVar] public Vector3 syncArrowPos;
	[SyncVar] public Quaternion syncArrowRot;
	[SyncVar] public Vector2 syncArrowSize;
	[SyncVar] public bool syncArrowShotFlag;
	[SyncVar] public Vector2 syncArrowDistance;
	[SyncVar] public Vector3 syncUnitPos;
	[SyncVar] public int syncTurnPlayerId;
	[SyncVar] public bool syncTurnEnd;
	//[SyncVar] private int playerUniqueIdentity;

	//ゲームオブジェクトとコンポーネント
	public GameObject arrow;
	private PullArrow pullArrow;
	private GameSceneManager gameSceneManager;

	//Arrow関連データ
	public Vector3 arrowPos;
	public Quaternion arrowRot;
	public Vector2 arrowSize;
	public bool arrowShotFlag;
	public Vector2 arrowDistance;

	private int turnPlayerId;
	private bool remoteShotFlag;
	private NetworkInstanceId playerNetID;
	private Transform myTransform;
	public int playerNetIdInt;
	public bool startUnitStopCheckFlag;


	private bool turnChangeFlag;

	void Awake () {
		//自分の名前を取得する時に使う
		myTransform = transform;
	}


	void Start(){
		arrow = GameObject.Find("GameCanvas/Arrow");
		pullArrow = arrow.GetComponent<PullArrow> ();
		gameSceneManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
		remoteShotFlag = true;
		startUnitStopCheckFlag = false;

		SetDefaultServerParam ();


		turnPlayerId = syncTurnPlayerId;

		//NetIdを取得してファイル名を設定
		GetNetIdentity();
		SetIdentity();

		if(isLocalPlayer){
			gameSceneManager.myPlayerNetIdInt = playerNetIdInt;
		}

		gameSceneManager.TurnChange (syncTurnPlayerId);
	}
		

	void Update ()
	{
		//例外が発生した時にSetIdentityメソッド実行
		if (myTransform.name == "" || myTransform.name == "Player(Clone)") {
			SetIdentity();
		}
	}
		

	void SetIdentity ()
	{
		myTransform.name = "NetworkPlayerManager" + GetComponent<NetworkIdentity>().netId.ToString();
	}
		

	void FixedUpdate(){
		//このスクリプトの付随するオブジェクトが別のネットワーク端末から作られたものでないことの確認
		if (isLocalPlayer) {
			//自分のターンならArrowのパラメータをサーバに送る
			if(gameSceneManager.myTurnFlag){
				TransmitArrowData();
			}

			CheckNextTurnPlayer();
		}

		//ターンエンド判定(サーバのみ)
		TurnEndCheck();

		//自分のターン以外であれば受け手にまわる
		//どのユーザオブジェクトからもいじれる
		if (!gameSceneManager.myTurnFlag && !isLocalPlayer/* && syncTurnPlayerId == playerNetIdInt*/) {
			ReceveArrowData ();
		}



		//プレイヤーとサーバユーザ以外のターンエンド判定、syncTurnPlayerIdが変更されたら
		if(syncTurnEnd == true){
			if(turnChangeFlag){
				
				turnChangeFlag = false;
				gameSceneManager.turnPlayerId = syncTurnPlayerId;
				gameSceneManager.TurnChange (syncTurnPlayerId);

				pullArrow.myUnit.transform.position = syncUnitPos;

				//ターン終了をお知らせ
				Debug.Log("turn end");

				Invoke("ChangeSyncTurnEndFlag", 1.0f);
				//DeleyAction (0.1f, ChangeSyncTurnEndFlag);
			}
		} else {
			turnChangeFlag = true;
		}
	}

	[Server]
	void ChangeSyncTurnEndFlag(){
		syncTurnEnd = false;
	}


	//次のプレイヤーIDを吐き出す
	private int InclementTurnPlayerId(int id){
		//４人プレイなので
		if(id+1 >= 3){
			id = 1;
		} else {
			id++;
		}

		return id;
	}

	//Unitが発射後止まっているか確認
	private void UnitStopCheck ()
	{
		//速度が０でshotFlagがtrue(発射後)なら
		if(pullArrow.myUnit.GetComponent<Rigidbody2D> ().velocity.magnitude == 0){
			//呼ばれるのは一回だけでいい
			arrowShotFlag = false;
			startUnitStopCheckFlag = false;
			CmdProvideTurnEndToServer(pullArrow.myUnit.transform.position, playerNetIdInt);
		}
	}


	private void DeleyUnitStopCheck(){
		if(!startUnitStopCheckFlag){
			startUnitStopCheckFlag = true;
		}
	}

	//メソッドを渡して処理をwaitTimeだけ遅延させる
	IEnumerator DeleyAction (float waitTime, Action act)
	{
		yield return new WaitForSeconds(waitTime);
		act();
	}

	//syncで受け取ったデータをarrowに反映する
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

////////////////////////////////////////////////////////[Server]/////////////////////////////////////////////////////////////////

	[Server]
	void SetDefaultServerParam(){
		syncTurnPlayerId = gameSceneManager.firstTurnPlayerId;
	}

	//サーバのUnitが停止したら強制的に次のターンへ移項
	[Server]
	void TurnEndCheck(){
		if(playerNetIdInt == gameSceneManager.turnPlayerId/*syncTurnPlayerId*/){
			if (syncArrowShotFlag == true) {
				if (startUnitStopCheckFlag) {
					//Unitが移動いてるのかどうかのチェック
					UnitStopCheck ();
				} else {
					//ちょっと遅らせてstartUnitStopCheckFlagをtrueにする
					StartCoroutine(DeleyAction (0.1f, DeleyUnitStopCheck));
				}
			}
		}
	}
		

////////////////////////////////////////////////////////[Client]/////////////////////////////////////////////////////////////////

	[Client]
	void GetNetIdentity()
	{
		//NetworkIdentityのNetID取得
		playerNetID = GetComponent<NetworkIdentity>().netId;
		playerNetIdInt =  int.Parse(playerNetID.ToString());
		//名前を付けるメソッド実行
		//CmdTellServerMyIdentity(playerNetIdInt);
	}



	[Client]
	void CheckNextTurnPlayer(){
//		if (syncTurnPlayerId == playerNetIdInt) {
//			gameSceneManager.myTurnFlag = true;
//		} else {
//			gameSceneManager.myTurnFlag = false;
//		}
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
			arrowShotFlag = pullArrow.shotFlag;
			arrowDistance = new Vector2 (pullArrow.dx, pullArrow.dy);

			//サーバにパラメータ送信
			CmdProvideArrowDataToServer (arrowPos, arrowRot, arrowSize, arrowShotFlag, arrowDistance);
		} else {
			//Turnプレイヤーのタップがはなれた時点でArrowのSizeを0にしてその時点のarrowDistanceをsyncに入れる
			if(arrowPos != Vector3.zero && arrowSize != Vector2.zero){
				arrowPos = Vector3.zero;
				arrowSize = Vector2.zero;
				arrowShotFlag = pullArrow.shotFlag;

				//サーバにパラメータ送信
				CmdProvideArrowDataToServer (arrowPos, arrowRot, arrowSize, arrowShotFlag, arrowDistance);
			}
		}
	}




////////////////////////////////////////////////////////[Command]/////////////////////////////////////////////////////////////////

	//Command: SyncVar変数を変更し、変更結果を全クライアントへ送る
	[Command]
	void CmdTellServerMyIdentity (int nid)
	{
		//playerUniqueIdentity = nid;
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
		
	//サーバでターンエンドした時の情報を全クライアントに送る
	[Command]
	void CmdProvideTurnEndToServer (Vector3 unitPos, int playerNetIdInt){
		syncArrowShotFlag = arrowShotFlag;

		syncUnitPos = unitPos;
		syncTurnPlayerId = InclementTurnPlayerId (gameSceneManager.turnPlayerId/*syncTurnPlayerId*/);
		syncTurnEnd = true;
	}
}
