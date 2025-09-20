using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class UsersSessionListHandler : MonoBehaviour
{
    [SerializeField] private List<UserSessionInfoItem> userSessionInfoItems = new List<UserSessionInfoItem>();

    public void OnUpdatePlayersList(List<PlayerInfo> playerSessionDataList, PlayerRef localPlayer)
{
    // Trier pour mettre le joueur local en premier
    List<PlayerInfo> sortedList = new List<PlayerInfo>(playerSessionDataList);
    sortedList.Sort((a, b) =>
    {
        if (a.playerRef == localPlayer) return -1;
        if (b.playerRef == localPlayer) return 1;
        return 0;
    });

    // Mettre à jour l'affichage
    for (int i = 0; i < sortedList.Count; i++)
    {
        if (i < userSessionInfoItems.Count)
        {
            userSessionInfoItems[i].SetItemInfos(sortedList[i]);
        }
        else
        {
            Debug.LogWarning("Pas assez de UI items pour afficher tous les joueurs.");
        }
    }
}
}
