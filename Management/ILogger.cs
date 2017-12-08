namespace UnityUtilities.Management {
    public enum LogLevels {
        DEBUG,
        INFO,
        WARNING,
        ERROR
    }

    public interface ILogger {
        void Log(object o, LogLevels logLevels = LogLevels.DEBUG);
    }
}
