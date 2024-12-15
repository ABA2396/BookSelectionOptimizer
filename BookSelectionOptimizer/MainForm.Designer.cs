namespace BookSelectionOptimizer
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.txtTargetAmount = new System.Windows.Forms.TextBox();
            this.txtMinQty = new System.Windows.Forms.TextBox();
            this.txtMaxQty = new System.Windows.Forms.TextBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.dgvResults = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.allowZeroCheck = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.书名 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.定价 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.数量 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.小计 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).BeginInit();
            this.SuspendLayout();
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(133, 27);
            this.txtFilePath.Multiline = true;
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(452, 37);
            this.txtFilePath.TabIndex = 0;
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(606, 27);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(119, 37);
            this.btnSelectFile.TabIndex = 1;
            this.btnSelectFile.Text = "选择文件";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // txtTargetAmount
            // 
            this.txtTargetAmount.Location = new System.Drawing.Point(133, 85);
            this.txtTargetAmount.Multiline = true;
            this.txtTargetAmount.Name = "txtTargetAmount";
            this.txtTargetAmount.Size = new System.Drawing.Size(100, 37);
            this.txtTargetAmount.TabIndex = 2;
            // 
            // txtMinQty
            // 
            this.txtMinQty.Location = new System.Drawing.Point(316, 85);
            this.txtMinQty.Multiline = true;
            this.txtMinQty.Name = "txtMinQty";
            this.txtMinQty.Size = new System.Drawing.Size(100, 37);
            this.txtMinQty.TabIndex = 3;
            // 
            // txtMaxQty
            // 
            this.txtMaxQty.Location = new System.Drawing.Point(485, 85);
            this.txtMaxQty.Multiline = true;
            this.txtMaxQty.Name = "txtMaxQty";
            this.txtMaxQty.Size = new System.Drawing.Size(100, 37);
            this.txtMaxQty.TabIndex = 4;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(606, 87);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(119, 37);
            this.btnRun.TabIndex = 5;
            this.btnRun.Text = "开始凑单";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // dgvResults
            // 
            this.dgvResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.书名,
            this.定价,
            this.数量,
            this.小计});
            this.dgvResults.Location = new System.Drawing.Point(40, 149);
            this.dgvResults.Name = "dgvResults";
            this.dgvResults.RowHeadersWidth = 82;
            this.dgvResults.RowTemplate.Height = 33;
            this.dgvResults.Size = new System.Drawing.Size(685, 329);
            this.dgvResults.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 21);
            this.label1.TabIndex = 7;
            this.label1.Text = "文件路径";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(157, 21);
            this.label2.TabIndex = 7;
            this.label2.Text = "目标金额（分）";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(257, 99);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 21);
            this.label3.TabIndex = 7;
            this.label3.Text = "数量下限";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(426, 99);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 21);
            this.label4.TabIndex = 7;
            this.label4.Text = "数量上限";
            // 
            // allowZeroCheck
            // 
            this.allowZeroCheck.AutoSize = true;
            this.allowZeroCheck.Location = new System.Drawing.Point(395, 128);
            this.allowZeroCheck.Name = "allowZeroCheck";
            this.allowZeroCheck.Size = new System.Drawing.Size(127, 27);
            this.allowZeroCheck.TabIndex = 8;
            this.allowZeroCheck.Text = "允许为 0";
            this.allowZeroCheck.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // 书名
            // 
            this.书名.HeaderText = "书名";
            this.书名.MinimumWidth = 10;
            this.书名.Name = "书名";
            this.书名.Width = 250;
            // 
            // 定价
            // 
            this.定价.HeaderText = "定价";
            this.定价.MinimumWidth = 10;
            this.定价.Name = "定价";
            // 
            // 数量
            // 
            this.数量.HeaderText = "数量";
            this.数量.MinimumWidth = 10;
            this.数量.Name = "数量";
            // 
            // 小计
            // 
            this.小计.HeaderText = "小计（元）";
            this.小计.MinimumWidth = 10;
            this.小计.Name = "小计";
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(765, 486);
            this.Controls.Add(this.allowZeroCheck);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvResults);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.txtMaxQty);
            this.Controls.Add(this.txtMinQty);
            this.Controls.Add(this.txtTargetAmount);
            this.Controls.Add(this.btnSelectFile);
            this.Controls.Add(this.txtFilePath);
            this.Name = "MainForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvResults)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnSelectFile;
        private System.Windows.Forms.TextBox txtTargetAmount;
        private System.Windows.Forms.TextBox txtMinQty;
        private System.Windows.Forms.TextBox txtMaxQty;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox allowZeroCheck;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridViewTextBoxColumn 书名;
        private System.Windows.Forms.DataGridViewTextBoxColumn 定价;
        private System.Windows.Forms.DataGridViewTextBoxColumn 数量;
        private System.Windows.Forms.DataGridViewTextBoxColumn 小计;
    }
}

