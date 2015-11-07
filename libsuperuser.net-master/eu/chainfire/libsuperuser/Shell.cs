using System;
using System.Collections.Generic;
using Java.Util;
using Java.Lang;
using System.Linq;

namespace libsuperuser_net
{
	/**
 * Class providing functionality to execute commands in a (root) shell
 */
	public class Shell
	{
		/**
     * <p>
     * Runs commands using the supplied shell, and returns the output, or null
     * in case of errors.
     * </p>
     * <p>
     * This method is deprecated and only provided for backwards compatibility.
     * Use {@link #run(String, String[], String[], boolean)} instead, and see
     * that same method for usage notes.
     * </p>
     * 
     * @param shell The shell to use for executing the commands
     * @param commands The commands to execute
     * @param wantSTDERR Return STDERR in the output ?
     * @return Output of the commands, or null in case of an error
     */
		[Obsolete]
		public static List<string> run (string shell, string[] commands, bool wantSTDERR)
		{
			return run (shell, commands, null, wantSTDERR);
		}

		/**
     * <p>
     * Runs commands using the supplied shell, and returns the output, or null
     * in case of errors.
     * </p>
     * <p>
     * Note that due to compatibility with older Android versions, wantSTDERR is
     * not implemented using redirectErrorStream, but rather appended to the
     * output. STDOUT and STDERR are thus not guaranteed to be in the correct
     * order in the output.
     * </p>
     * <p>
     * Note as well that this code will intentionally crash when run in debug
     * mode from the main thread of the application. You should always execute
     * shell commands from a background thread.
     * </p>
     * <p>
     * When in debug mode, the code will also excessively log the commands
     * passed to and the output returned from the shell.
     * </p>
     * <p>
     * Though this function uses background threads to gobble STDOUT and STDERR
     * so a deadlock does not occur if the shell produces massive output, the
     * output is still stored in a List&lt;String&gt;, and as such doing
     * something like <em>'ls -lR /'</em> will probably have you run out of
     * memory.
     * </p>
     * 
     * @param shell The shell to use for executing the commands
     * @param commands The commands to execute
     * @param environment List of all environment variables (in 'key=value'
     *            format) or null for defaults
     * @param wantSTDERR Return STDERR in the output ?
     * @return Output of the commands, or null in case of an error
     */
		public static List<string> run (string shell, string[] commands, string[] environment, bool wantSTDERR)
		{
			string shellUpper = shell.ToUpperInvariant (); //.ToUpperCase(Locale.ENGLISH);

			if (Debug.getSanityChecksEnabledEffective () && Debug.onMainThread ()) {
				// check if we're running in the main thread, and if so, crash if
				// we're in debug mode, to let the developer know attention is
				// needed here.

				Debug.log (ShellOnMainThreadException.EXCEPTION_COMMAND);
// vhe				throw new ShellOnMainThreadException (ShellOnMainThreadException.EXCEPTION_COMMAND);
			}
			Debug.logCommand (string.Format ("[{0}] START", shellUpper));

			var res = Java.Util.Collections.SynchronizedList (new System.Collections.ArrayList ());

			try {
				// Combine passed environment with system environment
				if (environment != null) {
					var newEnvironment = new Dictionary<string, string> (global::Java.Lang.JavaSystem.Getenv ());
					//int split;
					var split = new char[]{ '=' };
					foreach (string entry in environment) {
						var splat = entry.Split (split, 1, StringSplitOptions.RemoveEmptyEntries);
						if (splat != null && splat.Length == 2) {
							newEnvironment [splat [0]] = splat [1];
						}
					}
//				int i = 0;
//					environment = new String[newEnvironment.size()];
//					foreach (Map.Entry<String, String> entry in newEnvironment.entrySet()) {
//						environment[i] = entry.getKey() + "=" + entry.getValue();
//						i++;
//					}
					environment = newEnvironment.Select (x => string.Format ("{0}={1}", x.Key, x.Value)).ToArray ();

				}

				// setup our process, retrieve STDIN stream, and STDOUT/STDERR
				// gobblers
				Process process = Runtime.GetRuntime ().Exec (shell, environment);
				var STDIN = new Java.IO.DataOutputStream (process.OutputStream);
				StreamGobbler STDOUT = new StreamGobbler (shellUpper + "-", process.InputStream, res);
				StreamGobbler STDERR = new StreamGobbler (shellUpper + "*", process.ErrorStream, wantSTDERR ? res : null);

				// start gobbling and write our commands to the shell
				STDOUT.Start ();
				STDERR.Start ();
				try {
					foreach (string write in commands) {
						Debug.logCommand (string.Format ("[{0}+] {1}", shellUpper, write));
						STDIN.Write ((write + "\n").getBytes ("UTF-8"));
						STDIN.Flush ();
					}
					STDIN.Write ("exit\n".getBytes ("UTF-8"));
					STDIN.Flush ();
				} catch (System.Exception e) {
					if (e.ToString ().Contains ("EPIPE")) {
						// method most horrid to catch broken pipe, in which case we
						// do nothing. the command is not a shell, the shell closed
						// STDIN, the script already contained the exit command, etc.
						// these cases we want the output instead of returning null
					} else {
						// other issues we don't know how to handle, leads to
						// returning null
						throw;
					}
				}

				// wait for our process to finish, while we gobble away in the
				// background
				process.WaitFor ();

				// make sure our threads are done gobbling, our streams are closed,
				// and the process is destroyed - while the latter two shouldn't be
				// needed in theory, and may even produce warnings, in "normal" Java
				// they are required for guaranteed cleanup of resources, so lets be
				// safe and do this on Android as well
				try {
					STDIN.Close ();
				} catch (System.Exception e) {
					// might be closed already
				}
				STDOUT.Join ();
				STDERR.Join ();
				process.Destroy ();

				// in case of su, 255 usually indicates access denied
				if (SU.isSU (shell) && (process.ExitValue() != 255)) {  // vhe
					res = null;
				}
			} catch (Java.IO.IOException e) {
				// shell probably not found
				res = null;
			} catch (InterruptedException e) {
				// this should really be re-thrown
				res = null;
			}

			Debug.logCommand (string.Format ("[{0}%] END", shell.ToUpperInvariant ()));
			var result = new List<string> ();
			foreach (var r in res)
				result.Add (r.ToString ());
			return result;
		}

