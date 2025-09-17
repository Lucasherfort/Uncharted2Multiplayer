using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUp
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]

    public static void InstantiatePrefabs()
    {
        Debug.Log("-- Instantiating objets --");

        GameObject[] prefabsToInstantiate = Resources.LoadAll<GameObject>("InstantiateOnLoad/");

        foreach (GameObject prefab in prefabsToInstantiate)
        {
            Debug.Log($"Creating {prefab.name}");

            GameObject.Instantiate(prefab);
        }

        Debug.Log("-- Instantiating objets done --");
    }
}
