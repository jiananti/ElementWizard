using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2.0f;
    public float movementRange = 5.0f; // Adjust this range as needed

    public bool isMovingRight = true;
    private Vector3 startingPosition;
    private Vector3 leftLimit;
    private Vector3 rightLimit;

    private void Start()
    {
        startingPosition = transform.position;
        leftLimit = startingPosition - new Vector3(movementRange / 2, 0, 0);
        rightLimit = startingPosition + new Vector3(movementRange / 2, 0, 0);
    }

    private void Update()
    {
       // Move the enemy within the specified range
        if (isMovingRight)
        {
            transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
            if (transform.position.x >= rightLimit.x)
            {
                isMovingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
            if (transform.position.x <= leftLimit.x)
            {
                isMovingRight = true;
            }
        }
    }
	
	// Public property for isMovingRight
    public bool IsMovingRight
    {
        get { return isMovingRight; }
        set { isMovingRight = value; }
    }
}
