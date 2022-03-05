using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float speed;
    private float magnitude;
    public Vector3 spawnPoint; // where the user will spawn and respawn

    //Inventory
    Hashtable InventorySpace = new Hashtable();
    public int inventoryLimit;
    public int currentInventory;

    //Stats
    int Atk;
    int Def;
    int SAtk;
    int SDef;
    int HP;
    public int SenseRadius;

    //Chair interaction
    public bool IsSitting;
    float OldSpeed;
    Vector3 OldLocation;

    private AudioSource source;
    public AudioClip sound;

    //Direction Tracker
    private float direction;
    private float directionx;
    private float directiony;


    // Start is called before the first frame update
    void Start()
    {
        OldSpeed = speed;
        IsSitting = false;
        _rb = GetComponent<Rigidbody2D>();
        magnitude = 1f;
        //Set up spawnpoint
        spawnPoint = transform.position;
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }


    void FixedUpdate()
    {
        Movement();
        Inventory();
        Abilities();
    }

    void Abilities() 
    {
        if (Input.GetKey(KeyCode.C)) 
        {
            Sense(_rb.transform.position, SenseRadius);
        }
    }

    void Movement()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _rb.velocity = new Vector2(0, magnitude * speed);
            direction = 0f;
            directiony = .5f;
            directionx = 0f;
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _rb.velocity = new Vector2(0, magnitude * -speed);
            direction = 1f;
            directiony = -.5f;
            directionx = 0f;
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _rb.velocity = new Vector2(magnitude * -speed, 0);
            direction = 2f;
            directionx = -.5f;
            directiony = 0f;
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _rb.velocity = new Vector2(magnitude * speed, 0);
            direction = 3f;
            directionx = .5f;
            directiony = 0f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        if (Input.GetKey(KeyCode.Space) && IsSitting)
        {
            speed = OldSpeed;
            IsSitting = false;
            gameObject.transform.position = OldLocation;
        }
    }

    void Inventory() 
    {
        if (Input.GetKey(KeyCode.Z) && currentInventory > 0)
        {
            
            GameObject prefab = Resources.Load("prefabs/Thing") as GameObject;
            Instantiate(prefab, new Vector3(transform.position.x + directionx, transform.position.y + directiony, direction), Quaternion.identity);
            InventorySpace.Clear();
            currentInventory--;
        }
    }

    void Sense(Vector3 center, float radius)
    {
       
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(center, radius);

        foreach (var hitCollider in hitColliders)
        {
            Debug.Log(hitCollider.gameObject.tag);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "hazard")
        {
            transform.position = spawnPoint;
        }
        if (collision.gameObject.tag == "goal")
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);

            source.PlayOneShot(sound, 1);
        }

        

    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Thing" && Input.GetKey(KeyCode.X))
        {
            if (InventorySpace.Count < inventoryLimit)
            {
                InventorySpace.Add(collision.gameObject.tag, collision.gameObject);
                currentInventory++;
                Destroy(collision.gameObject);
            }

        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        
        if (other.gameObject.tag == "chair") 
        {
            if (Input.GetKey(KeyCode.X) && !IsSitting) 
            {
                OldLocation = gameObject.transform.position;
                Vector3 ChairLocation = other.gameObject.transform.position;
                gameObject.transform.position = ChairLocation;
                speed = 0;
                IsSitting = true;
            }

        }

       

            //if we hit a dummy item then we copy dummy item in hash table and then we can make an instance of it. Keep it in hash table then remove from hash table
        }
}
