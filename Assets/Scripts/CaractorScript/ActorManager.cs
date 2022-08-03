using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LogManagers;

// 必須なオブジェクトを定義
[RequireComponent(typeof(ActorMove))] // キャラクター移動部品
[RequireComponent(typeof(ActorDir))] // キャラクター向き管理部品
// キャラクターの動きをつかさどるクラス → 各部品をまとめた動きを表現
public class ActorManager : MonoBehaviour
{
    [SerializeField] private string _Name;
    [SerializeField] private int _MaxHP;
    [SerializeField] private int _HP;
    [SerializeField] private int _MaxStamina;
    [SerializeField] private int _Stamina;
    [SerializeField] private int _ActorPosX,_ActorPosY;
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
    [SerializeField] private ActorMove _ThisActorMove;
    [SerializeField] private ActorDir _ThisActorDir;
    [SerializeField] private ActorAttack _ThisActorAttack;
    [SerializeField] private LayerMask _targetLayer;
    [SerializeField] private ActorAnimation _Aanim;

    public OutPutLog opl;

    public bool isAttack;
    [SerializeField] private BoxCollider2D bc2d;
    public enum ActorAction
    {
        isNone,
        isMove,
        isAttack,
    }

    public ActorAction ThisAction = ActorAction.isNone;

    public int moveHorizontal = 0;
    public int moveVatical = 0;
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
            return _result;
        }
        else
        {
            return false;
        }
    }

    public void Attack()
    {
        isAttack = true;
        _ThisActorAttack.Attack(_ThisActorDir.GetSetThisDir, _targetLayer);
        if (_Aanim != null)
        {
            _Aanim.setAttackAnimParam(true);
            // bc2d.enabled = false;
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
        _HP = _HP - damage;
        if (_HP < 1 || _HP == 0)
        {
            _HP = 0;
            ThisAction = ActorAction.isNone;
            // Destroy(gameObject);
        }
        Debug.Log(damage + "ダメージを受けた。");
        opl.OutputLog(gameObject.GetComponent<ActorManager>().GetName + "は" + damage + "ダメージを受けた。");
    }

    public void StaminaCost(){
        _Stamina--;
    }

    public void StaminaCharge(int charge){
        _Stamina += charge;
        if(_Stamina > _MaxStamina){
            _Stamina = _MaxStamina;
        }
    }
}
