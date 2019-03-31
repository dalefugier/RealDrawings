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

    private void OnListMouseDoubleClick(object sender, MouseEventArgs e)
    {
      var doc = RhinoDoc.ActiveDoc;
      if (null == doc)
        return;

      ListViewHitTestInfo info = ((ListView)sender).HitTest(e.X, e.Y);
      var item = info.Item;
      if (null != item)
      {
        var sn = (uint)item.Tag;
        foreach (var view in doc.Views.GetPageViews())
        {
          if (view.RuntimeSerialNumber == sn)
          {
            doc.Views.ActiveView = view;
            doc.Views.Redraw();
            break;
          }
        }
      }
    }

    private void OnCopyButtonClick(object sender, EventArgs e)
    {
      RhinoApp.RunScript("_CopyLayout", false);
      FillList();
    }

    private void OnNewButtonClick(object sender, EventArgs e)
    {
      RhinoApp.RunScript("_Layout", false);
      FillList();
    }

    private void OnButtonPropsClick(object sender, EventArgs e)
    {
      var doc = RhinoDoc.ActiveDoc;
      if (null == doc)
        return;

      var view = doc.Views.ActiveView;
      if (null == view)
        return;

      var page_view = view as RhinoPageView;
      if (null == page_view)
        return;

      RhinoApp.RunScript("_LayoutProperties", false);
      FillList();
    }

    private void OnButtonDeleteClick(object sender, EventArgs e)
    {
      var doc = RhinoDoc.ActiveDoc;
      if (null == doc)
        return;

      var view = doc.Views.ActiveView;
      if (null == view)
        return;

      var page_view = view as RhinoPageView;
      if (null == page_view)
        return;

      RhinoApp.RunScript("_CloseViewport", false);
      FillList();
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
  }
}
