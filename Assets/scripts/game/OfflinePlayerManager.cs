using UnityEngine;
using System.Collections;

public class OfflinePlayerManager : MonoBehaviour {

	public PullArrow pullArrow;
	public GameStateManager gameStateManager;
	public GameSceneManager gameSceneManager;
	public bool startUnitStopCheckFlag;

	void Start () {
		//offline時はつねに俺のターン
		gameSceneManager.myTurnFlag = true;
		SetAllUnitParamator ();
	}
	
	void Update () {
	
	}

	void FixedUpdate(){
		CheckUnitStop ();
	}

	//次のプレイヤーIDを吐き出す
	private int InclementTurnPlayerId(int id){
		if(id+1 > 3){
			id = 0;
		} else {
			id++;
		}
		return id;
	}

	private void CheckUnitStop(){
		if (startUnitStopCheckFlag == false) {
			if (pullArrow.myUnit.GetComponent<Rigidbody2D> ().velocity.magnitude != 0) {
				startUnitStopCheckFlag = true;
			}
		} else if(startUnitStopCheckFlag == true){
			if (pullArrow.myUnit.GetComponent<Rigidbody2D> ().velocity.magnitude == 0) {
				startUnitStopCheckFlag = false;

				Debug.Log("turn end");
				StartCoroutine (gameSceneManager.DisplayTurnEndText(0.7f));

				//移動してたら止める
				pullArrow.myUnit.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;
				//shotFlagをfalse(まだshotしてない)に
				pullArrow.shotFlag = false;
				//一度全ユニットの移動を止める　これがないと誤判定してターンが飛ぶ
				gameSceneManager.StopAllunitVelocity ();

				//コントロールするユニットも変更
				int nextTurnPlayerId = InclementTurnPlayerId(gameSceneManager.turnPlayerId);
				gameSceneManager.turnPlayerId = nextTurnPlayerId;
				gameSceneManager.ChangeControllUnit (nextTurnPlayerId);
			}
		}
	}

	private void SetAllUnitParamator(){
		gameSceneManager.SetAllUnitParamator (gameStateManager.partyUnitDic);
	}
}
