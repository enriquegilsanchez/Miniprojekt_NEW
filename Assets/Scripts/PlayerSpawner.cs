using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPrefab;
    private bool spawned = false;

    void Start()
    {
        if (!spawned)
            Instantiate(playerPrefab, transform.position, Quaternion.identity);
        spawned = true;
    }
}
