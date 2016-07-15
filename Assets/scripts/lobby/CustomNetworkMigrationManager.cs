using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.Collections;

public class CustomNetworkMigrationManager : NetworkMigrationManager
{
	[SerializeField]
	CustomNetworkLobbyManager customNetworkLobbyManager;

	public float reconnectTimeout = 10f;

	Coroutine reconnectCoroutine;

	void Start(){
		customNetworkLobbyManager = this.gameObject.GetComponent<CustomNetworkLobbyManager>();
	}


	protected override void OnClientDisconnectedFromHost(NetworkConnection conn, out SceneChangeOption sceneChange){
		Debug.Log ("OnClientDisconnectedFromHost");
		base.OnClientDisconnectedFromHost(conn, out sceneChange);

		PeerInfoMessage mess;
		bool beHost;
		if (FindNewHost(out mess, out beHost))
		{
			newHostAddress = mess.address;
			if (beHost)
				waitingToBecomeNewHost = true;
			else
				waitingReconnectToNewHost = true;
		}

		// タイムアウトループ.
		reconnectCoroutine = StartCoroutine(ReconnectCoroutine());
	}


	IEnumerator ReconnectCoroutine()
	{
		// タイムアウトループ.
		for (var t=0f;t<reconnectTimeout;t+=Time.deltaTime)
		{
			if (waitingToBecomeNewHost && customNetworkLobbyManager != null) {
				
				//if(BecomeNewHost (NetworkManager.singleton.networkPort)){
				if(BecomeNewHost (customNetworkLobbyManager.networkPort)){
					Debug.Log ("Become new host");
					yield break;
				}
			} else if (waitingReconnectToNewHost)
			{
				Reset(oldServerConnectionId);

				if (customNetworkLobbyManager != null)
				{
					customNetworkLobbyManager.networkAddress = newHostAddress;
//					if(customNetworkLobbyManager.client.ReconnectToNewHost(newHostAddress,customNetworkLobbyManager.networkPort)){
//						Debug.Log ("Reconnect success");
//						yield break;
//					}
				}
			}
			yield return null;
		}
		reconnectCoroutine = null;
		Debug.Log ("Time Out");
	}

	protected override void OnServerReconnectPlayer(NetworkConnection newConnection, GameObject oldPlayer, int oldConnectionId, short playerControllerId){
		Debug.Log ("OnServerReconnectPlayer");
		Debug.Log (oldConnectionId);
		Debug.Log (playerControllerId);

		base.OnServerReconnectPlayer(newConnection, oldPlayer, oldConnectionId, playerControllerId);
	}
}