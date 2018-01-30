﻿namespace TestHBIL
{
	partial class TestHBILForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if ( disposing && (components != null) )
			{
				components.Dispose();
				m_device.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestHBILForm));
			this.buttonReload = new System.Windows.Forms.Button();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.floatTrackbarControlEnvironmentIntensity = new Nuaj.Cirrus.Utility.FloatTrackbarControl();
			this.floatTrackbarControlExposure = new Nuaj.Cirrus.Utility.FloatTrackbarControl();
			this.label5 = new System.Windows.Forms.Label();
			this.floatTrackbarControlAlbedo = new Nuaj.Cirrus.Utility.FloatTrackbarControl();
			this.label6 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.checkBoxAnimate = new System.Windows.Forms.CheckBox();
			this.checkBoxPause = new System.Windows.Forms.CheckBox();
			this.buttonClear = new System.Windows.Forms.Button();
			this.checkBoxEnableHBIL = new System.Windows.Forms.CheckBox();
			this.checkBoxEnableBentNormal = new System.Windows.Forms.CheckBox();
			this.checkBoxEnableConeVisibility = new System.Windows.Forms.CheckBox();
			this.checkBoxForceAlbedo = new System.Windows.Forms.CheckBox();
			this.textBoxInfo = new System.Windows.Forms.TextBox();
			this.panelOutput = new TestHBIL.PanelOutput(this.components);
			this.checkBoxMonochrome = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.floatTrackbarControlConeAngleBias = new Nuaj.Cirrus.Utility.FloatTrackbarControl();
			this.SuspendLayout();
			// 
			// buttonReload
			// 
			this.buttonReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonReload.Location = new System.Drawing.Point(1552, 709);
			this.buttonReload.Name = "buttonReload";
			this.buttonReload.Size = new System.Drawing.Size(75, 23);
			this.buttonReload.TabIndex = 2;
			this.buttonReload.Text = "Reload";
			this.buttonReload.UseVisualStyleBackColor = true;
			this.buttonReload.Click += new System.EventHandler(this.buttonReload_Click);
			// 
			// timer
			// 
			this.timer.Enabled = true;
			this.timer.Interval = 10;
			// 
			// floatTrackbarControlEnvironmentIntensity
			// 
			this.floatTrackbarControlEnvironmentIntensity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.floatTrackbarControlEnvironmentIntensity.Location = new System.Drawing.Point(1427, 657);
			this.floatTrackbarControlEnvironmentIntensity.MaximumSize = new System.Drawing.Size(10000, 20);
			this.floatTrackbarControlEnvironmentIntensity.MinimumSize = new System.Drawing.Size(70, 20);
			this.floatTrackbarControlEnvironmentIntensity.Name = "floatTrackbarControlEnvironmentIntensity";
			this.floatTrackbarControlEnvironmentIntensity.RangeMax = 100F;
			this.floatTrackbarControlEnvironmentIntensity.RangeMin = 0F;
			this.floatTrackbarControlEnvironmentIntensity.Size = new System.Drawing.Size(200, 20);
			this.floatTrackbarControlEnvironmentIntensity.TabIndex = 1;
			this.floatTrackbarControlEnvironmentIntensity.Value = 1F;
			this.floatTrackbarControlEnvironmentIntensity.VisibleRangeMax = 1F;
			// 
			// floatTrackbarControlExposure
			// 
			this.floatTrackbarControlExposure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.floatTrackbarControlExposure.Location = new System.Drawing.Point(1427, 683);
			this.floatTrackbarControlExposure.MaximumSize = new System.Drawing.Size(10000, 20);
			this.floatTrackbarControlExposure.MinimumSize = new System.Drawing.Size(70, 20);
			this.floatTrackbarControlExposure.Name = "floatTrackbarControlExposure";
			this.floatTrackbarControlExposure.RangeMax = 1000F;
			this.floatTrackbarControlExposure.RangeMin = 0F;
			this.floatTrackbarControlExposure.Size = new System.Drawing.Size(200, 20);
			this.floatTrackbarControlExposure.TabIndex = 1;
			this.floatTrackbarControlExposure.Value = 1F;
			this.floatTrackbarControlExposure.VisibleRangeMax = 1F;
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(1306, 687);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(51, 13);
			this.label5.TabIndex = 3;
			this.label5.Text = "Exposure";
			// 
			// floatTrackbarControlAlbedo
			// 
			this.floatTrackbarControlAlbedo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.floatTrackbarControlAlbedo.Enabled = false;
			this.floatTrackbarControlAlbedo.Location = new System.Drawing.Point(1427, 631);
			this.floatTrackbarControlAlbedo.MaximumSize = new System.Drawing.Size(10000, 20);
			this.floatTrackbarControlAlbedo.MinimumSize = new System.Drawing.Size(70, 20);
			this.floatTrackbarControlAlbedo.Name = "floatTrackbarControlAlbedo";
			this.floatTrackbarControlAlbedo.RangeMax = 1F;
			this.floatTrackbarControlAlbedo.RangeMin = 0F;
			this.floatTrackbarControlAlbedo.Size = new System.Drawing.Size(200, 20);
			this.floatTrackbarControlAlbedo.TabIndex = 1;
			this.floatTrackbarControlAlbedo.Value = 0.5F;
			this.floatTrackbarControlAlbedo.VisibleRangeMax = 1F;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(1306, 661);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(108, 13);
			this.label6.TabIndex = 3;
			this.label6.Text = "Environment Intensity";
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel1.BackgroundImage")));
			this.panel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panel1.Location = new System.Drawing.Point(1441, 207);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(186, 12);
			this.panel1.TabIndex = 5;
			// 
			// checkBoxAnimate
			// 
			this.checkBoxAnimate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxAnimate.AutoSize = true;
			this.checkBoxAnimate.Location = new System.Drawing.Point(1309, 713);
			this.checkBoxAnimate.Name = "checkBoxAnimate";
			this.checkBoxAnimate.Size = new System.Drawing.Size(64, 17);
			this.checkBoxAnimate.TabIndex = 4;
			this.checkBoxAnimate.Text = "Animate";
			this.checkBoxAnimate.UseVisualStyleBackColor = true;
			// 
			// checkBoxPause
			// 
			this.checkBoxPause.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxPause.AutoSize = true;
			this.checkBoxPause.Location = new System.Drawing.Point(1379, 713);
			this.checkBoxPause.Name = "checkBoxPause";
			this.checkBoxPause.Size = new System.Drawing.Size(56, 17);
			this.checkBoxPause.TabIndex = 4;
			this.checkBoxPause.Text = "Pause";
			this.checkBoxPause.UseVisualStyleBackColor = true;
			this.checkBoxPause.CheckedChanged += new System.EventHandler(this.checkBoxPause_CheckedChanged);
			// 
			// buttonClear
			// 
			this.buttonClear.Location = new System.Drawing.Point(1441, 709);
			this.buttonClear.Name = "buttonClear";
			this.buttonClear.Size = new System.Drawing.Size(75, 23);
			this.buttonClear.TabIndex = 6;
			this.buttonClear.Text = "Clear";
			this.buttonClear.UseVisualStyleBackColor = true;
			this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
			// 
			// checkBoxEnableHBIL
			// 
			this.checkBoxEnableHBIL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxEnableHBIL.AutoSize = true;
			this.checkBoxEnableHBIL.Checked = true;
			this.checkBoxEnableHBIL.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxEnableHBIL.Location = new System.Drawing.Point(1309, 526);
			this.checkBoxEnableHBIL.Name = "checkBoxEnableHBIL";
			this.checkBoxEnableHBIL.Size = new System.Drawing.Size(50, 17);
			this.checkBoxEnableHBIL.TabIndex = 7;
			this.checkBoxEnableHBIL.Text = "HBIL";
			this.checkBoxEnableHBIL.UseVisualStyleBackColor = true;
			// 
			// checkBoxEnableBentNormal
			// 
			this.checkBoxEnableBentNormal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxEnableBentNormal.AutoSize = true;
			this.checkBoxEnableBentNormal.Checked = true;
			this.checkBoxEnableBentNormal.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxEnableBentNormal.Location = new System.Drawing.Point(1379, 526);
			this.checkBoxEnableBentNormal.Name = "checkBoxEnableBentNormal";
			this.checkBoxEnableBentNormal.Size = new System.Drawing.Size(106, 17);
			this.checkBoxEnableBentNormal.TabIndex = 8;
			this.checkBoxEnableBentNormal.Text = "Use Bent Normal";
			this.checkBoxEnableBentNormal.UseVisualStyleBackColor = true;
			// 
			// checkBoxEnableConeVisibility
			// 
			this.checkBoxEnableConeVisibility.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxEnableConeVisibility.AutoSize = true;
			this.checkBoxEnableConeVisibility.Checked = true;
			this.checkBoxEnableConeVisibility.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxEnableConeVisibility.Location = new System.Drawing.Point(1491, 526);
			this.checkBoxEnableConeVisibility.Name = "checkBoxEnableConeVisibility";
			this.checkBoxEnableConeVisibility.Size = new System.Drawing.Size(108, 17);
			this.checkBoxEnableConeVisibility.TabIndex = 8;
			this.checkBoxEnableConeVisibility.Text = "Use Cone Angles";
			this.checkBoxEnableConeVisibility.UseVisualStyleBackColor = true;
			// 
			// checkBoxForceAlbedo
			// 
			this.checkBoxForceAlbedo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxForceAlbedo.AutoSize = true;
			this.checkBoxForceAlbedo.Location = new System.Drawing.Point(1309, 634);
			this.checkBoxForceAlbedo.Name = "checkBoxForceAlbedo";
			this.checkBoxForceAlbedo.Size = new System.Drawing.Size(89, 17);
			this.checkBoxForceAlbedo.TabIndex = 4;
			this.checkBoxForceAlbedo.Text = "Force Albedo";
			this.checkBoxForceAlbedo.UseVisualStyleBackColor = true;
			this.checkBoxForceAlbedo.CheckedChanged += new System.EventHandler(this.checkBoxForceAlbedo_CheckedChanged);
			// 
			// textBoxInfo
			// 
			this.textBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxInfo.Location = new System.Drawing.Point(1298, 12);
			this.textBoxInfo.Multiline = true;
			this.textBoxInfo.Name = "textBoxInfo";
			this.textBoxInfo.ReadOnly = true;
			this.textBoxInfo.Size = new System.Drawing.Size(329, 267);
			this.textBoxInfo.TabIndex = 9;
			// 
			// panelOutput
			// 
			this.panelOutput.Location = new System.Drawing.Point(12, 12);
			this.panelOutput.Name = "panelOutput";
			this.panelOutput.Size = new System.Drawing.Size(1280, 720);
			this.panelOutput.TabIndex = 0;
			this.panelOutput.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelOutput_MouseDown);
			this.panelOutput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelOutput_MouseMove);
			this.panelOutput.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelOutput_MouseUp);
			// 
			// checkBoxMonochrome
			// 
			this.checkBoxMonochrome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBoxMonochrome.AutoSize = true;
			this.checkBoxMonochrome.Checked = true;
			this.checkBoxMonochrome.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxMonochrome.Location = new System.Drawing.Point(1309, 609);
			this.checkBoxMonochrome.Name = "checkBoxMonochrome";
			this.checkBoxMonochrome.Size = new System.Drawing.Size(118, 17);
			this.checkBoxMonochrome.TabIndex = 4;
			this.checkBoxMonochrome.Text = "Force Monochrome";
			this.checkBoxMonochrome.UseVisualStyleBackColor = true;
			this.checkBoxMonochrome.CheckedChanged += new System.EventHandler(this.checkBoxForceAlbedo_CheckedChanged);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(1306, 553);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 13);
			this.label1.TabIndex = 3;
			this.label1.Text = "Cone Angle Bias";
			// 
			// floatTrackbarControlConeAngleBias
			// 
			this.floatTrackbarControlConeAngleBias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.floatTrackbarControlConeAngleBias.Location = new System.Drawing.Point(1427, 549);
			this.floatTrackbarControlConeAngleBias.MaximumSize = new System.Drawing.Size(10000, 20);
			this.floatTrackbarControlConeAngleBias.MinimumSize = new System.Drawing.Size(70, 20);
			this.floatTrackbarControlConeAngleBias.Name = "floatTrackbarControlConeAngleBias";
			this.floatTrackbarControlConeAngleBias.RangeMax = 1F;
			this.floatTrackbarControlConeAngleBias.RangeMin = -1F;
			this.floatTrackbarControlConeAngleBias.Size = new System.Drawing.Size(200, 20);
			this.floatTrackbarControlConeAngleBias.TabIndex = 1;
			this.floatTrackbarControlConeAngleBias.Value = -0.2F;
			this.floatTrackbarControlConeAngleBias.VisibleRangeMax = 1F;
			this.floatTrackbarControlConeAngleBias.VisibleRangeMin = -1F;
			// 
			// TestHBILForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1639, 744);
			this.Controls.Add(this.textBoxInfo);
			this.Controls.Add(this.floatTrackbarControlConeAngleBias);
			this.Controls.Add(this.floatTrackbarControlEnvironmentIntensity);
			this.Controls.Add(this.checkBoxEnableConeVisibility);
			this.Controls.Add(this.checkBoxEnableBentNormal);
			this.Controls.Add(this.checkBoxEnableHBIL);
			this.Controls.Add(this.buttonClear);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.buttonReload);
			this.Controls.Add(this.floatTrackbarControlAlbedo);
			this.Controls.Add(this.floatTrackbarControlExposure);
			this.Controls.Add(this.checkBoxPause);
			this.Controls.Add(this.checkBoxMonochrome);
			this.Controls.Add(this.checkBoxForceAlbedo);
			this.Controls.Add(this.checkBoxAnimate);
			this.Controls.Add(this.panelOutput);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "TestHBILForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "HBIL Test";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private PanelOutput panelOutput;
		private System.Windows.Forms.Button buttonReload;
		private System.Windows.Forms.Timer timer;
		private Nuaj.Cirrus.Utility.FloatTrackbarControl floatTrackbarControlEnvironmentIntensity;
		private Nuaj.Cirrus.Utility.FloatTrackbarControl floatTrackbarControlExposure;
		private System.Windows.Forms.Label label5;
		private Nuaj.Cirrus.Utility.FloatTrackbarControl floatTrackbarControlAlbedo;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.CheckBox checkBoxAnimate;
		private System.Windows.Forms.CheckBox checkBoxPause;
		private System.Windows.Forms.Button buttonClear;
		private System.Windows.Forms.CheckBox checkBoxEnableHBIL;
		private System.Windows.Forms.CheckBox checkBoxEnableBentNormal;
		private System.Windows.Forms.CheckBox checkBoxEnableConeVisibility;
		private System.Windows.Forms.CheckBox checkBoxForceAlbedo;
		private System.Windows.Forms.TextBox textBoxInfo;
		private System.Windows.Forms.CheckBox checkBoxMonochrome;
		private System.Windows.Forms.Label label1;
		private Nuaj.Cirrus.Utility.FloatTrackbarControl floatTrackbarControlConeAngleBias;
	}
}

