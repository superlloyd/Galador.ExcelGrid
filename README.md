## Galador.ExcelGrid

This code is a rework and trim down of that other GitHub repository:
https://github.com/PropertyTools/PropertyTools

![Simple Grid](Screenshot1.png)

I extracted only the code relevant to their (Excel like) DataGrid, targeted .NET6, renamed a few namespace and voila.
Aiming for a simple, self contained spreadsheet control looking like Excel.

I also added their original WPF samples (*ExcelGridDemo*).
And added a sample to experiment with the data model I am aiming for: *SimpleGrid*.


**About SimpleGrid sample and other changes**
Also made the following changes to the original `DataGrid` (from https://github.com/PropertyTools/PropertyTools) 

- Removed some steps between `IDataGridOperator` to rendering and edit control instantiation
- Removed the build color edit support (since I didn't copy across the color picker)
- Fixed some resize (column/rows) bugs
- Added IsReadOnly property
- Added full support to 2 additionals models types that act like `string[,]` and `Cell[,]`.
  - `Galador.Document.Grid.StringGridModel`
  - `Galador.Document.Grid.CellGridModel`

*Remark* to succesfully use those 2 new models, one need to use the new
`Galador.WPF.ExcelGrid.ExcelDataGrid` that use the appropriate `IDataGridOperator` and `IControlFactory`.

*Remark* One can also use custom subclass of `Cell` for the `CellGridModel` and register an `IControlFactory`
to render them.

*Remark* maybe one day add formula to Cells with Lua?
(https://www.moonsharp.org/)

