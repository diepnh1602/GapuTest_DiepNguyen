using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "newColor", menuName = "Config/Block Color")]
public class BlockColorConfig : ScriptableObject
{
    public int id;
    public Color color;
}