		protected static string[] availableTestCommands = new string[] {
			"echo -BOC-",
			"id"
		};

		/**
     * See if the shell is alive, and if so, check the UID
     * 
     * @param ret Standard output from running availableTestCommands
     * @param checkForRoot true if we are expecting this shell to be running as
     *            root
     * @return true on success, false on error
     */
		protected static bool parseAvailableResult (List<string> ret, bool checkForRoot)
		{
			if (ret == null)
				return false;

			// this is only one of many ways this can be done
			bool echo_seen = false;

			foreach (string line in ret) {
				if (line.Contains ("uid=")) {
					// id command is working, let's see if we are actually root
					return !checkForRoot || line.Contains ("uid=0");
				} else if (line.Contains ("-BOC-")) {
					// if we end up here, at least the su command starts some kind
					// of shell,
					// let's hope it has root privileges - no way to know without
					// additional
					// native binaries
					echo_seen = true;
				}
			}

			return echo_seen;
		}

		/**
     * This class provides utility functions to easily execute commands using SH
     */
		public static class SH
		{
			/**
         * Runs command and return output
         * 
         * @param command The command to run
         * @return Output of the command, or null in case of an error
         */
			public static List<string> run (string command)
			{
				return Shell.run ("sh", new string[] {
					command
				}, null, false);
			}

			/**
         * Runs commands and return output
         * 
         * @param commands The commands to run
         * @return Output of the commands, or null in case of an error
         */
			public static List<string> run (List<string> commands)
			{
				return Shell.run ("sh", commands.ToArray (), null, false);
			}

