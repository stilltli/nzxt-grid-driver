using System;
using System.Collections.Generic;
using OpenHardwareMonitor.Hardware;

namespace grid
{
	public class Computer : IDisposable, IComputer
	{
		private ISettings _settings;
		private ILog _log;
		private List<IGroup> _groups = new List<IGroup>();

		public Computer(ISettings settings, ILog log)
		{
			_settings = (settings != null ? settings : new Settings());
			_log = (log != null ? log : new Log());

			Add(new OpenHardwareMonitor.Hardware.ATI.ATIGroup(_settings));
			Add(new OpenHardwareMonitor.Hardware.Nvidia.NvidiaGroup(_settings));
			Add(new OpenHardwareMonitor.Hardware.Nzxt.NzxtGridGroup(_settings, _log));
		}

		~Computer()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				while (_groups.Count > 0)
				{
					IGroup group = _groups[_groups.Count - 1];
					Remove(group);
				}
			}
		}

		private void Add(IGroup group)
		{
			if (_groups.Contains(group)) return;

			_groups.Add(group);

			if (HardwareAdded != null)
			{
				foreach (IHardware hardware in group.Hardware)
					HardwareAdded(hardware);
			}
		}

		private void Remove(IGroup group)
		{
			if (!_groups.Contains(group)) return;

			_groups.Remove(group);

			if (HardwareRemoved != null)
			{
				foreach (IHardware hardware in group.Hardware)
					HardwareRemoved(hardware);
			}

			group.Close();
		}

		public void Update()
		{
			foreach (IGroup group in _groups)
			{
				foreach (IHardware hardware in group.Hardware)
					hardware.Update();
			}
		}

		//
		// IComputer
		//

		public IHardware[] Hardware
		{
			get
			{
				List<IHardware> list = new List<IHardware>();
				foreach (IGroup group in _groups)
				{
					foreach (IHardware hardware in group.Hardware)
						list.Add(hardware);
				}
				return list.ToArray();
			}
		}

		public bool MainboardEnabled { get { return false; } }
		public bool CPUEnabled { get { return false; } }
		public bool RAMEnabled { get { return false; } }
		public bool GPUEnabled { get { return true; } }
		public bool FanControllerEnabled { get { return false; } }
		public bool HDDEnabled { get { return false; } }

		public string GetReport() { return ""; }

		public event HardwareEventHandler HardwareAdded;
		public event HardwareEventHandler HardwareRemoved;

		//
		// IElement
		//

		public void Accept(IVisitor visitor)
		{
			if (visitor == null)
				throw new ArgumentNullException("visitor");
			visitor.VisitComputer(this);
		}

		public void Traverse(IVisitor visitor)
		{
			foreach (IGroup group in _groups)
				foreach (IHardware hardware in group.Hardware)
					hardware.Accept(visitor);
		}
	}

	internal class Settings : ISettings
	{
		public bool Contains(string name) { return false; }
		public void SetValue(string name, string value) { }
		public string GetValue(string name, string value) { return value; }
		public void Remove(string name) { }
	}

	internal class Log : ILog
	{
		public void WriteLine(string msg) { System.Diagnostics.Debug.WriteLine(msg); }
		public void WriteLine(string format, params object[] args) { System.Diagnostics.Debug.WriteLine(format, args); }
		public void WriteException(Exception ex) { System.Diagnostics.Debug.WriteLine(ex.ToString()); }
	}
}
