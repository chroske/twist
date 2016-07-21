using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[NetworkSettings(sendInterval=0.2f)]
public class DuelNetworkPlayerController : NetworkBehaviour {
	[SyncVar] public Vector3 syncArrowPos;
	[SyncVar] public Quaternion syncArrowRot;
	[SyncVar] public Vector2 syncArrowSize;
	[SyncVar] public bool syncArrowShotFlag;

	[SyncVar(hook="ReceveShotFlag")] public Vector2 syncArrowDistance;
	[SyncVar] public Vector3 syncUnitPos;
	[SyncVar] public int syncTurnPlayerId;
	[SyncVar] public int syncPlayerNetID;

	//ユニットのパラメータSyncListStruct
	public UnitParamsData unitParamsData = new UnitParamsData();

	//ゲームオブジェクトとコンポーネント
	private GameObject arrow;
	private DuelPullArrow pullArrow;
	private DuelGameSceneManager gameSceneManager;
	private NetworkManager networkManager;

	//Arrow関連データ
	public Vector3 arrowPos;
	public Quaternion arrowRot;
	public Vector2 arrowSize;
	public bool arrowShotFlag;
	public Vector2 arrowDistance;

	public bool startUnitStopCheckFlag;

	//public List<OwnedUnitData> partyUnitParamList = new List<OwnedUnitData>();

	void Awake () {
		//自分の名前を取得する時に使う
		gameSceneManager = GameObject.Find("/GameSceneManager").GetComponent<DuelGameSceneManager>();
		arrow = GameObject.Find("/GameCanvas/DuelArrow");
		pullArrow = arrow.GetComponent<DuelPullArrow> ();
		networkManager = GameObject.Find("/MainCanvas/BattlePanel/BattlePanelOnlineDuel").GetComponent<NetworkLobbyManager>();

		//サーバに初期データセット
		SetDefaultSyncParam();
	}

	void Start(){
		startUnitStopCheckFlag = false;

		if(isLocalPlayer){
			gameSceneManager.myPlayerNetIdInt = syncPlayerNetID;
		}

		gameSceneManager.TurnChange (syncTurnPlayerId);
		gameSceneManager.SetControllUnit ();
	}

	public override void OnStartClient(){
		//unitParamsClassはローカルのクラスなのでListにして送る（めんどくさいのでunitParamsClassはちゃんとクラス化する
		List<OwnedUnitData> partyUnitParamList = new List<OwnedUnitData> ();
		foreach(var partyUnitParam in unitParamsData){
			Dictionary<string, object> data = new Dictionary<string, object> () {
				{ "unit_id", partyUnitParam.unit_id },
				{ "unit_acount_id", partyUnitParam.unit_account_id },
				{ "unit_name", partyUnitParam.unit_name },
				{ "unit_icon_url", partyUnitParam.unit_icon_url },
				{ "party_id", 0 },
				{ "attack", partyUnitParam.attack },
				{ "hitPoint", partyUnitParam.hitPoint },
				{ "speed", partyUnitParam.speed },
				{ "type", partyUnitParam.type },
				{ "Level", partyUnitParam.Level },
				{ "combo", partyUnitParam.combo },
				{ "ability_1", partyUnitParam.ability_1 },
				{ "ability_2", partyUnitParam.ability_2 },
				{ "ability_3", partyUnitParam.ability_3 },
				{ "strikeShot", partyUnitParam.strikeShot },
				{ "comboType", partyUnitParam.comboType },
				{ "comboAttack", partyUnitParam.comboAttack },
				{ "maxComboNum", partyUnitParam.maxComboNum }
			};

			OwnedUnitData partyUnitData = new OwnedUnitData (data);

			partyUnitParamList.Add (partyUnitData);
		}
		gameSceneManager.SetUnitParamatorByNetId (syncPlayerNetID, partyUnitParamList);
	}

