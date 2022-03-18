using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InspectController : MonoBehaviour
{
    public DialougeTrigger Dt;
    public whatsInside wt;
    string insideItem = "";
    // Start is called before the first frame update
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
        if (other.gameObject.tag == "Player")
        {
            if (Input.GetKey(KeyCode.C))
            {
                insideItem = wt.inside;
                Dt.dialouge.sentences[0] = insideItem;
                Dt.TriggerDialouge();
            }

        }
    }
}
