public class MazeCell {
    
    private int x;
    private int y;
    private MazeCellType type;

    public MazeCell(int x, int y, MazeCellType type) {
        this.x = x;
        this.y = y;
        this.type = type;
    }

    public int GetX() {
        return x;
    }

    public int GetY() {
        return y;
    }

    public MazeCellType GetCellType() {
        return type;
    }

    public void SetCellType(MazeCellType newType) {
        type = newType;
    }
}
