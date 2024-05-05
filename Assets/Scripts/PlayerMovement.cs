using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] new Rigidbody rigidbody;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpHeight;

    private Vector2 axis = new Vector2();

    private void Start()
    {
        
    }

    private void Update()
    {
        if (GameManager.singleton.pause)
            return;

        if (UnityEngine.Input.GetButtonDown("Map"))
        {
            UIManager.singleton.ToggleMap();
        }
        Input();
        Jump();

    }

    private void FixedUpdate()
    {
        if (GameManager.singleton.pause)
            return;

        UIManager.singleton.UpdateMap(transform.position);

        Move();

    }

    private void Input()
    {
        axis.x = UnityEngine.Input.GetAxis("Horizontal");
        axis.y = UnityEngine.Input.GetAxis("Vertical");
    }

    private void Jump()
    {
        if (UnityEngine.Input.GetButton("Jump") && OnGround())
        {
            rigidbody.AddForce(Vector3.up * jumpHeight);
        }
    }

    private void Move()
    {
        Vector3 forward = axis.y * transform.forward;
        Vector3 right = axis.x * transform.right;

        float averageMagnitude = Mathf.Clamp(Mathf.Abs(axis.y) + Math.Abs(axis.x), 0f, 1f) / 2;

        Vector3 velocity = (forward + right).normalized * averageMagnitude *(float)GameManager.singleton.output["SPD"] * moveSpeed * Time.deltaTime;
        velocity = new Vector3(velocity.x, rigidbody.velocity.y, velocity.z);
        rigidbody.velocity = velocity;
    }

    private bool OnGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.05f);
    }
}
