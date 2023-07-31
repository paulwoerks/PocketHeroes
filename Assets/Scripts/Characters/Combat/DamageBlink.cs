using System.Collections;
using UnityEngine;
using System;

public class DamageBlink : MonoBehaviour
{
    // IMPORTANT! Emission has to be enabled by default on the material
    [SerializeField] float flashTime = .15f;
    [SerializeField] Color flashColor = new Color(0.39f, 0.39f, 0.39f);
    [SerializeField] Damagable health;
    [SerializeField] Renderer rend;
    bool isEmission;
    Color originalEmissionColor;

    Coroutine DoBlink;

    void Start() {
        originalEmissionColor = rend.material.GetColor("_EmissionColor");
        isEmission = rend.material.IsKeywordEnabled("_EMISSION");      
    }

    void OnEnable(){
        health.OnTakeDamage += StartBlink;
    }

    void OnDisable(){
        health.OnTakeDamage -= StartBlink;
        
        if (DoBlink != null){
            StopCoroutine(DoBlink);
            Stop();
        }
    }

    void StartBlink(int damage){
        if (DoBlink != null){
            StopCoroutine(DoBlink);
            Stop();
        }
        DoBlink = StartCoroutine(Blink());
    }

    IEnumerator Blink(){
        rend.material.SetColor("_EmissionColor", flashColor);
        rend.material.EnableKeyword("_EMISSION");
        yield return new WaitForSeconds(flashTime);
        Stop();
    }

    void Stop(){
        rend.material.SetColor("_EmissionColor", originalEmissionColor);
        if (!isEmission)
            rend.material.DisableKeyword("_EMISSION");
        DoBlink = null;
    }
}
