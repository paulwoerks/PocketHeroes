using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Toolbox.Events
{
    /// <summary>Channel with a Float value</summary>
    [CreateAssetMenu(fileName = "FloatChannelSO", menuName = "Channels/Float Channel", order = 3)]
    public class FloatChannelSO : ChannelSO
    {
        public event Action<float> OnInvoke; // calls listeners when triggered.

        [Button("Invoke")]
        ///<summary>Trigger the event</summary>
        public void Invoke(float value)
        {
            if (OnInvoke == null)
                return; // no listeners

            PrintSubscribers();

            OnInvoke(value);
        }

        public void Subscribe(Action<float> call, object subscriber)
        {
            OnInvoke += call;
            base.AddSubscriber(subscriber);
        }

        public void Unsubscribe(Action<float> call, object subscriber)
        {
            OnInvoke -= call;
            base.RemoveSubscriber(subscriber);
        }
    }
}
