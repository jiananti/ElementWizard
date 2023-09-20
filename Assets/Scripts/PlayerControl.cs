using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerControl : MonoBehaviour
{
    // List to store references to all tokens.
    private List<Transform> tokens = new List<Transform>(); 
    
    // List to store references to all keys.
    private List<Transform> keys = new List<Transform>(); 
    
    // List to stor references to all doors.
    private List<Transform> doors = new List<Transform>();

    // List to stor references to all grounds.
    private List<Transform> grounds = new List<Transform>();

    // player moving speed
    private float speed = 8.0f;
    
    // The distance player can gain the resource
    private float resourceDistance = 2.0f;
    
    // the distance player can pick the key
    private float keyDistance = 1.5f;
    
    // the distance player can open the door
    private float doorDistance = 2.0f;
    
    // resource counter: 0 beginning
    private int resource = 0;
    
    // key counter: 0 beginning
    private int key = 0;
    
    // store the horizontal input 
    private float horizontalInput;

    public GameObject Ice;
    
    // Start is called before the first frame update
    void Start()
    {   
        // find all tokens in the scene by tag
        GameObject[] tokenObjects = GameObject.FindGameObjectsWithTag("Token");
        foreach (GameObject tokenObject in tokenObjects)
        {
            tokens.Add(tokenObject.transform);
        }
        
        // find all keys in the scene by tag
        GameObject[] keyObjects = GameObject.FindGameObjectsWithTag("Key");
        foreach (GameObject keyObject in keyObjects)
        {
            keys.Add(keyObject.transform);
        }
        
        // find all doors in the scene by tag
        GameObject[] doorObjects = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject doorObject in doorObjects)
        {
            doors.Add(doorObject.transform);
        }

        // find all grounds in the scene by tag
        GameObject[] groundObjects = GameObject.FindGameObjectsWithTag("Ground");
        foreach (GameObject groundObject in groundObjects)
        {
            grounds.Add(groundObject.transform);
        }
        Debug.Log("Token Resource: " + resource);
        Debug.Log("Key: " + key);
        // Find the player GameObject by name.
        //player = GameObject.Find("Player");
        //tokenInstance = Instantiate(tokenPrefab, new Vector3(10.09f, 0.96f, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        HorizontalMove();
        GainResource();
        PickKeys();
        OpenDoor();
        CraftIce();
    }
    
    // player move left or right by pressing horizontal keys like A D lA RA
    void HorizontalMove()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(horizontalInput, 0f, 0f) * speed * Time.deltaTime;
        transform.Translate(movement);
        
    }
    
    // player craft ice
    void CraftIce()
    {
        
        Renderer iceRenderer = Ice.GetComponent<Renderer>();
        float iceWidth = iceRenderer.bounds.size.x;
        float iceHeight = iceRenderer.bounds.size.y;


        if ((Input.GetKeyDown(KeyCode.X)) && (resource > 0))
        {            
            GameObject player = GameObject.Find("Player");
            CapsuleCollider2D playerCollider = player.GetComponent<CapsuleCollider2D>();
            float left = transform.position.x - iceWidth / 2;
            float right = transform.position.x + iceWidth / 2;
            float bottom = playerCollider.bounds.min.y;
            float up = playerCollider.bounds.max.y + iceHeight;
          
            foreach (Transform groundObject in grounds)
            {
                BoxCollider2D gCollider = groundObject.GetComponent<BoxCollider2D>();
                float g_bottom = gCollider.bounds.min.y;
                float g_left = gCollider.bounds.min.x;
                float g_right = gCollider.bounds.max.x;
                if ((left > g_left && left < g_right || right > g_left && right < g_right) && g_bottom > bottom && g_bottom < up)
                {
                    return;
                }
            }



            Vector3 spawnPosition = transform.position;

            GameObject iceInstance = Instantiate(Ice, spawnPosition, Quaternion.identity);

            Vector3 offset = Vector3.up * iceInstance.transform.localScale.y;

            transform.position = iceInstance.transform.position + offset;

            resource = resource - 1;
      
        }
    }
    
    // player craft fire()
    void CraftFire()
    {
        
    }
    
    // player pick keys()
    void PickKeys()
    {
        if (keys.Count > 0)
        {
            Transform nearestKey = FindNearestKey();

            float distance = Vector2.Distance(transform.position, nearestKey.position);

            if (Input.GetKeyDown(KeyCode.E) && distance < keyDistance)
            {
                key++;
                Debug.Log("Key: " + key);
                
                keys.Remove(nearestKey);
                
                Destroy(nearestKey.gameObject);
            }
        }
    }
    
    // Function to find the nearest keys to the player
    Transform FindNearestKey()
    {
        Transform nearestKey = null;
        float shortestDistance = float.MaxValue;

        foreach (Transform key in keys)
        {
            float distance = Vector2.Distance(transform.position, key.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestKey = key;
            }
        }

        return nearestKey;
    }
    
    // player open doors
    void OpenDoor()
    {
        if (doors.Count > 0)
        {
            // Find the nearest token to the player.
            Transform nearestDoor = FindNearestDoor();
            float distance = Vector2.Distance(transform.position, nearestDoor.position);
            if (Input.GetKeyDown(KeyCode.F) && distance < doorDistance)
            {
                if (key > 0)
                {
                    key--;
                    Debug.Log("Key: " + key);
                    doors.Remove(nearestDoor);
                
                    Destroy(nearestDoor.gameObject);
                }
                else
                {
                    Debug.Log("You don't have the key to open the door!");
                }
            }
        }
    }
    
    // Function to find the nearest door
    Transform FindNearestDoor()
    {
        Transform nearestDoor = null;
        float shortestDistance = float.MaxValue;

        foreach (Transform door in doors)
        {
            float distance = Vector2.Distance(transform.position, door.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestDoor = door;
            }
        }

        return nearestDoor;
    }
    
    //player gain token resource
    void GainResource()
    {
        if (tokens.Count > 0)
        {
            // Find the nearest token to the player.
            Transform nearestToken = FindNearestToken();
            float distance = Vector2.Distance(transform.position, nearestToken.position);
            if (Input.GetKeyDown(KeyCode.Q) && distance < resourceDistance)
            {
                resource += 5;
                
                Debug.Log("Token Resource: " + resource);
                
                tokens.Remove(nearestToken);
                
                Destroy(nearestToken.gameObject);
            }
        }
    }
    
    // Function to find the nearest token to the player.
    Transform FindNearestToken()
    {
        Transform nearestToken = null;
        float shortestDistance = float.MaxValue;

        foreach (Transform token in tokens)
        {
            float distance = Vector2.Distance(transform.position, token.position);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestToken = token;
            }
        }

        return nearestToken;
    }

}
