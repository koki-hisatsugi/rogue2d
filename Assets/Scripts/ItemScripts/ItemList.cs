using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemList : MonoBehaviour
{
    public Text ItemInfoText;
    public GameObject ItemContent;
    public List<GameObject> SelectItems;
    public int selectNum;
    public ItemMasterList ItemMseterDB;
    public List<int> MyItemIDList;

    const string initItemText = "----------";
    // Start is called before the first frame update
    void Awake()
    {
        selectNum = 0;
        MyItemIDList = new List<int>();
        ItemContent = transform.Find("Viewport").transform.Find("Content").gameObject;
        foreach (Transform selectitem in ItemContent.transform) {
            SelectItems.Add(selectitem.gameObject);
        }

        for (int i = 0; i < SelectItems.Count; i++){
            if (i == 0){
                SelectItems[i].GetComponent<Toggle>().isOn = true;
            } else {
                SelectItems[i].GetComponent<Toggle>().isOn = false;
            }

            SelectItems[i].transform.Find("Background").gameObject.GetComponent<Image>().enabled = false;
            SelectItems[i].transform.Find("Label").gameObject.GetComponent<Text>().text = initItemText;
        }
    }

    public bool GetItem(string id) {
        bool result = false;
        ItemData _ItemData = new ItemData();
        if(MyItemIDList.Count < 8){
            MyItemIDList.Add(Int32.Parse(id));
            result = true;
            for(int i = 0; i < MyItemIDList.Count; i++){
                string setText = ItemMseterDB.Items.Find(item => item.ID == MyItemIDList[i]).ItemName;
                SelectItems[i].transform.Find("Label").gameObject.GetComponent<Text>().text = setText;
            }
        }
        return result;
    }

    public enum CursorInput {
        cursorUP,
        cursorDown,
    }
    public void CursorController (CursorInput _input){
        if (_input == CursorInput.cursorDown) {
            if(selectNum < SelectItems.Count-1){
                selectNum ++;
            }else{
                selectNum = 0;
            }
        } else {
            if(selectNum > 0){
                selectNum --;
            }else{
                selectNum = SelectItems.Count-1;
            }
        }
        SelectItems[selectNum].GetComponent<Toggle>().isOn = true;
        if(selectNum < MyItemIDList.Count){
            string sf = string.Format(ItemMseterDB.Items.Find(item => item.ID == MyItemIDList[selectNum]).ItemInfo
            , ItemMseterDB.Items.Find(item => item.ID == MyItemIDList[selectNum]).Value + "");
            ItemInfoText.text = sf;
            Debug.Log("テキスト置換");
        } else {
            ItemInfoText.text = "なし";
        }
    }

    public void setInfoText(){
        if(selectNum < MyItemIDList.Count){
            string sf = string.Format(ItemMseterDB.Items.Find(item => item.ID == MyItemIDList[selectNum]).ItemInfo
            , ItemMseterDB.Items.Find(item => item.ID == MyItemIDList[selectNum]).Value + "");
            ItemInfoText.text = sf;
        } else {
            ItemInfoText.text = "なし";
        }
    }

    public ItemData UseItem() {
        ItemData result = new ItemData();
        if(!initItemText.Equals(SelectItems[selectNum].transform.Find("Label").gameObject.GetComponent<Text>().text)){
            result.ID = ItemMseterDB.Items.Find(item => item.ID == MyItemIDList[selectNum]).ID;
            result.InCureType = ItemMseterDB.Items.Find(item => item.ID == MyItemIDList[selectNum]).InCureType;
            result.InEquipType = ItemMseterDB.Items.Find(item => item.ID == MyItemIDList[selectNum]).InEquipType;
            result.InItemType = ItemMseterDB.Items.Find(item => item.ID == MyItemIDList[selectNum]).InItemType;
            result.Value = ItemMseterDB.Items.Find(item => item.ID == MyItemIDList[selectNum]).Value;
            MyItemIDList.RemoveAt(selectNum);
        }
        else
        {
            return null;
        }
        ItemListReset();
        return result;
    }

    public void ItemListReset(){
        for (int i = 0; i < SelectItems.Count; i++){
            SelectItems[i].transform.Find("Label").gameObject.GetComponent<Text>().text = initItemText;
        }

        for(int i = 0; i < MyItemIDList.Count; i++){
            string setText = ItemMseterDB.Items.Find(item => item.ID == MyItemIDList[i]).ItemName;
            SelectItems[i].transform.Find("Label").gameObject.GetComponent<Text>().text = setText;
        }
    }
}
