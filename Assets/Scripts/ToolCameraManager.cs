using UnityEngine;

public class ToolCameraManager : MonoBehaviour
{
    [SerializeField] private GameObject LeftButton;
    [SerializeField] private GameObject RightButton;
    [SerializeField] private GameObject ZoomButton;

    public float speed = 5f;
    private bool isZoomed = false;
    private Vector3 origPos;
    private Vector3 partPos;

    private Vector3 origRot;
    private Vector3 currentRot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        origPos = transform.position;
        partPos = GameObject.FindGameObjectWithTag("Part").transform.position;

        Debug.Log(partPos);

        partPos = new Vector3(partPos.x, partPos.y + 0.35f, partPos.z);

        origRot = transform.eulerAngles;

        ZoomButton.SetActive(true);

        LeftButton.SetActive(false);
        RightButton.SetActive(false);
    }

    void Update()
    {
    }

    public void Zoom()
    {
        if (!isZoomed)
        {
            transform.position = Vector3.MoveTowards(origPos, partPos, speed);
            transform.eulerAngles = new Vector3(origRot.x - 15f, origRot.y, origRot.z);
            currentRot = transform.eulerAngles;
            isZoomed = true;
            LeftButton.SetActive(true);
            RightButton.SetActive(true);
        }
        else
        {
            transform.position = Vector3.MoveTowards(partPos, origPos, speed);
            transform.eulerAngles = origRot;
            isZoomed = false;
            LeftButton.SetActive(false);
            RightButton.SetActive(false);
        }
    }

    public void RotateRight()
    {
        transform.eulerAngles = new Vector3(currentRot.x, currentRot.y + 90f, currentRot.z);
        currentRot = transform.eulerAngles;
    }

    public void RotateLeft()
    {
        transform.eulerAngles = new Vector3(currentRot.x, currentRot.y - 90f, currentRot.z);
        currentRot = transform.eulerAngles;
    }
}
