using System.Collections.Generic;
using UnityEngine;
using System;

namespace Toolbox
{
    [CreateAssetMenu(fileName = "TransformGroup", menuName = "Data/Transform/Group")]
    public class TransformGroup : SO
    {
        [SerializeField] List<Transform> transforms = new();
        public List<Transform> Transforms => transforms;
        public Action OnUpdated;

        void OnDisable() => transforms = new();

        public void Add(Transform transform)
        {
            if (transforms.Contains(transform))
                return;

            transforms.Add(transform);
            OnUpdated?.Invoke();
        }

        public void Remove(Transform transform)
        {
            if (transform == null || !transforms.Contains(transform))
                return;

            transforms.Remove(transform);
            OnUpdated?.Invoke();
        }

        public Transform GetClosest(Vector3 fromPosition, float range = float.MaxValue)
        {
            if (transforms.Count == 0)
                return null;

            float closestDistance = range;
            Transform closestTransform = null;

            foreach (Transform transform in transforms)
            {
                float distance = Vector3.Distance(fromPosition, transform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTransform = transform;
                }
            }
            return closestTransform;
        }

        public Transform[] GetInRange(Vector3 fromPosition, float range)
        {
            List<Transform> transformsInRange = new();

            foreach (Transform transform in transforms)
            {
                if (Vector3.Distance(fromPosition, transform.position) <= range)
                    transformsInRange.Add(transform);
            }
            return transformsInRange.ToArray();
        }
    }
}
