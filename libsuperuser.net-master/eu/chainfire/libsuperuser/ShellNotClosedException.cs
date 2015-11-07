using System;

namespace libsuperuser_net
{
	/**
 * Exception class used to notify developer that a shell was not close()d
 */
	public class ShellNotClosedException:Exception
	{
		public const string EXCEPTION_NOT_CLOSED = "Application did not close() interactive shell";

		public ShellNotClosedException ():base(EXCEPTION_NOT_CLOSED)
		{
		}
	}
}

