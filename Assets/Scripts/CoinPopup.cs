using UnityEngine;

public class CoinPopup : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 1.2f); // match animation length
    }
}
