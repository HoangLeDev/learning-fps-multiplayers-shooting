using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController I;
    public Slider weaponSlider;
    public TextMeshProUGUI overheatedText;

    private void Awake()
    {
        I = this;
    }
}