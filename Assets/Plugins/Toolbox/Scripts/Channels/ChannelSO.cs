using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    /// <summary>Base Class of SO Channels. Functions like a Radio. Objects can Subscribe / Unsubscribe to channels and send stuff</summary>
    public abstract class ChannelSO : SO
    {
        public Subscribers subscribers = new();

        ///<summary>Print Subscribers</summary>
        public virtual void PrintSubscribers()
        {
            if (!debug)
                return;

            string subs = "Invoked GameObjects: ";
            foreach (GameObject go in subscribers.GameObjects)
                subs += $"[*] '{go.name}', ";
            subs += "\nInvoked ScriptableObjects:";
            foreach (ScriptableObject so in subscribers.ScriptableObjects)
                subs += $"[*] '{so.name}', ";
            this.Log(subs, true);
        }

        public virtual void AddSubscriber(object subscriber)
        {
            if (subscriber == null)
                return;

            if (subscriber.GetType().Equals(typeof(GameObject)))
            {
                subscribers.GameObjects.Add((GameObject)subscriber);
                this.Log($"[+] '{((GameObject)subscriber).name}'", debug);
            }

            if (subscriber.GetType().Equals(typeof(ScriptableObject)))
            {
                subscribers.ScriptableObjects.Add((ScriptableObject)subscriber);
                this.Log($"[+] '{((ScriptableObject)subscriber).name}'", debug);
            }
        }
        public virtual void RemoveSubscriber(object subscriber)
        {
            if (subscriber == null)
                return;

            if (subscriber.GetType().Equals(typeof(GameObject)))
            {
                subscribers.GameObjects.Remove((GameObject)subscriber);
                this.Log($"[-] '{((GameObject)subscriber).name}'", debug);
            }

            if (subscriber.GetType().Equals(typeof(ScriptableObject)))
            {
                subscribers.ScriptableObjects.Remove((ScriptableObject)subscriber);
                this.Log($"[-] '{((ScriptableObject)subscriber).name}'", debug);
            }
        }

        [System.Serializable]
        public class Subscribers
        {
            public List<GameObject> GameObjects = new();
            public List<ScriptableObject> ScriptableObjects = new();
        }
    }
}
