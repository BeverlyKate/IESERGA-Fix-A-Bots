using UnityEngine;
using System.Collections.Generic;

public class WireStation : MonoBehaviour
{
    private Wire draggedWire = null;
    private Vector3 offset;
    public List<Wire> wires = new List<Wire>();
    public Transform[] wireSlots; // Assign in the Inspector
    public GameObject correctIndicator; // Assign a visual indicator

    public Camera mainCamera; // Assign in Inspector!

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            Debug.LogError("No Camera assigned or found in the scene!");
        }
    }

private void Update()
{
    if (draggedWire != null)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane dragPlane = new Plane(Vector3.up, draggedWire.transform.position); // Plane aligned with the wire's height
        float distance;

        if (dragPlane.Raycast(ray, out distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            draggedWire.transform.position = new Vector3(worldPosition.x + offset.x, draggedWire.transform.position.y, worldPosition.z + offset.z);
        }

        if (Input.GetMouseButtonUp(0)) // Release the wire
        {
            DropWire(draggedWire);
            draggedWire = null;
        }
    }
}

public void StartDragging(Wire wire)
{
    draggedWire = wire;

    // Free up the slot before dragging
    if (wire.currentSlot != null)
    {
        WireSlot previousSlot = wire.currentSlot.GetComponent<WireSlot>();
        if (previousSlot != null)
        {
            previousSlot.SetOccupied(false);
        }
        wire.currentSlot = null;
    }

    // Raycast to find initial mouse position in world space
    Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    Plane dragPlane = new Plane(Vector3.up, wire.transform.position);
    float distance;

    if (dragPlane.Raycast(ray, out distance))
    {
        Vector3 worldPosition = ray.GetPoint(distance);
        offset = wire.transform.position - worldPosition;
    }
}




 private void CheckCorrectOrder()
{
    // Ensure all slots are occupied before checking order
    if (wires.Count != wireSlots.Length)
    {
        correctIndicator.SetActive(false);
        ResetAllSlots(); // Reset slot colors
        return;
    }

    // Sort wires based on X position (left to right)
    wires.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

    // Check if the exact order is Yellow → Red → Red → Red
    if (wires.Count == 4 &&
        wires[0].wireColor == WireColor.Yellow &&
        wires[1].wireColor == WireColor.Red &&
        wires[2].wireColor == WireColor.Red &&
        wires[3].wireColor == WireColor.Red)
    {
        correctIndicator.SetActive(true); // Show correct order indicator
    }
    else
    {
        correctIndicator.SetActive(false);
        ResetAllSlots(); // Reset slot colors if incorrect
    }
}

// Reset all wire slots to default material
private void ResetAllSlots()
{
    foreach (Transform slot in wireSlots)
    {
        WireSlot wireSlot = slot.GetComponent<WireSlot>();
        if (wireSlot != null)
        {
            wireSlot.ResetSlot();
        }
    }
}


}
