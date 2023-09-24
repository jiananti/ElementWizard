using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerControl : MonoBehaviour
{
    // List to store references to all tokens.
    private List<Transform> tokens = new List<Transform>(); 
    
    // List to store references to all keys.
    private List<Transform> keys = new List<Transform>(); 
    
    // List to store references to all doors.
    private List<Transform> doors = new List<Transform>();

    // List to store references to all grounds.
    private List<Transform> grounds = new List<Transform>();
    
    // List to store references to all enemeys.
    //private List<Transform> enemies = new List<Transform>();

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
    
    // to record if player is hurted
    private bool hurted = false;
    
    private Renderer playerRenderer;
    
    // to record player's original color;
    private Color originalColor;
    
    // total blink times when player hurted
    private int blinkCount = 3;
    
    // the duration between two blink
    private float blinkDuration = 0.8f;
    
    // blink color's alpha value
    float blinkAlpha = 0.7f;
    
    // store the horizontal input 
    private float horizontalInput;
    
    public UnityEngine.UI.Text resourcetext;
    public GameObject Ice;
    public GameObject Fire;
    public GameObject canvas;
    public float fireSpeed = 10f;
    private Vector3 moveDirection ;
    
    // ice disappear time
    public float iceDisappearTime = 9f; 

    // Start is called before the first frame update
    void Start()
    {
        canvas.SetActive(false);
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
        
        // Get player original color
        playerRenderer = GetComponent<Renderer>();
        originalColor = playerRenderer.material.color;
        /*
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObject in enemyObjects)
        {
            enemies.Add(enemyObject.transform);
        }
        */
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
        CraftFire();
        resourcetext.text = "Resource: " + resource;
    }
    
    // player move left or right by pressing horizontal keys like A D lA RA
    void HorizontalMove()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(horizontalInput, 0f, 0f) * speed * Time.deltaTime;
        if (horizontalInput !=0)
        {
            moveDirection = new Vector3(horizontalInput, 0f, 0f);
            moveDirection = moveDirection.normalized;
        }
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

            // 启动协程来定时销毁冰块
            StartCoroutine(DestroyIceAfterTime(iceInstance));

        }
    }
    // 协程来定时销毁冰块
    IEnumerator DestroyIceAfterTime(GameObject iceInstance)
    {
        yield return new WaitForSeconds(iceDisappearTime);

        if (iceInstance != null)
        {
            Destroy(iceInstance);
        }
    }
    // player craft fire()
    void CraftFire()
    {
        if ((Input.GetKeyDown(KeyCode.Space )) && (resource > 0))
        {

            Vector3 spawnPosition = transform.position;

            GameObject fireInstance = Instantiate(Fire, spawnPosition + moveDirection, Quaternion.identity);

            Transform fireTransform = fireInstance.transform;

            fireTransform.forward = moveDirection;

            Rigidbody2D fireRigidbody2D = fireInstance.GetComponent<Rigidbody2D>();

            fireRigidbody2D.velocity = moveDirection * fireSpeed;
            Debug.Log("Fire Direction: " + moveDirection);

            fireInstance.transform.eulerAngles = Vector3.zero;
            
            resource -= 1;
        }
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
            // Find the nearest door to the player.
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
    
    // Function to detect collision with enemies and goal
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision involves an object with the "Enemy" tag.
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (hurted == false)
            {
                // Perform actions when the player collides with an enemy.
                Debug.Log("Player collided with an enemy!" + hurted);
                hurted = !hurted;
                if(resource>0)
                    resource--;
                Debug.Log("Token Resource: " + resource);
                StartCoroutine(BlinkPlayerColor());
                
            }
            
        }
        // Check if the collision involves an object with the "Enemy" tag.
        if (collision.gameObject.CompareTag("Goal"))
        {
            Debug.Log("Game Ends!");
            canvas.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    
    private IEnumerator BlinkPlayerColor()
    {
        Debug.Log("Player is hurted!" + hurted);
        
        for (int i = 0; i < blinkCount; i++)
        {
            // Blink In (Gradual Change to Blink Color)
            float elapsedTime = 0f;

            while (elapsedTime < blinkDuration)
            {
                // Calculate the interpolation value (0 to 1) based on elapsed time
                float t = elapsedTime / blinkDuration;

                // Interpolate between the original color and blink color (changing alpha)
                Color lerpedColor = Color.Lerp(originalColor, new Color(originalColor.r, originalColor.g, originalColor.b, blinkAlpha), t);

                // Set the player's color with the interpolated color
                playerRenderer.material.color = lerpedColor;

                // Update elapsed time
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // Blink Out (Gradual Change Back to Original Color)
            elapsedTime = 0f;

            while (elapsedTime < blinkDuration)
            {
                // Calculate the interpolation value (0 to 1) based on elapsed time
                float t = elapsedTime / blinkDuration;

                // Interpolate between the blink color and original color (changing alpha)
                Color lerpedColor = Color.Lerp(new Color(originalColor.r, originalColor.g, originalColor.b, blinkAlpha), originalColor, t);

                // Set the player's color with the interpolated color
                playerRenderer.material.color = lerpedColor;

                // Update elapsed time
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            // Restore the original color
            playerRenderer.material.color = originalColor;

            yield return new WaitForSeconds(0.2f); // Optional: Add a small delay between blinks
        }

        hurted = !hurted;
    }
    
}
