using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TopDownplayerController : MonoBehaviour
{
    // Start is called before the first frame update
    private float direction;
    private float directionx;
    private float directiony;
    private Rigidbody2D _rb;
    public float speed;
    private float magnitude;
    private float Isboost; // running mechanic, User will run if button is pressed
    public Object ammo;

    public int ammunition; //ammo the user has
    public int health; //number of health points
    public Vector3 spawnPoint; // where the user will spawn and respawn

    private AudioSource source; //Audio source
    public AudioClip Damage; // sound effect to play when the user takes damage
    public AudioClip hit; //sound effect to play when user hits an enemy
    public AudioClip Shoot; //sound effect to play when user shoots
    public AudioClip healthGain; //sound effect to play when user gains health

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        magnitude = 1f;
        Isboost = 0f;
        //Set up spawnpoint
        spawnPoint = transform.position;
        //load save data, If there is no save data use the default values
        health = PlayerPrefs.GetInt("health", 3);
        ammunition = PlayerPrefs.GetInt("Ammo", 5);
        //prepare audio source
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        BOOST();
        Fire();

        if (health < 1)
        {
            SceneManager.LoadScene(((SceneManager.sceneCountInBuildSettings) - 1));
        }
    }

    void FixedUpdate()
    {
        Movement();

    }
    

    //MOVEMENT player will move to the given direction and their player model will rotate to that direction. If up arrow was pressed, then player will now face up
    void Movement()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _rb.velocity = new Vector2(0,  (magnitude + Isboost)*speed);
            direction = 0f;
            directiony = .5f;
            directionx = 0f;
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _rb.velocity = new Vector2(0, (magnitude + Isboost) * -speed);
            direction = 1f;
            directiony = -.5f;
            directionx = 0f;
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _rb.velocity = new Vector2((magnitude + Isboost) * -speed, 0);
            direction = 2f;
            directionx = -.5f;
            directiony = 0f;
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _rb.velocity = new Vector2((magnitude + Isboost) * speed, 0);
            direction = 3f;
            directionx = .5f;
            directiony = 0f;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    void BOOST() {
        //if button is pressed the user will move faster
        if (Input.GetKey(KeyCode.Z))
        {
            Isboost = 1;
        }
        else
        {
            Isboost = 0;
        }
    }

    void Fire()
    {
        //User can't use their 
        if (Input.GetKeyDown(KeyCode.X) && Isboost == 0f && ammunition > 0)
        {
            Object.Instantiate(ammo, new Vector3(transform.position.x + directionx, transform.position.y + directiony, direction), Quaternion.identity);
            ammunition--;
            source.PlayOneShot(Shoot, 1);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            health--;
            source.PlayOneShot(Damage, 1);
            transform.position = spawnPoint;
            
        }

        if (collision.gameObject.tag == "hazard")
        {
            health--;
            transform.position = spawnPoint;
            source.PlayOneShot(Damage, 1);
        }

        if (collision.gameObject.tag == "goal")
        {
            //Save current health points and refill ammo
            PlayerPrefs.SetInt("health", health);
            PlayerPrefs.SetInt("Ammo", 5);

        }

        if (collision.gameObject.tag == "HealthPack")
        {
            //Saves the restored health and ammo
            PlayerPrefs.SetInt("health", 3);
            PlayerPrefs.SetInt("Ammo", 5);

            //restore health and ammo
            health = 3;
            ammunition = 5;
            source.PlayOneShot(healthGain, 1);

        }

        if (collision.gameObject.tag == "bulletTag")
        {
            source.PlayOneShot(Damage, 1);
            respawn();
        }

        if (collision.gameObject.tag == "door")
        {
            source.PlayOneShot(hit, 1);
            
        }
    }

    void respawn(){
        transform.position = spawnPoint;
        health--;
    }


}
