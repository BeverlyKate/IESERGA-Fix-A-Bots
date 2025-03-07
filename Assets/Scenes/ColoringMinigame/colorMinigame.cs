using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class colorMinigame : MonoBehaviour
{
    public TextMeshProUGUI sequenceText;
    public Button redButton, yellowButton, blueButton, submitButton, retryButton;
    public TextMeshProUGUI resultText;

    private List<string> selectedColors = new List<string>();
    private List<string> fullSequence = new List<string>();
    private int missingIndex;
    private string missingColor;

    private Dictionary<string, string> colorMixes = new Dictionary<string, string>
    {
    { "Red", "Red" },
    { "RedRed", "Red" },
    { "RedRedRed", "Red" },

    { "RedYellow", "Orange" },
    { "YellowRed", "Orange" },

    { "RedYellowRed", "Redder Orange" },
    { "YellowRedRed", "Redder Orange" },
    { "RedRedYellow", "Redder Orange" },

    { "YellowYellowRed", "Yellower Orange" },
    { "YellowRedYellow", "Yellower Orange" },
    { "RedYellowYellow", "Yellower Orange" },

    { "Yellow", "Yellow" },
    { "YellowYellow", "Yellow" },
    { "YellowYellowYellow", "Yellow" },

    { "BlueYellowYellow", "Yellower Green" },
    { "YellowBlueYellow", "Yellower Green" },
    { "YellowYellowBlue", "Yellower Green" },

    { "BlueYellow", "Green" },
    { "YellowBlue", "Green" },
    
    { "BlueBlueYellow", "Bluer Green" },
    { "YellowBlueBlue", "Bluer Green" },
    { "BlueYellowBlue", "Bluer Green" },

    { "Blue", "Blue" },
    { "BlueBlue", "Blue" },
    { "BlueBlueBlue", "Blue" },

    { "BlueBlueRed", "Bluer Purple" },
    { "BlueRedBlue", "Bluer Purple" },
    { "RedBlueBlue", "Bluer Purple" },

    { "BlueRed", "Purple" },
    { "RedBlue", "Purple" },

    { "RedRedBlue", "Redder Purple" },
    { "RedBlueRed", "Redder Purple" },
    { "BlueRedRed", "Redder Purple" },
    };

    private Dictionary<string, string> colorHexCodes = new Dictionary<string, string>
    {
        { "Red", "#FF0000" },            
        { "Orange", "#FFA500" },      
        { "Redder Orange", "#FF4500" },   
        { "Yellower Orange", "#FFD700" },
        { "Yellow", "#FFFF00" },        
        { "Yellower Green", "#ADFF2F" },
        { "Green", "#008000" },     
        { "Bluer Green", "#006400" }, 
        { "Blue", "#0000FF" },          
        { "Bluer Purple", "#483D8B" },    
        { "Purple", "#800080" },        
        { "Redder Purple", "#9932CC" }      
    };

    private List<string> GenerateValidSequence()
{
    List<string> sequence = new List<string>();
    List<string> orderedColors = new List<string>
    {
        "Red", "Orange", "Redder Orange", "Yellower Orange", 
        "Yellow", "Yellower Green",  "Green", "Bluer Green", 
        "Blue", "Bluer Purple", "Purple", "Redder Purple"
    };

    int index = Random.Range(0, orderedColors.Count - 1);
    

    bool isReversed = Random.value > 0.5f;

    if (isReversed){
        for (int i = 0; i < 5; i++)
            if (index == 0){  
                sequence.Add(orderedColors[index]);
                index = orderedColors.Count - 1;
            }
            else{
                sequence.Add(orderedColors[index]);
                index = index - 1;
            }   
    }
    else{
        for (int i = 0; i < 5; i++)
            if (index == orderedColors.Count - 1){
                sequence.Add(orderedColors[index]);
                index = 0;
            }
            else{
                sequence.Add(orderedColors[index]);
                index = index + 1;
            }   
    }

    return sequence;
}

    public void Setup()
    {
        fullSequence = GenerateValidSequence();

        missingIndex = Random.Range(0, fullSequence.Count - 1);
        missingColor = fullSequence[missingIndex];
        fullSequence[missingIndex] = "???";

        sequenceText.text = GetColoredSequence();
        selectedColors.Clear();
        resultText.text = "";
    }

    private string GetColoredSequence()
    {
        List<string> coloredSequence = new List<string>();

        for (int i = 0; i < fullSequence.Count; i++)
        {
            if (i == missingIndex && selectedColors.Count >= 0)
            {
                string mixResult = GetMixedColor();
                if (!string.IsNullOrEmpty(mixResult))
                {
                    string colorHex = colorHexCodes.ContainsKey(mixResult) ? colorHexCodes[mixResult] : "#FFFFFF";
                    coloredSequence.Add($"<color={colorHex}>{mixResult}</color>");
                }
                else
                {
                    coloredSequence.Add("<color=#FFFFFF>???</color>");
                }
            }
            else if (colorHexCodes.ContainsKey(fullSequence[i]))
            {
                coloredSequence.Add($"<color={colorHexCodes[fullSequence[i]]}>{fullSequence[i]}</color>");
            }
            else
            {
                coloredSequence.Add(fullSequence[i]);
            }
        }

        return string.Join(" → ", coloredSequence);
    }

    private string GetMixedColor()
    {
        List<string> sortedColors = selectedColors.OrderBy(c => c).ToList();
        string key = string.Join("", sortedColors);
        return colorMixes.ContainsKey(key) ? colorMixes[key] : "";
    }

    public void SelectColor(string color)
    {
        if (selectedColors.Count <= 2)
        {
            selectedColors.Add(color);
        }

        sequenceText.text = GetColoredSequence();
    }

    public void Submit()
    {
        string mixedColor = GetMixedColor();

        if (!string.IsNullOrEmpty(mixedColor) && mixedColor == missingColor)
        {
            resultText.text = "Correct!";
        }
        else
        {
            resultText.text = "Try Again!";
            selectedColors.Clear();
            sequenceText.text = GetColoredSequence();
        }
    }

    public void Retry()
    {
        selectedColors.Clear();
        sequenceText.text = GetColoredSequence();
        resultText.text = "Select colors again!";
    }

    void Start()
    {
        redButton.onClick.AddListener(() => SelectColor("Red"));
        yellowButton.onClick.AddListener(() => SelectColor("Yellow"));
        blueButton.onClick.AddListener(() => SelectColor("Blue"));
        submitButton.onClick.AddListener(Submit);
        retryButton.onClick.AddListener(Retry);

        Setup();
    }
}
