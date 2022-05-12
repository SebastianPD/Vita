using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Step;
using Step.Interpreter;
using System;

public class NPCController : MonoBehaviour
{
    //TODO have NPC interact with each other
    // Start is called before the first frame update




    public DialougeTrigger Dt;
    //public StepEngine ST;

    public string[] PlayerActions = {"sit", "item"};

    //AI Movement

    public float timer;
    public float activityTimer;

    private float OldTimer;
    public float OldactivityTimer;

    private int Direction;

    private int OldDirection;

    public float speed;
    float oldspeed;

    private Rigidbody2D _rb;

    public double sights;


    //Inventory
    public List<GameObject> children = new List<GameObject>();

    public int inventoryLimit;
    public int currentInventory;
    public List<string> inventoryList = new List<string>();
    int ItemListPointer = 0;

    //TODO string array with varous feelings? give different personalities. Choose what actions to have?

    Module mod;

    public int activity;

    public int exposedArea = 1;
    void Start()
    {
        activity = 0;
        _rb = GetComponent<Rigidbody2D>();
        OldTimer = timer;
        OldactivityTimer = activityTimer;
        Direction = UnityEngine.Random.Range(0, 5);
        OldDirection = Direction;
        oldspeed = speed;

        //start step
        mod = new Module("Module");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //check if the queue of sentences has changed. If they have then update
        UpdateST();
        
        UpdateColor();
        //checkArea();
        Action();
        Sense();
        movement();
    }

    void Action()
    {
        activityTimer = activityTimer - Time.deltaTime;
        if (activityTimer < 0)
        {
            
            activity = UnityEngine.Random.Range(0, 5);

   
            
            activityTimer = OldactivityTimer;

            switch (activity)
            {
                case 1:
                    Debug.Log("1");
                    checkArea();

                    break;
                case 2:
                    Debug.Log("2");
                    Inventory();
                    //insert code here

                    break;
                case 3:
                    Debug.Log("3");
                    //insert code here
                    //
                    break;
                default:
                    Debug.Log("default");
                    break;
            }


        }

       

    }

    void checkArea() 
    {
        //If there is an item walk towards it.
        Vector2 Location = new Vector2(transform.position.x, transform.position.y);
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 5);

        Collider2D x = col[0];

        foreach (Collider2D item in col)
        {
            if (item.tag == "Thing")
            {
                x = item;
            }
        }

        

        Debug.Log(Location.x);

        if (x.tag == "Thing")
        {
            Vector3 ItemLoc = x.transform.position;
          
            if (ItemLoc.x > Location.x - sights && ItemLoc.x < Location.x + sights && ItemLoc.y > Location.y)
            {
                //Debug.Log("item above");
               // timer = OldTimer;
                Direction = 1;
               
            }

            if (ItemLoc.x > Location.x - sights && ItemLoc.x < Location.x + sights && ItemLoc.y < Location.y)
            {
              //  Debug.Log("item below");
              //  timer = OldTimer;
                Direction = 2;
            }

            if (ItemLoc.y < Location.y - sights && ItemLoc.y > Location.y + sights && ItemLoc.x < Location.x)
            {
            //    Debug.Log("item left");
               // timer = OldTimer;
                Direction = 3;
            }

            if (ItemLoc.y < Location.y - sights && ItemLoc.y > Location.y + sights && ItemLoc.x > Location.x)
            {
              //  Debug.Log("item right");
              //  timer = OldTimer;
                Direction = 4;
            }

        }

    }

    void Sense() 
    {

        Collider2D[] npcArr = Physics2D.OverlapCircleAll(transform.position, exposedArea, LayerMask.GetMask("Default"));


        foreach (Collider2D item in npcArr)
        {
            if (item.gameObject.tag == "NPC")
            {
                if (item.gameObject.transform.position == transform.position) 
                {
                    continue;
                }
                //Change Activity
               // Debug.Log("Theres a npc near me");
            }

            if (item.gameObject.tag == "Player")
            {
              //  Debug.Log("Theres a player near me");
            }

        }
    }


    void EmergencyChange()
    {

        OldDirection = Direction;
        Direction = UnityEngine.Random.Range(0, 5);

        while (Direction == OldDirection)
        {
            Direction = UnityEngine.Random.Range(0, 5);
        }

        if (Direction == 0)
        {
            _rb.velocity = new Vector2(0, 0);

        }

        if (Direction == 1)
        {
            _rb.velocity = new Vector2(0, speed);

        }
        if (Direction == 2)
        {
            _rb.velocity = new Vector2(0, -speed);

        }
        if (Direction == 3)
        {
            _rb.velocity = new Vector2(-speed, 0);

        }
        if (Direction == 4)
        {
            _rb.velocity = new Vector2(speed, 0);
        }
    }


    void movement()
    {
        //Check Movement
        timer = timer - Time.deltaTime;
        if (timer <= 0)
        {
            OldDirection = Direction;
            Direction = UnityEngine.Random.Range(0, 5);

            while (Direction == OldDirection)
            {
                Direction = UnityEngine.Random.Range(0, 5);
            }
            timer = OldTimer;


        }



        if (Direction == 0)
        {
            _rb.velocity = new Vector2(0, 0);

        }

        if (Direction == 1)
        {
            _rb.velocity = new Vector2(0, speed);

        }
        if (Direction == 2)
        {
            _rb.velocity = new Vector2(0, -speed);

        }
        if (Direction == 3)
        {
            _rb.velocity = new Vector2(-speed, 0);

        }
        if (Direction == 4)
        {
            _rb.velocity = new Vector2(speed, 0);
        }
    }
    //UPDATE ST checks step to see if the play has done any possible player actions
    void UpdateST() 
    {
        int counter = 0;

            foreach (string item in PlayerActions)
            {
                bool x = MakeACall(item);

                if (x)
                {
                     Dt.dialouge.sentences[counter] = "you have " + item;
                
                }
                else 
                {
                    Dt.dialouge.sentences[counter] = "you have NOT " + item;

                
             }

            counter++;
        }
        
    
    }

    void UpdateColor()
    {
        int counter = 0;
        SpriteRenderer m_SpriteRenderer = GetComponent<SpriteRenderer>();
      
            bool x = MakeACall("sit");

            if (x)
            { 
                m_SpriteRenderer.color = Color.blue;
            }
            else
            {

                 m_SpriteRenderer.color = Color.green;

                }

            counter++;
           
    }

    //MODULE STUFF
    public void MakeModule(string x)
    {
        mod.AddDefinitions(x);
       
    }

    public bool MakeACall(string x)
    {
        try
        {
            mod.ParseAndExecute("[PlayerAction " + x + "]");
            //mod.CallPredicate("PlayerAction", c# object) to document objects.


            return true;
        }
        catch
        {

            return false;
        }


    }


    private void OnTriggerStay2D(Collider2D other)
    {
        //if the player interacts with them activate dialouge box
        if (other.gameObject.tag == "Player")
        {
            speed = 0;
            if (Input.GetKey(KeyCode.X))
            {
                Dt.TriggerDialouge();
            }

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //if the player interacts with them activate dialouge box
        if (other.gameObject.tag == "Player")
        {
            speed = oldspeed;

        }
    }

    void Inventory()
    {
        if (currentInventory > 0)
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
            return;
        }
        else
        {
            ItemListPointer++;
            ItemListPointer = ItemListPointer % children.Count;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "Thing" || collision.gameObject.tag == "Thing2"))
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

                MakeModule("Picked up:");

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
        // EmergencyChange();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
       // EmergencyChange();
    }
}
