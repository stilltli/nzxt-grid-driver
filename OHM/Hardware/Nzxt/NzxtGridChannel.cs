using System;
using System.Collections.Generic;

namespace OpenHardwareMonitor.Hardware.Nzxt
{
	internal class NzxtGridChannel
	{
		private NzxtGridDevice _device;
		private int _channelId;
		private Sensor _voltage;
		private Sensor _rpm;
		private Sensor _speed;
		private Control _speedControl;

		public Sensor VoltageSens { get { return _voltage; } }
		public Sensor RpmSens { get { return _rpm; } }
		public Sensor SpeedSens { get { return _speed; } }
		public Control SpeedControl { get { return _speedControl; } }

		public NzxtGridChannel(NzxtGridDevice device, int channelId, ISettings settings)
		{
			_device = device;
			_channelId = channelId;

			_voltage = new Sensor("Fan " + GetId(), channelId, SensorType.Voltage, device, settings);
			_rpm = new Sensor("Fan " + GetId(), channelId, SensorType.Fan, device, settings);
			_speed = new Sensor("Control " + GetId(), channelId, SensorType.Control, device, settings);
			_speedControl = new Control(_speed, settings, NzxtGridDevice.MinSpeed, NzxtGridDevice.MaxSpeed);

			_speedControl.ControlModeChanged += ControlModeChanged;
			_speedControl.SoftwareControlValueChanged += SoftwareControlValueChanged;
			_speed.Control = _speedControl;
			ControlModeChanged(_speedControl);

			device.EnableSensor(_voltage);
			device.EnableSensor(_rpm);
			device.EnableSensor(_speed);
		}

		public void Close()
		{
			_speedControl.ControlModeChanged -= ControlModeChanged;
			_speedControl.SoftwareControlValueChanged -= SoftwareControlValueChanged;

			if (_speedControl.ControlMode != ControlMode.Undefined) {
				_device.SetSpeed(_channelId, NzxtGridDevice.DefSpeed);
			}
		}

		private void ControlModeChanged(IControl control)
		{
			switch (control.ControlMode)
			{
				case ControlMode.Undefined:
					return;

				case ControlMode.Default:
					_device.SetSpeed(_channelId, NzxtGridDevice.DefSpeed);
					break;

				case ControlMode.Software:
					SoftwareControlValueChanged(control);
					break;

				default:
					return;
			}
		}

		private void SoftwareControlValueChanged(IControl control)
		{
			if (control.ControlMode == ControlMode.Software)
			{
				_device.SetSpeed(_channelId, control.SoftwareValue);
			}
		}

		private string GetId() { return "#" + _channelId.ToString(); }
	}
}
