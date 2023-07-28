using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinGenerator : MonoBehaviour
{

    private int amountOfCoins;
    [SerializeField]private GameObject coinPrefab;

    [SerializeField] private int minCoins;
    [SerializeField] private int maxCoins;


    // Start is called before the first frame update
    void Start()
    {
        amountOfCoins = Random.Range(minCoins, maxCoins);


        int additionalOffset = amountOfCoins / 2;

        for (int i = 0; i < amountOfCoins; i++)
        {
            Vector3 offset = new Vector2(i-additionalOffset, 0);
            Instantiate(coinPrefab, transform.position + offset, Quaternion.identity, transform);
        }


    }
}
