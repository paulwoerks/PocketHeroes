using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;
using Toolbox.Pooling;
using Toolbox.Events;

public class LootMagnet : MonoBehaviour
{
    [SerializeField] bool debug;
    [SerializeField] float force;
    [SerializeField] float magnetRange;
    float collectionDistance = 0.5f;

    [SerializeField] TransformGroup unmagnetized;
    [SerializeField] List<Transform> magnetized = new List<Transform>();

    [Header("Broadcasting on ...")]
    [SerializeField] IntChannelSO OnGetCollected;

    void OnEnable(){
        StartCoroutine(Scan());
    }

    void OnDisable(){
        StopAllCoroutines();
    }

    void Update(){
        Pull();
    }

    IEnumerator Scan(){
        while (true){
            Magnetize();
            Collect();
            yield return new WaitForSeconds(0.1f);
        }
    }

    void Pull(){
        foreach (Transform loot in magnetized)
            loot.position = Vector3.MoveTowards(loot.position, transform.position, force * Time.deltaTime);
    }

    void Magnetize(){
        Transform[] magnetizable = unmagnetized.GetInRange(transform.position, magnetRange);

        foreach (Transform loot in magnetizable){
            magnetized.Add(loot);
            unmagnetized.Remove(loot);
        }
    }

    void Collect(){
        List<Transform> collected = new();

        foreach (Transform loot in magnetized){
            if (Vector3.Distance(transform.position, loot.position) <= collectionDistance){
                collected.Add(loot);
                this.Log("Add Coin", debug);
                OnGetCollected.Invoke(1);

                Pooler.Despawn(loot.gameObject);
            }
        }

        foreach (Transform loot in collected){
            magnetized.Remove(loot);
        }
    }
}
