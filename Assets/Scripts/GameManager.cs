using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LogManagers;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    BoardManager _boardManager;
    // スタミナを消費するまでのターン数
    private int _StaminaCostTurn;
    // 体力を回復するまでのターン数
    private int _HpHelthTurn;
    [Header("エネミーが発生するターン数")] public int SpawnTurn;
    [SerializeField] private int _SpawnTurnCount;
    [SerializeField] private GameObject _Player;
    [SerializeField] private ActorMove _PlayerActorMove;
    [SerializeField] private ActorManager _PlayerActorManager;
    [SerializeField] private GameObject _Enemy;
    [SerializeField] private ActorMove _EnemyActorMove;
    [SerializeField] private ActorManager _EnemyActorManager;
    [SerializeField] private Array2D _a2d;
    [SerializeField] private Array2D _ma2d;
    [SerializeField] private GameObject _ConfirmPanel_NextFloor;
    [SerializeField] private GameObject _OKButton_NextFloor;
    [SerializeField] private GameObject _NOButton_NextFloor;
    [SerializeField] private Button _OK_NextFloor;
    [SerializeField] private Button _NO_NextFloor;
    private bool _selectbutton;
    [SerializeField] private GameObject _NextStandByPanel;
    [SerializeField] private Text _NextStandText;
    private List<GameObject> _Enemylist;
    [SerializeField] private GameObject _Enemys;
    [SerializeField] private List<GameObject> _MapObj;
    private Text floorLevel;
    private int intFloorLevel;
    public GameObject EnemyExplodeParticle;
    private GameObject _EnemyExplodeParticle;
    // 選択音
    public AudioClip Select1;
    // 決定音
    public AudioClip Decision;
    // 階層の移動音
    public AudioClip ExitSound;
    // リザルト画面のBGM
    public AudioClip ResultBGM;
    // オーディオソース
    [SerializeField] private AudioSource _AudioSource;
    // ランキングパネル
    [SerializeField] private GameObject _RankingPanel;
    [SerializeField] private GameObject _ScoreView;
    [SerializeField] private PlayFabManager _PlayfabManager;

    public enum GameState
    {
        InputStay,
        EnemyOperateFase,
        PlayerAct,
        EnemyAct,
        MoveFase,
        EndFase,
        GameEnd,
        GameStart,
        SelectFase,
        NextStandByFase,
    }

    [SerializeField] private GameState _thisGameState;

    public void setGamestate(GameState gs)
    {
        _thisGameState = gs;
    }

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
        // DontDestroyOnLoad(gameObject);
        this.gameObject.name = "GameManager";
        this.gameObject.transform.position = new Vector2(0,0);
        // ヒエラルキー内の順番を先頭にする
        this.gameObject.transform.SetSiblingIndex(0);

        // マップ作製クラス(BoardManagerを取得)
        _boardManager = GetComponent<BoardManager>();

        // プレイヤーオブジェクトを取得し、付随するスクリプトを取得
        _Player = GameObject.Find("Player");
        _PlayerActorMove = _Player.GetComponent<ActorMove>();
        _PlayerActorManager = _Player.GetComponent<ActorManager>();
        _boardManager.player = _Player;

        // 次フロアへの移動確認のパネルを取得する
        _ConfirmPanel_NextFloor = GameObject.Find("ConfirmPanel_NextFloor");

        // ボタンの設定
        _OKButton_NextFloor = _ConfirmPanel_NextFloor.transform.Find("OK_NextFloor").gameObject;
        _OK_NextFloor = _OKButton_NextFloor.GetComponent<Button>();
        _OK_NextFloor.onClick.AddListener(onClick_OK_Next);
        _NOButton_NextFloor = _ConfirmPanel_NextFloor.transform.Find("NO_NextFloor").gameObject;
        _NO_NextFloor = _NOButton_NextFloor.GetComponent<Button>();
        _NO_NextFloor.onClick.AddListener(onClick_NO_Next);
        EventSystem.current.SetSelectedGameObject(_OKButton_NextFloor);
        _OK_NextFloor.Select();
        _ConfirmPanel_NextFloor.SetActive(false);

        intFloorLevel = 0;
        floorLevel = GameObject.Find("FloorLevelText").GetComponent<Text>();

        _NextStandByPanel = GameObject.Find("NextStandByPanel");
        _NextStandText = _NextStandByPanel.transform.Find("FloorText").GetComponent<Text>();
        _NextStandByPanel.SetActive(false);

        _EnemyExplodeParticle = Instantiate(EnemyExplodeParticle, new Vector3(0, 0, 0), Quaternion.identity);
        _EnemyExplodeParticle.SetActive(false);

        _AudioSource = GetComponent<AudioSource>();
        _selectbutton = true;

        _RankingPanel = GameObject.Find("RankingPanel");
        _PlayfabManager = _RankingPanel.GetComponent<PlayFabManager>();
        _ScoreView = _RankingPanel.transform.Find("ScoreView").gameObject;
        _RankingPanel.SetActive(false);
        
        // ゲームスタートフェーズを設定する
        _thisGameState = GameState.GameStart;
    }

    // フロア遷移確認OKボタンの処理
    public void onClick_OK_Next()
    {
        _AudioSource.PlayOneShot(Decision);
        _thisGameState = GameState.GameStart;
    }

    // フロア遷移確認NOボタンの処理
    public void onClick_NO_Next()
    {
        _AudioSource.PlayOneShot(Decision);
        _ConfirmPanel_NextFloor.SetActive(false);
        _thisGameState = GameState.InputStay;
    }

    // async(えいしんく)をつける
    private void Start()
    {
        _ = UpdateLoop();
        // InitGame();
    }

    // フェーズの管理をして各行動を制限する
    async UniTask UpdateLoop()
    {
        while (true)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);  // Unityのupdate関数と同じフレームで
            if (_thisGameState != GameState.GameStart && 
                _thisGameState != GameState.SelectFase &&
                Input.GetKey(KeyCode.LeftShift) &&
                (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
            {
                Time.timeScale = 5;
            }
            else
            {
                Time.timeScale = 1;
            }
            switch (_thisGameState)
            {
                case GameState.InputStay:         // 入力待ちフェイズ
                    // await AllActionDecision();
                    await PlayerAction();
                    break;
                case GameState.EnemyOperateFase:
                    await EnemyAction();
                    break;
                case GameState.PlayerAct:
                    await PlayerActorFase();
                    break;
                case GameState.EnemyAct:
                    await EnemyActorFase();
                    break;
                case GameState.MoveFase:
                    await MoveFase();
                    break;
                case GameState.EndFase:           // 終了処理のフェイズ
                    await EndProcess();
                    break;
                case GameState.GameStart:           // 終了処理のフェイズ
                    await InitGame();
                    break;
                case GameState.SelectFase:           // セレクト画面
                    await SelectScrn();
                    break;
                case GameState.NextStandByFase:           // フロア遷移アニメーション
                    await NextStandByScrn();
                    break;
                case GameState.GameEnd:           // フロア遷移アニメーション
                    await GameOver();
                    break;
            }
        }
    }

    public async UniTask InitGame()
    {
        // エネミー発生ターンの初期化
        _SpawnTurnCount = 0;
        // 次フロアへの遷移決定パネルを非表示
        _ConfirmPanel_NextFloor.SetActive(false);
        intFloorLevel++;
        floorLevel.text = "フロア:" + intFloorLevel;
        _NextStandText.text = "第" + intFloorLevel + "階層";
        // 次フロアの読み込み画面を表示
        _NextStandByPanel.SetActive(true);
        DungeonSet();
        // 読み込み待機、1秒待つ
        await UniTask.Delay(TimeSpan.FromSeconds(1));
        _AudioSource.PlayOneShot(ExitSound);
        _NextStandByPanel.SetActive(false);
        _thisGameState = GameState.InputStay;
    }

    public void DungeonSet()
    {
        // 仮、プレイヤーの向きを右向きにする
        _PlayerActorManager.moveVatical = -1;
        _PlayerActorManager.setDir(ActorDirection.DOWN);
        _boardManager.delwall();
        _boardManager.dellMap();
        _a2d = _boardManager.SetupScene(intFloorLevel);
        _Enemylist = _boardManager.Enemylist;
        _Enemys = _boardManager.Enemys;
        _StaminaCostTurn = 0;
        _HpHelthTurn = 0;
        _MapObj = _boardManager.mapping(_a2d);
    }

    // オートマッピングのメソッド
    public void AutoMapping()
    {
        Dictionary<string, GameObject> Maps = _boardManager.MapDic;
        foreach (string key in Maps.Keys)
        {
            // マップマスのベース色を設定
            Maps[key].GetComponent<Image>().color = new Color32(255, 255, 255, 90);
        }

        // マップの中のプレイヤーとエネミーを探す
        for (int h = 0; h < _a2d.height; h++)
        {
            for (int w = 0; w < _a2d.width; w++)
            {
                if (_a2d.Get(w, h).GetSetMapValue == BoardRemote.StairsNum)
                {
                    Maps[w + ":" + h].GetComponent<Image>().color = new Color32(14, 209, 69, 90);
                }
                else if (_a2d.Get(w, h).GetSetMapValue == BoardRemote.foodNum)
                {
                    Maps[w + ":" + h].GetComponent<Image>().color = new Color32(255, 0, 207, 90);
                }
                // マップのプレイヤーかエネミーだった場合、色を変える
                switch (_a2d.Get(w, h).GetSetMapOnActor)
                {
                    case BoardRemote.PlayerNum:
                        Maps[w + ":" + h].GetComponent<Image>().color = new Color32(255, 0, 0, 90);
                        break;
                    case BoardRemote.EnemyNum:
                        Maps[w + ":" + h].GetComponent<Image>().color = new Color32(0, 0, 255, 90);
                        break;
                }
            }
        }
        for (int h = 0; h < _a2d.height; h++)
        {
            for (int w = 0; w < _a2d.width; w++)
            {
                // マップのプレイヤーがまだ開いていない部屋に踏み入れた場合、その部屋のマップを開放する
                if (_a2d.Get(w, h).GetSetMapOnActor == BoardRemote.PlayerNum)
                {
                    // プレイヤーがいる位置のマスの部屋名を取得し、同じ部屋名のマスを開放
                    if (_a2d.Get(w, h).GetSetTileAttribute == MapData2D.TileAttribute.floor)
                    {
                        // 部屋名のオブジェクトを取得
                        GameObject room = GameObject.Find(_a2d.Get(w, h).GetSetRoomName);
                        // 子を全てアクティブにする
                        foreach (Transform go in room.transform)
                        {
                            go.gameObject.SetActive(true);
                        }
                        // 部屋のマスクをアクティブにする
                        GameObject.Find("Mask_" + _a2d.Get(w, h).GetSetRoomName).GetComponent<SpriteMask>().enabled = true;
                    }
                    else
                    {
                        Maps[w + ":" + h].gameObject.SetActive(true);
                        foreach (GameObject mask in _boardManager._RoomMasks)
                        {
                            mask.GetComponent<SpriteMask>().enabled = false;
                        }

                    }
                }
            }
        }
    }

    private async UniTask SelectScrn()
    {
        if(_selectbutton){
            _OK_NextFloor.Select();
        }else{
            _NO_NextFloor.Select();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            _AudioSource.PlayOneShot(Select1);
            _selectbutton = !_selectbutton;
        }
    }

    private async UniTask NextStandByScrn()
    {

    }
    
    private async UniTask GameOver()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            SceneLoader.SceneLoading("TitleScene");
        }
    }

    private async UniTask PlayerAction()
    {
        // ターンの開始にオートマッピングを行う
        AutoMapping();
        bool inputTrue = false;
        await UniTask.WaitUntil(() => Input.anyKey);
        // プレイヤーの行動を選定
        if (Input.GetMouseButtonDown(1))
        {
            AutoMapping();
            Debug.Log("オートマッピング");
            return;
        }
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            // 矢印キーが押された場合、以下の処理を行う
            // horizontalとverticalの値を矢印の入力に合わせる。
            int horizontal = (int)Input.GetAxisRaw("Horizontal");
            int vertical = (int)Input.GetAxisRaw("Vertical");

            // 左右のキーが押されていた場合
            if (horizontal != 0)
            {
                // 左右優先で上下の入力値を0にする　変な方向に動かないように
                vertical = 0;
            }

            // プレイヤーの移動値と向きを設定する
            _PlayerActorManager.moveHorizontal = horizontal;
            _PlayerActorManager.moveVatical = vertical;
            _PlayerActorManager.setDir();

            // プレイヤーの座標を格納する
            int _PlayerLocalPosX = _PlayerActorManager.GetSet_ActorPosX
            , _PlayerLocalPosY = _PlayerActorManager.GetSet_ActorPosY;

            // 進もうとした場所に壁かエネミーがいないか判定
            if (_a2d.Get(_PlayerLocalPosX + horizontal, _PlayerLocalPosY + vertical).GetSetMapValue != BoardRemote.WallNum &&
            _a2d.Get(_PlayerLocalPosX + horizontal, _PlayerLocalPosY + vertical).GetSetMapOnActor != BoardRemote.EnemyNum)
            {
                // 入力が有効だった場合のフラグを立てる
                inputTrue = true;
                // 障害物がない場合には、内部マップのプレイヤー位置を変える
                _a2d.Get(_PlayerLocalPosX, _PlayerLocalPosY).GetSetMapOnActor = 0;
                _a2d.Get(_PlayerLocalPosX + horizontal, _PlayerLocalPosY + vertical).GetSetMapOnActor = BoardRemote.PlayerNum;
                if (_a2d.Get(_PlayerLocalPosX + horizontal, _PlayerLocalPosY + vertical).GetSetMapValue == BoardRemote.foodNum)
                {
                    _a2d.Get(_PlayerLocalPosX + horizontal, _PlayerLocalPosY + vertical).GetSetMapValue = BoardRemote.FloorNum;
                };
                _PlayerActorManager.GetSet_ActorPosX = _PlayerLocalPosX + horizontal;
                _PlayerActorManager.GetSet_ActorPosY = _PlayerLocalPosY + vertical;
                _PlayerActorManager.ThisAction = ActorManager.ActorAction.isMove;
            }
        }
        // 攻撃ボタンを押された場合
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            // 入力が有効だった場合のフラグを立てる
            inputTrue = true;
            _PlayerActorManager.ThisAction = ActorManager.ActorAction.isAttack;
        }

        if (inputTrue)
        {
            _thisGameState = GameState.EnemyOperateFase; // ??何に設定するか
        }
    }

    private async UniTask EnemyAction()
    {
        // エネミーの行動を決定する
        foreach (Transform enemy in _Enemys.transform)
        {
            // エネミーのスクリプトを格納しておく
            _EnemyActorMove = enemy.gameObject.GetComponent<ActorMove>();
            _EnemyActorManager = enemy.gameObject.GetComponent<ActorManager>();
            // エネミーのAIをインスタンスする
            EnemyAI EAI = new EnemyAI();
            // エネミーの座標を格納する
            int enemyLocalPosX = _EnemyActorManager.GetSet_ActorPosX
            , enemyLocalPosY = _EnemyActorManager.GetSet_ActorPosY;
            CoordinateXY _CXY = EAI.AstarAlgo(enemyLocalPosX, enemyLocalPosY, _a2d);
            if (_CXY.GetSetX == 0 && _CXY.GetSetY == 0)
            {
                if (enemyLocalPosX > _CXY.GetSetPX)
                {
                    _EnemyActorManager.setDir(ActorDirection.LEFT);
                }
                else if (enemyLocalPosX < _CXY.GetSetPX)
                {
                    _EnemyActorManager.setDir(ActorDirection.RIGHT);
                }
                else if (enemyLocalPosY > _CXY.GetSetPY)
                {
                    _EnemyActorManager.setDir(ActorDirection.DOWN);
                }
                else if (enemyLocalPosY < _CXY.GetSetPY)
                {
                    _EnemyActorManager.setDir(ActorDirection.UP);
                }
                _EnemyActorManager.ThisAction = ActorManager.ActorAction.isAttack;
            }
            else if (_CXY.GetSetX == 2 && _CXY.GetSetY == 2)
            {
                _EnemyActorManager.ThisAction = ActorManager.ActorAction.isNone;
            }
            else
            {
                _EnemyActorManager.ThisAction = ActorManager.ActorAction.isMove;
                _EnemyActorManager.moveHorizontal = _CXY.GetSetX - _EnemyActorManager.GetSet_ActorPosX;
                _EnemyActorManager.moveVatical = _CXY.GetSetY - _EnemyActorManager.GetSet_ActorPosY;
                if (_a2d.Get(_CXY.GetSetX, _CXY.GetSetY).GetSetMapOnActor != BoardRemote.PlayerNum
                && _a2d.Get(_CXY.GetSetX, _CXY.GetSetY).GetSetMapOnActor != BoardRemote.EnemyNum
                && _a2d.Get(_CXY.GetSetX, _CXY.GetSetY).GetSetMapValue != BoardRemote.WallNum)
                {
                    _a2d.Get(enemyLocalPosX, enemyLocalPosY).GetSetMapOnActor = BoardRemote.FloorNum; // 自分がいなくなったマスに0をセットする
                    _a2d.Get(_CXY.GetSetX, _CXY.GetSetY).GetSetMapOnActor = BoardRemote.EnemyNum; // 5をセットする
                    _EnemyActorManager.GetSet_ActorPosX = _CXY.GetSetX;
                    _EnemyActorManager.GetSet_ActorPosY = _CXY.GetSetY;
                }
                else
                {
                    _EnemyActorManager.ThisAction = ActorManager.ActorAction.isNone;
                }
            }
        }

        _thisGameState = GameState.PlayerAct;
    }

    private async UniTask PlayerActorFase()
    {
        // 攻撃が設定されている場合、
        if (_PlayerActorManager.ThisAction == ActorManager.ActorAction.isAttack)
        {
            float playerPosX = _Player.transform.position.x, playerPosY = _Player.transform.position.y;
            // プレイヤーの攻撃を行う
            _PlayerActorManager.Action();
            _StaminaCostTurn++; // スタミナを消費するターンを加算
            // プレイヤーの攻撃アニメーションの終了を待つ
            await UniTask.WaitUntil(() => _PlayerActorManager.isAttack == false);
            // アニメーションを行った際にポジションが微妙にズレるので強制する
            _Player.transform.position = new Vector3((float)Math.Round(playerPosX), (float)Math.Round(playerPosY), 0.0f);
            _PlayerActorManager.ThisAction = ActorManager.ActorAction.isNone;

            // エネミーの死亡判定
            foreach (Transform enemy in _Enemys.transform)
            {
                _EnemyActorMove = enemy.gameObject.GetComponent<ActorMove>();
                _EnemyActorManager = enemy.gameObject.GetComponent<ActorManager>();
                Debug.Log(_EnemyActorManager.ThisAction);
                if (_EnemyActorManager.GetHP <= 0)
                {
                    _a2d.Get(_EnemyActorManager.GetSet_ActorPosX, _EnemyActorManager.GetSet_ActorPosY).GetSetMapValue = BoardRemote.FloorNum;
                    _a2d.Get(_EnemyActorManager.GetSet_ActorPosX, _EnemyActorManager.GetSet_ActorPosY).GetSetMapOnActor = 0;
                    GameObject.Find("Log").GetComponent<OutPutLog>().OutputLog(_EnemyActorManager.GetName + "は倒れた");
                    _EnemyExplodeParticle.transform.position = new Vector3(_EnemyActorManager.GetSet_ActorPosX, _EnemyActorManager.GetSet_ActorPosY, 0);
                    _EnemyExplodeParticle.SetActive(false);
                    _EnemyExplodeParticle.SetActive(true);
                    _PlayerActorManager.AddExp(_EnemyActorManager.GetExp);
                    Destroy(enemy.gameObject);
                }
            }
        }
        // 移動行動フェイズに移行する
        _thisGameState = GameState.MoveFase;
    }

    private async UniTask MoveFase()
    {
        // プレイヤーの移動を行う
        if (_PlayerActorManager.ThisAction == ActorManager.ActorAction.isMove)
        {
            _PlayerActorManager.Action();
            _StaminaCostTurn++; // スタミナを消費するターンを加算
            _HpHelthTurn++;
            // プレイヤーの行動をNoneにする
            _PlayerActorManager.ThisAction = ActorManager.ActorAction.isNone;
        }
        // エネミーの移動を行う
        foreach (Transform enemy in _Enemys.transform)
        {
            _EnemyActorMove = enemy.gameObject.GetComponent<ActorMove>();
            _EnemyActorManager = enemy.gameObject.GetComponent<ActorManager>();
            if (_EnemyActorManager.ThisAction == ActorManager.ActorAction.isMove)
            {
                _EnemyActorManager.Action();
                // エネミーの行動をNoneにする
                _EnemyActorManager.ThisAction = ActorManager.ActorAction.isNone;
            }
        }
        // プレイヤーが移動し終わるのを待つ
        if(_PlayerActorMove != null){
            await UniTask.WaitUntil(() => _PlayerActorMove.isMoveing == false);
        }
        // エネミーが移動し終わるのを待つ
        if(_EnemyActorMove != null){
            await UniTask.WaitUntil(() => _EnemyActorMove.isMoveing == false);
        }
        // エネミーの行動フェイズに移行する
        _thisGameState = GameState.EnemyAct;
    }

    private async UniTask EnemyActorFase()
    {
        // エネミーの攻撃を行う
        foreach (Transform enemy in _Enemys.transform)
        {
            _EnemyActorMove = enemy.gameObject.GetComponent<ActorMove>();
            _EnemyActorManager = enemy.gameObject.GetComponent<ActorManager>();
            if (_EnemyActorManager.ThisAction == ActorManager.ActorAction.isAttack)
            {
                float EnemyPosX = enemy.transform.position.x, EnemyPosY = enemy.transform.position.y;
                _EnemyActorManager.Action();
                // エネミーの攻撃アニメーションの終了を待つ
                await UniTask.WaitUntil(() => _EnemyActorManager.isAttack == false);
                // アニメーションを行った際にポジションが微妙にズレるので強制する
                enemy.transform.position = new Vector3((float)Math.Round(EnemyPosX), (float)Math.Round(EnemyPosY), 0.0f);
            }
        }

        // エンドフェイズに移行する
        _thisGameState = GameState.EndFase;
    }

    private async UniTask EndProcess()
    {
        _EnemyActorMove = null;
        _EnemyActorManager = null;
        // プレイヤーの攻撃アニメーションの終了を待つ
        await UniTask.WaitUntil(() => _PlayerActorManager.isAttack == false);
        
        if(_PlayerActorManager.GetHP < 1){
            // BGMを切り替える
            _AudioSource.clip = ResultBGM;
            _AudioSource.Play();
            // HPがゼロになった場合、
            // ランキング用のスコアをPlayfabManagerにセットする
            _PlayfabManager.score = intFloorLevel;
            _ScoreView.GetComponent<Text>().text = "最終到達地点："+intFloorLevel+"階";
            // パネルを表示する
            _RankingPanel.SetActive(true);
            // PlayFabへのログインを実行する
            _PlayfabManager.Login();
            // ログインを待つ
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            // PlayFabからランキングデータを取得する
            _PlayfabManager.RequestLeaderBoard();
            // ゲーム終了フェーズにする
            _thisGameState = GameState.GameEnd;
            return;
        }
        // スタミナ消費を判定し、消費するターンならスタミナを消費
        if (_StaminaCostTurn > 5 && _PlayerActorManager.GetStamina > 0)
        {
            _PlayerActorManager.StaminaCost();
            _StaminaCostTurn = 0;
            if (_HpHelthTurn > 2)
            {
                _PlayerActorManager.HpHelth();
                _HpHelthTurn = 0;
            }
        }
        else if (_PlayerActorManager.GetStamina < 1)
        {
            _PlayerActorManager.HpLost();
        }
        _thisGameState = GameState.InputStay;
        if (_Player.GetComponent<ItemGetter>() != null)
        {
            if (_Player.GetComponent<ItemGetter>().isOnExit)
            {
                _Player.GetComponent<ItemGetter>().isOnExit = false;
                AutoMapping();
                _ConfirmPanel_NextFloor.SetActive(true);
                _OK_NextFloor.Select();
                _thisGameState = GameState.SelectFase;
            }
        }

        // エネミーの発生ターンをカウントする
        // フィールドにエネミーがいない場合、
        if (isNullEnemyList()){
            _SpawnTurnCount++;
            if(_SpawnTurnCount >= SpawnTurn){
                _SpawnTurnCount = 0;
                // エネミー発生処理
                _Enemylist = _boardManager.EnemySpawn(intFloorLevel);
                foreach(GameObject enemy in _Enemylist){
                    Debug.Log("テスト");
                    enemy.transform.parent = _Enemys.transform;
                    _a2d.Get(enemy.GetComponent<ActorManager>().GetSet_ActorPosX,
                    enemy.GetComponent<ActorManager>().GetSet_ActorPosY).GetSetMapOnActor = BoardRemote.EnemyNum;
                }
            }
        }
    }

    bool isNullEnemyList(){
        bool isNullList = true;
        foreach(GameObject tmp in _Enemylist){
            if(tmp != null){
                isNullList = false;
            }
        }
        return isNullList;
    }
}
