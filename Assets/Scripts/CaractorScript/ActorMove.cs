using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// キャラクターの移動を行うクラス(部品)
public class ActorMove : MonoBehaviour
{
    private Rigidbody2D _ActorRb2d; // addCompornentされたオブジェクトのRigidbody2D
    private ActorAnimation _Aanim;
    private Vector2 _Movement;
    private Vector2 _LastMove;
    public float moveTime = 0.5f;   // キャラクター移動の速度
    public bool isMoveing = false;  // 移動判定用変数
    public LayerMask blockingLayer; // 移動可能なマスかを判定する基準のレイヤー
    [SerializeField] private BoxCollider2D _BoxCollider2D; // addCompornentされたオブジェクトのBoxCollider2D
    private CameraFllow _CameraFllow; // カメラの追従をする際に必要になる
    private void Awake()
    {
        _ActorRb2d = GetComponent<Rigidbody2D>();
        _BoxCollider2D = GetComponent<BoxCollider2D>();
        _CameraFllow = GetComponent<CameraFllow>();
        _Aanim = GetComponent<ActorAnimation>();
    }

    public bool Moving(int horizontal, int vertical)
    {
        if (horizontal != 0)
        {
            vertical = 0;
        }
        else if (vertical != 0)
        {
            horizontal = 0;
        }

        _Movement = new Vector2(horizontal, vertical);
        if (Mathf.Abs(_Movement.x) > 0.5f)
        {
            _LastMove.x = _Movement.x;
            _LastMove.y = 0;
        }
        if (Mathf.Abs(_Movement.y) > 0.5f)
        {
            _LastMove.y = _Movement.y;
            _LastMove.x = 0;
        }
        
        if(_Aanim != null){
            // _Aanim.setDirAnimParam(_Movement);
            _Aanim.setLastMoveAnimParam(_LastMove);
        }

        if (horizontal != 0 || vertical != 0)
        {
            return Move(horizontal, vertical);
        }

        return false;
    }
    private bool Move(int horizontal, int vertical)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(horizontal, vertical);
        RaycastHit2D hit;

        _BoxCollider2D.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        _BoxCollider2D.enabled = true;

        // hitの条件を除いてみる
        if (!isMoveing)
        {
            StartCoroutine(Movement(end));
            return true;
        }
        return false;
    }
    IEnumerator Movement(Vector3 end)
    {
        isMoveing = true;
        float remainingDistance = (transform.position - end).sqrMagnitude;
        while (remainingDistance > float.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, end, 1f / moveTime * Time.deltaTime);
            if (_CameraFllow != null)
            {
                _CameraFllow.cameraMoveTowards(new Vector3(transform.position.x, transform.position.y, -10), 1f / moveTime * Time.deltaTime);
            }
            remainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        transform.position = end;
        isMoveing = false;
        if(_Aanim != null){
            _Aanim.setDirAnimParam(Vector2.zero);
        }
    }
}
