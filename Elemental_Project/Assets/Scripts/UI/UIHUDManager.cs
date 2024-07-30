using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHUDManager : MonoBehaviour
{
    [SerializeField] private Slider Healthbar;
    [SerializeField] private Image skill;
    [SerializeField] private Image dash;
    public void SetHUD(PlayerConfig config)
    {
        Healthbar.maxValue = config.maxHealth;
        Healthbar.value = config.health;
    }
}
