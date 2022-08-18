using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LogManagers;

// 必須なオブジェクトを定義
[RequireComponent(typeof(ActorMove))] // キャラクター移動部品
[RequireComponent(typeof(ActorDir))] // キャラクター向き管理部品
// キャラクターの動きをつかさどるクラス → 各部品をまとめた動きを表現
public class ActorManager : MonoBehaviour
{
    [SerializeField] private string _Name;
    [SerializeField] private int _Level;
    [SerializeField] private int _HP;
    [SerializeField] private int _Atk;
    [SerializeField] private int _Def;
    [SerializeField] private int _Stamina;
    [SerializeField] private int _Exp;
    [SerializeField] private Slider _HPbar;
    [SerializeField] private Slider _STbar;
    [SerializeField] private Text _PlayerLevelText;
    [SerializeField] private SpriteRenderer _ActorSprite;
    [SerializeField] private AudioClip _Attack1;
    [SerializeField] private AudioSource _AudioSource;
    [SerializeField] private int _ActorPosX,_ActorPosY;
    [SerializeField] private AudioClip _AtkMiss;
    [SerializeField] private AudioClip _STHeal;
    [SerializeField] private AudioClip _HPHeal;
    [SerializeField] private AudioClip _LevelUp;
    
    public int GetSet_ActorPosX
    {
        get { return _ActorPosX; }
        set { _ActorPosX = value; }
    }
    public int GetSet_ActorPosY
    {
        get { return _ActorPosY; }
        set { _ActorPosY = value; }
    }
    public int GetSet_Level
    {
        get { return _Level; }
        set { _Level = value; }
    }
    public int GetHP
    {
        get { return _HP; }
    }
    public int GetStamina
    {
        get { return _Stamina; }
    }
    public string GetName
    {
        get { return _Name; }
    }
    public int GetExp
    {
        get { return _Exp; }
    }
    private ActorMove _ThisActorMove;
    private ActorDir _ThisActorDir;
    private ActorAttack _ThisActorAttack;
    [SerializeField] private LayerMask _targetLayer;
    private ActorAnimation _Aanim;

    private OutPutLog opl;

    public bool isAttack;
    private BoxCollider2D bc2d;
    public enum ActorAction
    {
        isNone,
        isMove,
        isAttack,
    }

    public ActorAction ThisAction = ActorAction.isNone;

    public int moveHorizontal = 0;
    public int moveVatical = 0;
    [SerializeField] private PlayerStatusScrObj _PlayerStatus;
    [SerializeField] private EnemyStatusScrObj _EnemyStatus;
    // Start is called before the first frame update
    void Start()
    {
        _ThisActorMove = GetComponent<ActorMove>();
        _ThisActorDir = GetComponent<ActorDir>();
        _ThisActorAttack = GetComponent<ActorAttack>();
        opl = GameObject.Find("Log").GetComponent<OutPutLog>();
        _Aanim = GetComponent<ActorAnimation>();
        isAttack = false;
        bc2d = GetComponent<BoxCollider2D>();
            _EnemyStatus = Resources.Load<EnemyStatusScrObj>("EnemyStatusDataTable");
            _PlayerStatus = Resources.Load<PlayerStatusScrObj>("PlayerStatusDB");

        if(gameObject.tag == "Player"){
            // ActorManagerがアタッチされているオブジェクトがPlayerなら、
            // HPバーのスライダーを取得する
            _Level = 1;
            // _PlayerStatus = Resources.Load<PlayerStatusScrObj>("PlayerStatusDB");
            _Name = _PlayerStatus.name;
            _HPbar = GameObject.Find("HPbar").GetComponent<Slider>();
            _HPbar.maxValue = _PlayerStatus.list[0].maxHp;
            _HP = _PlayerStatus.list[0].maxHp;
            _HPbar.value = _HP;
            // スタミナのスライダーを取得する
            _STbar = GameObject.Find("STbar").GetComponent<Slider>();
            _STbar.maxValue = _PlayerStatus.list[0].maxStamina;
            _Stamina = _PlayerStatus.list[0].maxStamina;
            _STbar.value = _Stamina;
            _Atk = _PlayerStatus.list[0].atk;
            _Def = _PlayerStatus.list[0].def;
            _PlayerLevelText = GameObject.Find("PlayerLevelText").GetComponent<Text>();
            _PlayerLevelText.text = "LV:"+_Level;
        }else{
            // _EnemyStatus = Resources.Load<EnemyStatusScrObj>("EnemyStatusDataTable");
            _HPbar = transform.Find("HPberCanvas").gameObject.transform.Find("HPbar").gameObject.GetComponent<Slider>();
            if(_Level-1 < _EnemyStatus.list.Count){
                _HPbar.maxValue = _EnemyStatus.list[_Level-1].maxHp;
                _HP = _EnemyStatus.list[_Level-1].maxHp;
                _Atk = _EnemyStatus.list[_Level-1].atk;
                _Def = _EnemyStatus.list[_Level-1].def;
            }else{
                _HPbar.maxValue = _EnemyStatus.list[_EnemyStatus.list.Count-1].maxHp;
                _HP = _EnemyStatus.list[_EnemyStatus.list.Count-1].maxHp;
                _Atk = _EnemyStatus.list[_EnemyStatus.list.Count-1].atk;
                _Def = _EnemyStatus.list[_EnemyStatus.list.Count-1].def;
            }
            _HPbar.value = _HP;
            _STbar = null;
        }

        _ActorSprite = GetComponent<SpriteRenderer>();
        _AudioSource = GetComponent<AudioSource>();
    }

