using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyUI : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private GameObject panelLobby;
    [SerializeField] private GameObject panelSearching;
    [SerializeField] private TMP_Text[] playerTexts;
    [SerializeField] private GameObject recherchepartiesButton;
    [SerializeField] private GameObject startButton;

    private NetworkRunner runner;

    async public void OnClickSearchMatch()
{
    // 1️⃣ UI : passer en mode recherche
    panelLobby.SetActive(false);
    panelSearching.SetActive(true);

    // 2️⃣ Récupérer le NetworkRunner depuis le FusionLauncher
    if (runner == null)
    {
        runner = FindObjectOfType<NetworkRunner>();
        if (runner == null)
        {
            Debug.LogError("❌ Aucun NetworkRunner trouvé dans la scène ! Assure-toi que FusionLauncher est présent.");
            panelLobby.SetActive(true);
            panelSearching.SetActive(false);
            return;
        }
    }

    // 3️⃣ Ajouter les callbacks si pas déjà fait
    runner.AddCallbacks(this);

    // 4️⃣ Préparer les arguments pour démarrer la partie
    var args = new StartGameArgs()
    {
        GameMode = GameMode.AutoHostOrClient,
        SessionName = "Room_2Players",
        Scene = SceneRef.FromIndex(1), // LobbyScene
        SceneManager = runner.GetComponent<INetworkSceneManager>(),
        PlayerCount = 2 // nombre max de joueurs
    };

    // 5️⃣ Démarrer la session
    var result = await runner.StartGame(args);

    // 6️⃣ Gérer l’erreur
    if (!result.Ok)
    {
        Debug.LogError($"❌ Impossible de démarrer la partie : {result.ShutdownReason}");
        panelLobby.SetActive(true);
        panelSearching.SetActive(false);
        return;
    }

    // 7️⃣ Afficher le bouton "Start" seulement si je suis l’hôte
    startButton.SetActive(runner.IsServer);

    // Optionnel : mettre à jour la liste des joueurs si déjà présents
    UpdatePlayerList();
}

    // ============ Callbacks Fusion ============

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"👤 Joueur {player.PlayerId} a rejoint");

        UpdatePlayerList();

        // Si on a atteint 2 joueurs → on lance la partie
        if (runner.ActivePlayers.Count() == 2)
        {
            if (runner.IsServer) // ✅ seul le host lance la nouvelle scène
            {
                //runner.SetActiveScene("GameScene"); // Nom de ta scène de jeu
            }
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"👤 Joueur {player.PlayerId} a quitté");
        UpdatePlayerList();
    }

 public void UpdatePlayerList()
    {
        if (runner == null) return;

        // 1️⃣ On récupère les joueurs
        var allPlayers = runner.ActivePlayers.ToList();

        // 2️⃣ On force mon joueur en premier
        var orderedPlayers = allPlayers
            .OrderByDescending(p => p == runner.LocalPlayer) // true = 1, false = 0 → donc local en premier
            .ThenBy(p => p.PlayerId) // les autres triés par ID (optionnel)
            .ToList();

        // 3️⃣ On remplit les textes
        for (int i = 0; i < playerTexts.Length; i++)
        {
            if (i < orderedPlayers.Count)
            {
                var player = orderedPlayers[i];

                // ⚠️ Ici j’affiche juste l’ID → mais tu peux mettre un pseudo stocké dans un dictionnaire
                playerTexts[i].text = (player == runner.LocalPlayer) 
                    ? $"Moi : Joueur {player.PlayerId}" 
                    : $"Joueur {player.PlayerId}";
            }
            else
            {
                // Si pas assez de joueurs → on vide
                playerTexts[i].text = "Recherche de joueurs...";
            }
        }
    }

    public void OnClickStartGame()
    {
        StartGameScene();

        /*
        // Si le nombre de joueurs connectés = PlayerCount, on lance la scène de jeu
        if (runner.ActivePlayers.Count() == 10)
        {
            StartGameScene();
        }
        */
    }

    void StartGameScene()
    {
        if (runner.IsSceneAuthority)
        {
            runner.LoadScene(SceneRef.FromIndex(2), LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("INetworkSceneManager non trouvé sur le runner !");
        }
    }

    // === Callbacks requis par Fusion (on peut les laisser vides si inutiles) ===
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }
}

