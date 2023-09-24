using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    // // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision detected at position: " + transform.position);

        if (!other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }


        GameObject[] obstacleObjects = GameObject.FindGameObjectsWithTag("Obstacle");

        foreach (GameObject obstacleObject in obstacleObjects)
        {
            if (obstacleObject == other.gameObject)
            {
                Destroy(other.gameObject);
                break; 
            }
        }
        
    }
}
