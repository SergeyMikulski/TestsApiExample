using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Application.AutomatedBackendTests.Helpers;
using Application.AutomatedBackendTests.Modules;
using Application.AutomatedBackendTests.Requests;
using Application.AutomatedBackendTests.TestData;
using Application.AutomatedBackendTests.Tests.Workspace;
using Application.Contract;
using NUnit.Framework;
using System.Linq;
using FluentAssertions;
using Application.AutomatedBackendTests.Extensions;
using Application.AutomatedBackendTests.Tests.Workbook;
using Series = Application.AutomatedBackendTests.TestData.Series;

namespace Application.AutomatedBackendTests.Tests.ChinaUser
{
    [TestFixture]
    [Category("ApplicationBackendTests")]
    [Category("ApplicationChinaUserTests")]
    public class ChinaUserTests : WorkbookTestBase
    {
        private WorkbookFunctionSettings _workbookFunctionSettings;
        protected override User User { get; } = new User(123, "user", UserHelper.GetPassword("TestUserPassword"));
        private readonly Lazy<int> _folderId = new Lazy<int>(() => EnvHelper.GetFolderId(491583, 508421));

        protected override int FolderId => _folderId.Value;

        private DatasetSettingsModule _datasetSettingsModule;
        private BuildQueryModule _buildQueryModule;
        private MySavedItemCleaner _cleaner;
        private WorkspaceModule _workspaceModule;

        [OneTimeSetUp]
        public async Task GettingWorkbookFunctionSettings()
        {
            _datasetSettingsModule = new DatasetSettingsModule(User);
            _buildQueryModule = new BuildQueryModule(User);
            _cleaner = new MySavedItemCleaner(User);
            _workbookFunctionSettings = await WorkspaceModule.GetFunctionSettings().ConfigureAwait(false);
            _workspaceModule = new WorkspaceModule(User);
        }

        [TestCase(Dataset.CountryRiskScores)]
        public async Task ChinaUserTaxonomyFiltersDataDp(Dataset dataset)
        {
            var chinaGeosExpected = ChinaUserGeosExpected(dataset);
            var taxonomyGeoFilterId = "source_geographic_location";
            var geoChildToFindIn = "Asia-Pacific";
            var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var geoTaxonomyFilter = taxonomyFilters.Filters.Single(x => x.Id == taxonomyGeoFilterId);
            var geoChildTaxonomyFilter = geoTaxonomyFilter.Items.SingleOrDefault(x => x.Name == geoChildToFindIn);

            foreach (var geoExpected in chinaGeosExpected)
            {
                geoChildTaxonomyFilter.Children.SingleOrDefault(x => x.Name == geoExpected).Should().NotBeNull();
            }
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task ChinaUserTaxonomyFiltersDataCube(Dataset dataset)
        {
            var chinaGeosExpected = ChinaUserGeosExpected(dataset);
            var taxonomyGeoFilterId = "[Geography].[Geography]";
            var geoChildToFindIn = "Asia-Pacific (17)";
            var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var geoTaxonomyFilter = taxonomyFilters.Filters.Single(x => x.Id == taxonomyGeoFilterId).Items.Last();
            var geoChildTaxonomyFilter = geoTaxonomyFilter.Children.SingleOrDefault(x => x.Name == geoChildToFindIn);

            foreach (var geoExpected in chinaGeosExpected)
            {
                geoChildTaxonomyFilter.Children.SingleOrDefault(x => x.Name == geoExpected).Should().NotBeNull();
            }
        }

        [TestCase(Dataset.GtaForecasting)]
        public async Task ChinaUserTaxonomyFiltersDataGtaf(Dataset dataset)
        {
            var chinaGeosExpected = ChinaUserGeosExpected(dataset);
            var taxonomyGeoFilterIds = new string[] { "[Export Country].[Export Country]", "[Import Country].[Import Country]" };
            var geoFirstChildToFindIn = "Asia";
            var geoSecondChildToFindIn = "Eastern Asia";
            var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            foreach (var taxonomyGeoFilterId in taxonomyGeoFilterIds)
            {
                var geoTaxonomyFilter = taxonomyFilters.Filters.Single(x => x.Id == taxonomyGeoFilterId).Items.Last();
                var geoFirstChildTaxonomyFilter = geoTaxonomyFilter.Children.SingleOrDefault(x => x.Name == geoFirstChildToFindIn);
                var geoSecondChildTaxonomyFilter = geoFirstChildTaxonomyFilter.Children.SingleOrDefault(x => x.Name == geoSecondChildToFindIn);

                foreach (var geoExpected in chinaGeosExpected)
                {
                    geoSecondChildTaxonomyFilter.Children.SingleOrDefault(x => x.Name == geoExpected).Should().NotBeNull();
                }
            }
        }

        [TestCase(Dataset.AssetCapacityByCompany)]
        public async Task ChinaUserHasNoTaiwanTaxonomyFilters(Dataset dataset)
        {
            var chinaGeosExpected = ChinaUserGeosExpected(dataset);
            var taxonomyGeoFilterId = "source_system_region";
            var geoChildToFindIn = "Northeast Asia";
            var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var geoTaxonomyFilter = taxonomyFilters.Filters.Single(x => x.Id == taxonomyGeoFilterId).Items.First();
            var geoChildTaxonomyFilter = geoTaxonomyFilter.Children.SingleOrDefault(x => x.Name == geoChildToFindIn);
            foreach (var geoExpected in chinaGeosExpected)
            {
                geoChildTaxonomyFilter.Children.SingleOrDefault(x => x.Name == geoExpected).Should().BeNull();
            }

            var taxonomyCompanyFilterId = "producer";
            foreach (var geoExpected in chinaGeosExpected)
            {
                geoChildTaxonomyFilter.Children.SingleOrDefault(x => x.Name == taxonomyCompanyFilterId).Should().BeNull();
            }
        }

        [TestCase(Dataset.CountryRiskScores)]
        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task ChinaUserGridData(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var chinaGeosExpected = ChinaUserGeosExpected(dataset);

            var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            Dictionary<string, List<string>> taxonomyFiltersWithItems = TaxonomyFiltersData.GetChinaUserTaxomonyFiltersItems(dataset);
            var seriesFromGridRequest = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems);

            var columnNamesToBeChecked = ChinaUserColumnsToBeChecked(dataset);
            foreach (var columnName in columnNamesToBeChecked)
            {
                displaySettings = await _buildQueryModule.ShowHideColumn(dataset, displaySettings, columnName, true);

                var columnKey = _buildQueryModule.GetColumnKeyByName(displaySettings, columnName);
                var columnValues = _buildQueryModule.GetGridValuesForParticularColumn(seriesFromGridRequest, columnKey);

                foreach (var geoExpected in chinaGeosExpected)
                {
                    columnValues.Should().Contain(geoExpected);
                }
            }
        }

