using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Threading;

namespace OpenHardwareMonitor.Hardware.Nzxt
{
	internal class NzxtGridDevice : Hardware
	{
		public const float MinSpeed = 40;
		public const float MaxSpeed = 100;
		public const float DefSpeed = 50;

		private class Chan
		{
			public float voltage = 0;
			public float rpm = 0;
			public float speed = 0;
			public float targetSpeed = 0;
			public NzxtGridChannel grid = null;
		}

		private ILog _log;

		private int _numChannels = 6;
		private List<Chan> _channels = new List<Chan>();

		private string _portName = "";
		private int _portSpeed = 4800;
		private int _portPoll = 20;
		private int _portCmdTimeout = 100;
		private SerialPort _port = null;
		private byte[] _cmd = new byte[10];
		private byte[] _data = new byte[10];

		private int _updateRateMillis = 1000 / 2;
		private int _stopTimeout = 2000;
		private Thread _worker = null;
		private volatile bool _runFlag = false;
		private object _stateMx = new object();

		public NzxtGridDevice(string portName, ISettings settings, ILog log) :
			base("Nzxt Grid " + portName, new Identifier("grid", portName.ToLower()), settings)
		{
			_log = log;
			_portName = portName;

			for (int i = 0; i < _numChannels; ++i)
			{
				var chan = new Chan();
				chan.grid = new NzxtGridChannel(this, i, settings);
				//chan.targetSpeed = DefSpeed;
				_channels.Add(chan);
			}

			Start();
			Update();
		}

		public override HardwareType HardwareType { get { return HardwareType.SuperIO; } }
		public override string GetReport() { return ""; }
		public override void Close() { Stop(); }

		public override void Update() // called to refresh sensor values
		{
			for (int i = 0; i < _numChannels; ++i)
			{
				var chan = _channels[i];
				lock (chan)
				{
					if (chan.voltage > 0)
						chan.grid.VoltageSens.Value = chan.voltage;
					else
						chan.grid.VoltageSens.Value = null;

					if (chan.rpm > 0)
						chan.grid.RpmSens.Value = chan.rpm;
					else
						chan.grid.RpmSens.Value = null;

					if (chan.speed > 0)
						chan.grid.SpeedSens.Value = chan.speed;
					else
						chan.grid.SpeedSens.Value = null;
				}
			}
		}

		public void SetSpeed(int chanId, float speed)
		{
			var chan = _channels[chanId];
			lock (chan)
			{
				chan.targetSpeed = Clamp(speed, MinSpeed, MaxSpeed);
			}
		}

		public void EnableSensor(Sensor sensor)
		{
			ActivateSensor(sensor);
		}

		public void DisableSensor(Sensor sensor)
		{
			DeactivateSensor(sensor);
			sensor.Value = null;
		}

		//
		// Worker
		//

		private void Start()
		{
			_log.WriteLine("Start " + this.Identifier.ToString());

			lock (_stateMx)
			{
				if (_worker != null) throw new InvalidOperationException("Worker already started");

				_runFlag = true;
				_worker = new Thread(new ThreadStart(ThreadProc));
				_worker.Start();
			}
		}

		private void Stop()
		{
			_log.WriteLine("Stop " + this.Identifier.ToString());

			lock (_stateMx)
			{
				_runFlag = false;
				if (_worker != null)
				{
					try { if (!_worker.Join(_stopTimeout)) { _worker.Abort(); } }
					catch { }
					_worker = null;
				}

				ClosePort();
			}
		}

		private void ThreadProc()
		{
			_log.WriteLine("Enter ThreadProc " + this.Identifier.ToString());

			try
			{
				OpenPort();

				_cmd[0] = 0xC0;
				WriteCmd(1, 1);

				while (_runFlag)
				{
					UpdateData();
				}
			}
			catch (Exception ex)
			{
				_log.WriteException(ex);
			}

			_log.WriteLine("Exit ThreadProc " + this.Identifier.ToString());
		}

