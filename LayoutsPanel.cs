using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rhino;
using Rhino.UI;
using Rhino.Display;
using System.Collections;

namespace RealDrawings
{
  [System.Runtime.InteropServices.Guid("29C790A7-1AE9-4350-900F-40AA54EA5AA0")]
  public partial class LayoutsPanel : UserControl, IPanel, IHelp
  {
    public static Guid PanelId => typeof(LayoutsPanel).GUID;

    ToolTip m_tt_new = new ToolTip();
    ToolTip m_tt_copy = new ToolTip();
    ToolTip m_tt_delete = new ToolTip();
    ToolTip m_tt_props = new ToolTip();
    ToolTip m_tt_up = new ToolTip();
    ToolTip m_tt_down = new ToolTip();

    private ListViewColumnSorter lvwColumnSorter;

    public LayoutsPanel()
    {
      InitializeComponent();

      var icon_size = SystemInformation.SmallIconSize.Width;
      m_btn_new.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.New.ico", 24);
      m_btn_copy.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Copy.ico", 24);
      m_btn_delete.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Delete.ico", 24);
      m_btn_props.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Props.ico", 24);
      m_btn_up.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Up.ico", 24);
      m_btn_down.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Down.ico", 24);

      RhinoDoc.EndOpenDocument += OnEndOpenDocument;
      RhinoView.Create += OnCreateViewEventHandler;
      RhinoView.Rename += OnRenameViewEventHandler;
      RhinoView.SetActive += OnSetActiveViewEventHandler;
      RhinoView.Destroy += OnDestroyViewEventHandler;

      m_tt_new.SetToolTip(m_btn_new, "New");
      m_tt_copy.SetToolTip(m_btn_copy, "Copy");
      m_tt_delete.SetToolTip(m_btn_delete, "Delete");
      m_tt_props.SetToolTip(m_btn_props, "Properties");
      m_tt_up.SetToolTip(m_btn_up, "Move Up");
      m_tt_down.SetToolTip(m_btn_down, "Move Down");

      var image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Panel.ico", 16);
      var imageList = new ImageList();
      imageList.Images.Add("main", image);
      m_list.SmallImageList = imageList;

      lvwColumnSorter = new ListViewColumnSorter();
      m_list.ListViewItemSorter = lvwColumnSorter;

      FillList();
    }


    /// <summary>
    /// A view was created
    /// </summary>
    private void OnCreateViewEventHandler(object sender, ViewEventArgs e)
    {
      FillList();
    }

    /// <summary>
    /// A view was renamed
    /// </summary>
    private void OnRenameViewEventHandler(object sender, ViewEventArgs e)
    {
      FillList();
    }

    /// <summary>
    /// A view was destroyed
    /// </summary>
    private void OnDestroyViewEventHandler(object sender, ViewEventArgs e)
    {
      FillList();
    }

    private void OnSetActiveViewEventHandler(object sender, ViewEventArgs e)
    {
      var view = e.View;
      var page_view = view as RhinoPageView;
      if (null != page_view)
      {
        var sn = page_view.RuntimeSerialNumber;
        var found = false;
        foreach (ListViewItem item in m_list.Items)
        {
          if ((uint)item.Tag == sn)
          {
            m_list.SelectedItems.Clear();
            item.Selected = true;
            found = true;
            break;
          }
        }

        if (!found)
        {
          FillList();
        }

        foreach (ListViewItem item in m_list.Items)
        {
          if ((uint)item.Tag == sn)
          {
            m_list.SelectedItems.Clear();
            item.Selected = true;
            break;
          }
        }
      }
    }

    private void OnIdle(object sender, EventArgs e)
    {
      FillList();
      RhinoApp.Idle -= OnIdle;
    }

    private void OnEndOpenDocument(object sender, DocumentOpenEventArgs e)
    {
      //FillList();
      RhinoApp.Idle += OnIdle;
    }

