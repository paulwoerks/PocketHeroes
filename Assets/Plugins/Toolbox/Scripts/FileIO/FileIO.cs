using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

// BY  : Paul Woerner, info@paulwoerner.com
// RES : https://www.youtube.com/watch?v=XOjd_qU2Ido

/* What it does: Saves and Loads variables and classes locally to desired location.
 * What it doesn't: It cant save ScriptableObjects. If you want to save them, save the data they contain.
 * 
 * GENERIC VARIABLES need a fileName to properly get identified for loading.
 * Save: 'FileIO.Save(goldStored, "gold")'
 * Load: 'int goldStored = FileIO.Load(gold, "gold");

 * COMPLEX DATA will by default be saved by their className. You can override it though to interfere with duplicates
 * Save: 'FileIO.Save(testClass)'
 * Load: 'ExampleClass testClass = (ExampleClass)FileIO.Load(testClass)'*/

namespace Toolbox.Data
{
    /// <summary>Saves and Loads data locally.</summary>
    public class FileIO
    {
        private static readonly string subfolder = "fileIO";
        private static readonly string fileExtention = "io"; // saved file

        #region Save
        /// <summary>Saves a File locally. Example Use: 'Save(playerHealth)'</summary>
        public static void Save<T>(T data, bool debug = false) { Save(data, data.ToString(), debug); }

        /// <summary>Saves a File locally with custom fileName. Example Use: 'Save(playerHealth, "healthOfPlayer")'</summary>
        public static void Save<T>(T data, string key, bool debug = false)
        {
            string path = $"{Application.persistentDataPath}/{subfolder}";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            path += $"/{key.ToLower()}.{fileExtention}";
            FileStream stream = new(path, FileMode.Create);
            GetBinaryFormatter().Serialize(stream, data);
            stream.Close();

            if (debug)
                Debug.Log($"Save '{key}' at:   '{path}'");
        }
        #endregion

        #region Load
        /// <summary>Loads an Object locally. Example Use: 'PlayerHealth health = (PlayerHealth)Load(health)'</summary>
        // Example Use(Settings)SaveSystem.Load(class)
        public static T Load<T>(T data, bool debug = false)
        {
            return (T)Load(data, data.ToString().ToLower(), debug);
        }

        /// <summary>Loads an Object locally by fileName. Example Use: 'PlayerHealth health = (PlayerHealth)Load(health, "healthOfPlayer")'</summary>
        public static T Load<T>(T data, string key, bool debug = false)
        {
            string path = $"{Application.persistentDataPath}/{subfolder}/{key}.{fileExtention}";
            // No saved data found
            if (!File.Exists(path))
            {
                if (debug)
                    Debug.LogWarning($"[!]Load '{key}': no file found at: '{path}'. Returning input value");
                return data;
            }

            FileStream stream = new(path, FileMode.Open);
            data = (T)GetBinaryFormatter().Deserialize(stream);
            stream.Close();

            if (debug)
                Debug.Log($"Load '{key}' from: '{path}'");

            return data;
        }
        #endregion

        #region Binary Formatter
        /// <summary>Formats Data into binary text</summary>
        static BinaryFormatter GetBinaryFormatter()
        {
            BinaryFormatter formatter = new();
            SurrogateSelector selector = new();
            // Custom DataTypes
            Vector2_SerializationSurrogate vector2 = new();
            selector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), vector2);

            Vector3_SerializationSurrogate vector3 = new();
            selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), vector3);

            Quaternion_SerializationSurrogate quaternion = new();
            selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), quaternion);

            Color_SerializationSurrogate color = new();
            selector.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), color);

            formatter.SurrogateSelector = selector;

            return formatter;
        }
        #endregion
    }

    // Convert Custom Data types to binary and back.
    // Extend as needed. But don't forget to also extend the binary formatter.
    #region Surrogates

    #region Vector2
    public class Vector2_SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3 data = (Vector2)obj;
            info.AddValue("x", data.x);
            info.AddValue("y", data.y);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector3 data = (Vector2)obj;
            data.x = (float)info.GetValue("x", typeof(float));
            data.y = (float)info.GetValue("y", typeof(float));
            return data;
        }
    }
    #endregion

    #region Vector3
    public class Vector3_SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3 data = (Vector3)obj;
            info.AddValue("x", data.x);
            info.AddValue("y", data.y);
            info.AddValue("z", data.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector3 data = (Vector3)obj;
            data.x = (float)info.GetValue("x", typeof(float));
            data.y = (float)info.GetValue("y", typeof(float));
            data.z = (float)info.GetValue("z", typeof(float));
            return data;
        }
    }
    #endregion

    #region Vector4
    public class Vector4_SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector4 data = (Vector4)obj;
            info.AddValue("x", data.x);
            info.AddValue("y", data.y);
            info.AddValue("z", data.z);
            info.AddValue("w", data.w);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector4 data = (Vector4)obj;
            data.x = (float)info.GetValue("x", typeof(float));
            data.y = (float)info.GetValue("y", typeof(float));
            data.z = (float)info.GetValue("z", typeof(float));
            data.w = (float)info.GetValue("w", typeof(float));
            return data;
        }
    }
    #endregion

    #region Quaternion
    public class Quaternion_SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Quaternion data = (Quaternion)obj;
            info.AddValue("x", data.x);
            info.AddValue("y", data.y);
            info.AddValue("z", data.z);
            info.AddValue("w", data.w);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Quaternion data = (Quaternion)obj;
            data.x = (float)info.GetValue("x", typeof(float));
            data.y = (float)info.GetValue("y", typeof(float));
            data.z = (float)info.GetValue("z", typeof(float));
            data.w = (float)info.GetValue("w", typeof(float));
            return data;
        }
    }
    #endregion

    #region Color
    public class Color_SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Color data = (Color)obj;
            info.AddValue("r", data.r);
            info.AddValue("g", data.g);
            info.AddValue("b", data.b);
            info.AddValue("a", data.a);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Color data = (Color)obj;
            data.r = (float)info.GetValue("r", typeof(float));
            data.g = (float)info.GetValue("g", typeof(float));
            data.b = (float)info.GetValue("b", typeof(float));
            data.a = (float)info.GetValue("a", typeof(float));
            return data;
        }
    }
    #endregion

    #region DateTime
    public class DateTime_SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            DateTime data = (DateTime)obj;
            info.AddValue("ticks", data.Ticks);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            return new DateTime((long)info.GetValue("ticks", typeof(long)));
        }
    }
    #endregion

    #endregion
}
