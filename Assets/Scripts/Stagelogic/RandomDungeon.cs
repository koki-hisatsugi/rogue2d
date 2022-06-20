
using System.Collections.Generic;
using UnityEngine;

public class RandomDungeon
{
    private const int minArea = 8;
    private const int minRoom = 2;
    private const int margin = 1;
    private Array2D data;
    private List<Area2D> areas;
    private List<Rect2D> rooms;

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
        Area2D area = new Area2D();
        area.outLine = new Rect2D(0, 0, w - 1, h - 1);
        Split(area, Random.Range(0, 2) == 0);
        CreateRooms();
        CreateRoads();
        createSingleObj(2);
        createSingleObj(3);
        createSingleObj(5);
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
            if (baseArea.outLine.left + minArea >= baseArea.outLine.right - minArea)
            {
                areas.Add(baseArea);
                return;
            }
            int p = Random.Range(baseArea.outLine.left + minArea, baseArea.outLine.right - minArea);
            rect1 = new Rect2D(baseArea.outLine.left, baseArea.outLine.top, p, baseArea.outLine.bottom);
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
            if (baseArea.outLine.top + minArea >= baseArea.outLine.bottom - minArea)
            {
                areas.Add(baseArea);
                return;
            }
            int p = Random.Range(baseArea.outLine.top + minArea, baseArea.outLine.bottom - minArea);
            rect1 = new Rect2D(baseArea.outLine.left, baseArea.outLine.top, baseArea.outLine.right, p);
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
            int minW = aw / Random.Range(2, 6);
            int minH = ah / Random.Range(2, 6);
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
            roomNumber++;
            int itemQuantity = Random.Range(0, 4);
            createMaltiObj(4, itemQuantity, area);
            int enemyQuantity = Random.Range(0, 2);
            createMaltiObj(5, enemyQuantity, area);
        }
        rooms = tmprooms;
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
        string roomRoad = null;
        for (int x = area1.room.right; x < area1.outLine.right; x++)
        {
            data.Get(x, y1).GetSetMapValue = 0;
            if (data.Get(x, y1).GetSetTileAttribute == MapData2D.TileAttribute.floor)
            {
                roomRoad = data.Get(x, y1).GetSetRoomName;
            }
            else
            {
                if (x <= 2)
                {
                    Debug.Log(roomRoad);
                    data.Get(x, y1).GetSetRoomName = roomRoad;
                    Debug.Log(data.Get(x, y1).GetSetRoomName);
                }
                else
                {
                    
                    Debug.Log(data.Get(x, y1).GetSetRoomName);
                    data.Get(x, y1).GetSetRoomName = "Road";
                }
                data.Get(x, y1).GetSetTileAttribute = MapData2D.TileAttribute.road;
            }
        }
        for (int x = area2.outLine.left; x < area2.room.left; x++)
        {
            data.Get(x, y2).GetSetMapValue = 0;
            if (
            data.Get(x, y2).GetSetTileAttribute == MapData2D.TileAttribute.floor)
            {
                roomRoad = data.Get(x, y1).GetSetRoomName;
            }
            else
            {
                if (x <= 2)
                {
                    data.Get(x, y2).GetSetRoomName = roomRoad;
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
            data.Get(area1.outLine.right, y).GetSetMapValue = 0;
            if (data.Get(area1.outLine.right, y).GetSetTileAttribute != MapData2D.TileAttribute.floor)
            {
                data.Get(area1.outLine.right, y).GetSetRoomName = "Road";
                data.Get(area1.outLine.right, y).GetSetTileAttribute = MapData2D.TileAttribute.road;
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
        string roomRoad = null;
        for (int y = area1.room.bottom; y < area1.outLine.bottom; y++)
        {
            data.Get(x1, y).GetSetMapValue = 0;
            if (data.Get(x1, y).GetSetTileAttribute == MapData2D.TileAttribute.floor)
            {
                roomRoad = data.Get(x1, y).GetSetRoomName;
            }
            else
            {
                if (y <= 2)
                {
                    data.Get(x1, y).GetSetRoomName = roomRoad;
                }
                else
                {
                    data.Get(x1, y).GetSetRoomName = "Road";
                }
                data.Get(x1, y).GetSetTileAttribute = MapData2D.TileAttribute.road;
            }
        }
        for (int y = area2.outLine.top; y < area2.room.top; y++)
        {
            data.Get(x2, y).GetSetMapValue = 0;
            if (data.Get(x2, y).GetSetTileAttribute == MapData2D.TileAttribute.floor)
            {
                roomRoad = data.Get(x2, y).GetSetRoomName;
            }
            else
            {
                if (y <= 2)
                {
                    data.Get(x2, y).GetSetRoomName = roomRoad;
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
            data.Get(x, area1.outLine.bottom).GetSetMapValue = 0;
            if (data.Get(x, area1.outLine.bottom).GetSetTileAttribute != MapData2D.TileAttribute.floor)
            {
                data.Get(x, area1.outLine.bottom).GetSetRoomName = "Road";
                data.Get(x, area1.outLine.bottom).GetSetTileAttribute = MapData2D.TileAttribute.road;
            }
        }
    }

    /**
    * 道を作成
    */
    private void CreateRoads()
    {

        for (int i = 0; i < areas.Count - 1; i++)
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
                data.Get(institutionX, institutionY).GetSetMapValue = num;
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
                    data.Get(institutionX, institutionY).GetSetMapValue = num;
                    break;
                }
            }
        }
    }
}