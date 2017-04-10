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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.F_label = new System.Windows.Forms.Label();
            this.T_label = new System.Windows.Forms.Label();
            this.Speed_label = new System.Windows.Forms.Label();
            this.band_label = new System.Windows.Forms.Label();
            this.refreshButton = new System.Windows.Forms.Button();
            this.comboBoxFFT = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fftChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.fftChart)).BeginInit();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "1free-38.png");
            this.imageList1.Images.SetKeyName(1, "free-38.png");
            // 
            // F_label
            // 
            this.F_label.AutoSize = true;
            this.F_label.BackColor = System.Drawing.Color.GhostWhite;
            this.F_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.F_label.Location = new System.Drawing.Point(362, 497);
            this.F_label.Name = "F_label";
            this.F_label.Size = new System.Drawing.Size(118, 15);
            this.F_label.TabIndex = 18;
            this.F_label.Text = "Центральна частота: ";
            // 
            // T_label
            // 
            this.T_label.AutoSize = true;
            this.T_label.BackColor = System.Drawing.Color.GhostWhite;
            this.T_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.T_label.Location = new System.Drawing.Point(39, 513);
            this.T_label.Name = "T_label";
            this.T_label.Size = new System.Drawing.Size(108, 15);
            this.T_label.TabIndex = 17;
            this.T_label.Text = "Період маніпуляції: ";
            // 
            // Speed_label
            // 
            this.Speed_label.AutoSize = true;
            this.Speed_label.BackColor = System.Drawing.Color.GhostWhite;
            this.Speed_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Speed_label.Location = new System.Drawing.Point(39, 497);
            this.Speed_label.Name = "Speed_label";
            this.Speed_label.Size = new System.Drawing.Size(116, 15);
            this.Speed_label.TabIndex = 16;
            this.Speed_label.Text = "Частота маніпуляції: ";
            // 
            // band_label
            // 
            this.band_label.AutoSize = true;
            this.band_label.BackColor = System.Drawing.Color.GhostWhite;
            this.band_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.band_label.Location = new System.Drawing.Point(362, 513);
            this.band_label.Name = "band_label";
            this.band_label.Size = new System.Drawing.Size(93, 15);
            this.band_label.TabIndex = 15;
            this.band_label.Text = "Виділена смуга: ";
            // 
            // refreshButton
            // 
            this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshButton.ImageKey = "1free-38.png";
            this.refreshButton.ImageList = this.imageList1;
            this.refreshButton.Location = new System.Drawing.Point(536, 28);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(23, 23);
            this.refreshButton.TabIndex = 14;
            this.refreshButton.UseVisualStyleBackColor = true;
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
            this.comboBoxFFT.Location = new System.Drawing.Point(447, 30);
            this.comboBoxFFT.Name = "comboBoxFFT";
            this.comboBoxFFT.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.comboBoxFFT.Size = new System.Drawing.Size(83, 21);
            this.comboBoxFFT.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.GhostWhite;
            this.label1.Location = new System.Drawing.Point(359, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Порядок ШПФ";
            // 
            // fftChart
            // 
            this.fftChart.BackColor = System.Drawing.Color.GhostWhite;
            this.fftChart.BorderSkin.BackColor = System.Drawing.SystemColors.Highlight;
            this.fftChart.BorderSkin.BackHatchStyle = System.Windows.Forms.DataVisualization.Charting.ChartHatchStyle.DiagonalCross;
            this.fftChart.BorderSkin.BorderColor = System.Drawing.Color.White;
            this.fftChart.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Raised;
            chartArea1.AxisX.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
            chartArea1.AxisX.IntervalAutoMode = System.Windows.Forms.DataVisualization.Charting.IntervalAutoMode.VariableCount;
            chartArea1.AxisX.MajorTickMark.Interval = 0D;
            chartArea1.AxisX.Title = "Гц";
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Enabled = false;
            chartArea1.AxisY.LabelStyle.IsEndLabelVisible = false;
            chartArea1.AxisY.MajorTickMark.Enabled = false;
            chartArea1.AxisY2.LabelStyle.Enabled = false;
            chartArea1.Name = "ChartArea1";
            this.fftChart.ChartAreas.Add(chartArea1);
            this.fftChart.Cursor = System.Windows.Forms.Cursors.Default;
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.fftChart.Legends.Add(legend1);
            this.fftChart.Location = new System.Drawing.Point(1, 0);
            this.fftChart.Name = "fftChart";
            this.fftChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.SeaGreen;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Сигнал";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.Red;
            series2.IsVisibleInLegend = false;
            series2.LabelBorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            series2.Legend = "Legend1";
            series2.Name = "АЧХ";
            this.fftChart.Series.Add(series1);
            this.fftChart.Series.Add(series2);
            this.fftChart.Size = new System.Drawing.Size(595, 547);
            this.fftChart.TabIndex = 11;
            this.fftChart.Text = "АЧХ";
            title1.Name = "Title1";
            title1.Text = "АЧХ";
            this.fftChart.Titles.Add(title1);
            // 
            // VisualForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(597, 548);
            this.Controls.Add(this.F_label);
            this.Controls.Add(this.T_label);
            this.Controls.Add(this.Speed_label);
            this.Controls.Add(this.band_label);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.comboBoxFFT);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fftChart);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VisualForm";
            this.Text = "Характеристики фільтру";
            this.Load += new System.EventHandler(this.VisualForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fftChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label F_label;
        private System.Windows.Forms.Label T_label;
        private System.Windows.Forms.Label Speed_label;
        private System.Windows.Forms.Label band_label;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.ComboBox comboBoxFFT;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataVisualization.Charting.Chart fftChart;
    }
}