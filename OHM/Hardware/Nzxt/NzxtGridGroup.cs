using System;
using System.Collections.Generic;

namespace OpenHardwareMonitor.Hardware.Nzxt
{
	public class NzxtGridGroup : IGroup
	{
		private List<NzxtGridDevice> _devices = new List<NzxtGridDevice>();

		public NzxtGridGroup(ISettings settings, ILog log)
		{
			var devices = NzxtGridDevice.EnumerateDevices();
			foreach (var device in devices)
			{
				_devices.Add(new NzxtGridDevice(device.DeviceID, settings, log));
			}
		}

		public IHardware[] Hardware { get { return _devices.ToArray(); } }

		public string GetReport() { return ""; }

		public void Close()
		{
			foreach (var device in _devices)
			{
				device.Close();
			}
			_devices.Clear();
		}
	}
}
