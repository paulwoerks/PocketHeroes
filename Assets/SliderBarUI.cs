using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Toolbox.Events;

public class SliderBarUI : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Health health;
    void OnEnable(){
        health.OnUpdateHealth += SetValue;
    }

    void OnDisable(){
        health.OnUpdateHealth -= SetValue;
    }

    void SetValue(){
        slider.value = health.HP;
        slider.maxValue = health.HPmax;
    }
}
