
using System.Collections.Generic;
using UnityEngine;

public class RandomDungeon
{
    private const int minArea = 6;
    private const int minRoom = 2;
    private const int margin = 1;
    private Array2D data;
    private List<Area2D> areas;
    private List<Rect2D> rooms;
    public List<Rect2D> Get_rooms{
        get { return rooms; }
    }
    private List<GameObject> roomMasks;
    public List<GameObject> Get_roomMasks
    {
        get { return roomMasks; }
    }

    /**
    * ダンジョンを作成する
    */
    public Array2D Create(int w, int h)
    {
        data = new Array2D(w, h);
        for (int x = 0; x < data.width; x++)
        {
            for (int y = 0; y < data.height; y++)
            {
                data.Set(x, y, new MapData2D(1, "walls", MapData2D.TileAttribute.wall));
            }
        }
        areas = new List<Area2D>();
        rooms = new List<Rect2D>();
        roomMasks = new List<GameObject>();
        Area2D area = new Area2D();
        area.outLine = new Rect2D(0, 0, w - 1, h - 1);
        // 区画を分割する
        Split(area, Random.Range(0, 2) == 0);
        // 分割した区画に部屋を作成
        CreateRooms();
        // 部屋同士を結ぶ道を作る
        CreateRoads();
        // マップ内に一意のオブジェクトを作成する
        createSingleObj(BoardRemote.PlayerNum);
        createSingleObj(BoardRemote.StairsNum);
        // createSingleObj(5);
        return data;
    }

    /**
    * 区画を分割
    */
    private void Split(Area2D baseArea, bool isVertical)
    {
        Rect2D rect1, rect2;
        if (isVertical)
        {
            // 垂直フラグがTrueの場合、縦に分割する
            if (baseArea.outLine.left + minArea > baseArea.outLine.right - minArea)
            {
                // 区画の大きさが最少を下回っている場合、LISTに詰めて処理を終了する
                areas.Add(baseArea);
                return;
            }
            // 分割点を計算する(乱数)
            int p = Random.Range(baseArea.outLine.left + minArea, baseArea.outLine.right - minArea);
            rect1 = new Rect2D(baseArea.outLine.left, baseArea.outLine.top, p - 1, baseArea.outLine.bottom);
            rect2 = new Rect2D(p + 1, baseArea.outLine.top, baseArea.outLine.right, baseArea.outLine.bottom);
            if ((rect1.width < rect2.width) ||
                (rect1.width == rect2.width && Random.Range(0, 2) == 0))
            {
                Rect2D tmp = rect1;
                rect1 = rect2;
                rect2 = tmp;
            }
        }
        else
        {
            if (baseArea.outLine.top + minArea > baseArea.outLine.bottom - minArea)
            {
                areas.Add(baseArea);
                return;
            }
            int p = Random.Range(baseArea.outLine.top + minArea, baseArea.outLine.bottom - minArea);
            rect1 = new Rect2D(baseArea.outLine.left, baseArea.outLine.top, baseArea.outLine.right, p - 1);
            rect2 = new Rect2D(baseArea.outLine.left, p + 1, baseArea.outLine.right, baseArea.outLine.bottom);
            if ((rect1.height < rect2.height) ||
                (rect1.height == rect2.height && Random.Range(0, 2) == 0))
            {
                Rect2D tmp = rect1;
                rect1 = rect2;
                rect2 = tmp;
            }
        }
        Area2D area1 = new Area2D();
        area1.outLine = rect1;
        Area2D area2 = new Area2D();
        area2.outLine = rect2;
        areas.Add(area2);
        Split(area1, !isVertical);
    }

