using System;
using Java.Lang;
using Java.IO;
using System.Collections;

namespace libsuperuser_net
{
	/**
 * Thread utility class continuously reading from an InputStream
 */
	public class StreamGobbler:Thread
	{
		/**
     * Line callback interface
     */
		public interface IOnLineListener {		
			/**
         * <p>Line callback</p>
         * 
         * <p>This callback should process the line as quickly as possible.
         * Delays in this callback may pause the native process or even
         * result in a deadlock</p>
         * 
         * @param line String that was gobbled
         */
			void OnLine(string line);
		}

		private readonly string shell = null;
		private readonly BufferedReader reader = null;
		private readonly IList writer = null;
		private readonly IOnLineListener listener = null;

		/**
     * <p>StreamGobbler constructor</p>
     * 
     * <p>We use this class because shell STDOUT and STDERR should be read as quickly as 
     * possible to prevent a deadlock from occurring, or Process.waitFor() never
     * returning (as the buffer is full, pausing the native process)</p>
     * 
     * @param shell Name of the shell
     * @param inputStream InputStream to read from
     * @param outputList List<String> to write to, or null
     */
		public StreamGobbler(string shell, System.IO.Stream inputStream, IList outputList) {
			this.shell = shell;
			reader = new BufferedReader(new InputStreamReader(inputStream));
			writer = outputList;
		}
		/**
     * <p>StreamGobbler constructor</p>
     * 
     * <p>We use this class because shell STDOUT and STDERR should be read as quickly as 
     * possible to prevent a deadlock from occurring, or Process.waitFor() never
     * returning (as the buffer is full, pausing the native process)</p>
     * 
     * @param shell Name of the shell
     * @param inputStream InputStream to read from
     * @param onLineListener OnLineListener callback
     */
		public StreamGobbler(string shell, System.IO.Stream inputStream, IOnLineListener onLineListener) {
			this.shell = shell;
			reader = new BufferedReader(new InputStreamReader(inputStream));
			listener = onLineListener;
		}

		public override void Run ()
		{
			// keep reading the InputStream until it ends (or an error occurs)
			try {
				string line;
				while ((line = reader.ReadLine()) != null) {
					Debug.logOutput(string.Format("[{0}] {1}", shell, line));
					if (writer != null) writer.Add(line);
					if (listener != null) listener.OnLine(line);
				}
			} catch (IOException e) {
				// reader probably closed, expected exit condition
			}

			// make sure our stream is closed and resources will be freed
			try {
				reader.Close();
			} catch (IOException e) {
				// read already closed
			}
		}
	}
}

