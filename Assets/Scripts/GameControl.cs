using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControl : MonoBehaviour
{
    public static int Score = 0;
    public static float Health = 3;

    public Text TXTScore;
    public Text Timer_Display;
    public float Minutes;
    public float Seconds;
    public float Timer;
    public Slider HealthBar;

    public GameObject GameOver;

    void Start()
    {
        Score = 0;
        Health = 5;
        HealthBar.maxValue = Health;
        HealthBar.value = Health;
        GameOver.SetActive(false);
    }

    void Update()
    {
        HealthBar.value = Health;
        TXTScore.text = Score.ToString();
        Timer += Time.deltaTime;
        Minutes = Mathf.FloorToInt(Timer / 60);
        Seconds = Mathf.FloorToInt(Timer % 60);
        Timer_Display.text = string.Format("{0:00}:{1:00}", Minutes, Seconds);
        if (Health <= 0)
        {
            int i = 1;
            GameOver.SetActive(true);
            Time.timeScale = 0;
        }

    }

    public void BTNRestart()
    {
        Time.timeScale = 1;
        Score = 0;
        Health = 5;
        HealthBar.maxValue = Health;
        HealthBar.value = Health;
        GameOver.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ChangeHp(int val)
    {
        Health += val;
    }

    public void GetHp()
    {
        Debug.Log("Hp: " + Health);
    }
}
