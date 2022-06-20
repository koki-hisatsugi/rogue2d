using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGetter : MonoBehaviour
{
    // private Rigidbody2D _ItemGetterRb2d;
    // void Awake(){
    //     _ItemGetterRb2d = GetComponent<Rigidbody2D>();
    // }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Food")
        {
            collision.gameObject.SetActive(false);
        }
        else if (collision.tag == "Soda")
        {
            collision.gameObject.SetActive(false);
        }
        else if (collision.tag == "Exit")
        {
            // gameManager.DungeonSet();
        }
    }
}
