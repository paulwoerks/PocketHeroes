using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] Damagable damagable;
    void OnEnable() => damagable.OnUpdateHealth += SetValue;

    void OnDisable() => damagable.OnUpdateHealth -= SetValue;

    void SetValue(){
        slider.value = damagable.HP;
        slider.maxValue = damagable.MaxHP;
    }
}
