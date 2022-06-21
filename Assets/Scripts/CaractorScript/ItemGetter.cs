using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGetter : MonoBehaviour
{
    // private Rigidbody2D _ItemGetterRb2d;
    // void Awake(){
    //     _ItemGetterRb2d = GetComponent<Rigidbody2D>();
    // }

    private ActorManager _AM;
    void Awake(){
        _AM = GetComponent<ActorManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Food")
        {
            collision.gameObject.SetActive(false);
            _AM.StaminaCharge(10);
        }
        else if (collision.tag == "Soda")
        {
            collision.gameObject.SetActive(false);
            _AM.StaminaCharge(20);
        }
        else if (collision.tag == "Exit")
        {
            // gameManager.DungeonSet();
        }
    }
}
