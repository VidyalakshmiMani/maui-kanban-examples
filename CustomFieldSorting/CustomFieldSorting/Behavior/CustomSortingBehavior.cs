namespace CustomFieldSorting
{
    using Syncfusion.Maui.Kanban;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Represents the behavior for managing sorting cards in a sample view.
    /// </summary>
    public class CustomSortingBehavior : Behavior<ContentPage>
    {
        #region Fields

        /// <summary>
        /// The sorting order combo box instance.
        /// </summary>
        private Picker? sortOrderPicker;

        /// <summary>
        /// The sorting mapping math combo box instance.
        /// </summary>
        private Picker? mappingPathPicker;

        /// <summary>
        /// The kanban control instance
        /// </summary>
        private SfKanban? kanban;

        /// <summary>
        /// To store initial Kanban SortingMappingPath value
        /// </summary>
        private string? sortingMappingPathValue;

        #endregion

        #region Override methods

        /// <summary>
        /// Invoked when behavior is attached to a view.
        /// </summary>
        /// <param name="contentPage">The sample view to which the behavior is attached.</param>
        protected override void OnAttachedTo(ContentPage contentPage)
        {
            base.OnAttachedTo(contentPage);

            this.kanban = contentPage.FindByName<SfKanban>("kanban");
            this.sortOrderPicker = contentPage.FindByName<Picker>("sortOrderPicker");
            this.mappingPathPicker = contentPage.FindByName<Picker>("mappingPathPicker");
            if (this.kanban != null)
            {
                this.kanban.DragEnd += this.OnCardDragEnd;
            }

            if (this.sortOrderPicker != null)
            {
                this.sortOrderPicker.ItemsSource = new ObservableCollection<string>() { "Ascending", "Descending" };
                this.sortOrderPicker.SelectedIndex = 0;
                this.sortOrderPicker.SelectedIndexChanged += OnSortOrderPickerSelectedIndexChanged;
            }

            if (this.mappingPathPicker != null)
            {
                this.mappingPathPicker.ItemsSource = new ObservableCollection<string>() { "Title", "Priority" };
                this.mappingPathPicker.SelectedIndex = 0;
                this.mappingPathPicker.SelectedIndexChanged += OnMappingPathPickerSelectedIndexChanged;
            }
        }

        /// <summary>
        /// Invoked when behavior is detached from a view.
        /// </summary>
        /// <param name="contentPage">The sample view from which the behavior is detached.</param>
        protected override void OnDetachingFrom(ContentPage contentPage)
        {
            base.OnDetachingFrom(contentPage);
            if (this.kanban != null)
            {
                this.kanban.DragEnd -= this.OnCardDragEnd;
                this.kanban = null;
            }

            if (this.sortOrderPicker != null)
            {
                this.sortOrderPicker.SelectedIndexChanged -= OnSortOrderPickerSelectedIndexChanged;
                this.sortOrderPicker = null;
            }

            if (this.mappingPathPicker != null)
            {
                this.mappingPathPicker.SelectedIndexChanged -= OnMappingPathPickerSelectedIndexChanged;
                this.mappingPathPicker = null;
            }
        }

        #endregion

        #region Property changed

        /// <summary>
        /// Occurs when the sorting order value is changed.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="e">The event args.</param>
        private void OnSortOrderPickerSelectedIndexChanged(object? sender, EventArgs e)
        {
            var selectedItem = (sender as Picker)?.SelectedItem?.ToString();
            if (this.kanban == null || selectedItem == null)
            {
                return;
            }

            if (Enum.TryParse<KanbanSortingOrder>(selectedItem.ToString(), out KanbanSortingOrder sortOrder))
            {
                this.kanban.SortingOrder = sortOrder;
            }
        }

        /// <summary>
        /// Occurs when the sorting mapping path value is changed.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="e">The event args.</param>
        private void OnMappingPathPickerSelectedIndexChanged(object? sender, EventArgs e)
        {
            var selectedItem = (sender as Picker)?.SelectedItem?.ToString();
            if (this.kanban == null || string.IsNullOrEmpty(selectedItem))
            {
                return;
            }

            this.kanban.SortingMappingPath = selectedItem;
            this.sortingMappingPathValue = selectedItem;
        }

        /// <summary>
        /// Occurs when a card drag end event is completed.
        /// </summary>
        /// <param name="sender">The object.</param>
        /// <param name="e">The event args.</param>
        private void OnCardDragEnd(object? sender, KanbanDragEndEventArgs e)
        {
            if (this.kanban == null)
            {
                return;
            }

            // Update the card's progress when moving between specific columns
            this.UpdateProgressOnColumnChange(e);
            this.kanban.RefreshKanbanColumn();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Method to update card progress value on column changes.
        /// </summary>
        /// <param name="e">The drag end event args.</param>
        private void UpdateProgressOnColumnChange(KanbanDragEndEventArgs e)
        {
            if (e == null || e.Data is not CardDetails cardDetails || e.SourceColumn == null || e.TargetColumn == null)
            {
                return;
            }

            // Get source and target category from the column's categories
            string? sourceCategory = this.GetPrimaryCategoryValue(e.SourceColumn);
            string? targetCategory = this.GetPrimaryCategoryValue(e.TargetColumn);

            if (string.IsNullOrEmpty(sourceCategory) || string.IsNullOrEmpty(targetCategory)
                || string.Equals(sourceCategory, targetCategory, StringComparison.Ordinal))
            {
                return;
            }
        }

        /// <summary>
        /// Method to get the primary category value from a column.
        /// </summary>
        /// <param name="column">The kanban column.</param>
        /// <returns>The title value.</returns>
        private string? GetPrimaryCategoryValue(KanbanColumn column)
        {
            if (column?.Categories is IEnumerable<string> list)
            {
                return list.FirstOrDefault();
            }

            return column?.Title;
        }

        #endregion
    }
}
