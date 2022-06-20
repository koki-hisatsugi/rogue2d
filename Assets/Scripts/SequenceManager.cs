using UnityEngine;

public class SequenceManager
{
    public enum State
    {
        playerTurn,
        EnemyTurn,
    }

    State state{ get; set; }

    public void test(){
        state = State.playerTurn;
        Debug.Log(state);
    }

    
    public void test2(){
        state = State.EnemyTurn;
        Debug.Log(state);
    }

    public State getstate(){
        return state;
    }

}
