 using UnityEngine;

public class PropSettings : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject Collider;
    [SerializeField]
    private GameObject Visual;

    [Header("Settings")]
    public bool PathBlocking = false;
    public bool HasSupporting = false;

    public Vector3 GetSize()
    {
        return Collider.transform.localScale;
    }

    public void SetPosition(Quaternion rotation, Vector3 position)
    {
        Collider.transform.position = position;
        Visual.transform.position = position;

        Collider.transform.rotation = rotation;
        Visual.transform.rotation = rotation;
    }
}
