using Application.AutomatedBackendTests.Extensions;
using Application.AutomatedBackendTests.Models;
using Application.AutomatedBackendTests.Modules;
using Application.AutomatedBackendTests.Requests;
using Application.AutomatedBackendTests.TestData;
using Application.Contract;
using Application.Contract.DataSetsList;
using Application.Contract.Export;
using FluentAssertions;
using FluentAssertions.Execution;
using Frequencies;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Application.AutomatedBackendTests.TestData.DataSetSettingsTestData;

namespace Application.AutomatedBackendTests.Tests.Settings
{
    [TestFixture]
    [Category("ApplicationBackendTests")]
    public class DataSetSettingsTests
    {
        protected DatasetSettingsModule DatasetSettingsModule;
        private BuildQueryModule _buildQueryModule;
        protected User User { get; } = User.SettingsUser;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            DatasetSettingsModule = new DatasetSettingsModule(User);
            _buildQueryModule = new BuildQueryModule(User);
        }

        [Test]
        [TestCase(Sector.None)] //allSector
        [TestCase(Sector.CountryRisk)]
        [TestCase(Sector.Chemical)]
        public async Task SectorTypeSettingsTest(Sector sector)
        {
            var expectedColumns = DefaultColumns;
            var response = await DatasetSettingsModule.GetDataSetSettings(sector);
            var datasetValues = response.DataSet;
            var columnsInResponse = response.UserSettings.Columns.ToList();
            using (new AssertionScope())
            {
                columnsInResponse.Select(c => c.HeaderText).Should()
                    .BeEquivalentTo(expectedColumns.Select(c => c.HeaderText), "Correct header text in response");
                columnsInResponse.Select(c => c.Key).Should()
                    .BeEquivalentTo(expectedColumns.Select(c => c.Key), "Correct key in response");
                datasetValues.Id.Should().Be((int)sector);
            }
        }

        [TestCase(Dataset.GlobalEconomy)]
        [TestCase(Dataset.HealthcareForecast)]
        public async Task DatasetTypeSettingsTest(Dataset dataset)
        {
            var expectedColumns = GetColumnListForDataset(dataset);
            var response = await DatasetSettingsModule.GetDataSetSettings(dataset);
            var datasetValues = response.DataSet;
            var columnsInResponse = response.UserSettings.Columns.ToList();
            using (new AssertionScope())
            {
                columnsInResponse.Select(c => c.HeaderText).Should()
                    .BeEquivalentTo(expectedColumns.Select(c => c.HeaderText), "Correct header text in response");
                columnsInResponse.Select(c => c.Key).Should()
                    .BeEquivalentTo(expectedColumns.Select(c => c.Key), "Correct key in response");
                datasetValues.Id.Should().Be(dataset.GetDatasetInfo().GetId());
            }
        }

