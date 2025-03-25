using UnityEngine;

public class Nail : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void hammerNail()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z);
    }

    public void setNailPosition(Vector3 position)
    {
        Vector3 sizeVec = transform.GetComponent<Renderer>().bounds.size;
        Vector3 addedPos = position;
        float newY = addedPos.y + (sizeVec.y / 2);
        transform.position = new Vector3(addedPos.x, newY, addedPos.z);
    }
}