	void FixedUpdate(){
		//このスクリプトの付随するオブジェクトが別のネットワーク端末から作られたものでないことの確認
		if (isLocalPlayer) {
			//自分のターンならArrowのパラメータをサーバに送る
			if(gameSceneManager.myTurnFlag/* && syncPlayerNetID == gameSceneManager.turnPlayerId*/){
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
		Debug.Log ("InclementTurnPlayerId");

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
		//if(!isLocalPlayer){
			if (shotVector != Vector2.zero) {
				//pullArrow.RemoteShot (shotVector);
				//gameSceneManager.RemoteShot(shotVector);
			}
		//}
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
			CmdProvideArrowDataToServer (arrowPos, arrowRot, arrowSize, arrowShotFlag, arrowDistance, gameSceneManager.myPlayerNetIdInt);
		} else {
			//Turnプレイヤーのタップがはなれた時点でArrowのSizeを0にしてその時点のarrowDistanceをsyncに入れる
			if(arrowPos != Vector3.zero && arrowSize != Vector2.zero){
				arrowSize = Vector2.zero;
				arrowShotFlag = pullArrow.shotFlag;

				//サーバにパラメータ送信
				CmdProvideArrowDataToServer (arrowPos, arrowRot, arrowSize, arrowShotFlag, pullArrow.shotVector, gameSceneManager.myPlayerNetIdInt);
			}
		}
	}


	private Vector2 guestArrowDistance = new Vector2();
	private Vector2 hostArrowDistance = new Vector2();

	////////////////////////////////////////////////////////[Command]/////////////////////////////////////////////////////////////////
	//クライアント側から受け取ったパラメータをサーバ側でsyncにつめる
	[Command]
	void CmdProvideArrowDataToServer (Vector3 arrowPos, Quaternion arrowRot, Vector2 arrowSize, bool arrowShotFlag, Vector2 arrowDistance, int netId){
		syncArrowPos = arrowPos;
		syncArrowRot = arrowRot;
		syncArrowSize = arrowSize;

		if(arrowShotFlag){
			//syncArrowDistance = arrowDistance;

			if (netId == 0) {
				gameSceneManager.hostPlayerShotFlag = true;
				gameSceneManager.hostShotVector = arrowDistance;
				Debug.Log ("shotPlayerIsLocal ");
			} else if(netId == 1) {
				gameSceneManager.guestPlayerShotFlag = true;
				gameSceneManager.guestShotVector = arrowDistance;
				Debug.Log ("not shotPlayerIsLocal ");
			}
			//お互いshot済みならばRpcShot実行で全クライアントでshot
			if (gameSceneManager.hostPlayerShotFlag && gameSceneManager.guestPlayerShotFlag) {
				Debug.Log ("RpcShot");
				RpcShot (gameSceneManager.hostShotVector, gameSceneManager.guestShotVector);
				gameSceneManager.hostPlayerShotFlag = false;
				gameSceneManager.guestPlayerShotFlag = false;
			}
		}
		syncArrowShotFlag = arrowShotFlag;
	}

	[Command]
	void CmdProvideTurnEndToServer(Vector3 unitPos){
		if(syncPlayerNetID == gameSceneManager.turnPlayerId){
			//次のplayerIdを生成
			int nextTurnPlayerId = InclementTurnPlayerId(gameSceneManager.turnPlayerId);

			//サーバのユニットのポジションを取得
			Vector3[] myUnitPositionArray = gameSceneManager.GetAllMyUnitPosition();
			Vector3[] enemyUnitPositionArray = gameSceneManager.GetAllEnemyUnitPosition();

			//全クライアントをターンエンドさせる
			RpcTurnEndClient(myUnitPositionArray, enemyUnitPositionArray, nextTurnPlayerId);
		}
	}

	////////////////////////////////////////////////////////[ClientRpc]/////////////////////////////////////////////////////////////////

	//全クライアントをターンエンドさせる
	[ClientRpc]
	void RpcTurnEndClient(Vector3[] myUnitPosition, Vector3[] enemyUnitPosition, int nextTurnPlayerId){
		//ターン終了をお知らせ
		Debug.Log("turn end");
		StartCoroutine (gameSceneManager.DisplayTurnEndText(0.7f));

		//サーバの停止位置を反映
		//pullArrow.myUnit.transform.position = unitPos;
		gameSceneManager.SyncAllUnitPosition(myUnitPosition, enemyUnitPosition);
		//移動してたら止める
		pullArrow.myUnit.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
		//shotFlagをfalse(まだshotしてない)に
		pullArrow.shotFlag = false;
		//TurnPlayerIdを１増やしてsyncにつめる
		gameSceneManager.turnPlayerId = nextTurnPlayerId;
		//ターンチェンジ判定
		gameSceneManager.TurnChange (gameSceneManager.turnPlayerId);
		//全ユニットのComboNumを初期化
		gameSceneManager.ResetComboNumAllUnit ();
	}

	[ClientRpc]
	void RpcShot(Vector2 hostShotVector, Vector2 guestShotVector){
		gameSceneManager.RemoteShot(hostShotVector, guestShotVector);
	}
}
