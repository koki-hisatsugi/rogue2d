using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class YesButton : MonoBehaviour
{
    private GameObject GMO;
    private GameManager GM;

    private Button testButton;
	private Text buttonText;
    // Start is called before the first frame update
    void Start()
    {
        GMO = GameObject.Find("GameManager");
        GM = GMO.GetComponent<GameManager>();

        testButton = GetComponent <Button>();
		buttonText = testButton.transform.GetChild (0).GetComponent <Text> ();
		buttonText.text = "test1";
		testButton.onClick.AddListener (OnClickButton);
    }

    // Update is called once per frame
	public void OnClickButton() {
		Debug.Log ("クリックされた");
		//　ボタンの文字列に応じて書き換える
		buttonText.text = (buttonText.text == "test1") ? "test2" : "test1"; 
	}
}
