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
      this.m_btn_new = new System.Windows.Forms.Button();
      this.m_list = new System.Windows.Forms.ListView();
      this.m_col_name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.m_col_size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.m_col_category = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
      this.m_btn_copy = new System.Windows.Forms.Button();
      this.m_btn_delete = new System.Windows.Forms.Button();
      this.m_btn_props = new System.Windows.Forms.Button();
      this.m_btn_up = new System.Windows.Forms.Button();
      this.m_btn_down = new System.Windows.Forms.Button();
      this.m_text = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // m_btn_new
      // 
      this.m_btn_new.Location = new System.Drawing.Point(4, 3);
      this.m_btn_new.Name = "m_btn_new";
      this.m_btn_new.Size = new System.Drawing.Size(26, 26);
      this.m_btn_new.TabIndex = 0;
      this.m_btn_new.UseVisualStyleBackColor = true;
      this.m_btn_new.Click += new System.EventHandler(this.OnNewButtonClick);
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
      this.m_list.Location = new System.Drawing.Point(4, 56);
      this.m_list.MultiSelect = false;
      this.m_list.Name = "m_list";
      this.m_list.Size = new System.Drawing.Size(203, 233);
      this.m_list.TabIndex = 1;
      this.m_list.UseCompatibleStateImageBehavior = false;
      this.m_list.View = System.Windows.Forms.View.Details;
      this.m_list.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.OnAfterListViewLabelEdit);
      this.m_list.BeforeLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.OnBeforeListViewLabelEdit);
      this.m_list.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.OnListViewColumnClick);
      this.m_list.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.OnListMouseDoubleClick);
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
      // m_btn_copy
      // 
      this.m_btn_copy.Location = new System.Drawing.Point(30, 3);
      this.m_btn_copy.Name = "m_btn_copy";
      this.m_btn_copy.Size = new System.Drawing.Size(26, 26);
      this.m_btn_copy.TabIndex = 2;
      this.m_btn_copy.UseVisualStyleBackColor = true;
      this.m_btn_copy.Click += new System.EventHandler(this.OnCopyButtonClick);
      // 
      // m_btn_delete
      // 
      this.m_btn_delete.Location = new System.Drawing.Point(56, 3);
      this.m_btn_delete.Name = "m_btn_delete";
      this.m_btn_delete.Size = new System.Drawing.Size(26, 26);
      this.m_btn_delete.TabIndex = 3;
      this.m_btn_delete.UseVisualStyleBackColor = true;
      this.m_btn_delete.Click += new System.EventHandler(this.OnButtonDeleteClick);
      // 
      // m_btn_props
      // 
      this.m_btn_props.Location = new System.Drawing.Point(82, 3);
      this.m_btn_props.Name = "m_btn_props";
      this.m_btn_props.Size = new System.Drawing.Size(26, 26);
      this.m_btn_props.TabIndex = 4;
      this.m_btn_props.UseVisualStyleBackColor = true;
      this.m_btn_props.Click += new System.EventHandler(this.OnButtonPropsClick);
      // 
      // m_btn_up
      // 
      this.m_btn_up.Location = new System.Drawing.Point(108, 3);
      this.m_btn_up.Name = "m_btn_up";
      this.m_btn_up.Size = new System.Drawing.Size(26, 26);
      this.m_btn_up.TabIndex = 5;
      this.m_btn_up.UseVisualStyleBackColor = true;
      this.m_btn_up.Click += new System.EventHandler(this.OnButtonUpClick);
      // 
      // m_btn_down
      // 
      this.m_btn_down.Location = new System.Drawing.Point(134, 3);
      this.m_btn_down.Name = "m_btn_down";
      this.m_btn_down.Size = new System.Drawing.Size(26, 26);
      this.m_btn_down.TabIndex = 6;
      this.m_btn_down.UseVisualStyleBackColor = true;
      this.m_btn_down.Click += new System.EventHandler(this.OnButtonDownClick);
      // 
      // m_text
      // 
      this.m_text.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.m_text.Location = new System.Drawing.Point(4, 30);
      this.m_text.Name = "m_text";
      this.m_text.Size = new System.Drawing.Size(203, 20);
      this.m_text.TabIndex = 7;
      this.m_text.TextChanged += new System.EventHandler(this.OnTextBoxTextChanged);
      // 
      // LayoutsPanel
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.m_text);
      this.Controls.Add(this.m_btn_down);
      this.Controls.Add(this.m_btn_up);
      this.Controls.Add(this.m_btn_props);
      this.Controls.Add(this.m_btn_delete);
      this.Controls.Add(this.m_btn_copy);
      this.Controls.Add(this.m_list);
      this.Controls.Add(this.m_btn_new);
      this.Name = "LayoutsPanel";
      this.Size = new System.Drawing.Size(210, 292);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button m_btn_new;
    private System.Windows.Forms.ListView m_list;
    private System.Windows.Forms.ColumnHeader m_col_name;
    private System.Windows.Forms.ColumnHeader m_col_size;
    private System.Windows.Forms.ColumnHeader m_col_category;
    private System.Windows.Forms.Button m_btn_copy;
    private System.Windows.Forms.Button m_btn_delete;
    private System.Windows.Forms.Button m_btn_props;
    private System.Windows.Forms.Button m_btn_up;
    private System.Windows.Forms.Button m_btn_down;
    private System.Windows.Forms.TextBox m_text;
  }
}
