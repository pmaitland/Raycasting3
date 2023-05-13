using UnityEngine;
using UnityEngine.UI;

public class HeartBehaviour : MonoBehaviour
{

    public Sprite HeartSprite;
    public Sprite EmptyHeartSprite;
    public Sprite NoHeartSprite;

    private Health _playerHealth;

    void Start()
    {
        _playerHealth = GameObject.Find("Player").GetComponent<Health>();
    }

    void Update()
    {
        int playerMaxHealth = _playerHealth.MaxHealth;
        int playerCurrentHealth = _playerHealth.CurrentHealth;

        foreach (Transform heart in transform)
        {
            Image heartImage = heart.GetComponent<Image>();
            int heartNumber = int.Parse(heart.name.Split(' ')[1]);

            if (heartNumber > playerMaxHealth) { heartImage.sprite = NoHeartSprite; }
            else if (heartNumber > playerCurrentHealth) { heartImage.sprite = EmptyHeartSprite; }
            else { heartImage.sprite = HeartSprite; }
        }
    }
}
