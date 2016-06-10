using UnityEngine;
using System.Collections;

public class ConfigPanelTopController : MonoBehaviour {

	[SerializeField]
	private GameObject ConfigPanelTwitterConnect;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
	public void OnClickTwitterConnectNode(){
		ConfigPanelTwitterConnect.SetActive (true);
	}
}
