using UnityEngine;

public class EmployeeCoinSpawner : MonoBehaviour
{
    public GameObject coinPopupPrefab;
    public float interval = 1f;

    void Start()
    {
        InvokeRepeating("SpawnCoin", 1f, interval);
    }

    void SpawnCoin()
    {
        if (coinPopupPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0f, 1.2f, 0f);
            Instantiate(coinPopupPrefab, spawnPos, Quaternion.identity);
        }
    }
}
