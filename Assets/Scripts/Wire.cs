using UnityEngine;

public enum WireColor { Red, Yellow }

public class Wire : MonoBehaviour
{
    public WireColor wireColor; // Assign in Inspector
    private WireStation wireStation;
    public Transform currentSlot;
    public float defaultZ; // Stores the original Z position

    private void Start()
    {
        wireStation = FindObjectOfType<WireStation>();
        defaultZ = transform.position.z; // Save original Z position
    }

    private void OnMouseDown()
    {
        wireStation.StartDragging(this);
    }
}
