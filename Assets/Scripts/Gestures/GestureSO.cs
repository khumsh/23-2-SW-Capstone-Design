using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandType
{
    Left, Right
}

[CreateAssetMenu(fileName = "Gesture")]
public class GestureSO : ScriptableObject
{
    public HandType handType = HandType.Left;
    public Gesture gesture;
}
