using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Toolbox.Events
{
    /// <summary>Event with a double value</summary>
    [CreateAssetMenu(fileName = "DoubleChannelSO", menuName = "Channels/Double Channel", order = 4)]
    public class DoubleChannelSO : ChannelSO
    {
        public event Action<double> OnInvoke; // calls listeners when triggered.

        [Button("Invoke")]
        ///<summary>Trigger the event</summary>
        public void Invoke(double value)
        {
            if (OnInvoke == null)
                return; // no listeners

            PrintSubscribers();

            OnInvoke(value);
        }

        public void Subscribe(Action<double> call, object subscriber)
        {
            OnInvoke += call;
            base.AddSubscriber(subscriber);
        }

        public void Unsubscribe(Action<double> call, object subscriber)
        {
            OnInvoke -= call;
            base.RemoveSubscriber(subscriber);
        }
    }
}
