namespace grid
{
	public partial class GridForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GridForm));
			this.updateTimer = new System.Windows.Forms.Timer(this.components);
			this.gridCurve = new System.Windows.Forms.DataGridView();
			this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.comboControl = new System.Windows.Forms.ComboBox();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.viewSensors = new grid.ListViewEx();
			this.colSensorName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colSensorValue = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colSensorMin = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.colSensorMax = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.textControlHysteresis = new System.Windows.Forms.TextBox();
			this.checkControlEnabled = new System.Windows.Forms.CheckBox();
			this.btnResetController = new System.Windows.Forms.Button();
			this.btnDeleteController = new System.Windows.Forms.Button();
			this.listControllers = new System.Windows.Forms.ListBox();
			this.btnSaveController = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboSensor = new System.Windows.Forms.ComboBox();
			this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
			this.notifyMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.notifyMenuExit = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.gridCurve)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.notifyMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// updateTimer
			// 
			this.updateTimer.Interval = 500;
			this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
			// 
			// gridCurve
			// 
			this.gridCurve.AllowUserToResizeColumns = false;
			this.gridCurve.AllowUserToResizeRows = false;
			this.gridCurve.CausesValidation = false;
			this.gridCurve.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
			this.gridCurve.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.gridCurve.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.gridCurve.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2});
			this.gridCurve.Location = new System.Drawing.Point(6, 60);
			this.gridCurve.Name = "gridCurve";
			this.gridCurve.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			this.gridCurve.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.gridCurve.Size = new System.Drawing.Size(249, 199);
			this.gridCurve.TabIndex = 3;
			// 
			// Column1
			// 
			this.Column1.HeaderText = "Sensor";
			this.Column1.Name = "Column1";
			// 
			// Column2
			// 
			this.Column2.HeaderText = "Control";
			this.Column2.Name = "Column2";
			// 
			// comboControl
			// 
			this.comboControl.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboControl.FormattingEnabled = true;
			this.comboControl.Location = new System.Drawing.Point(55, 6);
			this.comboControl.Name = "comboControl";
			this.comboControl.Size = new System.Drawing.Size(200, 21);
			this.comboControl.TabIndex = 4;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(484, 462);
			this.tabControl1.TabIndex = 5;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.viewSensors);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(476, 436);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Status";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// viewSensors
			// 
			this.viewSensors.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.viewSensors.CausesValidation = false;
			this.viewSensors.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSensorName,
            this.colSensorValue,
            this.colSensorMin,
            this.colSensorMax});
			this.viewSensors.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewSensors.FullRowSelect = true;
			this.viewSensors.GridLines = true;
			this.viewSensors.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.viewSensors.HideSelection = false;
			this.viewSensors.Location = new System.Drawing.Point(3, 3);
			this.viewSensors.MultiSelect = false;
			this.viewSensors.Name = "viewSensors";
			this.viewSensors.Size = new System.Drawing.Size(470, 430);
			this.viewSensors.TabIndex = 2;
			this.viewSensors.UseCompatibleStateImageBehavior = false;
			this.viewSensors.View = System.Windows.Forms.View.Details;
			// 
			// colSensorName
			// 
			this.colSensorName.Text = "Sensor";
			this.colSensorName.Width = 200;
			// 
			// colSensorValue
			// 
			this.colSensorValue.Text = "Value";
			this.colSensorValue.Width = 80;
			// 
			// colSensorMin
			// 
			this.colSensorMin.Text = "Min";
			this.colSensorMin.Width = 80;
			// 
			// colSensorMax
			// 
			this.colSensorMax.Text = "Max";
			this.colSensorMax.Width = 80;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.label4);
			this.tabPage2.Controls.Add(this.label3);
			this.tabPage2.Controls.Add(this.textControlHysteresis);
			this.tabPage2.Controls.Add(this.checkControlEnabled);
			this.tabPage2.Controls.Add(this.btnResetController);
			this.tabPage2.Controls.Add(this.btnDeleteController);
			this.tabPage2.Controls.Add(this.listControllers);
			this.tabPage2.Controls.Add(this.btnSaveController);
			this.tabPage2.Controls.Add(this.label2);
			this.tabPage2.Controls.Add(this.label1);
			this.tabPage2.Controls.Add(this.comboSensor);
			this.tabPage2.Controls.Add(this.gridCurve);
			this.tabPage2.Controls.Add(this.comboControl);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(476, 436);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Control";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(272, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(49, 13);
			this.label4.TabIndex = 15;
			this.label4.Text = "Enabled:";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(272, 36);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(58, 13);
			this.label3.TabIndex = 14;
			this.label3.Text = "Hysteresis:";
			// 
			// textControlHysteresis
			// 
			this.textControlHysteresis.Location = new System.Drawing.Point(336, 33);
			this.textControlHysteresis.Name = "textControlHysteresis";
			this.textControlHysteresis.Size = new System.Drawing.Size(75, 20);
			this.textControlHysteresis.TabIndex = 13;
			// 
			// checkControlEnabled
			// 
			this.checkControlEnabled.AutoSize = true;
			this.checkControlEnabled.Location = new System.Drawing.Point(336, 9);
			this.checkControlEnabled.Name = "checkControlEnabled";
			this.checkControlEnabled.Size = new System.Drawing.Size(15, 14);
			this.checkControlEnabled.TabIndex = 12;
			this.checkControlEnabled.UseVisualStyleBackColor = true;
			// 
			// btnResetController
			// 
			this.btnResetController.Location = new System.Drawing.Point(99, 265);
			this.btnResetController.Name = "btnResetController";
			this.btnResetController.Size = new System.Drawing.Size(75, 23);
			this.btnResetController.TabIndex = 11;
			this.btnResetController.Text = "Reset";
			this.btnResetController.UseVisualStyleBackColor = true;
			this.btnResetController.Click += new System.EventHandler(this.btnResetController_Click);
			// 
			// btnDeleteController
			// 
			this.btnDeleteController.Location = new System.Drawing.Point(336, 265);
			this.btnDeleteController.Name = "btnDeleteController";
			this.btnDeleteController.Size = new System.Drawing.Size(75, 23);
			this.btnDeleteController.TabIndex = 10;
			this.btnDeleteController.Text = "Delete";
			this.btnDeleteController.UseVisualStyleBackColor = true;
			this.btnDeleteController.Click += new System.EventHandler(this.btnDeleteController_Click);
			// 
			// listControllers
			// 
			this.listControllers.FormattingEnabled = true;
			this.listControllers.Location = new System.Drawing.Point(261, 60);
			this.listControllers.Name = "listControllers";
			this.listControllers.Size = new System.Drawing.Size(150, 199);
			this.listControllers.TabIndex = 9;
			this.listControllers.SelectedIndexChanged += new System.EventHandler(this.listControllers_SelectedIndexChanged);
			// 
			// btnSaveController
			// 
			this.btnSaveController.Location = new System.Drawing.Point(180, 265);
			this.btnSaveController.Name = "btnSaveController";
			this.btnSaveController.Size = new System.Drawing.Size(75, 23);
			this.btnSaveController.TabIndex = 8;
			this.btnSaveController.Text = "Save";
			this.btnSaveController.UseVisualStyleBackColor = true;
			this.btnSaveController.Click += new System.EventHandler(this.btnSaveController_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(43, 13);
			this.label2.TabIndex = 7;
			this.label2.Text = "Control:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(43, 13);
			this.label1.TabIndex = 6;
			this.label1.Text = "Sensor:";
			// 
			// comboSensor
			// 
			this.comboSensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSensor.FormattingEnabled = true;
			this.comboSensor.Location = new System.Drawing.Point(55, 33);
			this.comboSensor.Name = "comboSensor";
			this.comboSensor.Size = new System.Drawing.Size(200, 21);
			this.comboSensor.TabIndex = 5;
			// 
			// notifyIcon
			// 
			this.notifyIcon.ContextMenuStrip = this.notifyMenu;
			this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			this.notifyIcon.Visible = true;
			this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
			// 
			// notifyMenu
			// 
			this.notifyMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.notifyMenuExit});
			this.notifyMenu.Name = "notifyMenu";
			this.notifyMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
			this.notifyMenu.ShowImageMargin = false;
			this.notifyMenu.Size = new System.Drawing.Size(128, 48);
			// 
			// notifyMenuExit
			// 
			this.notifyMenuExit.Name = "notifyMenuExit";
			this.notifyMenuExit.Size = new System.Drawing.Size(127, 22);
			this.notifyMenuExit.Text = "Exit";
			this.notifyMenuExit.Click += new System.EventHandler(this.notifyMenuExit_Click);
			// 
			// GridForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
			this.CausesValidation = false;
			this.ClientSize = new System.Drawing.Size(484, 462);
			this.Controls.Add(this.tabControl1);
			this.DoubleBuffered = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "GridForm";
			this.Text = "Grid";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GridForm_FormClosing);
			this.Shown += new System.EventHandler(this.GridForm_Shown);
			this.Move += new System.EventHandler(this.GridForm_Move);
			this.Resize += new System.EventHandler(this.GridForm_Resize);
			((System.ComponentModel.ISupportInitialize)(this.gridCurve)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.notifyMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Timer updateTimer;
		private ListViewEx viewSensors;
		private System.Windows.Forms.ColumnHeader colSensorName;
		private System.Windows.Forms.ColumnHeader colSensorValue;
		private System.Windows.Forms.ColumnHeader colSensorMin;
		private System.Windows.Forms.ColumnHeader colSensorMax;
		private System.Windows.Forms.DataGridView gridCurve;
		private System.Windows.Forms.ComboBox comboControl;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
		private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboSensor;
		private System.Windows.Forms.Button btnSaveController;
		private System.Windows.Forms.ListBox listControllers;
		private System.Windows.Forms.Button btnDeleteController;
		private System.Windows.Forms.Button btnResetController;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textControlHysteresis;
		private System.Windows.Forms.CheckBox checkControlEnabled;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.NotifyIcon notifyIcon;
		private System.Windows.Forms.ContextMenuStrip notifyMenu;
		private System.Windows.Forms.ToolStripMenuItem notifyMenuExit;
	}
}

