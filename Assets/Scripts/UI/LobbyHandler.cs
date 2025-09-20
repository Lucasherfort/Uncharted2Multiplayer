using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyHandler : MonoBehaviour
{
    public GameObject LobbyUI;
    public GameObject MatchMakingUI;  
    public TextMeshProUGUI PlayerNameTxt;

    private UsersSessionListHandler usersSessionListHandler;

    private void Awake()
    {
        usersSessionListHandler = GetComponentInChildren<UsersSessionListHandler>(true);
    }

    private void Start()
    {
        string pseudo = GenerateRandomPseudo();
        PlayerNameTxt.text = pseudo;
    }

    public void OnJoinGameClicked()
    {
        LobbyUI.SetActive(false);
        MatchMakingUI.SetActive(true);

        // Crée la data du joueur
        // TODO info de la BDD

        // Sérialise en JSON et stocke dans le GameManager

        /*
        string json = JsonUtility.ToJson(playerData);
        byte[] connectionToken = System.Text.Encoding.UTF8.GetBytes(json);
        GameManager.instance.SetConnectionToken(connectionToken);
        */

        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();
        networkRunnerHandler.OnJoinLobby();
    }

    private string GenerateRandomPseudo()
    {
        string[] prefixes = { "Dark", "Light", "Fire", "Shadow", "Ice", "Storm", "Epic", "Crazy", "Swift", "Mighty" };
        string[] suffixes = { "Wolf", "Dragon", "Ninja", "Knight", "Wizard", "Hunter", "Fox", "Lion", "Phantom", "Gamer" };

        string prefix = prefixes[Random.Range(0, prefixes.Length)];
        string suffix = suffixes[Random.Range(0, suffixes.Length)];
        int number = Random.Range(10, 9999);

        return prefix + suffix + number;
    }
}
