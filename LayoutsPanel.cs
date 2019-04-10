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
  /// <summary>
  /// Layouts panel as a UserControl
  /// </summary>
  [Guid("29C790A7-1AE9-4350-900F-40AA54EA5AA0")]
  public partial class LayoutsPanel : UserControl, IPanel, IHelp
  {
    public static Guid PanelId => typeof(LayoutsPanel).GUID;

    private readonly ListViewItemSorter m_item_sorter;
    private readonly Font m_font_regular;
    private readonly Font m_font_bold;
    private bool m_events_hooked;
    private string m_filter;

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
      m_button_properties.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Props.ico", icon_size);
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


    #region Helper functions

    /// <summary>
    /// FillListview
    /// </summary>
    private void FillListView()
    {
      // Save the sizes of the columns
      var column_sizes = new List<int>();
      foreach (ColumnHeader column in m_list.Columns)
        column_sizes.Add(column.Width);

      var selected_index = -1;

      // Here we go
      m_list.BeginUpdate();

      // Clear the list
      m_list.Items.Clear();

      var doc = RhinoDoc.ActiveDoc;
      if (null != doc)
      {
        // Active layout's runtime serial number (can be 0)
        var sn = ActiveLayoutSerialNumber;

        // Get the document page views
        var page_views = doc.Views.GetPageViews();

        if (string.IsNullOrEmpty(m_filter))
        {
          // Process all document layouts
          foreach (var view in page_views)
          {
            var item = PageViewItem(view, view.RuntimeSerialNumber == sn);
            if (null != item)
            {
              var new_item = m_list.Items.Add(item);
              if (new_item.Selected)
                selected_index = new_item.Index;
            }
          }
        }
        else
        {
          // Find the page views that contain the text in the name
          foreach (var view in page_views.Where(v => v.PageName.ToLower().Contains(m_filter.ToLower())))
          {
            var item = PageViewItem(view, view.RuntimeSerialNumber == sn);
            var new_item = m_list.Items.Add(item);
            if (new_item.Selected)
              selected_index = new_item.Index;
          }
        }
      }

      // Restore column widths
      for (var i = 0; i < m_list.Columns.Count; i++)
        m_list.Columns[i].Width = column_sizes[i];
      m_list.Columns[m_list.Columns.Count - 1].Width = -2;

      m_list.Sort();

      if (selected_index >= 0)
        m_list.EnsureVisible(selected_index);

      m_list.EndUpdate();
    }

    /// <summary>
    /// Creates a ListViewItem from a RhinoPageView
    /// </summary>
    private ListViewItem PageViewItem(RhinoPageView view, bool active)
    {
      if (null == view)
        return null;

      var detail_count = 0;
      var details = view.GetDetailViews();
      if (null != details)
        detail_count = view.GetDetailViews().Length;

      var arr = new string[3];
      arr[0] = view.PageName;
      arr[1] = $"{view.PageWidth} x {view.PageHeight}";
      arr[2] = $"{detail_count}";

      var item = new ListViewItem(arr) { ImageIndex = 0, Tag = view.RuntimeSerialNumber };
      if (active)
      {
        item.Font = m_font_bold;
        item.Selected = true;
      }

      return item;
    }

    /// <summary>
    /// Enables and disabled toolstrip buttons
    /// </summary>
    private void SetButtonsEnabled(int selectedCount)
    {
      if (selectedCount <= 0)
      {
        m_button_copy.Enabled = false;
        m_button_delete.Enabled = false;
        m_button_properties.Enabled = false;
      }
      else if (selectedCount == 1)
      {
        m_button_copy.Enabled = true;
        m_button_delete.Enabled = true;
        m_button_properties.Enabled = true;
      }
      else if (selectedCount > 1)
      {
        m_button_copy.Enabled = false;
        m_button_delete.Enabled = true;
        m_button_properties.Enabled = false;
      }
    }

    /// <summary>
    /// Gets the runtime serial number of the active page view
    /// </summary>
    private static uint ActiveLayoutSerialNumber
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
    private static void DoActiveLayout(uint serialNumber)
    {
      var doc = RhinoDoc.ActiveDoc;
      if (null != doc)
      {
        foreach (var view in doc.Views.GetPageViews())
        {
          if (view.RuntimeSerialNumber == serialNumber)
          {
            doc.Views.ActiveView = view;
            doc.Views.Redraw();
            break;
          }
        }
      }
    }

    /// <summary>
    /// Creates a new layout
    /// </summary>
    private void DoNewLayout()
    {
      m_current_event = LayoutEvent.New;
      RhinoApp.RunScript("_Layout", false);
      FillListView();
      m_current_event = LayoutEvent.None;
    }

    /// <summary>
    /// Copies the currently selected layout
    /// </summary>
    private void DoCopyLayout()
    {
      if (0 == m_list.SelectedItems.Count)
        return;

      var selected = m_list.SelectedItems[0];
      if (null != selected)
      {
        DoActiveLayout((uint)selected.Tag);
        RhinoApp.RunScript("_CopyLayout", false);
        FillListView();
      }
    }

    /// <summary>
    /// Deletes the currently selected layouts
    /// </summary>
    private void DoDeleteLayout()
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
        DoActiveLayout((uint)item.Tag);
        RhinoApp.Wait();
        RhinoApp.RunScript("_-CloseViewport _Yes", false);
        RhinoApp.Wait();
      }

      FillListView();

      m_current_event = LayoutEvent.None;
    }

    /// <summary>
    /// Renames the currently selected layout
    /// </summary>
    private void DoRenameLayout()
    {
      if (0 == m_list.SelectedItems.Count)
        return;

      var selected = m_list.SelectedItems[0];
      selected?.BeginEdit();
    }

    /// <summary>
    /// Show the layout properties user interface
    /// </summary>
    private void DoLayoutProperties()
    {
      if (0 == m_list.SelectedItems.Count)
        return;

      var selected = m_list.SelectedItems[0];
      if (null != selected)
      {
        DoActiveLayout((uint)selected.Tag);
        RhinoApp.RunScript("_LayoutProperties", false);
        FillListView();
      }
    }


    #endregion // Helper functions


    #region Rhino event handling

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

      uint page_view_sn = 0;
      if (e.View is RhinoPageView page_view)
        page_view_sn = page_view.RuntimeSerialNumber;

      m_list.BeginUpdate();

      m_list.SelectedItems.Clear();

      var selected_index = -1;

      foreach (ListViewItem item in m_list.Items)
      {
        if ((uint)item.Tag == page_view_sn)
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

    #endregion // Rhino event handling


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

    #endregion //  IPanel interface


    #region IHelp inteface

    public string HelpUrl => "https://github.com/dalefugier/RealDrawings";

    #endregion // IHelp inteface


    #region ListView event handlers

    /// <summary>
    /// ListView double-click event hander
    /// </summary>
    private void OnListViewMouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        var info = ((ListView)sender).HitTest(e.X, e.Y);
        var item = info.Item;
        if (null != item)
          DoActiveLayout((uint)item.Tag);
      }
      catch
      {
        // ignored
      }
    }

    /// <summary>
    /// ListView after edit label event hander
    /// </summary>
    private void OnListViewAfterLabelEdit(object sender, LabelEditEventArgs e)
    {
      // Always cancel. Item will be refreshed when the page view is renamed
      e.CancelEdit = true;

      if (string.IsNullOrEmpty(e.Label))
        return;

      var label = e.Label.Trim();
      if (string.IsNullOrEmpty(label))
        return;

      // User layer's name validator
      if (!Rhino.DocObjects.Layer.IsValidName(label))
        return;

      var doc = RhinoDoc.ActiveDoc;
      if (null == doc)
        return;

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
    /// List view selected index changed event handler
    /// </summary>
    private void OnListViewSelectedIndexChanged(object sender, EventArgs e)
    {
      SetButtonsEnabled(m_list.SelectedItems.Count);
    }

    /// <summary>
    /// ListView column click event handler
    /// </summary>
    private void OnListViewColumnClick(object sender, ColumnClickEventArgs e)
    {
      if (e.Column == m_item_sorter.Column)
      {
        m_item_sorter.Order = m_item_sorter.Order == SortOrder.Ascending
          ? SortOrder.Descending
          : SortOrder.Ascending;
      }
      else
      {

        m_item_sorter.Column = e.Column;
        m_item_sorter.Order = SortOrder.Ascending;
      }

      m_list.Sort();
    }

    /// <summary>
    /// ListView mouse click event handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnListViewMouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        var pt = m_list.PointToScreen(e.Location);
        m_menu.Show(pt);
      }
    }

    #endregion // ListView event handlers


    #region TextBox event handlers

    /// <summary>
    /// TextBox text changed event handler
    /// </summary>
    private void OnTextBoxTextChanged(object sender, EventArgs e)
    {
      if (m_current_event != LayoutEvent.None)
        return;

      m_filter = m_text.Text.Trim();
      FillListView();
    }

    #endregion // TextBox event handlers


    #region ToolStripButton event handlers

    /// <summary>
    /// "New" button was clicked
    /// </summary>
    private void OnButtonNewClick(object sender, EventArgs e)
    {
      DoNewLayout();
    }

    /// <summary>
    /// "Copy" button was clicked
    /// </summary>
    private void OnButtonCopyClick(object sender, EventArgs e)
    {
      DoCopyLayout();
    }

    /// <summary>
    /// "Delete" button was clicked
    /// </summary>
    private void OnButtonDeleteClick(object sender, EventArgs e)
    {
      DoDeleteLayout();
    }

    /// <summary>
    /// "Properties" button was clicked
    /// </summary>
    private void OnButtonPropertiesClick(object sender, EventArgs e)
    {
      DoLayoutProperties();
    }

    /// <summary>
    /// "Help" button was clicked
    /// </summary>
    private void OnButtonHelpClick(object sender, EventArgs e)
    {
      Process.Start(HelpUrl);
    }

    #endregion // ToolStripButton event handlers


    #region Context menu event handlers

    /// <summary>
    /// Context nenu opening
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

    /// <summary>
    /// "Active" menu was clicked
    /// </summary>
    private void OnMenuActiveClick(object sender, EventArgs e)
    {
      if (0 == m_list.SelectedItems.Count)
        return;

      var selected = m_list.SelectedItems[0];
      if (null != selected)
        DoActiveLayout((uint)selected.Tag);
    }

    /// <summary>
    /// "Copy" menu was clicked
    /// </summary>
    private void OnMenuCopyClick(object sender, EventArgs e)
    {
      DoCopyLayout();
    }

    /// <summary>
    /// "Delete" menu was clicked
    /// </summary>
    private void OnMenuDeleteClick(object sender, EventArgs e)
    {
      DoDeleteLayout();
    }

    /// <summary>
    /// "Rename" menu was clicked
    /// </summary>
    private void OnMenuRenameClick(object sender, EventArgs e)
    {
      DoRenameLayout();
    }

    /// <summary>
    /// "Properties" menu was clicked
    /// </summary>
    private void OnmMenuPropertiesClick(object sender, EventArgs e)
    {
      DoLayoutProperties();
    }

    /// <summary>
    /// "New" menu was clicked
    /// </summary>
    private void OnMenuNewClick(object sender, EventArgs e)
    {
      DoNewLayout();
    }

    #endregion // Context menu event handlers


    /// <summary>
    /// SendMessage Win32 wrapper
    /// </summary>
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);
    private const int EM_SETCUEBANNER = 0x1501;
  }


  /// <summary>
  /// ListView column sorter
  /// </summary>
  public class ListViewItemSorter : IComparer
  {
    /// <summary>
    /// Public constructor
    /// </summary>
    public ListViewItemSorter()
    {
      Column = 0;
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
        rc = StrCmpLogicalW(item_x.SubItems[Column].Text, item_y.SubItems[Column].Text);

      if (Order == SortOrder.Ascending)
        return rc;
      if (Order == SortOrder.Descending)
        return -rc;

      return 0;
    }

    /// <summary>
    /// Gets or sets the sort column
    /// </summary>
    public int Column { set; get; }

    /// <summary>
    /// Gets or sets sort order
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