    /**
    * 部屋を作成する
    */
    private void CreateRooms()
    {
        List<Rect2D> tmprooms = new List<Rect2D>();
        int roomNumber = 1;
        foreach (var area in areas)
        {
            int aw = area.outLine.width - margin * 2;
            int ah = area.outLine.height - margin * 2;
            int minW = aw / Random.Range(3, 6);
            int minH = ah / Random.Range(3, 6);
            int width = Random.Range(minW, aw);
            int height = Random.Range(minH, ah);
            int rw = aw - width;
            int rh = ah - height;
            int rx = Random.Range(margin, rw - margin);
            int ry = Random.Range(margin, rh - margin);
            int left = area.outLine.left + rx;
            int top = area.outLine.top + ry;
            int right = left + width;
            int bottom = top + height;
            area.room = new Rect2D(left, top, right, bottom);
            tmprooms.Add(area.room);
            FillRoom(area.room, "Room" + roomNumber);
            roomMasks.Add(CreateMask(area.room, "Mask_Room" + roomNumber, 2));
            roomNumber++;
            // 部屋に配置するアイテムの数を選定 0~3個をランダム
            int itemQuantity = Random.Range(0, 4);
            // 部屋にアイテムを配置する
            createMaltiObj(BoardRemote.foodNum, itemQuantity, area);
            // 部屋に配置するエネミーの数を選定 0~1個をランダム
            int enemyQuantity = Random.Range(0, 2);
            // 部屋にエネミーを配置する
            createMaltiObj(BoardRemote.EnemyNum, enemyQuantity, area);
        }
        rooms = tmprooms;
    }

    private GameObject CreateMask(Rect2D room, string roomname, float margin){
        GameObject result = new GameObject();
        result.name = roomname;
        result.AddComponent(typeof(SpriteMask));
        result.GetComponent<SpriteMask>().sprite = Resources.Load<Sprite>("MaskSquare");

        float width = room.right - room.left;
        float height = room.bottom - room.top;
        float xpos = (width/2) + room.left;
        float ypos = (height/2) + room.top;

        result.transform.localScale = new Vector3(width + margin,height + margin,1);
        result.transform.position = new Vector3(xpos,ypos,1);

        result.GetComponent<SpriteMask>().enabled = false;

        return result;
    }

    /**
    * マップ配列に部屋を作る
    */
    private void FillRoom(Rect2D room, string roomName)
    {
        for (int x = room.left; x <= room.right; x++)
        {
            for (int y = room.top; y <= room.bottom; y++)
            {
                data.Get(x, y).GetSetMapValue = 0;
                data.Get(x, y).GetSetRoomName = roomName;
                data.Get(x, y).GetSetTileAttribute = MapData2D.TileAttribute.floor;
            }
        }
    }

    /**
    * 水平方向に道をのばす
    */
    private void CreateHorizontalRoad(Area2D area1, Area2D area2)
    {
        int y1 = Random.Range(area1.room.top, area1.room.bottom);
        int y2 = Random.Range(area2.room.top, area2.room.bottom);
        string roomRoad1 = data.Get(area1.room.right, y1).GetSetRoomName;
        string roomRoad2 = data.Get(area2.room.left, y2).GetSetRoomName;
        for (int x = area1.room.right +1; x < area1.outLine.right + 1; x++)
        {
            data.Get(x, y1).GetSetMapValue = 0;
            if (data.Get(x, y1).GetSetTileAttribute == MapData2D.TileAttribute.floor)
            {
                // roomRoad1 = data.Get(x, y1).GetSetRoomName;
            }
            else
            {
                if (x == area1.room.right+1)
                {
                    data.Get(x, y1).GetSetRoomName = roomRoad1;
                }
                else
                {
                    data.Get(x, y1).GetSetRoomName = "Road";
                }
                data.Get(x, y1).GetSetTileAttribute = MapData2D.TileAttribute.road;
            }
        }
        for (int x = area2.outLine.left-1; x < area2.room.left; x++)
        {
            data.Get(x, y2).GetSetMapValue = 0;
            if (
            data.Get(x, y2).GetSetTileAttribute == MapData2D.TileAttribute.floor)
            {
                // roomRoad2 = data.Get(x, y1).GetSetRoomName;
            }
            else
            {
                if (x == area2.room.left-1)
                {
                    data.Get(x, y2).GetSetRoomName = roomRoad2;
                }
                else
                {
                    data.Get(x, y2).GetSetRoomName = "Road";
                }
                data.Get(x, y2).GetSetTileAttribute = MapData2D.TileAttribute.road;
            }
        }
        for (int y = Mathf.Min(y1, y2), end = Mathf.Max(y1, y2); y <= end; y++)
        {
            data.Get(area1.outLine.right+1, y).GetSetMapValue = 0;
            if (data.Get(area1.outLine.right+1, y).GetSetTileAttribute != MapData2D.TileAttribute.floor)
            {
                data.Get(area1.outLine.right+1, y).GetSetRoomName = "Road";
                data.Get(area1.outLine.right+1, y).GetSetTileAttribute = MapData2D.TileAttribute.road;
            }
        }
    }

