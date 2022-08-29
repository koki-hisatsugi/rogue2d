using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MyScriptable/Create ItemMaster")]
public class ItemMasterList : ScriptableObject
{
    public List<ItemPram> Item;
    [System.Serializable]
    public class ItemPram
    {
        public int ID;
        public string ItemName;
        public string ItemInfo;
        public ItemType InItemType;
        public EquipType InEquipType;
        public CureType InCureType;

        public enum ItemType
        {
            Cure,
            Equip,
            Nomal
        }

        public enum EquipType
        {
            Weapon,
            Armor
        }

        public enum CureType
        {
            Hp,
            Stamina
        }
    }
}
