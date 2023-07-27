using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Toolbox.Events
{
    /// <summary>Event with a long value</summary>
    [CreateAssetMenu(fileName = "LongChannelSO", menuName = "Channels/Long Channel", order = 5)]
    public class LongChannelSO : ChannelSO
    {
        public event Action<long> OnInvoke; // calls listeners when triggered.

        [Button("Invoke")]
        ///<summary>Trigger the event</summary>
        public void Invoke(long value)
        {
            if (OnInvoke == null)
                return; // no listeners

            PrintSubscribers();

            OnInvoke(value);
        }

        public void Subscribe(Action<long> call, object subscriber)
        {
            OnInvoke += call;
            base.AddSubscriber(subscriber);
        }

        public void Unsubscribe(Action<long> call, object subscriber)
        {
            OnInvoke -= call;
            base.RemoveSubscriber(subscriber);
        }
    }
}
