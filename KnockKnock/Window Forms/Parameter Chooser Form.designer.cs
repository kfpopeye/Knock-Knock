namespace KnockKnock
{
    partial class Parameter_Chooser_Form
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
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.AddButton = new System.Windows.Forms.Button();
            this.RemoveButton = new System.Windows.Forms.Button();
            this.CnclButton = new System.Windows.Forms.Button();
            this.OKbutton = new System.Windows.Forms.Button();
            this.UpButton = new System.Windows.Forms.Button();
            this.DownButton = new System.Windows.Forms.Button();
            this.tokenTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.CheckAllButton = new System.Windows.Forms.Button();
            this.AddAllButton = new System.Windows.Forms.Button();
            this.RemoveAllButton = new System.Windows.Forms.Button();
            this.HlpButton = new System.Windows.Forms.Button();
            this.comboBox_Phase = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grid1 = new SourceGrid.Grid();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.UncheckAllButton = new System.Windows.Forms.Button();
            this.charMapButton = new System.Windows.Forms.Button();
            this.bodyFontButton = new System.Windows.Forms.Button();
            this.headerFontButton = new System.Windows.Forms.Button();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::KnockKnock.Properties.Settings.Default, "HeaderFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.listBox1.Font = global::KnockKnock.Properties.Settings.Default.HeaderFont;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 23;
            this.listBox1.Location = new System.Drawing.Point(16, 33);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(252, 211);
            this.listBox1.Sorted = true;
            this.listBox1.TabIndex = 0;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox1.DoubleClick += new System.EventHandler(this.AddButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Available Parameters:";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(452, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(202, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Scheduled Parameters (in order):";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // AddButton
            // 
            this.AddButton.Enabled = false;
            this.AddButton.Location = new System.Drawing.Point(277, 82);
            this.AddButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(164, 28);
            this.AddButton.TabIndex = 2;
            this.AddButton.Text = "Add -->";
            this.AddButton.UseCompatibleTextRendering = true;
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // RemoveButton
            // 
            this.RemoveButton.Enabled = false;
            this.RemoveButton.Location = new System.Drawing.Point(277, 118);
            this.RemoveButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(164, 28);
            this.RemoveButton.TabIndex = 3;
            this.RemoveButton.Text = "<-- Remove";
            this.RemoveButton.UseCompatibleTextRendering = true;
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // CnclButton
            // 
            this.CnclButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CnclButton.Location = new System.Drawing.Point(512, 128);
            this.CnclButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CnclButton.Name = "CnclButton";
            this.CnclButton.Size = new System.Drawing.Size(100, 28);
            this.CnclButton.TabIndex = 1;
            this.CnclButton.Text = "Cancel";
            this.CnclButton.UseCompatibleTextRendering = true;
            this.CnclButton.UseVisualStyleBackColor = true;
            this.CnclButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // OKbutton
            // 
            this.OKbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKbutton.Enabled = false;
            this.OKbutton.Location = new System.Drawing.Point(404, 128);
            this.OKbutton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OKbutton.Name = "OKbutton";
            this.OKbutton.Size = new System.Drawing.Size(100, 28);
            this.OKbutton.TabIndex = 0;
            this.OKbutton.Text = "OK";
            this.OKbutton.UseCompatibleTextRendering = true;
            this.OKbutton.UseVisualStyleBackColor = true;
            this.OKbutton.Click += new System.EventHandler(this.OKbutton_Click);
            // 
            // UpButton
            // 
            this.UpButton.Enabled = false;
            this.UpButton.Location = new System.Drawing.Point(407, 198);
            this.UpButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.UpButton.Name = "UpButton";
            this.UpButton.Size = new System.Drawing.Size(35, 28);
            this.UpButton.TabIndex = 6;
            this.UpButton.Text = "^";
            this.UpButton.UseCompatibleTextRendering = true;
            this.UpButton.UseVisualStyleBackColor = true;
            this.UpButton.Click += new System.EventHandler(this.UpButton_Click);
            // 
            // DownButton
            // 
            this.DownButton.Enabled = false;
            this.DownButton.Location = new System.Drawing.Point(407, 234);
            this.DownButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.DownButton.Name = "DownButton";
            this.DownButton.Size = new System.Drawing.Size(35, 28);
            this.DownButton.TabIndex = 7;
            this.DownButton.Text = "v";
            this.DownButton.UseCompatibleTextRendering = true;
            this.DownButton.UseVisualStyleBackColor = true;
            this.DownButton.Click += new System.EventHandler(this.DownButton_Click);
            // 
            // tokenTextBox
            // 
            this.tokenTextBox.DataBindings.Add(new System.Windows.Forms.Binding("Font", global::KnockKnock.Properties.Settings.Default, "ScheduleFont", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.tokenTextBox.Font = global::KnockKnock.Properties.Settings.Default.ScheduleFont;
            this.tokenTextBox.Location = new System.Drawing.Point(20, 31);
            this.tokenTextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tokenTextBox.Multiline = true;
            this.tokenTextBox.Name = "tokenTextBox";
            this.tokenTextBox.Size = new System.Drawing.Size(164, 125);
            this.tokenTextBox.TabIndex = 3;
            this.tokenTextBox.Text = "X";
            this.toolTip1.SetToolTip(this.tokenTextBox, " Enter allowable tokens in this textbox. One per line.");
            this.tokenTextBox.WordWrap = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 4);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 20);
            this.label3.TabIndex = 13;
            this.label3.Text = "Tokens (one per line)";
            this.label3.UseCompatibleTextRendering = true;
            // 
            // CheckAllButton
            // 
            this.CheckAllButton.Location = new System.Drawing.Point(620, 4);
            this.CheckAllButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.CheckAllButton.Name = "CheckAllButton";
            this.CheckAllButton.Size = new System.Drawing.Size(100, 28);
            this.CheckAllButton.TabIndex = 8;
            this.CheckAllButton.Text = "Check All";
            this.CheckAllButton.UseCompatibleTextRendering = true;
            this.CheckAllButton.UseVisualStyleBackColor = true;
            this.CheckAllButton.Click += new System.EventHandler(this.CheckAllButton_Click);
            // 
            // AddAllButton
            // 
            this.AddAllButton.Location = new System.Drawing.Point(277, 47);
            this.AddAllButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AddAllButton.Name = "AddAllButton";
            this.AddAllButton.Size = new System.Drawing.Size(164, 28);
            this.AddAllButton.TabIndex = 1;
            this.AddAllButton.Text = "Add All -->>";
            this.AddAllButton.UseCompatibleTextRendering = true;
            this.AddAllButton.UseVisualStyleBackColor = true;
            this.AddAllButton.Click += new System.EventHandler(this.AddAllButton_Click);
            // 
            // RemoveAllButton
            // 
            this.RemoveAllButton.Enabled = false;
            this.RemoveAllButton.Location = new System.Drawing.Point(277, 154);
            this.RemoveAllButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.RemoveAllButton.Name = "RemoveAllButton";
            this.RemoveAllButton.Size = new System.Drawing.Size(164, 28);
            this.RemoveAllButton.TabIndex = 4;
            this.RemoveAllButton.Text = "<<-- Remove All";
            this.RemoveAllButton.UseCompatibleTextRendering = true;
            this.RemoveAllButton.UseVisualStyleBackColor = true;
            this.RemoveAllButton.Click += new System.EventHandler(this.RemoveAllButton_Click);
            // 
            // HlpButton
            // 
            this.HlpButton.Location = new System.Drawing.Point(620, 128);
            this.HlpButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.HlpButton.Name = "HlpButton";
            this.HlpButton.Size = new System.Drawing.Size(100, 28);
            this.HlpButton.TabIndex = 2;
            this.HlpButton.Text = "Help";
            this.HlpButton.UseCompatibleTextRendering = true;
            this.HlpButton.UseVisualStyleBackColor = true;
            this.HlpButton.Click += new System.EventHandler(this.HelpButtonClick);
            // 
            // comboBox_Phase
            // 
            this.comboBox_Phase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Phase.Location = new System.Drawing.Point(404, 75);
            this.comboBox_Phase.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.comboBox_Phase.Name = "comboBox_Phase";
            this.comboBox_Phase.Size = new System.Drawing.Size(315, 24);
            this.comboBox_Phase.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(348, 79);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 20);
            this.label4.TabIndex = 19;
            this.label4.Text = "Phase";
            this.label4.UseCompatibleTextRendering = true;
            // 
            // grid1
            // 
            this.grid1.EnableSort = false;
            this.grid1.Location = new System.Drawing.Point(452, 33);
            this.grid1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.grid1.Name = "grid1";
            this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid1.Size = new System.Drawing.Size(268, 226);
            this.grid1.TabIndex = 5;
            this.grid1.TabStop = true;
            this.grid1.ToolTipText = "Click and drag to select multipe parameters";
            this.grid1.Click += new System.EventHandler(this.grid1_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            this.splitContainer1.Panel1.Controls.Add(this.grid1);
            this.splitContainer1.Panel1.Controls.Add(this.listBox1);
            this.splitContainer1.Panel1.Controls.Add(this.RemoveAllButton);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.AddAllButton);
            this.splitContainer1.Panel1.Controls.Add(this.AddButton);
            this.splitContainer1.Panel1.Controls.Add(this.DownButton);
            this.splitContainer1.Panel1.Controls.Add(this.RemoveButton);
            this.splitContainer1.Panel1.Controls.Add(this.UpButton);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.UncheckAllButton);
            this.splitContainer1.Panel2.Controls.Add(this.charMapButton);
            this.splitContainer1.Panel2.Controls.Add(this.bodyFontButton);
            this.splitContainer1.Panel2.Controls.Add(this.headerFontButton);
            this.splitContainer1.Panel2.Controls.Add(this.tokenTextBox);
            this.splitContainer1.Panel2.Controls.Add(this.HlpButton);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.CnclButton);
            this.splitContainer1.Panel2.Controls.Add(this.comboBox_Phase);
            this.splitContainer1.Panel2.Controls.Add(this.OKbutton);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.CheckAllButton);
            this.splitContainer1.Size = new System.Drawing.Size(733, 938);
            this.splitContainer1.SplitterDistance = 750;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 22;
            // 
            // UncheckAllButton
            // 
            this.UncheckAllButton.Location = new System.Drawing.Point(452, 4);
            this.UncheckAllButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.UncheckAllButton.Name = "UncheckAllButton";
            this.UncheckAllButton.Size = new System.Drawing.Size(100, 28);
            this.UncheckAllButton.TabIndex = 7;
            this.UncheckAllButton.Text = "Uncheck All";
            this.UncheckAllButton.UseVisualStyleBackColor = true;
            this.UncheckAllButton.Click += new System.EventHandler(this.UncheckAllButton_Click);
            // 
            // charMapButton
            // 
            this.charMapButton.Location = new System.Drawing.Point(193, 128);
            this.charMapButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.charMapButton.Name = "charMapButton";
            this.charMapButton.Size = new System.Drawing.Size(148, 28);
            this.charMapButton.TabIndex = 6;
            this.charMapButton.Text = "Character Map";
            this.charMapButton.UseVisualStyleBackColor = true;
            this.charMapButton.Click += new System.EventHandler(this.charMapButton_Click);
            // 
            // bodyFontButton
            // 
            this.bodyFontButton.Location = new System.Drawing.Point(193, 66);
            this.bodyFontButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bodyFontButton.Name = "bodyFontButton";
            this.bodyFontButton.Size = new System.Drawing.Size(148, 28);
            this.bodyFontButton.TabIndex = 5;
            this.bodyFontButton.Text = "Body Font";
            this.bodyFontButton.UseVisualStyleBackColor = true;
            this.bodyFontButton.Click += new System.EventHandler(this.bodyFontButton_Click);
            // 
            // headerFontButton
            // 
            this.headerFontButton.Location = new System.Drawing.Point(193, 31);
            this.headerFontButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.headerFontButton.Name = "headerFontButton";
            this.headerFontButton.Size = new System.Drawing.Size(148, 28);
            this.headerFontButton.TabIndex = 4;
            this.headerFontButton.Text = "Header Font";
            this.headerFontButton.UseVisualStyleBackColor = true;
            this.headerFontButton.Click += new System.EventHandler(this.headerFontButton_Click);
            // 
            // fontDialog
            // 
            this.fontDialog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            // 
            // Parameter_Chooser_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(733, 938);
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(751, 2451);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(751, 688);
            this.Name = "Parameter_Chooser_Form";
            this.ShowIcon = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Parameter Chooser";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.Parameter_Chooser_Form_Load);
            this.Resize += new System.EventHandler(this.Parameter_Chooser_Form_Resize);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.ComboBox comboBox_Phase;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button HlpButton;

        #endregion

        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.Button CnclButton;
        private System.Windows.Forms.Button OKbutton;
        private System.Windows.Forms.Button UpButton;
        private System.Windows.Forms.Button DownButton;
        private System.Windows.Forms.TextBox tokenTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button CheckAllButton;
        private System.Windows.Forms.Button AddAllButton;
        private System.Windows.Forms.Button RemoveAllButton;
        private SourceGrid.Grid grid1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.Button bodyFontButton;
        private System.Windows.Forms.Button headerFontButton;
        private System.Windows.Forms.Button charMapButton;
        private System.Windows.Forms.Button UncheckAllButton;
    }
}