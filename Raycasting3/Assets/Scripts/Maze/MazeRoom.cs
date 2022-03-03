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
        return roomA.GetX() < roomB.GetX() + roomB.GetWidth()  && roomA.GetX() + roomA.GetWidth()  > roomB.GetX()
            && roomA.GetY() < roomB.GetY() + roomB.GetHeight() && roomA.GetY() + roomA.GetHeight() > roomB.GetY();
    }
}