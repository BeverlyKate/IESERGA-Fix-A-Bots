using UnityEngine;

public enum WireColor3 { Red, Yellow }

public class Wire3 : MonoBehaviour
{
    public WireColor3 wireColor3; // Assign in Inspector
    private WireStation3 wireStation3;
    public Transform currentSlot;
    public float defaultZ; // Stores the original Z position

    private void Start()
    {
        wireStation3 = FindObjectOfType<WireStation3>();
        defaultZ = transform.position.z; // Save original Z position
    }

    private void OnMouseDown()
    {
        wireStation3.StartDragging(this);
    }
}
