using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace libQB.Behaviors;

public class DataGridExtendBehavior
    : Behavior<DataGrid>
{
    #region Properties

    public static readonly DependencyProperty IsEditingProperty =
        DependencyProperty.Register(nameof(IsEditing),
            typeof(bool),
            typeof(DataGridExtendBehavior),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool IsEditing
    {
        get => (bool)GetValue(IsEditingProperty);
        set => SetValue(IsEditingProperty, value);
    }

    public static readonly DependencyProperty IsSortedProperty =
        DependencyProperty.Register(nameof(IsSorted),
            typeof(bool),
            typeof(DataGridExtendBehavior),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public bool IsSorted
    {
        get => (bool)GetValue(IsSortedProperty);
        set => SetValue(IsSortedProperty, value);
    }

    public static readonly DependencyProperty SelectedRowsProperty =
        DependencyProperty.Register(nameof(SelectedRows),
            typeof(IList),
            typeof(DataGridExtendBehavior),
            new FrameworkPropertyMetadata(new List<object>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public IList SelectedRows
    {
        get => (IList)GetValue(SelectedRowsProperty);
        set => SetValue(SelectedRowsProperty, value);
    }

    public static readonly DependencyProperty SelectedRowProperty =
        DependencyProperty.Register(nameof(SelectedRow),
            typeof(object),
            typeof(DataGridExtendBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public object SelectedRow
    {
        get => GetValue(SelectedRowProperty);
        set => SetValue(SelectedRowProperty, value);
    }

    public static readonly DependencyProperty DeleteCommandProperty =
        DependencyProperty.Register(nameof(DeleteCommand), typeof(ICommand), typeof(DataGridExtendBehavior));

    public ICommand DeleteCommand
    {
        get => (ICommand)GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }

    public static readonly DependencyProperty SelectionChangedCommandProperty =
        DependencyProperty.Register(nameof(SelectionChangedCommand), typeof(ICommand), typeof(DataGridExtendBehavior));

    public ICommand SelectionChangedCommand
    {
        get => (ICommand)GetValue(SelectionChangedCommandProperty);
        set => SetValue(SelectionChangedCommandProperty, value);
    }

    #endregion

    #region Attach

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.SelectionChanged += OnSelectionChanged;
        AssociatedObject.BeginningEdit += OnBeginningEdit;
        AssociatedObject.CellEditEnding += OnCellEditEnding;
        AssociatedObject.PreviewKeyDown += OnPreviewKeyDown;
        AssociatedObject.Sorting += OnSorting;

        SelectedRows?.Clear();
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.SelectionChanged -= OnSelectionChanged;
        AssociatedObject.BeginningEdit -= OnBeginningEdit;
        AssociatedObject.CellEditEnding -= OnCellEditEnding;
        AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;
        AssociatedObject.Sorting -= OnSorting;

        SelectedRows?.Clear();
    }

    #endregion

    #region Medhod

    private void OnSorting(object sender, DataGridSortingEventArgs e)
    {
        if (sender is DataGrid dataGrid)
        {
            if (e.Column.SortDirection == System.ComponentModel.ListSortDirection.Descending)
            {
                e.Handled = true;
                e.Column.SortDirection = null;
                dataGrid.Items.SortDescriptions.Clear();
                IsSorted = false;
            }
            else
            {
                IsSorted = true;
            }
        }
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender == null) return;
        if (e.OriginalSource.GetType() != typeof(DataGrid)) return;
        if (e.AddedItems.Count == 0 && e.RemovedItems.Count == 0) return;

        SelectedRows.Clear();
        foreach (var item in AssociatedObject.SelectedItems)
        {
            SelectedRows.Add(item);
        }

        if (SelectedRows.Count == 1)
        {
            SelectedRow = SelectedRows[0]!;
        }
        else
        {
            SelectedRow = null!;
        }

        if (SelectionChangedCommand != null)
        {
            SelectionChangedCommand?.Execute(SelectedRows.Count);
        }
    }

    private void OnBeginningEdit(object? sender, DataGridBeginningEditEventArgs e)
    {
        IsEditing = true;
    }

    private void OnCellEditEnding(object? sender, DataGridCellEditEndingEventArgs e)
    {
        if (IsEditing == false) return;
        if (sender is not DataGrid dataGrid) return;

        IsEditing = false;
        dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
        dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (!AssociatedObject.IsKeyboardFocusWithin) return;

        if (IsEditing && e.Key == Key.Enter)
        {
            // 編集確定し、行移動を抑止
            AssociatedObject.CommitEdit(DataGridEditingUnit.Cell, true);
            AssociatedObject.CommitEdit(DataGridEditingUnit.Row, true);

            e.Handled = true;
            return;
        }

        if (IsEditing) return;

        if (e.Key != Key.Delete) return;

        var dataGrid = sender as DataGrid;

        if (dataGrid?.SelectedItems.Count > 0 &&
            DeleteCommand != null &&
            DeleteCommand.CanExecute(dataGrid.SelectedItems))
        {
            DeleteCommand.Execute(dataGrid.SelectedItems);
            e.Handled = true;
        }
        else
        {
            e.Handled = true;
        }
    }

    #endregion
}
