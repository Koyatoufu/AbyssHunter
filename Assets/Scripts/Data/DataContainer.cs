using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Data
{
    public class EquipmentContainer
    {
        [SerializeField]
        private List<Dictionary<int, string>> m_equipDatas = new List<Dictionary<int, string>>((int)PartsType.MAX);

        public void Init()
        {
            for (int i = 0; i < m_equipDatas.Capacity; i++)
            {
                Dictionary<int, string> temp = new Dictionary<int, string>();
                m_equipDatas.Add(temp);
            }

            XmlLoad();
        }

        void XmlLoad()
        {
            //TextAsset equipDatasText = Resources.Load<TextAsset>("Data/EquipDatas");
            string equipDatasText = File.ReadAllText(Application.streamingAssetsPath + "/Data/EquipDatas.xml");

            if (equipDatasText == null)
                return;

            XmlDocument document = new XmlDocument();
            document.LoadXml(equipDatasText);
            //document.LoadXml(equipDatasText.text);

            XmlNode root = document.SelectSingleNode("EquipDatas");

            #region PartsAdd

            XmlNode body = root.SelectSingleNode("Body");
            PartsAdd(PartsType.BODY, body);

            XmlNode arm = root.SelectSingleNode("Arm");
            PartsAdd(PartsType.ARM, arm);

            XmlNode leg = root.SelectSingleNode("Leg");
            PartsAdd(PartsType.LEG, leg);

            XmlNode head = root.SelectSingleNode("Head");
            PartsAdd(PartsType.HEAD, head);

            XmlNode weapon = root.SelectSingleNode("Weapon");
            PartsAdd(PartsType.WEAPON, weapon);

            #endregion

        }

        void PartsAdd(PartsType type, XmlNode parent)
        {
            XmlNodeList parts = parent.SelectNodes("Parts");

            for (int i = 0; i < parts.Count; i++)
            {
                int nIdx = int.Parse(parts[i].SelectSingleNode("index").InnerText);
                string name = parts[i].SelectSingleNode("name").InnerText;

                m_equipDatas[(int)type].Add(nIdx, name);
            }
        }

        public IEquipment GetEquipment(PartsType type, int nIdx)
        {
            if (type == PartsType.MAX)
            {
                Debug.LogError("You can not Use MAXType");
                return null;
            }


            if (!m_equipDatas[(int)type].ContainsKey(nIdx))
            {
                Debug.Log(type);
                Debug.LogError(nIdx + " is not matched data");
                return null;
            }

            IEquipment equipment = null;
            string path = "Scriptable/Item/Equipment/";
            string itemName = m_equipDatas[(int)type][nIdx];

            switch (type)
            {
                case PartsType.BODY:
                    path += "Wearable/Body/" + itemName;
                    equipment = Resources.Load<CWearable>(path);
                    break;
                case PartsType.ARM:
                    path += "Wearable/Arm/" + itemName;
                    equipment = Resources.Load<CWearable>(path);
                    break;
                case PartsType.LEG:
                    path += "Wearable/Leg/" + itemName;
                    equipment = Resources.Load<CWearable>(path);
                    break;
                case PartsType.HEAD:
                    path += "Wearable/Head/" + itemName;
                    equipment = Resources.Load<CWearable>(path);
                    break;
                case PartsType.WEAPON:
                    path += "Weapon/" + itemName;
                    equipment = Resources.Load<CWeapon>(path);
                    break;
            }

            return equipment;
        }
    }

    [System.Serializable]
    public class ItemContainer
    {
        [SerializeField]
        private List<string> m_itemData = new List<string>();

        public void Init()
        {
            //TextAsset equipDatasText = Resources.Load<TextAsset>("Data/ItemData");
            string equipDatasText = File.ReadAllText(Application.streamingAssetsPath + "/Data/ItemData.xml");

            if (equipDatasText == null)
                return;

            XmlDocument document = new XmlDocument();
            document.LoadXml(equipDatasText);
            //document.LoadXml(equipDatasText.text);

            XmlNode root = document.SelectSingleNode("ItemDatas");
            XmlNodeList itemlist = root.SelectNodes("Item");
            for (int i = 0; i < itemlist.Count; i++)
            {
                m_itemData.Add(itemlist[i].InnerText);
            }

        }

        public CItemBase GetItem(int nIdx)
        {
            if (m_itemData.Count <= nIdx)
                return null;

            string path = "Scriptable/Item/";

            path += m_itemData[nIdx];

            CItemBase item = Resources.Load<CItemBase>(path);

            return item;
        }

    }

    [System.Serializable]
    public class PlayerRecord
    {
        [SerializeField]
        private PlayerInfo m_playerInfo;
        public PlayerInfo PlayerInfo { get { return m_playerInfo; } }
        [SerializeField]
        private InventoryInfo m_inventoryInfo = new InventoryInfo();
        public InventoryInfo InventoryInfo { get { return m_inventoryInfo; } }
        [SerializeField]
        private BoxInfo m_boxInfo = new BoxInfo();
        public BoxInfo BoxInfo { get { return m_boxInfo; } }

        private ItemContainer m_itemContainier;

        public void Init(ItemContainer itemContainer)
        {
            m_itemContainier = itemContainer;

            LoadPlayerInfo();
            LoadInvenToryData();
            LoadBoxInfo();
        }

        public void SaveUserData(string userName, Gender gender = Gender.Female)
        {
            PlayerInfo playerInfo = new PlayerInfo();

            if (string.IsNullOrEmpty(m_playerInfo.name))
            {
                playerInfo.name = userName;
                playerInfo.gender = gender;
                playerInfo.passWord = Random.Range(0, int.MaxValue).ToString();
            }
            else
            {
                playerInfo = m_playerInfo;
            }

            m_playerInfo = playerInfo;

            SavePlayerInfo(playerInfo);
            SaveInventoryInfo(m_inventoryInfo);
            SaveBoxInfo(m_boxInfo);
        }

        public void SaveAllData()
        {
            if (m_playerInfo != null)
                SavePlayerInfo(m_playerInfo);
            if (m_inventoryInfo != null)
                SaveInventoryInfo(m_inventoryInfo);
            if (m_boxInfo != null)
                SaveBoxInfo(m_boxInfo);
        }

        public void SavePlayerInfo(PlayerInfo playerInfo)
        {
            m_playerInfo = playerInfo;

            XmlDocument document = new XmlDocument();
            document.AppendChild(document.CreateXmlDeclaration("1.0", "utf-8", "yes"));

            XmlNode root = document.CreateNode(XmlNodeType.Element, "PlayerInfo", string.Empty);
            document.AppendChild(root);

            XmlElement name = document.CreateElement("Name");
            name.InnerText = playerInfo.name;
            root.AppendChild(name);

            XmlElement passWord = document.CreateElement("PassWord");
            passWord.InnerText = playerInfo.passWord;
            root.AppendChild(passWord);

            XmlElement gender = document.CreateElement("Gender");
            gender.InnerText = playerInfo.gender.ToString();
            root.AppendChild(gender);

            //document.Save("./Assets/Resources/Data/PlayerInfo.xml");
            File.WriteAllText(Application.streamingAssetsPath + "/Data/PlayerInfo.xml", document.OuterXml);

        }

        public void SaveInventoryInfo(InventoryInfo inventoryInfo)
        {
            m_inventoryInfo = inventoryInfo;

            XmlDocument document = new XmlDocument();
            document.AppendChild(document.CreateXmlDeclaration("1.0", "utf-8", "yes"));

            XmlNode root = document.CreateNode(XmlNodeType.Element, "Inventory", string.Empty);
            document.AppendChild(root);

            #region ItemSave

            XmlNode items = document.CreateNode(XmlNodeType.Element, "Items", string.Empty);
            root.AppendChild(items);

            if (inventoryInfo.itemList == null)
                return;

            for (int i = 0; i < inventoryInfo.itemList.Count; i++)
            {
                if (inventoryInfo.itemList[i] == null || inventoryInfo.itemList[i].item == null)
                    continue;

                XmlNode item = document.CreateNode(XmlNodeType.Element, "Item", string.Empty);
                items.AppendChild(item);

                XmlElement index = document.CreateElement("index");
                index.InnerText = inventoryInfo.itemList[i].item.ItemIdx.ToString();
                item.AppendChild(index);

                XmlElement count = document.CreateElement("count");
                count.InnerText = inventoryInfo.itemList[i].count.ToString();
                item.AppendChild(count);
            }

            #endregion

            #region SpellSave
            XmlNode spells = document.CreateNode(XmlNodeType.Element, "Spells", string.Empty);
            root.AppendChild(spells);

            if (inventoryInfo.spellList == null)
                return;

            for (int i = 0; i < inventoryInfo.spellList.Count; i++)
            {
                if (inventoryInfo.spellList[i] == null)
                    continue;

                XmlNode item = document.CreateNode(XmlNodeType.Element, "Spell", string.Empty);
                spells.AppendChild(item);

                XmlElement index = document.CreateElement("index");
                index.InnerText = inventoryInfo.spellList[i].item.ItemIdx.ToString();
                item.AppendChild(index);

                XmlElement count = document.CreateElement("count");
                count.InnerText = inventoryInfo.spellList[i].count.ToString();
                item.AppendChild(count);
            }
            #endregion

            #region EqiupsSave

            XmlNode equips = document.CreateNode(XmlNodeType.Element, "Equips", string.Empty);
            root.AppendChild(equips);

            XmlElement weapon = document.CreateElement("Weapon");
            weapon.InnerText = inventoryInfo.curWeaponIdx.ToString();
            equips.AppendChild(weapon);

            XmlElement subWeapon = document.CreateElement("SubWeapon");
            subWeapon.InnerText = inventoryInfo.subWeaponIdx.ToString();
            equips.AppendChild(subWeapon);

            XmlElement body = document.CreateElement("Body");
            body.InnerText = inventoryInfo.curBodyIdx.ToString();
            equips.AppendChild(body);

            XmlElement leg = document.CreateElement("Leg");
            leg.InnerText = inventoryInfo.curLegIdx.ToString();
            equips.AppendChild(leg);

            XmlElement arm = document.CreateElement("Arm");
            arm.InnerText = inventoryInfo.curArmIdx.ToString();
            equips.AppendChild(arm);

            XmlElement head = document.CreateElement("Head");
            head.InnerText = inventoryInfo.curHeadIdx.ToString();
            equips.AppendChild(head);

            #endregion
            //document.Save("./Assets/Resources/Data/Inventory.xml");
            File.WriteAllText(Application.streamingAssetsPath + "/Data/Inventory.xml", document.OuterXml);
        }

        public void SaveBoxInfo(BoxInfo boxInfo)
        {
            m_boxInfo = boxInfo;

            XmlDocument document = new XmlDocument();
            document.AppendChild(document.CreateXmlDeclaration("1.0", "utf-8", "yes"));

            XmlNode root = document.CreateNode(XmlNodeType.Element, "BoxInfo", string.Empty);
            document.AppendChild(root);

            XmlNode items = document.CreateNode(XmlNodeType.Element, "Items", string.Empty);
            root.AppendChild(items);

            if (boxInfo.itemList == null)
                return;
            for (int i = 0; i < boxInfo.itemList.Count; i++)
            {
                if (boxInfo.itemList[i].item == null)
                    return;

                XmlNode item = document.CreateNode(XmlNodeType.Element, "Item", string.Empty);
                items.AppendChild(item);

                XmlElement index = document.CreateElement("index");
                index.InnerText = boxInfo.itemList[i].item.ItemIdx.ToString();
                item.AppendChild(index);

                XmlElement count = document.CreateElement("count");
                count.InnerText = boxInfo.itemList[i].count.ToString();
                item.AppendChild(count);
            }

            XmlNode equips = document.CreateNode(XmlNodeType.Element, "Equips", string.Empty);
            root.AppendChild(equips);

            for (int i = 0; i < boxInfo.equipmentList.Count; i++)
            {
                XmlNode partNode = document.CreateNode(XmlNodeType.Element, "Parts", string.Empty);
                equips.AppendChild(partNode);

                XmlElement type = document.CreateElement("partsType");
                type.InnerText = boxInfo.equipmentList[i].partsType.ToString();
                partNode.AppendChild(type);

                XmlElement index = document.CreateElement("index");
                index.InnerText = boxInfo.equipmentList[i].index.ToString();
                partNode.AppendChild(index);
            }

            //document.Save("./Assets/Resources/Data/BoxInfo.xml");
            File.WriteAllText(Application.streamingAssetsPath + "/Data/BoxInfo.xml", document.OuterXml);
        }

        void LoadPlayerInfo()
        {
            //TODO: 데이터 읽어 오기
            //TextAsset playerText = Resources.Load<TextAsset>("Data/PlayerInfo");
            string playerText = File.ReadAllText(Application.streamingAssetsPath + "/Data/PlayerInfo.xml");

            //if (playerText == null)
            //    return;

            XmlDocument document = new XmlDocument();
            document.LoadXml(playerText);
            //document.LoadXml(playerText.text);

            XmlNode root = document.SelectSingleNode("PlayerInfo");

            m_playerInfo.name = root.SelectSingleNode("Name").InnerText;
            m_playerInfo.passWord = root.SelectSingleNode("PassWord").InnerText;
            m_playerInfo.gender = (Gender)System.Enum.Parse(typeof(Gender), root.SelectSingleNode("Gender").InnerText);

        }

        void LoadInvenToryData()
        {
            //TextAsset inventoryText = Resources.Load<TextAsset>("Data/Inventory");
            string inventoryText = File.ReadAllText(Application.streamingAssetsPath + "/Data/Inventory.xml");

            //if (inventoryText == null)
            //    return;

            XmlDocument document = new XmlDocument();
            document.LoadXml(inventoryText);
            //document.LoadXml(inventoryText.text);

            XmlNode root = document.SelectSingleNode("Inventory");

            #region ItemLoad

            XmlNodeList items = root.SelectNodes("Items/Item");

            for (int i = 0; i < items.Count; i++)
            {
                int nIdx = int.Parse(items[i].SelectSingleNode("index").InnerText);
                int nCount = int.Parse(items[i].SelectSingleNode("count").InnerText);

                CItemBase item = m_itemContainier.GetItem(nIdx);

                if (item == null)
                    continue;

                ItemData itemData = new ItemData();
                itemData.item = item;
                itemData.count = nCount;

                m_inventoryInfo.itemList.Add(itemData);
            }

            #endregion

            #region SpellLoad

            XmlNodeList spells = root.SelectNodes("Spells/Spell");

            for (int i = 0; i < spells.Count; i++)
            {
                int nIdx = int.Parse(spells[i].SelectSingleNode("index").InnerText);
                int nCount = int.Parse(spells[i].SelectSingleNode("count").InnerText);

                CItemBase item = m_itemContainier.GetItem(nIdx);

                if (item == null)
                    continue;

                if (!(item is CSpellItem))
                    continue;

                ItemData itemData = new ItemData();
                itemData.item = item;
                itemData.count = nCount;

                m_inventoryInfo.spellList.Add(itemData);
            }

            #endregion

            #region EquipLoad
            XmlNode equips = root.SelectSingleNode("Equips");

            XmlNode weapon = equips.SelectSingleNode("Weapon");
            m_inventoryInfo.curWeaponIdx = int.Parse(weapon.InnerText);

            XmlNode subWeapon = equips.SelectSingleNode("SubWeapon");
            m_inventoryInfo.subWeaponIdx = int.Parse(subWeapon.InnerText);

            XmlNode body = equips.SelectSingleNode("Body");
            m_inventoryInfo.curBodyIdx = int.Parse(body.InnerText);

            XmlNode leg = equips.SelectSingleNode("Leg");
            m_inventoryInfo.curLegIdx = int.Parse(leg.InnerText);

            XmlNode head = equips.SelectSingleNode("Head");
            m_inventoryInfo.curHeadIdx = int.Parse(head.InnerText);

            XmlNode arm = equips.SelectSingleNode("Arm");
            m_inventoryInfo.curArmIdx = int.Parse(arm.InnerText);
            #endregion

        }

        void LoadBoxInfo()
        {
            //TextAsset boxText = Resources.Load<TextAsset>("Data/BoxInfo");
            string boxText = File.ReadAllText(Application.streamingAssetsPath + "/Data/BoxInfo.xml");

            //if (inventoryText == null)
            //    return;

            XmlDocument document = new XmlDocument();
            document.LoadXml(boxText);
            //document.LoadXml(boxText.text);

            XmlNode root = document.SelectSingleNode("BoxInfo");
            XmlNodeList items = root.SelectNodes("Items/Item");

            for (int i = 0; i < items.Count; i++)
            {
                int nIdx = int.Parse(items[i].SelectSingleNode("index").InnerText);
                int nCount = int.Parse(items[i].SelectSingleNode("count").InnerText);

                CItemBase item = m_itemContainier.GetItem(nIdx);

                if (item == null)
                    continue;

                ItemData itemData = new ItemData();
                itemData.item = item;
                itemData.count = nCount;

                m_boxInfo.itemList.Add(itemData);
            }

            XmlNode equips = root.SelectSingleNode("Equips");
            XmlNodeList parts = equips.SelectNodes("Parts");

            for (int i = 0; i < parts.Count; i++)
            {
                XmlNode type = parts[i].SelectSingleNode("partsType");
                XmlNode index = parts[i].SelectSingleNode("index");

                EquipmentBoxInfo equipment = new EquipmentBoxInfo();

                equipment.partsType = (PartsType)System.Enum.Parse(typeof(PartsType), type.InnerText);
                equipment.index = int.Parse(index.InnerText);

                m_boxInfo.equipmentList.Add(equipment);

            }
        }

    }

    public class UnitContainer
    {
        private Dictionary<UnitType, Dictionary<int, string>> m_unitData = new Dictionary<UnitType, Dictionary<int, string>>();

        public void Init()
        {
            //TODO: 데이터 파싱
            //TextAsset unitDatasText = Resources.Load<TextAsset>("Data/UnitData");
            string unitDatasText = File.ReadAllText(Application.streamingAssetsPath + "/Data/UnitData.xml");

            //if (equipDatasText == null)
            //    return;

            XmlDocument document = new XmlDocument();
            document.LoadXml(unitDatasText);
            //document.LoadXml(unitDatasText.text);

            XmlNode root = document.SelectSingleNode("UnitData");

            XmlNode npc = root.SelectSingleNode("Npc");
            XmlUnitLoad(UnitType.Npc, npc);

            XmlNode enemy = root.SelectSingleNode("Enemy");
            XmlUnitLoad(UnitType.Enemy, enemy);

            XmlNode boss = root.SelectSingleNode("Boss");
            XmlUnitLoad(UnitType.Boss, boss);
        }

        void XmlUnitLoad(UnitType type, XmlNode parent)
        {
            XmlNodeList unitlist = parent.SelectNodes("Unit");

            Dictionary<int, string> unitMap = new Dictionary<int, string>();

            for (int i = 0; i < unitlist.Count; i++)
            {
                XmlNode idx = unitlist[i].SelectSingleNode("Index");
                XmlNode name = unitlist[i].SelectSingleNode("Name");

                int nIdx = int.Parse(idx.InnerText);
                string nameText = name.InnerText;

                unitMap.Add(nIdx, nameText);
            }

            m_unitData.Add(type, unitMap);
        }

        public int GetUnitCount(UnitType type)
        {
            if (m_unitData[type] == null)
                return 0;

            return m_unitData[type].Count;
        }

        public CUnitData GetUnitData(UnitType type, int nIdx)
        {
            if (!m_unitData[type].ContainsKey(nIdx))
            {
                return null;
            }

            string name = m_unitData[type][nIdx];
            string path = "Scriptable/Unit/";
            switch (type)
            {
                case UnitType.Npc:
                    path += "Npc/";
                    break;
                case UnitType.Enemy:
                    path += "Enemy/";
                    break;
                case UnitType.Boss:
                    path += "Boss/";
                    break;
            }
            path += name;

            CUnitData enemy = Resources.Load<CUnitData>(path);

            return enemy;
        }
    }

    [System.Serializable]
    public class EffectContainier
    {
        [SerializeField]
        private Dictionary<EffectType, Dictionary<int, string>> m_effectData = new Dictionary<EffectType, Dictionary<int, string>>();

        public void Init()
        {
            //TextAsset textAsset = Resources.Load<TextAsset>("Data/EffectData");
            string effectText = File.ReadAllText(Application.streamingAssetsPath + "/Data/EffectData.xml");

            //if (textAsset == null)
            //    return;

            XmlDocument document = new XmlDocument();
            document.LoadXml(effectText);
            //document.LoadXml(textAsset.text);

            XmlNode root = document.SelectSingleNode("EffectData");

            XmlNode particle = root.SelectSingleNode("Particle");
            AddDataMap(EffectType.Particle, particle);

            XmlNode projectile = root.SelectSingleNode("Projectile");
            AddDataMap(EffectType.Projectile, projectile);
        }

        void AddDataMap(EffectType type, XmlNode parent)
        {
            XmlNodeList list = parent.SelectNodes("Effect");

            Dictionary<int, string> dictionary = new Dictionary<int, string>();

            for (int i = 0; i < list.Count; i++)
            {
                XmlNode index = list[i].SelectSingleNode("Index");
                XmlNode name = list[i].SelectSingleNode("Name");

                int nIdx = int.Parse(index.InnerText);

                dictionary.Add(nIdx, name.InnerText);
            }

            m_effectData.Add(type, dictionary);
        }

        public int GetEffectCount(EffectType type)
        {
            if (m_effectData[type] == null)
                return 0;

            return m_effectData[type].Count;
        }

        public GameObject GetEffects(EffectType type, int nIdx)
        {
            GameObject effects = null;

            string name = m_effectData[type][nIdx];
            string path = "Prefabs/Effects/" + (type == EffectType.Particle ? "Particle/" : "Projectile/");
            path += name;

            effects = Resources.Load<GameObject>(path);

            return effects;
        }
    }

    [System.Serializable]
    public class QuestContainer
    {
        private List<QuestInfo> m_questInfos = new List<QuestInfo>();
        public int QuestCount { get { return m_questInfos.Count; } }

        public void Init()
        {
            //TextAsset textAsset = Resources.Load<TextAsset>("Data/QuestInfos");
            string questText = File.ReadAllText(Application.streamingAssetsPath + "/Data/QuestInfos.xml");

            //if (textAsset == null)
            //    return;

            XmlDocument document = new XmlDocument();
            document.LoadXml(questText);
            //document.LoadXml(textAsset.text);

            XmlNode root = document.SelectSingleNode("QuestInfos");
            XmlNodeList quests = root.SelectNodes("Quest");

            for (int i = 0; i < quests.Count; i++)
            {
                QuestInfo info = new QuestInfo();

                XmlNode name = quests[i].SelectSingleNode("Name");
                info.questName = name.InnerText;

                XmlNode areaName = quests[i].SelectSingleNode("AreaName");
                info.areaName = areaName.InnerText;

                XmlNode targetType = quests[i].SelectSingleNode("Type");
                info.mainTargetType = (UnitType)int.Parse(targetType.InnerText);

                XmlNode targetIdx = quests[i].SelectSingleNode("TargetIndex");
                info.mainTargetIdx = int.Parse(targetIdx.InnerText);

                XmlNode targetCount = quests[i].SelectSingleNode("TargetCount");
                info.mainTargetCount = int.Parse(targetCount.InnerText);

                XmlNode timeLimit = quests[i].SelectSingleNode("TimeLimit");
                info.timeLimit = float.Parse(timeLimit.InnerText);

                m_questInfos.Add(info);
            }

        }

        public QuestInfo? GetQuest(int nIdx)
        {
            if (m_questInfos.Count < 1)
                return null;

            if (m_questInfos.Count <= nIdx)
                return null;

            return m_questInfos[nIdx];
        }
    }

    [System.Serializable]
    public class AreaContainer
    {
        private Dictionary<string, AreaInfo> m_areaInfos = new Dictionary<string, AreaInfo>();

        public void Init()
        {
            //TextAsset textAsset = Resources.Load<TextAsset>("Data/AreaInfos");
            string areaText = File.ReadAllText(Application.streamingAssetsPath + "/Data/AreaInfos.xml");

            //if (textAsset == null)
            //    return;

            XmlDocument document = new XmlDocument();
            document.LoadXml(areaText);
            //document.LoadXml(textAsset.text);

            XmlNode root = document.SelectSingleNode("AreaInfos");
            XmlNodeList areas = root.SelectNodes("Area");

            for (int i = 0; i < areas.Count; i++)
            {
                AreaInfo info = new AreaInfo();

                XmlNode name = areas[i].SelectSingleNode("Name");
                info.areaName = name.InnerText;

                XmlNode fullName = areas[i].SelectSingleNode("FullName");
                info.FullName = fullName.InnerText;

                XmlNode skyboxName = areas[i].SelectSingleNode("SkyboxName");
                info.skyBoxName = skyboxName.InnerText;

                XmlNode targetIdx = areas[i].SelectSingleNode("SubAreaCount");
                info.subAreaCount = int.Parse(targetIdx.InnerText);

                m_areaInfos.Add(info.areaName, info);
            }
        }

        public AreaInfo? GetAreaInfo(string name)
        {
            if (!m_areaInfos.ContainsKey(name))
                return null;

            return m_areaInfos[name];
        }
    }
}