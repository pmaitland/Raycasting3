using UnityEngine;
using UnityEngine.UI;

public class MinimapBehaviour : MonoBehaviour
{

    public GameObject CellPrefab;

    private GameBehaviour _gameController;

    private float _scaleFactor;
    private float _minimapCellSize;
    private GameObject _playerMinimapCell;

    void Start()
    {
        _gameController = GameObject.Find("Controller").GetComponent<GameBehaviour>();

        _scaleFactor = GetComponentInParent<Canvas>().scaleFactor;
        _minimapCellSize = transform.GetComponent<RectTransform>().rect.width / (float)_gameController.GetMazeSize();
        _playerMinimapCell = CreateMinimapCell(0, 0, "Player", Color.green, true);
    }

    public GameObject CreateMinimapCell(int x, int y, string name, Color color, bool active)
    {
        GameObject cell = Instantiate(CellPrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));

        cell.name = name;
        cell.GetComponent<Image>().color = color;
        cell.SetActive(active);

        cell.GetComponent<RectTransform>().sizeDelta = new Vector2(_minimapCellSize * _scaleFactor, _minimapCellSize * _scaleFactor);

        cell.transform.SetParent(transform);
        cell.transform.localPosition = new Vector3(
            -transform.GetComponent<RectTransform>().rect.width * 0.5f + _minimapCellSize * (y + 0.5f),
            transform.GetComponent<RectTransform>().rect.height * 0.5f - _minimapCellSize * (x + 0.5f),
            0
        );

        return cell;
    }

    public void ActivateMinimapCell(int x, int y)
    {
        transform.Find(x + "," + y).gameObject.SetActive(true);
    }

    public void ActivateMinimapCell(float x, float y)
    {
        ActivateMinimapCell(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }

    public void MovePlayerMinimapCell(int x, int y)
    {
        Transform currentCell = transform.Find(x + "," + y);
        _playerMinimapCell.transform.position = new Vector3(currentCell.position.x, currentCell.position.y, 0);
        _playerMinimapCell.transform.SetAsLastSibling();
    }

    public void MovePlayerMinimapCell(float x, float y)
    {
        MovePlayerMinimapCell(Mathf.RoundToInt(x), Mathf.RoundToInt(y));
    }
}
