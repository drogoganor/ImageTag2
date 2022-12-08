# ImageTag2
.NET 7 WPF reimplementation of ImageTagWPF

# TODO

* Scaffold EF context: dotnet ef dbcontext scaffold "Data Source=T:\imagetag.db" Microsoft.EntityFrameworkCore.Sqlite

* Restore 'SelectedItemChanged="FileTree_OnSelectedItemChanged"' on DirectoryExplorer.FileTree XAML:

        <TreeView x:Name="FileTree" HorizontalAlignment="Stretch" Margin="0,54,0,0" VerticalAlignment="Stretch"
                  SelectedItemChanged="FileTree_OnSelectedItemChanged"
                  Grid.RowSpan="2">

* Move to IoC implementation
* Find replacement for color picker
