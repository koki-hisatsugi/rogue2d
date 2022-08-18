using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public string SceneName;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            SceneManager.LoadScene(SceneName);
        }
    }

    public static void SceneLoading(string sceneName){
        SceneManager.LoadScene(sceneName);
    }
}