			/**
         * Runs commands and return output
         * 
         * @param commands The commands to run
         * @return Output of the commands, or null in case of an error
         */
			public static List<string> run (params string[] commands)
			{
				return Shell.run ("sh", commands, null, false);
			}
		}

		/**
     * This class provides utility functions to easily execute commands using SU
     * (root shell), as well as detecting whether or not root is available, and
     * if so which version.
     */
		public static class SU
		{
			private static bool? isSELinuxEnforcing = null;
			private static string[] suVersion = new string[] {
				null, null
			};

			/**
         * Runs command as root (if available) and return output
         * 
         * @param command The command to run
         * @return Output of the command, or null if root isn't available or in
         *         case of an error
         */
			public static List<string> run (string command)
			{
				return Shell.run ("su", new string[] {
					command
				}, null, false);
			}

			/**
         * Runs commands as root (if available) and return output
         * 
         * @param commands The commands to run
         * @return Output of the commands, or null if root isn't available or in
         *         case of an error
         */
			public static List<string> run (List<string> commands)
			{
				return Shell.run ("su", commands.ToArray (), null, false);
			}

			/**
         * Runs commands as root (if available) and return output
         * 
         * @param commands The commands to run
         * @return Output of the commands, or null if root isn't available or in
         *         case of an error
         */
			public static List<string> run (string[] commands)
			{
				return Shell.run ("su", commands, null, false);
			}


			/**
         * Detects whether or not superuser access is available, by checking the
         * output of the "id" command if available, checking if a shell runs at
         * all otherwise
         * 
         * @return True if superuser access available
         */
			public static bool available ()
			{
				// this is only one of many ways this can be done

				List<string> ret = run (Shell.availableTestCommands);
				return Shell.parseAvailableResult (ret, true);
			}

			/**
         * <p>
         * Detects the version of the su binary installed (if any), if supported
         * by the binary. Most binaries support two different version numbers,
         * the public version that is displayed to users, and an internal
         * version number that is used for version number comparisons. Returns
         * null if su not available or retrieving the version isn't supported.
         * </p>
         * <p>
         * Note that su binary version and GUI (APK) version can be completely
         * different.
         * </p>
         * <p>
         * This function caches its result to improve performance on multiple
         * calls
         * </p>
         * 
         * @param internalVersion Request human-readable version or application
         *            internal version
         * @return String containing the su version or null
         */
			public static string version (bool internalVersion)
			{
				int idx = internalVersion ? 0 : 1;
				if (suVersion [idx] == null) {
					string version = null;

					List<string> ret = Shell.run (
						                  internalVersion ? "su -V" : "su -v",
						                  new string[] { "exit" },
						                  null,
						                  false
					                  );

					if (ret != null) {
						foreach (var line in ret) {
							if (!internalVersion) {
								if (!line.Trim ().Equals ("")) {
									version = line;
									break;
								}
							} else {
								try {
									if (Integer.ParseInt (line) > 0) {
										version = line;
										break;
									}
								} catch (NumberFormatException e) {
									// should be parsable, try next line otherwise
								}
							}
						}
					}

					suVersion [idx] = version;
				}
				return suVersion [idx];
			}

			/**
         * Attempts to deduce if the shell command refers to a su shell
         * 
         * @param shell Shell command to run
         * @return Shell command appears to be su
         */
			public static bool isSU(string shell) {
				// Strip parameters
				int pos = shell.IndexOf(' ');
				if (pos >= 0) {
					shell = shell.Substring(0, pos);
				}

				// Strip path
				pos = shell.LastIndexOf('/');
				if (pos >= 0) {
					shell = shell.Substring(pos + 1);
				}

				return shell.Equals("su");
			}

		

		}
	}

	internal static class StringHelper
	{
		public static byte[] getBytes (this string str, string encoding)
		{
			return System.Text.Encoding.GetEncoding (encoding).GetBytes (str);
		}
	}
}

