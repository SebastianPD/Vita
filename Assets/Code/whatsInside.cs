using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class whatsInside : MonoBehaviour
{
    // Start is called before the first frame update
    public string inside = "";

    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        //if the player interacts with them activate dialouge box
        if (other.gameObject.tag == "Thing")
        {
            inside = "Thing";
            Debug.Log("Thing");

        }

        if (other.gameObject.tag == "Player")
        {
            inside = "Player";
            Debug.Log("Player");

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        inside = "";
    }



}
