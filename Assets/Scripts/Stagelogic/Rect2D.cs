public class Rect2D
{
    public int left;
    public int top;
    public int right;
    public int bottom;
    public int width { get { return right - left + 1; } }
    public int height { get { return bottom - top + 1; } }

    public Rect2D(int l, int t, int r, int b)
    {
        left = l;
        top = t;
        right = r;
        bottom = b;
    }
}
