using UnityEngine;

public class PropBase : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject Collider;
    [SerializeField]
    private GameObject Visual;

    public void Initialize(Side direction, Vector3 position)
    {
        TransformProp(direction, position);
    }

    private void TransformProp(Side direction, Vector3 position)
    {
        Collider.transform.position = position;
        Visual.transform.position = position;

        Quaternion orientation = direction.GetRotation();
        Collider.transform.rotation = orientation;
        Visual.transform.rotation = orientation;
    }
}
