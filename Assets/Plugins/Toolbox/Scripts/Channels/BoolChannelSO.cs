using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Toolbox.Events
{
    /// <summary>Channel with an Integer value</summary>
    [CreateAssetMenu(fileName = "BoolChannelSO", menuName = "Channels/Bool Channel", order = 1)]
    public class BoolChannelSO : ChannelSO
    {
        public event Action<bool> OnInvoke; // calls listeners when triggered.

        [Button("Invoke")]
        ///<summary>Trigger the event</summary>
        public void Invoke(bool value)
        {
            if (OnInvoke == null)
                return; // no listeners

            PrintSubscribers();

            OnInvoke(value);
        }

        public void Subscribe(Action<bool> call, object subscriber)
        {
            OnInvoke += call;
            base.AddSubscriber(subscriber);
        }

        public void Unsubscribe(Action<bool> call, object subscriber)
        {
            OnInvoke -= call;
            base.RemoveSubscriber(subscriber);
        }
    }
}
