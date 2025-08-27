using UnityEngine;
using UnityEngine.UI;

public class HeartsManager : MonoBehaviour
{

    public Image heart1, heart2, heart3, heartExtra;
    public Sprite fullHeart, halfHeart, emptyHeart, fullExtraHeart, halfExtraHeart;

    private int maxHealth = 6; // 3 full hearts
    private int _currentHealth;

    public void SetFullHealth()
    {
        _currentHealth = maxHealth;

        heart1.sprite = fullHeart;
        heart2.sprite = fullHeart;
        heart3.sprite = fullHeart;
        heartExtra.enabled = false;
    }

    public void SetHearts(int health)
    {
        _currentHealth = Mathf.Clamp(health, 0, maxHealth);

        UpdateHeart(heart1, _currentHealth, 0);
        UpdateHeart(heart2, _currentHealth, 2);
        UpdateHeart(heart3, _currentHealth, 4);
    }

    public void SetExtraHeart(int extraHealth)
    {
        if (extraHealth <= 0)
        {
            heartExtra.enabled = false;
        }
        else 
        { 
            heartExtra.enabled = true;

            if (extraHealth == 1) 
            { 
                heartExtra.sprite = halfExtraHeart;
            }
            else 
            { 
                heartExtra.sprite = fullExtraHeart;
            }
        }
    }     

    private void UpdateHeart(Image heartImage, int health, int heartIndex)
    {
        int healthCurrentHeart = health - heartIndex;

        if (healthCurrentHeart >= 2)
        { 
            heartImage.sprite = fullHeart;
        }
        else if (healthCurrentHeart == 1) 
        { 
            heartImage.sprite = halfHeart;
        }
        else 
        { 
            heartImage.sprite = emptyHeart;
        }
    }
}
