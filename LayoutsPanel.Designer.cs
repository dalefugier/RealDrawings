namespace RealDrawings
{
  partial class LayoutsPanel
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutsPanel));
      this.m_list = new System.Windows.Forms.ListView();
      this.m_col_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.m_col_size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.m_col_details = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.m_text = new System.Windows.Forms.TextBox();
      this.m_toolstrip = new System.Windows.Forms.ToolStrip();
      this.m_button_new = new System.Windows.Forms.ToolStripButton();
      this.m_button_copy = new System.Windows.Forms.ToolStripButton();
      this.m_button_delete = new System.Windows.Forms.ToolStripButton();
      this.m_button_props = new System.Windows.Forms.ToolStripButton();
      this.m_button_help = new System.Windows.Forms.ToolStripButton();
      this.m_menu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.m_menu_active = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.m_menu_copy = new System.Windows.Forms.ToolStripMenuItem();
      this.m_menu_delete = new System.Windows.Forms.ToolStripMenuItem();
      this.m_menu_rename = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.m_menu_properties = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.m_menu_new = new System.Windows.Forms.ToolStripMenuItem();
      this.m_toolstrip.SuspendLayout();
      this.m_menu.SuspendLayout();
      this.SuspendLayout();
      // 
      // m_list
      // 
      this.m_list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.m_col_name,
            this.m_col_size,
            this.m_col_details});
      this.m_list.FullRowSelect = true;
      this.m_list.HideSelection = false;
      this.m_list.LabelEdit = true;
      this.m_list.Location = new System.Drawing.Point(4, 54);
      this.m_list.Name = "m_list";
      this.m_list.Size = new System.Drawing.Size(203, 229);
      this.m_list.TabIndex = 1;
      this.m_list.UseCompatibleStateImageBehavior = false;
      this.m_list.View = System.Windows.Forms.View.Details;
      this.m_list.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.OnListViewAfterLabelEdit);
      this.m_list.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.OnListViewBeforeLabelEdit);
      this.m_list.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.OnListViewColumnClick);
      this.m_list.SelectedIndexChanged += new System.EventHandler(this.OnListViewSelectedIndexChanged);
      this.m_list.MouseClick += new System.Windows.Forms.MouseEventHandler(this.OnListViewMouseClick);
      this.m_list.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnListViewMouseDoubleClick);
      // 
      // m_col_name
      // 
      this.m_col_name.Text = "Name";
      this.m_col_name.Width = 100;
      // 
      // m_col_size
      // 
      this.m_col_size.Text = "Size";
      this.m_col_size.Width = 100;
      // 
      // m_col_details
      // 
      this.m_col_details.Text = "Details";
      this.m_col_details.Width = 25;
      // 
      // m_text
      // 
      this.m_text.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_text.Location = new System.Drawing.Point(3, 28);
      this.m_text.Name = "m_text";
      this.m_text.Size = new System.Drawing.Size(203, 20);
      this.m_text.TabIndex = 7;
      this.m_text.TextChanged += new System.EventHandler(this.OnTextBoxTextChanged);
      // 
      // m_toolstrip
      // 
      this.m_toolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.m_toolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_button_new,
            this.m_button_copy,
            this.m_button_delete,
            this.m_button_props,
            this.m_button_help});
      this.m_toolstrip.Location = new System.Drawing.Point(0, 0);
      this.m_toolstrip.Name = "m_toolstrip";
      this.m_toolstrip.Size = new System.Drawing.Size(210, 25);
      this.m_toolstrip.TabIndex = 8;
      this.m_toolstrip.Text = "toolStrip1";
      // 
      // m_button_new
      // 
      this.m_button_new.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_button_new.Image = ((System.Drawing.Image)(resources.GetObject("m_button_new.Image")));
      this.m_button_new.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_button_new.Name = "m_button_new";
      this.m_button_new.Size = new System.Drawing.Size(23, 22);
      this.m_button_new.ToolTipText = "New Layout...";
      this.m_button_new.Click += new System.EventHandler(this.OnButtonNewClick);
      // 
      // m_button_copy
      // 
      this.m_button_copy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_button_copy.Image = ((System.Drawing.Image)(resources.GetObject("m_button_copy.Image")));
      this.m_button_copy.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_button_copy.Name = "m_button_copy";
      this.m_button_copy.Size = new System.Drawing.Size(23, 22);
      this.m_button_copy.ToolTipText = "Copy Layout";
      this.m_button_copy.Click += new System.EventHandler(this.OnButtonCopyClick);
      // 
      // m_button_delete
      // 
      this.m_button_delete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_button_delete.Image = ((System.Drawing.Image)(resources.GetObject("m_button_delete.Image")));
      this.m_button_delete.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_button_delete.Name = "m_button_delete";
      this.m_button_delete.Size = new System.Drawing.Size(23, 22);
      this.m_button_delete.ToolTipText = "Delete";
      this.m_button_delete.Click += new System.EventHandler(this.OnButtonDeleteClick);
      // 
      // m_button_props
      // 
      this.m_button_props.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_button_props.Image = ((System.Drawing.Image)(resources.GetObject("m_button_props.Image")));
      this.m_button_props.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_button_props.Name = "m_button_props";
      this.m_button_props.Size = new System.Drawing.Size(23, 22);
      this.m_button_props.ToolTipText = "Properties...";
      this.m_button_props.Click += new System.EventHandler(this.OnButtonPropsClick);
      // 
      // m_button_help
      // 
      this.m_button_help.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_button_help.Image = ((System.Drawing.Image)(resources.GetObject("m_button_help.Image")));
      this.m_button_help.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_button_help.Name = "m_button_help";
      this.m_button_help.Size = new System.Drawing.Size(23, 22);
      this.m_button_help.ToolTipText = "Help";
      this.m_button_help.Click += new System.EventHandler(this.OnButtonHelpClick);
      // 
      // m_menu
      // 
      this.m_menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_menu_active,
            this.toolStripSeparator1,
            this.m_menu_copy,
            this.m_menu_delete,
            this.m_menu_rename,
            this.toolStripSeparator2,
            this.m_menu_properties,
            this.toolStripSeparator3,
            this.m_menu_new});
      this.m_menu.Name = "m_menu";
      this.m_menu.Size = new System.Drawing.Size(147, 154);
      this.m_menu.Opening += new System.ComponentModel.CancelEventHandler(this.OnMenuOpening);
      // 
      // m_menu_active
      // 
      this.m_menu_active.Name = "m_menu_active";
      this.m_menu_active.Size = new System.Drawing.Size(146, 22);
      this.m_menu_active.Text = "Set Current";
      this.m_menu_active.Click += new System.EventHandler(this.OnMenuActiveClick);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(143, 6);
      // 
      // m_menu_copy
      // 
      this.m_menu_copy.Name = "m_menu_copy";
      this.m_menu_copy.Size = new System.Drawing.Size(146, 22);
      this.m_menu_copy.Text = "Copy Layout";
      this.m_menu_copy.Click += new System.EventHandler(this.OnMenuCopyClick);
      // 
      // m_menu_delete
      // 
      this.m_menu_delete.Name = "m_menu_delete";
      this.m_menu_delete.Size = new System.Drawing.Size(146, 22);
      this.m_menu_delete.Text = "Delete";
      this.m_menu_delete.Click += new System.EventHandler(this.OnMenuDeleteClick);
      // 
      // m_menu_rename
      // 
      this.m_menu_rename.Name = "m_menu_rename";
      this.m_menu_rename.Size = new System.Drawing.Size(146, 22);
      this.m_menu_rename.Text = "Rename";
      this.m_menu_rename.Click += new System.EventHandler(this.OnMenuRenameClick);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(143, 6);
      // 
      // m_menu_properties
      // 
      this.m_menu_properties.Name = "m_menu_properties";
      this.m_menu_properties.Size = new System.Drawing.Size(146, 22);
      this.m_menu_properties.Text = "Properties...";
      this.m_menu_properties.Click += new System.EventHandler(this.OnmMenuPropertiesClick);
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(143, 6);
      // 
      // m_menu_new
      // 
      this.m_menu_new.Name = "m_menu_new";
      this.m_menu_new.Size = new System.Drawing.Size(146, 22);
      this.m_menu_new.Text = "New Layout...";
      this.m_menu_new.Click += new System.EventHandler(this.OnMenuNewClick);
      // 
      // LayoutsPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_toolstrip);
      this.Controls.Add(this.m_text);
      this.Controls.Add(this.m_list);
      this.Name = "LayoutsPanel";
      this.Size = new System.Drawing.Size(210, 286);
      this.m_toolstrip.ResumeLayout(false);
      this.m_toolstrip.PerformLayout();
      this.m_menu.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.ListView m_list;
    private System.Windows.Forms.ColumnHeader m_col_name;
    private System.Windows.Forms.ColumnHeader m_col_size;
    private System.Windows.Forms.ColumnHeader m_col_details;
    private System.Windows.Forms.TextBox m_text;
    private System.Windows.Forms.ToolStrip m_toolstrip;
    private System.Windows.Forms.ToolStripButton m_button_new;
    private System.Windows.Forms.ToolStripButton m_button_copy;
    private System.Windows.Forms.ToolStripButton m_button_delete;
    private System.Windows.Forms.ToolStripButton m_button_props;
    private System.Windows.Forms.ToolStripButton m_button_help;
    private System.Windows.Forms.ContextMenuStrip m_menu;
    private System.Windows.Forms.ToolStripMenuItem m_menu_active;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem m_menu_copy;
    private System.Windows.Forms.ToolStripMenuItem m_menu_delete;
    private System.Windows.Forms.ToolStripMenuItem m_menu_rename;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripMenuItem m_menu_properties;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripMenuItem m_menu_new;
  }
}
