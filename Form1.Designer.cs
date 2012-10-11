namespace bms
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btLoad = new System.Windows.Forms.Button();
            this.tbLoad = new System.Windows.Forms.TextBox();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.ofdLoad = new System.Windows.Forms.OpenFileDialog();
            this.btStart = new System.Windows.Forms.Button();
            this.cb_all = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_artist = new System.Windows.Forms.TextBox();
            this.tb_version = new System.Windows.Forms.TextBox();
            this.tb_title = new System.Windows.Forms.TextBox();
            this.tb_source = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btLoad
            // 
            this.btLoad.Location = new System.Drawing.Point(13, 13);
            this.btLoad.Name = "btLoad";
            this.btLoad.Size = new System.Drawing.Size(75, 23);
            this.btLoad.TabIndex = 0;
            this.btLoad.Text = "Load";
            this.btLoad.UseVisualStyleBackColor = true;
            this.btLoad.Click += new System.EventHandler(this.btLoad_Click);
            // 
            // tbLoad
            // 
            this.tbLoad.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbLoad.Location = new System.Drawing.Point(94, 13);
            this.tbLoad.Name = "tbLoad";
            this.tbLoad.Size = new System.Drawing.Size(352, 21);
            this.tbLoad.TabIndex = 1;
            // 
            // tbResult
            // 
            this.tbResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbResult.Location = new System.Drawing.Point(11, 194);
            this.tbResult.Multiline = true;
            this.tbResult.Name = "tbResult";
            this.tbResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResult.Size = new System.Drawing.Size(434, 91);
            this.tbResult.TabIndex = 2;
            // 
            // ofdLoad
            // 
            this.ofdLoad.Filter = "BMS file|*.bms;*.bme";
            // 
            // btStart
            // 
            this.btStart.Location = new System.Drawing.Point(370, 156);
            this.btStart.Name = "btStart";
            this.btStart.Size = new System.Drawing.Size(75, 23);
            this.btStart.TabIndex = 3;
            this.btStart.Text = "Start";
            this.btStart.UseVisualStyleBackColor = true;
            this.btStart.Click += new System.EventHandler(this.btStart_Click);
            // 
            // cb_all
            // 
            this.cb_all.AutoSize = true;
            this.cb_all.Location = new System.Drawing.Point(17, 160);
            this.cb_all.Name = "cb_all";
            this.cb_all.Size = new System.Drawing.Size(252, 16);
            this.cb_all.TabIndex = 4;
            this.cb_all.Text = "Convert all bms files in the directory";
            this.cb_all.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tb_source);
            this.groupBox1.Controls.Add(this.tb_title);
            this.groupBox1.Controls.Add(this.tb_version);
            this.groupBox1.Controls.Add(this.tb_artist);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 38);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(434, 110);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Meta";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(311, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Notice: Leave field empty to use the original value";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Artist";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "Title";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(220, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "Version";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(222, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "Source";
            // 
            // tb_artist
            // 
            this.tb_artist.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_artist.Location = new System.Drawing.Point(54, 42);
            this.tb_artist.Name = "tb_artist";
            this.tb_artist.Size = new System.Drawing.Size(157, 21);
            this.tb_artist.TabIndex = 5;
            // 
            // tb_version
            // 
            this.tb_version.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_version.Location = new System.Drawing.Point(271, 42);
            this.tb_version.Name = "tb_version";
            this.tb_version.Size = new System.Drawing.Size(157, 21);
            this.tb_version.TabIndex = 6;
            // 
            // tb_title
            // 
            this.tb_title.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_title.Location = new System.Drawing.Point(54, 73);
            this.tb_title.Name = "tb_title";
            this.tb_title.Size = new System.Drawing.Size(157, 21);
            this.tb_title.TabIndex = 7;
            // 
            // tb_source
            // 
            this.tb_source.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tb_source.Location = new System.Drawing.Point(271, 73);
            this.tb_source.Name = "tb_source";
            this.tb_source.Size = new System.Drawing.Size(157, 21);
            this.tb_source.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 297);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cb_all);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.tbLoad);
            this.Controls.Add(this.btLoad);
            this.Name = "Form1";
            this.Text = "BMS converter";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btLoad;
        private System.Windows.Forms.TextBox tbLoad;
        private System.Windows.Forms.TextBox tbResult;
        private System.Windows.Forms.OpenFileDialog ofdLoad;
        private System.Windows.Forms.Button btStart;
        private System.Windows.Forms.CheckBox cb_all;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_source;
        private System.Windows.Forms.TextBox tb_title;
        private System.Windows.Forms.TextBox tb_version;
        private System.Windows.Forms.TextBox tb_artist;
    }
}

