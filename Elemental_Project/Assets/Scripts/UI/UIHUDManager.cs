using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHUDManager : MonoBehaviour
{
    [SerializeField] private Slider Healthbar;
    public void SetHUD(PlayerConfig config)
    {
        Healthbar.maxValue = config.maxHealth;
        Healthbar.value = config.health;
    }
}
