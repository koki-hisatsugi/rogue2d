using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "MyScriptable/Create ItemMaster")]
public class ItemMasterList : ScriptableObject
{
    public List<ItemParam> Items;

    
    [System.Serializable]
    public class ItemParam
    {
            public int ID;
            public string ItemName;
            public string ItemInfo;
            public ItemType InItemType;
            public EquipType InEquipType;
            public CureType InCureType;
            public int Value;
            public GameObject Prefab;

            public enum ItemType
            {
                Cure,
                Equip,
                Nomal
            }

            public enum EquipType
            {
                Weapon,
                Armor,
                None,
            }

            public enum CureType
            {
                Hp,
                Stamina
            }
    }

    public List<ItemParam> GetItemList(){
        return Items;
    }
}
