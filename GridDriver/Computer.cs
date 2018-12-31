using System;
using System.Collections.Generic;
using OpenHardwareMonitor.Hardware;

namespace grid
{
	public class Computer : IDisposable, IComputer
	{
		private ISettings settings = new Settings();
		private List<IGroup> groups = new List<IGroup>();

		public Computer()
		{
			Add(new OpenHardwareMonitor.Hardware.ATI.ATIGroup(settings));
			Add(new OpenHardwareMonitor.Hardware.Nvidia.NvidiaGroup(settings));
			Add(new OpenHardwareMonitor.Hardware.Nzxt.NzxtGridGroup(settings));
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
				while (groups.Count > 0)
				{
					IGroup group = groups[groups.Count - 1];
					Remove(group);
				}
			}
		}

		private void Add(IGroup group)
		{
			if (groups.Contains(group)) return;

			groups.Add(group);

			if (HardwareAdded != null)
			{
				foreach (IHardware hardware in group.Hardware)
					HardwareAdded(hardware);
			}
		}

		private void Remove(IGroup group)
		{
			if (!groups.Contains(group)) return;

			groups.Remove(group);

			if (HardwareRemoved != null)
			{
				foreach (IHardware hardware in group.Hardware)
					HardwareRemoved(hardware);
			}

			group.Close();
		}

		public void Update()
		{
			foreach (IGroup group in groups)
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
				foreach (IGroup group in groups)
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
			foreach (IGroup group in groups)
				foreach (IHardware hardware in group.Hardware)
					hardware.Accept(visitor);
		}
	}

	public class Settings : ISettings
	{
		public bool Contains(string name) { return false; }
		public void SetValue(string name, string value) { }
		public string GetValue(string name, string value) { return value; }
		public void Remove(string name) { }
	}
}
