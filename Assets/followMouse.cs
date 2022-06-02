using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followMouse : MonoBehaviour
{
    public float moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            if (Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse X") < 0)
            {
                transform.position -= new Vector3(Input.GetAxisRaw("Mouse X") * Time.deltaTime * moveSpeed,
                                                  Input.GetAxisRaw("Mouse Y") * Time.deltaTime * moveSpeed);
            }
        }
    }
}
