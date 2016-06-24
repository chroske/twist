using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class BattlePanelOfflineController : MonoBehaviour {
	public GameStateManager gameStateManager;

	public void OnClickQuestButton(){
		StartTheOfflineGame ();
	}

	private void StartTheOfflineGame(){
		gameStateManager.offlineGame = true;
		SceneManager.LoadScene("GameMain", LoadSceneMode.Additive);
	}
}
