using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MazeGrid {

    private int size;
    private List<List<MazeCell>> grid;

    public MazeGrid(int size) {
        this.size = size;
        grid = new List<List<MazeCell>>();
        for (int i = 0; i < size; i++) {
            grid.Add(new List<MazeCell>());
            for (int j = 0; j < size; j++) {
                grid[i].Add(new MazeCell(j, i, MazeCellType.WALL));
            }
        }
    }

    public MazeCell GetCell(int x, int y) {
        return grid[y][x];
    }

    public MazeCell GetNorthNeighbour(int x, int y) {
        return y > 0 ? grid[y - 1][x] : null;
    }

    public MazeCell GetSouthNeighbour(int x, int y) {
        return y < size - 1 ? grid[y + 1][x] : null;
    }

    public MazeCell GetEastNeighbour(int x, int y) {
        return x < size - 1 ? grid[y][x + 1] : null;
    }

    public MazeCell GetWestNeighbour(int x, int y) {
        return x > 0 ? grid[y][x - 1] : null;
    }

    public List<MazeCell> GetNeighboursOfType(MazeCell cell, params MazeCellType[] types) {
        return GetNeighboursOfType(cell.GetX(), cell.GetY(), types);
    }

    public List<MazeCell> GetNeighboursOfType(int x, int y, params MazeCellType[] types) {
        List<MazeCell> neighbours = new List<MazeCell>();
        MazeCell neighbour = GetNorthNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
        neighbour = GetEastNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
        neighbour = GetSouthNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
        neighbour = GetWestNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
        return neighbours;
    }

    public List<MazeCell> GetNeighboursNotOfType(MazeCell cell, params MazeCellType[] types) {
        return GetNeighboursNotOfType(cell.GetX(), cell.GetY(), types);
    }

    public List<MazeCell> GetNeighboursNotOfType(int x, int y, params MazeCellType[] types) {
        List<MazeCell> neighbours = new List<MazeCell>();
        MazeCell neighbour = GetNorthNeighbour(x, y);
        if (neighbour != null && !types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
        neighbour = GetEastNeighbour(x, y);
        if (neighbour != null && !types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
        neighbour = GetSouthNeighbour(x, y);
        if (neighbour != null && !types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
        neighbour = GetWestNeighbour(x, y);
        if (neighbour != null && !types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
        return neighbours;
    }

    public MazeCell GetSecondNorthNeighbour(int x, int y) {
        return y > 1 ? grid[y - 2][x] : null;
    }

    public MazeCell GetSecondSouthNeighbour(int x, int y) {
        return y < size - 2 ? grid[y + 2][x] : null;
    }

    public MazeCell GetSecondEastNeighbour(int x, int y) {
        return x < size - 2 ? grid[y][x + 2] : null;
    }

    public MazeCell GetSecondWestNeighbour(int x, int y) {
        return x > 1 ? grid[y][x - 2] : null;
    }

    public List<MazeCell> GetSecondNeighboursOfType(int x, int y, params MazeCellType[] types) {
        List<MazeCell> neighbours = new List<MazeCell>();
        MazeCell neighbour = GetSecondNorthNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
        neighbour = GetSecondEastNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
        neighbour = GetSecondSouthNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
        neighbour = GetSecondWestNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.GetCellType())) neighbours.Add(neighbour);
        return neighbours;
    }

    public void SetCellType(int x, int y, MazeCellType newType) {
        grid[y][x].SetCellType(newType);
    }

    public void CleanUp() {
        MazeCellType cellType;
        for (int i = 1; i < size - 1; i++) {
            for (int j = 1; j < size - 1; j++) {
                cellType = grid[j][i].GetCellType();
                if (cellType == MazeCellType.PASSAGE) {
                    if (GetNeighboursOfType(i, j, MazeCellType.WALL).Count == 3 && GetNeighboursOfType(i, j, MazeCellType.PASSAGE).Count == 1) {
                        grid[j][i].SetCellType(MazeCellType.DEAD_END);
                    }
                } else if (cellType == MazeCellType.DISCONNECTED_DOOR) {
                    grid[j][i].SetCellType(MazeCellType.ROOM);
                } else if (cellType == MazeCellType.DISCONNECTED_HIDDEN_DOOR) {
                    grid[j][i].SetCellType(MazeCellType.HIDDEN_ROOM);
                }
            }
        }
    }
}
