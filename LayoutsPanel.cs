using System;
using System.Linq;
using System.Windows.Forms;
using Rhino;
using Rhino.UI;
using Rhino.Display;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RealDrawings
{
  [System.Runtime.InteropServices.Guid("29C790A7-1AE9-4350-900F-40AA54EA5AA0")]
  public partial class LayoutsPanel : UserControl, IPanel, IHelp
  {
    public static Guid PanelId => typeof(LayoutsPanel).GUID;

    private readonly ListViewItemSorter m_item_sorter;
    private readonly System.Drawing.Font m_font_regular;
    private readonly System.Drawing.Font m_font_bold;
    private bool m_events_hooked;

    /// <summary>
    /// Public constructor
    /// </summary>
    public LayoutsPanel()
    {
      InitializeComponent();

      var icon_size = SystemInformation.SmallIconSize.Width;
      m_button_new.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.New.ico", icon_size);
      m_button_copy.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Copy.ico", icon_size);
      m_button_delete.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Delete.ico", icon_size);
      m_button_props.Image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Props.ico", icon_size);

      var image = DrawingUtilities.LoadBitmapWithScaleDown("RealDrawings.Resources.Panel.ico", icon_size);
      var image_list = new ImageList();
      image_list.Images.Add("main", image);
      m_list.SmallImageList = image_list;

      m_item_sorter = new ListViewItemSorter();
      m_list.ListViewItemSorter = m_item_sorter;

      m_font_regular = new Font(m_list.Font, FontStyle.Regular);
      m_font_bold = new Font(m_list.Font, FontStyle.Bold);

      SendMessage(m_text.Handle, EM_SETCUEBANNER, 1, "Search");

      FillListView();
      SetButtonsEnabled(0);
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
    /// A view was created
    /// </summary>
    private void OnCreateViewEventHandler(object sender, ViewEventArgs e)
    {
      FillListView();
    }

    /// <summary>
    /// A view was renamed
    /// </summary>
    private void OnRenameViewEventHandler(object sender, ViewEventArgs e)
    {
      FillListView();
    }

    /// <summary>
    /// A view was destroyed
    /// </summary>
    private void OnDestroyViewEventHandler(object sender, ViewEventArgs e)
    {
      FillListView();
    }

    private void OnSetActiveViewEventHandler(object sender, ViewEventArgs e)
    {
      if (e.View is RhinoPageView page_view)
      {
        var sn = page_view.RuntimeSerialNumber;
        var found = false;
        foreach (ListViewItem item in m_list.Items)
        {
          if ((uint)item.Tag == sn)
          {
            m_list.SelectedItems.Clear();
            item.Selected = true;
            item.Font = m_font_bold;
            found = true;
            //break;
          }
          else
          {
            item.Font = m_font_regular;
          }
        }

        if (!found)
        {
          FillListView();
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
      RhinoApp.Idle -= OnIdle;
      FillListView();
      ResizeViewListColumns();
    }

    private void OnEndOpenDocument(object sender, DocumentOpenEventArgs e)
    {
      RhinoApp.Idle += OnIdle;
    }

    /// <summary>
    /// FillListview
    /// </summary>
    void FillListView()
    {
      var sizes = new int[m_list.Columns.Count];
      for (var i = 0; i < m_list.Columns.Count; i++)
        sizes[i] = m_list.Columns[i].Width;

      m_list.BeginUpdate();

      m_list.Items.Clear();

      var doc = RhinoDoc.ActiveDoc;
      if (null != doc)
      {
        try
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

            if (view.RuntimeSerialNumber == view_sn)
              item.Font = m_font_bold;

            m_list.Items.Add(item);
          }
        }
        catch
        {
          // Do nothing
        }
      }

      for (var i = 0; i < m_list.Columns.Count; i++)
        m_list.Columns[i].Width = sizes[i];

      m_list.EndUpdate();
    }

    void ResizeViewListColumns()
    {
      foreach (ColumnHeader column in m_list.Columns)
        column.Width = -2;
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

    public string HelpUrl => "https://github.com/dalefugier/RealDrawings";

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

    private void OnListViewMouseDoubleClick(object sender, MouseEventArgs e)
    {
      var doc = RhinoDoc.ActiveDoc;
      if (null == doc)
        return;

      ListViewHitTestInfo info = ((ListView)sender).HitTest(e.X, e.Y);
      var item = info.Item;
      if (null != item)
        SetActiveLayout((uint)item.Tag);
    }

    private void OnListViewBeforeLabelEdit(object sender, LabelEditEventArgs e)
    {
    }

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

    private void OnListViewSelectedIndexChanged(object sender, EventArgs e)
    {
      SetButtonsEnabled(m_list.SelectedItems.Count);
    }

    private void SetButtonsEnabled(int selectedCount)
    {
      if (0 == selectedCount)
      {
        m_button_copy.Enabled = false;
        m_button_delete.Enabled = false;
        m_button_props.Enabled = false;
      }
      else if (1 == selectedCount)
      {
        m_button_copy.Enabled = true;
        m_button_delete.Enabled = true;
        m_button_props.Enabled = true;
      }
      else
      {
        m_button_copy.Enabled = false;
        m_button_delete.Enabled = true;
        m_button_props.Enabled = false;
      }
    }

    private void OnListViewColumnClick(object sender, ColumnClickEventArgs e)
    {
      // Determine if clicked column is already the column that is being sorted.
      if (e.Column == m_item_sorter.SortColumn)
      {
        // Reverse the current sort direction for this column.
        if (m_item_sorter.Order == SortOrder.Ascending)
        {
          m_item_sorter.Order = SortOrder.Descending;
        }
        else
        {
          m_item_sorter.Order = SortOrder.Ascending;
        }
      }
      else
      {
        // Set the column number that is to be sorted; default to ascending.
        m_item_sorter.SortColumn = e.Column;
        m_item_sorter.Order = SortOrder.Ascending;
      }

      // Perform the sort with these new sort options.
      m_list.Sort();
    }

    private void OnTextBoxTextChanged(object sender, EventArgs e)
    {
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
      RhinoApp.RunScript("_Layout", false);
      FillListView();
    }

    /// <summary>
    /// OnButtonCopyClick
    /// </summary>
    private void OnButtonCopyClick(object sender, EventArgs e)
    {
      ListViewItem selected = m_list.SelectedItems[0];
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

      var current_sn = ActiveLayout;

      foreach (ListViewItem item in m_list.SelectedItems)
      {
        SetActiveLayout((uint)item.Tag);
        RhinoApp.Wait();
        RhinoApp.RunScript("_-CloseViewport _Yes", false);
        RhinoApp.Wait();
      }

      FillListView();
      SetActiveLayout(current_sn);
    }

    /// <summary>
    /// OnButtonPropsClick
    /// </summary>
    private void OnButtonPropsClick(object sender, EventArgs e)
    {
      ListViewItem selected = m_list.SelectedItems[0];
      if (null != selected)
      {
        SetActiveLayout((uint)selected.Tag);
        RhinoApp.RunScript("_LayoutProperties", false);
        FillListView();
      }
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)]string lParam);
    private const int EM_SETCUEBANNER = 0x1501;


  }

  public class ListViewItemSorter : IComparer
  {
    /// <summary>
    /// Specifies the column to be sorted
    /// </summary>
    private int m_column_to_sort;
    /// <summary>
    /// Specifies the order in which to sort (i.e. 'Ascending').
    /// </summary>
    private SortOrder m_order_of_sort;
    /// <summary>
    /// Case insensitive comparer object
    /// </summary>
    private CaseInsensitiveComparer m_object_compare;

    /// <summary>
    /// Class constructor.  Initializes various elements
    /// </summary>
    public ListViewItemSorter()
    {
      // Initialize the column to '0'
      m_column_to_sort = 0;

      // Initialize the sort order to 'none'
      m_order_of_sort = SortOrder.None;

      // Initialize the CaseInsensitiveComparer object
      m_object_compare = new CaseInsensitiveComparer();
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
      compareResult = m_object_compare.Compare(listviewX.SubItems[m_column_to_sort].Text, listviewY.SubItems[m_column_to_sort].Text);

      // Calculate correct return value based on object comparison
      if (m_order_of_sort == SortOrder.Ascending)
      {
        // Ascending sort is selected, return normal result of compare operation
        return compareResult;
      }
      else if (m_order_of_sort == SortOrder.Descending)
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
        m_column_to_sort = value;
      }
      get
      {
        return m_column_to_sort;
      }
    }

    /// <summary>
    /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
    /// </summary>
    public SortOrder Order
    {
      set
      {
        m_order_of_sort = value;
      }
      get
      {
        return m_order_of_sort;
      }
    }
  }
}
