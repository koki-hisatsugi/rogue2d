using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private List<Node> _allNodeList = new List<Node>();
    private List<Node> _shotWay = new List<Node>();
    public enum NodeStatus{
        none,
        open,
        close,
    }
    public class Node{
        int _x , _y;
        public int GetSetX{
            get { return _x; }
            set { _x = value; }
        }
        public int GetSetY{
            get { return _y; }
            set { _y = value; }
        }
        int _endX, _endY;
        public int GetSetEndX{
            get { return _endX; }
            set { _endX = value; }
        }
        public int GetSetEndY{
            get { return _endY; }
            set { _endY = value; }
        }
        int _cost;
        public int GetCost{
            get { return _cost; }
        }
        int _score;
        public int GetSetScore{
            get { return _score; }
            set { _score = value; }
        }
        int _parentPointer;
        public int GetSetPP{
            get { return _parentPointer; }
            set { _parentPointer = value; }
        }
        NodeStatus _thisStatus;
        public NodeStatus GetStatus{
            get { return _thisStatus; }
        }

        public void setStatus(int judgeInt){
            switch(judgeInt){
                case 0:
                    _thisStatus = NodeStatus.none;
                    break;
                case 1:
                    _thisStatus = NodeStatus.open;
                    break;
                case 2:
                    _thisStatus = NodeStatus.close;
                    break;
            }
        }

        public Node(int sX, int sY, int eX, int eY, int cost){
            _x = sX;
            _y = sY;
            _endX = eX;
            _endY = eY;
            _cost = cost;

            _score = Mathf.Abs(_endX - _x) + Mathf.Abs(_endY - _y) + _cost;

            _parentPointer = -1;
            _thisStatus = NodeStatus.none;
        }
    }

    // private int playerPosX, playerPosY;

    public CoordinateXY AstarAlgo(int startX, int startY, Array2D baseA2d){
        // 返却用の座標格納クラス
        CoordinateXY resultCXY = new CoordinateXY(0,0,0,0);
        PlayerSearch(baseA2d, resultCXY);
        // プレイヤーの位置がエネミーから離れすぎている場合は経路の判別を行わない
        float dX = (float)Mathf.Abs(Mathf.Abs(resultCXY.GetSetPX)-Mathf.Abs(startX));
        float dY = (float)Mathf.Abs(Mathf.Abs(resultCXY.GetSetPY)-Mathf.Abs(startY));
        if(dX+dY > 50){
            resultCXY.GetSetX = 2;
            resultCXY.GetSetY = 2;
            return resultCXY;
        }

        // ゴール到達判定用のノード
        Node endNode = new Node(resultCXY.GetSetPX,resultCXY.GetSetPY,resultCXY.GetSetPX,resultCXY.GetSetPY,0);
        endNode.setStatus(0);
        // スタートノード
        Node startNode = new Node(startX, startY, resultCXY.GetSetPX,resultCXY.GetSetPY, 0);
        startNode.setStatus(1);
        startNode.GetSetPP = -1;
        _allNodeList.Add(startNode);
        bool breaker = false;
        while(true){
            int minScore = 100000;
            int minPointer = 0;
            for(int i = 0; i < _allNodeList.Count; i++){
                if(_allNodeList[i].GetStatus == NodeStatus.open){
                    if(_allNodeList[i].GetSetScore < minScore){
                        minScore = _allNodeList[i].GetSetScore;
                        minPointer = i;
                    }
                }
            }
            if(NodeJudge(_allNodeList[minPointer],endNode)){
                Debug.Log("ジャッジブレイク");
                breaker = true;
                endNode = _allNodeList[minPointer];
                endNode.setStatus(2);
                break;
            }
            // 経路のOpen数が50を超えたら探索打ち切り
            if(_allNodeList.Count >= 50){
                breaker = true;
                endNode = _allNodeList[minPointer];
                endNode.setStatus(2);
                break;
            }
            _allNodeList[minPointer].setStatus(2);
            openFourDir(_allNodeList[minPointer], baseA2d, minPointer);
        }
        // END_WHILE:
        // Debug.Log("allNodeListの要素数:"+_allNodeList.Count);
        // Debug.Log(_allNodeList[_allNodeList.Count-1].GetSetX+"   " + _allNodeList[_allNodeList.Count-1].GetSetY);
        Node tmpNode = endNode;
        if(breaker){
            while(tmpNode.GetSetPP > 0){
                _shotWay.Add(_allNodeList[tmpNode.GetSetPP]);
                tmpNode = _allNodeList[tmpNode.GetSetPP];
            }
            if(_shotWay.Count > 0){
                resultCXY.GetSetX = _shotWay[_shotWay.Count - 1].GetSetX;
                resultCXY.GetSetY = _shotWay[_shotWay.Count - 1].GetSetY;
            }
        }
        return resultCXY;
    }

    private void openFourDir(Node baseNode, Array2D baseA2d, int _pP){
        // 上
        if(baseA2d.Get(baseNode.GetSetX,baseNode.GetSetY + 1).GetSetMapValue != BoardRemote.WallNum){
            Node upNode = new Node(baseNode.GetSetX,baseNode.GetSetY + 1,baseNode.GetSetEndX,baseNode.GetSetEndY, baseNode.GetCost + Random.Range(1,3));
            upNode.GetSetPP = _pP;
            upNode.setStatus(1);
            bool isAdd = true;
            foreach(Node tmpNode in _allNodeList){
                if(NodeJudge(tmpNode,upNode)){
                    isAdd =false;
                    break;
                    // goto UP_IF_END;
                }
            }
            if(isAdd){
                _allNodeList.Add(upNode);
            }
        }
        // UP_IF_END:
        // 下
        if(baseA2d.Get(baseNode.GetSetX,baseNode.GetSetY - 1).GetSetMapValue != BoardRemote.WallNum){
            Node downNode = new Node(baseNode.GetSetX,baseNode.GetSetY - 1,baseNode.GetSetEndX,baseNode.GetSetEndY, baseNode.GetCost + Random.Range(1,3));
            downNode.GetSetPP = _pP;
            downNode.setStatus(1);
            bool isAdd = true;
            foreach(Node tmpNode in _allNodeList){
                if(NodeJudge(tmpNode,downNode)){
                    isAdd =false;
                    break;
                    // goto DOWN_IF_END;
                }
            }
            if(isAdd){
                _allNodeList.Add(downNode);
            }
        }
        // DOWN_IF_END:
        // 右
        if(baseA2d.Get(baseNode.GetSetX + 1,baseNode.GetSetY).GetSetMapValue != BoardRemote.WallNum){
            Node rightNode = new Node(baseNode.GetSetX + 1,baseNode.GetSetY,baseNode.GetSetEndX,baseNode.GetSetEndY, baseNode.GetCost + Random.Range(1,3));
            rightNode.GetSetPP = _pP;
            rightNode.setStatus(1);
            bool isAdd = true;
            foreach(Node tmpNode in _allNodeList){
                if(NodeJudge(tmpNode,rightNode)){
                    isAdd =false;
                    break;
                    // goto RIGHT_IF_END;
                }
            }
            if(isAdd){
                _allNodeList.Add(rightNode);
            }
        }
        // RIGHT_IF_END:
        // 左
        if(baseA2d.Get(baseNode.GetSetX - 1,baseNode.GetSetY).GetSetMapValue != BoardRemote.WallNum){
            Node leftNode = new Node(baseNode.GetSetX - 1,baseNode.GetSetY,baseNode.GetSetEndX,baseNode.GetSetEndY, baseNode.GetCost + Random.Range(1,3));
            leftNode.GetSetPP = _pP;
            leftNode.setStatus(1);
            bool isAdd = true;
            foreach(Node tmpNode in _allNodeList){
                if(NodeJudge(tmpNode,leftNode)){
                    isAdd =false;
                    break;
                }
            }
            if(isAdd){
                _allNodeList.Add(leftNode);
            }
        }
    }

    private void PlayerSearch(Array2D baseA2d, CoordinateXY CXY){
        for(int z = 0; z < baseA2d.height; z++){
            for(int x = 0; x < baseA2d.width; x++){
                if(baseA2d.Get(x,z).GetSetMapOnActor == BoardRemote.PlayerNum){
                    CXY.GetSetPX = x;
                    CXY.GetSetPY = z;
                    // playerPosX = x;
                    // playerPosY = z;
                    // Debug.Log("playerPosX:"+playerPosX + "   playerPosY:"+playerPosY);
                    return;
                }
            }
        }
    }
    private bool NodeJudge(Node farstNode, Node secondNode){
        if(farstNode.GetSetX == secondNode.GetSetX && farstNode.GetSetY == secondNode.GetSetY){
            return true;
        }

        return false;
    }

}
