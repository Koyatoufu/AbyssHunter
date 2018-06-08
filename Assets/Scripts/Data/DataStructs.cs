using UnityEngine;
using System.Collections.Generic;


namespace Data
{
    [System.Serializable]
    public class PlayerInfo
    {
        public string name;
        public string passWord;
        public Gender gender;
    }

    public class InventoryInfo
    {
                        //index, count
        public List<ItemData> itemList = new List<ItemData>();
        public List<ItemData> spellList = new List<ItemData>();

        public int curWeaponIdx;
        public int subWeaponIdx;

        public int curBodyIdx;
        public int curArmIdx;
        public int curLegIdx;
        public int curHeadIdx;
    }

    public class BoxInfo
    {               
               //index, count
        public List<ItemData> itemList = new List<ItemData>();
               //PartsType(Parse Int), index
        public List<EquipmentBoxInfo> equipmentList = new List<EquipmentBoxInfo>();
    }

    public class EquipmentBoxInfo
    {
        public PartsType partsType;
        public int index;
    }

    [System.Serializable]
    public struct QuestInfo
    {
        public string questName;
        public string areaName;
        public UnitType mainTargetType;
        public int mainTargetIdx;
        public int mainTargetCount;
        public float timeLimit;
    }

    [System.Serializable]
    public struct AreaInfo
    {
        public string areaName;
        public string FullName;

        public string skyBoxName;
        public int subAreaCount;
    }
}

public enum Gender { Female, Male };