    /**
    * 垂直方向に道をのばす
    */
    private void CreateVerticalRoad(Area2D area1, Area2D area2)
    {
        int x1 = Random.Range(area1.room.left, area1.room.right);
        int x2 = Random.Range(area2.room.left, area2.room.right);
        string roomRoad1 = data.Get(x1, area1.room.bottom).GetSetRoomName;
        string roomRoad2 = data.Get(x2, area2.room.top).GetSetRoomName;
        for (int y = area1.room.bottom +1; y < area1.outLine.bottom + 1; y++)
        {
            data.Get(x1, y).GetSetMapValue = 0;
            if (data.Get(x1, y).GetSetTileAttribute == MapData2D.TileAttribute.floor)
            {
                // roomRoad1 = data.Get(x1, y).GetSetRoomName;
            }
            else
            {
                if (y == area1.room.bottom+1)
                {
                    data.Get(x1, y).GetSetRoomName = roomRoad1;
                }
                else
                {
                    data.Get(x1, y).GetSetRoomName = "Road";
                }
                data.Get(x1, y).GetSetTileAttribute = MapData2D.TileAttribute.road;
            }
        }
        for (int y = area2.outLine.top - 1; y < area2.room.top; y++)
        {
            data.Get(x2, y).GetSetMapValue = 0;
            if (data.Get(x2, y).GetSetTileAttribute == MapData2D.TileAttribute.floor)
            {
                // roomRoad2 = data.Get(x2, y).GetSetRoomName;
            }
            else
            {
                if (y == area2.room.top-1)
                {
                    data.Get(x2, y).GetSetRoomName = roomRoad2;
                }
                else
                {
                    data.Get(x2, y).GetSetRoomName = "Road";
                }
                data.Get(x2, y).GetSetTileAttribute = MapData2D.TileAttribute.road;
            }
        }
        for (int x = Mathf.Min(x1, x2), end = Mathf.Max(x1, x2); x <= end; x++)
        {
            data.Get(x, area1.outLine.bottom+1).GetSetMapValue = 0;
            if (data.Get(x, area1.outLine.bottom+1).GetSetTileAttribute != MapData2D.TileAttribute.floor)
            {
                data.Get(x, area1.outLine.bottom+1).GetSetRoomName = "Road";
                data.Get(x, area1.outLine.bottom+1).GetSetTileAttribute = MapData2D.TileAttribute.road;
            }
        }
    }

    /**
    * 道を作成
    */
    // private void CreateRoads()
    // {

