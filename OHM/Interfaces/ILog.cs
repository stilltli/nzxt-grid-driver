using System;

namespace OpenHardwareMonitor.Hardware
{
	public interface ILog
	{
		void WriteLine(string msg);
		void WriteLine(string format, params object[] args);
		void WriteException(Exception ex);
	}
}