        [TestCase(Dataset.Banking)]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.AssetCapacityByCompany)]
        public async Task DataColumnWidthTest(Dataset dataset)
        {
            var expectedWidth = GetDefaultColumnWidthForDataset(dataset);
            var response = await DatasetSettingsModule.GetDataSetSettings(dataset);
            var datasetActualWidth = response.DataColumnWidth;

            datasetActualWidth.Should().Be(expectedWidth);
        }

        [TestCase(Sector.None)]
        [TestCase(Sector.CountryRisk)]
        public async Task DataColumnWidthTest(Sector sector)
        {
            var defaultSectorColumnWidth = 80;
            var response = await DatasetSettingsModule.GetDataSetSettings(sector);
            var datasetActualWidth = response.DataColumnWidth;

            datasetActualWidth.Should().Be(defaultSectorColumnWidth);
        }

        [Test]
        public async Task ZeroDecimalsTest()
        {
            var datasets = GetDatasetsWithZeroDecimals();
            using (new AssertionScope())
            {
                foreach (var dataset in datasets)
                {
                    var response = await DatasetSettingsModule.GetDataSetSettings(dataset);
                    response.UserSettings.NumberOfDecimals.Should().Be(0);
                }
            }
        }

        [Test]
        public async Task TwoDecimalsTest()
        {
            var datasets = GetDatasetsWithTwoDecimals();
            using (new AssertionScope())
            {
                foreach (var dataset in datasets)
                {
                    var response = await DatasetSettingsModule.GetDataSetSettings(dataset);
                    response.UserSettings.NumberOfDecimals.Should().Be(2);
                }
            }

            var sectors = new List<Sector>()
                {
                    Sector.Chemical,
                    Sector.CountryRisk,
                    Sector.Economics,
                    Sector.Energy,
                    Sector.LifeSciences,
                    Sector.MaritimeAndTrade,
                    Sector.None
                };
            using (new AssertionScope())
            {
                foreach (var sector in sectors)
                {
                    var response = await DatasetSettingsModule.GetDataSetSettings(sector);
                    response.UserSettings.NumberOfDecimals.Should().Be(2);
                }
            }
        }

        [Test]
        public async Task OneDecimalsTest()
        {
            var datasets = GetDatasetsWithOneDecimals();
            using (new AssertionScope())
            {
                foreach (var dataset in datasets)
                {
                    var response = await DatasetSettingsModule.GetDataSetSettings(dataset);
                    response.UserSettings.NumberOfDecimals.Should().Be(1);
                }
            }
        }

        [TestCase(Dataset.Banking)]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task ExportSettingsChangeTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var dataSetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);

            var isVerticalDatesNew = true;
            var leftPaddingNew = 5;
            var topPaddingNew = 7;
            var splitBySheetsNew = SplitBySheetsType.Groups;
            var exportDateModeNew = ExportDateMode.EndOfPeriod;
            var rowsBetweenFrequenciesNew = 4;
            var frequenciesOrderNew = FrequenciesOrder.HighToLow;

            dataSetSettings.UserSettings.ExportSettings = new ExportSettings()
            {
                IsVerticalDates = isVerticalDatesNew,
                LeftPadding = leftPaddingNew,
                TopPadding = topPaddingNew,
                SplitBySheets = splitBySheetsNew,
                ExportDateMode = exportDateModeNew,
                RowsBetweenFrequencies = rowsBetweenFrequenciesNew,
                FrequenciesOrder = frequenciesOrderNew
            };

            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(dataSetSettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            dataSetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);

            var exportSettings = dataSetSettings.UserSettings.ExportSettings;
            exportSettings.IsVerticalDates.Should().Be(isVerticalDatesNew);
            exportSettings.LeftPadding.Should().Be(leftPaddingNew);
            exportSettings.TopPadding.Should().Be(topPaddingNew);
            exportSettings.SplitBySheets.Should().Be(splitBySheetsNew);
            exportSettings.ExportDateMode.Should().Be(exportDateModeNew);
            exportSettings.RowsBetweenFrequencies.Should().Be(rowsBetweenFrequenciesNew);
            exportSettings.FrequenciesOrder.Should().Be(frequenciesOrderNew);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Sector.None)] //allSector
        [TestCase(Sector.CountryRisk)]
        public async Task ExportSettingsChangeTest(Sector sector)
        {
            await _buildQueryModule.ResetSettingsInGrid(sector);

            var dataSetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);

            var isVerticalDatesNew = true;
            var leftPaddingNew = 5;
            var topPaddingNew = 7;
            var splitBySheetsNew = SplitBySheetsType.Groups;
            var exportDateModeNew = ExportDateMode.EndOfPeriod;
            var rowsBetweenFrequenciesNew = 4;
            var frequenciesOrderNew = FrequenciesOrder.HighToLow;

            dataSetSettings.UserSettings.ExportSettings = new ExportSettings()
            {
                IsVerticalDates = isVerticalDatesNew,
                LeftPadding = leftPaddingNew,
                TopPadding = topPaddingNew,
                SplitBySheets = splitBySheetsNew,
                ExportDateMode = exportDateModeNew,
                RowsBetweenFrequencies = rowsBetweenFrequenciesNew,
                FrequenciesOrder = frequenciesOrderNew
            };

            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(dataSetSettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            dataSetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);

            var exportSettings = dataSetSettings.UserSettings.ExportSettings;
            exportSettings.IsVerticalDates.Should().Be(isVerticalDatesNew);
            exportSettings.LeftPadding.Should().Be(leftPaddingNew);
            exportSettings.TopPadding.Should().Be(topPaddingNew);
            exportSettings.SplitBySheets.Should().Be(splitBySheetsNew);
            exportSettings.ExportDateMode.Should().Be(exportDateModeNew);
            exportSettings.RowsBetweenFrequencies.Should().Be(rowsBetweenFrequenciesNew);
            exportSettings.FrequenciesOrder.Should().Be(frequenciesOrderNew);

            await _buildQueryModule.ResetSettingsInGrid(sector);
        }

        [TestCase(Dataset.Banking)]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task DecimalsNumberChangeTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var dataSetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);

            var numberOfDecimalsNew = 4;

            dataSetSettings.UserSettings.NumberOfDecimals = numberOfDecimalsNew;

            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(dataSetSettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            dataSetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);

            dataSetSettings.UserSettings.NumberOfDecimals.Should().Be(numberOfDecimalsNew);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Sector.None)] //allSector
        [TestCase(Sector.CountryRisk)]
        public async Task DecimalsNumberChangeTest(Sector sector)
        {
            await _buildQueryModule.ResetSettingsInGrid(sector);

            var dataSetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);

            var numberOfDecimalsNew = 4;

            dataSetSettings.UserSettings.NumberOfDecimals = numberOfDecimalsNew;

            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(dataSetSettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            dataSetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);

            dataSetSettings.UserSettings.NumberOfDecimals.Should().Be(numberOfDecimalsNew);

            await _buildQueryModule.ResetSettingsInGrid(sector);
        }

        [Test]
        [TestCaseSource("GetAllDatasets")]
        public async Task DatasetAvailableFunctionsTest(Dataset dataset)
        {
            var expectedFunctions = GetFunctionListForDataset(dataset);
            var response = await DatasetSettingsModule.GetDataSetSettings(dataset);

            var functionsInResponse = response.AvailableFunctions.Select(af => af.FunctionId).ToList();

            expectedFunctions.Sort();
            functionsInResponse.Sort();
            functionsInResponse.Should().Equal(expectedFunctions);

            if (IsRebasingAvailable(dataset))
            {
                var supportsRebasing = response.RebasingConfiguration.SupportsRebasing;
                supportsRebasing.Should().BeTrue();
            }
            else
            {
                var supportsRebasing = response.RebasingConfiguration?.SupportsRebasing ?? false;
                supportsRebasing.Should().BeFalse();
            }

            if (IsSubtotalsAvailableExpected(dataset))
            {
                var isSubtotalsSupported = response.IsSubtotalsAvailable;
                isSubtotalsSupported.Should().BeTrue();
            }
        }

        [TestCase(Sector.None)] //allSector
        [TestCase(Sector.CountryRisk)]
        public async Task AllSectorAvailableFunctionsTest(Sector sector)
        {
            var expectedFunctions = new List<int>();
            var response = await DatasetSettingsModule.GetDataSetSettings(sector);

            var functionsInResponse = response.AvailableFunctions.Select(af => af.FunctionId).ToList();

            expectedFunctions.Sort();
            functionsInResponse.Sort();
            functionsInResponse.Should().Equal(expectedFunctions);

            var supportsRebasing = response.RebasingConfiguration?.SupportsRebasing ?? false;
            supportsRebasing.Should().BeFalse();

            var isSubtotalsSupported = response.IsSubtotalsAvailable;
            isSubtotalsSupported.Should().BeFalse();
        }

        [TestCase(Dataset.AssetCapacityByCompany)]
        [TestCase(Dataset.AssetCapacityByShareholder)]
        [TestCase(Dataset.AggregatedAssetByGeography)]
        public async Task CapsAvailableUnitsTest(Dataset dataset)
        {
            var expectedUnits = GetUnits().ToList();

            var response = await DatasetSettingsModule.GetDataSetSettings(dataset);

            var unitsInResponse = response.GetAvailableUnits().ToList();

            using (new AssertionScope())
            {
                unitsInResponse.Count().Should().Be(expectedUnits.Count);
                unitsInResponse.All(uir => expectedUnits.Contains(uir));
            }
        }

        private IEnumerable<AllowedParameter> GetUnits()
        {
            var units = ODataRequests.GetAvailableCapsUnits();

            foreach (var unitCAPS in units.Result.Value.Select(x => x.Name))
            {
                yield return new AllowedParameter
                { DisplayValue = unitCAPS };
            }
        }

        [TestCase(Dataset.AssetCapacityByCompany)]
        [TestCase(Dataset.AssetCapacityByShareholder)]
        public async Task DefaultGroupingForLocalDatasets(Dataset dataset)
        {
            var expectedGrouping = GetExpectedGroupingSettings(dataset);
            var response = await DatasetSettingsModule.GetDataSetSettings(dataset);
            var grouping = response.UserSettings.Grouping;
            expectedGrouping.Should().BeEquivalentTo(grouping);
        }

        [TestCase(Dataset.Banking)]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task UpdatingVisibleTaxonomyFilterTest(Dataset dataset)
        {
            var datasetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
            var filters = datasetSettings.UserSettings.Filters.ToList();
            var visibleFilter = filters.FirstOrDefault(f => f.Visible);
            if (visibleFilter == null)
            {
                await SetAllTaxonomyFiltersVisible(filters, datasetSettings);
                datasetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
                filters = datasetSettings.UserSettings.Filters.ToList();
                visibleFilter = filters.FirstOrDefault(f => f.Visible);
            }
            await ChangeVisibilityOfTaxonomyFilterTest(dataset, visibleFilter, filters, datasetSettings);
        }

        [TestCase(Sector.CountryRisk)]
        public async Task UpdatingVisibleTaxonomyFilterTest(Sector sector)
        {
            var datasetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);
            var filters = datasetSettings.UserSettings.Filters.ToList();
            var visibleFilter = filters.FirstOrDefault(f => f.Visible);
            if (visibleFilter == null)
            {
                await SetAllTaxonomyFiltersVisible(filters, datasetSettings);
                datasetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);
                filters = datasetSettings.UserSettings.Filters.ToList();
                visibleFilter = filters.FirstOrDefault(f => f.Visible);
            }
            await ChangeVisibilityOfTaxonomyFilterTest(sector, visibleFilter, filters, datasetSettings);
        }

        [TestCase(Dataset.Banking)]
        [TestCase(Dataset.CoalHistoricalTrade)]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task ChangingColumnOrderInBuildQueryTest(Dataset dataset)
        {
            var datasetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
            var columns = datasetSettings.UserSettings.Columns.ToList();
            var secondColumn = columns.ElementAt(1);
            columns.RemoveAt(1);
            columns.Insert(0, secondColumn);
            datasetSettings.UserSettings.Columns = columns;

            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(datasetSettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var updatedDatasetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
            updatedDatasetSettings.UserSettings.Columns.Should().BeEquivalentTo(datasetSettings.UserSettings.Columns);
            updatedDatasetSettings.UserSettings.Columns.First().Should().BeEquivalentTo(secondColumn);
        }

        [TestCase(Sector.None)] //allSector
        [TestCase(Sector.CountryRisk)]
        public async Task ChangingColumnOrderInBuildQueryTest(Sector sector)
        {
            var datasetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);
            var columns = datasetSettings.UserSettings.Columns.ToList();
            var secondColumn = columns.ElementAt(1);
            columns.RemoveAt(1);
            columns.Insert(0, secondColumn);
            datasetSettings.UserSettings.Columns = columns;

            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(datasetSettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var updatedDatasetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);
            updatedDatasetSettings.UserSettings.Columns.Should().BeEquivalentTo(datasetSettings.UserSettings.Columns);
            updatedDatasetSettings.UserSettings.Columns.First().Should().BeEquivalentTo(secondColumn);
        }

        [TestCase(Dataset.Banking)]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task AddRemoveColumnBuildQueryTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var datasetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
            var columns = datasetSettings.UserSettings.Columns.ToList();
            var columnsDisplayed = columns.Where(x => x.Hidden == false);
            var columnsHidden = columns.Where(x => x.Hidden == true);
            columnsDisplayed.First().Hidden = true;
            columnsHidden.Last().Hidden = false;
            var newColumnsList = new List<ColumnSettings>();
            newColumnsList.AddRange(columnsDisplayed);
            newColumnsList.AddRange(columnsHidden);
            datasetSettings.UserSettings.Columns = newColumnsList;

            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(datasetSettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var updatedDatasetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
            updatedDatasetSettings.UserSettings.Columns.Should().BeEquivalentTo(datasetSettings.UserSettings.Columns);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Sector.None)] //allSector
        [TestCase(Sector.CountryRisk)]
        public async Task AddRemoveColumnBuildQueryTest(Sector sector)
        {
            await _buildQueryModule.ResetSettingsInGrid(sector);

            var datasetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);
            var columns = datasetSettings.UserSettings.Columns.ToList();
            var columnsDisplayed = columns.Where(x => x.Hidden == false);
            var columnsHidden = columns.Where(x => x.Hidden == true);
            columnsDisplayed.First().Hidden = true;
            columnsHidden.Last().Hidden = false;
            var newColumnsList = new List<ColumnSettings>();
            newColumnsList.AddRange(columnsDisplayed);
            newColumnsList.AddRange(columnsHidden);
            datasetSettings.UserSettings.Columns = newColumnsList;

            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(datasetSettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var updatedDatasetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);
            updatedDatasetSettings.UserSettings.Columns.Should().BeEquivalentTo(datasetSettings.UserSettings.Columns);

            await _buildQueryModule.ResetSettingsInGrid(sector);
        }

        [TestCase(Dataset.Banking)]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task ChangeColumnWidthBuildQueryTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var datasetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
            var columns = datasetSettings.UserSettings.Columns.ToList();
            var columnsDisplayed = columns.Where(x => x.Hidden == false);
            var initialColumnWidth = columnsDisplayed.First().Width;
            var columnWidthPixelsNumber = initialColumnWidth.Remove(initialColumnWidth.Length - 2);
            var newColumnWidthPixelsNumber = int.Parse(columnWidthPixelsNumber) + 10;
            var newColumnWidth = newColumnWidthPixelsNumber.ToString() + "px";
            columnsDisplayed.First().Width = newColumnWidth;

            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(datasetSettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var updatedDatasetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
            updatedDatasetSettings.UserSettings.Columns.Where(x => x.Hidden == false).First().Width.Should().BeEquivalentTo(newColumnWidth);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Sector.None)] //allSector
        [TestCase(Sector.CountryRisk)]
        public async Task ChangeColumnWidthBuildQueryTest(Sector sector)
        {
            await _buildQueryModule.ResetSettingsInGrid(sector);

            var datasetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);
            var columns = datasetSettings.UserSettings.Columns.ToList();
            var columnsDisplayed = columns.Where(x => x.Hidden == false);
            var initialColumnWidth = columnsDisplayed.First().Width;
            var columnWidthPixelsNumber = initialColumnWidth.Remove(initialColumnWidth.Length - 2);
            var newColumnWidthPixelsNumber = int.Parse(columnWidthPixelsNumber) + 10;
            var newColumnWidth = newColumnWidthPixelsNumber.ToString() + "px";
            columnsDisplayed.First().Width = newColumnWidth;

            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(datasetSettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var updatedDatasetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);
            updatedDatasetSettings.UserSettings.Columns.Where(x => x.Hidden == false).First().Width.Should().BeEquivalentTo(newColumnWidth);

            await _buildQueryModule.ResetSettingsInGrid(sector);
        }

        [TestCase(Dataset.Banking)]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task ColumnFreezingBuildQueryTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var datasetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
            var fixedColumnsList = new List<string>() { datasetSettings.UserSettings.Columns.ToList().First().Key + "_left", datasetSettings.UserSettings.Columns.ToList().Last().Key + "_right" };
            datasetSettings.UserSettings.FixedColumns = fixedColumnsList;

            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(datasetSettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var updatedDatasetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
            updatedDatasetSettings.UserSettings.FixedColumns.SequenceEqual(fixedColumnsList).Should().BeTrue();


            fixedColumnsList = new List<string>();
            updatedDatasetSettings.UserSettings.FixedColumns = fixedColumnsList;

            updateResult = await DatasetSettingsModule.UpdateDatasetSettings(updatedDatasetSettings);
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var newUpdatedDatasetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
            newUpdatedDatasetSettings.UserSettings.FixedColumns.Should().BeNullOrEmpty();

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Sector.None)] //allSector
        [TestCase(Sector.CountryRisk)]
        public async Task ColumnFreezingBuildQueryTest(Sector sector)
        {
            await _buildQueryModule.ResetSettingsInGrid(sector);

            var datasetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);
            var fixedColumnsList = new List<string>() { datasetSettings.UserSettings.Columns.ToList().First().Key + "_left", datasetSettings.UserSettings.Columns.ToList().Last().Key + "_right" };
            datasetSettings.UserSettings.FixedColumns = fixedColumnsList;

            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(datasetSettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var updatedDatasetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);
            updatedDatasetSettings.UserSettings.FixedColumns.SequenceEqual(fixedColumnsList).Should().BeTrue();


            fixedColumnsList = new List<string>();
            updatedDatasetSettings.UserSettings.FixedColumns = fixedColumnsList;

            updateResult = await DatasetSettingsModule.UpdateDatasetSettings(updatedDatasetSettings);
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var newUpdatedDatasetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);
            newUpdatedDatasetSettings.UserSettings.FixedColumns.Should().BeNullOrEmpty();

            await _buildQueryModule.ResetSettingsInGrid(sector);
        }

        [Test]
        [TestCaseSource("GetAllDatasets")]
        public async Task CheckNewMnemonicColumnPresence(Dataset dataset)
        {
            var datasetListWithNewMnemonicColumn = new List<Dataset>() { Dataset.Banking, Dataset.ConstructionCanada, Dataset.CanadianEconomy, Dataset.ChinaRegional
            ,Dataset.ChinaRegionalHistorical, Dataset.CountryRiskScores, Dataset.FinancialMarketsHistorical, Dataset.EuropeanCommissionForecast
            ,Dataset.GlobalAgriculture, Dataset.GlobalConsumerMarkets, Dataset.GlobalEconomy, Dataset.GlobalEconomyNew, Dataset.GlobalEconomyHistorical, Dataset.WorldMarketMonitor
            ,Dataset.IMFForecast, Dataset.IndustryHistorical, Dataset.IndustryForecastMonitor, Dataset.PMIMarkit, Dataset.OECDForecast, Dataset.PricingAndPurchasingForecasts
            ,Dataset.PricingAndPurchasingHistoricalPrices, Dataset.USAgriculture, Dataset.USEconomy, Dataset.USEconomyHistorical, Dataset.USIndustryRisk, Dataset.USRegional
            ,Dataset.USRegionalHousing, Dataset.USRegionalHistorical, Dataset.USAlternativeScenariosService, Dataset.USComparativeIndustry, Dataset.OilMarketsMidstreamAndDownstream};

            var datasetListWithOutNewMnemonicColumn = new List<Dataset>() { Dataset.ARARentalytics, Dataset.CoalHistoricalPricesAndStatistics, Dataset.CoalHistoricalTrade
            ,Dataset.EuropeanEnergyScenarios, Dataset.GasAndPower, Dataset.ConstructionGlobal, Dataset.HealthcareForecast, Dataset.PricingAndPurchasingPowerPlanner
            ,Dataset.RenewablePower, Dataset.USConsumerMarkets, Dataset.GtaForecasting, Dataset.PointlogicProduction, Dataset.CityRiskRatings};

            DataSetSettings datasetSettings;
            List<ColumnSettings> columns;
            var newMnemonicColumnKey = "default_mnemonic";

            if (datasetListWithNewMnemonicColumn.Contains(dataset))
            {
                datasetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
                columns = datasetSettings.UserSettings.Columns.ToList();
                var columnsKeys = columns.Select(x => x.Key).ToList();
                columnsKeys.Should().Contain(newMnemonicColumnKey);
            }

            if (datasetListWithOutNewMnemonicColumn.Contains(dataset))
            {
                datasetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);
                columns = datasetSettings.UserSettings.Columns.ToList();
                var columnsKeys = columns.Select(x => x.Key).ToList();
                columnsKeys.Should().NotContain(newMnemonicColumnKey);
            }
        }

        [TestCase(Sector.None)] //allSector
        [TestCase(Sector.Chemical)]
        [TestCase(Sector.CountryRisk)]
        [TestCase(Sector.Economics)]
        [TestCase(Sector.Energy)]
        [TestCase(Sector.LifeSciences)]
        [TestCase(Sector.MaritimeAndTrade)]
        public async Task CheckNewMnemonicColumnPresence(Sector sector)
        {
            var newMnemonicColumnKey = "default_mnemonic";

            var datasetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);
            var columns = datasetSettings.UserSettings.Columns.ToList();
            var columnsKeys = columns.Select(x => x.Key).ToList();
            columnsKeys.Should().NotContain(newMnemonicColumnKey);
        }

        [Test]
        public async Task CheckDatasetsInAlphaOrder()
        {
            var datasetsList = await DatasetSettingsModule.GetDatasetsList();
            datasetsList.Datasets[0].Id.Should().Be(0);
            datasetsList.Datasets[1].Type.Should().BeEquivalentTo(DataSetType.Sector);
            var sectorsList = datasetsList.Datasets.Where(x => x.Type == DataSetType.Sector).ToList();
            var sectorNames = sectorsList.Select(x => x.Name);

            var sorted = new List<string>();
            sorted.AddRange(sectorNames.OrderBy(o => o));
            sectorNames.SequenceEqual(sorted).Should().BeTrue();

            CheckDatasetsOrder(datasetsList.Datasets);
        }

        [TestCaseSource("GetAllDatasets")]
        public async Task CheckDefaultDateRangeLabels(Dataset dataset)
        {
            List<Frequency> frequencyList = new List<Frequency>() { Frequency.Annual, Frequency.Semiannual, Frequency.Quarterly, Frequency.Monthly
                                            ,Frequency.Semimonthly, Frequency.WeeklyMonday, Frequency.WeeklyTuesday, Frequency.WeeklyWednesday, Frequency.WeeklyThursday
                                            ,Frequency.WeeklyFriday, Frequency.WeeklySaturday, Frequency.WeeklySunday, Frequency.Weekday, Frequency.Daily};

            var dataSetSettings = await DatasetSettingsModule.GetDataSetSettings(dataset);

            foreach (var frequency in frequencyList)
            {
                var defaultDateRange = dataSetSettings.DefaultDateRanges[frequency.ToString()];
                var dateRangeLabel = defaultDateRange.Label;
                var expectedLabel = DatasetTestData.GetCustomDefaultDateRangeLabels(dataset, frequency);

                expectedLabel.Should().Be(dateRangeLabel, $"Date Range Label for dataset {dataset} frequency {frequency} should be correct.");
            }
        }

        [TestCase(Sector.None)] //allSector
        [TestCase(Sector.Chemical)]
        [TestCase(Sector.CountryRisk)]
        [TestCase(Sector.Economics)]
        [TestCase(Sector.Energy)]
        [TestCase(Sector.LifeSciences)]
        [TestCase(Sector.MaritimeAndTrade)]
        public async Task CheckDefaultDateRangeLabels(Sector sector)
        {
            List<Frequency> frequencyList = new List<Frequency>() { Frequency.Annual, Frequency.Semiannual, Frequency.Quarterly, Frequency.Monthly
                                            ,Frequency.Semimonthly, Frequency.WeeklyMonday, Frequency.WeeklyTuesday, Frequency.WeeklyWednesday, Frequency.WeeklyThursday
                                            ,Frequency.WeeklyFriday, Frequency.WeeklySaturday, Frequency.WeeklySunday, Frequency.Weekday, Frequency.Daily};

            var dataSetSettings = await DatasetSettingsModule.GetDataSetSettings(sector);

            foreach (var frequency in frequencyList)
            {
                var defaultDateRange = dataSetSettings.DefaultDateRanges[frequency.ToString()];
                var dateRangeLabel = defaultDateRange.Label;
                var expectedLabel = DatasetTestData.GetSectorCustomDefaultDateRangeLabels(frequency);

                expectedLabel.Should().Be(dateRangeLabel, $"Date Range Label for dataset {sector} frequency {frequency} should be correct.");
            }
        }

        private void CheckDatasetsOrder(List<DataSetDefinitionWithAliases> datasets)
        {
            List<int> sectorIndices = new List<int>();
            foreach (var dataset in datasets)
            {
                if (dataset.Type != DataSetType.DataSet)
                {
                    sectorIndices.Add(datasets.IndexOf(dataset));
                }
            }

            List<DataSetDefinitionWithAliases> datasetIDs = new List<DataSetDefinitionWithAliases>();
            for (int elementIndex = 2; elementIndex < datasets.Count(); elementIndex++)
            {
                if (!sectorIndices.Contains(elementIndex))
                {
                    datasetIDs.Add(datasets[elementIndex]);
                }
                else
                {
                    var datasetsForCheck = datasets.Where(x => datasetIDs.Contains(x));
                    var datasetNames = datasetsForCheck.Select(x => x.Name);
                    var sorted = new List<string>();
                    sorted.AddRange(datasetNames.OrderBy(o => o));
                    datasetNames.SequenceEqual(sorted).Should().BeTrue();
                    datasetIDs = new List<DataSetDefinitionWithAliases>();
                }
            }

        }

        private async Task ChangeVisibilityOfTaxonomyFilterTest(Dataset dataset, FilterState visibleFilter,
            IReadOnlyCollection<FilterState> filters, DataSetSettings datasetSettings)
        {
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            visibleFilter.Visible = !visibleFilter.Visible;
            datasetSettings.UserSettings.Filters = filters;
            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(datasetSettings);
            updateResult.Should().BeEquivalentTo(expectedResponse);
            var datasetSettingsAfterUpdate = await DatasetSettingsModule.GetDataSetSettings(dataset);
            datasetSettingsAfterUpdate.UserSettings.Filters.Should().BeEquivalentTo(filters);
        }

        private async Task ChangeVisibilityOfTaxonomyFilterTest(Sector sector, FilterState visibleFilter,
            IReadOnlyCollection<FilterState> filters, DataSetSettings datasetSettings)
        {
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            visibleFilter.Visible = !visibleFilter.Visible;
            datasetSettings.UserSettings.Filters = filters;
            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(datasetSettings);
            updateResult.Should().BeEquivalentTo(expectedResponse);
            var datasetSettingsAfterUpdate = await DatasetSettingsModule.GetDataSetSettings(sector);
            datasetSettingsAfterUpdate.UserSettings.Filters.Should().BeEquivalentTo(filters);
        }

        private async Task SetAllTaxonomyFiltersVisible(IReadOnlyCollection<FilterState> filters, DataSetSettings datasetSettings)
        {
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            foreach (var filter in filters)
            {
                filter.Visible = true;
            }
            datasetSettings.UserSettings.Filters = filters;
            var updateResult = await DatasetSettingsModule.UpdateDatasetSettings(datasetSettings);
            updateResult.Should().BeEquivalentTo(expectedResponse);
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
