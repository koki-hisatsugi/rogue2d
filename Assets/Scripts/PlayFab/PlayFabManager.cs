using Cysharp.Threading.Tasks;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabManager : MonoBehaviour
{
    const string STATISTICS_NAME = "HighScore[hierarchy]";

    public int score;
    // public string customId;
    public GameObject Content;
    public GameObject RankText;
    private GameObject MyRankText;
    private Text MyText;
    private string _DisplayName;
    public Text UserName;
    public InputField InputUserName;
    // ランキングを取得するまでの秒数
    const int RankingBoardGetDelaySpan = 1;

    public void Login(){
        PlayFabClientAPI.LoginWithCustomID(
            new LoginWithCustomIDRequest { CustomId = CreateNewId(), CreateAccount = true},
            result => Debug.Log("ログイン成功！"),
            error => Debug.Log("ログイン失敗"));
    }
    // 一意のIdを生成する
    string CreateNewId () {
        return System.Guid.NewGuid().ToString();
    }

    public void ButtonMethod(){
        SubmitScoreButton();
    }

    async private UniTask SubmitScoreButton(){
        SubmitDisplayName();
        SubmitScore(score);
        await UniTask.Delay(TimeSpan.FromSeconds(RankingBoardGetDelaySpan));
        RequestLeaderBoard();
    }

    void SubmitDisplayName(){
        _DisplayName = (InputUserName.text).Equals("") ? _DisplayName : InputUserName.text;
        PlayFabClientAPI.UpdateUserTitleDisplayName(
            new UpdateUserTitleDisplayNameRequest{
                DisplayName = _DisplayName
            },
            result =>
            {
                InputUserName.text = "";
                Debug.Log("送信完了");
            },
            error =>
            {
                Debug.Log(error.GenerateErrorReport());
            });
    }
    void SubmitScore(int playerScore)
    {
        PlayFabClientAPI.UpdatePlayerStatistics(
            new UpdatePlayerStatisticsRequest
            {
                Statistics = new List<StatisticUpdate>()
                {
                    new StatisticUpdate
                    {
                        StatisticName = STATISTICS_NAME,
                        Value = playerScore
                    }
                }
            },
            result =>
            {
                Debug.Log("スコア送信");
            },
            error =>
            {
                Debug.Log(error.GenerateErrorReport());
            }
            );
    }


    public void RequestLeaderBoard()
    {
        if(Content.transform.childCount > 0){
            foreach(Transform content in Content.transform){
                Destroy(content.gameObject);
            }
        }
        PlayFabClientAPI.GetLeaderboard(
            new GetLeaderboardRequest
            {
                StatisticName = STATISTICS_NAME,
                StartPosition = 0,
                MaxResultsCount = 10
            },
            result =>
            {
                result.Leaderboard.ForEach(
                    x => {
                        MyRankText = Instantiate(RankText, Content.transform.position, Quaternion.identity);
                        MyRankText.transform.parent = Content.transform;
                        MyText = MyRankText.GetComponent<Text>();
                        MyText.text = string.Format("{0}位 [階層：{1}] {2} ", x.Position + 1, x.StatValue, x.DisplayName);
                        }
                    
                    );
            },
            error =>
            {
                Debug.Log(error.GenerateErrorReport());
            }
            );
    }

    public void TitleBack(){
        SceneLoader.SceneLoading("TitleScene");
    }

}
