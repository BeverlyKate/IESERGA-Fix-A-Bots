using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WireStation : MonoBehaviour
{
    private Wire draggedWire = null;
    private Vector3 offset;
    public List<Wire> wires = new List<Wire>();
    public Transform[] wireSlots; // Assign in the Inspector
    public GameObject correctIndicator; // Assign a visual indicator

    public Camera mainCamera; // Assign in Inspector!
    public Transform targetObject; // Assign in Inspector
    public float yOffset = 1.0f; // Adjust in Inspector
    private bool isDone = false;

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
            // Find the touch that's dragging this wire
            Touch? activeTouch = null;
            foreach (Touch touch in Input.touches)
            {
                if (touch.fingerId == draggedWire.GetTouchId())
                {
                    activeTouch = touch;
                    break;
                }
            }

            if (activeTouch.HasValue)
            {
                Ray ray = mainCamera.ScreenPointToRay(activeTouch.Value.position);
                Plane dragPlane = new Plane(Vector3.up, draggedWire.transform.position);
                float distance;

                if (dragPlane.Raycast(ray, out distance))
                {
                    Vector3 worldPosition = ray.GetPoint(distance);
                    draggedWire.transform.position = new Vector3(worldPosition.x + offset.x, draggedWire.transform.position.y, worldPosition.z + offset.z);
                }
            }
            else
            {
                // Touch was lost, drop the wire
                DropWire(draggedWire);
                draggedWire = null;
            }
        }
        else
        {
            // Check for new touches on wires
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = mainCamera.ScreenPointToRay(touch.position);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        Wire wire = hit.collider.GetComponent<Wire>();
                        if (wire != null)
                        {
                            wire.StartTouch(touch.fingerId);
                            break;
                        }
                    }
                }
            }
        }
    }

    public void EndTouch(Wire wire)
    {
        if (draggedWire == wire)
        {
            DropWire(wire);
            draggedWire = null;
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
                previousSlot.SetOccupied(false, false);
            }
            wire.currentSlot = null;
        }

        // Raycast to find initial touch position in world space
        Touch? touch = null;
        foreach (Touch t in Input.touches)
        {
            if (t.fingerId == wire.GetTouchId())
            {
                touch = t;
                break;
            }
        }

        if (touch.HasValue)
        {
            Ray ray = mainCamera.ScreenPointToRay(touch.Value.position);
            Plane dragPlane = new Plane(Vector3.up, wire.transform.position);
            float distance;

            if (dragPlane.Raycast(ray, out distance))
            {
                Vector3 worldPosition = ray.GetPoint(distance);
                offset = wire.transform.position - worldPosition;
            }
        }
    }

    private void DropWire(Wire wire)
    {
        wire.transform.localScale = Vector3.one; // Ensure scale remains unchanged

        float snapRange = 1.0f; // Detection range
        Transform closestSlot = null;
        WireSlot closestWireSlot = null;
        float minDistance = float.MaxValue;

        // Find the touch position for this wire
        Vector2 touchPosition = Vector2.zero;
        bool foundTouch = false;
        foreach (Touch touch in Input.touches)
        {
            if (touch.fingerId == wire.GetTouchId())
            {
                touchPosition = touch.position;
                foundTouch = true;
                break;
            }
        }

        if (!foundTouch) return;

        Ray ray = mainCamera.ScreenPointToRay(touchPosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

        foreach (RaycastHit hit in hits)
        {
            WireSlot wireSlotComponent = hit.collider.GetComponent<WireSlot>();

            if (wireSlotComponent != null)
            {
                float distance = Vector3.Distance(wire.transform.position, hit.point);
                if (distance < minDistance && distance < snapRange)
                {
                    closestSlot = hit.collider.transform;
                    closestWireSlot = wireSlotComponent;
                    minDistance = distance;
                }
            }
        }

        if (closestSlot != null)
        {
            wire.currentSlot = closestSlot;
            bool isCorrect = CheckIfCorrectWire(closestSlot, wire);
            closestWireSlot.SetOccupied(true, isCorrect);
            closestWireSlot.isOccupied = true;
        }

        CheckCorrectOrder();
    }

    private void CheckCorrectOrder()
    {
        bool allCorrect = true;

        foreach (Transform slot in wireSlots)
        {
            WireSlot wireSlot = slot.GetComponent<WireSlot>();

            if (wireSlot == null || !wireSlot.isCorrect)
            {
                allCorrect = false;
                break;
            }
        }

        if (allCorrect)
        {
            correctIndicator.SetActive(true);
            GetComponentInParent<WiringTeleport>().TriggerTeleport();
        }
        else
        {
            correctIndicator.SetActive(false);
        }
    }

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

    private bool CheckIfCorrectWire(Transform slot, Wire wire)
    {
        int slotIndex = System.Array.IndexOf(wireSlots, slot);
        WireColor[] expectedColors = { WireColor.Yellow, WireColor.Red, WireColor.Red, WireColor.Red };

        if (slotIndex >= 0 && slotIndex < expectedColors.Length)
        {
            return wire.wireColor == expectedColors[slotIndex];
        }

        return false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (draggedWire != null)
        {
            float snapRange = 1.0f;
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(draggedWire.transform.position, Vector3.up, snapRange);
        }
    }
#endif
}

// Add this extension method to access the touch ID
public static class WireExtensions
{
    public static int GetTouchId(this Wire wire)
    {
        var field = typeof(Wire).GetField("touchId", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (int?)field?.GetValue(wire) ?? -1;
    }
}