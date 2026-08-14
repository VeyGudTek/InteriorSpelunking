using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PropDatabase", menuName = "Scriptable Objects/PropDatabase")]
public class PropDatabase : ScriptableObject
{
    [SerializeField]
    private List<GameObject> EdgeProps = new();
    [SerializeField]
    private List<GameObject> CenterProps = new();
    [SerializeField]
    private List<GameObject> SupportingProps = new();
    [SerializeField]
    private List<GameObject> WallDecoration = new();
    [SerializeField]
    private List<GameObject> RandomProps = new();
    [SerializeField]
    private List<GameObject> TableTopProps = new();

    public GameObject GetRandomProp(PropType type)
    {
        return type switch
        {
            PropType.Edge => GetRandomFromList(EdgeProps),
            PropType.Center => GetRandomFromList(CenterProps),
            PropType.Supporting => GetRandomFromList(SupportingProps),
            PropType.WallDecoration => GetRandomFromList(WallDecoration),
            PropType.Random => GetRandomFromList(RandomProps),
            PropType.TableTop => GetRandomFromList(TableTopProps),
            _ => throw new System.ArgumentException($"Invalid PropType: {type}"),
        };
    }

    private GameObject GetRandomFromList(List<GameObject> list)
    {
        return list[Random.Range(0, list.Count)];
    }
}
