using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAnimation : MonoBehaviour
{

    private Animator _Animator; // キャラクターのAnimatorを取得する

    // Start is called before the first frame update
    void Start()
    {
        _Animator = GetComponent<Animator>();
    }

    public void setDirAnimParam(Vector2 movement)
    {
        if (_Animator != null)
        {
            _Animator.SetFloat("Dir_Y", movement.y);
            _Animator.SetFloat("Dir_X", movement.x);
            _Animator.SetFloat("Input", movement.magnitude);
        }
    }

    public void setLastMoveAnimParam(Vector2 lastMove)
    {
        if (_Animator != null)
        {
            _Animator.SetFloat("LastMove_Y", lastMove.y);
            _Animator.SetFloat("LastMove_X", lastMove.x);
        }
    }

    public void setAttackAnimParam(bool Attack)
    {
        if (_Animator != null)
        {
            _Animator.SetBool("Attack", Attack);
        }
    }

    public void setAttackFalse(){
        if (_Animator != null)
        {
            _Animator.SetBool("Attack", false);
        }
    }
}
