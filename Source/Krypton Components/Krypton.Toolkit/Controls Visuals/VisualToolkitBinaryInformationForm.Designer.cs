﻿namespace Krypton.Toolkit
{
    partial class VisualToolkitBinaryInformationForm
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
            this.kryptonPanel1 = new Krypton.Toolkit.KryptonPanel();
            this.kbtnOk = new Krypton.Toolkit.KryptonButton();
            this.kryptonBorderEdge1 = new Krypton.Toolkit.KryptonBorderEdge();
            this.kryptonPanel2 = new Krypton.Toolkit.KryptonPanel();
            this.tlpContent = new System.Windows.Forms.TableLayoutPanel();
            this.kwlblDockingTitle = new Krypton.Toolkit.KryptonWrapLabel();
            this.kwlblDockingFileInformation = new Krypton.Toolkit.KryptonWrapLabel();
            this.kwlblNavigatorTitle = new Krypton.Toolkit.KryptonWrapLabel();
            this.kwlblNavigatorFileInformation = new Krypton.Toolkit.KryptonWrapLabel();
            this.kwlblRibbonTitle = new Krypton.Toolkit.KryptonWrapLabel();
            this.kwlblRibbonFileInformation = new Krypton.Toolkit.KryptonWrapLabel();
            this.kwlblToolkitTitle = new Krypton.Toolkit.KryptonWrapLabel();
            this.kwlblToolkitFileInformation = new Krypton.Toolkit.KryptonWrapLabel();
            this.kwlblWorkspaceTitle = new Krypton.Toolkit.KryptonWrapLabel();
            this.kwlblWorkspaceFileInformation = new Krypton.Toolkit.KryptonWrapLabel();
            this.kryptonPanel3 = new Krypton.Toolkit.KryptonPanel();
            this.pbxIcon = new System.Windows.Forms.PictureBox();
            this.kwlblTitle = new Krypton.Toolkit.KryptonWrapLabel();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).BeginInit();
            this.kryptonPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).BeginInit();
            this.kryptonPanel2.SuspendLayout();
            this.tlpContent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel3)).BeginInit();
            this.kryptonPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // kryptonPanel1
            // 
            this.kryptonPanel1.Controls.Add(this.kbtnOk);
            this.kryptonPanel1.Controls.Add(this.kryptonBorderEdge1);
            this.kryptonPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.kryptonPanel1.Location = new System.Drawing.Point(0, 379);
            this.kryptonPanel1.Name = "kryptonPanel1";
            this.kryptonPanel1.PanelBackStyle = Krypton.Toolkit.PaletteBackStyle.PanelAlternate;
            this.kryptonPanel1.Size = new System.Drawing.Size(676, 50);
            this.kryptonPanel1.TabIndex = 0;
            // 
            // kbtnOk
            // 
            this.kbtnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.kbtnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.kbtnOk.Location = new System.Drawing.Point(570, 13);
            this.kbtnOk.Name = "kbtnOk";
            this.kbtnOk.Size = new System.Drawing.Size(90, 25);
            this.kbtnOk.TabIndex = 1;
            this.kbtnOk.Values.DropDownArrowColor = System.Drawing.Color.Empty;
            this.kbtnOk.Values.Text = "O&K";
            this.kbtnOk.Values.UseAsADialogButton = true;
            this.kbtnOk.Click += new System.EventHandler(this.kbtnOk_Click);
            // 
            // kryptonBorderEdge1
            // 
            this.kryptonBorderEdge1.BorderStyle = Krypton.Toolkit.PaletteBorderStyle.HeaderPrimary;
            this.kryptonBorderEdge1.Dock = System.Windows.Forms.DockStyle.Top;
            this.kryptonBorderEdge1.Location = new System.Drawing.Point(0, 0);
            this.kryptonBorderEdge1.Name = "kryptonBorderEdge1";
            this.kryptonBorderEdge1.Size = new System.Drawing.Size(676, 1);
            this.kryptonBorderEdge1.Text = "kryptonBorderEdge1";
            // 
            // kryptonPanel2
            // 
            this.kryptonPanel2.Controls.Add(this.tlpContent);
            this.kryptonPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel2.Location = new System.Drawing.Point(0, 0);
            this.kryptonPanel2.Name = "kryptonPanel2";
            this.kryptonPanel2.Size = new System.Drawing.Size(676, 379);
            this.kryptonPanel2.TabIndex = 2;
            // 
            // tlpContent
            // 
            this.tlpContent.BackColor = System.Drawing.Color.Transparent;
            this.tlpContent.ColumnCount = 2;
            this.tlpContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpContent.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpContent.Controls.Add(this.kwlblDockingTitle, 0, 1);
            this.tlpContent.Controls.Add(this.kwlblDockingFileInformation, 1, 1);
            this.tlpContent.Controls.Add(this.kwlblNavigatorTitle, 0, 2);
            this.tlpContent.Controls.Add(this.kwlblNavigatorFileInformation, 1, 2);
            this.tlpContent.Controls.Add(this.kwlblRibbonTitle, 0, 3);
            this.tlpContent.Controls.Add(this.kwlblRibbonFileInformation, 1, 3);
            this.tlpContent.Controls.Add(this.kwlblToolkitTitle, 0, 4);
            this.tlpContent.Controls.Add(this.kwlblToolkitFileInformation, 1, 4);
            this.tlpContent.Controls.Add(this.kwlblWorkspaceTitle, 0, 5);
            this.tlpContent.Controls.Add(this.kwlblWorkspaceFileInformation, 1, 5);
            this.tlpContent.Controls.Add(this.kryptonPanel3, 0, 0);
            this.tlpContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpContent.Location = new System.Drawing.Point(0, 0);
            this.tlpContent.Name = "tlpContent";
            this.tlpContent.RowCount = 6;
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpContent.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tlpContent.Size = new System.Drawing.Size(676, 379);
            this.tlpContent.TabIndex = 0;
            // 
            // kwlblDockingTitle
            // 
            this.kwlblDockingTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kwlblDockingTitle.LabelStyle = Krypton.Toolkit.LabelStyle.BoldControl;
            this.kwlblDockingTitle.Location = new System.Drawing.Point(5, 55);
            this.kwlblDockingTitle.Margin = new System.Windows.Forms.Padding(5);
            this.kwlblDockingTitle.Name = "kwlblDockingTitle";
            this.kwlblDockingTitle.Padding = new System.Windows.Forms.Padding(5);
            this.kwlblDockingTitle.Size = new System.Drawing.Size(328, 55);
            this.kwlblDockingTitle.Text = "kryptonWrapLabel2";
            this.kwlblDockingTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // kwlblDockingFileInformation
            // 
            this.kwlblDockingFileInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kwlblDockingFileInformation.LabelStyle = Krypton.Toolkit.LabelStyle.AlternateControl;
            this.kwlblDockingFileInformation.Location = new System.Drawing.Point(343, 55);
            this.kwlblDockingFileInformation.Margin = new System.Windows.Forms.Padding(5);
            this.kwlblDockingFileInformation.Name = "kwlblDockingFileInformation";
            this.kwlblDockingFileInformation.Padding = new System.Windows.Forms.Padding(5);
            this.kwlblDockingFileInformation.Size = new System.Drawing.Size(328, 55);
            this.kwlblDockingFileInformation.Text = "kryptonWrapLabel3";
            this.kwlblDockingFileInformation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // kwlblNavigatorTitle
            // 
            this.kwlblNavigatorTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kwlblNavigatorTitle.LabelStyle = Krypton.Toolkit.LabelStyle.BoldControl;
            this.kwlblNavigatorTitle.Location = new System.Drawing.Point(5, 120);
            this.kwlblNavigatorTitle.Margin = new System.Windows.Forms.Padding(5);
            this.kwlblNavigatorTitle.Name = "kwlblNavigatorTitle";
            this.kwlblNavigatorTitle.Padding = new System.Windows.Forms.Padding(5);
            this.kwlblNavigatorTitle.Size = new System.Drawing.Size(328, 55);
            this.kwlblNavigatorTitle.Text = "kryptonWrapLabel4";
            this.kwlblNavigatorTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // kwlblNavigatorFileInformation
            // 
            this.kwlblNavigatorFileInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kwlblNavigatorFileInformation.LabelStyle = Krypton.Toolkit.LabelStyle.AlternateControl;
            this.kwlblNavigatorFileInformation.Location = new System.Drawing.Point(343, 120);
            this.kwlblNavigatorFileInformation.Margin = new System.Windows.Forms.Padding(5);
            this.kwlblNavigatorFileInformation.Name = "kwlblNavigatorFileInformation";
            this.kwlblNavigatorFileInformation.Padding = new System.Windows.Forms.Padding(5);
            this.kwlblNavigatorFileInformation.Size = new System.Drawing.Size(328, 55);
            this.kwlblNavigatorFileInformation.Text = "kryptonWrapLabel5";
            this.kwlblNavigatorFileInformation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // kwlblRibbonTitle
            // 
            this.kwlblRibbonTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kwlblRibbonTitle.LabelStyle = Krypton.Toolkit.LabelStyle.BoldControl;
            this.kwlblRibbonTitle.Location = new System.Drawing.Point(5, 185);
            this.kwlblRibbonTitle.Margin = new System.Windows.Forms.Padding(5);
            this.kwlblRibbonTitle.Name = "kwlblRibbonTitle";
            this.kwlblRibbonTitle.Padding = new System.Windows.Forms.Padding(5);
            this.kwlblRibbonTitle.Size = new System.Drawing.Size(328, 55);
            this.kwlblRibbonTitle.Text = "kryptonWrapLabel6";
            this.kwlblRibbonTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // kwlblRibbonFileInformation
            // 
            this.kwlblRibbonFileInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kwlblRibbonFileInformation.LabelStyle = Krypton.Toolkit.LabelStyle.AlternateControl;
            this.kwlblRibbonFileInformation.Location = new System.Drawing.Point(343, 185);
            this.kwlblRibbonFileInformation.Margin = new System.Windows.Forms.Padding(5);
            this.kwlblRibbonFileInformation.Name = "kwlblRibbonFileInformation";
            this.kwlblRibbonFileInformation.Padding = new System.Windows.Forms.Padding(5);
            this.kwlblRibbonFileInformation.Size = new System.Drawing.Size(328, 55);
            this.kwlblRibbonFileInformation.Text = "kryptonWrapLabel7";
            this.kwlblRibbonFileInformation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // kwlblToolkitTitle
            // 
            this.kwlblToolkitTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kwlblToolkitTitle.LabelStyle = Krypton.Toolkit.LabelStyle.BoldControl;
            this.kwlblToolkitTitle.Location = new System.Drawing.Point(5, 250);
            this.kwlblToolkitTitle.Margin = new System.Windows.Forms.Padding(5);
            this.kwlblToolkitTitle.Name = "kwlblToolkitTitle";
            this.kwlblToolkitTitle.Padding = new System.Windows.Forms.Padding(5);
            this.kwlblToolkitTitle.Size = new System.Drawing.Size(328, 55);
            this.kwlblToolkitTitle.Text = "kryptonWrapLabel8";
            this.kwlblToolkitTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // kwlblToolkitFileInformation
            // 
            this.kwlblToolkitFileInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kwlblToolkitFileInformation.LabelStyle = Krypton.Toolkit.LabelStyle.AlternateControl;
            this.kwlblToolkitFileInformation.Location = new System.Drawing.Point(343, 250);
            this.kwlblToolkitFileInformation.Margin = new System.Windows.Forms.Padding(5);
            this.kwlblToolkitFileInformation.Name = "kwlblToolkitFileInformation";
            this.kwlblToolkitFileInformation.Padding = new System.Windows.Forms.Padding(5);
            this.kwlblToolkitFileInformation.Size = new System.Drawing.Size(328, 55);
            this.kwlblToolkitFileInformation.Text = "kryptonWrapLabel9";
            this.kwlblToolkitFileInformation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // kwlblWorkspaceTitle
            // 
            this.kwlblWorkspaceTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kwlblWorkspaceTitle.LabelStyle = Krypton.Toolkit.LabelStyle.BoldControl;
            this.kwlblWorkspaceTitle.Location = new System.Drawing.Point(5, 315);
            this.kwlblWorkspaceTitle.Margin = new System.Windows.Forms.Padding(5);
            this.kwlblWorkspaceTitle.Name = "kwlblWorkspaceTitle";
            this.kwlblWorkspaceTitle.Padding = new System.Windows.Forms.Padding(5);
            this.kwlblWorkspaceTitle.Size = new System.Drawing.Size(328, 59);
            this.kwlblWorkspaceTitle.Text = "kryptonWrapLabel10";
            this.kwlblWorkspaceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // kwlblWorkspaceFileInformation
            // 
            this.kwlblWorkspaceFileInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kwlblWorkspaceFileInformation.LabelStyle = Krypton.Toolkit.LabelStyle.AlternateControl;
            this.kwlblWorkspaceFileInformation.Location = new System.Drawing.Point(343, 315);
            this.kwlblWorkspaceFileInformation.Margin = new System.Windows.Forms.Padding(5);
            this.kwlblWorkspaceFileInformation.Name = "kwlblWorkspaceFileInformation";
            this.kwlblWorkspaceFileInformation.Padding = new System.Windows.Forms.Padding(5);
            this.kwlblWorkspaceFileInformation.Size = new System.Drawing.Size(328, 59);
            this.kwlblWorkspaceFileInformation.Text = "kryptonWrapLabel11";
            this.kwlblWorkspaceFileInformation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // kryptonPanel3
            // 
            this.tlpContent.SetColumnSpan(this.kryptonPanel3, 2);
            this.kryptonPanel3.Controls.Add(this.kwlblTitle);
            this.kryptonPanel3.Controls.Add(this.pbxIcon);
            this.kryptonPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kryptonPanel3.Location = new System.Drawing.Point(3, 3);
            this.kryptonPanel3.Name = "kryptonPanel3";
            this.kryptonPanel3.Size = new System.Drawing.Size(670, 44);
            this.kryptonPanel3.TabIndex = 11;
            // 
            // pbxIcon
            // 
            this.pbxIcon.Dock = System.Windows.Forms.DockStyle.Left;
            this.pbxIcon.Location = new System.Drawing.Point(0, 0);
            this.pbxIcon.Name = "pbxIcon";
            this.pbxIcon.Size = new System.Drawing.Size(48, 44);
            this.pbxIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbxIcon.TabIndex = 0;
            this.pbxIcon.TabStop = false;
            // 
            // kwlblTitle
            // 
            this.kwlblTitle.AutoSize = false;
            this.kwlblTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kwlblTitle.LabelStyle = Krypton.Toolkit.LabelStyle.TitleControl;
            this.kwlblTitle.Location = new System.Drawing.Point(48, 0);
            this.kwlblTitle.Margin = new System.Windows.Forms.Padding(5);
            this.kwlblTitle.Name = "kwlblTitle";
            this.kwlblTitle.Size = new System.Drawing.Size(622, 44);
            this.kwlblTitle.Text = "kryptonWrapLabel1";
            this.kwlblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // VisualToolkitBinaryInformationForm
            // 
            this.AcceptButton = this.kbtnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 429);
            this.Controls.Add(this.kryptonPanel2);
            this.Controls.Add(this.kryptonPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VisualToolkitBinaryInformationForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.VisualToolkitBinaryInformationForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel1)).EndInit();
            this.kryptonPanel1.ResumeLayout(false);
            this.kryptonPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel2)).EndInit();
            this.kryptonPanel2.ResumeLayout(false);
            this.tlpContent.ResumeLayout(false);
            this.tlpContent.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.kryptonPanel3)).EndInit();
            this.kryptonPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbxIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private KryptonPanel kryptonPanel1;
        private KryptonButton kbtnOk;
        private KryptonBorderEdge kryptonBorderEdge1;
        private KryptonPanel kryptonPanel2;
        private TableLayoutPanel tlpContent;
        private KryptonWrapLabel kwlblDockingTitle;
        private KryptonWrapLabel kwlblDockingFileInformation;
        private KryptonWrapLabel kwlblNavigatorTitle;
        private KryptonWrapLabel kwlblNavigatorFileInformation;
        private KryptonWrapLabel kwlblRibbonTitle;
        private KryptonWrapLabel kwlblRibbonFileInformation;
        private KryptonWrapLabel kwlblToolkitTitle;
        private KryptonWrapLabel kwlblToolkitFileInformation;
        private KryptonWrapLabel kwlblWorkspaceTitle;
        private KryptonWrapLabel kwlblWorkspaceFileInformation;
        private KryptonPanel kryptonPanel3;
        private PictureBox pbxIcon;
        private KryptonWrapLabel kwlblTitle;
    }
}