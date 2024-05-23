using System;
using System.Collections.Generic;
using System.Reflection;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace utilities
{
    public class NetworkRunnerController : MonoBehaviour, INetworkRunnerCallbacks
    {
        public NetworkRunner networkRunnerPrefab;

        public NetworkRunner networkRunnerInstance;

        public event Action OnStartedRunnerConnection;
        public event Action OnPlayerJoinedSuccessfully;

        public void Start()
        {
            DontDestroyOnLoad(this);
        }


        public async void StartGame(GameMode gameMode, string roomName)
        {
            OnStartedRunnerConnection?.Invoke();
            
            if (!networkRunnerInstance)
            {
                networkRunnerInstance = Instantiate(networkRunnerPrefab);
            }
            
            // Register so we will get the callback as well
            // everything we write in INetworkRunnerCallbacks will register to networkRunnerInstance
            // EX.on player joined have debug in networkRunnerInstance have debug too
            networkRunnerInstance.AddCallbacks(this);

            //ProvideInput Means that player is recording and sending input to the server
            networkRunnerInstance.ProvideInput = true;

            var startGameArgs = new StartGameArgs
            {
                GameMode = gameMode,
                SessionName = roomName,
                PlayerCount = 4,
                SceneManager = networkRunnerInstance.GetComponent<INetworkSceneManager>()
            };

            var result = await networkRunnerInstance.StartGame(startGameArgs);

            if (result.Ok)
            {
                //can join or host game
                networkRunnerInstance.SetActiveScene("MainGame");
            }
            else
            {
                Debug.LogError($"Failed To Start {result.ShutdownReason}");
            }
            
        }

        public void ShutDownRunner()
        {
            networkRunnerInstance.Shutdown();
        }
        
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        { 
           Debug.Log("OnPlayerJoined");
           OnPlayerJoinedSuccessfully?.Invoke();
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            Debug.Log("OnPlayerLeft");
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            Debug.Log("OnInput");
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
            Debug.Log("OnInputMissing");
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
            Debug.Log("OnShutdown");
            
            SceneManager.LoadScene("Lobby");
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
            Debug.Log("OnConnectedToServer");
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
            Debug.Log("OnDisconnectedFromServer");
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
            Debug.Log("OnConnectRequest");
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
            Debug.Log("OnConnectFailed");
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
            Debug.Log("OnUserSimulationMessage");
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
            Debug.Log("OnSessionListUpdated");
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
            Debug.Log("OnCustomAuthenticationResponse");
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
            Debug.Log("OnHostMigration");
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
            Debug.Log("OnReliableDataReceived");
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
            Debug.Log("OnSceneLoadDone");
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
            Debug.Log("OnSceneLoadStart");
        }
    }
}