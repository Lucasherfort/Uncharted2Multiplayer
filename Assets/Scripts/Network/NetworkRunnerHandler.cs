using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;
    NetworkRunner networkRunner;

    private void Awake()
    {
        NetworkRunner networkRunnerInScene = FindAnyObjectByType<NetworkRunner>();

        if (networkRunnerInScene != null)
        {
            networkRunner = networkRunnerInScene;
        }
    }

    private void Start()
    {
        if (networkRunner == null)
        {
            networkRunner = Instantiate(networkRunnerPrefab);

            networkRunner.name = "Network runner";

            if (SceneManager.GetActiveScene().name != "Lobby")
            {
                var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, "TsstSession", GameManager.instance.GetConnectionToken(), NetAddress.Any(), SceneRef.FromIndex(2));
            }

            Debug.Log($"Server NetworkRunner started");
        }
    }

    public void StartHostMigration(HostMigrationToken hostMigrationToken)
    {
        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "Network runner - Migrated";

        var clientTask = InitializeNetworkRunnerHostMigration(networkRunner, hostMigrationToken);

    }

    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null)
        {
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        return sceneManager;
    }

    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, string sessionName, byte[] connectionToken, NetAddress address, SceneRef scene)
    {
        var sceneManager = GetSceneManager(runner);

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = sessionName,
            CustomLobbyName = "OurLobbyID",
            SceneManager = sceneManager,
            ConnectionToken = connectionToken
        });
    }

    protected virtual Task InitializeNetworkRunnerHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        var sceneManager = GetSceneManager(runner);

        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            //GameMode = gameMode,
            //Address = address,
            //Scene = scene,
            //SessionName = "TestRoom",
            SceneManager = sceneManager,
            HostMigrationToken = hostMigrationToken,
            HostMigrationResume = HostMigrationResume,
            ConnectionToken = GameManager.instance.GetConnectionToken()
        });
    }

    private void HostMigrationResume(NetworkRunner runner)
    {
        foreach (var resumeNetworkObject in runner.GetResumeSnapshotNetworkObjects())
        {
            if (resumeNetworkObject.TryGetBehaviour<NetworkCharacterController>(out var characterController))
            {
                Vector3 pos = characterController.transform.position;
                Quaternion rot = characterController.transform.rotation;

                runner.Spawn(resumeNetworkObject,
                    position: pos,
                    rotation: rot,
                    onBeforeSpawned: (r, newNetworkObject) =>
                    {
                        newNetworkObject.CopyStateFrom(resumeNetworkObject);

                        if (resumeNetworkObject.TryGetBehaviour<HPHandler>(out HPHandler oldHPHandler))
                        {
                            HPHandler newHPHandler = newNetworkObject.GetComponent<HPHandler>();
                            newHPHandler.CopyStateFrom(oldHPHandler);

                            newHPHandler.skipSettingsStartValues = true;
                        }

                        if (resumeNetworkObject.TryGetBehaviour<NetworkPlayer>(out var oldNetworkPlayer))
                        {
                            FindObjectOfType<Spawner>().SetConnectionTokenMapping(oldNetworkPlayer.token, newNetworkObject.GetComponent<NetworkPlayer>());
                        }
                    });
            }
        }

        StartCoroutine(CleanUpHostMigrationCO());
    }

    IEnumerator CleanUpHostMigrationCO()
    {
        yield return new WaitForSeconds(5.0f);

        FindObjectOfType<Spawner>().OnHostMigrationCleanUp();
    }

    public void OnJoinLobby()
    {
        var clientTask = JoinLobby();
    }

    private async Task JoinLobby()
    {
        Debug.Log("JoinLobby started");

        // Lobby FFA pour chacun pour soit
        // Lobby TDM (Team Deathmatch) match à mort par équipes
        string lobbyID = "Lobby_FFA";

        var result = await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyID);

        if (!result.Ok)
        {
            Debug.Log($"Unable to join lobby {lobbyID}");
        }
        else
        {
            Debug.Log("JoinLobby ok");

            // TODO rechercher ou créer une session 
        }
    }

    public void CreateGame(string sessionName, string sceneName)
    {
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Host, sessionName, GameManager.instance.GetConnectionToken(), NetAddress.Any(), SceneRef.FromIndex(2));
    }

    public void JoinGame(SessionInfo sessionInfo)
    {
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.Client, sessionInfo.Name, GameManager.instance.GetConnectionToken(), NetAddress.Any(), SceneRef.FromIndex(2));
    }
}
