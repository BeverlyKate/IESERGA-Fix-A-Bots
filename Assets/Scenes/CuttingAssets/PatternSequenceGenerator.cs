using UnityEngine;
using System.Collections.Generic;

public class PatternSequenceGenerator : MonoBehaviour
{
    public GameObject patternBoardPrefab;
    public int totalBoards = 20; // Y - Total boards
    public int patternLength = 3; // N - Number of unique shapes in the repeating pattern
    public float spacing = 3f; // Distance between boards

    private List<Pattern3x3Controller.Shape> patternSequence = new List<Pattern3x3Controller.Shape>();

    void Start()
    {
        GeneratePatternSequence();
        GenerateBoards();
    }

    void GeneratePatternSequence()
    {
        // Get all possible shapes
        List<Pattern3x3Controller.Shape> allShapes = new List<Pattern3x3Controller.Shape>(
            (Pattern3x3Controller.Shape[])System.Enum.GetValues(typeof(Pattern3x3Controller.Shape))
        );

        // Shuffle the list to get randomness
        for (int i = allShapes.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            var temp = allShapes[i];
            allShapes[i] = allShapes[randomIndex];
            allShapes[randomIndex] = temp;
        }

        // Take N shapes for the repeating pattern
        for (int i = 0; i < patternLength; i++)
        {
            patternSequence.Add(allShapes[i]);
        }

        Debug.Log("Generated Pattern: " + string.Join(", ", patternSequence));
    }

    void GenerateBoards()
    {
        for (int i = 0; i < totalBoards; i++)
        {
            Vector3 spawnPosition = new Vector3(i * spacing, 0, 0); // Row along X-axis
            GameObject newBoard = Instantiate(patternBoardPrefab, spawnPosition, Quaternion.identity, transform);

            Pattern3x3Controller boardController = newBoard.GetComponent<Pattern3x3Controller>();

            // Repeat the pattern sequence
            int shapeIndex = i % patternSequence.Count;
            boardController.currentShape = patternSequence[shapeIndex];
            boardController.UpdateShape();
        }
    }
}
