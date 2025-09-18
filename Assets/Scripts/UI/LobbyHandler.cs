using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyHandler : MonoBehaviour
{
    public TMP_InputField inputField;

    private void Start()
    {
        if (PlayerPrefs.HasKey("PlayerNickname"))
        {
            inputField.text = PlayerPrefs.GetString("PlayerNickname");
        }
    }

    public void OnJoinGameClicked()
    {
        PlayerPrefs.SetString("PlayerNickname", inputField.text);
        PlayerPrefs.Save();

        GameManager.instance.playerNickName = inputField.text;

        NetworkRunnerHandler networkRunnerHandler = FindObjectOfType<NetworkRunnerHandler>();

        networkRunnerHandler.OnJoinLobby();
    }
}
