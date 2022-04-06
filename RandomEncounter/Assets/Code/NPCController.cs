using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Step;
using Step.Interpreter;

public class NPCController : MonoBehaviour
{
    // Start is called before the first frame update

    public DialougeTrigger Dt;
    //public StepEngine ST;

    public string[] PlayerActions = {"sit", "item"};

    //AI Movement

    public float timer;

    private float OldTimer;

    private int Direction;

    private int OldDirection;

    public float speed;
    float oldspeed;

    private Rigidbody2D _rb;

    //give player a step module

    Module mod;


    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        OldTimer = timer;
        Direction = Random.Range(0, 5);
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

        //Check Movement
        timer = timer - Time.deltaTime;
        if (timer < 0)
        {
            OldDirection = Direction;
            Direction = Random.Range(0, 5);

            while (Direction == OldDirection)
            {
                Direction = Random.Range(0, 5);
            }
            timer = OldTimer;
        }
        movement(Direction);

    }

    void movement(int x)
    {
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
}
