# RealDrawings

<img width="128" height="128" src="https://github.com/dalefugier/RealDrawings/raw/master/Resources/final_hawk_256.png">

**RhinoDrawings** is an open-source, [Rhino](https://www.rhino3d.com/) plug-in project that was developed during the [**AEC Tech 2019 West Coast Hackathon**](http://core.thorntontomasetti.com/event/aec-tech-2019-seattle/) held in Seattle, Washington. The focus of the project and the team was to ease some of the pain in organizing and producing 2-D drawings in Rhino.

### Team Members

[Daniel Depoe](mailto:daniel.depoe@katerra.com), [Katerra](https://katerra.com/)
[James Munns](mailto:jmunns@structurecraft.com> ), [StructureCraft](https://structurecraft.com/)
[Dale Fugier](mailto: <dale@mcneel.com), [Robert McNeel & Associates](https://www.rhino3d.com/)
[Mary Fugier](mailto:mary@mcneel.com), [Robert McNeel & Associates](https://www.rhino3d.com/)

### Tools

#### LayoutHawks

The initial tools works to solve the problem of Rhino's layout tabs. Layout tabs appear below Rhino's graphics area.

##### Problem

Once you start creating multiple layouts, the tab control becomes unusable, as there are too many tabs to view. Thus, you are stuck having to click arrows to switch between layouts. If you have a lot of layouts, this involves a lot of clicking, and is very annoying. This user-interface is referred to a *papercut*.

##### Solution

The team's solution was to developer a new `Layouts` panel. This panel would show all layouts in a easy-to-use list. Double-clicking on layout items in the list would activate the layout, thus eliminating the need for the layout tabs. The panel would also provide additional functionality, such as creating new layouts, copying existing layouts, renaming layouts, deleting layouts, and more.

### License

Code licensed under the [MIT License](https://github.com/dalefugier/RealDrawings/blob/master/LICENSE).

