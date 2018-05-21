using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using Debug = UnityEngine.Debug;
using Object = System.Object;

namespace UnityUtilities.Management
{
    [UsedImplicitly]
    public class UnityEditorLogger : ILogger
    {
        public void UnityLog(string s, LogLevels logLevels = LogLevels.DEBUG) {
            DateTime date = DateTime.Now;
            string nicetime = date.ToString("[HH:mm:ss] ");
            switch (logLevels) {
                case LogLevels.DEBUG:
                    s = "[DEBUG] " + s;
                    Debug.Log(nicetime + s);
                    break;
                case LogLevels.INFO:
                    s = "[INFO] " + s;
                    Debug.Log(nicetime + s);
                    break;
                case LogLevels.WARNING:
                    s = "[WARNING] " + s;
                    Debug.LogWarning(nicetime + s);
                    break;
                case LogLevels.ERROR:
                    s = "[ERROR] " + s;
                    Debug.LogError(nicetime + s);
                    break;
                default:
                    UnityLog("I was given a log level that doesn't exist.", LogLevels.ERROR);
                    break;
            }
        }

        public void UnityLog(int i, LogLevels logLevels = LogLevels.DEBUG)
        {
            string s = i.ToString();
            switch (logLevels)
            {
                case LogLevels.DEBUG:
                    s = "[DEBUG]" + s;
                    Debug.Log(s);
                    break;
                case LogLevels.INFO:
                    s = "[INFO]" + s;
                    Debug.Log(s);
                    break;
                case LogLevels.WARNING:
                    s = "[WARNING]" + s;
                    Debug.LogWarning(s);
                    break;
                case LogLevels.ERROR:
                    Debug.LogError(s);
                    s = "[ERROR]" + s;
                    break;
                default:
                    s = "[DEFAULT]" + s;
                    UnityLog("I was given a log level that doesn't exist.", LogLevels.WARNING);
                    break;
            }
        }

        public static string var_dump(Object o)
        {
            StringBuilder sb = new StringBuilder();

            // Include the type of the object
            Type type = o.GetType();
            sb.Append("Type: " + type.Name);

            // Include information for each Field
            sb.Append("\r\n\r\nFields:");
            FieldInfo[] fi = type.GetFields();
            if (fi.Length > 0)
            {
                foreach (FieldInfo f in fi)
                {
                    sb.Append("\r\n " + f + " = " +
                              f.GetValue(o));
                }
            }
            else
                sb.Append("\r\n None");

            // Include information for each Property
            sb.Append("\r\n\r\nProperties:");
            PropertyInfo[] pi = type.GetProperties();
            if (pi.Length > 0)
            {
                foreach (PropertyInfo p in pi)
                {
                    sb.Append("\r\n " + p + " = " +
                              p.GetValue(o, null));
                }
            }
            else
                sb.Append("\r\n None");

            return sb.ToString();
        }

        public void Log(object o, LogLevels logLevels) {
            // var_dump this object if it hasn't overriden the ToString() method. Reflection! *makes spooky ghost noises*
            if (o is string)
                UnityLog(o.ToString(), logLevels);
            else {
                var toString = o.GetType().GetMethod("ToString", Type.EmptyTypes);
                UnityLog(toString.DeclaringType == typeof(object) ? o.ToString() : var_dump(o), logLevels);
            }
        }
    }
}