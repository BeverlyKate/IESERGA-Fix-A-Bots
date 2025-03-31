using UnityEngine;
using UnityEngine.UI;

public class EnableDisable : MonoBehaviour
{
    public Button resetButton;  // Reference to the main camera

    void OnEnable()
    {
        // Get the button's renderer component to control visibility
        resetButton.gameObject.SetActive(true);
    }

    void OnDisable()
    {
        resetButton.gameObject.SetActive(false);
    }

  
}
