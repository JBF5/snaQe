using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour
{
    bool eaten = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SnakeController sc = collision.gameObject.GetComponent<SnakeController>();
        if (sc != null)
        {
            sc.Eat();
            eaten = true;
        }

        int x = Random.Range(-4, 5);
        int y = Random.Range(-4, 5);
        Vector2 v2 = new Vector2(x, y);
        transform.position = v2;
    }
}
