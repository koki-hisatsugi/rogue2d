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
        if(collision.tag == "Food" || collision.tag == "HPball")
        {
            collision.gameObject.SetActive(false);
            _AM.HpHelth(10);
        }
        else if (collision.tag == "Soda" || collision.tag == "STball")
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

    private void OnTriggerExit2D(Collider2D collision){
        if (collision.tag == "Exit")
        {
            isOnExit = false;
            // gameManager.DungeonSet();
        }
    }
}
