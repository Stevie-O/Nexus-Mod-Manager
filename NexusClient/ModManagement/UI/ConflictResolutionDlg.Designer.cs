namespace Nexus.Client.ModManagement.UI
{
    partial class ConflictResolutionDlg
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
            this.label1 = new System.Windows.Forms.Label();
            this.tvConflicts = new Nexus.Client.ModManagement.UI.FixedTreeView();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.btnUncheckAll = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.chkGroupByMod = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(978, 66);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // tvConflicts
            // 
            this.tvConflicts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvConflicts.CheckBoxes = true;
            this.tvConflicts.Location = new System.Drawing.Point(12, 105);
            this.tvConflicts.Name = "tvConflicts";
            this.tvConflicts.Size = new System.Drawing.Size(978, 205);
            this.tvConflicts.TabIndex = 1;
            this.tvConflicts.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvConflicts_AfterCheck);
            // 
            // btnCheckAll
            // 
            this.btnCheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCheckAll.Location = new System.Drawing.Point(15, 316);
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.Size = new System.Drawing.Size(141, 31);
            this.btnCheckAll.TabIndex = 2;
            this.btnCheckAll.Text = "&Check All";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // btnUncheckAll
            // 
            this.btnUncheckAll.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUncheckAll.Location = new System.Drawing.Point(162, 316);
            this.btnUncheckAll.Name = "btnUncheckAll";
            this.btnUncheckAll.Size = new System.Drawing.Size(111, 31);
            this.btnUncheckAll.TabIndex = 2;
            this.btnUncheckAll.Text = "&Uncheck All";
            this.btnUncheckAll.UseVisualStyleBackColor = true;
            this.btnUncheckAll.Click += new System.EventHandler(this.btnUncheckAll_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(879, 316);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(111, 31);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // chkGroupByMod
            // 
            this.chkGroupByMod.AutoSize = true;
            this.chkGroupByMod.Checked = true;
            this.chkGroupByMod.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkGroupByMod.Location = new System.Drawing.Point(12, 78);
            this.chkGroupByMod.Name = "chkGroupByMod";
            this.chkGroupByMod.Size = new System.Drawing.Size(120, 21);
            this.chkGroupByMod.TabIndex = 3;
            this.chkGroupByMod.Text = "Group by Mod";
            this.chkGroupByMod.UseVisualStyleBackColor = true;
            this.chkGroupByMod.CheckedChanged += new System.EventHandler(this.chkGroupByMod_CheckedChanged);
            // 
            // ConflictResolutionDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 359);
            this.ControlBox = false;
            this.Controls.Add(this.chkGroupByMod);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnUncheckAll);
            this.Controls.Add(this.btnCheckAll);
            this.Controls.Add(this.tvConflicts);
            this.Controls.Add(this.label1);
            this.MinimizeBox = false;
            this.Name = "ConflictResolutionDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Conflict Detected";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private FixedTreeView tvConflicts;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.Button btnUncheckAll;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chkGroupByMod;
    }
}