using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using System;

namespace Jitto.NetZeroGPlayer
{
	public class FusionConnection : MonoBehaviour, INetworkRunnerCallbacks
	{

		public bool connectOnAwake;

		public NetworkRunner runner;

		private void Awake() 
		{
			
		}

		public async void ConnectToRunner()
		{
			if (runner == null)
			{
				runner.gameObject.AddComponent<NetworkRunner>();
			}
		}

		public void OnConnectedToServer(NetworkRunner runner)
		{
			Debug.Log("OConnectedtoServer");
		}

		public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
		{
			Debug.Log("OnConnectFailed");
		}

		public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
		{

		}

		public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
		{

		}

		public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
		{

		}

		public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
		{

		}

		public void OnInput(NetworkRunner runner, NetworkInput input)
		{

		}

		public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
		{

		}

		public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{

		}

		public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{

		}

		public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			Debug.Log("OnPlayerJoined");
		}

		public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			Debug.Log("OnPlayerLeft");
		}

		public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
		{

		}

		public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
		{

		}

		public void OnSceneLoadDone(NetworkRunner runner)
		{

		}

		public void OnSceneLoadStart(NetworkRunner runner)
		{

		}

		public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
		{

		}

		public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
		{

		}

		public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
		{

		}
	}
}
