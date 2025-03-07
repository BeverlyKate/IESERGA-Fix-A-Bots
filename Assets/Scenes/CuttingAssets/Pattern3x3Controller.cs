using UnityEngine;

public class Pattern3x3Controller : MonoBehaviour
{
    public GameObject[] squareTiles; // assign in Inspector or find dynamically

    public enum Shape
    {
        Full,
        Cross,
        LShape,
        TShape
    }

    public Shape currentShape;

    void Start()
    {
        UpdateShape();
    }

    public void UpdateShape()
    {
        switch (currentShape)
        {
            case Shape.Full:
                SetActiveTiles(new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 });
                break;
            case Shape.Cross:
                SetActiveTiles(new int[] { 1, 3, 4, 5, 7 });
                break;
            case Shape.LShape:
                SetActiveTiles(new int[] { 0, 3, 6, 7, 8 });
                break;
            case Shape.TShape:
                SetActiveTiles(new int[] { 0, 1, 2, 4, 7});
                break;
        }
    }

    void SetActiveTiles(int[] activeIndices)
    {
        // Deactivate all tiles first
        foreach (GameObject tile in squareTiles)
        {
            tile.SetActive(false);
        }

        // Activate specified tiles
        foreach (int index in activeIndices)
        {
            squareTiles[index].SetActive(true);
        }
    }
}
