using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class FusionLauncher : MonoBehaviour
{
    public NetworkRunner runnerPrefab;
    private NetworkRunner runner;

    void Awake()
    {
        if (runner == null)
        {
            runner = Instantiate(runnerPrefab);
            DontDestroyOnLoad(runner.gameObject);
        }
    }

    async void Start()
    {
        if (runner == null)
        {
            runner = Instantiate(runnerPrefab);
            DontDestroyOnLoad(runner.gameObject);
        }   

        // Récupérer les datas

        // Exemple : connexion au lobby
        var result = await runner.JoinSessionLobby(SessionLobby.ClientServer);
        if (result.Ok) Debug.Log("✅ Connecté au lobby !");
    }
}
