namespace ZDevTools.ServiceConsole
{
    partial class ScheduleManageForm
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
            this.lvScheduleManage = new System.Windows.Forms.ListView();
            this.chTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chEnabled = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bAdd = new System.Windows.Forms.Button();
            this.bEdit = new System.Windows.Forms.Button();
            this.bDel = new System.Windows.Forms.Button();
            this.bCancel = new System.Windows.Forms.Button();
            this.bOK = new System.Windows.Forms.Button();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // lvScheduleManage
            // 
            this.lvScheduleManage.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chTitle,
            this.chDescription,
            this.chEnabled});
            this.lvScheduleManage.Dock = System.Windows.Forms.DockStyle.Top;
            this.lvScheduleManage.FullRowSelect = true;
            this.lvScheduleManage.HideSelection = false;
            this.lvScheduleManage.Location = new System.Drawing.Point(0, 0);
            this.lvScheduleManage.Name = "lvScheduleManage";
            this.lvScheduleManage.Size = new System.Drawing.Size(724, 369);
            this.lvScheduleManage.TabIndex = 0;
            this.lvScheduleManage.UseCompatibleStateImageBehavior = false;
            this.lvScheduleManage.View = System.Windows.Forms.View.Details;
            this.lvScheduleManage.SelectedIndexChanged += new System.EventHandler(this.lvScheduleManage_SelectedIndexChanged);
            this.lvScheduleManage.DoubleClick += new System.EventHandler(this.lvScheduleManage_DoubleClick);
            // 
            // chTitle
            // 
            this.chTitle.Text = "计划类型";
            this.chTitle.Width = 128;
            // 
            // chDescription
            // 
            this.chDescription.Text = "详细信息";
            this.chDescription.Width = 458;
            // 
            // chEnabled
            // 
            this.chEnabled.Text = "状态";
            this.chEnabled.Width = 95;
            // 
            // bAdd
            // 
            this.bAdd.Location = new System.Drawing.Point(14, 382);
            this.bAdd.Name = "bAdd";
            this.bAdd.Size = new System.Drawing.Size(75, 23);
            this.bAdd.TabIndex = 1;
            this.bAdd.Text = "新建";
            this.bAdd.UseVisualStyleBackColor = true;
            this.bAdd.Click += new System.EventHandler(this.bAdd_Click);
            // 
            // bEdit
            // 
            this.bEdit.Enabled = false;
            this.bEdit.Location = new System.Drawing.Point(95, 382);
            this.bEdit.Name = "bEdit";
            this.bEdit.Size = new System.Drawing.Size(75, 23);
            this.bEdit.TabIndex = 2;
            this.bEdit.Text = "编辑";
            this.bEdit.UseVisualStyleBackColor = true;
            this.bEdit.Click += new System.EventHandler(this.bEdit_Click);
            // 
            // bDel
            // 
            this.bDel.Enabled = false;
            this.bDel.Location = new System.Drawing.Point(176, 382);
            this.bDel.Name = "bDel";
            this.bDel.Size = new System.Drawing.Size(75, 23);
            this.bDel.TabIndex = 3;
            this.bDel.Text = "删除";
            this.bDel.UseVisualStyleBackColor = true;
            this.bDel.Click += new System.EventHandler(this.bDel_Click);
            // 
            // bCancel
            // 
            this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bCancel.Location = new System.Drawing.Point(640, 382);
            this.bCancel.Name = "bCancel";
            this.bCancel.Size = new System.Drawing.Size(75, 23);
            this.bCancel.TabIndex = 5;
            this.bCancel.Text = "取消";
            this.bCancel.UseVisualStyleBackColor = true;
            // 
            // bOK
            // 
            this.bOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.bOK.Location = new System.Drawing.Point(559, 382);
            this.bOK.Name = "bOK";
            this.bOK.Size = new System.Drawing.Size(75, 23);
            this.bOK.TabIndex = 4;
            this.bOK.Text = "确定";
            this.bOK.UseVisualStyleBackColor = true;
            // 
            // ScheduleManageForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(724, 417);
            this.Controls.Add(this.bCancel);
            this.Controls.Add(this.bOK);
            this.Controls.Add(this.bDel);
            this.Controls.Add(this.bEdit);
            this.Controls.Add(this.bAdd);
            this.Controls.Add(this.lvScheduleManage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScheduleManageForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "计划管理";
            this.Load += new System.EventHandler(this.ScheduleManageForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvScheduleManage;
        private System.Windows.Forms.ColumnHeader chTitle;
        private System.Windows.Forms.ColumnHeader chDescription;
        private System.Windows.Forms.ColumnHeader chEnabled;
        private System.Windows.Forms.Button bAdd;
        private System.Windows.Forms.Button bEdit;
        private System.Windows.Forms.Button bDel;
        private System.Windows.Forms.Button bCancel;
        private System.Windows.Forms.Button bOK;
        private System.Windows.Forms.BindingSource bindingSource1;
    }
}