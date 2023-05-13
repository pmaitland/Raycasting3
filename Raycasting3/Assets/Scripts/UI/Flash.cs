using UnityEngine;
using UnityEngine.UI;

public class Flash : MonoBehaviour
{

    private Image _image;

    void Start()
    {
        _image = GetComponent<Image>();
    }

    void Update()
    {
        if (_image.color.a > 0)
        {
            _image.color -= new Color(0, 0, 0, 0.01f);
        }
    }

    private void DoFlash(Color color)
    {
        _image.color = color;
        _image.color -= new Color(0, 0, 0, 0.75f);
    }

    public void Health()
    {
        DoFlash(Color.green);
    }

    public void Mana()
    {
        DoFlash(Color.blue);
    }

    public void Damage()
    {
        DoFlash(Color.red);
    }

    public void Pickup()
    {
        DoFlash(Color.yellow);
    }
}
