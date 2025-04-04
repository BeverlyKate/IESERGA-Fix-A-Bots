using UnityEngine;
using System;
using TMPro;

public class ClockManager : MonoBehaviour
{
    [SerializeField] private TMP_Text clockText;

    [SerializeField] private GameObject EndofDay;

    [SerializeField] private float timeInDay = 86400f;

    [SerializeField] private float timeScale = 2.0f;

    private int currentTime;

    private float elaspedTime;

    private int setMinute;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        elaspedTime = 10 * 3600f;
        EndofDay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        elaspedTime += Time.deltaTime * timeScale;
        elaspedTime %= timeInDay;
        UpdateTime();
        CheckWorkTime();
    }

    private void UpdateTime()
    {
        int hours = Mathf.FloorToInt(elaspedTime / 3600f);
        int minutes = Mathf.FloorToInt((elaspedTime - hours * 3600f) / 60f);

        if (minutes % 10 == 0)
        {
            setMinute = minutes;
        }

        string ampm = hours < 12 ? "AM" : "PM";
        hours = hours % 12;

        if(hours == 0)
        {
            hours = 12;
        }

        string clockString = string.Format("{0:00}:{1:00} {2}", hours, setMinute, ampm);

        clockText.text = clockString;
    }

    private void CheckWorkTime()
    {
        if(Mathf.FloorToInt(elaspedTime / 3600f)%12 == 5)
        {
            EndWorkTime();
        }
    }

    public void EndWorkTime()
    {
        EndofDay.SetActive (true);
        clockText.gameObject.SetActive(false);
    }
}
