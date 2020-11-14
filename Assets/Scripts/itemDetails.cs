using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class itemDetails : MonoBehaviour
{
    string itemName;
    string leftNum;
    int type;

    public itemDetails(string name, string leftNum, int type)
    {
        this.itemName = name;
        this.leftNum = leftNum;
        this.type = type;
    }

    public string ItemName { get => itemName; set => itemName = value; }

    public string LeftNum { get => leftNum; set => leftNum = value; }

    public int Type { get => type; set => type = value; }
}
