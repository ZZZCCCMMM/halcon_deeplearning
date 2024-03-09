namespace identity
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.inputbtn = new System.Windows.Forms.Button();
            this.identitybtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl1.Location = new System.Drawing.Point(13, 13);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(619, 425);
            this.hWindowControl1.TabIndex = 0;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(619, 425);
            // 
            // inputbtn
            // 
            this.inputbtn.Location = new System.Drawing.Point(669, 63);
            this.inputbtn.Name = "inputbtn";
            this.inputbtn.Size = new System.Drawing.Size(89, 52);
            this.inputbtn.TabIndex = 1;
            this.inputbtn.Text = "导入模型";
            this.inputbtn.UseVisualStyleBackColor = true;
            this.inputbtn.Click += new System.EventHandler(this.inputbtn_Click);
            // 
            // identitybtn
            // 
            this.identitybtn.Location = new System.Drawing.Point(669, 216);
            this.identitybtn.Name = "identitybtn";
            this.identitybtn.Size = new System.Drawing.Size(89, 53);
            this.identitybtn.TabIndex = 2;
            this.identitybtn.Text = "异常检测";
            this.identitybtn.UseVisualStyleBackColor = true;
            this.identitybtn.Click += new System.EventHandler(this.identitybtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.identitybtn);
            this.Controls.Add(this.inputbtn);
            this.Controls.Add(this.hWindowControl1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private HalconDotNet.HWindowControl hWindowControl1;
        private System.Windows.Forms.Button inputbtn;
        private System.Windows.Forms.Button identitybtn;
    }
}

