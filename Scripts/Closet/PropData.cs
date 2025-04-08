using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Prop Data", menuName = "Scriptable Object/Prop Data", order = 0)]
public class PropData : ScriptableObject
{
    [SerializeField] private int propID;
    public int PropId { get { return propID; } }

    [SerializeField] private string propName;
    public string PropName { get { return propName; } }
    
    [SerializeField] private string propExplain;
    public string PropExplain { get { return propExplain; } }

    [SerializeField] private GameObject propObject;
    public GameObject PropObject { get { return propObject; } }
}