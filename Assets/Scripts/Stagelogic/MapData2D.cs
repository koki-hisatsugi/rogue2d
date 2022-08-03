[System.Serializable]
public class MapData2D
{
    [UnityEngine.SerializeField] private int _MapValue;
    [UnityEngine.SerializeField] private int _MapOnActor;
    [UnityEngine.SerializeField] private string _RoomName;
    public enum TileAttribute
    {
        wall,
        road,
        floor
    }

    [UnityEngine.SerializeField] private TileAttribute _ThisTileAttribute;

    public MapData2D(int value, string floorName, TileAttribute tileAttribute){
        GetSetMapValue = value;
        GetSetRoomName = floorName;
        GetSetTileAttribute = tileAttribute;
    }

    public int GetSetMapValue
    {
        get { return _MapValue; }
        set { _MapValue = value; }
    }

    public int GetSetMapOnActor
    {
        get { return _MapOnActor; }
        set { _MapOnActor = value; }
    }

    public string GetSetRoomName
    {
        get { return _RoomName; }
        set { _RoomName = value; }
    }

    
    public TileAttribute GetSetTileAttribute
    {
        get { return _ThisTileAttribute; }
        set { _ThisTileAttribute = value; }
    }
}
