using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Windows.Forms;
using OpenHardwareMonitor.Hardware;

namespace grid
{
	public partial class GridForm : Form
	{
		public const string LogName = "Grid.log";
		private const float DeltaX = 2;
		private const float DeltaY = 1;

		private LogWriter _log;
		private Computer _computer;
		private List<ISensor> _sensors = new List<ISensor>();
		private List<IHardware> _hardware = new List<IHardware>();
		private Dictionary<string, Controller> _controllers = new Dictionary<string, Controller>();

		#region FORM

		public GridForm()
		{
			InitializeComponent();
			notifyIcon.Visible = true;
			notifyIcon.Text = this.Text;

			bool isVisible = false;
			string[] args = Environment.GetCommandLineArgs();
			foreach (var arg in args)
			{
				if (arg == "startvisible")
				{
					isVisible = true;
					break;
				}
			}

			ShowInTaskbar = isVisible;
			FormBorderStyle = isVisible ? FormBorderStyle.Sizable : FormBorderStyle.SizableToolWindow;
			WindowState = isVisible ? FormWindowState.Normal : FormWindowState.Minimized;
		}

		private void GridForm_Shown(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized) { Hide(); }
			InitApp();
		}

		private void GridForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			ShutdownApp();
		}

		private void notifyMenuExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void GridForm_Move(object sender, EventArgs e)
		{
		}

		private void GridForm_Resize(object sender, EventArgs e)
		{
			if (WindowState == FormWindowState.Minimized)
			{
				ShowInTaskbar = false;
				FormBorderStyle = FormBorderStyle.SizableToolWindow;
				Hide();
			}
		}

		private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && this.WindowState == FormWindowState.Minimized)
			{
				Show();
				WindowState = FormWindowState.Normal;
				FormBorderStyle = FormBorderStyle.Sizable;
				ShowInTaskbar = true;
			}
		}

		private void updateTimer_Tick(object sender, EventArgs e)
		{
			UpdateApp();
		}

		#endregion

		#region APP_STATE

		private void InitApp()
		{
			try
			{
				_log = new LogWriter();
				_log.Open(LogName);
				_log.WriteLine("InitApp");

				LoadAppConfig();
				_computer = new Computer(null, _log);
				InitSensors();

				updateTimer.Start();
			}
			catch (Exception ex)
			{
				_log.WriteException(ex);
				MessageBox.Show(ex.Message);
			}
		}

		private void ShutdownApp()
		{
			_log.WriteLine("ShutdownApp");

			try
			{
				updateTimer.Stop();

				SetControllersToDefault();
				System.Threading.Thread.Sleep(100);

				ResetSensors();
			}
			catch (Exception ex)
			{
				_log.WriteException(ex);
			}

			if (_computer != null)
			{
				_computer.Dispose();
				_computer = null;
			}

			if (_log != null)
			{
				_log.Dispose();
				_log = null;
			}
		}

		private void SaveAppConfig()
		{
			try
			{
				var config = new AppConfig();
				config.Controllers = new List<Controller>();

				foreach (var pair in _controllers)
				{
					config.Controllers.Add(pair.Value);
				}

				var json = Serialize(config);
				File.WriteAllText("config.json", json);
			}
			catch (Exception ex)
			{
				_log.WriteException(ex);
			}
		}

		private void LoadAppConfig()
		{
			listControllers.Items.Clear();
			_controllers = new Dictionary<string, Controller>();

			try
			{
				var json = File.ReadAllText("config.json");
				var config = Deserialize<AppConfig>(json);

				foreach (var curve in config.Controllers)
				{
					InitController(curve);
				}
			}
			catch (FileNotFoundException) { /* IGNORE */ }
			catch (Exception ex) { _log.WriteException(ex); }
		}

		#endregion

		#region UPDATE

		private void UpdateApp()
		{
			try
			{
				foreach (var hardware in _hardware)
				{
					hardware.Update();
				}

				foreach (var pair in _controllers)
				{
					UpdateController(pair.Value);
				}

				if (Visible)
				{
					UpdateSensorView();
				}
			}
			catch (Exception ex)
			{
				updateTimer.Stop();
				_log.WriteException(ex);
				MessageBox.Show(ex.Message);
			}
		}

		private Color cDisabledColor = SystemColors.Window;
		private Color cNotFoundColor = Color.LightGray;
		private Color cIdleColor = Color.PaleTurquoise;
		private Color cUpdatedColor = Color.LightGreen;
		private Color cErrorColor = Color.Red;

		private void UpdateController(Controller controller)
		{
			var sensorControl = FindSensor(controller.ControlUid);
			var sensor = FindSensor(controller.SensorUid);
			var item = FindSensorItem(sensorControl);

			if (sensorControl == null || sensor == null || item == null)
			{
				if (item != null)
				{
					_log.WriteLine("Invalid config: Control={0} Sensor={1}", controller.ControlUid, controller.SensorUid);
					item.BackColor = cNotFoundColor;
				}
				return;
			}

			if (!controller.Enabled)
			{
				item.BackColor = cDisabledColor;
				return;
			}

			int t = Environment.TickCount;
			float x = (float)sensor.Value;
			float dx = x - controller.UpdX;

			if (Math.Abs(dx) <= DeltaX) // micro-fluctuation
			{
				item.BackColor = cIdleColor;
				return;
			}

			if (dx < -DeltaX) // fall
			{
				float hdx = x - controller.HystX;
				if (Math.Abs(hdx) > DeltaX)
				{
					controller.HystX = x;
					controller.HystT = t;
				}

				float h = (t - controller.HystT) * 0.001f;
				if (h < controller.Hysteresis)
				{
					item.BackColor = cIdleColor;
					return;
				}
			}

			float y = 0;
			if (!Interpolate(x, controller.Curve, out y))
			{
				_log.WriteLine("Interpolate failed: X={0}", x);
				sensorControl.Control.SetDefault();
				item.BackColor = cErrorColor;
				return;
			}

			if (y < sensorControl.Control.MinSoftwareValue || y > sensorControl.Control.MaxSoftwareValue)
			{
				_log.WriteLine("Interpolate failed: X={0} Y={0}", x, y);
				sensorControl.Control.SetDefault();
				item.BackColor = cErrorColor;
				return;
			}

			float dy = y - controller.UpdY;
			float dt = (t - controller.UpdT) * 0.001f;
			float hdt = (t - controller.HystT) * 0.001f;

			controller.UpdX = x;
			controller.UpdY = y;
			controller.UpdT = t;

			controller.HystX = x;
			controller.HystT = t;

			if (Math.Abs(dy) <= DeltaY)
			{
				item.BackColor = cIdleColor;
				return;
			}

			_log.WriteLine("SetSpeed {0} x={1} y={2} dx={3} dy={4} dt={5} hdt={6}",
				controller.ControlUid, Str(x), Str(y), Str(dx), Str(dy), Str(dt), Str(hdt));

			sensorControl.Control.SetSoftware(y);
			item.BackColor = cUpdatedColor;
		}

		#endregion

		#region SENSORS

		private void InitSensors()
		{
			ResetSensors();

			var hardware = _computer.Hardware;
			foreach (var device in hardware)
			{
				foreach (var sensor in device.Sensors)
				{
					_sensors.Add(sensor);

					if (!_hardware.Contains(sensor.Hardware))
					{
						_hardware.Add(sensor.Hardware);
					}

					if (sensor.SensorType == SensorType.Control)
					{
						comboControl.Items.Add(sensor.Identifier.ToString());
					}
					else
					{
						comboSensor.Items.Add(sensor.Identifier.ToString());
					}
				}

				FillSensorView(device.Sensors, SensorType.Voltage);
				FillSensorView(device.Sensors, SensorType.Temperature);
				FillSensorView(device.Sensors, SensorType.Fan);
				FillSensorView(device.Sensors, SensorType.Control);
			}
		}

		private void FillSensorView(ISensor[] sensors, SensorType type)
		{
			string[] arr = new string[viewSensors.Columns.Count];
			foreach (var sensor in sensors)
			{
				if (type == sensor.SensorType)
				{
					arr[0] = sensor.Identifier.ToString();
					var item = new ListViewItem(arr);
					item.Tag = sensor;
					viewSensors.Items.Add(item);
				}
			}
		}

		private void UpdateSensorView()
		{
			viewSensors.BeginUpdate();
			{
				foreach (ListViewItem item in viewSensors.Items)
				{
					var sensor = item.Tag as ISensor;
					if (sensor != null)
					{
						item.SubItems[1].Text = Str(sensor.Value);
						item.SubItems[2].Text = Str(sensor.Min);
						item.SubItems[3].Text = Str(sensor.Max);
					}
				}
			}
			viewSensors.EndUpdate();
		}

		private ListViewItem FindSensorItem(ISensor sensor)
		{
			foreach (ListViewItem item in viewSensors.Items)
			{
				if (item.Tag == sensor) return item;
			}
			return null;
		}

		private void ResetSensors()
		{
			_sensors.Clear();
			_hardware.Clear();
			viewSensors.Items.Clear();
			comboSensor.Items.Clear();
			comboControl.Items.Clear();
		}

		#endregion

		#region CONTROLLERS

		private void InitController(Controller controller)
		{
			if (!controller.Enabled)
			{
				SetSensorControlToDefault(controller.ControlUid);
			}

			controller.Curve.Sort((a, b) => a.X.CompareTo(b.X));

			_controllers[controller.ControlUid] = controller;

			if (!listControllers.Items.Contains(controller.ControlUid))
			{
				listControllers.Items.Add(controller.ControlUid);
			}
		}

		private void DeleteController(string uid)
		{
			SetSensorControlToDefault(uid);

			var controller = FindController(uid);
			if (controller != null)
			{
				_controllers.Remove(uid);
			}

			listControllers.Items.Remove(uid);
		}

		private void SetSensorControlToDefault(string uid)
		{
			var sensorControl = FindSensor(uid);
			if (sensorControl != null && sensorControl.Control != null)
			{
				sensorControl.Control.SetDefault();
			}
		}

		private void SetControllersToDefault()
		{
			foreach (var pair in _controllers)
			{
				if (pair.Value.Enabled)
				{
					SetSensorControlToDefault(pair.Key);
				}
			}
		}

		private void ShowControllerConfig(string uid)
		{
			var controller = FindController(uid);
			if (controller != null)
			{
				AddSelectItem(comboControl, controller.ControlUid);
				AddSelectItem(comboSensor, controller.SensorUid);

				gridCurve.Rows.Clear();
				foreach (var pt in controller.Curve)
				{
					gridCurve.Rows.Add(new object[] { pt.X.ToString(), pt.Y.ToString() });
				}

				checkControlEnabled.Checked = controller.Enabled;
				textControlHysteresis.Text = Str(controller.Hysteresis);
			}
			else
			{
				ResetControllerConfig();
			}
		}

		private void ResetControllerConfig()
		{
			comboControl.SelectedIndex = -1;
			comboSensor.SelectedIndex = -1;
			checkControlEnabled.Checked = false;
			textControlHysteresis.Text = "";
			gridCurve.Rows.Clear();
		}

		private void listControllers_SelectedIndexChanged(object sender, EventArgs e)
		{
			ShowControllerConfig(listControllers.SelectedItem as string);
		}

		private void btnSaveController_Click(object sender, EventArgs e)
		{
			var control = FindSensor(comboControl.SelectedItem as string);
			var sensor = FindSensor(comboSensor.SelectedItem as string);
			var curve = ParseCurve(gridCurve);

			if (control != null && control.SensorType == SensorType.Control
				&& sensor != null && sensor.SensorType != SensorType.Control)
			{
				var controller = new Controller();
				controller.ControlUid = control.Identifier.ToString();
				controller.SensorUid = sensor.Identifier.ToString();
				controller.Curve = curve;
				controller.Enabled = checkControlEnabled.Checked;
				ParseF(textControlHysteresis.Text, out controller.Hysteresis);

				InitController(controller);
				listControllers.SelectedItem = controller.ControlUid;
				SaveAppConfig();
			}
		}

		private List<Point> ParseCurve(DataGridView grid)
		{
			var points = new List<Point>();
			for (int rowId = 0; rowId < grid.RowCount; ++rowId)
			{
				var row = grid.Rows[rowId];
				float x = 0, y = 0;

				if (ParseF((string)row.Cells[0].Value, out x)
					&& ParseF((string)row.Cells[1].Value, out y))
				{
					points.Add(new Point(x, y));
				}
			}
			return points;
		}

		private void btnDeleteController_Click(object sender, EventArgs e)
		{
			DeleteController(listControllers.SelectedItem as string);
			listControllers.SelectedIndex = -1;
		}

		private void btnResetController_Click(object sender, EventArgs e)
		{
			ResetControllerConfig();
			listControllers.SelectedIndex = -1;
		}

		#endregion

		#region UTILS

		private string Serialize<T>(T obj)
		{
			var stream = new System.IO.MemoryStream();
			var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
			serializer.WriteObject(stream, obj);
			byte[] json = stream.ToArray();
			stream.Close();
			return System.Text.Encoding.UTF8.GetString(json, 0, json.Length);
		}

		private T Deserialize<T>(string json)
		{
			var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(json));
			var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(T));
			var obj = (T)serializer.ReadObject(stream);
			stream.Close();
			return obj;
		}

		private bool ParseF(string str, out float result)
		{
			return float.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
		}

		private string Str(float x)
		{
			return x.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
		}

		private string Str(float? x)
		{
			if (x != null)
				return Str((float)x);
			else
				return "";
		}

		private ISensor FindSensor(string uid)
		{
			foreach (var sensor in _sensors)
			{
				if (sensor.Identifier.ToString() == uid)
				{
					return sensor;
				}
			}
			return null;
		}

		private Controller FindController(string uid)
		{
			return ((!string.IsNullOrEmpty(uid) && _controllers.ContainsKey(uid)) ? _controllers[uid] : null);
		}

		// curve points must be sorted by X values
		private bool Interpolate(float x, List<Point> curve, out float result)
		{
			if (curve.Count < 1) { result = 0; return false; }
			if (curve.Count == 1) { result = curve[0].Y; return true; }

			if (x <= curve[0].X) { result = curve[0].Y; return true; }
			if (x >= curve[curve.Count - 1].X) { result = curve[curve.Count - 1].Y; return true; }

			for (int i = 0; i + 1 < curve.Count; ++i)
			{
				if (x >= curve[i].X && x < curve[i + 1].X)
				{
					float scale = ((x - curve[i].X) / (curve[i + 1].X - curve[i].X));
					result = curve[i].Y + (curve[i + 1].Y - curve[i].Y) * scale;
					return true;
				}
			}

			result = 0;
			return false;
		}

		private void AddSelectItem(ComboBox combo, string item)
		{
			if (!combo.Items.Contains(item)) { combo.Items.Add(item); }
			combo.SelectedItem = item;
		}

		#endregion

		#region CLASSES

		internal class LogWriter : IDisposable, ILog
		{
			private StreamWriter _writer;

			public LogWriter() { }
			~LogWriter() { Dispose(false); }
			public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
			protected virtual void Dispose(bool disposing) { if (disposing) { Close(); } }

			public void Open(string filename) { _writer = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Read)); }
			public void Close() { if (_writer != null) { _writer.Dispose(); _writer = null; } }

			public void WriteLine(string msg)
			{
				var line = GetTimestamp() + " # " + msg;
				System.Diagnostics.Debug.WriteLine(line);
				_writer.WriteLine(line);
			}

			public void WriteLine(string format, params object[] args)
			{
				WriteLine(String.Format(format, args));
			}

			public void WriteException(Exception ex)
			{
				WriteLine(ex.ToString());

				using (var exlog = new StreamWriter(new FileStream("ex.log", FileMode.Append, FileAccess.Write, FileShare.Read)))
				{
					exlog.WriteLine(GetTimestamp());
					exlog.WriteLine(ex.ToString());
					exlog.WriteLine();
				}
			}

			private string GetTimestamp() { return DateTime.Now.ToString("yyyyMMdd_HHmmss_ffff"); }
		}

		[DataContract]
		public class Point
		{
			[DataMember]
			public float X; // input

			[DataMember]
			public float Y; // output

			public Point(float x, float y) { X = x; Y = y; }
		}

		[DataContract]
		public class Controller
		{
			[DataMember]
			public string ControlUid;

			[DataMember]
			public string SensorUid;

			[DataMember]
			public List<Point> Curve;

			[DataMember]
			public bool Enabled;

			[DataMember]
			public float Hysteresis;

			public float UpdX;
			public float UpdY;
			public int UpdT;

			public float HystX;
			public int HystT;
		}

		[DataContract]
		public class AppConfig
		{
			[DataMember]
			public List<Controller> Controllers;
		}

		#endregion
	}
}
