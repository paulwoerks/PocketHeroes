using UnityEngine;
using Toolbox;

public class Enemy : MonoBehaviour
{
    [SerializeField] TransformGroup enemies;

    void OnEnable(){
        enemies.Add(transform);
    }

        void OnDisable(){
        enemies.Remove(transform);
    }
}
