using UnityEngine;
using Toolbox;

public class TransformGroupMember : MonoBehaviour
{
    ///<summary>Adds and removes itself as a Member of a transform group on Enable/Disable</summary>///
    [SerializeField] TransformGroup group;

    void OnEnable(){
        group?.Add(transform);
    }

        void OnDisable(){
        group?.Remove(transform);
    }
}
