using System;
using Android.Util;
using Android.OS;

namespace libsuperuser_net
{
	public class Debug
	{
		// ----- DEBUGGING -----
		#if DEBUG
		private static bool _debug = true;
		#else
		private static bool _debug = false;
		#endif

		/**
     * <p>Enable or disable debug mode</p>
     * 
     * <p>By default, debug mode is enabled for development
     * builds and disabled for exported APKs - see
     * BuildConfig.DEBUG</p>
     * 
     * @param enable Enable debug mode ?
     */	
		public static void setDebug (bool enable)
		{ 
			_debug = enable; 
		}

		/// <summary>
		/// Is debug mode enabled ?
		/// </summary>
		/// <returns><c>true</c>, if debug mode enabled</returns>
		public static bool getDebug ()
		{ 
			return _debug;
		}
	
		// ----- LOGGING -----

		public interface IOnLogListener
		{
			void OnLog (LogType type, String typeIndicator, String message);
		}

		[Flags]
		public enum LogType
		{
			None = 0,
			General = 1,
			Command = 2,
			Output = 3,
			All = 255
		}

		const string Tag = "libsuperuser";
		private static IOnLogListener logListener = null;
		private static LogType logTypes = LogType.All;


		/**
     * <p>Log a message (internal)</p>
     * 
     * <p>Current debug and enabled logtypes decide what gets logged - 
     * even if a custom callback is registered</p>  
     * 
     * @param type Type of message to log
     * @param typeIndicator String indicator for message type
     * @param message The message to log
     */
		private static void logCommon (LogType type, String typeIndicator, String message)
		{
			if (_debug) {
				if (logListener != null) {
					logListener.OnLog (type, typeIndicator, message);
				} else {
					Log.Debug (Tag, "[" + Tag + "][" + typeIndicator + "]" + (!message.StartsWith ("[") && !message.StartsWith (" ") ? " " : "") + message);
				}
			}
		}
		/**
     * <p>Log a "general" message</p>
     * 
     * <p>These messages are infrequent and mostly occur at startup/shutdown or on error</p>
     * 
     * @param message The message to log
     */
		public static void log(String message) {
			logCommon(LogType.General, "G", message);
		}

		/**
     * <p>Log a "per-command" message</p>
     * 
     * <p>This could produce a lot of output if the client runs many commands in the session</p>
     * 
     * @param message The message to log
     */
		public static void logCommand (String message)
		{
			logCommon (LogType.Command, "C", message);
		}

		/**
     * <p>Log a line of stdout/stderr output</p>
     * 
     * <p>This could produce a lot of output if the shell commands are noisy</p>
     * 
     * @param message The message to log
     */
		public static void logOutput (String message)
		{
			logCommon (LogType.Output, "O", message);
		}

		/**
     * <p>Enable or disable logging specific types of message</p>
     * 
     * <p>You may | (or) LOG_* constants together. Note that
     * debug mode must also be enabled for actual logging to
     * occur.</p>
     * 
     * @param type LOG_* constants
     * @param enable Enable or disable
     */
		public static void setLogTypeEnabled (LogType type, bool enable)
		{ 
			if (enable) {
				logTypes |= type;
			} else {
				logTypes &= ~type;
			}
		}

		/**
     * <p>Is logging for specific types of messages enabled ?</p>
     * 
     * <p>You may | (or) LOG_* constants together, to learn if
     * <b>all</b> passed message types are enabled for logging. Note
     * that debug mode must also be enabled for actual logging
     * to occur.</p>
     * 
     * @param type LOG_* constants
     */
		public static bool getLogTypeEnabled (LogType type)
		{ 
			return ((logTypes & type) == type); 
		}

		/**
     * <p>Is logging for specific types of messages enabled ?</p>
     * 
     * <p>You may | (or) LOG_* constants together, to learn if
     * <b>all</b> message types are enabled for logging. Takes
     * debug mode into account for the result.</p>
     * 
     * @param type LOG_* constants
     */
		public static bool getLogTypeEnabledEffective (LogType type)
		{
			return getDebug () && getLogTypeEnabled (type);
		}

		/**
     * <p>Register a custom log handler</p>
     * 
     * <p>Replaces the log method (write to logcat) with your own
     * handler. Whether your handler gets called is still dependent
     * on debug mode and message types being enabled for logging.</p>
     * 
     * @param onLogListener Custom log listener or NULL to revert to default
     */
		public static void setOnLogListener (IOnLogListener onLogListener)
		{
			logListener = onLogListener;
		}

		/**
     * <p>Get the currently registered custom log handler</p>
     * 
     * @return Current custom log handler or NULL if none is present 
     */
		public static IOnLogListener getOnLogListener ()
		{
			return logListener;
		}

		// ----- SANITY CHECKS -----

		private static bool sanityChecks = true;

		/**
     * <p>Enable or disable sanity checks</p>
     * 
     * <p>Enables or disables the library crashing when su is called 
     * from the main thread.</p>
     * 
     * @param enable Enable or disable
     */
		public static void setSanityChecksEnabled(bool enable) {
			sanityChecks = enable;
		}

		/**
     * <p>Are sanity checks enabled ?</p>
     * 
     * <p>Note that debug mode must also be enabled for actual
     * sanity checks to occur.</p> 
     * 
     * @return True if enabled
     */
		public static bool getSanityChecksEnabled() {
			return sanityChecks;
		}


		/**
     * <p>Are sanity checks enabled ?</p>
     * 
     * <p>Takes debug mode into account for the result.</p> 
     * 
     * @return True if enabled
     */
		public static bool getSanityChecksEnabledEffective() {
			return getDebug() && getSanityChecksEnabled();
		}

		/**
     * <p>Are we running on the main thread ?</p>
     * 
     * @return Running on main thread ?
     */	
		public static bool onMainThread() {
			return ((Looper.MyLooper() != null) && (Looper.MyLooper() == Looper.MainLooper));
		}
	}
}

