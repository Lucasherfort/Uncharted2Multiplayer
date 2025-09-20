using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    public NetworkPlayer playerPrefab;

    Dictionary<int, NetworkPlayer> mapTokenIDWithNetworkPlayer;

    CharacterInputHandler characterInputHandler;

    private NetworkRunnerHandler runnerHandler;

    private Dictionary<PlayerRef, PlayerInfo> players = new Dictionary<PlayerRef, PlayerInfo>();


    private void Awake()
    {
        mapTokenIDWithNetworkPlayer = new Dictionary<int, NetworkPlayer>();
        runnerHandler = FindObjectOfType<NetworkRunnerHandler>();
    }

    int GetPlayerToken(NetworkRunner runner, PlayerRef player)
    {
        if (runner.LocalPlayer == player)
        {
            return ConnectionTokenUtils.HasToken(GameManager.instance.GetConnectionToken());
        }
        else
        {
            var token = runner.GetPlayerConnectionToken(player);
            if (token != null)
                return ConnectionTokenUtils.HasToken(token);

            return 0;
        }
    }

    public void SetConnectionTokenMapping(int token, NetworkPlayer networkPlayer)
    {
        mapTokenIDWithNetworkPlayer.Add(token, networkPlayer);
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
// Exemple : récupérer le pseudo depuis le client
    string playerName = "Joueur_" + player.PlayerId; // Remplace par ton système réel
    int xp = 1; // valeur par défaut ou récupérée côté client

    // Ajouter dans le dictionnaire
    players[player] = new PlayerInfo(playerName, xp, player);
    
    // Créer la liste à partir du dictionnaire
    List<PlayerInfo> playerList = new List<PlayerInfo>(players.Values);
    
    // Récupérer le composant UI
    UsersSessionListHandler uiHandler = FindObjectOfType<UsersSessionListHandler>();
    if (uiHandler != null)
    {
        // Passer le PlayerRef local pour que l'UI puisse mettre le joueur local en premier
        uiHandler.OnUpdatePlayersList(playerList, runner.LocalPlayer);
    }
/*
                                        var token = runner.GetPlayerConnectionToken(player);

                                        string json = System.Text.Encoding.UTF8.GetString(token);
                                        PlayerInfo info = JsonUtility.FromJson<PlayerInfo>(json);
                                        */


        if (runner.IsServer)
        {
            /*
            int playerToken = GetPlayerToken(runner, player);

            if (mapTokenIDWithNetworkPlayer.TryGetValue(playerToken, out NetworkPlayer networkPlayer))
            {
                networkPlayer.GetComponent<NetworkObject>().AssignInputAuthority(player);

                networkPlayer.Spawned();
            }
            else
            {
                NetworkPlayer spawnedNetworkPlayer = runner.Spawn(playerPrefab, Utils.GetRandomSpawnPoint(), Quaternion.identity, player);

                spawnedNetworkPlayer.token = playerToken;

                mapTokenIDWithNetworkPlayer[playerToken] = spawnedNetworkPlayer;
            }
            */
        }
        else
        {

        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (characterInputHandler == null && NetworkPlayer.Local != null)
        {
            characterInputHandler = NetworkPlayer.Local.GetComponent<CharacterInputHandler>();
        }

        if (characterInputHandler != null)
        {
            input.Set(characterInputHandler.GetNetworkInput());
        }
    }


    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("OnConnectedToServer");
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

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

    public async void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        await runner.Shutdown(shutdownReason: ShutdownReason.HostMigration);

        FindObjectOfType<NetworkRunnerHandler>().StartHostMigration(hostMigrationToken);
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

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (players.ContainsKey(player))
        players.Remove(player);

        // Afficher la liste mise à jour
        foreach (var p in players.Values)
        {
            Debug.Log($"Pseudo: {p.username}, XP: {p.xp}");
        }
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
        if (sessionList.Count == 0)
        {
            Debug.Log("Joined lobby, no session found → create one");
            runnerHandler.CreateGame("TestSession", "GameScene"); 
        }
        else
        {
            // Ici je prends la première session trouvée, 
            // tu pourrais ajouter une logique de filtrage
            SessionInfo sessionToJoin = sessionList[0];
            Debug.Log($"Session found → join {sessionToJoin.Name}");
            runnerHandler.JoinGame(sessionToJoin);
        }
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

    public void OnHostMigrationCleanUp()
    {
        foreach (KeyValuePair<int, NetworkPlayer> entry in mapTokenIDWithNetworkPlayer)
        {
            NetworkObject networkObjetInDictionnary = entry.Value.GetComponent<NetworkObject>();

            if (networkObjetInDictionnary.InputAuthority.IsNone)
            {
                networkObjetInDictionnary.Runner.Despawn(networkObjetInDictionnary);
            }
        }
    }
}
