using Application.AutomatedBackendTests.Builders;
using Application.AutomatedBackendTests.Extensions;
using Application.AutomatedBackendTests.Models;
using Application.AutomatedBackendTests.Modules;
using Application.AutomatedBackendTests.Requests;
using Application.AutomatedBackendTests.TestData;
using Application.Contract;
using Application.Contract.Filters;
using Application.Contract.Functions;
using Application.Contract.SearchResults;
using FluentAssertions;
using FluentAssertions.Execution;
using Frequencies;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Application.AutomatedBackendTests.Tests.Grid
{
    [TestFixture]
    [Category("ApplicationBackendTests")]
    public class GridTests
    {
        private readonly User _user = User.SettingsUser;
        private DatasetSettingsModule _datasetSettingsModule;
        private BuildQueryModule _buildQueryModule;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _datasetSettingsModule = new DatasetSettingsModule(_user);
            _buildQueryModule = new BuildQueryModule(_user);
        }

        [TestCase(Dataset.AssetCapacityByCompany)]
        [TestCase(Dataset.AssetCapacityByShareholder)]
        [TestCase(Dataset.AggregatedAssetByGeography)]
        public async Task ExcludeSeriesFromChartEditorFlagShouldBeSetToTrueForChemAssetsSeries(Dataset dataset)
        {
            await CheckExcludeSeriesFromChartEditorFlag(dataset, true);
        }

        [TestCase(Dataset.Banking)]
        [TestCase(Dataset.HealthcareForecast)]
        [TestCase(Dataset.WorldMarketMonitor)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task ExcludeSeriesFromChartEditorFlagShouldBeSetToFalseForNonChemAssetsSeries(Dataset dataset)
        {
            await CheckExcludeSeriesFromChartEditorFlag(dataset, false);
        }

        private async Task CheckExcludeSeriesFromChartEditorFlag(Dataset dataset, bool expectedValue)
        {
            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
            var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);

            var searchFilters = new SearchFiltersBuilder()
                .SetEmptyFilters(taxonomyFilters)
                .SetDataset(dataset)
                .SetFrequency()
                .Build();

            var grid = await _buildQueryModule.GetSearchResults(displaySettings, searchFilters);
            foreach (var gridSeries in grid.Series)
            {
                gridSeries.ExcludeSeriesFromChartEditor.Should().Be(expectedValue);
            }
        }

        [Test]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task CubesResultTruncationNotificationTest(Dataset dataset)
        {
            var expectedTruncationSeriesMessage = "To optimize your performance, some results and columns are not displayed. Please export the table to view complete set of results and columns.";

            var parameters = await SetInitialParametersForComparativeIndustryTruncationTest(dataset);
            var response = await _buildQueryModule.GetSearchResults(parameters);

            using (new AssertionScope())
            {
                response.TruncationInfo.TruncationMessage.Should().Be(expectedTruncationSeriesMessage);
                response.TruncationInfo.AreObservationsTruncated.Should().BeTrue();
            }
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task NewMnemonicsCalculationTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var maximalCaseNumber = 18;
            for (int caseNumber = 0; caseNumber <= maximalCaseNumber; caseNumber++)
            {
                await CommonTestsParts.CheckNewMnemonicsCalculation(_buildQueryModule, _datasetSettingsModule, dataset, caseNumber);
            }

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Sector.MaritimeAndTrade)]
        public async Task CubeSeriesNotEmptyInSectorTest(Sector sector)
        {
            await _buildQueryModule.ResetSettingsInGrid(sector);

            TaxonomyFilters taxonomyFilters = null;
            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(sector);

            displaySettings.ChangeUserSettings()
                .ChangePageSize(20);

            var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var displaySettingsAfterChange = await _datasetSettingsModule.GetDataSetSettings(sector);

            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            var seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(sector, displaySettingsAfterChange, taxonomyFilters, taxonomyFiltersWithItems);
            var seriesFromPageRequest = await _buildQueryModule.GetSearchResultsSeriesFromPageRequest(sector, displaySettingsAfterChange, taxonomyFilters, taxonomyFiltersWithItems);

            _buildQueryModule.IsAnySeriesValueNotEmpty(seriesFromGridRequest).Should().BeTrue("cube series from sectors should have values");
            _buildQueryModule.IsAnySeriesValueNotEmpty(seriesFromPageRequest).Should().BeTrue("cube series from sectors should have values");
        }

        [Test]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task CISLocalCurrencyHasValuesTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);
            await _buildQueryModule.ResetSettingsInTaxonomyFilters(dataset, _datasetSettingsModule);
            var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            displaySettings.ChangeUserSettings()
                .ChangePageSize(100);

            var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var displaySettings100SeriesPerPage = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var taxonomyFiltersWithItems = TaxonomyFiltersData.GetTaxomonyFiltersItems(dataset);

            var seriesFromGridRequestInitial = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings100SeriesPerPage, taxonomyFilters, taxonomyFiltersWithItems);
            var seriesFromPageRequestInitial = await _buildQueryModule.GetSearchResultsSeriesFromPageRequest(dataset, displaySettings100SeriesPerPage, taxonomyFilters, taxonomyFiltersWithItems);

            List<AppliedFunction> appliedFunctions = new List<AppliedFunction>()
            {
                new AppliedFunction
                {
                    FunctionId = (int)Function.CurrencyConversion,
                    Parameter = "LCU"
                }
            }.ToList();

            await _buildQueryModule.AddFunctionsToQuery(dataset, _datasetSettingsModule, displaySettings, appliedFunctions);

            var displaySettingsAfterChange = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettingsAfterChange, taxonomyFilters, taxonomyFiltersWithItems);
            var seriesFromPageRequest = await _buildQueryModule.GetSearchResultsSeriesFromPageRequest(dataset, displaySettingsAfterChange, taxonomyFilters, taxonomyFiltersWithItems);

            _buildQueryModule.IsAnySeriesValueChanged(seriesFromGridRequestInitial, seriesFromGridRequest).Should().BeTrue();
            _buildQueryModule.IsAnySeriesValueChanged(seriesFromPageRequestInitial, seriesFromPageRequest).Should().BeTrue();

            await _buildQueryModule.ResetSettingsInTaxonomyFilters(dataset, _datasetSettingsModule);
        }

        [TestCaseSource("GetAllDatasets")]
        public async Task ColumnsHaveFiltersTest(Dataset dataset)
        {
            var datasetType = Datasets.GetDatasetType(dataset);

            if ((datasetType != DatasetType.DataPlatform) & (dataset != Dataset.ChemAssetChemicalCapacityFormattedTables))
            {
                if (datasetType == DatasetType.Cube)
                {
                    await CheckCubesFilters(datasetType, dataset);
                }
                else
                {
                    var expectedColumnsWithFiltersMaximum = await GetExpectedColumnKeysHavingFilters(datasetType, dataset);

                    List<string> expectedColumnsWithFilters = new List<string>();
                    var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
                    var datasetColumnNames = displaySettings.UserSettings.Columns.Select(x => x.Key);
                    foreach (var column in expectedColumnsWithFiltersMaximum)
                    {
                        if (datasetColumnNames.Contains(column))
                        {
                            expectedColumnsWithFilters.Add(column);
                        }
                    }

                    var filters = await _buildQueryModule.GetAvailableColumnFilters(dataset);

                    List<string> columnKeys = new List<string>();
                    foreach (var filter in filters)
                    {
                        columnKeys.Add(filter.ColumnKey);
                    }

                    expectedColumnsWithFilters.Sort();
                    columnKeys.Sort();

                    columnKeys.SequenceEqual(expectedColumnsWithFilters).Should().BeTrue();
                }
            }
        }

        private async Task CheckCubesFilters(DatasetType datasetType, Dataset dataset)
        {
            var expectedFilterIdsFormTaxomomyFilters = await GetExpectedColumnKeysHavingFilters(datasetType, dataset);

            List<string> expectedFilterIdsWithoutFrequency = new List<string>();
            for (int i = 0; i < expectedFilterIdsFormTaxomomyFilters.Count(); i++)
            {
                if (!expectedFilterIdsFormTaxomomyFilters[i].Contains("Frequency"))
                {
                    expectedFilterIdsWithoutFrequency.Add(expectedFilterIdsFormTaxomomyFilters[i]);
                }
            }

            var columns = await _buildQueryModule.GetAvailableColumnFilters(dataset);
            var filterIds = columns.Select(x => x.Id).ToList();

            expectedFilterIdsWithoutFrequency.Sort();
            filterIds.Sort();

            filterIds.SequenceEqual(expectedFilterIdsWithoutFrequency).Should().BeTrue();
        }

        [TestCase(Dataset.Banking)]
        [TestCase(Dataset.ConstructionUS)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task ChangeFrequencyTest(Dataset dataset)
        {
            var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            displaySettings.ChangeUserSettings()
                .ChangePageSize(100);

            var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var displaySettingsAfterChangePageSize = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var taxonomyFiltersWithItems = TaxonomyFiltersData.GetTaxomonyFiltersItems(dataset);

            await SeriesContainExpectedFrequency(dataset, displaySettingsAfterChangePageSize, taxonomyFilters, taxonomyFiltersWithItems, "Annual");
            await SeriesContainExpectedFrequency(dataset, displaySettingsAfterChangePageSize, taxonomyFilters, taxonomyFiltersWithItems, "Quarterly");

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Sector.None)]
        [TestCase(Sector.CountryRisk)]
        public async Task ChangeFrequencyTest(Sector sector)
        {
            TaxonomyFilters taxonomyFilters = null;
            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(sector);

            displaySettings.ChangeUserSettings()
                .ChangePageSize(100);

            var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var displaySettingsAfterChangePageSize = await _datasetSettingsModule.GetDataSetSettings(sector);

            await SeriesContainExpectedFrequency(sector, displaySettingsAfterChangePageSize, taxonomyFilters, taxonomyFiltersWithItems, "Annual");
            await SeriesContainExpectedFrequency(sector, displaySettingsAfterChangePageSize, taxonomyFilters, taxonomyFiltersWithItems, "Quarterly");

            await _buildQueryModule.ResetSettingsInGrid(sector);
        }

        [TestCase(Dataset.CanadianEconomy)]
        [TestCase(Dataset.HealthcareForecast)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task PagingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);
            TaxonomyFilters taxonomyFilters = null;
            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            var seriesToCount = await _buildQueryModule.GetSeriesHighLevelAttributesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, offSet: 0);
            var totalSeriesCount = seriesToCount.TotalCount;

            var datasetType = Datasets.GetDatasetType(dataset);
            string frequency;
            if (datasetType == DatasetType.Magellan)
            {
                frequency = "";
            }
            else
            {
                frequency = "Annual";
            }

            List<int> pageSizes = new List<int>() { 5, 10, 20, 25, 50, 75, 100 };
            foreach (var pageSize in pageSizes)
            {
                var displaySettingsAfterPageSizeChange = await CommonTestsParts.SetPageSizeChange(_datasetSettingsModule, dataset, pageSize);

                var firstPageOffset = 0;
                var secondPageOffSet = pageSize;
                int lastPageOffset;
                if ((int)totalSeriesCount % pageSize == 0)
                {
                    lastPageOffset = ((int)totalSeriesCount / pageSize - 1) * pageSize;
                }
                else
                {
                    lastPageOffset = ((int)totalSeriesCount / pageSize) * pageSize;
                }
                var lastButOnePageOffset = lastPageOffset - pageSize;

                var seriesFromPageRequestFirstPage = await _buildQueryModule.GetSearchResultsSeriesFromPageRequest(dataset, displaySettingsAfterPageSizeChange, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, offSet: firstPageOffset);
                var seriesSourceId = seriesFromPageRequestFirstPage.Select(x => x.Id.SourceId).First();
                seriesFromPageRequestFirstPage.Count().Should().Be(pageSize);

                List<int> pagesOffsetsToGo = new List<int>() { secondPageOffSet, lastPageOffset, lastButOnePageOffset, firstPageOffset };
                foreach (var pageOffSet in pagesOffsetsToGo)
                {
                    seriesSourceId = await CheckSeriesChangedOnPageSingleSeries(seriesSourceId, dataset, displaySettingsAfterPageSizeChange, taxonomyFilters, taxonomyFiltersWithItems, pageOffSet);
                }
            }
            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Sector.None)]
        [TestCase(Sector.CountryRisk)]
        public async Task PagingTest(Sector sector)
        {
            await _buildQueryModule.ResetSettingsInGrid(sector);
            TaxonomyFilters taxonomyFilters = null;
            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(sector);

            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;
            var keywordToShortenSeriesNumber = "interstate";
            string frequency = "Annual";

            var seriesToCount = await _buildQueryModule.GetSeriesHighLevelAttributesFromGrid(sector, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, offSet: 0, keyword: keywordToShortenSeriesNumber);
            var totalSeriesCount = seriesToCount.TotalCount;

            List<int> pageSizes = new List<int>() { 5, 10, 20, 25, 50, 75, 100 };
            foreach (var pageSize in pageSizes)
            {
                var displaySettingsAfterPageSizeChange = await CommonTestsParts.SetPageSizeChange(_datasetSettingsModule, sector, pageSize);

                var firstPageOffset = 0;
                var secondPageOffSet = pageSize;
                int lastPageOffset;
                if ((int)totalSeriesCount % pageSize == 0)
                {
                    lastPageOffset = ((int)totalSeriesCount / pageSize - 1) * pageSize;
                }
                else
                {
                    lastPageOffset = ((int)totalSeriesCount / pageSize) * pageSize;
                }
                var lastButOnePageOffset = lastPageOffset - pageSize;

                var seriesFromPageRequestFirstPage = await _buildQueryModule.GetSearchResultsSeriesFromPageRequest(sector, displaySettingsAfterPageSizeChange, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, offSet: firstPageOffset, keyword: keywordToShortenSeriesNumber);
                var seriesSourceId = seriesFromPageRequestFirstPage.Select(x => x.Id.SourceId).First();
                seriesFromPageRequestFirstPage.Count().Should().Be(pageSize);

                List<int> pagesOffsetsToGo = new List<int>() { secondPageOffSet, lastPageOffset, lastButOnePageOffset, firstPageOffset };
                foreach (var pageOffSet in pagesOffsetsToGo)
                {
                    seriesSourceId = await CheckSeriesChangedOnPageSingleSeries(seriesSourceId, sector, displaySettingsAfterPageSizeChange, taxonomyFilters, taxonomyFiltersWithItems, pageOffSet, keywordToShortenSeriesNumber);
                }
            }
            await _buildQueryModule.ResetSettingsInGrid(sector);
        }

        [TestCase(Dataset.Banking)]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task GridContainsTaxomonyFiltersItemsOnly(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var pageSize = 100;
            var displaySettings = await CommonTestsParts.SetPageSizeChange(_datasetSettingsModule, dataset, pageSize);

            var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var taxonomyFiltersWithItems = TaxonomyFiltersData.GetTaxomonyFiltersItems(dataset);

            var seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);
            var seriesFromPageRequest = await _buildQueryModule.GetSearchResultsSeriesFromPageRequest(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);

            var allColumnsDataExpected = TaxonomyFiltersData.GetTaxomonyFiltersItems(dataset);

            CheckColumnsValuesMatchTaxonomyFilters(displaySettings, allColumnsDataExpected, seriesFromGridRequest);
            CheckColumnsValuesMatchTaxonomyFilters(displaySettings, allColumnsDataExpected, seriesFromPageRequest);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.GlobalEconomyHistorical)]
        public async Task DefaultDateRangeIsAppliedTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var userDateRange = UserDateRange.DefaultDateRange;
            var expectedDateRange = DateRangesTestData.GetDateRange(userDateRange);

            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
            displaySettings.ChangeUserSettings()
                .ChangeDateRange(expectedDateRange);

            var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var displaySettingsAfterChangingDateRange = await _datasetSettingsModule.GetDataSetSettings(dataset);
            displaySettingsAfterChangingDateRange.UserSettings.DateRange.Should().BeEquivalentTo(expectedDateRange);

            TaxonomyFilters taxonomyFilters = null;
            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            List<Frequency> frequencyList = new List<Frequency>() { Frequency.Annual, Frequency.Semiannual, Frequency.Quarterly, Frequency.Monthly };

            //List<Frequency> frequencyList = new List<Frequency>() { Frequency.Annual, Frequency.Semiannual, Frequency.Quarterly, Frequency.Monthly
            //                                ,Frequency.Semimonthly, Frequency.WeeklyMonday, Frequency.WeeklyTuesday, Frequency.WeeklyWednesday, Frequency.WeeklyThursday
            //                                ,Frequency.WeeklyFriday, Frequency.WeeklySaturday, Frequency.WeeklySunday, Frequency.Weekday, Frequency.Daily};

            foreach (var frequency in frequencyList)
            {
                var result = await _buildQueryModule.GetSeriesHighLevelAttributesFromGrid(dataset, displaySettingsAfterChangingDateRange, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency.ToString());
                var isObservationsTruncated = result.TruncationInfo.AreObservationsTruncated;
                var seriesFromGridRequest = result.Series.ToList();

                await CheckGridHasCorrectDateRange(expectedDateRange, displaySettingsAfterChangingDateRange, frequency, seriesFromGridRequest, dataset, userDateRange, isObservationsTruncated);
            }

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Sector.None)] //allSector
        [TestCase(Sector.Economics)]
        public async Task DefaultDateRangeIsAppliedTest(Sector sector)
        {
            await _buildQueryModule.ResetSettingsInGrid(sector);

            var userDateRange = UserDateRange.DefaultDateRange;
            var expectedDateRange = DateRangesTestData.GetDateRange(userDateRange);

            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(sector);
            displaySettings.ChangeUserSettings()
                .ChangeDateRange(expectedDateRange);

            var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var displaySettingsAfterChangingDateRange = await _datasetSettingsModule.GetDataSetSettings(sector);
            displaySettingsAfterChangingDateRange.UserSettings.DateRange.Should().BeEquivalentTo(expectedDateRange);

            TaxonomyFilters taxonomyFilters = null;
            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            List<Frequency> frequencyList = new List<Frequency>() { Frequency.Annual, Frequency.Semiannual, Frequency.Quarterly, Frequency.Monthly };

            //List<Frequency> frequencyList = new List<Frequency>() { Frequency.Annual, Frequency.Semiannual, Frequency.Quarterly, Frequency.Monthly
            //                                ,Frequency.Semimonthly, Frequency.WeeklyMonday, Frequency.WeeklyTuesday, Frequency.WeeklyWednesday, Frequency.WeeklyThursday
            //                                ,Frequency.WeeklyFriday, Frequency.WeeklySaturday, Frequency.WeeklySunday, Frequency.Weekday, Frequency.Daily};

            foreach (var frequency in frequencyList)
            {
                var result = await _buildQueryModule.GetSeriesHighLevelAttributesFromGrid(sector, displaySettingsAfterChangingDateRange, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency.ToString());
                var isObservationsTruncated = result.TruncationInfo.AreObservationsTruncated;
                var seriesFromGridRequest = result.Series.ToList();

                await CheckGridHasCorrectDateRange(expectedDateRange, displaySettingsAfterChangingDateRange, frequency, seriesFromGridRequest, sector, userDateRange, isObservationsTruncated);
            }

            await _buildQueryModule.ResetSettingsInGrid(sector);
        }

        [TestCase(Dataset.Banking)]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task DateRangeTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            List<UserDateRange> userDateRanges = new List<UserDateRange>()
            { UserDateRange.FirstAndLastAvailableValue, UserDateRange.CustomObservationPeriodAndLastAvailable, UserDateRange.CustomYearPeriodAndLastAvailable
            ,UserDateRange.CustomDateForStartAndEnd, UserDateRange.CustomObservationPeriodBeforeLastDateAndLastAvailable
            ,UserDateRange.CustomYearPeriodAndCustomObservationPeriod, UserDateRange.CustomQuarterPeriodAndCustomMonthPeriod, UserDateRange.DefaultDateRange};

            List<UserDateRange> dateRangesTakenFromCubeDescription = new List<UserDateRange>()
            { UserDateRange.FirstAndLastAvailableValue, UserDateRange.CustomObservationPeriodAndLastAvailable, UserDateRange.CustomYearPeriodAndLastAvailable
            ,UserDateRange.CustomObservationPeriodBeforeLastDateAndLastAvailable};

            foreach (var userDateRange in userDateRanges)
            {
                var expectedDateRange = DateRangesTestData.GetDateRange(userDateRange);

                var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
                displaySettings.ChangeUserSettings()
                    .ChangeDateRange(expectedDateRange);

                var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
                var expectedResponse = new UpdateFiltersResponse { Success = true };
                updateResult.Should().BeEquivalentTo(expectedResponse);

                var displaySettingsAfterChangingDateRange = await _datasetSettingsModule.GetDataSetSettings(dataset);
                displaySettingsAfterChangingDateRange.UserSettings.DateRange.Should().BeEquivalentTo(expectedDateRange);

                TaxonomyFilters taxonomyFilters;
                Dictionary<string, List<string>> taxonomyFiltersWithItems;

                List<Frequency> frequencyList;
                if (dataset == Dataset.Banking)
                {
                    frequencyList = new List<Frequency>() { Frequency.Annual, Frequency.Quarterly, Frequency.Monthly };

                    taxonomyFilters = null;
                    taxonomyFiltersWithItems = null;
                }
                else
                {
                    frequencyList = new List<Frequency>() { Frequency.Annual };

                    taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
                    taxonomyFiltersWithItems = TaxonomyFiltersData.GetTaxomonyFiltersItems(dataset);
                }

                foreach (var frequency in frequencyList)
                {
                    var result = await _buildQueryModule.GetSeriesHighLevelAttributesFromGrid(dataset, displaySettingsAfterChangingDateRange, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency.ToString());
                    var isObservationsTruncated = result.TruncationInfo.AreObservationsTruncated;
                    var seriesFromGridRequest = result.Series.ToList();

                    var datasetType = Datasets.GetDatasetType(dataset);
                    if ((datasetType == DatasetType.Cube) && (dateRangesTakenFromCubeDescription.Contains(userDateRange)))
                    {
                        var firstSeries = seriesFromGridRequest.First();
                        var seriesDatesList = firstSeries.Values.Select(x => x.Date).ToList();
                        seriesDatesList.Count().Should().BePositive("First-Last observations mode should return some dates.");
                    }
                    else
                    {
                        await CheckGridHasCorrectDateRange(expectedDateRange, displaySettingsAfterChangingDateRange, frequency, seriesFromGridRequest, dataset, userDateRange, isObservationsTruncated);
                    }
                }
            }

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Sector.None)] //allSector
        [TestCase(Sector.Economics)]
        public async Task DateRangeTest(Sector sector)
        {
            await _buildQueryModule.ResetSettingsInGrid(sector);

            List<UserDateRange> userDateRanges = new List<UserDateRange>()
            { UserDateRange.FirstAndLastAvailableValue, UserDateRange.CustomObservationPeriodAndLastAvailable, UserDateRange.CustomYearPeriodAndLastAvailable
            ,UserDateRange.CustomDateForStartAndEnd, UserDateRange.CustomObservationPeriodBeforeLastDateAndLastAvailable
            ,UserDateRange.CustomYearPeriodAndCustomObservationPeriod, UserDateRange.CustomQuarterPeriodAndCustomMonthPeriod, UserDateRange.DefaultDateRange};

            List<Frequency> frequencyList = new List<Frequency>() { Frequency.Annual, Frequency.Quarterly, Frequency.Monthly };

            foreach (var userDateRange in userDateRanges)
            {
                var expectedDateRange = DateRangesTestData.GetDateRange(userDateRange);

                var displaySettings = await _datasetSettingsModule.GetDataSetSettings(sector);
                displaySettings.ChangeUserSettings()
                    .ChangeDateRange(expectedDateRange);

                var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
                var expectedResponse = new UpdateFiltersResponse { Success = true };
                updateResult.Should().BeEquivalentTo(expectedResponse);

                var displaySettingsAfterChangingDateRange = await _datasetSettingsModule.GetDataSetSettings(sector);
                displaySettingsAfterChangingDateRange.UserSettings.DateRange.Should().BeEquivalentTo(expectedDateRange);

                TaxonomyFilters taxonomyFilters = null;
                Dictionary<string, List<string>> taxonomyFiltersWithItems = null;
                var keywordToShortenSeriesNumber = "user";

                foreach (var frequency in frequencyList)
                {
                    var result = await _buildQueryModule.GetSeriesHighLevelAttributesFromGrid(sector, displaySettingsAfterChangingDateRange, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency.ToString(), keyword: keywordToShortenSeriesNumber);
                    var isObservationsTruncated = result.TruncationInfo.AreObservationsTruncated;
                    var seriesFromGridRequest = result.Series.ToList();

                    await CheckGridHasCorrectDateRange(expectedDateRange, displaySettingsAfterChangingDateRange, frequency, seriesFromGridRequest, sector, userDateRange, isObservationsTruncated, keywordToShortenSeriesNumber);
                }
            }

            await _buildQueryModule.ResetSettingsInGrid(sector);
        }

        [TestCase(Dataset.AssetCapacityByCompany)]
        public async Task HideEmptySeriesTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var expectedDateRange = DateRangesTestData.GetDateRange(UserDateRange.DefaultDateRange);
            displaySettings.ChangeUserSettings().ChangeDateRange(expectedDateRange);
            displaySettings.ChangeUserSettings().ChangePageSize(100);

            var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            TaxonomyFilters taxonomyFilters = null;
            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            var seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);
            var seriesFromPageRequest = await _buildQueryModule.GetSearchResultsSeriesFromPageRequest(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);

            _buildQueryModule.IsAnySeriesHasNullValuesOnly(seriesFromGridRequest).Should().BeTrue("there should series with empty values present if HideEmptySeries = false");
            _buildQueryModule.IsAnySeriesHasNullValuesOnly(seriesFromPageRequest).Should().BeTrue("there should series with empty values present if HideEmptySeries = false");



            expectedDateRange.HideEmptySeries = true;
            displaySettings.ChangeUserSettings().ChangeDateRange(expectedDateRange);

            updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);
            seriesFromPageRequest = await _buildQueryModule.GetSearchResultsSeriesFromPageRequest(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);

            _buildQueryModule.IsAnySeriesHasNullValuesOnly(seriesFromGridRequest).Should().BeFalse("there should Not present series with empty values if HideEmptySeries = true");
            _buildQueryModule.IsAnySeriesHasNullValuesOnly(seriesFromPageRequest).Should().BeFalse("there should Not present series with empty values if HideEmptySeries = true");
        }

        [TestCase(Dataset.Banking)]
        public async Task SearchMnemonicByKeywordTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            TaxonomyFilters taxonomyFilters = null;
            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
            var seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);

            var newMnemonicsColumnName = "New Mnemonic";
            var columnNewMnemonicKey = _buildQueryModule.GetColumnKeyByName(displaySettings, newMnemonicsColumnName);
            var newMnemonicsColumnValue = _buildQueryModule.GetGridValuesForParticularColumn(seriesFromGridRequest, columnNewMnemonicKey).First();

            seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, keyword: newMnemonicsColumnValue);
            var newMnemonicsColumnValueResult = _buildQueryModule.GetGridValuesForParticularColumn(seriesFromGridRequest, columnNewMnemonicKey).First();

            newMnemonicsColumnValueResult.Should().Be(newMnemonicsColumnValue, "mnemonics search by keyword should work.");

            var mnemonicNoResult = "123";
            seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, keyword: mnemonicNoResult);
            seriesFromGridRequest.Count().Should().Be(0, $"incorrect mnemonic {mnemonicNoResult} should not be found.");
        }

        [TestCase(Dataset.Banking)]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.AssetCapacityByCompany)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task ViewResultsButtonReturnsTimeseriesTest(Dataset dataset)
        {
            TaxonomyFilters taxonomyFilters = null;
            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
            var seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);

            seriesFromGridRequest.Count().Should().BeGreaterThan(0, $"dataset {dataset} should have series.");
        }

        [TestCase(Sector.None)] //allSector
        [TestCase(Sector.Economics)]
        public async Task ViewResultsButtonReturnsTimeseriesTest(Sector sector)
        {
            TaxonomyFilters taxonomyFilters = null;
            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(sector);
            var seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(sector, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);

            seriesFromGridRequest.Count().Should().BeGreaterThan(0, $"sector {sector} should have series.");
        }

        //[TestCase(Dataset.ChemicalSupplyAndDemand2)]
        //public async Task WaspSpecialDimensionsHaveData(Dataset dataset)
        //{
        //    var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
        //    var taxonomyFiltersWithItems = _buildQueryModule.WaspDatasetSetMinimalQuery(taxonomyFilters);

        //    var versionTaxonomyFilterName = "Version";
        //    var conceptDetailsColumnName = "Concept Details";

        //    var versionTaxonomyFilter = taxonomyFilters.Filters.Single(x => x.Name == versionTaxonomyFilterName);
        //    var versionTaxonomyFilterItems = versionTaxonomyFilter.Items;
        //    versionTaxonomyFilterItems.Count().Should().BeGreaterThan(0, $"drawer {versionTaxonomyFilterName} in dataset {dataset} should have data.");

        //    var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
        //    var seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);
        //    var conceptDetailsColumnKey = _buildQueryModule.GetColumnKeyByName(displaySettings, conceptDetailsColumnName);
        //    var columnValues = _buildQueryModule.GetGridValuesForParticularColumn(seriesFromGridRequest, conceptDetailsColumnKey);
        //    IsColumnHasData(columnValues).Should().BeTrue($"{conceptDetailsColumnName} column in dataset {dataset} should have data.");
        //}

        [TestCase(Dataset.AssetCapacityByCompany)]
        public async Task CAPSGrandTotalOnlyTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var expectedDateRange = DateRangesTestData.GetDateRange(UserDateRange.DefaultDateRange);
            expectedDateRange.HideEmptySeries = true;
            displaySettings.UserSettings.IsSubtotalsEnabled = true;
            displaySettings.UserSettings.Grouping = null;
            displaySettings.ChangeUserSettings().ChangeDateRange(expectedDateRange);
            displaySettings.ChangeUserSettings().ChangePageSize(100);

            var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var taxonomyFiltersWithItems = TaxonomyFiltersData.GetTaxomonyFiltersItems(dataset);

            var resultFromGridRequest = await _buildQueryModule.GetSeriesHighLevelAttributesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);
            var totalsFromGridRequest = new Dictionary<DateTime, string>(resultFromGridRequest.Totals);
            var seriesFromGridRequest = resultFromGridRequest.Series.ToList();
            var resultFromPageRequest = await _buildQueryModule.GetSeriesFromPageRequest(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);
            var totalsFromPageRequest = new Dictionary<DateTime, string>(resultFromPageRequest.Totals);
            var seriesFromPageRequest = resultFromPageRequest.Series.ToList();

            CheckSubTotalsCalculation(seriesFromGridRequest, totalsFromGridRequest);
            CheckSubTotalsCalculation(seriesFromPageRequest, totalsFromPageRequest);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.AssetCapacityByCompany)]
        [TestCase(Dataset.AssetCapacityByShareholder)]
        public async Task CAPSSubTotalsAllCalulationTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var expectedDateRange = DateRangesTestData.GetDateRange(UserDateRange.DefaultDateRange);
            expectedDateRange.HideEmptySeries = true;
            displaySettings.UserSettings.IsSubtotalsEnabled = true;
            displaySettings.ChangeUserSettings().ChangeDateRange(expectedDateRange);
            displaySettings.ChangeUserSettings().ChangePageSize(100);

            var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
            var groupings = displaySettings.UserSettings.Grouping.ToList();
            var groupedColumnKeys = groupings.Select(x => x.Field).ToList();

            var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var taxonomyFiltersWithItems = TaxonomyFiltersData.GetTaxomonyFiltersItems(dataset);

            var resultFromGridRequest = await _buildQueryModule.GetSeriesHighLevelAttributesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);
            var totalsFromGridRequest = new Dictionary<DateTime, string>(resultFromGridRequest.Totals);
            var seriesFromGridRequest = resultFromGridRequest.Series.ToList();
            var summariesFromGridRequest = resultFromGridRequest.Summaries;

            SplitSeriesForSubtotalsCheck(groupedColumnKeys, seriesFromGridRequest, summariesFromGridRequest);
            CheckSubTotalsCalculation(seriesFromGridRequest, totalsFromGridRequest);

            var resultFromPageRequest = await _buildQueryModule.GetSeriesFromPageRequest(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);
            var totalsFromPageRequest = new Dictionary<DateTime, string>(resultFromPageRequest.Totals);
            var seriesFromPageRequest = resultFromPageRequest.Series.ToList();
            var summariesFromPageRequest = resultFromPageRequest.Summaries;

            SplitSeriesForSubtotalsCheck(groupedColumnKeys, seriesFromPageRequest, summariesFromPageRequest);
            CheckSubTotalsCalculation(seriesFromPageRequest, totalsFromPageRequest);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        private void SplitSeriesForSubtotalsCheck(List<string> groupedColumnKeys, List<Contract.SearchResults.Series> series
            , Dictionary<string, Dictionary<DateTime, Dictionary<string, Dictionary<string, object>>>> summaries)
        {
            for (int i = 0; i < groupedColumnKeys.Count(); i++)
            {
                var subtotalsNumberWithValues = GetSubtotalsValues(summaries, groupedColumnKeys[i]);
                int startSeriesListIndex = 0;
                foreach (var subtotalNumber in subtotalsNumberWithValues)
                {
                    List<Contract.SearchResults.Series> seriesListToCheck = new List<Contract.SearchResults.Series>();
                    for (int k = startSeriesListIndex; k <= subtotalNumber.Key; k++)
                    {
                        seriesListToCheck.Add(series[k]);
                    }
                    startSeriesListIndex = subtotalNumber.Key + 1;

                    CheckSubTotalsCalculation(seriesListToCheck, subtotalNumber.Value, groupedColumnKeys[i], startSeriesListIndex - 1);
                }
            }
        }

        private Dictionary<int, Dictionary<DateTime, string>> GetSubtotalsValues(Dictionary<string, Dictionary<DateTime, Dictionary<string, Dictionary<string, object>>>> summaries, string columnKey)
        {
            Dictionary<int, Dictionary<DateTime, string>> subtotalsNumberWithValues = new Dictionary<int, Dictionary<DateTime, string>>();
            for (int i = 0; i < summaries.Count(); i++)
            {
                Dictionary<DateTime, string> dateTimeAndValues = new Dictionary<DateTime, string>();

                var dateTimeValues = summaries.ElementAt(i).Value;
                foreach (var dateTimeValue in dateTimeValues)
                {
                    var sums = dateTimeValue.Value;
                    foreach (var sum in sums)
                    {
                        var columnSums = sum.Value;
                        foreach (var column in columnSums)
                        {
                            if (column.Key == columnKey)
                            {
                                dateTimeAndValues.Add(dateTimeValue.Key, column.Value.ToString());
                            }
                        }
                    }
                }

                if (dateTimeAndValues.Count() > 0)
                {
                    subtotalsNumberWithValues.Add(int.Parse(summaries.ElementAt(i).Key), dateTimeAndValues);
                }
            }

            return subtotalsNumberWithValues;
        }

        private void CheckSubTotalsCalculation(List<Contract.SearchResults.Series> seriesList, Dictionary<DateTime, string> totals, string columnKey = "Grand Total", int seriesListIndex = 0)
        {
            Dictionary<DateTime, double> seriesValuesSumPerDate = new Dictionary<DateTime, double>();
            var valuesForInitialization = seriesList.First().Values.ToList();
            for (int k = 0; k < seriesList.First().Values.Count(); k++)
            {
                seriesValuesSumPerDate.Add(valuesForInitialization[k].Date, 0);
            }

            foreach (var series in seriesList)
            {
                var sereiesValuesList = series.Values.ToList();
                for (int i = 0; i < sereiesValuesList.Count(); i++)
                {
                    var valueToAdd = sereiesValuesList[i].Value ?? 0;
                    var oldValueToSum = seriesValuesSumPerDate.Single(x => x.Key == sereiesValuesList[i].Date).Value;
                    var newValue = oldValueToSum + valueToAdd;
                    seriesValuesSumPerDate.Remove(sereiesValuesList[i].Date);
                    seriesValuesSumPerDate.Add(sereiesValuesList[i].Date, newValue);
                }
            }

            Dictionary<DateTime, string> seriesValuesSumToCompare = new Dictionary<DateTime, string>();
            foreach (var dict in seriesValuesSumPerDate)
            {
                seriesValuesSumToCompare.Add(dict.Key, dict.Value.ToString("N2", CultureInfo.InvariantCulture));
            }

            var differencies = seriesValuesSumToCompare.Except(totals);
            differencies.Count().Should().Be(0, $"subtotals should be calculated correctly so there should not be any differencies. ColumnKey with difference = {columnKey}," +
                $" summary Dictionary.key index (series number index) with error = {seriesListIndex}");

        }

        private bool IsColumnHasData(List<string> columnValues)
        {
            foreach (var columnValue in columnValues)
            {
                if (columnValue != null)
                {
                    return true;
                }
            }

            return false;
        }

        private async Task CheckGridHasCorrectDateRange(DateRangeOptions expectedDateRange, DataSetSettings dataSetSettings, Frequency frequency
            , List<Contract.SearchResults.Series> series, Dataset dataset, UserDateRange userDateRange, bool isObservationsTruncated)
        {
            expectedDateRange = await GetCustomDates(dataSetSettings, frequency, expectedDateRange, dataset);

            CheckGridHasCorrectDateRangeCalculation(expectedDateRange, dataSetSettings, frequency, series, userDateRange, isObservationsTruncated);
        }

        private async Task CheckGridHasCorrectDateRange(DateRangeOptions expectedDateRange, DataSetSettings dataSetSettings, Frequency frequency
            , List<Contract.SearchResults.Series> series, Sector sector, UserDateRange userDateRange, bool isObservationsTruncated, string keyword = "")
        {
            expectedDateRange = await GetCustomDates(dataSetSettings, expectedDateRange, sector, frequency, keyword);

            CheckGridHasCorrectDateRangeCalculation(expectedDateRange, dataSetSettings, frequency, series, userDateRange, isObservationsTruncated);
        }

        private void CheckGridHasCorrectDateRangeCalculation(DateRangeOptions expectedDateRange, DataSetSettings dataSetSettings, Frequency frequency
            , List<Contract.SearchResults.Series> series, UserDateRange userDateRange, bool isObservationsTruncated)
        {
            var firstSeries = series.First();
            var seriesDatesList = firstSeries.Values.Select(x => x.Date).ToList();

            expectedDateRange.CustomStartDate.Should().Be(seriesDatesList.First().Date, "Start Date should be correct for Frequency = {0}, userDateRange = {1}", frequency, userDateRange);

            if (isObservationsTruncated)
            {
                seriesDatesList.Count().Should().Be(120, "observations should be truncated to max 120 observations");
            }
            else
            {
                expectedDateRange.CustomEndDate.Should().Be(seriesDatesList.Last().Date, "End Date should be correct for Frequency = {0}, userDateRange = {1}", frequency, userDateRange);
            }
        }

        private async Task<DateRangeOptions> GetCustomDates(DataSetSettings dataSetSettings, Frequency frequency
            , DateRangeOptions expectedDateRange, Dataset dataset)
        {
            var firstLastAvailableDates = await GetFirstLastAvailableDates(dataset, frequency);
            var dateRangeResult = GetCustomDatesCalculation(dataSetSettings, frequency, expectedDateRange, firstLastAvailableDates);
            return dateRangeResult;
        }

        private async Task<DateRangeOptions> GetCustomDates(DataSetSettings dataSetSettings, DateRangeOptions expectedDateRange
            , Sector sector, Frequency frequency = Frequency.Annual, string keyword = "")
        {
            var firstLastAvailableDates = await GetFirstLastAvailableDates(sector, frequency, keyword);
            var dateRangeResult = GetCustomDatesCalculation(dataSetSettings, frequency, expectedDateRange, firstLastAvailableDates);
            return dateRangeResult;
        }

        private DateRangeOptions GetCustomDatesCalculation(DataSetSettings dataSetSettings, Frequency frequency
            , DateRangeOptions expectedDateRange, Values firstLastAvailableDates)
        {
            var firstAvailableDate = firstLastAvailableDates.MinDate;
            var lastAvailableDate = firstLastAvailableDates.MaxDate;

            var dateRange = dataSetSettings.UserSettings.DateRange;
            if (dateRange.IsDefaultDateRange)
            {
                var defaultDateRange = dataSetSettings.DefaultDateRanges[frequency.ToString()];

                if (defaultDateRange.BeginDateMode.ToString().Contains("FirstAvailable"))
                {
                    expectedDateRange.StartDateMode = CustomDateRangeMode.FirstAvailable;
                    expectedDateRange.CustomStartDate = firstAvailableDate.Date;
                }
                else
                {
                    expectedDateRange.StartDateMode = CustomDateRangeMode.CustomDate;
                    expectedDateRange.CustomStartDate = defaultDateRange.BeginDate.As<DateTime>().Date;
                }

                if (defaultDateRange.EndDateMode.ToString().Contains("LastAvailable"))
                {
                    expectedDateRange.EndDateMode = CustomDateRangeMode.LastAvailable;
                    expectedDateRange.CustomEndDate = lastAvailableDate.Date;
                }
                else
                {
                    expectedDateRange.EndDateMode = CustomDateRangeMode.CustomDate;
                    expectedDateRange.CustomEndDate = defaultDateRange.EndDate.As<DateTime>().Date;
                }

                return expectedDateRange;
            }

            if (dateRange.StartDateMode.ToString().Contains("CustomDate"))
            {
                expectedDateRange.CustomStartDate = expectedDateRange.CustomStartDate.As<DateTime>().Date;
            }

            if (dateRange.EndDateMode.ToString().Contains("CustomDate"))
            {
                expectedDateRange.CustomEndDate = expectedDateRange.CustomEndDate.As<DateTime>().Date;
            }

            if (dateRange.StartDateMode.ToString().Contains("FirstAvailable"))
            {
                expectedDateRange.CustomStartDate = firstAvailableDate.Date;
            }

            if (dateRange.EndDateMode.ToString().Contains("LastAvailable"))
            {
                expectedDateRange.CustomEndDate = lastAvailableDate.Date;
            }

            expectedDateRange = GetCustomPeriodDates(dateRange, expectedDateRange, frequency, firstLastAvailableDates);

            return expectedDateRange;
        }

        private DateRangeOptions GetCustomPeriodDates(DateRangeOptions dateRange, DateRangeOptions expectedDateRange, Frequency frequency, Values firstLastAvailableDates)
        {
            var firstAvailableDate = firstLastAvailableDates.MinDate;
            var lastAvailableDate = firstLastAvailableDates.MaxDate;

            var isPeriodTypeObservation = false;

            if (dateRange.StartDateMode.ToString().Contains("CustomPeriod"))
            {
                if (dateRange.CustomStartPeriodType.ToString().Contains("Observation"))
                {
                    dateRange = GetCorrectPeriodTypeForObservations(dateRange, frequency, true);
                    isPeriodTypeObservation = true;
                }

                var today = DateTime.Now.Date;
                if (!(bool)dateRange.IsCustomStartPeriodBeforeEndDate)
                {
                    if (dateRange.CustomStartPeriodType.ToString().Contains("Year"))
                    {
                        var resultDate = today.AddYears(-(int)expectedDateRange.CustomStartPeriodCount);
                        resultDate = DateTime.Parse(resultDate.Year.ToString() + "-" + GetCorrectMonth(frequency, resultDate) + "-01");
                        expectedDateRange.CustomStartDate = new DateTime(resultDate.Year, resultDate.Month, resultDate.Day, 10, 00, 00).Date;
                    }
                    if (dateRange.CustomStartPeriodType.ToString().Contains("Quarter"))
                    {
                        var resultDate = today.AddMonths(-(int)expectedDateRange.CustomStartPeriodCount * 3);
                        resultDate = DateTime.Parse(resultDate.Year.ToString() + "-" + GetCorrectMonth(frequency, resultDate) + "-01");
                        expectedDateRange.CustomStartDate = new DateTime(resultDate.Year, resultDate.Month, resultDate.Day, 10, 00, 00).Date;
                    }
                    if (dateRange.CustomStartPeriodType.ToString().Contains("Month"))
                    {
                        var resultDate = today.AddMonths(-(int)expectedDateRange.CustomStartPeriodCount);
                        resultDate = DateTime.Parse(resultDate.Year.ToString() + "-" + GetCorrectMonth(frequency, resultDate) + "-01");
                        expectedDateRange.CustomStartDate = new DateTime(resultDate.Year, resultDate.Month, resultDate.Day, 10, 00, 00).Date;
                    }
                }
                else
                {
                    if (dateRange.CustomStartPeriodType.ToString().Contains("Year"))
                    {
                        var resultDate = lastAvailableDate.AddYears(-(int)expectedDateRange.CustomStartPeriodCount);
                        resultDate = DateTime.Parse(resultDate.Year.ToString() + "-" + GetCorrectMonth(frequency, resultDate) + "-01");
                        expectedDateRange.CustomStartDate = new DateTime(resultDate.Year, resultDate.Month, resultDate.Day, 10, 00, 00).Date;
                    }
                    if (dateRange.CustomStartPeriodType.ToString().Contains("Quarter"))
                    {
                        var resultDate = lastAvailableDate.AddMonths(-(int)expectedDateRange.CustomStartPeriodCount * 3);
                        resultDate = DateTime.Parse(resultDate.Year.ToString() + "-" + GetCorrectMonth(frequency, resultDate) + "-01");
                        expectedDateRange.CustomStartDate = new DateTime(resultDate.Year, resultDate.Month, resultDate.Day, 10, 00, 00).Date;
                    }
                    if (dateRange.CustomStartPeriodType.ToString().Contains("Month"))
                    {
                        var resultDate = lastAvailableDate.AddMonths(-(int)expectedDateRange.CustomStartPeriodCount);
                        resultDate = DateTime.Parse(resultDate.Year.ToString() + "-" + GetCorrectMonth(frequency, resultDate) + "-01");
                        expectedDateRange.CustomStartDate = new DateTime(resultDate.Year, resultDate.Month, resultDate.Day, 10, 00, 00).Date;
                    }
                }

                if (isPeriodTypeObservation)
                {
                    dateRange.CustomStartPeriodType = CustomPeriodType.Observation;
                    isPeriodTypeObservation = false;
                }
            }

            if (dateRange.EndDateMode.ToString().Contains("CustomPeriod"))
            {
                if (dateRange.CustomEndPeriodType.ToString().Contains("Observation"))
                {
                    dateRange = GetCorrectPeriodTypeForObservations(dateRange, frequency, false);
                    isPeriodTypeObservation = true;
                }

                var today = DateTime.Now.Date;
                if (dateRange.CustomEndPeriodType.ToString().Contains("Year"))
                {
                    var resultDate = today.AddYears((int)expectedDateRange.CustomEndPeriodCount);
                    resultDate = DateTime.Parse(resultDate.Year.ToString() + "-" + GetCorrectMonth(frequency, resultDate) + "-01");
                    expectedDateRange.CustomEndDate = new DateTime(resultDate.Year, resultDate.Month, resultDate.Day, 10, 00, 00).Date;
                }
                if (dateRange.CustomEndPeriodType.ToString().Contains("Quarter"))
                {
                    var resultDate = today.AddMonths((int)expectedDateRange.CustomEndPeriodCount * 3);
                    resultDate = DateTime.Parse(resultDate.Year.ToString() + "-" + GetCorrectMonth(frequency, resultDate) + "-01");
                    expectedDateRange.CustomEndDate = new DateTime(resultDate.Year, resultDate.Month, resultDate.Day, 10, 00, 00).Date;
                }
                if (dateRange.CustomEndPeriodType.ToString().Contains("Month"))
                {
                    var resultDate = today.AddMonths((int)expectedDateRange.CustomEndPeriodCount);
                    resultDate = DateTime.Parse(resultDate.Year.ToString() + "-" + GetCorrectMonth(frequency, resultDate) + "-01");
                    expectedDateRange.CustomEndDate = new DateTime(resultDate.Year, resultDate.Month, resultDate.Day, 10, 00, 00).Date;
                }

                if (isPeriodTypeObservation)
                {
                    dateRange.CustomEndPeriodType = CustomPeriodType.Observation;
                    isPeriodTypeObservation = false;
                }
            }

            return expectedDateRange;
        }

        private DateRangeOptions GetCorrectPeriodTypeForObservations(DateRangeOptions dateRange, Frequency frequency, bool isStartPeriodType)
        {
            CustomPeriodType periodType;
            switch (frequency)
            {
                case Frequency.Annual:
                    {
                        periodType = CustomPeriodType.Year;
                        break;
                    }
                case Frequency.Quarterly:
                    {
                        periodType = CustomPeriodType.Quarter;
                        break;
                    }
                case Frequency.Monthly:
                    {
                        periodType = CustomPeriodType.Month;
                        break;
                    }
                default:
                    return dateRange;
            }

            if (isStartPeriodType)
            {
                dateRange.CustomStartPeriodType = periodType;
            }
            else
            {
                dateRange.CustomEndPeriodType = periodType;
            }
            return dateRange;
        }

        private string GetCorrectMonth(Frequency frequency, DateTime resultDate)
        {
            switch (frequency)
            {
                case Frequency.Annual:
                    return "01";
                case Frequency.Quarterly:
                    {
                        var quarter = (resultDate.Month + 2) / 3;
                        var month = 3 * quarter - 2;
                        if (month <= 9)
                        {
                            return "0" + month.ToString();
                        }
                        else
                        {
                            return month.ToString();
                        }
                    }
                case Frequency.Monthly:
                    {
                        if (resultDate.Month <= 9)
                        {
                            return "0" + resultDate.Month.ToString();
                        }
                        else
                        {
                            return resultDate.Month.ToString();
                        }
                    }
                default:
                    return "Incorrect frequency!";
            }
        }

        private async Task<Values> GetFirstLastAvailableDates(Dataset dataset, Frequency frequency)
        {
            FirstLastAvailableDates firstLastAvailableDatesList;
            var datasetType = Datasets.GetDatasetType(dataset);

            if (datasetType == DatasetType.Magellan)
            {
                firstLastAvailableDatesList = await ODataRequests.GetFirstLastAvailableDates(dataset, frequency);
            }
            else
            {
                if (dataset == Dataset.ComparativeIndustryRev4)
                {
                    firstLastAvailableDatesList = new FirstLastAvailableDates()
                    {
                        Value = new List<Values>()
                        {
                            new Values()
                            {
                                MinDate = Convert.ToDateTime("1980-01-01"),
                                MaxDate = Convert.ToDateTime("2039-01-01")
                            }
                        }
                    };
                }
                else //GtaForecasting dataset
                {
                    firstLastAvailableDatesList = new FirstLastAvailableDates()
                    {
                        Value = new List<Values>()
                        {
                            new Values()
                            {
                                MinDate = Convert.ToDateTime("2000-01-01"),
                                MaxDate = Convert.ToDateTime("2035-01-01")
                            }
                        }
                    };
                }
            }
            var firstLastAvailableDates = firstLastAvailableDatesList.Value.First();

            return firstLastAvailableDates;
        }

        private async Task<Values> GetFirstLastAvailableDates(Sector sector, Frequency frequency = Frequency.Annual, string keyword = "")
        {
            var firstLastAvailableDatesList = await ODataRequests.GetFirstLastAvailableDates(sector, frequency, keyword);
            var firstLastAvailableDates = firstLastAvailableDatesList.Value.First();

            return firstLastAvailableDates;
        }

        private void CheckColumnsValuesMatchTaxonomyFilters(DataSetSettings displaySettings, Dictionary<string, List<string>> allColumnsDataExpected, List<Contract.SearchResults.Series> series)
        {
            for (int i = 0; i < allColumnsDataExpected.Count(); i++)
            {
                var columnKey = _buildQueryModule.GetColumnKeyByName(displaySettings, allColumnsDataExpected.Keys.ElementAt(i));
                var dimensionForCheckName = allColumnsDataExpected.Keys.ElementAt(i);
                var columnValuesExpected = allColumnsDataExpected[dimensionForCheckName];

                if ((columnValuesExpected.Count() > 0) && (columnKey != "frequency"))
                {
                    var columnValues = _buildQueryModule.GetGridValuesForParticularColumn(series, columnKey).Distinct().ToList();

                    foreach (var columnValue in columnValues)
                    {
                        columnValuesExpected.Should().Contain(columnValue);
                    }
                }
            }
        }

        private async Task<string> CheckSeriesChangedOnPageSingleSeries
            (string seriesSourceIdInitial, Dataset dataset, DataSetSettings dataSetSettings, TaxonomyFilters taxonomyFilters
            , Dictionary<string, List<string>> taxonomyFiltersWithItems, int offset)
        {
            var seriesFromPageRequest = await _buildQueryModule.GetSearchResultsSeriesFromPageRequest(dataset, dataSetSettings, taxonomyFilters, taxonomyFiltersWithItems, offSet: offset);
            var newSeriesSourceId = seriesFromPageRequest.Select(x => x.Id.SourceId).First();

            seriesSourceIdInitial.Should().NotContain(newSeriesSourceId, "series in different pages should be different. Offset = {0}", offset);

            return newSeriesSourceId;
        }

        private async Task<string> CheckSeriesChangedOnPageSingleSeries
            (string seriesSourceId, Sector sector, DataSetSettings dataSetSettings, TaxonomyFilters taxonomyFilters
            , Dictionary<string, List<string>> taxonomyFiltersWithItems, int offset, string keyword = "")
        {
            var seriesFromPageRequest = await _buildQueryModule.GetSearchResultsSeriesFromPageRequest(sector, dataSetSettings, taxonomyFilters, taxonomyFiltersWithItems, offSet: offset, keyword: keyword);
            var newSeriesSourceId = seriesFromPageRequest.Select(x => x.Id.SourceId).First();

            seriesSourceId.Should().NotContain(newSeriesSourceId, "series in different pages should be different. Offset = {0}", offset);

            return newSeriesSourceId;
        }

        private async Task SeriesContainExpectedFrequency
            (Dataset dataset, DataSetSettings dataSetSettings, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems, string expectedFrequency)
        {
            var seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, dataSetSettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: expectedFrequency);
            var seriesFromPageRequest = await _buildQueryModule.GetSearchResultsSeriesFromPageRequest(dataset, dataSetSettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: expectedFrequency);

            SeriesContainExpectedFrequencyCheck(dataSetSettings, expectedFrequency, seriesFromGridRequest, seriesFromPageRequest);
        }

        private async Task SeriesContainExpectedFrequency
            (Sector sector, DataSetSettings dataSetSettings, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems, string expectedFrequency)
        {
            var keywordToShortenSeriesNumber = "infrastructure netherlands";
            var seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(sector, dataSetSettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: expectedFrequency, keyword: keywordToShortenSeriesNumber);
            var seriesFromPageRequest = await _buildQueryModule.GetSearchResultsSeriesFromPageRequest(sector, dataSetSettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: expectedFrequency, keyword: keywordToShortenSeriesNumber);

            SeriesContainExpectedFrequencyCheck(dataSetSettings, expectedFrequency, seriesFromGridRequest, seriesFromPageRequest);
        }

        private void SeriesContainExpectedFrequencyCheck
            (DataSetSettings dataSetSettings, string expectedFrequency, List<Contract.SearchResults.Series> seriesFromGridRequest, List<Contract.SearchResults.Series> seriesFromPageRequest)
        {
            var seriesAttributeFrequency = "frequency";
            var seriesFrequencyAttributeValuesGrid = _buildQueryModule.GetGridValuesForParticularColumn(seriesFromGridRequest, seriesAttributeFrequency);
            var seriesFrequencyAttributeValuesPage = _buildQueryModule.GetGridValuesForParticularColumn(seriesFromPageRequest, seriesAttributeFrequency);

            CheckColumnNotContainOddItems(seriesFrequencyAttributeValuesGrid, expectedFrequency).Should().BeTrue();
            CheckColumnNotContainOddItems(seriesFrequencyAttributeValuesPage, expectedFrequency).Should().BeTrue();
        }

        private bool CheckColumnNotContainOddItems(List<string> columnValues, string expectedValue)
        {
            foreach (var value in columnValues)
            {
                if (value != expectedValue)
                    return false;
            }
            return true;
        }

        public async Task<List<string>> GetExpectedColumnKeysHavingFilters(DatasetType datasetType, Dataset dataset)
        {
            if (datasetType != DatasetType.Cube)
            {
                var columnSettings = DataSetSettingsTestData.GetMagellanChemicalsColumnsWithFilters(datasetType, dataset);
                var columnKeys = columnSettings.Select(x => x.Key).ToList();
                return columnKeys;
            }
            else
            {
                var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
                var columnKeysExpected = taxonomyFilters.Filters.Select(x => x.Id).ToList();
                return columnKeysExpected;
            }
        }

        public async Task<List<string>> GetExpectedColumnsHavingFilters(DatasetType datasetType, Dataset dataset)
        {
            if (datasetType != DatasetType.Cube)
            {
                var columnSettings = DataSetSettingsTestData.GetMagellanChemicalsColumnsWithFilters(datasetType, dataset);
                var columnsNames = columnSettings.Select(x => x.HeaderText).ToList();
                return columnsNames;
            }
            else
            {
                var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
                var columnsNamesExpected = taxonomyFilters.Filters.Select(x => x.Name).ToList();
                return columnsNamesExpected;
            }
        }

        private async Task<SearchOptions> SetInitialParametersForComparativeIndustryTruncationTest(Dataset dataset)
        {
            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var taxonomyRequestList = new List<TaxonomyFilterValues>()
            {
                new TaxonomyFilterValues{FilterId = "[Concept].[Concept]", Values = new List<string>() },
                new TaxonomyFilterValues{FilterId = "[Geography].[Geography]", Values = new List<string>()},
                new TaxonomyFilterValues{FilterId = "[Industry].[Industry]", Values = new List<string>()}
            };

            var searchFilters = new SearchFiltersBuilder()
                .SetDataset(dataset)
                .SetFrequency()
                .SetFilters(taxonomyRequestList)
                .Build();

            displaySettings.ChangeUserSettings()
                .ChangeDateRange(DateRangesTestData.GetDateRange(UserDateRange.ForGridTest))
                .Build();


            var searchOption = displaySettings.ExtractSearchOptions(searchFilters);

            searchOption.ObservationsLimit = 1;
            searchOption.IsSubtotalsEnabled = false;

            return searchOption;
        }

        protected static Array GetAllDatasets()
        {
            return Enum.GetValues(typeof(Dataset))
                .Cast<Dataset>()
                .Except(new[] { Dataset.None })
                .ToArray();
        }
    }
}
