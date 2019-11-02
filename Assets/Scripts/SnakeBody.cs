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

    public void MoveTo(Vector2 newPos)
    {
        Vector2 oldPos = transform.position;
        transform.position = newPos;

        if (back != null)
        {
            back.MoveTo(oldPos);
        }

        if (!(rb = GetComponent<Rigidbody2D>()).simulated)
        {
            rb.simulated = true;
        }
    }

    public void Remove()
    {
        if (back != null)
        {
            back.Remove();
        }
        Destroy(this.gameObject);
    }
}
