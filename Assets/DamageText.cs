using UnityEngine;
using Toolbox.Pooling;

public class DamageText : MonoBehaviour
{
    [SerializeField] Health health;

    [SerializeField] GameObject prefabFX;

    void OnEnable() => health.OnTakeDamage += SpawnDamageText;

    void OnDisable() => health.OnTakeDamage -= SpawnDamageText;

    void SpawnDamageText(int damage){
        if (prefabFX == null)
            return;

        GameObject damageText = Pooler.Spawn(prefabFX, transform.position, Quaternion.identity);
        damageText.GetComponent<FloatingText>().SetText(damage, transform);
    }
}
