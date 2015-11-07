using System;

namespace libsuperuser_net
{
	public class ShellOnMainThreadException:Exception
	{
		public const string EXCEPTION_COMMAND = "Application attempted to run a shell command from the main thread";
		public const string EXCEPTION_NOT_IDLE = "Application attempted to wait for a non-idle shell to close on the main thread";
		public const string EXCEPTION_WAIT_IDLE = "Application attempted to wait for a shell to become idle on the main thread";

		public ShellOnMainThreadException (string message):base(message)
		{
		}
	}
}

