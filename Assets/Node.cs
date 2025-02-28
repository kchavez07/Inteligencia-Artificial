public class Node
{
    public int x, y;
    public bool isWalkable;
    public bool partOfRoute = false;
    public Node parentRef;

    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
        isWalkable = true;
        parentRef = null;
    }
}
