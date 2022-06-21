using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using LogManagers;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    BoardManager boardManager;
    // スタミナを消費するまでのターン数
    private int _StaminaCostTurn;
    public Text COSTTEXT;

    private Slider _HPbar;

    [SerializeField] private GameObject player;
    [SerializeField] private ActorMove _ActorMovePlayer;
    [SerializeField] private ActorManager _PlayerActorManager;
    [SerializeField] private GameObject enemy;
    [SerializeField] private ActorMove _ActorMoveEnemy;
    [SerializeField] private ActorManager _EnemyActorManager;

    [SerializeField] private GameObject text;

    [SerializeField] private Array2D _a2d;
    [SerializeField] private Array2D _ma2d;

    public enum GameState
    {
        InputStay,
        MoveFase,
        AttackFase,
        EndFase,
        GameEnd,
    }

    [SerializeField] private GameState _thisGameState;

    private List<GameObject> _Enemylist;
    [SerializeField] private GameObject _Enemys;

    [SerializeField] private List<GameObject> _MapObj;

    private void Awake()
    {
        // ゲームマネージャーをシングルトン化
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        this.gameObject.name = "GameManager";

        // マップ作製クラス(BoardManagerを取得)
        boardManager = GetComponent<BoardManager>();

        player = GameObject.Find("Player");
        _ActorMovePlayer = player.GetComponent<ActorMove>();
        _PlayerActorManager = player.GetComponent<ActorManager>();
        boardManager.player = player;
        _thisGameState = GameState.InputStay;

        // コスト表示用のテキストを取得する
        COSTTEXT = GameObject.Find("FoodText").GetComponent<Text>();
        COSTTEXT.text = "スタミナ:"+_PlayerActorManager.GetStamina;

        // HPバーのスライダーを取得する
        _HPbar = GameObject.Find("HPbar").GetComponent<Slider>();
        _HPbar.value = _PlayerActorManager.GetHP;

        InitGame();
        _Enemylist = boardManager.Enemylist;
        _Enemys = boardManager.Enemys;
        _StaminaCostTurn = 0;
    }

    // async(えいしんく)をつける
    private async void Start()
    {
        _ = UpdateLoop();
    }

    public void InitGame()
    {
        DungeonSet();
    }

    public void DungeonSet()
    {
        boardManager.delwall();
        _a2d = boardManager.SetupScene();
        boardManager.dellMap();
        _MapObj = boardManager.mapping(_a2d);
        AutoMapping();
    }

    public void AutoMapping()
    {
        Dictionary<string, GameObject> Maps = boardManager.MapDic;
        foreach (string key in Maps.Keys)
        {
            Maps[key].GetComponent<Image>().color = new Color32(255, 255, 255, 150);
        }

        for (int h = 0; h < _a2d.height; h++)
        {
            for (int w = 0; w < _a2d.width; w++)
            {
                switch (_a2d.Get(w, h).GetSetMapValue)
                {
                    case 2:
                        Maps[w + ":" + h].GetComponent<Image>().color = new Color32(255, 0, 0, 150);
                        break;
                    case 5:
                        Maps[w + ":" + h].GetComponent<Image>().color = new Color32(0, 0, 255, 150);
                        break;
                }
            }
        }
        for (int h = 0; h < _a2d.height; h++)
        {
            for (int w = 0; w < _a2d.width; w++)
            {
                if (_a2d.Get(w, h).GetSetMapValue == 2)
                {
                    if (_a2d.Get(w, h).GetSetTileAttribute == MapData2D.TileAttribute.floor)
                    {
                        GameObject room = GameObject.Find(_a2d.Get(w, h).GetSetRoomName);
                        foreach (Transform go in room.transform)
                        {
                            go.gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        Maps[w + ":" + h].gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    // フェーズの管理をして各行動を制限する
    async UniTask UpdateLoop()
    {
        while (true)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);  // Unityのupdate関数と同じフレームで
            switch (_thisGameState)
            {
                case GameState.InputStay:
                    await AllActionDecision();
                    break;
                case GameState.MoveFase:
                    await AllMoveExe();
                    break;
                case GameState.AttackFase:
                    await AllAttackExe();
                    break;
                case GameState.EndFase:
                    await EndProcess();
                    break;
            }
        }
    }

    private async UniTask AllActionDecision()
    {
        bool inputTrue = false;
        await UniTask.WaitUntil(() => Input.anyKey);
        // プレイヤーの行動を選定
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            inputTrue = true;
            int horizontal = (int)Input.GetAxisRaw("Horizontal");
            int vertical = (int)Input.GetAxisRaw("Vertical");
            if (horizontal != 0)
            {
                vertical = 0;
            }
            _PlayerActorManager.moveHorizontal = horizontal;
            _PlayerActorManager.moveVatical = vertical;
            _PlayerActorManager.setDir();

            if (_a2d.Get((int)player.transform.position.x + horizontal, (int)player.transform.position.y + vertical).GetSetMapValue != 1 &&
            _a2d.Get((int)player.transform.position.x + horizontal, (int)player.transform.position.y + vertical).GetSetMapValue != 5)
            {
                _a2d.Get((int)player.transform.position.x, (int)player.transform.position.y).GetSetMapValue = 0;
                _a2d.Get((int)player.transform.position.x + horizontal, (int)player.transform.position.y + vertical).GetSetMapValue = 2;
            }
            else
            {
                return;
            }
            _PlayerActorManager.ThisAction = ActorManager.ActorAction.isMove;
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            inputTrue = true;
            _PlayerActorManager.ThisAction = ActorManager.ActorAction.isAttack;
        }

        // エネミーの行動を選定
        if (inputTrue)
        {
            foreach (Transform enemy in _Enemys.transform)
            {
                _ActorMoveEnemy = enemy.gameObject.GetComponent<ActorMove>();
                _EnemyActorManager = enemy.gameObject.GetComponent<ActorManager>();
                EnemyAI EAI = new EnemyAI();
                CoordinateXY _CXY = EAI.AstarAlgo((int)enemy.gameObject.transform.position.x, (int)enemy.gameObject.transform.position.y, _a2d);
                if (_CXY.GetSetX == 0 && _CXY.GetSetY == 0)
                {
                    if (enemy.gameObject.transform.position.x > _CXY.GetSetPX)
                    {
                        _EnemyActorManager.setDir(ActorDirection.LEFT);
                    }
                    else if (enemy.gameObject.transform.position.x < _CXY.GetSetPX)
                    {
                        _EnemyActorManager.setDir(ActorDirection.RIGHT);
                    }
                    else if (enemy.gameObject.transform.position.y > _CXY.GetSetPY)
                    {
                        _EnemyActorManager.setDir(ActorDirection.DOWN);
                    }
                    else if (enemy.gameObject.transform.position.y < _CXY.GetSetPY)
                    {
                        _EnemyActorManager.setDir(ActorDirection.UP);
                    }
                    _EnemyActorManager.ThisAction = ActorManager.ActorAction.isAttack;
                }
                else
                {
                    _EnemyActorManager.ThisAction = ActorManager.ActorAction.isMove;
                    _EnemyActorManager.moveHorizontal = _CXY.GetSetX - (int)enemy.gameObject.transform.position.x;
                    _EnemyActorManager.moveVatical = _CXY.GetSetY - (int)enemy.gameObject.transform.position.y;
                    if (_a2d.Get(_CXY.GetSetX, _CXY.GetSetY).GetSetMapValue != 2
                    && _a2d.Get(_CXY.GetSetX, _CXY.GetSetY).GetSetMapValue != 1
                    && _a2d.Get(_CXY.GetSetX, _CXY.GetSetY).GetSetMapValue != 5)
                    {
                        _a2d.Get((int)enemy.gameObject.transform.position.x, (int)enemy.gameObject.transform.position.y).GetSetMapValue = 0; // 自分がいなくなったマスに0をセットする
                        _a2d.Get(_CXY.GetSetX, _CXY.GetSetY).GetSetMapValue = 5; // 5をセットする
                    }
                    else
                    {
                        _EnemyActorManager.ThisAction = ActorManager.ActorAction.isNone;
                    }

                    // _EnemyActorManager.ActorMoving(_CXY.GetSetX - (int)enemy.transform.position.x, _CXY.GetSetY - (int)enemy.transform.position.y);
                }
            }
        }

        _thisGameState = GameState.MoveFase;
        return;

    }

    private async UniTask AllMoveExe()
    {
        // プレイヤーの移動を行う
        if (_PlayerActorManager.ThisAction == ActorManager.ActorAction.isMove)
        {
            _PlayerActorManager.Action();
            _StaminaCostTurn++; // スタミナを消費するターンを加算
        }
        // エネミーの移動を行う
        foreach (Transform enemy in _Enemys.transform)
        {
            _ActorMoveEnemy = enemy.gameObject.GetComponent<ActorMove>();
            _EnemyActorManager = enemy.gameObject.GetComponent<ActorManager>();
            if (_EnemyActorManager.ThisAction == ActorManager.ActorAction.isMove)
            {
                _EnemyActorManager.Action();
            }
        }
        // プレイヤーとエネミーが移動し終わるのを待つ
        await UniTask.WaitUntil(() => _ActorMovePlayer.isMoveing == false && _ActorMoveEnemy.isMoveing == false);
        // プレイヤーのアクションをNoneにする
        if (_PlayerActorManager.ThisAction == ActorManager.ActorAction.isMove)
        {
            _PlayerActorManager.ThisAction = ActorManager.ActorAction.isNone;
        }
        // 全てのエネミーのアクションをNoneにする
        foreach (Transform enemy in _Enemys.transform)
        {
            _ActorMoveEnemy = enemy.gameObject.GetComponent<ActorMove>();
            _EnemyActorManager = enemy.gameObject.GetComponent<ActorManager>();
            if (_EnemyActorManager.ThisAction == ActorManager.ActorAction.isMove)
            {
                _EnemyActorManager.ThisAction = ActorManager.ActorAction.isNone;
            }
        }
        //AutoMapping();
        _thisGameState = GameState.AttackFase;
    }

    private async UniTask AllAttackExe()
    {
        float playerPosX = player.transform.position.x, playerPosY = player.transform.position.y;
        // プレイヤーの攻撃を行う
        if (_PlayerActorManager.ThisAction == ActorManager.ActorAction.isAttack)
        {
            _PlayerActorManager.Action();
            _StaminaCostTurn++; // スタミナを消費するターンを加算
        }
        // プレイヤーの攻撃アニメーションの終了を待つ
        await UniTask.WaitUntil(() => _PlayerActorManager.isAttack == false);
        // アニメーションを行った際にポジションが微妙にズレるので強制する
        player.transform.position = new Vector3((float)Math.Round(playerPosX), (float)Math.Round(playerPosY), 0.0f);
        // エネミーの攻撃を行う
        foreach (Transform enemy in _Enemys.transform)
        {
            float EnemyPosX = enemy.transform.position.x, EnemyPosY = enemy.transform.position.y;
            _ActorMoveEnemy = enemy.gameObject.GetComponent<ActorMove>();
            _EnemyActorManager = enemy.gameObject.GetComponent<ActorManager>();
            if (_EnemyActorManager.ThisAction == ActorManager.ActorAction.isAttack)
            {
                _EnemyActorManager.Action();
            }
            // エネミーの攻撃アニメーションの終了を待つ
            await UniTask.WaitUntil(() => _EnemyActorManager.isAttack == false);
            // アニメーションを行った際にポジションが微妙にズレるので強制する
            enemy.transform.position = new Vector3((float)Math.Round(EnemyPosX), (float)Math.Round(EnemyPosY), 0.0f);
        }
        // プレイヤーとエネミーが移動し終わるのを待つ(アタックのアニメーションを作ったら待つ処理を入れる↓)
        // await UniTask.WaitUntil(() => _ActorMovePlayer.isMoveing == false && _ActorMoveEnemy.isMoveing == false);
        // プレイヤーのアクションをNoneにする
        _PlayerActorManager.ThisAction = ActorManager.ActorAction.isNone;
        // 全てのエネミーのアクションをNoneにする
        foreach (Transform enemy in _Enemys.transform)
        {
            _ActorMoveEnemy = enemy.gameObject.GetComponent<ActorMove>();
            _EnemyActorManager = enemy.gameObject.GetComponent<ActorManager>();
            _EnemyActorManager.ThisAction = ActorManager.ActorAction.isNone;
        }
        _thisGameState = GameState.EndFase;
    }

    private async UniTask EndProcess()
    {
        // エネミーの移動を行う
        foreach (Transform enemy in _Enemys.transform)
        {
            _ActorMoveEnemy = enemy.gameObject.GetComponent<ActorMove>();
            _EnemyActorManager = enemy.gameObject.GetComponent<ActorManager>();
            Debug.Log(_EnemyActorManager.ThisAction);
            if (_EnemyActorManager.GetHP <= 0)
            {
                _a2d.Get((int)enemy.gameObject.transform.position.x, (int)enemy.gameObject.transform.position.y).GetSetMapValue = 0;
                GameObject.Find("Log").GetComponent<OutPutLog>().OutputLog(_EnemyActorManager.GetName + "は倒れた");
                Destroy(enemy.gameObject);
            }
        }
        _Enemylist.Remove(null);
        // プレイヤーの攻撃アニメーションの終了を待つ
        await UniTask.WaitUntil(() => _PlayerActorManager.isAttack == false);
        // スタミナ消費を判定し、消費するターンならスタミナを消費
        if (_StaminaCostTurn > 3){
            _PlayerActorManager.StaminaCost();
            _StaminaCostTurn = 0;
        }
        COSTTEXT.text = "スタミナ:"+_PlayerActorManager.GetStamina;
        _HPbar.value = _PlayerActorManager.GetHP;
        AutoMapping();
        _thisGameState = GameState.InputStay;
    }
}
