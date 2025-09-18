using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UserSessionInfoItem : MonoBehaviour
{
    public TextMeshProUGUI PlayerNicknameText;
    public Image icon;

    private bool IsSearchingPlayers = true;

    private void Start()
    {

    }

    private void Update()
    {
        if (IsSearchingPlayers)
        {
            icon.transform.Rotate(Vector3.forward * -400 * Time.deltaTime);   
        }
    }
}
