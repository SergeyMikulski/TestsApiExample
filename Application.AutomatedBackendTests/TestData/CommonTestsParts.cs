using Application.AutomatedBackendTests.Extensions;
using Application.AutomatedBackendTests.Models;
using Application.AutomatedBackendTests.Modules;
using Application.Contract;
using Application.Contract.SearchResults;
using FluentAssertions;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.AutomatedBackendTests.TestData
{
    public static class CommonTestsParts
    {
        private static void SetSeriesToSortingCheck
            (List<string> columnToBeSortedKeys, BuildQueryModule buildQueryModule, List<Contract.SearchResults.Series> series
            , List<SortOrder> sortOrders, List<string> columnNamesToBeSorted, Dataset dataset, List<string> taxonomyFiltersWithCustomAggregatesNames = null)
        {
            List<List<string>> columnsWithValues = new List<List<string>>();
            foreach (var columnToBeSortedKey in columnToBeSortedKeys)
            {
                var columnValues = buildQueryModule.GetGridValuesForParticularColumn(series, columnToBeSortedKey);
                columnsWithValues.Add(columnValues);
            }

            SortInfoExtensions.CheckDataForMultipleColumnsSorted(columnsWithValues, sortOrders, columnNamesToBeSorted, dataset, taxonomyFiltersWithCustomAggregatesNames);
        }

        public static async Task CheckGroupingSorting
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule
            , Dataset dataset, List<SortOrder> sortOrders
            , List<string> columnNamesToBeGrouped = null, List<string> columnNamesToBeSorted = null, int requiredFiltersNumber = 1
            , List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            var taxonomyFilters = await buildQueryModule.GetTaxonomyFilters(dataset);
            var displaySettings = await datasetSettingsModule.GetDataSetSettings(dataset);

            List<string> columnNamesToBeGroupedSortedKeys = new List<string>();
            List<string> columnNamesToBeGroupedSorted = new List<string>();
            for (int i = 0; i < columnNamesToBeGrouped?.Count(); i++)
            {
                var columnToBeGroupedKey = buildQueryModule.GetColumnKeyByName(displaySettings, columnNamesToBeGrouped[i]);
                columnNamesToBeGroupedSortedKeys.Add(columnToBeGroupedKey);
                columnNamesToBeGroupedSorted.Add(columnNamesToBeGrouped[i]);
            }
            for (int i = 0; i < columnNamesToBeSorted?.Count(); i++)
            {
                var columnToBeSortedKey = buildQueryModule.GetColumnKeyByName(displaySettings, columnNamesToBeSorted[i]);
                columnNamesToBeGroupedSortedKeys.Add(columnToBeSortedKey);
                columnNamesToBeGroupedSorted.Add(columnNamesToBeSorted[i]);
            }

            displaySettings = SetGroupingSorting(displaySettings, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted);

            displaySettings.ChangeUserSettings()
                .ChangePageSize(100);

            var updateResult = await datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var displaySettingsAfterAddGroupingSorting = await datasetSettingsModule.GetDataSetSettings(dataset);

            if (!sortOrders.Contains(SortOrder.Tree))
            {
                CheckGroupingSortingInDisplaySettings(displaySettings, displaySettingsAfterAddGroupingSorting, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted);
            }

            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;
            if (!isDynamicFiltersChosen)
            {
                taxonomyFiltersWithItems = TaxonomyFiltersData.GetTaxomonyFiltersItems(dataset);
            }

            var seriesFromGridRequest = await buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettingsAfterAddGroupingSorting, taxonomyFilters, taxonomyFiltersWithItems, requiredFiltersNumber, aggregationIds);
            var seriesFromPageRequest = await buildQueryModule.GetSearchResultsSeriesFromPageRequest(dataset, displaySettingsAfterAddGroupingSorting, taxonomyFilters, taxonomyFiltersWithItems, requiredFiltersNumber, aggregationIds);

            var taxonomyFiltersWithCustomAggregatesNames = TaxonomyFiltersData.GetTaxonomyFiltersNameWithCustomAggregate(taxonomyFilters, aggregationIds);

            SetSeriesToSortingCheck(columnNamesToBeGroupedSortedKeys, buildQueryModule, seriesFromGridRequest, sortOrders, columnNamesToBeGroupedSorted, dataset, taxonomyFiltersWithCustomAggregatesNames);
            SetSeriesToSortingCheck(columnNamesToBeGroupedSortedKeys, buildQueryModule, seriesFromPageRequest, sortOrders, columnNamesToBeGroupedSorted, dataset, taxonomyFiltersWithCustomAggregatesNames);

            var datasetType = Datasets.GetDatasetType(dataset);
            if (datasetType == DatasetType.Magellan)
            {
                var seriesFromOverviewRequest = await buildQueryModule.GetSearchResultsSeriesFromOverviewRequest(dataset, displaySettingsAfterAddGroupingSorting, taxonomyFilters, taxonomyFiltersWithItems, requiredFiltersNumber, aggregationIds);
                SetSeriesToSortingCheck(columnNamesToBeGroupedSortedKeys, buildQueryModule, seriesFromOverviewRequest, sortOrders, columnNamesToBeGroupedSorted, dataset, taxonomyFiltersWithCustomAggregatesNames);
            }
        }

        public static async Task CheckMultiplePagesSorting
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule
            , Dataset dataset, List<SortOrder> sortOrders
            , List<string> columnNamesToBeGrouped = null, List<string> columnNamesToBeSorted = null, int requiredFiltersNumber = 1
            , List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            await CheckGroupingSorting
            (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, requiredFiltersNumber, aggregationIds, isDynamicFiltersChosen);

            var taxonomyFilters = await buildQueryModule.GetTaxonomyFilters(dataset);
            var displaySettings = await datasetSettingsModule.GetDataSetSettings(dataset);

            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;
            if (!isDynamicFiltersChosen)
            {
                taxonomyFiltersWithItems = TaxonomyFiltersData.GetTaxomonyFiltersItems(dataset);
            }

            List<int> pageSizes = new List<int>() { 5, 10, 20, 25, 50, 75 };
            foreach (var pageSize in pageSizes)
            {
                var displaySettingsAfterPageSizeChange = await SetPageSizeChange(datasetSettingsModule, dataset, pageSize);

                var seriesFromGridRequest100 = await buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, requiredFiltersNumber, aggregationIds);
                var seriesFromPageRequest100 = await buildQueryModule.GetSearchResultsSeriesFromPageRequest(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, requiredFiltersNumber, aggregationIds);


                for (int i = 0; i <= GetPagesNumber(pageSize); i++)
                {
                    var offSet = pageSize * i;

                    var seriesFromGridRequest = await buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettingsAfterPageSizeChange, taxonomyFilters, taxonomyFiltersWithItems, requiredFiltersNumber, aggregationIds, offSet: offSet);
                    var seriesFromPageRequest = await buildQueryModule.GetSearchResultsSeriesFromPageRequest(dataset, displaySettingsAfterPageSizeChange, taxonomyFilters, taxonomyFiltersWithItems, requiredFiltersNumber, aggregationIds, offSet: offSet);

                    List<Contract.SearchResults.Series> expectedSeriesGrid;
                    List<Contract.SearchResults.Series> expectedSeriesPage;
                    if (seriesFromGridRequest100.Count() <= pageSize)
                    {
                        expectedSeriesGrid = seriesFromGridRequest100.GetRange(0, seriesFromGridRequest100.Count());
                        expectedSeriesPage = seriesFromPageRequest100.GetRange(0, seriesFromGridRequest100.Count());

                        JsonConvert.SerializeObject(seriesFromGridRequest).Equals(JsonConvert.SerializeObject(expectedSeriesGrid)).Should().BeTrue();
                        JsonConvert.SerializeObject(seriesFromPageRequest).Equals(JsonConvert.SerializeObject(expectedSeriesPage)).Should().BeTrue();

                        break;
                    }
                    else
                    {
                        expectedSeriesGrid = seriesFromGridRequest100.GetRange(0, pageSize);
                        expectedSeriesPage = seriesFromPageRequest100.GetRange(0, pageSize);

                        seriesFromGridRequest100.RemoveRange(0, pageSize);
                        seriesFromPageRequest100.RemoveRange(0, pageSize);

                        JsonConvert.SerializeObject(seriesFromGridRequest).Equals(JsonConvert.SerializeObject(expectedSeriesGrid)).Should().BeTrue();
                        JsonConvert.SerializeObject(seriesFromPageRequest).Equals(JsonConvert.SerializeObject(expectedSeriesPage)).Should().BeTrue();
                    }
                }
            }
        }

        private static int GetPagesNumber(int pageSize)
        {
            var pagesNumber = 100 / pageSize;
            return pagesNumber;
        }

        public static async Task<DataSetSettings> SetPageSizeChange(DatasetSettingsModule datasetSettingsModule, Dataset dataset, int pageSize)
        {
            var displaySettings = await datasetSettingsModule.GetDataSetSettings(dataset);

            displaySettings.ChangeUserSettings()
                .ChangePageSize(pageSize);

            var updateResult = await datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var displaySettingsAfterAddGroupingSorting = await datasetSettingsModule.GetDataSetSettings(dataset);
            return displaySettingsAfterAddGroupingSorting;
        }

        public static async Task<DataSetSettings> SetPageSizeChange(DatasetSettingsModule datasetSettingsModule, Sector sector, int pageSize)
        {
            var displaySettings = await datasetSettingsModule.GetDataSetSettings(sector);

            displaySettings.ChangeUserSettings()
                .ChangePageSize(pageSize);

            var updateResult = await datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var displaySettingsAfterAddGroupingSorting = await datasetSettingsModule.GetDataSetSettings(sector);
            return displaySettingsAfterAddGroupingSorting;
        }

        private static DataSetSettings SetGroupingSorting
            (DataSetSettings displaySettings, List<SortOrder> sortOrders, List<string> columnNamesToBeGrouped = null, List<string> columnNamesToBeSorted = null)
        {
            var columnNamesToBeGroupedCount = 0;
            if (columnNamesToBeGrouped != null)
            {
                List<SortOrder> sorting = new List<SortOrder>();
                for (int i = 0; i < columnNamesToBeGrouped.Count(); i++)
                {
                    sorting.Add(sortOrders[i]);
                }
                displaySettings.ChangeUserSettings()
            .AddMultipleGrouping(columnNamesToBeGrouped, sorting);

                columnNamesToBeGroupedCount = columnNamesToBeGrouped.Count();
            }
            if (columnNamesToBeSorted != null)
            {
                List<SortOrder> sorting = new List<SortOrder>();
                var maximumSortColumnIteratorValue = columnNamesToBeGroupedCount + columnNamesToBeSorted.Count();
                for (int i = columnNamesToBeGroupedCount; i < maximumSortColumnIteratorValue; i++)
                {
                    sorting.Add(sortOrders[i]);
                }
                displaySettings.ChangeUserSettings()
                    .AddMultipleSorting(columnNamesToBeSorted, sorting);
            }
            return displaySettings;
        }

        private static void CheckGroupingSortingInDisplaySettings
            (DataSetSettings dataSetSettingsInitial, DataSetSettings displaySettingsAfterAddGroupingSorting, List<SortOrder> sortOrders, List<string> columnNamesToBeGrouped = null, List<string> columnNamesToBeSorted = null)
        {
            var columnNamesToBeGroupedCount = 0;
            if (columnNamesToBeGrouped != null)
            {
                List<SortOrder> sorting = new List<SortOrder>();
                for (int i = 0; i < columnNamesToBeGrouped.Count(); i++)
                {
                    sorting.Add(sortOrders[i]);
                }
                displaySettingsAfterAddGroupingSorting.UserSettings.Grouping.Should().BeEquivalentTo(dataSetSettingsInitial.UserSettings.Grouping);
                displaySettingsAfterAddGroupingSorting.UserSettings.Grouping.Select(x => x.SortOrder).Should().BeEquivalentTo(sorting);

                columnNamesToBeGroupedCount = columnNamesToBeGrouped.Count();
            }
            if (columnNamesToBeSorted != null)
            {
                List<SortOrder> sorting = new List<SortOrder>();
                var maximumSortColumnIteratorValue = columnNamesToBeGroupedCount + columnNamesToBeSorted.Count();
                for (int i = columnNamesToBeGroupedCount; i < maximumSortColumnIteratorValue; i++)
                {
                    sorting.Add(sortOrders[i]);
                }
                displaySettingsAfterAddGroupingSorting.UserSettings.Sorting.Should().BeEquivalentTo(dataSetSettingsInitial.UserSettings.Sorting);
                displaySettingsAfterAddGroupingSorting.UserSettings.Sorting.Select(x => x.SortOrder).Should().BeEquivalentTo(sorting);
            }
        }

        public static async Task CheckNewMnemonicsCalculation(BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, int caseNumber)
        {
            var taxonomyFilters = await buildQueryModule.GetTaxonomyFilters(dataset);
            var displaySettings = await datasetSettingsModule.GetDataSetSettings(dataset);

            var newMnemonicsColumnName = "API mnemonics";
            displaySettings = await buildQueryModule.ShowHideColumn(dataset, displaySettings, newMnemonicsColumnName, true);

            Dictionary<string, List<string>> taxonomyFiltersWithItems = NewMnemonicsData.GetTaxomonyFiltersItems(caseNumber);

            var seriesFromGridRequest = await buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);


            var columnNewMnemonicKey = buildQueryModule.GetColumnKeyByName(displaySettings, newMnemonicsColumnName);
            var newMnemonicsColumnValue = buildQueryModule.GetGridValuesForParticularColumn(seriesFromGridRequest, columnNewMnemonicKey).First();

            var taxonomyFiltersWithFiltersIds = TaxonomyFiltersData.GetFilterIdByName(taxonomyFilters, taxonomyFiltersWithItems);

            var conceptCode = taxonomyFiltersWithFiltersIds[taxonomyFilters.Filters.Single(x => x.Name == "Concept")].First();
            conceptCode = conceptCode.Split('&').Last();
            conceptCode = conceptCode.Remove(conceptCode.IndexOf('['), 1);
            conceptCode = conceptCode.Remove(conceptCode.IndexOf(']'), 1);
            var geoCode = taxonomyFiltersWithFiltersIds[taxonomyFilters.Filters.Single(x => x.Name == "Geography")].First();
            geoCode = geoCode.Split('&').Last();
            geoCode = geoCode.Remove(geoCode.IndexOf('['), 1);
            geoCode = geoCode.Remove(geoCode.IndexOf(']'), 1);
            var industryCode = taxonomyFiltersWithFiltersIds[taxonomyFilters.Filters.Single(x => x.Name == "Industry")].First();
            industryCode = industryCode.Split('&').Last();
            industryCode = industryCode.Remove(industryCode.IndexOf('['), 1);
            industryCode = industryCode.Remove(industryCode.IndexOf(']'), 1);

            var expectedNewMnemonic = NewMnemonicsData.GetNewMnemonic(conceptCode, geoCode, industryCode);

            newMnemonicsColumnValue.Should().BeEquivalentTo(expectedNewMnemonic, "see case number {0} to get data to verify", caseNumber);
        }
    }
}