        [TestCase(Series.GTAForecastingChinaUser1, Series.GTAForecastingChinaUser2, Series.GTAForecastingChinaUser3, Series.GTAForecastingChinaUser4)]
        [TestCase(Series.ComparativeIndustryChinaUser1, Series.ComparativeIndustryChinaUser2)]
        [TestCase(Series.CountryRiskScoresChinaUser1, Series.CountryRiskScoresChinaUser2, Series.CountryRiskScoresChinaUser3)]
        public async Task ChinaUserWorkbookGridData(params Series[] testSeries)
        {
            _workspaceModule.SetUp();
            var dataset = testSeries.First().GetTestSeries().Dataset;
            var chinaGeosExpected = ChinaUserGeosExpected(dataset);

            var displaySettings = await DataSetSettingsModule.GetDataSetSettings(DataSetSettingsType.Dataset, testSeries[0]);

            var results = await _workspaceModule.AddSeriesToWorkspace(displaySettings, testSeries);
            await WorkbookModule.CreateWorkbook("ChinaUserWorkbook");
            var openedWorkbook = await WorkbookModule.OpenWorkbook();
            var series = openedWorkbook.Response.Payload.Results.Series;

            var columnKeys = ChinaUserWorkbookColumnKeysToBeChecked(dataset);
            foreach (var columnKey in columnKeys)
            {
                var columnValues = _buildQueryModule.GetGridValuesForParticularColumn(series.ToList(), columnKey);

                foreach (var geoExpected in chinaGeosExpected)
                {
                    columnValues.Should().Contain(geoExpected);
                }
            }
        }

        private List<string> ChinaUserGeosExpected(Dataset dataset)
        {
            switch (dataset)
            {
                case Dataset.CountryRiskScores:
                    return new List<string> { "Hong Kong, China", "Macau, China", "Taiwan, China" };
                case Dataset.ComparativeIndustryRev4:
                    return new List<string> { "Hong Kong, China", "Taiwan, China" };
                case Dataset.GtaForecasting:
                    return new List<string> { "Hong Kong, China", "Macao, China", "Taiwan, China" };
                case Dataset.AssetCapacityByCompany:
                    return new List<string> { "Hong Kong", "Macao", "Taiwan" };
                default:
                    throw new Exception("Incorrect dataset name. Choose corect dataset or add new one to TaxonomyFiltersData.GetChinaUserTaxomonyFiltersItems method");
            }
        }

        private List<string> ChinaUserColumnsToBeChecked(Dataset dataset)
        {
            switch (dataset)
            {
                case Dataset.CountryRiskScores:
                    return new List<string> { "Geography" };
                case Dataset.ComparativeIndustryRev4:
                    return new List<string> { "Geography" };
                case Dataset.GtaForecasting:
                    return new List<string> { "Export Country/Territory", "Import Country/Territory" };
                case Dataset.AssetCapacityByCompany:
                    return new List<string> { "Geography", "Product" };
                default:
                    throw new Exception("Incorrect dataset name. Choose corect dataset or add new one to TaxonomyFiltersData.GetChinaUserTaxomonyFiltersItems method");
            }
        }

        private List<string> ChinaUserWorkbookColumnKeysToBeChecked(Dataset dataset)
        {
            switch (dataset)
            {
                case Dataset.CountryRiskScores:
                    return new List<string> { "geography" };
                case Dataset.ComparativeIndustryRev4:
                    return new List<string> { "geography" };
                case Dataset.GtaForecasting:
                    return new List<string> { "exportcountry", "importcountry" };
                default:
                    throw new Exception("Incorrect dataset name. Choose corect dataset or add new one to TaxonomyFiltersData.GetChinaUserTaxomonyFiltersItems method");
            }
        }
    }
}
