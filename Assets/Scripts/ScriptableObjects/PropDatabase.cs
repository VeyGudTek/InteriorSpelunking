using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PropDatabase", menuName = "Scriptable Objects/PropDatabase")]
public class PropDatabase : ScriptableObject
{
    private readonly List<GameObject> EdgeProps = new();
    private readonly List<GameObject> CenterProps = new();
    private readonly List<GameObject> SupportingProps = new();
    private readonly List<GameObject> Decoration = new();
    private readonly List<GameObject> RandomProps = new();
    private readonly List<GameObject> TableTopProps = new();

    public GameObject GetRandomProp(PropType type)
    {
        return type switch
        {
            PropType.Edge => GetRandomFromList(EdgeProps),
            PropType.Center => GetRandomFromList(CenterProps),
            PropType.Supporting => GetRandomFromList(SupportingProps),
            PropType.Decoration => GetRandomFromList(Decoration),
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
