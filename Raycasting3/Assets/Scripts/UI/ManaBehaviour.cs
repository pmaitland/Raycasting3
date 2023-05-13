using UnityEngine;
using UnityEngine.UI;

public class ManaBehaviour : MonoBehaviour
{
    public Sprite ManaSprite;
    public Sprite EmptyManaSprite;
    public Sprite NoManaSprite;

    private PlayerBehaviour _playerBehaviour;

    void Start()
    {
        _playerBehaviour = GameObject.Find("Player").GetComponent<PlayerBehaviour>();
    }

    void Update()
    {
        int playerMaxMana = _playerBehaviour.MaxMana;
        int playerCurrentMana = _playerBehaviour.CurrentMana;

        foreach (Transform mana in transform)
        {
            Image manaImage = mana.GetComponent<Image>();
            int manaNumber = int.Parse(mana.name.Split(' ')[1]);

            if (manaNumber > playerMaxMana) manaImage.sprite = NoManaSprite;
            else if (manaNumber > playerCurrentMana) manaImage.sprite = EmptyManaSprite;
            else manaImage.sprite = ManaSprite;
        }
    }
}
