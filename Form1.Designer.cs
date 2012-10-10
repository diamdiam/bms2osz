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
            this.tbLoad.Size = new System.Drawing.Size(396, 21);
            this.tbLoad.TabIndex = 1;
            // 
            // tbResult
            // 
            this.tbResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbResult.Location = new System.Drawing.Point(12, 72);
            this.tbResult.Multiline = true;
            this.tbResult.Name = "tbResult";
            this.tbResult.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbResult.Size = new System.Drawing.Size(477, 91);
            this.tbResult.TabIndex = 2;
            // 
            // ofdLoad
            // 
            this.ofdLoad.Filter = "BMS file|*.bms;*.bme";
            // 
            // btStart
            // 
            this.btStart.Location = new System.Drawing.Point(414, 42);
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
            this.cb_all.Location = new System.Drawing.Point(13, 46);
            this.cb_all.Name = "cb_all";
            this.cb_all.Size = new System.Drawing.Size(252, 16);
            this.cb_all.TabIndex = 4;
            this.cb_all.Text = "Convert all bms files in the directory";
            this.cb_all.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 175);
            this.Controls.Add(this.cb_all);
            this.Controls.Add(this.btStart);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.tbLoad);
            this.Controls.Add(this.btLoad);
            this.Name = "Form1";
            this.Text = "BMS converter";
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
    }
}

