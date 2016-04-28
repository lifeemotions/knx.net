using System;

namespace KNXLib.Log
{
    /// <summary>
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public delegate void DebugEvent(string id, string message);

        /// <summary>
        /// </summary>
        public static DebugEvent DebugEventEndpoint;

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public delegate void InfoEvent(string id, string message);

        /// <summary>
        /// </summary>
        public static InfoEvent InfoEventEndpoint;

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public delegate void WarnEvent(string id, string message);

        /// <summary>
        /// </summary>
        public static WarnEvent WarnEventEndpoint;

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public delegate void ErrorEvent(string id, string message);

        /// <summary>
        /// </summary>
        public static ErrorEvent ErrorEventEndpoint;

        internal static void Debug(string id, string message, params object[] arg)
        {
            if (DebugEventEndpoint != null)
                DebugEventEndpoint(id, string.Format(message, arg));
        }

        internal static void Info(string id, string message, params object[] arg)
        {
            if (InfoEventEndpoint != null)
                InfoEventEndpoint(id, string.Format(message, arg));
        }

        internal static void Warn(string id, string message, params object[] arg)
        {
            if (WarnEventEndpoint != null)
                WarnEventEndpoint(id, string.Format(message, arg));
        }

        internal static void Error(string id, string message, params object[] arg)
        {
            if (ErrorEventEndpoint != null)
                ErrorEventEndpoint(id, string.Format(message, arg));
        }

        internal static void Error(string id, Exception e)
        {
            Error(id, e.Message);
            Error(id, e.ToString());
            Error(id, e.StackTrace);

            if (e.InnerException != null)
                Error(id, e.InnerException);
        }
    }
}
