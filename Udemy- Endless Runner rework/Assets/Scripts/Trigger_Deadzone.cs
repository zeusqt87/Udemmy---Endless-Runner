using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Deadzone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            GameManager.instance.RestartLevel();
    }
}
