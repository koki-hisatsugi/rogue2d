public class CoordinateXY
{
    public CoordinateXY(int x, int y, int px, int py){
        _x = x;
        _y = y;
        _px = px;
        _py = py;
    }
    // エネミーの移動マス格納変数
    int _x,_y;
    // プレイヤーの現在地マス格納変数
    int _px,_py;
    public int GetSetX{
        get { return _x; }
        set { _x = value; }
    }
    public int GetSetY{
        get { return _y; }
        set { _y = value; }
    }

        public int GetSetPX{
        get { return _px; }
        set { _px = value; }
    }
        public int GetSetPY{
        get { return _py; }
        set { _py = value; }
    }
}
