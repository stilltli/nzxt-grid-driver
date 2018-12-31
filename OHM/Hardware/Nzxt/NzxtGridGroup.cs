using System;
using System.Collections.Generic;

namespace OpenHardwareMonitor.Hardware.Nzxt
{
	public class NzxtGridGroup : IGroup
	{
		private List<NzxtGridDevice> _drivers = new List<NzxtGridDevice>();

		public NzxtGridGroup(ISettings settings)
		{
			var devices = NzxtGridDevice.EnumerateDevices();
			foreach (var device in devices)
			{
				_drivers.Add(new NzxtGridDevice(device.DeviceID, settings));
			}
		}

		public IHardware[] Hardware { get { return _drivers.ToArray(); } }

		public string GetReport() { return ""; }

		public void Close()
		{
			foreach (var driver in _drivers)
			{
				driver.Close();
			}
			_drivers.Clear();
		}
	}
}
