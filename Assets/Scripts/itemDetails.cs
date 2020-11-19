using UnityEngine;

namespace Assets.Scripts
{
    public class itemDetails : MonoBehaviour
    {
        string itemName;
        string leftNum;
        string description;
        int type;
        int id;

        public itemDetails(string name, string leftNum, int type)
        {
            this.itemName = name;
            this.leftNum = leftNum;
            this.type = type;
        }

        public string ItemName
        {
            get => itemName;
            set => itemName = value;
        }

        public string LeftNum
        {
            get => leftNum;
            set => leftNum = value;
        }

        public int Type
        {
            get => type;
            set => type = value;
        }

        public int Id { get => id; set => id = value; }

        public string Description
        {
            get => description;
            set => description = value;
        }
    }
}