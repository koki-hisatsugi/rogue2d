using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDataObj : MonoBehaviour
{
    [SerializeField] int _x, _y;
    public int GetSetMap_x
    {
        get { return _x; }
        set { _x = value; }
    }
    public int GetSetMap_y
    {
        get { return _y; }
        set { _y = value; }
    }

    int _value;
    public int GetSetMap_value
    {
        get { return _value; }
        set { _value = value; }
    }
    [SerializeField] private string _RoomNameObj;
    public enum TileAttributeObj
    {
        wall,
        road,
        floor
    }

    [UnityEngine.SerializeField] private TileAttributeObj _ThisTileAttributeObj;
    public string GetSetRoomName
    {
        get { return _RoomNameObj; }
        set { _RoomNameObj = value; }
    }


    public TileAttributeObj GetSetTileAttribute
    {
        get { return _ThisTileAttributeObj; }
        set { _ThisTileAttributeObj = value; }
    }

    public void setActiveObj(string onoff)
    {
        switch (onoff)
        {
            case "ON":
                this.gameObject.SetActive(true);
                break;
            case "OFF":
                this.gameObject.SetActive(false);
                break;
        }
    }
}
