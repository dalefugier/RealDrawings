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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LayoutsPanel));
      this.m_list = new System.Windows.Forms.ListView();
      this.m_col_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.m_col_size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.m_col_category = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.m_text = new System.Windows.Forms.TextBox();
      this.m_toolstrip = new System.Windows.Forms.ToolStrip();
      this.m_button_new = new System.Windows.Forms.ToolStripButton();
      this.m_button_copy = new System.Windows.Forms.ToolStripButton();
      this.m_button_delete = new System.Windows.Forms.ToolStripButton();
      this.m_button_props = new System.Windows.Forms.ToolStripButton();
      this.m_toolstrip.SuspendLayout();
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
            this.m_col_category});
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
      this.m_list.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnListViewMouseDoubleClick);
      // 
      // m_col_name
      // 
      this.m_col_name.Text = "Name";
      this.m_col_name.Width = 40;
      // 
      // m_col_size
      // 
      this.m_col_size.Text = "Size";
      this.m_col_size.Width = 32;
      // 
      // m_col_category
      // 
      this.m_col_category.Text = "Details";
      this.m_col_category.Width = 127;
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
            this.m_button_props});
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
      this.m_button_new.ToolTipText = "New...";
      this.m_button_new.Click += new System.EventHandler(this.OnButtonNewClick);
      // 
      // m_button_copy
      // 
      this.m_button_copy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.m_button_copy.Image = ((System.Drawing.Image)(resources.GetObject("m_button_copy.Image")));
      this.m_button_copy.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.m_button_copy.Name = "m_button_copy";
      this.m_button_copy.Size = new System.Drawing.Size(23, 22);
      this.m_button_copy.ToolTipText = "Copy";
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
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.ListView m_list;
    private System.Windows.Forms.ColumnHeader m_col_name;
    private System.Windows.Forms.ColumnHeader m_col_size;
    private System.Windows.Forms.ColumnHeader m_col_category;
    private System.Windows.Forms.TextBox m_text;
    private System.Windows.Forms.ToolStrip m_toolstrip;
    private System.Windows.Forms.ToolStripButton m_button_new;
    private System.Windows.Forms.ToolStripButton m_button_copy;
    private System.Windows.Forms.ToolStripButton m_button_delete;
    private System.Windows.Forms.ToolStripButton m_button_props;
  }
}
