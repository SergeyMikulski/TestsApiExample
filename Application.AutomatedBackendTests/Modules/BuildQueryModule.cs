using Application.AutomatedBackendTests.Builders;
using Application.AutomatedBackendTests.Extensions;
using Application.AutomatedBackendTests.Models;
using Application.AutomatedBackendTests.Requests;
using Application.AutomatedBackendTests.TestData;
using Application.Contract;
using Application.Contract.Filters;
using Application.Contract.Functions;
using Application.Contract.SearchResults;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.AutomatedBackendTests.Modules
{
    public class BuildQueryModule
    {
        private readonly GridRequests _gridRequests;
        private readonly FilterRequests _filterRequests;
        private readonly User _user;

        public BuildQueryModule(User user)
        {
            _filterRequests = new FilterRequests(user);
            _gridRequests = new GridRequests(user);
            _user = user;
        }



        private SearchOptions AddSortingOnGroupingToSearchOptionsRequest(SearchOptions searchOptions, DataSetSettings settings)
        {
            if ((settings.UserSettings.Grouping != null) && (settings.UserSettings.Grouping.ToList().Count > 0))
            {
                List<SortInfo> sortInfoNew = new List<SortInfo>();
                var initialSortingCount = searchOptions.Sort != null ? searchOptions.Sort.Count() : 0;
                for (int i = initialSortingCount; i > 0; i--)
                {
                    sortInfoNew.Add(searchOptions.Sort.ToList()[i - 1]);
                }

                for (int i = settings.UserSettings.Grouping.Count(); i > 0; i--)
                {
                    SortInfo sortInfo = new SortInfo()
                    {
                        Field = settings.UserSettings.Grouping.ToList()[i - 1].Field,
                        SortOrder = settings.UserSettings.Grouping.ToList()[i - 1].SortOrder
                    };
                    sortInfoNew.Add(sortInfo);
                }
                sortInfoNew.Reverse();
                searchOptions = new SearchOptions
                {
                    AppliedFunctions = searchOptions.AppliedFunctions,
                    DateRange = searchOptions.DateRange,
                    Filters = searchOptions.Filters,
                    ObservationsLimit = 120,
                    Page = searchOptions.Page,
                    Sort = sortInfoNew,
                    GroupedColumns = searchOptions.GroupedColumns,
                    IsSubtotalsEnabled = searchOptions.IsSubtotalsEnabled
                };
            }
            return searchOptions;
        }

        public async Task<DataSetSettings> ResetSettingsInGrid(Dataset dataset)
        {
            var dataSetForQuery = GetDataSetId(dataset);

            return await _gridRequests.Reset(dataSetForQuery)
                .ConfigureAwait(false);
        }

        public async Task<DataSetSettings> ResetSettingsInGrid(Sector sector)
        {
            var sectorForQuery = GetDataSetId(sector);

            return await _gridRequests.Reset(sectorForQuery)
                .ConfigureAwait(false);
        }

        public async Task ResetSettingsInTaxonomyFilters(Dataset dataset, DatasetSettingsModule datasetSettingsModule)
        {
            var displaySettings = await datasetSettingsModule.GetDataSetSettings(dataset);

            var defaultTaxonomyFiltersList = displaySettings.AvailableFilters;

            foreach (var defaultTaxonomyFilter in defaultTaxonomyFiltersList)
            {
                displaySettings.UserSettings.Filters.ToList().Add(
                    new FilterState
                    {
                        Id = defaultTaxonomyFilter.Id,
                        Visible = defaultTaxonomyFilter.Visible
                    });
            }

            var defaultFunctionsList = GetDefaultFunctions(dataset);

            displaySettings.UserSettings.AppliedFunctions = new AppliedFunctionSettings()
            {
                FunctionComposition = new List<AppliedFunction>(),
                Functions = defaultFunctionsList,
                ReplaceOriginalSeries = false,
                Rebasing = new RebasingSettings()
            };

            var updateResult = await datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);
        }

        private List<AppliedFunction> GetDefaultFunctions(Dataset dataset)
        {
            if (dataset == Dataset.WorldMarketMonitor)
            {
                return new List<AppliedFunction>()
                {
                    new AppliedFunction
                    {
                        FunctionId = (int)Function.Annualization, Parameter = "Original"
                    },
                    new AppliedFunction
                    {
                        FunctionId = (int)Function.CurrencyConversion, Parameter = "ORIGINAL"
                    },
                    new AppliedFunction
                    {
                        FunctionId = (int)Function.Rescaling, Parameter = "Harmonized"
                    }
                };
            }

            if ((dataset == Dataset.AssetCapacityByCompany) || (dataset == Dataset.AssetCapacityByShareholder) || (dataset == Dataset.AggregatedAssetByGeography))
            {
                return new List<AppliedFunction>()
                {
                    new AppliedFunction
                    {
                        FunctionId = (int)Function.UnitConversion, Parameter = "000MetricTons"
                    }
                };
            }

            if (dataset == Dataset.GlobalEconomyNew)
            {
                return new List<AppliedFunction>()
                {
                    new AppliedFunction
                    {
                        FunctionId = (int)Function.CurrencyConversion, Parameter = "ORIGINAL"
                    },
                    new AppliedFunction
                    {
                        FunctionId = (int)Function.Rescaling, Parameter = "Harmonized"
                    }
                };
            }

            if ((dataset == Dataset.Banking) || (dataset == Dataset.PricingAndPurchasingForecasts))
            {
                return new List<AppliedFunction>()
                {
                    new AppliedFunction
                    {
                        FunctionId = (int)Function.CurrencyConversion, Parameter = "ORIGINAL"
                    }
                };
            }

            if ((dataset == Dataset.ConstructionGlobal) || (dataset == Dataset.ComparativeIndustryRev4))
            {
                return new List<AppliedFunction>()
                {
                    new AppliedFunction
                    {
                        FunctionId = (int)Function.CurrencyConversion, Parameter = "[Currency].[Currency].&[USREXA]"
                    }
                };
            }

            return new List<AppliedFunction>();
        }

        public async Task<List<ColumnFilterInfo>> GetAvailableColumnFilters(Dataset dataset)
        {
            var dataSetForQuery = GetDataSetId(dataset);

            return await _gridRequests.AvailableColumnFilters(dataSetForQuery)
                .ConfigureAwait(false);
        }

        public DataSetId GetDataSetId(Dataset dataset)
        {
            DataSetId dataSetForQuery = new DataSetId()
            {
                Id = dataset.GetDatasetInfo().GetId(),
                Type = DataSetType.DataSet
            };

            return dataSetForQuery;
        }

        public DataSetId GetDataSetId(Sector sector)
        {
            return sector == Sector.None ?
            new DataSetId()
            {
                Id = (int)sector,
                Type = DataSetType.All
            }
            :
            new DataSetId()
            {
                Id = (int)sector,
                Type = DataSetType.Sector
            };
        }

        public async Task<SeriesSearchResultsWithFrequencies> GetSearchResults(SearchOptions searchOptions)
        {
            return await _gridRequests.Grid(searchOptions)
                .ConfigureAwait(false);
        }

        public async Task<TaxonomyFilters> GetTaxonomyFilters(Dataset dataset, DataSetSettings dataSetSettings = null)
        {
            return await _filterRequests.GetTaxonomyFilters(dataset, dataSetSettings)
                .ConfigureAwait(false);
        }

        private SearchFilters SetEachTaxonomyFilterValues
            (Dataset dataset, TaxonomyFilters taxonomyFilters, int requiredFiltersNumber = 1, List<string> aggregationIds = null)
        {
            Dictionary<TaxonomyFilter, List<string>> taxonomyFiltersWithValues = new Dictionary<TaxonomyFilter, List<string>>();

            foreach (var taxonomyFilter in taxonomyFilters.Filters)
            {
                if (taxonomyFilter.Id == "frequency")
                {
                    List<string> frequencyList = new List<string>
                    {
                        "1"
                    };
                    taxonomyFiltersWithValues.Add(taxonomyFilter, frequencyList);
                }
                else
                {
                    var filtersForAggregate = ChooseFilterValues(taxonomyFilter, requiredFiltersNumber, aggregationIds);
                    taxonomyFiltersWithValues.Add(taxonomyFilter, filtersForAggregate);
                }
            }

            var searchFilters = new SearchFiltersBuilder()
                .SetFilterValues(taxonomyFilters, taxonomyFiltersWithValues)
                .SetDataset(dataset)
                .SetFrequency()
                .Build();

            return searchFilters;
        }

        private List<string> ChooseFilterValues(TaxonomyFilter taxonomyFilter, int requiredFiltersNumber = 1, List<string> aggregationIds = null)
        {
            IEnumerable<TaxonomyFilterItem> filtersForAggregate;
            string MyCustomDimensions = "MyCustomDimensions";

            var filters = taxonomyFilter.Items
                           .First(i => !i.Id.Equals(MyCustomDimensions))
                           .Children
                           .ElementAtOrDefault(0);

            if ((filters != null) && (filters.Children.Count() > 0))
            {
                filtersForAggregate = filters
                   .Children
                   .Take(requiredFiltersNumber);
            }
            else
            {
                filtersForAggregate = taxonomyFilter.Items
                   .First(i => !i.Id.Equals(MyCustomDimensions))
                   .Children
                   .Take(requiredFiltersNumber);
            }

            if (filtersForAggregate.Count() == 0)
            {
                filtersForAggregate = taxonomyFilter.Items
                .Where(i => !i.Id.Equals(MyCustomDimensions))
                .Take(requiredFiltersNumber);
            }

            List<string> filtersForAggregateIds = new List<string>();
            foreach (var filter in filtersForAggregate)
            {
                filtersForAggregateIds.Add(filter.Id);
            }

            var aggregationIdsNumber = aggregationIds != null ? aggregationIds.Count() : 0;
            for (int k = 0; k < aggregationIdsNumber; k++)
            {
                bool isfiltersContainsCustomAggregate = false;
                if (taxonomyFilter.Items.FirstOrDefault(i => i.Id.Equals(MyCustomDimensions)) != null)
                {
                    isfiltersContainsCustomAggregate = taxonomyFilter.Items
                       .FirstOrDefault(i => i.Id.Equals(MyCustomDimensions))
                       .Children.Any(x => x.Id.Equals(aggregationIds[k]));
                }

                if (isfiltersContainsCustomAggregate)
                {
                    filtersForAggregateIds.Add(aggregationIds[k]);
                }
            }

            return filtersForAggregateIds;
        }

        #region GetSeries and SearchResults region
        public async Task<SeriesSearchResultsWithFrequencies> GetSearchResults(DataSetSettings settings, SearchFilters searchFilters, int offSet = 0)
        {
            var searchOptions = settings.ExtractSearchOptions(searchFilters, offSet);
            searchOptions = AddSortingOnGroupingToSearchOptionsRequest(searchOptions, settings);
            return await _gridRequests.Grid(searchOptions)
                .ConfigureAwait(false);
        }

        public async Task<PageResults> GetSearchResultsFromPageRequest(DataSetSettings settings, SearchFilters searchFilters, int offSet = 0)
        {
            var searchOptions = settings.ExtractSearchOptions(searchFilters, offSet);
            searchOptions = AddSortingOnGroupingToSearchOptionsRequest(searchOptions, settings);
            return await _gridRequests.Page(searchOptions)
                .ConfigureAwait(false);
        }

        public async Task<PageResults> GetSearchResultsFromOverviewRequest(DataSetSettings settings, SearchFilters searchFilters, int offSet = 0)
        {
            var searchOptions = settings.ExtractSearchOptions(searchFilters, offSet);
            searchOptions = AddSortingOnGroupingToSearchOptionsRequest(searchOptions, settings);
            return await _gridRequests.Overview(searchOptions)
                .ConfigureAwait(false);
        }

        private SearchFilters SetSearchFilters
            (Dataset dataset, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , int requiredFiltersNumber = 1, List<string> aggregationIds = null, string frequency = "Annual", string keyword = "")
        {
            SearchFilters searchFilters;
            if (taxonomyFilters == null)
            {
                return TaxonomyFiltersData.SetSearchFiltersNoTaxonomyChosen(dataset, frequency, keyword);
            }

            if (taxonomyFiltersWithItems != null)
            {
                searchFilters = TaxonomyFiltersData.GetSearchFilters(dataset, taxonomyFilters, taxonomyFiltersWithItems, aggregationIds, frequency);
            }
            else
            {
                searchFilters = SetEachTaxonomyFilterValues(dataset, taxonomyFilters, requiredFiltersNumber, aggregationIds);
            }

            return searchFilters;
        }

        private SearchFilters SetSearchFilters(Sector sector, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , bool isFrequencyChosen = true, string frequency = "Annual", string keyword = "")
        {
            var searchFilters = TaxonomyFiltersData.GetSearchFilters(sector, taxonomyFilters, taxonomyFiltersWithItems, isFrequencyChosen, frequency, keyword);

            return searchFilters;
        }

        public async Task<PageResults> GetSeriesFromPageRequest
            (Dataset dataset, DataSetSettings settings, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , int requiredFiltersNumber = 1, List<string> aggregationIds = null, string frequency = "Annual", int offSet = 0)
        {
            var searchFilters = SetSearchFilters(dataset, taxonomyFilters, taxonomyFiltersWithItems, requiredFiltersNumber, aggregationIds, frequency);

            var series = await GetSearchResultsFromPageRequest(settings, searchFilters, offSet);
            return series;
        }

        public async Task<PageResults> GetSeriesFromPageRequest
            (Sector sector, DataSetSettings settings, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , int offSet = 0, bool isFrequencyChosen = true, string frequency = "Annual", string keyword = "")
        {
            var searchFilters = SetSearchFilters(sector, taxonomyFilters, taxonomyFiltersWithItems, isFrequencyChosen, frequency, keyword);

            var series = await GetSearchResultsFromPageRequest(settings, searchFilters, offSet);
            return series;
        }

        public async Task<List<Contract.SearchResults.Series>> GetSearchResultsSeriesFromPageRequest
            (Dataset dataset, DataSetSettings settings, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , int requiredFiltersNumber = 1, List<string> aggregationIds = null, string frequency = "Annual", int offSet = 0)
        {
            var series = await GetSeriesFromPageRequest(dataset, settings, taxonomyFilters, taxonomyFiltersWithItems, requiredFiltersNumber, aggregationIds, frequency, offSet);
            return series.Series.ToList();
        }

        public async Task<List<Contract.SearchResults.Series>> GetSearchResultsSeriesFromPageRequest
            (Sector sector, DataSetSettings settings, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , int offSet = 0, bool isFrequencyChosen = true, string frequency = "Annual", string keyword = "")
        {
            var series = await GetSeriesFromPageRequest(sector, settings, taxonomyFilters, taxonomyFiltersWithItems, offSet, isFrequencyChosen, frequency, keyword);
            return series.Series.ToList();
        }

        public async Task<SeriesSearchResultsWithFrequencies> GetSeriesHighLevelAttributesFromGrid
            (Dataset dataset, DataSetSettings settings, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , int requiredFiltersNumber = 1, List<string> aggregationIds = null, string frequency = "Annual", int offSet = 0, string keyword = "")
        {
            var searchFilters = SetSearchFilters(dataset, taxonomyFilters, taxonomyFiltersWithItems, requiredFiltersNumber, aggregationIds, frequency, keyword);

            var series = await GetSearchResults(settings, searchFilters, offSet);
            return series;
        }

        public async Task<SeriesSearchResultsWithFrequencies> GetSeriesHighLevelAttributesFromGrid
            (Sector sector, DataSetSettings settings, TaxonomyFilters taxonomyFilters = null, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , bool isFrequencyChosen = true, string frequency = "Annual", int offSet = 0, string keyword = "")
        {
            var searchFilters = SetSearchFilters(sector, taxonomyFilters, taxonomyFiltersWithItems, isFrequencyChosen, frequency, keyword);

            var series = await GetSearchResults(settings, searchFilters, offSet);
            return series;
        }

        public async Task<List<Contract.SearchResults.Series>> GetSearchResultsSeriesFromGrid
            (Dataset dataset, DataSetSettings settings, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , int requiredFiltersNumber = 1, List<string> aggregationIds = null, string frequency = "Annual", int offSet = 0, string keyword = "")
        {
            var series = await GetSeriesHighLevelAttributesFromGrid(dataset, settings, taxonomyFilters, taxonomyFiltersWithItems, requiredFiltersNumber, aggregationIds, frequency, offSet, keyword);
            return series.Series.ToList();
        }

        public async Task<List<Contract.SearchResults.Series>> GetSearchResultsSeriesFromGrid
            (Sector sector, DataSetSettings settings, TaxonomyFilters taxonomyFilters = null, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , bool isFrequencyChosen = true, string frequency = "Annual", int offSet = 0, string keyword = "")
        {
            var series = await GetSeriesHighLevelAttributesFromGrid(sector, settings, taxonomyFilters, taxonomyFiltersWithItems, isFrequencyChosen, frequency, offSet, keyword);
            return series.Series.ToList();
        }


        public async Task<PageResults> GetSeriesFromOverviewRequest
            (Dataset dataset, DataSetSettings settings, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , int requiredFiltersNumber = 1, List<string> aggregationIds = null, int offSet = 0, string frequency = "")
        {
            var searchFilters = SetSearchFilters(dataset, taxonomyFilters, taxonomyFiltersWithItems, requiredFiltersNumber, aggregationIds, frequency);

            var series = await GetSearchResultsFromOverviewRequest(settings, searchFilters, offSet);
            return series;
        }

        public async Task<PageResults> GetSeriesFromOverviewRequest
            (Sector sector, DataSetSettings settings, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , bool isFrequencyChosen = true, int offSet = 0, string frequency = "", string keyword = "")
        {
            var searchFilters = SetSearchFilters(sector, taxonomyFilters, taxonomyFiltersWithItems, isFrequencyChosen, frequency, keyword);

            var series = await GetSearchResultsFromOverviewRequest(settings, searchFilters, offSet);
            return series;
        }

        public async Task<List<Contract.SearchResults.Series>> GetSearchResultsSeriesFromOverviewRequest
            (Dataset dataset, DataSetSettings settings, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , int requiredFiltersNumber = 1, List<string> aggregationIds = null, int offSet = 0, string frequency = "")
        {
            var series = await GetSeriesFromOverviewRequest(dataset, settings, taxonomyFilters, taxonomyFiltersWithItems, requiredFiltersNumber, aggregationIds, offSet, frequency);
            return series.Series.ToList();
        }

        public async Task<List<Contract.SearchResults.Series>> GetSearchResultsSeriesFromOverviewRequest
            (Sector sector, DataSetSettings settings, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , bool isFrequencyChosen = true, int offSet = 0, string frequency = "", string keyword = "")
        {
            var series = await GetSeriesFromOverviewRequest(sector, settings, taxonomyFilters, taxonomyFiltersWithItems, isFrequencyChosen, offSet, frequency, keyword);
            return series.Series.ToList();
        }
        #endregion

        public string GetColumnKeyByName(DataSetSettings displaySettings, string columnName)
        {
            var columns = displaySettings.UserSettings.Columns.Where(x => x.HeaderText == columnName);
            var columnKey = columns.Select(x => x.Key).ToList()[0];
            return columnKey;
        }

        public string GetColumnNameByKey(DataSetSettings displaySettings, string columnKey)
        {
            var columns = displaySettings.UserSettings.Columns.Where(x => x.Key == columnKey);
            var columnName = columns.Select(x => x.HeaderText).ToList()[0];
            return columnName;
        }

        public List<string> GetGridValuesForParticularColumn(List<Contract.SearchResults.Series> series, string columnToBeSortedKey)
        {
            List<string> columnsValues = new List<string>();
            var seriesAttributesList = series.Select(x => x.Attributes);

            foreach (var attribute in seriesAttributesList)
            {
                var seriesAttributesValuesForChosenColumn = attribute.Where(x => x.Key == columnToBeSortedKey).ToDictionary(k => k.Key, n => n.Value);
                var attributeInTheList = seriesAttributesValuesForChosenColumn.Values;

                foreach (var column in attributeInTheList)
                {
                    var columnValue = column.Select(x => x.DisplayName).ToList();
                    columnsValues.Add(columnValue[0]);
                }
            }

            return columnsValues;
        }

        public async Task<DataSetSettings> ShowHideColumn(Dataset dataset, DataSetSettings displaySettings, string columnName, bool isColumnShouldBeDisplayed)
        {
            if (displaySettings.UserSettings.Columns.Single(x => x.HeaderText == columnName).Hidden != isColumnShouldBeDisplayed)
            {
                displaySettings.UserSettings.Columns.Single(x => x.HeaderText == columnName).Hidden = isColumnShouldBeDisplayed;

                var expectedResponse = new UpdateFiltersResponse { Success = true };
                var DatasetSettingsModule = new DatasetSettingsModule(_user);
                var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(displaySettings);
                updateResult.Should().BeEquivalentTo(expectedResponse);

                displaySettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
            }
            return displaySettings;
        }

        public bool IsAnySeriesValueNotEmpty(List<Contract.SearchResults.Series> series)
        {
            var seriesValuesList = series.Select(x => x.Values);

            foreach (var seriesValues in seriesValuesList)
            {

                foreach (var value in seriesValues)
                {
                    if (value.Value != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsAnySeriesHasNullValuesOnly(List<Contract.SearchResults.Series> seriesList)
        {
            foreach (var series in seriesList)
            {
                var seriesValues = series.Values.ToList();
                for (int i = 0; i < seriesValues.Count(); i++)
                {
                    if (seriesValues[i].Value != null)
                    {
                        break;
                    }

                    if (i == (seriesValues.Count() - 1))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool IsAnySeriesValueChanged(List<Contract.SearchResults.Series> seriesInitial, List<Contract.SearchResults.Series> seriesResult)
        {
            var seriesInitialValuesList = seriesInitial.Select(x => x.Values);
            var seriesResultValuesList = seriesResult.Select(x => x.Values);

            for (int i = 0; i < seriesInitial.Count(); i++)
            {
                var seriesInitialValues = seriesInitial[i];
                var seriesResultValues = seriesResult[i];
                for (int k = 0; k < seriesInitialValues.Values.Count(); k++)
                {
                    var seriesInitialValue = seriesInitialValues.Values.ToList()[k];
                    var seriesResultValue = seriesResultValues.Values.ToList()[k];

                    if (seriesInitialValue?.Value != seriesResultValue?.Value)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public async Task AddFunctionsToQuery(Dataset dataset, DatasetSettingsModule datasetSettingsModule, DataSetSettings displaySettings, List<AppliedFunction> appliedFunctions)
        {
            displaySettings.UserSettings.AppliedFunctions = new AppliedFunctionSettings(appliedFunctions);

            var updateResult = await datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);
        }

        //public Dictionary<string, List<string>> WaspDatasetSetMinimalQuery(TaxonomyFilters taxonomyFilters)
        //{
        //    var versionTaxonomyFilterName = "Version";

        //    var taxonomyFiltersWithItems = new Dictionary<string, List<string>>();
        //    foreach (var taxonomyFilter in taxonomyFilters.Filters)
        //    {
        //        if (taxonomyFilter.Name == versionTaxonomyFilterName)
        //        {
        //            taxonomyFiltersWithItems.Add(versionTaxonomyFilterName, new List<string>() { taxonomyFilter.Items.First().Name });
        //        }
        //        else
        //        {
        //            taxonomyFiltersWithItems.Add(taxonomyFilter.Name, new List<string>() { });
        //        }
        //    }

        //    return taxonomyFiltersWithItems;
        //}
    }
}