using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[NetworkSettings(sendInterval=0.2f)]
public class NetworkPlayerManager : NetworkBehaviour {
	[SyncVar] public Vector3 syncArrowPos;
	[SyncVar] public Quaternion syncArrowRot;
	[SyncVar] public Vector2 syncArrowSize;
	[SyncVar] public bool syncArrowShotFlag;

	[SyncVar(hook="ReceveShotFlag")] public Vector2 syncArrowDistance;
	[SyncVar] public Vector3 syncUnitPos;
	[SyncVar] public int syncTurnPlayerId;
	[SyncVar] public int syncPlayerNetID;

	//LobbyPlayerからわたってくるUnitのパラメータ
	[SyncVar] public int syncUnitId;
	[SyncVar] public string syncUnitAccountId;
	[SyncVar] public string syncUnitName;
	[SyncVar] public string syncUnitIconUrl;
	[SyncVar] public int syncUnitAttack;
	[SyncVar] public int syncUnitHitpoint;
	[SyncVar] public float syncUnitSpeed;
	[SyncVar] public int syncUnitType;
	[SyncVar] public int syncUnitLevel;
	[SyncVar] public int syncUnitCombo;
	[SyncVar] public int syncUintAbbility_1;
	[SyncVar] public int syncUintAbbility_2;
	[SyncVar] public int syncUintAbbility_3;
	[SyncVar] public int syncUintStrikeShot;
	[SyncVar] public int syncUintComboType;
	[SyncVar] public int syncUintComboAttack;
	[SyncVar] public int syncUintMaxComboNum;

	//ゲームオブジェクトとコンポーネント
	private GameObject arrow;
	private PullArrow pullArrow;
	private GameSceneManager gameSceneManager;
	private NetworkManager networkManager;

	//Arrow関連データ
	public Vector3 arrowPos;
	public Quaternion arrowRot;
	public Vector2 arrowSize;
	public bool arrowShotFlag;
	public Vector2 arrowDistance;

	public bool startUnitStopCheckFlag;

	void Awake () {
		//自分の名前を取得する時に使う
		gameSceneManager = GameObject.Find("/GameSceneManager").GetComponent<GameSceneManager>();
		arrow = GameObject.Find("/GameCanvas/Arrow");
		pullArrow = arrow.GetComponent<PullArrow> ();
		networkManager = GameObject.Find("/MainCanvas/BattlePanel/BattlePanelOnline").GetComponent<NetworkLobbyManager>();

		//サーバに初期データセット
		SetDefaultSyncParam();
	}

	void Start(){
		startUnitStopCheckFlag = false;

		if(isLocalPlayer){
			gameSceneManager.myPlayerNetIdInt = syncPlayerNetID;
		}

		SetUnitParamator();
			
		gameSceneManager.TurnChange (syncTurnPlayerId);
	}
		
	void FixedUpdate(){
		//このスクリプトの付随するオブジェクトが別のネットワーク端末から作られたものでないことの確認
		if (isLocalPlayer) {
			//自分のターンならArrowのパラメータをサーバに送る
			if(gameSceneManager.myTurnFlag && syncPlayerNetID == gameSceneManager.turnPlayerId){
				TransmitArrowData();
			}
		}

		if(gameSceneManager.myTurnFlag && isLocalPlayer){
			CheckUnitStop ();
		}

		//自分のターン以外であれば受け手にまわる
		//どのユーザオブジェクトからもいじれる
		if (!gameSceneManager.myTurnFlag && !isLocalPlayer && syncPlayerNetID == gameSceneManager.turnPlayerId) {
			ReceveArrowData ();
		}
	}
		
	//次のプレイヤーIDを吐き出す
	private int InclementTurnPlayerId(int id){
		if(id+1 > networkManager.numPlayers-1){
			id = 0;
		} else {
			id++;
		}
		return id;
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
		RectTransform rectTrans = arrow.transform.GetComponent <RectTransform> ();

		arrow.transform.position = syncArrowPos;
		arrow.transform.rotation = Quaternion.Slerp(arrow.transform.rotation, syncArrowRot, Time.deltaTime * 5);

		if (syncArrowSize.y == 0) {
			rectTrans.sizeDelta = syncArrowSize;
		} else {
			rectTrans.sizeDelta = Vector2.Lerp (rectTrans.sizeDelta, syncArrowSize, Time.deltaTime * 5);
		}
	}


