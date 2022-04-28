using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    public PlayerController player;
    public bool controlPlayer;
    public bool createPlayer;
    public bool createNPC;
    public float offset;
    public float moveSpeed;
    private Camera myCamera;
    public float Zoom;
    // Start is called before the first frame update
    void Start()
    {
        Zoom = 5;
        myCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controlPlayer)
        { transform.position = new Vector3(player.transform.position.x + 2f * offset, player.transform.position.y + offset, transform.position.z); }

        if (Input.GetMouseButtonDown(1))
        {
            if (createNPC)
            {
                Debug.Log("Create");
                Vector3 WorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameObject prefab = Resources.Load("prefabs/NPC") as GameObject;
                Instantiate(prefab, new Vector3(WorldPosition.x, WorldPosition.y, 1), Quaternion.identity);
            }
        }

        if (!controlPlayer)
        {
            Zoom = Zoom + 5 * Input.GetAxis("Mouse ScrollWheel");
            myCamera.orthographicSize = Mathf.Clamp(Zoom, 2, 8); ;

        }

    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && !controlPlayer)
        {
            if (Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse X") < 0)
            {
                transform.position -= new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * moveSpeed,
                                                  Input.GetAxisRaw("Mouse Y") * Time.deltaTime * moveSpeed);
            }
        }

        

    }

    public void switchPositions() 
    {
        controlPlayer = !controlPlayer;
    }

    public void EnableNPCCreation() 
    {
        createNPC = !createNPC;
    }
}
