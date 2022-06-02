using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class popupManager : MonoBehaviour
{
    // Start is called before the first frame update

    public Animator animator;
    public TMP_Text info;
    NPCController npc;
    public TMP_InputField input;

    void Start()
    {
        
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
                    npc = hit.collider.GetComponent<NPCController>();
                    info.text = "Current Activity: " + npc.activity;

                }
            }
        }

        if (npc != null) 
        {
            info.text = "Current Activity: " + npc.activity;
        }
    }

    public void OPEN() 
    {
        animator.SetBool("IsOpen", true);
    }

    public void CLOSE() 
    {
        animator.SetBool("IsOpen", false);
    }

    public void CHANGEACTIVITY() 
    {
        if (input.text == null) 
        {
            return;
        }
        string activity = input.text;
        Debug.Log("Activity");
        if (activity.Length == 1) 
        {
            int number = 0;
            int.TryParse(activity, out number);
            if (npc != null) 
            {
                npc.activity = number;
            }
        }
    }
}
