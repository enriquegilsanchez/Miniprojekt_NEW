using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject Menu;
    [SerializeField] private GameControl controller;
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    public void Exit()
    {
        Application.Quit();
    }
    public void Resume()
    {

        Menu.SetActive(false);
        Time.timeScale = 1f;
        controller.GetComponent<GameControl>().MenuIsOpen = false;
}


}
