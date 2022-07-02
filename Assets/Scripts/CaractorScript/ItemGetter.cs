using UnityEngine;

public class ItemGetter : MonoBehaviour
{
    public GameManager gameManager;
    public bool isOnExit;
    private ActorManager _AM;
    void Start(){
        _AM = GetComponent<ActorManager>();
        // gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        isOnExit = false;
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
            isOnExit = true;
            // gameManager.DungeonSet();
        }
    }
}
