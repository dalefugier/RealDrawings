using Rhino.PlugIns;
using Rhino.UI;

namespace RealDrawings
{
  public class RealDrawingsPlugIn : PlugIn
  {
    public RealDrawingsPlugIn()
    {
      Instance = this;
    }

    /// <summary>
    /// Gets the only instance of the RealDrawingsPlugIn plug-in.
    /// </summary>
    public static RealDrawingsPlugIn Instance
    {
      get; private set;
    }

    protected override LoadReturnCode OnLoad(ref string errorMessage)
    {
      var type = typeof(LayoutsPanel);
      var icon = DrawingUtilities.IconFromResource("RealDrawings.Resources.Panel.ico", GetType().Assembly);
      Panels.RegisterPanel(this, type, "Layouts",icon, PanelType.PerDoc);

      return LoadReturnCode.Success;
    }
  }
}