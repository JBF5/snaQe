/**
 * 
 * This Script handles the Apple Collisons
 * 
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    //Triggers on 2D Collision
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Getting instance of SnakeContoller Object to see if the collision is null
        SnakeController sc = collision.gameObject.GetComponent<SnakeController>();

        //If gameobject is not equal to null call Snake Controller eat method.
        if (sc != null)
        {
            sc.Eat();
        }

        //Move the apple is moved 
        transform.position = GameMaster.GetRandomOpenSpace();
    }
}
