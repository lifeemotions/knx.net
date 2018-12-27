namespace KNXLib.Log
{
    using System;

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
        public static DebugEvent DebugEventEndpoint = (id, message) => { };

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public delegate void InfoEvent(string id, string message);

        /// <summary>
        /// </summary>
        public static InfoEvent InfoEventEndpoint = (id, message) => { };

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public delegate void WarnEvent(string id, string message);

        /// <summary>
        /// </summary>
        public static WarnEvent WarnEventEndpoint = (id, message) => { };

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        public delegate void ErrorEvent(string id, string message);

        /// <summary>
        /// </summary>
        public static ErrorEvent ErrorEventEndpoint = (id, message) => { };

        internal static void Debug(string id, string message, params object[] arg)
        {
            DebugEventEndpoint(id, string.Format(message, arg));
        }

        internal static void Info(string id, string message, params object[] arg)
        {
            InfoEventEndpoint(id, string.Format(message, arg));
        }

        internal static void Warn(string id, string message, params object[] arg)
        {
            WarnEventEndpoint(id, string.Format(message, arg));
        }

        internal static void Error(string id, string message, params object[] arg)
        {
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