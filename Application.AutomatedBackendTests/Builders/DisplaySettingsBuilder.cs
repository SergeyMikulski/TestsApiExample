using Application.Contract;
using Application.Contract.Export;
using Application.Contract.SearchResults;
using System.Collections.Generic;
using System.Linq;

namespace Application.AutomatedBackendTests.Builders
{
    internal class UserSettingsBuilder
    {
        private readonly UserSettings _settings;

        public UserSettingsBuilder(UserSettings settingsFromResponse)
        {
            _settings = settingsFromResponse;
        }

        public UserSettings Build() => _settings;

        public UserSettingsBuilder AddSortingToFirstColumnWithDisabledSorting(SortOrder order)
        {
            var column = _settings.Columns.First(c => c.AllowSorting && !c.Hidden && !_settings.Sorting.Any(s => s.Field.Equals(c.Key)) && !_settings.Grouping.Any(s => s.Field.Equals(c.Key)));
            var sortInfo = new SortInfo
            {
                Field = column.Key,
                SortOrder = order
            };
            _settings.Sorting = new List<SortInfo>
            {
                sortInfo
            };
            return this;
        }

        public UserSettingsBuilder AddMultipleSorting(List<string> columnsToBeSorted, List<SortOrder> sortOrders)
        {
            List<SortInfo> sortInfoNew = new List<SortInfo>();
            for (int i = 0; i < columnsToBeSorted.Count(); i++)
            {
                var column = _settings.Columns.First(x => x.HeaderText == columnsToBeSorted[i]);
                var sortInfo = new SortInfo
                {
                    Field = column.Key,
                    SortOrder = sortOrders[i]
                };
                sortInfoNew.Add(sortInfo);
            }
            _settings.Sorting = sortInfoNew;
            return this;
        }

        public UserSettingsBuilder AddMultipleGrouping(List<string> columnsToBeGrouped, List<SortOrder> sortOrders)
        {
            List<SortInfo> sortInfoNew = new List<SortInfo>();
            for (int i = 0; i < columnsToBeGrouped.Count(); i++)
            {
                var column = _settings.Columns.First(x => x.HeaderText == columnsToBeGrouped[i]);
                var sortInfo = new SortInfo
                {
                    Field = column.Key,
                    SortOrder = sortOrders[i]
                };
                sortInfoNew.Add(sortInfo);
            }
            _settings.Grouping = sortInfoNew;
            return this;
        }

        public UserSettingsBuilder AddGroupingToFirstColumnWithDisabledGrouping(SortOrder order)
        {
            var column = _settings.Columns.First(c => c.AllowGrouping && !c.Hidden && !_settings.Grouping.Any(s => s.Field.Equals(c.Key)) && !_settings.Sorting.Any(s => s.Field.Equals(c.Key)));
            var sortInfo = new SortInfo
            {
                Field = column.Key,
                SortOrder = order
            };
            _settings.Grouping = new List<SortInfo>
            {
                sortInfo
            };
            return this;
        }

        public UserSettingsBuilder FreezeFirstNotFreezedColumn()
        {
            var column = _settings.Columns.First(c =>
                !c.Hidden && !_settings.FixedColumns.Any(fc => fc.Equals(c.HeaderText)));
            _settings.FixedColumns = new List<string>
            {
                column.HeaderText
            };
            return this;
        }

        public UserSettingsBuilder ChangePageSize(int pageSize)
        {
            _settings.PageSize = pageSize;
            return this;
        }

        public UserSettingsBuilder SwapTwoFirstColumns()
        {
            var showedColumns = _settings.Columns
                .Where(c => !c.Hidden).ToList();

            var columnForReorder = showedColumns.ElementAt(1);
            var reorderedColumns = showedColumns.Except(columnForReorder).ToList();
            reorderedColumns.Insert(0, columnForReorder);

            _settings.Columns = reorderedColumns;
            return this;
        }

        public UserSettingsBuilder ChangeExportSettings(ExportSettings exportSettings)
        {
            _settings.ExportSettings = exportSettings;
            return this;
        }

        public UserSettingsBuilder ChangeNumberOfDecimals(int numberOfDecimals)
        {
            _settings.NumberOfDecimals = numberOfDecimals;
            return this;
        }

        public UserSettingsBuilder ChangeDateRange(DateRangeOptions dateRangeOptions)
        {
            _settings.DateRange = dateRangeOptions;
            return this;
        }

        public UserSettingsBuilder ChangeWidthForFirstColumn(string width)
        {
            _settings.Columns.First(c => !c.Hidden).Width = width;
            return this;
        }

        public UserSettings GetUserSettings()
        {
            return _settings;
        }
    }
}
