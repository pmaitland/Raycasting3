using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class MazeGrid
{

    private readonly int _size;
    private readonly List<List<MazeCell>> _grid;

    public MazeGrid(int size, Color colour)
    {
        _size = size;
        _grid = new List<List<MazeCell>>();
        for (int i = 0; i < size; i++)
        {
            _grid.Add(new List<MazeCell>());
            for (int j = 0; j < size; j++)
            {
                _grid[i].Add(new MazeCell(j, i, MazeCell.Type.WALL, colour));
            }
        }
    }

    public MazeCell GetCell(int x, int y)
    {
        return x < 0 || y < 0 || x >= _size || y >= _size ? null : _grid[y][x];
    }

    public MazeCell GetCell(float x, float y)
    {
        return x < 0 || y < 0 || x >= _size || y >= _size ? null : _grid[Mathf.RoundToInt(y)][Mathf.RoundToInt(x)];
    }

    public MazeCell GetNorthNeighbour(int x, int y)
    {
        return y < _size - 1 ? _grid[y + 1][x] : null;
    }

    public MazeCell GetNorthNeighbour(MazeCell cell)
    {
        return cell.Y < _size - 1 ? _grid[cell.Y + 1][cell.X] : null;
    }

    public MazeCell GetSouthNeighbour(int x, int y)
    {
        return y > 0 ? _grid[y - 1][x] : null;
    }

    public MazeCell GetSouthNeighbour(MazeCell cell)
    {
        return cell.Y > 0 ? _grid[cell.Y - 1][cell.X] : null;
    }

    public MazeCell GetEastNeighbour(int x, int y)
    {
        return x < _size - 1 ? _grid[y][x + 1] : null;
    }

    public MazeCell GetEastNeighbour(MazeCell cell)
    {
        return cell.X < _size - 1 ? _grid[cell.Y][cell.X + 1] : null;
    }

    public MazeCell GetWestNeighbour(int x, int y)
    {
        return x > 0 ? _grid[y][x - 1] : null;
    }

    public MazeCell GetWestNeighbour(MazeCell cell)
    {
        return cell.X > 0 ? _grid[cell.Y][cell.X - 1] : null;
    }

    public List<MazeCell> GetNeighboursOfType(MazeCell cell, params MazeCell.Type[] types)
    {
        return GetNeighboursOfType(cell.X, cell.Y, types);
    }

    public List<MazeCell> GetNeighboursOfType(int x, int y, params MazeCell.Type[] types)
    {
        List<MazeCell> neighbours = new();
        MazeCell neighbour = GetNorthNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.CurrentType)) neighbours.Add(neighbour);
        neighbour = GetEastNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.CurrentType)) neighbours.Add(neighbour);
        neighbour = GetSouthNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.CurrentType)) neighbours.Add(neighbour);
        neighbour = GetWestNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.CurrentType)) neighbours.Add(neighbour);
        return neighbours;
    }

    public List<MazeCell> GetNeighboursNotOfType(MazeCell cell, params MazeCell.Type[] types)
    {
        return GetNeighboursNotOfType(cell.X, cell.Y, types);
    }

    public List<MazeCell> GetNeighboursNotOfType(int x, int y, params MazeCell.Type[] types)
    {
        List<MazeCell> neighbours = new();
        MazeCell neighbour = GetNorthNeighbour(x, y);
        if (neighbour != null && !types.Contains(neighbour.CurrentType)) neighbours.Add(neighbour);
        neighbour = GetEastNeighbour(x, y);
        if (neighbour != null && !types.Contains(neighbour.CurrentType)) neighbours.Add(neighbour);
        neighbour = GetSouthNeighbour(x, y);
        if (neighbour != null && !types.Contains(neighbour.CurrentType)) neighbours.Add(neighbour);
        neighbour = GetWestNeighbour(x, y);
        if (neighbour != null && !types.Contains(neighbour.CurrentType)) neighbours.Add(neighbour);
        return neighbours;
    }

    public MazeCell GetSecondNorthNeighbour(int x, int y)
    {
        return y < _size - 2 ? _grid[y + 2][x] : null;
    }

    public MazeCell GetSecondSouthNeighbour(int x, int y)
    {
        return y > 1 ? _grid[y - 2][x] : null;
    }

    public MazeCell GetSecondEastNeighbour(int x, int y)
    {
        return x < _size - 2 ? _grid[y][x + 2] : null;
    }

    public MazeCell GetSecondWestNeighbour(int x, int y)
    {
        return x > 1 ? _grid[y][x - 2] : null;
    }

    public List<MazeCell> GetSecondNeighboursOfType(int x, int y, params MazeCell.Type[] types)
    {
        List<MazeCell> neighbours = new();
        MazeCell neighbour = GetSecondNorthNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.CurrentType)) neighbours.Add(neighbour);
        neighbour = GetSecondEastNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.CurrentType)) neighbours.Add(neighbour);
        neighbour = GetSecondSouthNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.CurrentType)) neighbours.Add(neighbour);
        neighbour = GetSecondWestNeighbour(x, y);
        if (neighbour != null && types.Contains(neighbour.CurrentType)) neighbours.Add(neighbour);
        return neighbours;
    }

    public void SetCellType(int x, int y, MazeCell.Type newType)
    {
        _grid[y][x].CurrentType = newType;
    }

    public void SetCellType(float x, float y, MazeCell.Type newType)
    {
        _grid[Mathf.RoundToInt(y)][Mathf.RoundToInt(x)].CurrentType = newType;
    }

    public void SetCell(HiddenDoorMazeCell newCell)
    {
        _grid[newCell.Y][newCell.X] = newCell;
    }

    public void SetLightingLower(MazeCell mazeCell, Lighting.Type lightingType)
    {
        if (Lighting.IsStronger(mazeCell.LightingLower, lightingType)) return;

        mazeCell.LightingLower = lightingType;
        foreach (MazeCell neighbour in GetNeighboursNotOfType(mazeCell, MazeCell.Type.WALL))
        {
            if (neighbour is HiddenDoorMazeCell hiddenDoorMazeCell)
            {
                if (hiddenDoorMazeCell.IsDoorOpen()) { SetLightingLower(neighbour, Lighting.GetDarker(lightingType)); }
            }
            else
            {
                SetLightingLower(neighbour, Lighting.GetDarker(lightingType));
            }
        }
    }

    public void SetLightingUpper(MazeCell mazeCell, Lighting.Type lightingType)
    {
        if (Lighting.IsStronger(mazeCell.LightingUpper, lightingType)) return;

        mazeCell.LightingUpper = lightingType;
        foreach (MazeCell neighbour in GetNeighboursNotOfType(mazeCell, MazeCell.Type.WALL))
        {
            if (neighbour is HiddenDoorMazeCell hiddenDoorMazeCell)
            {
                if (hiddenDoorMazeCell.IsDoorOpen()) { SetLightingUpper(neighbour, Lighting.GetDarker(lightingType)); }
            }
            else
            {
                SetLightingUpper(neighbour, Lighting.GetDarker(lightingType));
            }
        }
    }

    public void SetTemporaryLightingLower(MazeCell mazeCell, Lighting.Type lightingType)
    {
        if (Lighting.IsStronger(mazeCell.TemporaryLightingLower, lightingType)) return;

        mazeCell.TemporaryLightingLower = lightingType;
        foreach (MazeCell neighbour in GetNeighboursNotOfType(mazeCell, MazeCell.Type.WALL))
        {
            if (neighbour is HiddenDoorMazeCell hiddenDoorMazeCell)
            {
                if (hiddenDoorMazeCell.IsDoorOpen()) { SetTemporaryLightingLower(neighbour, Lighting.GetDarker(lightingType)); }
            }
            else
            {
                SetTemporaryLightingLower(neighbour, Lighting.GetDarker(lightingType));
            }
        }
    }

    public void SetTemporaryLightingUpper(MazeCell mazeCell, Lighting.Type lightingType)
    {
        if (Lighting.IsStronger(mazeCell.TemporaryLightingUpper, lightingType)) return;

        mazeCell.TemporaryLightingUpper = lightingType;
        foreach (MazeCell neighbour in GetNeighboursNotOfType(mazeCell, MazeCell.Type.WALL))
        {
            if (neighbour is HiddenDoorMazeCell hiddenDoorMazeCell)
            {
                if (hiddenDoorMazeCell.IsDoorOpen()) { SetTemporaryLightingUpper(neighbour, Lighting.GetDarker(lightingType)); }
            }
            else
            {
                SetTemporaryLightingUpper(neighbour, Lighting.GetDarker(lightingType));
            }
        }
    }

    public void CleanUp()
    {
        MazeCell.Type cellType;
        for (int i = 1; i < _size - 1; i++)
        {
            for (int j = 1; j < _size - 1; j++)
            {
                cellType = _grid[j][i].CurrentType;
                if (cellType == MazeCell.Type.PASSAGE)
                {
                    if (GetNeighboursOfType(i, j, MazeCell.Type.WALL).Count == 3 && GetNeighboursOfType(i, j, MazeCell.Type.PASSAGE).Count == 1)
                    {
                        if (GetNorthNeighbour(i, j).CurrentType == MazeCell.Type.PASSAGE && GetSecondSouthNeighbour(i, j) != null && GetSecondSouthNeighbour(i, j).CurrentType == MazeCell.Type.PASSAGE)
                        {
                            _grid[j - 1][i].CurrentType = MazeCell.Type.PASSAGE;
                        }
                        else if (GetSouthNeighbour(i, j).CurrentType == MazeCell.Type.PASSAGE && GetSecondNorthNeighbour(i, j) != null && GetSecondNorthNeighbour(i, j).CurrentType == MazeCell.Type.PASSAGE)
                        {
                            _grid[j + 1][i].CurrentType = MazeCell.Type.PASSAGE;
                        }
                        else if (GetEastNeighbour(i, j).CurrentType == MazeCell.Type.PASSAGE && GetSecondWestNeighbour(i, j) != null && GetSecondWestNeighbour(i, j).CurrentType == MazeCell.Type.PASSAGE)
                        {
                            _grid[j][i - 1].CurrentType = MazeCell.Type.PASSAGE;
                        }
                        else if (GetWestNeighbour(i, j).CurrentType == MazeCell.Type.PASSAGE && GetSecondEastNeighbour(i, j) != null && GetSecondEastNeighbour(i, j).CurrentType == MazeCell.Type.PASSAGE)
                        {
                            _grid[j][i + 1].CurrentType = MazeCell.Type.PASSAGE;
                        }
                        else
                        {
                            _grid[j][i].CurrentType = MazeCell.Type.PASSAGE;
                        }
                    }
                }
                else if (cellType == MazeCell.Type.DISCONNECTED_DOOR)
                {
                    _grid[j][i].CurrentType = MazeCell.Type.DOOR;
                }
                else if (cellType == MazeCell.Type.DISCONNECTED_HIDDEN_DOOR)
                {
                    _grid[j][i].CurrentType = MazeCell.Type.HIDDEN_ROOM;
                }
            }
        }
    }
}