		private void UpdateData()
		{
			var t0 = Environment.TickCount;

			for (int i = 0; i < _numChannels; ++i)
			{
				var chan = _channels[i];

				float voltage = 0;
				_cmd[0] = 132; // ReadDCVoltage
				_cmd[1] = (byte)(i + 1);
				if (WriteCmd(2, 5))
				{
					voltage = (float)Math.Round((double)_data[3] + (double)(_data[4] / 16) / 10.0 + (double)(_data[4] % 16) / 100.0, 2);
				}

				float rpm = 0;
				_cmd[0] = 138; // ReadRPM
				_cmd[1] = (byte)(i + 1);
				if (WriteCmd(2, 5))
				{
					rpm = (int)_data[3] * 256 + (int)_data[4];
				}

				float speed = 0, targetSpeed = 0;
				lock (chan)
				{
					speed = chan.speed;
					targetSpeed = chan.targetSpeed;
				}

				if (Math.Abs(targetSpeed - speed) > 1.0f)
				{
					_cmd[0] = 68; // WriteVoltage
					_cmd[1] = (byte)(i + 1);
					_cmd[2] = 192;
					_cmd[3] = 0;
					_cmd[4] = 0;

					// https://github.com/Tankernn/JavaGridControl/blob/master/src/main/java/eu/tankernn/grid/model/Fan.java
					// https://github.com/akej74/grid-control/blob/master/grid-control/grid.py
					// https://github.com/CapitalF/gridfan/blob/master/gridfan

					float V = Clamp((12.0f * targetSpeed * 0.01f), 4, 12); // speed% to voltage
					float Vr = (float)Math.Floor((V * 2) + 0.5) * 0.5f; // round to nearest half

					int wantedVoltage = (int)(Vr * 100.0f);
					int firstByte = wantedVoltage / 100;
					int lastByte = (wantedVoltage - (firstByte * 100));

					_cmd[5] = (byte)firstByte;
					_cmd[6] = (byte)(lastByte < 50 ? 0x00 : 0x50);

					// cam
					//_cmd[5] = (byte)Vr;
					//_cmd[6] = (byte)((int)(Vr * 10.0) % 10 * 16 + (int)(Vr * 100.0) % 10);

					_log.WriteLine("WriteVoltage id=" + i.ToString() + " speed=" + Str(targetSpeed) + " V=" + Str(Vr) +
						" (" + _cmd[5].ToString("X2") + " " + _cmd[6].ToString("X2") + ")");

					bool ok = WriteCmd(7, 1);
					speed = (ok ? targetSpeed : 0);
				}

				lock (chan)
				{
					chan.voltage = voltage;
					chan.rpm = rpm;
					chan.speed = speed;
				}
			}

			var t1 = Environment.TickCount;

			var delta = t1 - t0;
			if (delta <= _updateRateMillis)
			{
				Thread.Sleep(_updateRateMillis - delta);
			}
		}

		private bool WriteCmd(int cmdSize, int responseSize)
		{
			_port.Write(_cmd, 0, cmdSize);

			if (responseSize <= 0) return true;

			var t0 = Environment.TickCount;
			for (;;)
			{
				if (_port.BytesToRead >= responseSize) break;
				Thread.Sleep(_portPoll);
				if (Environment.TickCount - t0 > _portCmdTimeout)
				{
					_log.WriteLine("WriteCmd timeout");
					FlushPort();
					break;
				}
			}

			int count = _port.BytesToRead;
			if (count != responseSize)
			{
				_log.WriteLine("WriteCmd invalid BytesToRead=" + count.ToString());
				FlushPort();
				return false;
			}

			int n = _port.Read(_data, 0, count);
			return (n == responseSize);
		}

		//
		// PORT
		//

		private void OpenPort()
		{
			if (_port != null) throw new InvalidOperationException("Port already opened");

			_log.WriteLine("OpenPort " + _portName);

			_port = new SerialPort(_portName, _portSpeed);
			_port.Open();

			FlushPort();
		}

		private void ClosePort()
		{
			if (_port != null)
			{
				_log.WriteLine("ClosePort " + _port.PortName);
				_port.Dispose();
				_port = null;
			}
		}

		private void FlushPort()
		{
			_port.DiscardInBuffer();
			_port.DiscardOutBuffer();
		}

		public struct DeviceInfo
		{
			public string PNPDeviceID;
			public string DeviceID;
			public string Name;
		}

		public static List<DeviceInfo> EnumerateDevices()
		{
			List<DeviceInfo> result = new List<DeviceInfo>();
			using (ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM WIN32_SerialPort"))
			{
				var list = managementObjectSearcher.Get();
				foreach (var obj in list)
				{
					if (obj["PNPDeviceID"].ToString().Contains("VID_04D8&PID_00DF&MI_00"))
					{
						var dev = new DeviceInfo();
						dev.PNPDeviceID = obj["PNPDeviceID"].ToString();
						dev.DeviceID = obj["DeviceID"].ToString();
						dev.Name = obj["Name"].ToString();
						result.Add(dev);
					}
				}
			}
			return result;
		}

		//
		// UTILS
		//

		private float Clamp(float x, float min, float max) { return (x < min ? min : (x > max ? max : x)); }

		private string Str(float x) { return x.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture); }
	}
}
