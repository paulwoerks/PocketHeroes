using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Toolbox.Events
{
    /// <summary>Channel with an Integer value</summary>
    [CreateAssetMenu(fileName = "IntChannelSO", menuName = "Channels/Int Channel", order = 2)]
    public class IntChannelSO : ChannelSO
    {
        public event Action<int> OnInvoke; // calls listeners when triggered.

        [Button("Invoke")]
        ///<summary>Trigger the event</summary>
        public void Invoke(int value)
        {
            if (OnInvoke == null)
                return; // no listeners

            PrintSubscribers();

            OnInvoke(value);
        }

        public void Subscribe(Action<int> call, object subscriber)
        {
            OnInvoke += call;
            base.AddSubscriber(subscriber);
        }

        public void Unsubscribe(Action<int> call, object subscriber)
        {
            OnInvoke -= call;
            base.RemoveSubscriber(subscriber);
        }
    }
}
