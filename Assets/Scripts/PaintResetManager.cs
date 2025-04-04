using UnityEngine;
using UnityEngine.UI;

public class PaintResetManager : MonoBehaviour
{
    [SerializeField]
    private GameObject resetBtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void EnableBtn()
    {
        resetBtn.SetActive(true);
    }

    public void DisableBtn()
    {
        resetBtn.SetActive(false);
    }
}
