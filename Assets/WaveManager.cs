using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox.Events;

public class WaveManager : MonoBehaviour
{
    // Types of Wave: 
    // Goal : Survive X Seconds [Countdown]
    // Goal : Defeat all Enemies [Clear]
    // Goal : Defeat the Boss [Boss]
    // Goal : Fight until you Die [Endless]
    // Goal : Collect X Drops [Collect]
    // Goal : Free X NPCs [Free]

    [SerializeField] EnemySpawner spawner;

    int waves = 5;
    int activeWave = 0;

    public int WavesLeft => waves - activeWave;

    [SerializeField] GameObject prefab;

    float waveTimer = 0;
    bool IsTimeLimit => waveTimer > 0f;
    [SerializeField] int waveTime = 20;

    bool waveActive;

    [Header("Listening to ...")]
    [SerializeField] VoidChannelSO OnStartWave;

    [Header("Broadcasting on ...")]
    [SerializeField] VoidChannelSO OnWaveEnd;

    void OnEnable() => OnStartWave.Subscribe(StartWave, this);
    void OnDisable() => OnStartWave.Unsubscribe(StartWave, this);

    // Time Limit Wave
    void Update(){
        if (IsTimeLimit){
            waveTimer -= Time.deltaTime;
        } else if (waveActive)
            StopWave();

        // Debug
        if (Input.GetKeyDown(KeyCode.Space))
            OnStartWave.Invoke();
        if (Input.GetKeyDown(KeyCode.R))
            spawner.Spawn(prefab);
    }

    void StartWave(){
        if (waveActive)
            return;

        waveActive = true;
        waveTimer = waveTime;
        StartCoroutine(RunWave());
        activeWave++;
    }

    IEnumerator RunWave(){
        while (waveTimer > 0f){
            spawner.Spawn(prefab);
            yield return new WaitForSeconds(1);
        }
    }

    void StopWave(){
        waveActive = false;
        waveTimer = 0f;
        spawner.DespawnAll();
        OnWaveEnd?.Invoke();
    }
}
