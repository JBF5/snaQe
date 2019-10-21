using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        SnakeController sc = collision.gameObject.GetComponent<SnakeController>();
        if (sc != null)
        {
            sc.Eat();
        }

        transform.position = GameMaster.GetRandomOpenSpace();
    }
}
