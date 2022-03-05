using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update

    private AudioSource source;
    public AudioClip song;
    public AudioClip buttonclick;
    void Start()
    {
        Screen.SetResolution(1280, 720, false);
        source = GetComponent<AudioSource>();
        source.PlayOneShot(song, 1);
    }

    public void ButtonPress()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        source.PlayOneShot(buttonclick, 1);
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void instructions()
    {
        int nextSceneIndex = SceneManager.sceneCountInBuildSettings - 1 ;
        source.PlayOneShot(buttonclick, 1);
        SceneManager.LoadScene(nextSceneIndex);
    }

    public void quit()
    {
        source.PlayOneShot(buttonclick, 1);
        Application.Quit();
    }

    public void PlayEasy()
    {
        source.PlayOneShot(buttonclick, 1);
        SceneManager.LoadScene(3);
    }
    public void PlayMedium()
    {
        source.PlayOneShot(buttonclick, 1);
        SceneManager.LoadScene(4);
    }
    public void PlayHard()
    {
        source.PlayOneShot(buttonclick, 1);
        SceneManager.LoadScene(5);
    }

    public void ReturntoMain()
    {
        source.PlayOneShot(buttonclick, 1);
        SceneManager.LoadScene(0);
    }
}
