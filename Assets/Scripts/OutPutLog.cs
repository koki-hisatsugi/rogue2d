using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LogManagers
{
    public class OutPutLog : MonoBehaviour
    {
        private int textCount; // 子オブジェクト(Text)の数
        private Text[] logText;
        private textProperty[] textProperty;
        [SerializeField] float fadeoutTime = 1f; // 完全に透明になるまでにかかる時間(秒)
        [SerializeField] float fadeoutStartTime = 5f; // 透明化が始まるまでにかかる時間(秒)

        void Start()
        {
            textCount = transform.childCount;
            logText = new Text[textCount];
            textProperty = new textProperty[textCount];
            for (int i = 0; i < textCount; i++)
            {
                logText[i] = transform.GetChild(i).GetComponent<Text>();
                logText[i].color = new Color(logText[i].color.r, logText[i].color.g, logText[i].color.b, 0f);
                textProperty[i].Alfa = 0f;
                textProperty[i].ElapsedTime = 0f;
            }
        }

        void Update()
        {
            // 一番上のテキストは強制的に透明化開始させる
            if (textProperty[0].Alfa == 1)
            {
                textProperty[0].ElapsedTime = fadeoutStartTime;
            }

            for (int i = textCount - 1; i >= 0; i--)
            {
                if (textProperty[i].Alfa > 0)
                {
                    // 経過時間がfadeoutStartTime未満なら時間をカウント
                    // そうでなければ透明度を下げる
                    if (textProperty[i].ElapsedTime < fadeoutStartTime)
                    {
                        textProperty[i].ElapsedTime += Time.deltaTime;
                    }
                    else
                    {
                        textProperty[i].Alfa -= Time.deltaTime / fadeoutTime;
                        logText[i].color = new Color(logText[i].color.r, logText[i].color.g, logText[i].color.b,
                                           textProperty[i].Alfa);
                    }
                }
                else
                {
                    break;
                }
            }
        }
        // ログ出力
        public void OutputLog(string itemName)
        {
            if (textProperty[textCount - 1].Alfa > 0)
            {
                UplogText();
            }
            logText[textCount - 1].text = itemName; // ここの文字列を変えればログの文章が変わります
            ResetTextPropety();
        }
        // ログを一つ上にずらす
        private void UplogText()
        {
            // 古いほうからずらす
            for (int i = 0; i < textCount - 1; i++)
            {
                logText[i].text = logText[i + 1].text;
                textProperty[i].Alfa = textProperty[i + 1].Alfa;
                textProperty[i].ElapsedTime = textProperty[i + 1].ElapsedTime;
                logText[i].color = new Color(logText[i].color.r, logText[i].color.g, logText[i].color.b,
                                   textProperty[i].Alfa);
            }
        }
        // ログの初期化
        private void ResetTextPropety()
        {
            textProperty[textCount - 1].Alfa = 1f;
            textProperty[textCount - 1].ElapsedTime = 0f;
            logText[textCount - 1].color = new Color(logText[textCount - 1].color.r, logText[textCount - 1].color.g, logText[textCount - 1].color.b,
                                           textProperty[textCount - 1].Alfa);
        }
    }

    struct textProperty
    {
        private float _alfa;
        public float Alfa // 透明度、0未満なら0にする
        {
            get
            {
                return _alfa;
            }
            set
            {
                _alfa = value < 0 ? 0 : value;
            }
        }
        public float ElapsedTime { get; set; } // ログが出力されてからの経過時間
    }
}