    public void setEnemyStatus(){
        if(_EnemyStatus == null) _EnemyStatus = Resources.Load<EnemyStatusScrObj>("EnemyStatusDataTable");
        _Name = _EnemyStatus.name;
        if(_HPbar == null) _HPbar = transform.Find("HPberCanvas").gameObject.transform.Find("HPbar").gameObject.GetComponent<Slider>();
        Debug.Log(_Level);
        if(_Level-1 >= 0 && _Level-1 < _EnemyStatus.list.Count){
            _HPbar.maxValue = _EnemyStatus.list[_Level-1].maxHp;
            _HP = _EnemyStatus.list[_Level-1].maxHp;
            _Atk = _EnemyStatus.list[_Level-1].atk;
            _Def = _EnemyStatus.list[_Level-1].def;
            _Exp = _EnemyStatus.list[_Level-1].giveExp;
        }else{
            _HPbar.maxValue = _EnemyStatus.list[_EnemyStatus.list.Count-1].maxHp;
            _HP = _EnemyStatus.list[_EnemyStatus.list.Count-1].maxHp;
            _Atk = _EnemyStatus.list[_EnemyStatus.list.Count-1].atk;
            _Def = _EnemyStatus.list[_EnemyStatus.list.Count-1].def;
            _Exp = _EnemyStatus.list[_EnemyStatus.list.Count-1].giveExp;
        }
        _HPbar.value = _HP;
    }

    public void setOrderInLayer(){
        _ActorSprite.sortingOrder = 100 - _ActorPosY;
    }

    public void Action()
    {
        switch (ThisAction)
        {
            case ActorAction.isNone:
                break;
            case ActorAction.isMove:
                ActorMoving(moveHorizontal, moveVatical);
                break;
            case ActorAction.isAttack:
                Attack();
                break;
        }
    }

    public void setDir(ActorDirection dir)
    {
        _ThisActorDir.GetSetThisDir = dir;
        if (ActorDirection.RIGHT == dir)
        {
            if (_Aanim != null)
                _Aanim.setLastMoveAnimParam(Vector2.right);
        }
        else if (ActorDirection.LEFT == dir)
        {
            if (_Aanim != null)
                _Aanim.setLastMoveAnimParam(Vector2.left);
        }
        else if (ActorDirection.UP == dir)
        {
            if (_Aanim != null)
                _Aanim.setLastMoveAnimParam(Vector2.up);
        }
        else if (ActorDirection.DOWN == dir)
        {
            if (_Aanim != null)
                _Aanim.setLastMoveAnimParam(Vector2.down);
        }
    }

    public void setDir()
    {
        if (moveHorizontal > 0)
        {
            _ThisActorDir.GetSetThisDir = ActorDirection.RIGHT;
            if (_Aanim != null)
                _Aanim.setLastMoveAnimParam(Vector2.right);
        }
        else if (moveHorizontal < 0)
        {
            _ThisActorDir.GetSetThisDir = ActorDirection.LEFT;
            if (_Aanim != null)
                _Aanim.setLastMoveAnimParam(Vector2.left);
        }
        else if (moveVatical > 0)
        {
            _ThisActorDir.GetSetThisDir = ActorDirection.UP;
            if (_Aanim != null)
                _Aanim.setLastMoveAnimParam(Vector2.up);
        }
        else if (moveVatical < 0)
        {
            _ThisActorDir.GetSetThisDir = ActorDirection.DOWN;
            if (_Aanim != null)
                _Aanim.setLastMoveAnimParam(Vector2.down);
        }
    }

