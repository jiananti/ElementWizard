using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightControl : MonoBehaviour
{  
    private GameObject enemy;
    private Transform enemyTransform; 
    private Vector3 enemyPosition;

    public GameObject Enemy;
    private float totalmass = 0f;
    private int isenemy = 1;
    
    
    void Start()
    {
        //Record the original location of enemy
        enemy = GameObject.FindWithTag("Enemy");
        enemyTransform = enemy.transform; 
        enemyPosition = enemyTransform.position; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        Rigidbody2D otherRigidbody = other.GetComponent<Rigidbody2D>();
        float mass = otherRigidbody.mass;
        totalmass += mass;
        Debug.Log("totalmass: " + totalmass);

        if(totalmass > 2 && isenemy == 1)
        {
            StartCoroutine(DestroyEnemyWithDelay());
        }


    }

    private IEnumerator DestroyEnemyWithDelay()
    {
        //wait
        yield return new WaitForSeconds(0.5f);

        GameObject enemy = GameObject.FindWithTag("Enemy");
        if (enemy != null)
        {
            Destroy(enemy);
            isenemy = 0;
        }
        else
        {
            Debug.LogWarning("No enemy found with the 'Enemy' tag.");
        }
    }
   

    private void OnTriggerExit2D(Collider2D other)
    {
        
        Rigidbody2D otherRigidbody = other.GetComponent<Rigidbody2D>();
        float mass = otherRigidbody.mass;
        totalmass -= mass;
        Debug.Log("totalmass: " + totalmass);

        if (totalmass < 2 && isenemy == 0)
        {
            GameObject newEnemy = Instantiate(Enemy, enemyPosition, Quaternion.identity);
            isenemy = 1;
        }


    }
}
