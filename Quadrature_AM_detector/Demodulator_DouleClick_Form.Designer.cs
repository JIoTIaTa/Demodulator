namespace demodulation_namespace
{
    partial class Demodulator_DoubleClick_Form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Demodulator_DoubleClick_Form));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.save = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SR = new System.Windows.Forms.Label();
            this.F = new System.Windows.Forms.Label();
            this.SRvalue = new System.Windows.Forms.Label();
            this.Fvalue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Show = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.exponentiationLevel = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.exponentiationLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(96, 84);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(156, 22);
            this.save.TabIndex = 8;
            this.save.Text = "Прийняти";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "1478805825_pause.png");
            this.imageList1.Images.SetKeyName(1, "1478805839_play 1.png");
            // 
            // SR
            // 
            this.SR.AutoSize = true;
            this.SR.Location = new System.Drawing.Point(12, 5);
            this.SR.Name = "SR";
            this.SR.Size = new System.Drawing.Size(125, 13);
            this.SR.TabIndex = 10;
            this.SR.Text = "Частота дискретизації:";
            // 
            // F
            // 
            this.F.AutoSize = true;
            this.F.Location = new System.Drawing.Point(12, 32);
            this.F.Name = "F";
            this.F.Size = new System.Drawing.Size(113, 13);
            this.F.TabIndex = 11;
            this.F.Text = "Центральна частота:";
            // 
            // SRvalue
            // 
            this.SRvalue.AutoSize = true;
            this.SRvalue.Location = new System.Drawing.Point(137, 5);
            this.SRvalue.Name = "SRvalue";
            this.SRvalue.Size = new System.Drawing.Size(49, 13);
            this.SRvalue.TabIndex = 12;
            this.SRvalue.Text = "8000000";
            // 
            // Fvalue
            // 
            this.Fvalue.Location = new System.Drawing.Point(122, 29);
            this.Fvalue.Name = "Fvalue";
            this.Fvalue.Size = new System.Drawing.Size(63, 20);
            this.Fvalue.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(186, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "Гц";
            // 
            // Show
            // 
            this.Show.AutoSize = true;
            this.Show.Location = new System.Drawing.Point(314, 81);
            this.Show.Name = "Show";
            this.Show.Size = new System.Drawing.Size(15, 14);
            this.Show.TabIndex = 15;
            this.Show.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(305, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(33, 27);
            this.button1.TabIndex = 16;
            this.button1.Text = "^2";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(258, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Інтерполяція";
            // 
            // exponentiationLevel
            // 
            this.exponentiationLevel.Location = new System.Drawing.Point(129, 58);
            this.exponentiationLevel.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.exponentiationLevel.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.exponentiationLevel.Name = "exponentiationLevel";
            this.exponentiationLevel.Size = new System.Drawing.Size(40, 20);
            this.exponentiationLevel.TabIndex = 18;
            this.exponentiationLevel.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Піднести до степеня";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(241, 21);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(33, 27);
            this.button2.TabIndex = 20;
            this.button2.Text = "^-2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(284, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(13, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "1";
            // 
            // Quadrature_AM_detectorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 107);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.exponentiationLevel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Show);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Fvalue);
            this.Controls.Add(this.SRvalue);
            this.Controls.Add(this.F);
            this.Controls.Add(this.SR);
            this.Controls.Add(this.save);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Quadrature_AM_detectorForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Визначення параметрів";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ExponentiationForm_Load);
            this.Shown += new System.EventHandler(this.FirFilterForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.exponentiationLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Label SR;
        private System.Windows.Forms.Label F;
        private System.Windows.Forms.Label SRvalue;
        private System.Windows.Forms.TextBox Fvalue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox Show;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown exponentiationLevel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label4;
    }
}