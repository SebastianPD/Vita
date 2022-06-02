using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Step;
using Step.Interpreter;
using System;
using Pathfinding;

public class NPCController : MonoBehaviour
{
    //TODO have NPC interact with each other
    // Start is called before the first frame update
    
    int wantsit; //flag if NPC wants to sit
    public bool wantitem; // flag if NPC wants an item
    public DialougeTrigger Dt; // for dialouge
    //public StepEngine ST;
    public bool IsSitting; // flag to check if it's sitting
    Vector3 OldLocation; //old location to save when the npc is sitting
    public string[] PlayerActions = { "sit", "item" }; // they can only sit and take items and this is used to interact with their step modules to see if they have taken an item.

    //AI Movement

    //amount of time to wait until next direction and next action respectivley. 
    public float timer;
    public float activityTimer;

    //keep track of old time for resets
    private float OldTimer;
    public float OldactivityTimer;

    public int Direction;

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


    //A* pathfinding
    public Transform target;
    //use speed variable listed above
    public float NextWaypoint; //how close it needs to be to a way point before moving

    Path path = null;
    public int currentWaypoint = 0;
    public bool reachedEndofPath = true;
    public float nextWaypointDistance = .15f;
    Seeker seeker;
    //use rigidbody _rb listed above

    Module mod; //STEP MODULE

    public int activity; //Keep track of what activity is being preformed. 

