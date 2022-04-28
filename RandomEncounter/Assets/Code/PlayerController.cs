using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float speed;
    private float magnitude;
    public Vector3 spawnPoint; // where the user will spawn and respawn

    //public StepEngine ST;

    //Inventory
    Hashtable InventorySpace = new Hashtable();
    public int inventoryLimit;
    public int currentInventory;
    public List<string> inventoryList = new List<string>();
    int ItemListPointer = 0;
    string itemString = "";

    //Stats
    int Atk;
    int Def;
    int SAtk;
    int SDef;

    public int HP;
    public TMP_Text healthText;
    public TMP_Text InventoryText;

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

    public int exposedArea = 30;

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
        healthText.text = "Health: " + HP;
        InventoryText.text = "Inventory: empty";
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
        UpdateText();
    }

    void UpdateText() 
    {
        healthText.text = "Health: " + HP;
        InventoryText.text = "Inventory: " + itemString + InventorySpace[itemString];
    }
    void Abilities() 
    {
        if (Input.GetKey(KeyCode.C)) 
        {
            inventorySwitcher();
        }
    }

    void inventorySwitcher() 
    {
        if (inventoryList.Count == 0) 
        {
            itemString = "";
            return;
        }

        ItemListPointer++;
        ItemListPointer = ItemListPointer % inventoryList.Count;
        itemString = inventoryList[ItemListPointer];
    }

    void inventoryupdate() 
    {
        ItemListPointer = 0;
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
        if (Input.GetKey(KeyCode.Z) && IsSitting)
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
            try
            {
                GameObject prefab = Resources.Load("prefabs/" + itemString) as GameObject;
                Instantiate(prefab, new Vector3(transform.position.x + directionx, transform.position.y + directiony, direction), Quaternion.identity);
                InventorySpace[itemString] = (int)InventorySpace[itemString] - 1;
                if ((int)InventorySpace[itemString] == 0)
                {
                    InventorySpace.Remove(itemString);
                    inventoryList.Remove(itemString);
                }
                //InventorySpace.Clear();
                currentInventory--;
            }
            catch 
            {

                return;
            }
            
           
            
        }
    }

    void Inspect()
    {
       
       
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
        if ((collision.gameObject.tag == "Thing" || collision.gameObject.tag == "Thing2") && Input.GetKey(KeyCode.X))
        {
            if (InventorySpace.Count < inventoryLimit)
            {
                if (InventorySpace.ContainsKey(collision.gameObject.tag))
                {
                    InventorySpace[collision.gameObject.tag] = collision.gameObject.tag + 1;
                }
                else 
                {
                    InventorySpace.Add(collision.gameObject.tag, 1);
                    inventoryList.Add(collision.gameObject.tag);
                }
                inventoryupdate();
                currentInventory++;
                Destroy(collision.gameObject);

                //foreach lop to get all npcs in the are and tell them about this
                Collider2D[] npcArr = Physics2D.OverlapCircleAll(transform.position, exposedArea);

                foreach (Collider2D item in npcArr)
                {
                    if (item.gameObject.tag == "NPC") 
                    {
                       item.gameObject.GetComponent<NPCController>().MakeModule("PlayerAction item:");
                    }

                }

                //ST.MakeModule("PlayerAction item:");
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
            if (Input.GetKey(KeyCode.Space) && !IsSitting) 
            {
                OldLocation = gameObject.transform.position;
                Vector3 ChairLocation = other.gameObject.transform.position;
                gameObject.transform.position = ChairLocation;
                speed = 0;
                IsSitting = true;

                Collider2D[] npcArr = Physics2D.OverlapCircleAll(transform.position, exposedArea);

                foreach (Collider2D item in npcArr)
                {
                    if (item.gameObject.tag == "NPC")
                    {
                        item.gameObject.GetComponent<NPCController>().MakeModule("PlayerAction sit:");
                    }

                }

                //ST.MakeModule("PlayerAction sit:");
            }

        }

       

            //if we hit a dummy item then we copy dummy item in hash table and then we can make an instance of it. Keep it in hash table then remove from hash table
        }
}