    void FillList()
    {
      m_list.BeginUpdate();

      m_list.Items.Clear();

      var doc = RhinoDoc.ActiveDoc;
      if (null == doc)
        return;

      try
      {
        var page_views = doc.Views.GetPageViews();
        foreach (var view in page_views)
        {
          var arr = new string[3];
          arr[0] = view.PageName;
          arr[1] = string.Format("{0} x {1}", view.PageWidth, view.PageHeight);
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
      catch
      {
        // Do nothing
      }

      foreach (ColumnHeader column in m_list.Columns)
      {
        column.Width = -2;
      }

      m_list.EndUpdate();
    }

    #region IPanel interface

    public void PanelClosing(uint documentSerialNumber, bool onCloseDocument)
    {
      //throw new NotImplementedException();
    }

    public void PanelHidden(uint documentSerialNumber, ShowPanelReason reason)
    {
      //throw new NotImplementedException();
    }

    public void PanelShown(uint documentSerialNumber, ShowPanelReason reason)
    {
      FillList();
    }

    #endregion

    public string HelpUrl => "https://github.com/dalefugier/RealDrawings";

    private bool SetActiveLayout(uint layout_sn)
    {
      var rc = false;
      var doc = RhinoDoc.ActiveDoc;
      if (null == doc)
        return false;

      foreach (var view in doc.Views.GetPageViews())
      {
        if (view.RuntimeSerialNumber == layout_sn)
        {
          doc.Views.ActiveView = view;
          doc.Views.Redraw();
          rc = true;
          break;
        }
      }
      return rc;
    }

    private void OnListMouseDoubleClick(object sender, MouseEventArgs e)
    {
      var doc = RhinoDoc.ActiveDoc;
      if (null == doc)
        return;

      ListViewHitTestInfo info = ((ListView)sender).HitTest(e.X, e.Y);
      var item = info.Item;
      if (null != item)
        SetActiveLayout((uint)item.Tag);
    }

    private void OnCopyButtonClick(object sender, EventArgs e)
    {
      ListViewItem selected = m_list.SelectedItems[0];
      if (null != selected)
      {
        SetActiveLayout((uint)selected.Tag);
        RhinoApp.RunScript("_CopyLayout", false);
        FillList();
      }

      //RhinoApp.RunScript("_CopyLayout", false);
      //FillList();
    }

    private void OnNewButtonClick(object sender, EventArgs e)
    {
      RhinoApp.RunScript("_Layout", false);
      FillList();
    }

    private void OnButtonPropsClick(object sender, EventArgs e)
    {
      ListViewItem selected = m_list.SelectedItems[0];
      if (null != selected)
      {
        SetActiveLayout((uint)selected.Tag);
        RhinoApp.RunScript("_LayoutProperties", false);
        FillList();
      }
      //var doc = RhinoDoc.ActiveDoc;
      //if (null == doc)
      //  return;

      //var view = doc.Views.ActiveView;
      //if (null == view)
      //  return;

      //var page_view = view as RhinoPageView;
      //if (null == page_view)
      //  return;

      //RhinoApp.RunScript("_LayoutProperties", false);
      //FillList();
    }

    private void OnButtonDeleteClick(object sender, EventArgs e)
    {
      ListViewItem selected = m_list.SelectedItems[0];
      if (null != selected)
      {
        SetActiveLayout((uint)selected.Tag);
        RhinoApp.RunScript("_CloseViewport", false);
        FillList();
      }

      //var doc = RhinoDoc.ActiveDoc;
      //if (null == doc)
      //  return;

      //var view = doc.Views.ActiveView;
      //if (null == view)
      //  return;

      //var page_view = view as RhinoPageView;
      //if (null == page_view)
      //  return;

      //RhinoApp.RunScript("_CloseViewport", false);
      //FillList();
    }

    private void ShuffleLayoutTabs()
    {
      //var doc = RhinoDoc.ActiveDoc;
      //if (null == doc)
      //  return;

      //var i = 0;
      //foreach (ListViewItem item in m_list.Items)
      //{
      //  var sn = (uint)item.Tag;
      //  foreach (var view in doc.Views.GetPageViews())
      //  {
      //    if (view.RuntimeSerialNumber == sn)
      //    {
      //      view.PageNumber = i++;
      //      break;
      //    }
      //  }
      //}

      //doc.Views.Redraw();
      //RhinoApp.RunScript("_-ViewportTabs _Hide", false);
      //RhinoApp.RunScript("_-ViewportTabs _Show", false);
    }

    private void OnButtonUpClick(object sender, EventArgs e)
    {
      try
      {
        if (m_list.SelectedItems.Count > 0)
        {
          ListViewItem selected = m_list.SelectedItems[0];
          int indx = selected.Index;
          int totl = m_list.Items.Count;

          if (indx == 0)
          {
            m_list.Items.Remove(selected);
            m_list.Items.Insert(totl - 1, selected);
          }
          else
          {
            m_list.Items.Remove(selected);
            m_list.Items.Insert(indx - 1, selected);
          }
          ShuffleLayoutTabs();
        }
        else
        {
        }
      }
      catch
      {
      }
    }

    private void OnButtonDownClick(object sender, EventArgs e)
    {
      try
      {
        if (m_list.SelectedItems.Count > 0)
        {
          ListViewItem selected = m_list.SelectedItems[0];
          int indx = selected.Index;
          int totl = m_list.Items.Count;

          if (indx == totl - 1)
          {
            m_list.Items.Remove(selected);
            m_list.Items.Insert(0, selected);
          }
          else
          {
            m_list.Items.Remove(selected);
            m_list.Items.Insert(indx + 1, selected);
          }
          ShuffleLayoutTabs();
        }
        else
        {
        }
      }
      catch
      {
      }
    }

    private void m_list_BeforeLabelEdit(object sender, LabelEditEventArgs e)
    {

    }

    private void m_list_AfterLabelEdit(object sender, LabelEditEventArgs e)
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

    private void m_list_ColumnClick(object sender, ColumnClickEventArgs e)
    {
      // Determine if clicked column is already the column that is being sorted.
      if (e.Column == lvwColumnSorter.SortColumn)
      {
        // Reverse the current sort direction for this column.
        if (lvwColumnSorter.Order == SortOrder.Ascending)
        {
          lvwColumnSorter.Order = SortOrder.Descending;
        }
        else
        {
          lvwColumnSorter.Order = SortOrder.Ascending;
        }
      }
      else
      {
        // Set the column number that is to be sorted; default to ascending.
        lvwColumnSorter.SortColumn = e.Column;
        lvwColumnSorter.Order = SortOrder.Ascending;
      }

      // Perform the sort with these new sort options.
      m_list.Sort();
    }

    private void m_text_TextChanged(object sender, EventArgs e)
    {
      var doc = RhinoDoc.ActiveDoc;
      if (null == doc)
        return;

      var views = doc.Views.GetPageViews();
      if (null == views || views.Length == 0)
        return;

      var text = m_text.Text.Trim();
      if (string.IsNullOrEmpty(text))
      {
        FillList();
        return;
      }

      m_list.Items.Clear();

      foreach (var view in views.Where(v => v.PageName.ToLower().Contains(text.ToLower())))
      {
        var arr = new string[3];
        arr[0] = view.PageName;
        arr[1] = string.Format("{0} x {1}", view.PageWidth, view.PageHeight);
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
  }

  public class ListViewColumnSorter : IComparer
  {
    /// <summary>
    /// Specifies the column to be sorted
    /// </summary>
    private int ColumnToSort;
    /// <summary>
    /// Specifies the order in which to sort (i.e. 'Ascending').
    /// </summary>
    private SortOrder OrderOfSort;
    /// <summary>
    /// Case insensitive comparer object
    /// </summary>
    private CaseInsensitiveComparer ObjectCompare;

    /// <summary>
    /// Class constructor.  Initializes various elements
    /// </summary>
    public ListViewColumnSorter()
    {
      // Initialize the column to '0'
      ColumnToSort = 0;

      // Initialize the sort order to 'none'
      OrderOfSort = SortOrder.None;

      // Initialize the CaseInsensitiveComparer object
      ObjectCompare = new CaseInsensitiveComparer();
    }

    /// <summary>
    /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
    /// </summary>
    /// <param name="x">First object to be compared</param>
    /// <param name="y">Second object to be compared</param>
    /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
    public int Compare(object x, object y)
    {
      int compareResult;
      ListViewItem listviewX, listviewY;

      // Cast the objects to be compared to ListViewItem objects
      listviewX = (ListViewItem)x;
      listviewY = (ListViewItem)y;

      // Compare the two items
      compareResult = ObjectCompare.Compare(listviewX.SubItems[ColumnToSort].Text, listviewY.SubItems[ColumnToSort].Text);

      // Calculate correct return value based on object comparison
      if (OrderOfSort == SortOrder.Ascending)
      {
        // Ascending sort is selected, return normal result of compare operation
        return compareResult;
      }
      else if (OrderOfSort == SortOrder.Descending)
      {
        // Descending sort is selected, return negative result of compare operation
        return (-compareResult);
      }
      else
      {
        // Return '0' to indicate they are equal
        return 0;
      }
    }

    /// <summary>
    /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
    /// </summary>
    public int SortColumn
    {
      set
      {
        ColumnToSort = value;
      }
      get
      {
        return ColumnToSort;
      }
    }

    /// <summary>
    /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
    /// </summary>
    public SortOrder Order
    {
      set
      {
        OrderOfSort = value;
      }
      get
      {
        return OrderOfSort;
      }
    }
  }
}
