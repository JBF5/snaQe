using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBody : MonoBehaviour
{
    private Rigidbody2D rb;

    public Queue<Vector2> qPastPos;
    public SnakeBody front;
    public SnakeBody back;
    // Start is called before the first frame update
    void Start()
    {
        qPastPos = new Queue<Vector2>();

        rb = GetComponent<Rigidbody2D>();
    }

    public void GameStep()
    {
        if (front != null)
        {
            front.GameStep();
            if (front.qPastPos.Count == 2)
            {
                Vector2 curPos = front.qPastPos.Dequeue();
                transform.position = (curPos);
                qPastPos.Enqueue(curPos);
            } else
            {
                front.qPastPos.Clear();
            }
        }
    }
}
