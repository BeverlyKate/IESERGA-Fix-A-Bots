using System.Numerics;
using UnityEngine;

public class RobotHead : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject[] missingLocations;

    [SerializeField]
    private float[] correctPosition;

    [SerializeField]
    private int[] correctBolts;

    [SerializeField]
    private GameObject[] completeIndicator;

    private GameObject[] filledLocations;

    public Material complete;

    private int checkCorrect = 0;

    void Start()
    {
        filledLocations = new GameObject[missingLocations.Length];
    }

    public int locationPlacement(GameObject objectToPlace)
    {
        int placement = -1;
        for (int i = 0; i < missingLocations.Length; i++)
        {
            if (objectToPlace == missingLocations[i])
            {
                placement = i;
            }
        }
        return placement;
    }

    public bool checkLocation(int placement, int fastenerNum)
    {
        if (filledLocations[placement] == null && fastenerNum == correctBolts[placement])
        {
            return true;
        }
        return false;
    }

    public void setObjectPlacement(GameObject fastenerToPlace, int placement)
    {
        filledLocations[placement] = fastenerToPlace;
    }

    public void incrementCorrect()
    {
        checkCorrect++;
    }

    public bool checkIncrement()
    {
       if(checkCorrect == missingLocations.Length)
       {
            return true;
       }
       return false;
    }

    public float checkCorrectPosition(int placement)
    {
        return correctPosition[placement];
    }

    //triggers when pattern is complete
    public void triggerDone()
    {
        foreach (GameObject part in completeIndicator)
        {
            part.GetComponent<Renderer>().material = complete;
        }
    }

    public int findFastenerPos(GameObject fastener)
    {
        for (int i = 0; i < filledLocations.Length; i++)
        {
            if (filledLocations[i] == fastener)
            {
                Debug.Log(i);
                return i;
            }
        }
        return -1;
    }
}
