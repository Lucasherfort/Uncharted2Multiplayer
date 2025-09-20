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

    public void SetItemInfos(PlayerInfo data)
    {
        IsSearchingPlayers = false;
        icon.transform.rotation = Quaternion.identity;
        PlayerNicknameText.text = (string)data.username;
        icon.sprite = Resources.Load<Sprite>("BadgeLevel/"+data.xp);
    }

    public void RemoveItemInfos()
    {
        IsSearchingPlayers = true;
    }
}
