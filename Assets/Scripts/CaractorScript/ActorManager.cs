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
    [SerializeField] protected string _Name;
    [SerializeField] protected int _Level;
    [SerializeField] protected int _HP;
    [SerializeField] protected int _Atk;
    [SerializeField] protected int _Def;
    // [SerializeField] protected int _Stamina;  // プレイヤーのみ
    [SerializeField] protected int _Exp;
    [SerializeField] protected Slider _HPbar;
    // [SerializeField] protected Slider _STbar;  // プレイヤーのみ
    // [SerializeField] protected Text _PlayerLevelText;  // プレイヤーのみ
    [SerializeField] protected SpriteRenderer _ActorSprite;
    [SerializeField] protected AudioClip _Attack1;
    [SerializeField] protected AudioSource _AudioSource;
    [SerializeField] protected int _ActorPosX,_ActorPosY;
    [SerializeField] protected AudioClip _AtkMiss;
    // [SerializeField] protected AudioClip _STHeal;  // プレイヤーのみ
    [SerializeField] protected AudioClip _HPHeal;  // プレイヤーのみ
    // [SerializeField] protected AudioClip _LevelUp;  // プレイヤーのみ
    
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
    public string GetName
    {
        get { return _Name; }
    }
    public int GetExp
    {
        get { return _Exp; }
    }
    protected ActorMove _ThisActorMove;
    [SerializeField] protected ActorDir _ThisActorDir;
    protected ActorAttack _ThisActorAttack;
    [SerializeField] protected LayerMask _targetLayer;
    protected ActorAnimation _Aanim;

    protected OutPutLog opl;

    public bool isAttack;
    protected BoxCollider2D bc2d;
    public enum ActorAction
    {
        isNone,
        isMove,
        isAttack,
    }

    public ActorAction ThisAction = ActorAction.isNone;

    public int moveHorizontal = 0;
    public int moveVatical = 0;

    protected void Start()
    {
        _ThisActorMove = GetComponent<ActorMove>();
        _ThisActorDir = GetComponent<ActorDir>();
        _ThisActorAttack = GetComponent<ActorAttack>();
        opl = GameObject.Find("Log").GetComponent<OutPutLog>();
        _Aanim = GetComponent<ActorAnimation>();
        isAttack = false;
        bc2d = GetComponent<BoxCollider2D>();

        _ActorSprite = GetComponent<SpriteRenderer>();
        _AudioSource = GetComponent<AudioSource>();
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
        int calcDamage = damage - _Def;
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
}
