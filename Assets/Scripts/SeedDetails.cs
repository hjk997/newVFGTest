using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class SeedDetails : MonoBehaviour
    {
        public Dictionary<int, SeedDetails> seedList;

        string seedName;
        int leftNum;

        public SeedDetails(string name, int leftNum)
        {
            this.seedName = name;
            this.leftNum = leftNum;
        }

        public string SeedName { get => seedName; set => seedName = value; }

        public int LeftNum { get => leftNum; set => leftNum = value; }
    }
}