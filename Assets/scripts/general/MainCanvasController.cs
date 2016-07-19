using UnityEngine;
using System.Collections;

public class MainCanvasController : MonoBehaviour {

	private static MainCanvasController instance = null;
	void Awake(){
		//シングルトン
		if( instance != null ) {
			Destroy( this.gameObject );
			return;
		}
		DontDestroyOnLoad (this.gameObject);
		instance = this;
	}
}
