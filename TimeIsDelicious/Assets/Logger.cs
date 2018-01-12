using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MakiMaki
{
    public class Logger : UnityEngine.Logger
    {
        private static Logger Instance = new Logger();

        private Logger() : base( UnityEngine.Debug.unityLogger )
        {
            /// Info("Logger Initialized.");
        }

        public static void Debug(object message)
        {
            // Instance.Log(LogType., message);
        }

        public static void Info(object message)
        {
            Instance.Log(LogType.Log, message);
        }

        public static void Warn(object message)
        {
            Instance.Log(LogType.Warning, message);
        }

        public static void Error(object message)
        {
            Instance.Log(LogType.Error, message);
        }

        public static void Fatal(object message)
        {
            Instance.Log(LogType.Exception, message);
        }
    }
}
