using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "shopMenu", menuName = "Scriptable Objects/New Sword Item", order = 1)]
public class SwordItemSO : ScriptableObject
{
    public int damage;
}