    public int exposedArea = 1;
    void Start()
    {
        wantitem = true;
        //wantsit = UnityEngine.Random.Range(0, 2);
        wantsit = 1;
        IsSitting = false;
        activity = 0;
        _rb = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        OldTimer = timer;
        OldactivityTimer = activityTimer;
        Direction = UnityEngine.Random.Range(0, 5);
        OldDirection = Direction;
        oldspeed = speed;
        InvokeRepeating("UpdatePath", 0f, .5f);
        //start step
        // Possible make a hive mind to keep track of all NPC IDs so NPCS can know which ones it interacted with.
        mod = new Module("Module");
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    bool ReachEndOfPath()
    {
        if (path == null)
        {
            
            reachedEndofPath = true;
            return true;
        }


        if (currentWaypoint >= path.vectorPath.Count)
        {
            
            reachedEndofPath = true;
            path = null;
            return true;
        }

        return false;

    }

    void UpdatePath()
    {
        if (target.parent == null) //means we need to get the object
        {

            if (seeker.IsDone()) //check if done with old path
            {
                seeker.StartPath(_rb.position, target.position, OnPathComplete); //function to use to start a path
                reachedEndofPath = false;
            }

        }
        else
        {
            ReachEndOfPath();
        }

    }

    //make functions that will maipulate the direction of said npc
    //check distance to see if we can go to next waypoint

    // Invokereapeating at a few seconds

    bool AvaliablePath()
    {
        if (path == null || reachedEndofPath)
        {
            return false;
        }

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.X))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2d = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2d, Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("click");
                Debug.Log(hit.transform.tag);
                if (hit.transform.tag == "NPC")
                {
                 
                        Dt.TriggerDialouge();
                    
                }
            }
        }

    }

    void FixedUpdate()
    {
        //check if the queue of sentences has changed. If they have then update
        UpdateST();
        UpdateColor();
        Action();
        Sense();
        movement();
    }




    void Action()
    {
        activityTimer = activityTimer - Time.deltaTime;
        if (activityTimer < 0 && !wantitem)
        {

            activity = UnityEngine.Random.Range(0, 6);

            activityTimer = OldactivityTimer;

            switch (activity)
            {
                case 1:
                    //Debug.Log("1");
                    //checks areas for near by items and will then head to thier direction. THe idea is to have the AI have a bias for interactable objects. 
                    checkArea();
                    break;
                case 2:
                    //Debug.Log("2");
                    //Code to let go of item in inventory
                    Inventory();
                    break;
                case 3:
                   // Debug.Log("3");
                    //lock on to a new target which would be a game object with an item tag
                    //first check if sitting
                    if (IsSitting)
                    {
                        wantsit = 0;
                        speed = oldspeed;
                        IsSitting = false;
                        gameObject.transform.position = OldLocation;
                        
                    }
                    wantitem = true;
                    NewTarget();
                    break;
                case 4:
                    // code to just remove AI from seat
                    if (IsSitting)
                    {
                        wantsit = UnityEngine.Random.Range(0, 2);
                        speed = oldspeed;
                        IsSitting = false;
                        gameObject.transform.position = OldLocation;
                    }
                    break;
                case 5:
                    wantsit = UnityEngine.Random.Range(0, 2);
                    break;
                default:
                    //Debug.Log("default");
                    break;
            }


        }

        if (activityTimer < -10) 
        {
            wantitem = false;
            path = null;
        }



    }

    void NewTarget()
    {
        GameObject[] arr = GameObject.FindGameObjectsWithTag("Thing");
        foreach (GameObject G in arr)
        {
            if (G.transform.parent == null) 
            {
                wantitem = true;
                target = GameObject.FindGameObjectWithTag("Thing").transform;
                UpdatePath();
                return;
            }
        }
        return;
        
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

        

       // Debug.Log(Location.x);

        if (x.tag == "Thing" || x.tag == "chair")
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

    void movement()
    {
        //Check Movement
        timer = timer - Time.deltaTime;
       

        if (AvaliablePath() && wantitem && currentWaypoint < path.vectorPath.Count)
        {
            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - _rb.position).normalized;

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (direction.x < 0f)
                {
                    Direction = 3;
                }
                else
                {
                    Direction = 4;
                }
            }
            else
            {
                if (direction.y < 0f)
                {
                    Direction = 2;
                }
                else
                {
                    Direction = 1;
                }
            }

            float distance = Vector2.Distance(_rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }

        }
        else if (timer <= 0)
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
                bool x = MakeSelfCall(item);

                if (x)
                {
                     Dt.dialouge.sentences[counter] = "I have " + item;
                
                }
                else 
                {
                    Dt.dialouge.sentences[counter] = "I have NOT " + item;

                
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
            mod.ParseAndExecute("[SelfAction " + x + "]");
            //mod.CallPredicate("PlayerAction", c# object) to document objects.


            return true;
        }
        catch
        {

            return false;
        } 

       


    }

    public bool MakeSelfCall(string x) 
    {
        try
        {
            mod.ParseAndExecute("[SelfAction " + x + "]");
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
                ItemListPointer = UnityEngine.Random.Range(0, children.Count);
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
            if (child.transform.tag != "Untagged") 
            {
                children.Add(child.gameObject);
            }
            
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
        if ((collision.gameObject.tag == "Thing" || collision.gameObject.tag == "Thing2") && wantitem)
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

                MakeModule("SelfAction item:");


                if (collision.transform == target.transform) 
                {
                    wantitem = false;
                    reachedEndofPath = true;
                    path = null;

                }
                

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

    private void OnCollisionStay2D(Collision2D collision)
    {
       


        if (collision.gameObject.tag == "chair")
        {
            if (!IsSitting && wantsit == 1)
            {
                OldLocation = gameObject.transform.position;
                Vector3 ChairLocation = collision.gameObject.transform.position;
                gameObject.transform.position = ChairLocation;
                oldspeed = speed;
                speed = 0;
                IsSitting = true;

                Collider2D[] npcArr = Physics2D.OverlapCircleAll(transform.position, exposedArea);

                foreach (Collider2D item in npcArr)
                {
                    if (item.gameObject.tag == "NPC")
                    {
                        item.gameObject.GetComponent<NPCController>().MakeModule("SelfAction sit:");
                    }

                }

                //ST.MakeModule("PlayerAction sit:");
            }

        }
    }
}
