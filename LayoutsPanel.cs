using System;
using System.Linq;
using System.Windows.Forms;
using Rhino;
using Rhino.UI;
using Rhino.Display;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RealDrawings
{
  [Guid("29C790A7-1AE9-4350-900F-40AA54EA5AA0")]
  public partial class LayoutsPanel : UserControl, IPanel, IHelp
  {
    public static Guid PanelId => typeof(LayoutsPanel).GUID;

    private readonly ListViewItemSorter m_item_sorter;
    private readonly Font m_font_regular;
    private readonly Font m_font_bold;
    private bool m_events_hooked;

    private enum LayoutEvent
    {
      /// <summary>
      /// No panel activity in progress
      /// </summary>
      None,
      /// <summary>
      /// File opening in progress
      /// </summary>
      Open,
      /// <summary>
      /// New layout in progress
      /// </summary>
      New,
      /// <summary>
      /// Delete layout in progress
      /// </summary>
      Delete
    }

    private LayoutEvent m_current_event = LayoutEvent.None;

    /// <summary>
    /// Public constructor
    /// </summary>
    public LayoutsPanel()
    {
      InitializeComponent();
      m_list.DoubleBuffered(true);

      // Toolbar icons
      var icon_size = SystemInformation.SmallIconSize.Width;
      m_button_new.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.New.ico", icon_size);
      m_button_copy.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Copy.ico", icon_size);
      m_button_delete.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Delete.ico", icon_size);
      m_button_props.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Props.ico", icon_size);
      m_button_help.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Help.ico", icon_size);

      // ListView image
      var image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Panel.ico", icon_size);
      var image_list = new ImageList();
      image_list.Images.Add("0", image);
      m_list.SmallImageList = image_list;
      

      // ListView item sorter
      m_item_sorter = new ListViewItemSorter();
      m_list.ListViewItemSorter = m_item_sorter;

      // Regular and bold ListView font
      m_font_regular = new Font(m_list.Font, FontStyle.Regular);
      m_font_bold = new Font(m_list.Font, FontStyle.Bold);

      // Sneaky way to add a cue banner to the text box
      SendMessage(m_text.Handle, EM_SETCUEBANNER, 1, "Search");

      // Fill the ListView
      FillListView();

      // Enable the toolbar buttons
      SetButtonsEnabled(0);

      // Hook some Rhino events
      HookRhinoEvents();
    }

    /// <summary>
    /// Hook up some Rhino event handlers
    /// </summary>
    public void HookRhinoEvents()
    {
      if (!m_events_hooked)
      {
        RhinoDoc.EndOpenDocument += OnEndOpenDocument;
        RhinoView.Create += OnCreateViewEventHandler;
        RhinoView.Rename += OnRenameViewEventHandler;
        RhinoView.SetActive += OnSetActiveViewEventHandler;
        RhinoView.Destroy += OnDestroyViewEventHandler;
        m_events_hooked = true;
      }
    }

    /// <summary>
    /// Unhook up some Rhino event handlers
    /// </summary>
    public void UnhookRhinoEvents()
    {
      if (m_events_hooked)
      {
        RhinoDoc.EndOpenDocument -= OnEndOpenDocument;
        RhinoView.Create -= OnCreateViewEventHandler;
        RhinoView.Rename -= OnRenameViewEventHandler;
        RhinoView.SetActive -= OnSetActiveViewEventHandler;
        RhinoView.Destroy -= OnDestroyViewEventHandler;
        m_events_hooked = false;
      }
    }

    /// <summary>
    /// RhinoView.Create event handler
    /// </summary>
    private void OnCreateViewEventHandler(object sender, ViewEventArgs e)
    {
      if (m_current_event == LayoutEvent.None)
      {
        if (e.View is RhinoPageView)
          FillListView();
      }
    }

    /// <summary>
    /// RhinoView.Rename event handler
    /// </summary>
    private void OnRenameViewEventHandler(object sender, ViewEventArgs e)
    {
      if (m_current_event == LayoutEvent.None)
      {
        if (e.View is RhinoPageView)
          FillListView();
      }
    }

    /// <summary>
    /// RhinoView.Destroy event handler
    /// </summary>
    private void OnDestroyViewEventHandler(object sender, ViewEventArgs e)
    {
      if (m_current_event == LayoutEvent.None)
      {
        if (e.View is RhinoPageView)
          FillListView();
      }
    }

    /// <summary>
    /// RhinoView.SetActive event handler
    /// </summary>
    private void OnSetActiveViewEventHandler(object sender, ViewEventArgs e)
    {
      if (m_current_event != LayoutEvent.None)
        return;

      if (e.View is RhinoPageView page_view)
      {
        var serial_number = page_view.RuntimeSerialNumber;

        m_list.BeginUpdate();

        m_list.SelectedItems.Clear();

        var selected_index = -1;

        foreach (ListViewItem item in m_list.Items)
        {
          if ((uint)item.Tag == serial_number)
          {
            item.Selected = true;
            item.Font = m_font_bold;
            selected_index = item.Index;
          }
          else
          {
            item.Font = m_font_regular;
          }
        }

        if (selected_index >= 0)
          m_list.EnsureVisible(selected_index);

        m_list.EndUpdate();
      }
    }

    /// <summary>
    /// RhinoDoc.EndOpenDocument event handler
    /// </summary>
    private void OnEndOpenDocument(object sender, DocumentOpenEventArgs e)
    {
      m_current_event = LayoutEvent.Open;
      m_text.Text = "";
      RhinoApp.Idle += OnIdle;
    }

    /// <summary>
    /// RhinoApp.Idle event handler
    /// </summary>
    private void OnIdle(object sender, EventArgs e)
    {
      m_current_event = LayoutEvent.None;
      RhinoApp.Idle -= OnIdle;
      FillListView();
    }

    /// <summary>
    /// FillListview
    /// </summary>
    private void FillListView()
    {
      // Save the sizes of the columns
      var column_sizes = new List<int>();
      foreach (ColumnHeader column in m_list.Columns)
        column_sizes.Add(column.Width);

      var seleccted_index = -1;

      // Here we go
      m_list.BeginUpdate();

      // Clear the list
      m_list.Items.Clear();

      var doc = RhinoDoc.ActiveDoc;
      if (null != doc)
      {
        var view_sn = ActiveLayout;
        var page_views = doc.Views.GetPageViews();
        foreach (var view in page_views)
        {
          var arr = new string[3];
          arr[0] = view.PageName;
          arr[1] = $"{view.PageWidth} x {view.PageHeight}";

          var count = 0;
          var details = view.GetDetailViews();
          if (null != details)
            count = view.GetDetailViews().Length;
          arr[2] = count.ToString();

          var item = new ListViewItem(arr)
          {
            Tag = view.RuntimeSerialNumber,
            ImageIndex = 0
          };

          // Is current page view?
          if (view.RuntimeSerialNumber == view_sn)
          {
            item.Font = m_font_bold;
            item.Selected = true;
          }

          var new_item = m_list.Items.Add(item);

          if (new_item.Selected)
            seleccted_index = new_item.Index;
        }
      }

      // Restore column widths
      for (var i = 0; i < m_list.Columns.Count; i++)
        m_list.Columns[i].Width = column_sizes[i];
      m_list.Columns[m_list.Columns.Count - 1].Width = -2;

      m_list.Sort();

      if (seleccted_index >= 0)
        m_list.EnsureVisible(seleccted_index);

      m_list.EndUpdate();
    }

    #region IPanel interface

    /// <summary>
    /// The panel is being closed
    /// </summary>
    public void PanelClosing(uint documentSerialNumber, bool onCloseDocument)
    {
      UnhookRhinoEvents();
    }

    /// <summary>
    /// The panel is being hidden
    /// </summary>
    public void PanelHidden(uint documentSerialNumber, ShowPanelReason reason)
    {
      UnhookRhinoEvents();
    }

    /// <summary>
    /// The panel is being shown
    /// </summary>
    public void PanelShown(uint documentSerialNumber, ShowPanelReason reason)
    {
      FillListView();
      HookRhinoEvents();
    }

    #endregion

    #region IHelp inteface

    public string HelpUrl => "https://github.com/dalefugier/RealDrawings";

    #endregion

    /// <summary>
    /// Gets the runtime serial number of the active page view
    /// </summary>
    private uint ActiveLayout
    {
      get
      {
        var doc = RhinoDoc.ActiveDoc;
        var view = doc?.Views.ActiveView;
        if (view != null)
        {
          if (view is RhinoPageView page_view)
            return page_view.RuntimeSerialNumber;
        }
        return 0;
      }
    }

    /// <summary>
    /// Sets the active page view, given a runtime serial number
    /// </summary>
    private void SetActiveLayout(uint runtimeSerialNumber)
    {
      var doc = RhinoDoc.ActiveDoc;
      if (null != doc)
      {
        foreach (var view in doc.Views.GetPageViews())
        {
          if (view.RuntimeSerialNumber == runtimeSerialNumber)
          {
            doc.Views.ActiveView = view;
            doc.Views.Redraw();
            break;
          }
        }
      }
    }

    /// <summary>
    /// OnListViewMouseDoubleClick
    /// </summary>
    private void OnListViewMouseDoubleClick(object sender, MouseEventArgs e)
    {
      var doc = RhinoDoc.ActiveDoc;
      if (null == doc)
        return;

      var info = ((ListView)sender).HitTest(e.X, e.Y);
      var item = info.Item;
      if (null != item)
        SetActiveLayout((uint)item.Tag);
    }

    /// <summary>
    /// OnListViewBeforeLabelEdit
    /// </summary>
    private void OnListViewBeforeLabelEdit(object sender, LabelEditEventArgs e)
    {
    }

    /// <summary>
    /// OnListViewAfterLabelEdit
    /// </summary>
    private void OnListViewAfterLabelEdit(object sender, LabelEditEventArgs e)
    {
      if (string.IsNullOrEmpty(e.Label))
      {
        e.CancelEdit = true;
        return;
      }

      var label = e.Label.Trim();
      if (string.IsNullOrEmpty(label))
      {
        e.CancelEdit = true;
        return;
      }

      if (!Rhino.DocObjects.Layer.IsValidName(label))
      {
        e.CancelEdit = true;
        return;
      }

      var doc = RhinoDoc.ActiveDoc;
      if (null == doc)
      {
        e.CancelEdit = true;
        return;
      }

      var index = e.Item;
      var item = m_list.Items[index];
      if (null != item)
      {
        var sn = (uint)item.Tag;
        foreach (var view in doc.Views.GetPageViews())
        {
          if (view.RuntimeSerialNumber == sn)
          {
            view.PageName = label;
            doc.Views.Redraw();
            break;
          }
        }
      }
    }

    /// <summary>
    /// OnListViewSelectedIndexChanged
    /// </summary>
    private void OnListViewSelectedIndexChanged(object sender, EventArgs e)
    {
      SetButtonsEnabled(m_list.SelectedItems.Count);
    }

    /// <summary>
    /// SetButtonsEnabled
    /// </summary>
    private void SetButtonsEnabled(int selectedCount)
    {
      if (selectedCount <= 0)
      {
        m_button_copy.Enabled = false;
        m_button_delete.Enabled = false;
        m_button_props.Enabled = false;
      }
      else if (selectedCount == 1)
      {
        m_button_copy.Enabled = true;
        m_button_delete.Enabled = true;
        m_button_props.Enabled = true;
      }
      else if (selectedCount > 1)
      {
        m_button_copy.Enabled = false;
        m_button_delete.Enabled = true;
        m_button_props.Enabled = false;
      }
    }

    /// <summary>
    /// OnListViewColumnClick
    /// </summary>
    private void OnListViewColumnClick(object sender, ColumnClickEventArgs e)
    {
      if (e.Column == m_item_sorter.SortColumn)
      {
        m_item_sorter.Order = m_item_sorter.Order == SortOrder.Ascending
          ? SortOrder.Descending
          : SortOrder.Ascending;
      }
      else
      {

        m_item_sorter.SortColumn = e.Column;
        m_item_sorter.Order = SortOrder.Ascending;
      }

      m_list.Sort();
    }

    /// <summary>
    /// OnTextBoxTextChanged
    /// </summary>
    private void OnTextBoxTextChanged(object sender, EventArgs e)
    {
      if (m_current_event == LayoutEvent.Open)
        return;

      var doc = RhinoDoc.ActiveDoc;
      var views = doc?.Views.GetPageViews();
      if (views == null || views.Length == 0)
        return;

      var text = m_text.Text.Trim();
      if (string.IsNullOrEmpty(text))
      {
        FillListView();
        return;
      }

      m_list.Items.Clear();

      foreach (var view in views.Where(v => v.PageName.ToLower().Contains(text.ToLower())))
      {
        var arr = new string[3];
        arr[0] = view.PageName;
        arr[1] = $"{view.PageWidth} x {view.PageHeight}";

        var count = 0;
        var details = view.GetDetailViews();
        if (null != details)
          count = view.GetDetailViews().Length;
        arr[2] = count.ToString();

        var item = new ListViewItem(arr)
        {
          Tag = view.RuntimeSerialNumber,
          ImageIndex = 0
        };
        m_list.Items.Add(item);
      }
    }

    /// <summary>
    /// OnButtonNewClick
    /// </summary>
    private void OnButtonNewClick(object sender, EventArgs e)
    {
      m_current_event = LayoutEvent.New;
      RhinoApp.RunScript("_Layout", false);
      FillListView();
      m_current_event = LayoutEvent.None;
    }

    /// <summary>
    /// OnButtonCopyClick
    /// </summary>
    private void OnButtonCopyClick(object sender, EventArgs e)
    {
      if (0 == m_list.SelectedItems.Count)
        return;

      var selected = m_list.SelectedItems[0];
      if (null != selected)
      {
        SetActiveLayout((uint)selected.Tag);
        RhinoApp.RunScript("_CopyLayout", false);
        FillListView();
      }
    }

    /// <summary>
    /// OnButtonDeleteClick
    /// </summary>
    private void OnButtonDeleteClick(object sender, EventArgs e)
    {
      var count = m_list.SelectedItems.Count;
      if (0 == count)
        return;

      var message = 1 == count
        ? "Permanently deleted the selected layout?"
        : "Permanently deleted the selected layouts?";

      var title = 1 == count
        ? "Delete Layout"
        : "Delete Layouts";

      var rc = Dialogs.ShowMessage(message, title, ShowMessageButton.YesNo, ShowMessageIcon.Warning);
      if (rc == ShowMessageResult.No)
        return;

      m_current_event = LayoutEvent.Delete;

      foreach (ListViewItem item in m_list.SelectedItems)
      {
        SetActiveLayout((uint)item.Tag);
        RhinoApp.Wait();
        RhinoApp.RunScript("_-CloseViewport _Yes", false);
        RhinoApp.Wait();
      }

      FillListView();

      m_current_event = LayoutEvent.None;
    }

    /// <summary>
    /// OnButtonPropsClick
    /// </summary>
    private void OnButtonPropsClick(object sender, EventArgs e)
    {
      if (0 == m_list.SelectedItems.Count)
        return;

      ListViewItem selected = m_list.SelectedItems[0];
      if (null != selected)
      {
        SetActiveLayout((uint)selected.Tag);
        RhinoApp.RunScript("_LayoutProperties", false);
        FillListView();
      }
    }

    /// <summary>
    /// OnButtonHelpClick
    /// </summary>
    private void OnButtonHelpClick(object sender, EventArgs e)
    {
      Process.Start("https://github.com/dalefugier/RealDrawings");
    }

    /// <summary>
    /// SendMessage Win32 wrapper
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);
    private const int EM_SETCUEBANNER = 0x1501;

    /// <summary>
    /// OnMenuOpening
    /// </summary>
    private void OnMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
    {
      var selected_count = m_list.SelectedItems.Count;
      if (selected_count <= 0)
      {
        m_menu_active.Enabled = false;
        m_menu_copy.Enabled = false;
        m_menu_delete.Enabled = false;
        m_menu_rename.Enabled = false;
        m_button_copy.Enabled = false;
        m_menu_properties.Enabled = false;
      }
      else if (selected_count == 1)
      {
        m_menu_active.Enabled = true;
        m_menu_copy.Enabled = true;
        m_menu_delete.Enabled = true;
        m_menu_rename.Enabled = true;
        m_button_copy.Enabled = true;
        m_menu_properties.Enabled = true;
      }
      else if (selected_count > 1)
      {
        m_menu_active.Enabled = false;
        m_menu_copy.Enabled = false;
        m_menu_delete.Enabled = true;
        m_menu_rename.Enabled = false;
        m_button_copy.Enabled = false;
        m_menu_properties.Enabled = false;
      }
    }

    private void OnListViewMouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        var pt = m_list.PointToScreen(e.Location);
        m_menu.Show(pt);
      }
    }

    private void OnMenuActiveClick(object sender, EventArgs e)
    {
      if (0 == m_list.SelectedItems.Count)
        return;

      var selected = m_list.SelectedItems[0];
      if (null != selected)
        SetActiveLayout((uint)selected.Tag);
    }

    private void OnMenuCopyClick(object sender, EventArgs e)
    {
      if (0 == m_list.SelectedItems.Count)
        return;

      var selected = m_list.SelectedItems[0];
      if (null != selected)
      {
        SetActiveLayout((uint)selected.Tag);
        RhinoApp.RunScript("_CopyLayout", false);
        FillListView();
      }
    }

    private void OnMenuDeleteClick(object sender, EventArgs e)
    {
      var count = m_list.SelectedItems.Count;
      if (0 == count)
        return;

      var message = 1 == count
        ? "Permanently deleted the selected layout?"
        : "Permanently deleted the selected layouts?";

      var title = 1 == count
        ? "Delete Layout"
        : "Delete Layouts";

      var rc = Dialogs.ShowMessage(message, title, ShowMessageButton.YesNo, ShowMessageIcon.Warning);
      if (rc == ShowMessageResult.No)
        return;

      m_current_event = LayoutEvent.Delete;

      foreach (ListViewItem item in m_list.SelectedItems)
      {
        SetActiveLayout((uint)item.Tag);
        RhinoApp.Wait();
        RhinoApp.RunScript("_-CloseViewport _Yes", false);
        RhinoApp.Wait();
      }

      FillListView();

      m_current_event = LayoutEvent.None;

    }

    private void OnmMenuPropertiesClick(object sender, EventArgs e)
    {
      if (0 == m_list.SelectedItems.Count)
        return;

      var selected = m_list.SelectedItems[0];
      if (null != selected)
      {
        SetActiveLayout((uint)selected.Tag);
        RhinoApp.RunScript("_LayoutProperties", false);
        FillListView();
      }
    }

    private void OnMenuNewClick(object sender, EventArgs e)
    {
      m_current_event = LayoutEvent.New;
      RhinoApp.RunScript("_Layout", false);
      FillListView();
      m_current_event = LayoutEvent.None;
    }

    private void OnMenuRenameClick(object sender, EventArgs e)
    {
      if (0 == m_list.SelectedItems.Count)
        return;

      var selected = m_list.SelectedItems[0];
      selected?.BeginEdit();
    }
  }


  public class ListViewItemSorter : IComparer
  {
    /// <summary>
    /// Public constructor
    /// </summary>
    public ListViewItemSorter()
    {
      SortColumn = 0;
      Order = SortOrder.None;
    }

    /// <summary>
    /// IComparer.Compare definition
    /// </summary>
    public int Compare(object x, object y)
    {
      var item_x = (ListViewItem)x;
      var item_y = (ListViewItem)y;

      int rc;
      if (null == item_x && null == item_y)
        rc = 0;
      else if (null == item_y)
        rc = -1;
      else if (null == item_x)
        rc = 1;
      else
        rc = StrCmpLogicalW(item_x.SubItems[SortColumn].Text, item_y.SubItems[SortColumn].Text);

      if (Order == SortOrder.Ascending)
        return rc;
      if (Order == SortOrder.Descending)
        return -rc;

      return 0;
    }

    /// <summary>
    /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
    /// </summary>
    public int SortColumn { set; get; }

    /// <summary>
    /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
    /// </summary>
    public SortOrder Order { set; get; }

    /// <summary>
    /// StrCmpLogicalW Win32 wrapper
    /// </summary>
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string psz1, string psz2);
  }

  /// <summary>
  /// Fancy extension methods
  /// </summary>
  public static class ControlExtensions
  {
    public static void DoubleBuffered(this Control control, bool enable)
    {
      var info = control.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
      if (null != info)
        info.SetValue(control, enable, null);
    }
  }
}
