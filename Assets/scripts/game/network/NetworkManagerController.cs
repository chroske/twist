using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
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
	private NetworkManager networkManager;

	//Arrow関連データ
	public Vector3 arrowPos;
	public Quaternion arrowRot;
	public Vector2 arrowSize;
	public bool arrowShotFlag;
	public Vector2 arrowDistance;

	private bool remoteShotFlag;
	private NetworkInstanceId playerNetID;
	private Transform myTransform;
	public int playerNetIdInt;
	public bool startUnitStopCheckFlag;

	private bool turnChangeFlag;


	void Awake () {
		//自分の名前を取得する時に使う
		myTransform = transform;

		gameSceneManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
		arrow = GameObject.Find("GameCanvas/Arrow");
		pullArrow = arrow.GetComponent<PullArrow> ();
		networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

		SetDefaultSyncParam();
	}

	void Start(){
		remoteShotFlag = true;
		startUnitStopCheckFlag = false;

		//NetIdを取得してファイル名を設定
		GetNetIdentity();
		SetIdentity();

		if(isLocalPlayer){
			gameSceneManager.myPlayerNetIdInt = playerNetIdInt;
		}

		//syncTurnPlayerId届くまでタイムラグあるっぽい　ちょっと待ってから実行しないとだめかも
		gameSceneManager.TurnChange (syncTurnPlayerId);
	}
		

	void Update ()
	{
		//例外が発生した時にSetIdentityメソッド実行
		if (myTransform.name == "" || myTransform.name == "Player(Clone)") {
			SetIdentity();
		}
	}
		
	void FixedUpdate(){
		//このスクリプトの付随するオブジェクトが別のネットワーク端末から作られたものでないことの確認
		if (isLocalPlayer) {
			//自分のターンならArrowのパラメータをサーバに送る
			if(gameSceneManager.myTurnFlag && playerNetIdInt == gameSceneManager.turnPlayerId){
				TransmitArrowData();
			}
		}

		//ターンエンド判定(サーバのみ)
		TurnEndCheck();

		//自分のターン以外であれば受け手にまわる
		//どのユーザオブジェクトからもいじれる
		if (!gameSceneManager.myTurnFlag && !isLocalPlayer && playerNetIdInt == gameSceneManager.turnPlayerId) {
			ReceveArrowData ();
		}
	}

	private void SetIdentity (){
		myTransform.name = "NetworkPlayerManager" + GetComponent<NetworkIdentity>().netId.ToString();
	}
		
	//次のプレイヤーIDを吐き出す
	private int InclementTurnPlayerId(int id){
		//４人プレイなので
		if(id+1 > networkManager.numPlayers){
			id = 1;
		} else {
			id++;
		}

		return id;
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
		yield break;
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
	void SetDefaultSyncParam(){
		syncTurnPlayerId = gameSceneManager.firstTurnPlayerId;
	}

	//サーバのUnitが停止したら強制的に次のターンへ移項
	[Server]
	void TurnEndCheck(){
		if(playerNetIdInt == gameSceneManager.turnPlayerId){
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

	//Unitが発射後止まっているか確認
	[Server]
	private void UnitStopCheck (){
		//速度が０でshotFlagがtrue(発射後)なら
		if(pullArrow.myUnit.GetComponent<Rigidbody2D> ().velocity.magnitude == 0){
			//呼ばれるのは一回だけでいい
			arrowShotFlag = false;
			startUnitStopCheckFlag = false;
			//CmdProvideTurnEndToServer(pullArrow.myUnit.transform.position, playerNetIdInt);

			int nextTurnPlayerId = InclementTurnPlayerId(gameSceneManager.turnPlayerId);
			//クライアントをターンエンドさせる
			RpcTurnEndClient(pullArrow.myUnit.transform.position, nextTurnPlayerId);
			syncArrowShotFlag = arrowShotFlag;
		}
	}
		

////////////////////////////////////////////////////////[Client]/////////////////////////////////////////////////////////////////

	[Client]
	void GetNetIdentity()
	{
		//NetworkIdentityのNetID取得
		playerNetID = GetComponent<NetworkIdentity>().netId;
		playerNetIdInt =  int.Parse(playerNetID.ToString());
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
		

	//クライアント側から受け取ったパラメータをサーバ側でsyncにつめる
	[Command]
	void CmdProvideArrowDataToServer (Vector3 arrowPos, Quaternion arrowRot, Vector2 arrowSize, bool arrowShotFlag, Vector2 arrowDistance){
		syncArrowPos = arrowPos;
		syncArrowRot = arrowRot;
		syncArrowSize = arrowSize;
		syncArrowShotFlag = arrowShotFlag;
		syncArrowDistance = arrowDistance;
	}

////////////////////////////////////////////////////////[ClientRpc]/////////////////////////////////////////////////////////////////

	[ClientRpc]
	void RpcTurnEndClient(Vector3 unitPos, int nextTurnPlayerId){
		//ターン終了をお知らせ
		Debug.Log("turn end");

		//サーバの停止位置を反映
		pullArrow.myUnit.transform.position = unitPos;
		//TurnPlayerIdを１増やしてsyncにつめる
		gameSceneManager.turnPlayerId =nextTurnPlayerId;
		//ターンチェンジ判定
		gameSceneManager.TurnChange (gameSceneManager.turnPlayerId);
	}
}
