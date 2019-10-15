using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSnake : MonoBehaviour
{
    private SnakeController sc;

    void Start()
    {
        sc = GetComponent<SnakeController>();
    }

    void Update()
    {
        if (Input.GetAxis("Vertical") != 0)
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                sc.TurnTo(SnakeController.Compas.NORTH);
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                sc.TurnTo(SnakeController.Compas.SOUTH);
            }
        } else if (Input.GetAxis("Horizontal") != 0)
        {
            if (Input.GetAxis("Horizontal") > 0)
            {
                sc.TurnTo(SnakeController.Compas.EAST);
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                sc.TurnTo(SnakeController.Compas.WEST);
            }
        }
    }
}
