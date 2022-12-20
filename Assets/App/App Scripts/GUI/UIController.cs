using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : Singleton<UIController>
{
    public Slider weaponSlider;
    public TextMeshProUGUI overheatedText;

    public GameObject deathScreen;
    public TMP_Text deathTMP;
}