using UnityEngine;
using System.Collections;

public class UnitDetailController : MonoBehaviour {
	[SerializeField]
	NavigationBarController navigationBar;

	public int unitId;

	//遷移アニメーション終了時に呼び出される
	public void AnimationEnd(){
		if (this.gameObject.GetComponent<RectTransform>().anchoredPosition.x == 0) {
			navigationBar.ChangeTweenPanel (this.gameObject);
		} else {
			navigationBar.RollBackTweenPanel ();
		}
	}
}
