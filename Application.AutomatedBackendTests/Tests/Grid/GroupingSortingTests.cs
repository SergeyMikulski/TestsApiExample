using Application.AutomatedBackendTests.Modules;
using Application.AutomatedBackendTests.TestData;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Application.AutomatedBackendTests.Tests.Grid
{
    [TestFixture]
    [Category("ApplicationBackendTests")]
    public class GroupingSortingTests
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

        [TestCase(Dataset.Banking)]
        public async Task SeriesMagellanSortingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverMagellanSortingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.Banking)]
        public async Task SeriesMagellanGroupingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverMagellanGroupingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task SeriesCubesSortingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverCubeSortingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task SeriesCubesGroupingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverCubeGroupingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.GtaForecasting)]
        public async Task SeriesGtafSortingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverGtafSortingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.GtaForecasting)]
        public async Task SeriesGtafGroupingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverGtafGroupingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.Banking)]
        public async Task SeriesMagellanGroupingSortingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverMagellanGroupingSortingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task SeriesCubesGroupingSortingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverCubesGroupingSortingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }
        [TestCase(Dataset.GtaForecasting)]
        public async Task SeriesGtafGroupingSortingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverGtafGroupingSortingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.Banking)]
        public async Task SeriesMagellanGroupingReorderingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverMagellanGroupingReorderingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task SeriesCubesGroupingReorderingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverCubesGroupingReorderingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.GtaForecasting)]
        public async Task SeriesGtafGroupingReorderingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverGtafGroupingReorderingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.Banking)]
        public async Task SeriesMagellanSortingReorderingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverMagellanSortingReorderingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task SeriesCubesSortingReorderingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverCubesSortingReorderingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.GtaForecasting)]
        public async Task SeriesGtafSortingReorderingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverGtafSortingReorderingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.Banking)]
        public async Task SeriesMagellanMultiplePagesSortingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverMagellanMultiplePagesSortingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task SeriesCubesMultiplePagesSortingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverCubeMultiplePagesSortingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.GtaForecasting)]
        public async Task SeriesGtafMultiplePagesSortingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            await GroupingSortingOptions.IterateOverGtafMultiplePagesSortingOptions(_buildQueryModule, _datasetSettingsModule, dataset);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }
    }
}