    // キャラクターの移動処理
    public bool ActorMoving(int horizontal, int vertical)
    {
        if (_ThisActorMove != null && _ThisActorDir != null)
        {
            _ThisActorDir.JudgeDir(horizontal, vertical);
            bool _result = _ThisActorMove.Moving(horizontal, vertical);
            setOrderInLayer();
            return _result;
        }
        else
        {
            return false;
        }
    }

    public void Attack()
    {
        Debug.Log("Attackを実行");
        isAttack = true;
        _ThisActorAttack.Attack(_ThisActorDir.GetSetThisDir, _targetLayer, _Atk);
        if (_Aanim != null)
        {
            _Aanim.setAttackAnimParam(true);
            bc2d.enabled = false;
        }
    }

    public void AttackEnd()
    {
        if (_Aanim != null)
        {
            _Aanim.setAttackAnimParam(false);
            bc2d.enabled = true;
        }
        isAttack = false;
    }

    public void Damage(int damage)
    {
        Debug.Log("Damageを実行");
        int calcDamage = damage/_Def;
        // 10分の1で攻撃が外れる
        if(Random.Range(0,10) == 1){
            opl.OutputLog(_Name + "に攻撃が当たらなかった");
            _AudioSource.PlayOneShot(_AtkMiss);
            return;
        }
        _HP = _HP - calcDamage;
        if (_HP < 1 || _HP == 0)
        {
            _HP = 0;
            ThisAction = ActorAction.isNone;
            // Destroy(gameObject);
        }
        _AudioSource.PlayOneShot(_Attack1);
        Debug.Log(calcDamage + "ダメージを受けた。");
        opl.OutputLog(_Name + "は" + calcDamage + "ダメージを受けた。");
        _HPbar.value = _HP;
    }

    public void StaminaCost(){
        _Stamina--;
        if(_STbar != null){
            _STbar.value = _Stamina;
        }
    }

    public void HpHelth(){
        if(gameObject.tag == "Player"){
            if(_HP < _PlayerStatus.list[_Level-1].maxHp){
                _HP++;
            }
        }else{
            if(_HP < _EnemyStatus.list[_Level-1].maxHp){
                _HP++;
            }
        }
        _HPbar.value = _HP;
    }

    public void HpHelth(int helth){
        if(gameObject.tag == "Player"){
            if(_HP < _PlayerStatus.list[_Level-1].maxHp){
                _HP = _HP + helth;
            }
        }else{
            if(_HP < _EnemyStatus.list[_Level-1].maxHp){
                _HP = _HP + helth;
            }
        }
        _AudioSource.PlayOneShot(_HPHeal);
        _HPbar.value = _HP;
    }

    public void HpLost(){
        if(_HP > 0){
            _HP--;
        }
        _HPbar.value = _HP;
    }

    public void StaminaCharge(int charge){
        _Stamina += charge;
        if(_Stamina > _PlayerStatus.list[_Level-1].maxStamina){
            _Stamina = _PlayerStatus.list[_Level-1].maxStamina;
        }
        if(_STbar != null){
            _STbar.value = _Stamina;
        } 
        _AudioSource.PlayOneShot(_STHeal);
    }

    public void AddExp(int Exp){
        _Exp = _Exp + Exp;
        if(_Exp >= _PlayerStatus.list[_Level-1].nextExp){
            _Exp = _Exp - _PlayerStatus.list[_Level-1].nextExp;
            _Level++;
            _Atk = _PlayerStatus.list[_Level-1].atk;
            _Def = _PlayerStatus.list[_Level-1].def;
            _HPbar.maxValue = _PlayerStatus.list[_Level-1].maxHp;
            _STbar.maxValue = _PlayerStatus.list[_Level-1].maxStamina;
            opl.OutputLog(_Name + "は" + _Level + "レベルになった");
            _PlayerLevelText.text = "LV:"+_Level;
            _AudioSource.PlayOneShot(_LevelUp);
        }
    }
}
