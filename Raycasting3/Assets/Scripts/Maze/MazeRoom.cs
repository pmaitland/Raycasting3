public class MazeRoom
{

    public int X { get; private set; }
    public int Y { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }

    public MazeRoom(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public static bool Overlap(MazeRoom roomA, MazeRoom roomB)
    {
        return roomA.X - 1 < roomB.X + roomB.Width + 1 && roomA.X + roomA.Width + 1 > roomB.X - 1
            && roomA.Y - 1 < roomB.Y + roomB.Height + 1 && roomA.Y + roomA.Height + 1 > roomB.Y - 1;
    }
}