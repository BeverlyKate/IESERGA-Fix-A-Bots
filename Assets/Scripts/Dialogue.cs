using UnityEngine;


[System.Serializable]
public class Dialogue
{
    public Sprite sprite;

    public string name;

    [TextArea(3, 10)]
    public string[] sentences;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
