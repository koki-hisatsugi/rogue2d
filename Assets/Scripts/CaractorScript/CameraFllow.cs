using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// カメラの追従を管理するクラス(部品)　基本的にプレイヤーにつける
public class CameraFllow : MonoBehaviour
{
    private GameObject mCamera;

    private void Awake()
    {
        mCamera = GameObject.Find("Main Camera");
        mCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    public void cameraMoveTowards(Vector3 target, float speed){
        mCamera.transform.position = Vector3.MoveTowards(mCamera.transform.position, target, speed);
    }
}