	//フラグを受け取ったらショットする
	void ReceveShotFlag(Vector2 shotVector){
		if(!isLocalPlayer){
			if (shotVector != Vector2.zero) {
				pullArrow.RemoteShot (shotVector);
			}
		}
	}

	//Unitが発射後の停止を確認
	private void CheckUnitStop(){
		//if(gameSceneManager.turnPlayerId == playerNetIdInt){
		if(pullArrow.myUnit.GetComponent<Rigidbody2D> ().velocity.magnitude != 0 && startUnitStopCheckFlag == false){ //unitが動き出した時に判定フラグをtrueに
			startUnitStopCheckFlag = true;
		} else if(pullArrow.myUnit.GetComponent<Rigidbody2D> ().velocity.magnitude == 0 && startUnitStopCheckFlag == true){ //速度が０で発射後なら
			startUnitStopCheckFlag = false;

			//サーバに停止をお知らせ
			CmdProvideTurnEndToServer(pullArrow.myUnit.transform.position);
		}
	}

	public void SetUnitParamator(){
		Dictionary<string, object> data = new Dictionary<string, object> () {
			{ "unit_id", syncUnitId },
			{ "unit_acount_id", syncUnitAccountId },
			{ "unit_name", syncUnitName },
			{ "unit_icon_url", syncUnitIconUrl },
			{ "party_id", 0 },
			{ "attack", syncUnitAttack },
			{ "hitPoint", syncUnitHitpoint },
			{ "speed", syncUnitSpeed },
			{ "type", syncUnitType },
			{ "Level", syncUnitLevel },
			{ "combo", syncUnitCombo },
			{ "ability_1", syncUintAbbility_1 },
			{ "ability_2", syncUintAbbility_2 },
			{ "ability_3", syncUintAbbility_3 },
			{ "strikeShot", syncUintStrikeShot },
			{ "comboType", syncUintComboType },
			{ "comboAttack", syncUintComboAttack },
			{ "maxComboNum", syncUintMaxComboNum }
		};

		OwnedUnitData myUnitParam = new OwnedUnitData(data);
		gameSceneManager.SetUnitParamatorByNetId (syncPlayerNetID, myUnitParam);
	}

////////////////////////////////////////////////////////[Server]/////////////////////////////////////////////////////////////////

	[Server]
	void SetDefaultSyncParam(){
		syncTurnPlayerId = gameSceneManager.firstTurnPlayerId;
	}

////////////////////////////////////////////////////////[Client]/////////////////////////////////////////////////////////////////
		
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
				arrowSize = Vector2.zero;
				arrowShotFlag = pullArrow.shotFlag;

				//サーバにパラメータ送信
				CmdProvideArrowDataToServer (arrowPos, arrowRot, arrowSize, arrowShotFlag, pullArrow.shotVector);
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

		if(arrowShotFlag){
			syncArrowDistance = arrowDistance;
		}
		syncArrowShotFlag = arrowShotFlag;
	}

	[Command]
	void CmdProvideTurnEndToServer(Vector3 unitPos){
		if(syncPlayerNetID == gameSceneManager.turnPlayerId){
			//次のplayerIdを生成
			int nextTurnPlayerId = InclementTurnPlayerId(gameSceneManager.turnPlayerId);
			//全クライアントをターンエンドさせる
			RpcTurnEndClient(unitPos, nextTurnPlayerId);
		}
	}

////////////////////////////////////////////////////////[ClientRpc]/////////////////////////////////////////////////////////////////

	//全クライアントをターンエンドさせる
	[ClientRpc]
	void RpcTurnEndClient(Vector3 unitPos, int nextTurnPlayerId){
		//ターン終了をお知らせ
		Debug.Log("turn end");
		StartCoroutine (gameSceneManager.DisplayTurnEndText(0.7f));

		//サーバの停止位置を反映
		pullArrow.myUnit.transform.position = unitPos;
		//移動してたら止める
		pullArrow.myUnit.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		//shotFlagをfalse(まだshotしてない)に
		pullArrow.shotFlag = false;
		//TurnPlayerIdを１増やしてsyncにつめる
		gameSceneManager.turnPlayerId =nextTurnPlayerId;
		//ターンチェンジ判定
		gameSceneManager.TurnChange (gameSceneManager.turnPlayerId);
		//コントロールするユニットも変更
		gameSceneManager.ChangeControllUnit (gameSceneManager.turnPlayerId);
	}
}
