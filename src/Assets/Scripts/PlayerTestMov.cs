using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestMov : MonoBehaviour
{
    public float speed;
    public Player player;
    
    private Rigidbody2D rb;
    public Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.A))
        {
            movement.x += -1f;
            movement.y += 0.5f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            movement.x += 1f;
            movement.y += 0.5f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            movement.x += -1f;
            movement.y += -0.5f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movement.x += 1f;
            movement.y += -0.5f;
        }
    }

    private void FixedUpdate()
    {
        if (player.canMove)
            rb.MovePosition(rb.position + speed * Time.fixedDeltaTime * movement);
    }
}