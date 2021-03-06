﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NavigationBarController : MonoBehaviour {

	[SerializeField]
	GameObject BackPanelButton;

	[SerializeField]
	uTools.uPlayTween playTween;

	public List<GameObject> panelList = new List<GameObject> ();

	public void ChangeTweenPanel(GameObject panel){
		//currentPanel = newCurrentpanel;
		playTween.ChangeTweenTarget(panel);
		panelList.Add (panel);
		//backボタンを出す
		BackPanelButton.SetActive(true);
	}

	public void RollBackTweenPanel(){
		panelList.RemoveAt (panelList.Count - 1);
		if (panelList.Count != 0) {
			playTween.ChangeTweenTarget (panelList [panelList.Count - 1]);
		} else {
			//backボタン消す
			BackPanelButton.SetActive(false);
		}
	}
}
