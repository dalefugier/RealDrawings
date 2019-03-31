using Rhino;
using Rhino.Commands;

namespace RealDrawings
{
  [CommandStyle(Style.Hidden)]
  public class TestListPageViewsCommand : Command
  {
    public override string EnglishName
    {
      get { return "TestListPageViews"; }
    }

    protected override Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      var page_views = doc.Views.GetPageViews();
      foreach (var view in page_views)
        RhinoApp.WriteLine(view.PageNumber.ToString());
      return Result.Success;
    }
  }
}