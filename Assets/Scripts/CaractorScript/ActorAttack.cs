using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAttack : MonoBehaviour
{
    public void Attack(ActorDirection dir, LayerMask targetLayer, int atk){

        Vector2 start = transform.position;
        Vector2 end = new Vector2();
        switch(dir){
            case ActorDirection.UP:
                end = new Vector2(transform.position.x, transform.position.y + 1.5f);
                break;
            case ActorDirection.DOWN:
                end = new Vector2(transform.position.x, transform.position.y - 1.5f);
                break;
            case ActorDirection.RIGHT:
                end = new Vector2(transform.position.x + 1.5f, transform.position.y);
                break;
            case ActorDirection.LEFT:
                end = new Vector2(transform.position.x - 1.5f, transform.position.y);
                break;
        }
        RaycastHit2D hit;
        BoxCollider2D _BoxCollider2D = GetComponent<BoxCollider2D>();
        _BoxCollider2D.enabled = false;
        hit = Physics2D.Linecast(start, end, targetLayer);
        _BoxCollider2D.enabled = true;
        Debug.Log("ここまで来てる");
        if(hit.transform == null) return;
        Debug.Log("hit.transformがnullじゃない");
        if(hit.collider.gameObject != null){
            Debug.Log(hit.collider.gameObject.name);
            if(hit.collider.gameObject.tag == "Enemy" || hit.collider.gameObject.tag == "Player"){
                hit.collider.gameObject.GetComponent<ActorManager>().Damage(atk);
            }
        }
    }
}
