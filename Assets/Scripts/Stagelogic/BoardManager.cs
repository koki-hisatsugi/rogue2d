using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public int colums = 10, rows = 10;
    private List<Vector3> _gridPositions = new List<Vector3>();
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] outWallTiles;
    public GameObject[] enemyTiles;

    public GameObject player;

    public GameObject enemy;
    public GameObject exit;

    [SerializeField] private GameObject _MainCamera;

    public int wallMinimum = 7, wallMaximun = 10, foodMinimum = 1, FoodMaximun = 5;
    public int dungeonSize = 80;
    
    public int dungeonSizeW = 50;
    
    public int dungeonSizeH = 50;
    public List<GameObject> Walllist = new List<GameObject>();
    public List<GameObject> Enemylist = new List<GameObject>();
    [SerializeField] private GameObject _Field;
    public GameObject Enemys;
    [SerializeField] private GameObject _MapFlame;
    [SerializeField] private GameObject _MapElement;

    public List<GameObject> _RoomMasks;
    public List<Rect2D> _Rooms;

    public Dictionary<string,GameObject> MapDic;

    void InitialiseList()
    {
        _gridPositions.Clear();

        for (int x = 1; x < colums - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                _gridPositions.Add(new Vector3(x, y, 0));
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, _gridPositions.Count);
        Vector3 randomPosition = _gridPositions[randomIndex];
        _gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectRandom(GameObject[] tileArray, int min, int max)
    {
        int objectCount = Random.Range(min, max + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public Array2D SetupScene(int floorLevel)
    {
        InitialiseList();
        _MainCamera = GameObject.Find("Main Camera");
        RandomDungeon rDungeon = new RandomDungeon();
        Array2D a2d = rDungeon.Create(dungeonSize, dungeonSize);
        _RoomMasks = rDungeon.Get_roomMasks;
        _Rooms = rDungeon.Get_rooms;
        _Field = new GameObject("Field");
        Enemys = new GameObject("Enemys");
        Enemys.transform.parent = _Field.transform;


        for (int i = 0; i < dungeonSize; i++)
        {
            for (int j = 0; j < dungeonSize; j++)
            {
                //Debug.Log("i=" + i + "/j="+j+":" + a2d.Get(i, j));
                if (a2d.Get(i, j).GetSetMapValue == BoardRemote.WallNum)
                {
                    GameObject tileChoice = outWallTiles[Random.Range(0, outWallTiles.Length)];
                    Walllist.Add(Instantiate(tileChoice, new Vector3(i, j, 0), Quaternion.identity, _Field.transform));
                }
                else
                {
                    GameObject tileChoice = floorTiles[Random.Range(0, outWallTiles.Length)];
                    //Instantiate(tileChoice, new Vector3(i, j, 0), Quaternion.identity);
                    Walllist.Add(Instantiate(tileChoice, new Vector3(i, j, 0), Quaternion.identity, _Field.transform));

                    if (a2d.Get(i, j).GetSetMapOnActor == BoardRemote.PlayerNum)
                    {
                        player.transform.position = new Vector3(i, j, 0);
                        _MainCamera.transform.position = new Vector3(i, j, -10);
                        ActorManager player_ActorManager = player.GetComponent<ActorManager>();
                        player_ActorManager.GetSet_ActorPosX = i;
                        player_ActorManager.GetSet_ActorPosY = j;
                        // a2d.Set(i, j, 0);
                    }
                    else if (a2d.Get(i, j).GetSetMapValue == BoardRemote.StairsNum)
                    {
                        tileChoice = exit;
                        Walllist.Add(Instantiate(tileChoice, new Vector3(i, j, 0), Quaternion.identity, _Field.transform));
                    }
                    else if (a2d.Get(i, j).GetSetMapValue == BoardRemote.foodNum)
                    {
                        tileChoice = foodTiles[Random.Range(0, foodTiles.Length)];
                        Walllist.Add(Instantiate(tileChoice, new Vector3(i, j, 0), Quaternion.identity, _Field.transform));
                        // a2d.Get(i, j).GetSetMapValue = 0;
                        // a2d.Get(i, j).GetSetMapOnActor = BoardRemote.foodNum;
                    }
                    else if (a2d.Get(i, j).GetSetMapOnActor == BoardRemote.EnemyNum)
                    {
                        tileChoice = enemyTiles[Random.Range(0, enemyTiles.Length)];
                        GameObject tmpEnemy = Instantiate(tileChoice, new Vector3(i, j, 0), Quaternion.identity, Enemys.transform);
                        ActorManager tmpEnemy_ActorManager = tmpEnemy.GetComponent<ActorManager>();
                        tmpEnemy_ActorManager.GetSet_ActorPosX = i;
                        tmpEnemy_ActorManager.GetSet_ActorPosY = j;
                        tmpEnemy_ActorManager.GetSet_Level = Random.Range(floorLevel,floorLevel+3);
                        tmpEnemy_ActorManager.setEnemyStatus();
                        Enemylist.Add(tmpEnemy);
                        // enemy.transform.position = new Vector3(i, j, 0);
                    }
                }
            }
        }
        return a2d;
    }
    public void EnemyListRemove(){
        Enemylist.Remove(null);
    }
    public List<GameObject> EnemySpawn(int floorLevel){
        Enemylist.Clear();
        GameObject tileChoice = enemyTiles[Random.Range(0, enemyTiles.Length)];
        
        // エネミーの発生数
        int randomSpawnNum = UnityEngine.Random.Range(1, 3);
        for(int i = 0; i <= randomSpawnNum; i++){
            int randomNum = Random.Range(0, _Rooms.Count -1);
            int ePosX = Random.Range(_Rooms[randomNum].left, _Rooms[randomNum].right);
            int ePosY = Random.Range(_Rooms[randomNum].bottom, _Rooms[randomNum].top);
            GameObject tmpEnemy = Instantiate(tileChoice, new Vector3(ePosX, ePosY, 0), Quaternion.identity, Enemys.transform);
            ActorManager tmpEnemy_ActorManager = tmpEnemy.GetComponent<ActorManager>();
            tmpEnemy_ActorManager.GetSet_ActorPosX = ePosX;
            tmpEnemy_ActorManager.GetSet_ActorPosY = ePosY;
            tmpEnemy_ActorManager.GetSet_Level = Random.Range(floorLevel,floorLevel+3);
            tmpEnemy_ActorManager.setEnemyStatus();
            Enemylist.Add(tmpEnemy);
        }

        return Enemylist;
    }

    public List<GameObject> mapping(Array2D a2d)
    {
        Dictionary<string,GameObject> rooms = new Dictionary<string, GameObject>();
        MapDic = new Dictionary<string, GameObject>();
        List<GameObject> resultMapObj = new List<GameObject>();
        GameObject _canvas = GameObject.Find("Canvas");

        _MapFlame = _canvas.transform.Find("MapFlame").gameObject;
        _MapFlame.GetComponent<Image>().color = new Color32(0, 0, 0, 0); // 無色

        _MapElement = new GameObject("Inner");
        _MapElement.AddComponent(typeof(CanvasRenderer));
        _MapElement.AddComponent(typeof(RectTransform));
        _MapElement.AddComponent(typeof(Image));
        _MapElement.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);
        _MapElement.GetComponent<Image>().color = new Color32(255, 255, 255, 90);

        for (int i = 0; i < dungeonSize; i++)
        {
            for (int j = 0; j < dungeonSize; j++)
            {
                if (a2d.Get(i, j).GetSetMapValue != 1)
                {
                    GameObject tInner = Instantiate(_MapElement, new Vector3(0, 0, 0), Quaternion.identity, _MapFlame.transform);
                    // サイズを5倍にしているので、配置する位置も5を掛ける
                    // X座標は、-100して調整している　右によりすぎてしまうため
                    tInner.GetComponent<RectTransform>().localPosition = new Vector3((i*5)-100, j*5, 0);
                    if(!rooms.ContainsKey(a2d.Get(i, j).GetSetRoomName)){
                        GameObject troom = new GameObject(a2d.Get(i, j).GetSetRoomName);
                        troom.AddComponent(typeof(RectTransform));
                        troom.transform.parent = _MapFlame.transform;
                        rooms.Add(a2d.Get(i, j).GetSetRoomName, troom);
                    }

                    if (a2d.Get(i, j).GetSetMapValue == BoardRemote.StairsNum){
                        tInner.GetComponent<Image>().color = new Color32(14, 209, 69 , 90);
                    }

                    if (a2d.Get(i, j).GetSetMapOnActor == BoardRemote.PlayerNum){
                        tInner.GetComponent<Image>().color = new Color32(255, 0, 0, 90);
                    } else if (a2d.Get(i, j).GetSetMapOnActor == BoardRemote.EnemyNum){
                        tInner.GetComponent<Image>().color = new Color32(0, 0, 255, 90);
                    }

                    tInner.transform.parent = rooms[a2d.Get(i, j).GetSetRoomName].transform;
                    Debug.Log(tInner);
                    MapDic.Add(i +":"+ j,tInner);
                    tInner.gameObject.SetActive(false);
                    resultMapObj.Add(tInner);
                }
            }
        }
        Destroy(_MapElement);
        return resultMapObj;
    }

    public void delwall()
    {
        Destroy(_Field);
        Walllist.Clear();
        Enemylist.Clear();
        if(_RoomMasks != null){
            foreach(GameObject mask in _RoomMasks){
                Destroy(mask);
            }
        }
    }

    public void dellMap()
    {
        if(_MapFlame!=null){
            // _MapFlameがまだセットされていない場合があるので
            foreach(Transform child in _MapFlame.transform){
                Destroy(child.gameObject);
            }
        }
        // Destroy(_MapFlame);
    }
}
