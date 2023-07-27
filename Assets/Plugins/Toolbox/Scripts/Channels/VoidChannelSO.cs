using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Toolbox.Events
{
    /// <summary>Channel with no value</summary>
    [CreateAssetMenu(fileName = "VoidChannelSO", menuName = "Channels/Void Channel", order = 0)]
    public class VoidChannelSO : ChannelSO
    {
        ///<summary>calls listeners when triggered</summary>
        public event Action OnInvoke;

        [Button("Invoke")]
        ///<summary>Trigger the event</summary>
        public void Invoke()
        {
            if (OnInvoke == null)
                return; // no listeners

            PrintSubscribers();

            OnInvoke();
        }

        public void Subscribe(Action call, object subscriber)
        {
            OnInvoke += call;
            base.AddSubscriber(subscriber);
        }

        public void Unsubscribe(Action call, object subscriber)
        {
            OnInvoke -= call;
            base.RemoveSubscriber(subscriber);
        }
    }
}
