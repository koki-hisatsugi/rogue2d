using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 向きのenum変数
public enum ActorDirection{
    UP,
    DOWN,
    RIGHT,
    LEFT
}
// キャラクターの向きを管理するクラス(部品)
public class ActorDir : MonoBehaviour
{
    [SerializeField]private ActorDirection _thisDir;
    public ActorDirection GetSetThisDir{
        get { return _thisDir; }
        set { _thisDir = value; }
    }

    // 現状の向きを判断する処理
    public void JudgeDir(int horizontal, int vertical){
        if(horizontal != 0)
        {
            vertical = 0;
            if(horizontal == 1)
            {
                _thisDir = ActorDirection.RIGHT;
            }
            else if(horizontal == -1)
            {
                _thisDir = ActorDirection.LEFT;
            }
        }
        else if(vertical != 0)
        {
            horizontal = 0;
            if(vertical == 1)
            {
                _thisDir = ActorDirection.UP;
            }
            else if(vertical == -1)
            {
                _thisDir = ActorDirection.DOWN;
            }
        }
    }
}
