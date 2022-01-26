## Galador.ExcelGrid

This code is a rework and trim down of that other GitHub repository:
https://github.com/PropertyTools/PropertyTools

![Simple Grid](Screenshot1.png)

I extracted only the code relevant to their (Excel like) DataGrid, targeted .NET6, renamed a few namespace and voila.
Aiming for a simple, self contained spreadsheet control looking like Excel.

I also added their original WPF samples (*ExcelGridDemo*).
And added a sample to experiment with the data model I am aiming for: *SimpleGrid*.

**Remark** Of note I cut most of their controls (but the DataGid), hence the default color editor is no longer editing colors.
I also simplify the grid code enough that one cannot just use one instead of the other.

**Remark** maybe one day add formula with Lua?
(https://www.moonsharp.org/)


**About SimpleGrid sample and other changes**

I reworked the original `DataGrid` (from https://github.com/PropertyTools/PropertyTools) 
to support 2 additionals models types that act like `string[,]` and `Cell[,]`.
- `Galador.Document.Grid.StringGridModel`
- `Galador.Document.Grid.CellGridModel`

Had to update the grid slightly. But they need the new 
`Galador.WPF.ExcelGrid.ExcelDataGrid` to be displayed succesfully.
Each `Cell` can now have their own renderer and editors by using update the 
`Galador.WPF.ExcelGrid.CellViewFactory`.