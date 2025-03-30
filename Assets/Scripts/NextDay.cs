using UnityEngine;
using UnityEngine.SceneManagement;

public class NextDay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private int currentDay;

    public void TriggerNextDay()
    {
        SceneManager.LoadScene(currentDay);
    }
}