    //     for (int i = 0; i < areas.Count; i++)
    //     {
    //         if (i != areas.Count -1)
    //         {
    //             if (areas[i].outLine.right < areas[i + 1].outLine.left)
    //                 CreateHorizontalRoad(areas[i], areas[i + 1]);
    //             if (areas[i + 1].outLine.right < areas[i].outLine.left)
    //                 CreateHorizontalRoad(areas[i + 1], areas[i]);
    //             if (areas[i].outLine.bottom < areas[i + 1].outLine.top)
    //                 CreateVerticalRoad(areas[i], areas[i + 1]);
    //             if (areas[i + 1].outLine.bottom < areas[i].outLine.top)
    //                 CreateVerticalRoad(areas[i + 1], areas[i]);
    //         }else{
    //             if (areas[i].outLine.right < areas[0].outLine.left)
    //                 CreateHorizontalRoad(areas[i], areas[0]);
    //             if (areas[0].outLine.right < areas[i].outLine.left)
    //                 CreateHorizontalRoad(areas[0], areas[i]);
    //             if (areas[i].outLine.bottom < areas[0].outLine.top)
    //                 CreateVerticalRoad(areas[i], areas[0]);
    //             if (areas[0].outLine.bottom < areas[i].outLine.top)
    //                 CreateVerticalRoad(areas[0], areas[i]);
    //         }
    //     }
    // }

    private void CreateRoads()
    {

        for (int i = 0; i < areas.Count; i++)
        {
            if (i != areas.Count -1)
            {
                if (areas[i].outLine.right < areas[i + 1].outLine.left)
                    CreateHorizontalRoad(areas[i], areas[i + 1]);
                if (areas[i + 1].outLine.right < areas[i].outLine.left)
                    CreateHorizontalRoad(areas[i + 1], areas[i]);
                if (areas[i].outLine.bottom < areas[i + 1].outLine.top)
                    CreateVerticalRoad(areas[i], areas[i + 1]);
                if (areas[i + 1].outLine.bottom < areas[i].outLine.top)
                    CreateVerticalRoad(areas[i + 1], areas[i]);
            }

            if(Random.Range(0,10) < 6){
                if (i < areas.Count -2)
                {
                    if((areas[i].outLine.right+1 == areas[i + 2].outLine.left-1)
                    ||(areas[i + 2].outLine.right+1 == areas[i].outLine.left-1)
                    ||(areas[i].outLine.bottom+1 == areas[i + 2].outLine.top-1)
                    ||(areas[i + 2].outLine.bottom+1 == areas[i].outLine.top-1)){
                        if (areas[i].outLine.right < areas[i + 2].outLine.left)
                            CreateHorizontalRoad(areas[i], areas[i + 2]);
                        if (areas[i + 2].outLine.right < areas[i].outLine.left)
                            CreateHorizontalRoad(areas[i + 2], areas[i]);
                        if (areas[i].outLine.bottom < areas[i + 2].outLine.top)
                            CreateVerticalRoad(areas[i], areas[i + 2]);
                        if (areas[i + 2].outLine.bottom < areas[i].outLine.top)
                            CreateVerticalRoad(areas[i + 2], areas[i]);
                    }
                }
            }
        }
    }
    public void createSingleObj(int num)
    {
        while (true)
        {
            int institutionRoom = Random.Range(0, areas.Count);
            int institutionX = Random.Range(areas[institutionRoom].room.left, areas[institutionRoom].room.right + 1);
            int institutionY = Random.Range(areas[institutionRoom].room.top, areas[institutionRoom].room.bottom + 1);
            if (data.Get(institutionX, institutionY).GetSetMapValue == 0)
            {
                if(num == BoardRemote.StairsNum){
                    data.Get(institutionX, institutionY).GetSetMapValue = num;
                }else{
                    data.Get(institutionX, institutionY).GetSetMapOnActor = num;
                }
                return;
            }
        }
    }

    public void createMaltiObj(int num, int quantity, Area2D institutionArea)
    {
        for (int i = 0; i < quantity; i++)
        {
            while (true)
            {
                int institutionX = Random.Range(institutionArea.room.left, institutionArea.room.right + 1);
                int institutionY = Random.Range(institutionArea.room.top, institutionArea.room.bottom + 1);
                if (data.Get(institutionX, institutionY).GetSetMapValue == 0)
                {
                    if(num == BoardRemote.StairsNum || num == BoardRemote.foodNum ){
                        data.Get(institutionX, institutionY).GetSetMapValue = num;
                    }else{
                        data.Get(institutionX, institutionY).GetSetMapOnActor = num;
                    }
                    break;
                }
            }
        }
    }
}