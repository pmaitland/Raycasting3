public class MazeRoom {
    
    private int x;
    private int y;
    private int width;
    private int height;

    public MazeRoom(int x, int y, int width, int height) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    public int GetX() {
        return x;
    }

    public int GetY() {
        return y;
    }

    public int GetWidth() {
        return width;
    }

    public int GetHeight() {
        return height;
    }

    public static bool Overlap(MazeRoom roomA, MazeRoom roomB) {
        return roomA.GetX() - 1 < roomB.GetX() + roomB.GetWidth() + 1 && roomA.GetX() + roomA.GetWidth() + 1  > roomB.GetX() - 1
            && roomA.GetY() - 1 < roomB.GetY() + roomB.GetHeight() + 1 && roomA.GetY() + roomA.GetHeight() + 1 > roomB.GetY() - 1;
    }
}