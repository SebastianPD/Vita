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
    public List<GameObject> children = new List<GameObject>();

    public int inventoryLimit;
    public int currentInventory;
    public List<string> inventoryList = new List<string>();
    int ItemListPointer = 0;


    string itemString = "";

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
        Abilities();
        Inventory();
    }


    void FixedUpdate()
    {
        Movement();
       
        UpdateText();
    }

    void UpdateText() 
    {
        healthText.text = "Health: " + HP;
        InventoryText.text = "Inventory: " + itemString;
    }
    void Abilities() 
    {
        if (Input.GetKeyUp(KeyCode.C)) 
        {
            inventorySwitcher();
        }
    }

    void inventorySwitcher() 
    {
        if (children.Count == 0) 
        {
            ItemListPointer = 0;
            itemString = "";
            return;
        }

        ItemListPointer++;
        ItemListPointer = ItemListPointer % children.Count;
        itemString = children[ItemListPointer].gameObject.tag;
    }

    void inventoryupdate() 
    {
        children.Clear();

        Transform[] ts = gameObject.GetComponentsInChildren<Transform>();

        if (ts == null) 
        {
            return;
        }

        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        if (children.Count == 0)
        {
            ItemListPointer = 0;
            itemString = "";
            return;
        }
        else 
        {
            ItemListPointer++;
            ItemListPointer = ItemListPointer % children.Count;
            itemString = children[ItemListPointer].gameObject.tag;
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
        if (Input.GetKey(KeyCode.Z) && IsSitting)
        {
            speed = OldSpeed;
            IsSitting = false;
            gameObject.transform.position = OldLocation;
        }
    }

    void Inventory() 
    {
        if (Input.GetKeyDown(KeyCode.Z) && currentInventory > 0)
        {

            try
            {
                SpriteRenderer sp = children[ItemListPointer].GetComponent<SpriteRenderer>();
                BoxCollider2D box = children[ItemListPointer].GetComponent<BoxCollider2D>();
                sp.enabled = true;
                box.enabled = true;
                children[ItemListPointer].transform.parent = null;
                children.Remove(children[ItemListPointer]);
                currentInventory--;
                inventoryupdate();
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
            if (inventoryList.Count < inventoryLimit)
            {

                BoxCollider2D box = collision.gameObject.GetComponent<BoxCollider2D>();
                box.enabled = false;

                SpriteRenderer SR = collision.gameObject.GetComponent<SpriteRenderer>();
                SR.enabled = false;

                collision.gameObject.transform.parent = this.gameObject.transform;
                currentInventory++;
                inventoryupdate();


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
        }
}
