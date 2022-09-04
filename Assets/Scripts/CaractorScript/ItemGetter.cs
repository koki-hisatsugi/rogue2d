using UnityEngine;

public class ItemGetter : MonoBehaviour
{
    public GameManager gameManager;
    public bool isOnExit;
    private PlayerManager _AM;

    public ItemList itemList;
    void Start(){
        _AM = GetComponent<PlayerManager>();
        // gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        isOnExit = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Food" || collision.tag == "HPball")
        {
            if(gameManager != null){
                gameManager.TikeItemSound();
            }
            if(itemList != null){
                if(itemList.GetItem(collision.gameObject.name)){
                    collision.gameObject.SetActive(false);
                }
            }
            // _AM.HpHelth(10);
        }
        else if (collision.tag == "Soda" || collision.tag == "STball")
        {
            if(gameManager != null){
                gameManager.TikeItemSound();
            }
            if(itemList != null){
                if(itemList.GetItem(collision.gameObject.name)){
                    collision.gameObject.SetActive(false);
                }
            }
            // _AM.StaminaCharge(20);
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
