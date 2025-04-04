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
    public Transform[] wireSlots;
    public GameObject correctIndicator;
    public Camera mainCamera;
    public Transform targetObject;
    public float yOffset = 1.0f;
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
        Vector3 inputPos = GetInputPosition();

        if (draggedWire != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(inputPos);
            Plane dragPlane = new Plane(Vector3.up, draggedWire.transform.position);
            float distance;

            if (dragPlane.Raycast(ray, out distance))
            {
                Vector3 worldPosition = ray.GetPoint(distance);
                draggedWire.transform.position = new Vector3(worldPosition.x + offset.x, draggedWire.transform.position.y, worldPosition.z + offset.z);
            }

            if (IsInputReleased())
            {
                DropWire(draggedWire, inputPos);
                draggedWire = null;
            }
        }
    }

    public void StartDragging(Wire wire)
    {
        draggedWire = wire;

        if (wire.currentSlot != null)
        {
            WireSlot previousSlot = wire.currentSlot.GetComponent<WireSlot>();
            if (previousSlot != null)
            {
                previousSlot.SetOccupied(false, false);
            }
            wire.currentSlot = null;
        }

        Vector3 inputPos = GetInputPosition();

        Ray ray = mainCamera.ScreenPointToRay(inputPos);
        Plane dragPlane = new Plane(Vector3.up, wire.transform.position);
        float distance;

        if (dragPlane.Raycast(ray, out distance))
        {
            Vector3 worldPosition = ray.GetPoint(distance);
            offset = wire.transform.position - worldPosition;
        }
    }

    private void DropWire(Wire wire, Vector3 inputPos)
    {
        wire.transform.localScale = Vector3.one;

        float snapRange = 1.0f;
        Transform closestSlot = null;
        WireSlot closestWireSlot = null;
        float minDistance = float.MaxValue;

        Ray ray = mainCamera.ScreenPointToRay(inputPos);
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

            Debug.Log($"🟢 Wire {wire.name} placed near slot {closestSlot.name} (Correct: {isCorrect})");
        }
        else
        {
            Debug.LogWarning($"⚠️ Wire {wire.name} was dropped but not near any slot!");
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

    private Vector3 GetInputPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;
        else
            return Input.mousePosition;
    }

    private bool IsInputReleased()
    {
        return Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Debug.Log("🟡 OnDrawGizmos is running!");
        Debug.Log($"🟡 OnDrawGizmos running! draggedWire = {(draggedWire != null ? draggedWire.name : "null")}");

        if (draggedWire != null)
        {
            float snapRange = 1.0f;
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(draggedWire.transform.position, Vector3.up, snapRange);
        }
    }
#endif
}
