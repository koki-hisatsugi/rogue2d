using System.Collections;
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

    public int wallMinimum = 5, wallMaximun = 9, foodMinimum = 1, FoodMaximun = 5;
    public int dungeonSize = 50;
    
    public int dungeonSizeW = 50;
    
    public int dungeonSizeH = 50;
    public List<GameObject> Walllist = new List<GameObject>();
    public List<GameObject> Enemylist = new List<GameObject>();
    [SerializeField] private GameObject _Field;
    public GameObject Enemys;
    [SerializeField] private GameObject _MapFlame;
    [SerializeField] private GameObject _MapElement;

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

    public Array2D SetupScene()
    {
        InitialiseList();
        _MainCamera = GameObject.Find("Main Camera");
        //LayoutObjectRandom(wallTiles, wallMinimum, wallMaximun);
        //LayoutObjectRandom(foodTiles, foodMinimum, FoodMaximun);
        Array2D a2d = new RandomDungeon().Create(dungeonSize, dungeonSize);
        // test = GameObject.Find("Image");
        // Destroy(test2.transform);
        _Field = new GameObject("Field");
        Enemys = new GameObject("Enemys");
        Enemys.transform.parent = _Field.transform;


        for (int i = 0; i < dungeonSize; i++)
        {
            for (int j = 0; j < dungeonSize; j++)
            {
                //Debug.Log("i=" + i + "/j="+j+":" + a2d.Get(i, j));
                if (a2d.Get(i, j).GetSetMapValue == 1)
                {
                    GameObject tileChoice = outWallTiles[Random.Range(0, outWallTiles.Length)];
                    Walllist.Add(Instantiate(tileChoice, new Vector3(i, j, 0), Quaternion.identity, _Field.transform));
                }
                else
                {
                    GameObject tileChoice = floorTiles[Random.Range(0, outWallTiles.Length)];
                    //Instantiate(tileChoice, new Vector3(i, j, 0), Quaternion.identity);
                    Walllist.Add(Instantiate(tileChoice, new Vector3(i, j, 0), Quaternion.identity, _Field.transform));

                    if (a2d.Get(i, j).GetSetMapValue == 2)
                    {
                        player.transform.position = new Vector3(i, j, 0);
                        _MainCamera.transform.position = new Vector3(i, j, -10);
                        // a2d.Set(i, j, 0);
                    }
                    else if (a2d.Get(i, j).GetSetMapValue == 3)
                    {
                        tileChoice = exit;
                        Walllist.Add(Instantiate(tileChoice, new Vector3(i, j, 0), Quaternion.identity, _Field.transform));
                    }
                    else if (a2d.Get(i, j).GetSetMapValue == 4)
                    {
                        tileChoice = foodTiles[Random.Range(0, foodTiles.Length)];
                        Walllist.Add(Instantiate(tileChoice, new Vector3(i, j, 0), Quaternion.identity, _Field.transform));
                        a2d.Get(i, j).GetSetMapValue = 0;
                    }
                    else if (a2d.Get(i, j).GetSetMapValue == 5)
                    {
                        tileChoice = enemyTiles[Random.Range(0, enemyTiles.Length)];
                        Enemylist.Add(Instantiate(tileChoice, new Vector3(i, j, 0), Quaternion.identity, Enemys.transform));
                        // enemy.transform.position = new Vector3(i, j, 0);
                    }
                }
            }
        }
        return a2d;
    }

    public List<GameObject> mapping(Array2D a2d)
    {
        Dictionary<string,GameObject> rooms = new Dictionary<string, GameObject>();
        MapDic = new Dictionary<string, GameObject>();
        List<GameObject> resultMapObj = new List<GameObject>();
        GameObject _canvas = GameObject.Find("Canvas");
        _MapFlame = new GameObject("Image");
        _MapFlame.transform.parent = _canvas.transform;
        _MapFlame.AddComponent(typeof(CanvasRenderer));
        _MapFlame.AddComponent(typeof(RectTransform));
        _MapFlame.AddComponent(typeof(Image));
        _MapFlame.GetComponent<RectTransform>().sizeDelta = new Vector2(160, 160);
        _MapFlame.GetComponent<RectTransform>().localPosition = new Vector2(200, 140);
        _MapFlame.GetComponent<Image>().color = new Color32(137, 137, 137, 100);

        _MapElement = new GameObject("Inner");
        _MapElement.AddComponent(typeof(CanvasRenderer));
        _MapElement.AddComponent(typeof(RectTransform));
        _MapElement.AddComponent(typeof(Image));
        // _MapElement.AddComponent(typeof(MapDataObj));
        _MapElement.GetComponent<RectTransform>().sizeDelta = new Vector2(5, 5);
        _MapElement.GetComponent<Image>().color = new Color32(255, 255, 255, 150);

        for (int i = 0; i < dungeonSize; i++)
        {
            for (int j = 0; j < dungeonSize; j++)
            {
                //Debug.Log("i=" + i + "/j="+j+":" + a2d.Get(i, j));
                if (a2d.Get(i, j).GetSetMapValue != 1)
                {
                    GameObject tInner = Instantiate(_MapElement, new Vector3(0, 0, 0), Quaternion.identity, _MapFlame.transform);
                    tInner.GetComponent<RectTransform>().localPosition = new Vector3((i * 5) - 77.5f, (j * 5) - 77.5f, 0);
                    if(!rooms.ContainsKey(a2d.Get(i, j).GetSetRoomName)){
                        GameObject troom = new GameObject(a2d.Get(i, j).GetSetRoomName);
                        troom.transform.parent = _MapFlame.transform;
                        rooms.Add(a2d.Get(i, j).GetSetRoomName, troom);
                    }
                    if (a2d.Get(i, j).GetSetMapValue == 2)
                    {
                        tInner.GetComponent<Image>().color = new Color32(255, 0, 0, 150);
                    }
                    else if (a2d.Get(i, j).GetSetMapValue == 3)
                    {
                    }
                    else if (a2d.Get(i, j).GetSetMapValue == 4)
                    {
                    }
                    else if (a2d.Get(i, j).GetSetMapValue == 5)
                    {
                        tInner.GetComponent<Image>().color = new Color32(0, 0, 255, 150);
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
    }

    public void dellMap()
    {
        Destroy(_MapFlame);
    }
}
