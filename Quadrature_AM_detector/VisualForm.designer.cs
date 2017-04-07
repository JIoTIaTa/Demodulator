namespace Exponentiation
{
    partial class VisualForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VisualForm));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title2 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.F_label = new System.Windows.Forms.Label();
            this.T_label = new System.Windows.Forms.Label();
            this.Speed_label = new System.Windows.Forms.Label();
            this.band_label = new System.Windows.Forms.Label();
            this.refreshButton = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.comboBoxFFT = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fftChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fftChart)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.F_label);
            this.splitContainer1.Panel2.Controls.Add(this.T_label);
            this.splitContainer1.Panel2.Controls.Add(this.Speed_label);
            this.splitContainer1.Panel2.Controls.Add(this.band_label);
            this.splitContainer1.Panel2.Controls.Add(this.refreshButton);
            this.splitContainer1.Panel2.Controls.Add(this.comboBoxFFT);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.fftChart);
            this.splitContainer1.Size = new System.Drawing.Size(876, 547);
            this.splitContainer1.SplitterDistance = 277;
            this.splitContainer1.TabIndex = 1;
            // 
            // F_label
            // 
            this.F_label.AutoSize = true;
            this.F_label.BackColor = System.Drawing.Color.GhostWhite;
            this.F_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.F_label.Location = new System.Drawing.Point(361, 497);
            this.F_label.Name = "F_label";
            this.F_label.Size = new System.Drawing.Size(118, 15);
            this.F_label.TabIndex = 10;
            this.F_label.Text = "Центральна частота: ";
            // 
            // T_label
            // 
            this.T_label.AutoSize = true;
            this.T_label.BackColor = System.Drawing.Color.GhostWhite;
            this.T_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.T_label.Location = new System.Drawing.Point(38, 513);
            this.T_label.Name = "T_label";
            this.T_label.Size = new System.Drawing.Size(108, 15);
            this.T_label.TabIndex = 9;
            this.T_label.Text = "Період маніпуляції: ";
            // 
            // Speed_label
            // 
            this.Speed_label.AutoSize = true;
            this.Speed_label.BackColor = System.Drawing.Color.GhostWhite;
            this.Speed_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Speed_label.Location = new System.Drawing.Point(38, 497);
            this.Speed_label.Name = "Speed_label";
            this.Speed_label.Size = new System.Drawing.Size(116, 15);
            this.Speed_label.TabIndex = 8;
            this.Speed_label.Text = "Частота маніпуляції: ";
            // 
            // band_label
            // 
            this.band_label.AutoSize = true;
            this.band_label.BackColor = System.Drawing.Color.GhostWhite;
            this.band_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.band_label.Location = new System.Drawing.Point(361, 513);
            this.band_label.Name = "band_label";
            this.band_label.Size = new System.Drawing.Size(93, 15);
            this.band_label.TabIndex = 7;
            this.band_label.Text = "Виділена смуга: ";
            // 
            // refreshButton
            // 
            this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshButton.ImageKey = "1free-38.png";
            this.refreshButton.ImageList = this.imageList1;
            this.refreshButton.Location = new System.Drawing.Point(535, 32);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(23, 23);
            this.refreshButton.TabIndex = 5;
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "1free-38.png");
            this.imageList1.Images.SetKeyName(1, "free-38.png");
            // 
            // comboBoxFFT
            // 
            this.comboBoxFFT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxFFT.FormattingEnabled = true;
            this.comboBoxFFT.Items.AddRange(new object[] {
            "256",
            "512",
            "1024",
            "2048",
            "4096",
            "8192",
            "16384",
            "32768",
            "65536"});
            this.comboBoxFFT.Location = new System.Drawing.Point(446, 34);
            this.comboBoxFFT.Name = "comboBoxFFT";
            this.comboBoxFFT.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.comboBoxFFT.Size = new System.Drawing.Size(83, 21);
            this.comboBoxFFT.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.GhostWhite;
            this.label1.Location = new System.Drawing.Point(358, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Порядок ШПФ";
            // 
            // fftChart
            // 
            this.fftChart.BackColor = System.Drawing.Color.GhostWhite;
            this.fftChart.BorderSkin.BackColor = System.Drawing.SystemColors.Highlight;
            this.fftChart.BorderSkin.BackHatchStyle = System.Windows.Forms.DataVisualization.Charting.ChartHatchStyle.DiagonalCross;
            this.fftChart.BorderSkin.BorderColor = System.Drawing.Color.White;
            this.fftChart.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Raised;
            chartArea2.AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
            chartArea2.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea2.AxisX.MajorTickMark.Interval = 0D;
            chartArea2.AxisX.Title = "Гц";
            chartArea2.AxisY.IsLabelAutoFit = false;
            chartArea2.AxisY.LabelStyle.Enabled = false;
            chartArea2.AxisY.LabelStyle.IsEndLabelVisible = false;
            chartArea2.AxisY.MajorTickMark.Enabled = false;
            chartArea2.AxisY2.LabelStyle.Enabled = false;
            chartArea2.Name = "ChartArea1";
            this.fftChart.ChartAreas.Add(chartArea2);
            this.fftChart.Cursor = System.Windows.Forms.Cursors.Default;
            legend2.Enabled = false;
            legend2.Name = "Legend1";
            this.fftChart.Legends.Add(legend2);
            this.fftChart.Location = new System.Drawing.Point(0, 0);
            this.fftChart.Name = "fftChart";
            this.fftChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SeaGreen;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Сигнал";
            series4.ChartArea = "ChartArea1";
            series4.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series4.Color = System.Drawing.Color.Red;
            series4.IsVisibleInLegend = false;
            series4.LabelBorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            series4.Legend = "Legend1";
            series4.Name = "АЧХ";
            this.fftChart.Series.Add(series3);
            this.fftChart.Series.Add(series4);
            this.fftChart.Size = new System.Drawing.Size(595, 547);
            this.fftChart.TabIndex = 0;
            this.fftChart.Text = "АЧХ";
            title2.Name = "Title1";
            title2.Text = "АЧХ";
            this.fftChart.Titles.Add(title2);
            this.fftChart.AxisViewChanged += new System.EventHandler<System.Windows.Forms.DataVisualization.Charting.ViewEventArgs>(this.fftChart_AxisViewChanged);
            // 
            // VisualForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 547);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VisualForm";
            this.Text = "Характеристики фільтру";
            this.Load += new System.EventHandler(this.VisualForm_Load);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fftChart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxFFT;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.DataVisualization.Charting.Chart fftChart;
        private System.Windows.Forms.Label band_label;
        private System.Windows.Forms.Label Speed_label;
        private System.Windows.Forms.Label T_label;
        private System.Windows.Forms.Label F_label;
    }
}