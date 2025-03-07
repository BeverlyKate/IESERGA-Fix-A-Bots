using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputPattern : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private GameObject[] patternDisplay;
    private int i;


    void Start()
    {
        patternDisplay = GameObject.FindGameObjectsWithTag("InputObject");
        i = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pattern (int number)
    {
        if (i <= 3)
        {
            if (number == 1)
            {
                patternDisplay[i].GetComponent<Image>().color = new Color32(113, 255, 117, 255);
                patternDisplay[i].GetComponentInChildren<TextMeshProUGUI>().text = "1";
            }
            else if (number == 2) {
                patternDisplay[i].GetComponent<Image>().color = new Color32(76, 114, 255, 255);
                patternDisplay[i].GetComponentInChildren<TextMeshProUGUI>().text = "2";
            }else if(number == 3)
            {
                patternDisplay[i].GetComponent<Image>().color = new Color32(255, 98, 106, 255);
                patternDisplay[i].GetComponentInChildren<TextMeshProUGUI>().text = "3";
            }
            else if(number == 4)
            {
                patternDisplay[i].GetComponent<Image>().color = new Color32(246, 255, 94, 255);
                patternDisplay[i].GetComponentInChildren<TextMeshProUGUI>().text = "4";
            }

            i++;
        }
    }
}
