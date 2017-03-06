using System;
namespace TShockAPI.Extensions
{
	/// <summary>
	/// Extensions for Exceptions
	/// </summary>
	public static class ExceptionExt
	{
		/// <summary>
		/// Builds a formatted string containing the messages of the given exception, and any inner exceptions it contains
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static string BuildExceptionString(this Exception ex)
		{
			string msg = ex.Message;
			Exception inner = ex.InnerException;
			while (inner != null)
			{
				msg += $"\r\n\t-> {inner.Message}";
				inner = inner.InnerException;
			}

			return msg;
		}
	}
}
