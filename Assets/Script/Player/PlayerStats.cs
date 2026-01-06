using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    [Header("Core Stats")]
    public float maxHealth = 100f;
    public float currentHealth;

    public float maxEnergy = 100f;
    public float currentEnergy;
    public float energyDrainRate = 15f;
    public float energyRegenRate = 10f;

    [Header("Currency")]
    public int coins = 1000;
    public int bullets = 50;
    public int maxBullets = 100;
    

    [Header("Slider")]
    public Slider healthSlider;
    public Slider energySlider;
    public TextMeshProUGUI coinsText;
    public Slider bulletSlider;

    public GameObject damageEffect;
    public TextMeshProUGUI bulletsText;
    private void Awake()
    {
        currentHealth = maxHealth;
        currentEnergy = maxEnergy;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
        energySlider.maxValue = maxEnergy;
        energySlider.value = maxEnergy;
        coinsText.text = coins.ToString();
        bulletSlider.maxValue = maxBullets;
        bulletsText.text = bullets.ToString();
        bulletSlider.value = bullets;
        
    }

    private void Update()
    {
        RegenerateEnergy();
    }

    public void DrainEnergy(float amount)
    {
        currentEnergy = Mathf.Max(currentEnergy - amount, 0f);
        if(energySlider != null)
        {
            energySlider.value = currentEnergy;
        }
        
    }

    public void RegenerateEnergy()
    {
        if (!PlayerController.Instance.onRun && energySlider != null)
        {
            currentEnergy = Mathf.Min(currentEnergy + energyRegenRate * Time.deltaTime, maxEnergy);
            energySlider.value = currentEnergy;
        }
    }

    public bool HasEnoughEnergy(float threshold)
    {
        return currentEnergy >= threshold;
    }

    public void UpdateCoins(int amount)
    {
        coins += amount;
        if(coinsText != null)
        {
            coinsText.text = coins.ToString();
        }
        
    }

    public void UpdateBullets(int amount)
    {
        bullets += amount;
        if(bullets > maxBullets)
        {
            bullets = maxBullets;
        }
        if (bulletSlider != null)
        {
            bulletSlider.value = bullets;
        }
        if(bulletsText != null)
        {
            bulletsText.text = bullets.ToString();
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0f);
        if(healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
        Instantiate(damageEffect, transform.position, Quaternion.identity);
        if(currentHealth <= 0)
        {
            Announcement.Instance.GameOver();
        }
    }

    public void RestoreHealth(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthSlider.value = currentHealth;
    }

    public void RestoreEnergy(float amount)
    {
        currentEnergy = Mathf.Min(currentEnergy + amount, maxEnergy);
        energySlider.value = currentEnergy;
    }

    public void IncreaseMaxHealth(float amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public void IncreaseMaxEnergy(float amount)
    {
        maxEnergy += amount;
        currentEnergy = maxEnergy;
        energySlider.maxValue = maxEnergy;
        energySlider.value = currentEnergy;
    }

    public void IncreaseEnergyRegen(float amount)
    {
        energyRegenRate += amount;
    }
}
