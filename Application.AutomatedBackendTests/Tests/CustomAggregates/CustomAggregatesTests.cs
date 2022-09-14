using Application.AutomatedBackendTests.Extensions;
using Application.AutomatedBackendTests.Helpers;
using Application.AutomatedBackendTests.Modules;
using Application.AutomatedBackendTests.Requests;
using Application.AutomatedBackendTests.TestData;
using Application.Contract.Filters;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Series = Application.AutomatedBackendTests.TestData.Series;

namespace Application.AutomatedBackendTests.Tests.CustomAggregates
{
    [TestFixture]
    [Category("ApplicationBackendTests")]
    public class CustomAggregatesTests
    {
        private readonly Lazy<int> _folderId = new Lazy<int>(() => EnvHelper.GetFolderId(123, 456));

        private readonly User _user = new User(123456, "Application.User", UserHelper.GetPassword());

        private int FolderId => _folderId.Value;

        private DatasetSettingsModule _datasetSettingsModule;
        private CustomAggregateModule _customAggregateModule;
        private WorkspaceModule _workspaceModule;
        private BuildQueryModule _buildQueryModule;
        private MySavedItemCleaner _cleaner;

        private readonly List<string> _availableCustomAggregationsList = new List<string>
            {
                "SUM", "AVG", "MIN", "MAX", "MEDIAN", "VARIANCE", "STDDEV", "COUNT", "WEIGHTED_ARITHMETIC_AVG", "WEIGHTED_GEOMETRIC_AVG"
            };
        private const string noNameCustomAggregateErrorMessage = "Name has not been specified";
        private const string noAggregationItemsSelectedErrorMessage = "Aggregation items have not been specified";
        private const string noFormulaSpecifiedErrorMessage = "Formula needs to be specified";
        private const string incorrectFormulaErrorMessage = "Formula operators cannot appear successively";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _datasetSettingsModule = new DatasetSettingsModule(_user);
            _customAggregateModule = new CustomAggregateModule(_user, FolderId);
            _workspaceModule = new WorkspaceModule(_user);
            _buildQueryModule = new BuildQueryModule(_user);
            _cleaner = new MySavedItemCleaner(_user);
        }

        [SetUp]
        public void SetUp()
        {
            _workspaceModule.SetUp();
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task CreatingCustomAggregateTest(Dataset dataset)
        {
            var filters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithCustomAggregates = filters.GetFirstTaxonomyFilterWithCustomAggregates();
            var createdCustomAggregate = await _customAggregateModule.CreateCustomAggregate(dataset,
                filterWithCustomAggregates, $"{dataset.GetDatasetName()} test");

            var filtersAfterAddingCustomAggregate = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithAddedCustomAggregate = filtersAfterAddingCustomAggregate.Filters
                .First(f => f.Id.Equals(filterWithCustomAggregates.Id));

            filterWithAddedCustomAggregate.Items.First().Children.Any(c => c.Id.Equals(createdCustomAggregate.Id)).Should()
                .BeTrue("Created custom aggregate should be available in taxonomy filters");

            var savedItem = await _customAggregateModule.GetUserObject(createdCustomAggregate);

            await _cleaner.DeleteCustomAggregate(savedItem.Id.Value);

            var filtersAfterRemovingCustomAggregate = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithDeletedCustomAggregate = filtersAfterRemovingCustomAggregate.Filters
                .First(f => f.Id.Equals(filterWithCustomAggregates.Id));

            filterWithDeletedCustomAggregate.Items.First().Children.Any(c => c.Id.Equals(createdCustomAggregate.Id)).Should()
                .BeFalse("Custom aggregate should be deleted correctly");
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task CheckErrorsForCustomAggregateTest(Dataset dataset)
        {
            var filters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithCustomAggregates = filters.GetFirstTaxonomyFilterWithCustomAggregates();
            var createdNoNameCustomAggregate = await _customAggregateModule.CreateErrorCustomAggregate(dataset,
                filterWithCustomAggregates, "");
            createdNoNameCustomAggregate.Should().Contain(noNameCustomAggregateErrorMessage);

            var createdAggregationItemsSelectedCustomAggregate = await _customAggregateModule.CreateErrorCustomAggregate(dataset,
                filterWithCustomAggregates, $"{dataset.GetDatasetName()} test", 0);
            createdAggregationItemsSelectedCustomAggregate.Should().Contain(noAggregationItemsSelectedErrorMessage);



            var secondFilterWithCustomAggregates = filters.GetTaxonomyFilterWithCustomAggregates();
            var solidFilterItems = secondFilterWithCustomAggregates.Items.Where(x => (x.Type == FilterItemType.Checkable) && (x.IsDeletable == null || x.IsDeletable == false)).ToList();

            string formula = "";
            for (int i = 0; i < solidFilterItems.Count; i++)
            {
                formula += $"[{solidFilterItems[i].Name}]";

                if (i != solidFilterItems.Count - 1)
                {
                    formula += "+";
                }
            }

            var createdNoNameFormula = await _customAggregateModule.CreateErrorFormula(dataset,
                    secondFilterWithCustomAggregates, "", formula);
            createdNoNameFormula.Should().Contain(noNameCustomAggregateErrorMessage);

            var createdNoFormulaSpecified = await _customAggregateModule.CreateErrorFormula(dataset,
                    secondFilterWithCustomAggregates, $"{dataset.GetDatasetName()} Formula test", "");
            createdNoFormulaSpecified.Should().Contain(noFormulaSpecifiedErrorMessage);

            formula += "+";

            var createdIncorrectFormula = await _customAggregateModule.CreateErrorFormula(dataset,
                    secondFilterWithCustomAggregates, $"{dataset.GetDatasetName()} Formula test", formula);
            createdIncorrectFormula.Should().Contain(incorrectFormulaErrorMessage);
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task CreatingDifferentCustomAggregateTypesTest(Dataset dataset)
        {
            var filters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithCustomAggregates = filters.GetFirstTaxonomyFilterWithCustomAggregates();

            for (int i = 0; i < _availableCustomAggregationsList.Count(); i++)
            {
                var createdCustomAggregate = await _customAggregateModule.CreateCustomAggregate(dataset,
                    filterWithCustomAggregates, $"{dataset.GetDatasetName()} + {_availableCustomAggregationsList[i]} aggregation test", aggregationType: _availableCustomAggregationsList[i]);

                var filtersAfterAddingCustomAggregate = await _buildQueryModule.GetTaxonomyFilters(dataset);
                var filterWithAddedCustomAggregate = filtersAfterAddingCustomAggregate.Filters
                    .First(f => f.Id.Equals(filterWithCustomAggregates.Id));

                filterWithAddedCustomAggregate.Items.First().Children.Any(c => c.Id.Equals(createdCustomAggregate.Id)).Should()
                    .BeTrue($"Created custom aggregate {_availableCustomAggregationsList[i]} should be available in taxonomy filters");

                createdCustomAggregate.AggregationType.Should().BeEquivalentTo(_availableCustomAggregationsList[i]
                , $"Aggregation method should be {_availableCustomAggregationsList[i]}");

                var savedItem = await _customAggregateModule.GetUserObject(createdCustomAggregate);

                await _cleaner.DeleteCustomAggregate(savedItem.Id.Value);

                var filtersAfterRemovingCustomAggregate = await _buildQueryModule.GetTaxonomyFilters(dataset);
                var filterWithDeletedCustomAggregate = filtersAfterRemovingCustomAggregate.Filters
                    .First(f => f.Id.Equals(filterWithCustomAggregates.Id));

                filterWithDeletedCustomAggregate.Items.First().Children.Any(c => c.Id.Equals(createdCustomAggregate.Id)).Should()
                    .BeFalse($"Custom aggregate {_availableCustomAggregationsList[i]} should be deleted correctly");
            }
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task EditingCustomAggregateTest(Dataset dataset)
        {
            var filters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var secondFilterWithCustomAggregates = filters.GetTaxonomyFilterWithCustomAggregates();
            var createdCustomAggregate = await _customAggregateModule.CreateCustomAggregate(dataset,
                secondFilterWithCustomAggregates, $"{dataset.GetDatasetName()} Edit aggregation test", aggregationType: _availableCustomAggregationsList[0]);

            var filtersAfterAddingCustomAggregate = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithAddedCustomAggregate = filtersAfterAddingCustomAggregate.Filters
                .First(f => f.Id.Equals(secondFilterWithCustomAggregates.Id));

            filterWithAddedCustomAggregate.Items.First().Children.Any(c => c.Id.Equals(createdCustomAggregate.Id)).Should()
                .BeTrue("Created custom aggregate should be available in taxonomy filters");

            var updatedCustomAggregate = await _customAggregateModule.UpdateCustomAggregate(dataset,
                secondFilterWithCustomAggregates, $"{dataset.GetDatasetName()} Edit aggregation test UPDATED", createdCustomAggregate.Id, aggregationType: _availableCustomAggregationsList[1]);

            var filtersAfterUpdatingCustomAggregate = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithUpdatedCustomAggregate = filtersAfterUpdatingCustomAggregate.Filters
                .First(f => f.Id.Equals(secondFilterWithCustomAggregates.Id));

            filterWithUpdatedCustomAggregate.Items.First().Children.Any(c => c.Id.Equals(updatedCustomAggregate.Id)).Should()
                .BeTrue("Updated custom aggregate should be available in taxonomy filters");

            var savedItem = await _customAggregateModule.GetUserObject(updatedCustomAggregate);

            savedItem.Name.Should().BeEquivalentTo($"{dataset.GetDatasetName()} Edit aggregation test UPDATED", "Updated Name should match expected new one");
            updatedCustomAggregate.AggregationType.Should().BeEquivalentTo(_availableCustomAggregationsList[1]
                , $"Aggregation method should be changed from {_availableCustomAggregationsList[0]} to {_availableCustomAggregationsList[1]}");

            await _cleaner.DeleteCustomAggregate(savedItem.Id.Value);

            var filtersAfterRemovingCustomAggregate = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithDeletedCustomAggregate = filtersAfterRemovingCustomAggregate.Filters
                .First(f => f.Id.Equals(secondFilterWithCustomAggregates.Id));

            filterWithDeletedCustomAggregate.Items.First().Children.Any(c => c.Id.Equals(updatedCustomAggregate.Id)).Should()
                .BeFalse("Updated custom aggregate should be deleted correctly");
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task CreatingFormulaTest(Dataset dataset)
        {
            var filters = await _buildQueryModule.GetTaxonomyFilters(dataset);

            var secondFilterWithCustomAggregates = filters.GetTaxonomyFilterWithCustomAggregates();

            var solidFilterItems = secondFilterWithCustomAggregates.Items.Where(x => (x.Type == FilterItemType.Checkable) && (x.IsDeletable == null || x.IsDeletable == false)).ToList();

            string formula = "";
            for (int i = 0; i < solidFilterItems.Count; i++)
            {
                formula += $"[{solidFilterItems[i].Name}]";

                if (i != solidFilterItems.Count - 1)
                {
                    formula += "+";
                }
            }

            var createdFormula = await _customAggregateModule.CreateFormula(dataset,
                    secondFilterWithCustomAggregates, $"{dataset.GetDatasetName()} Formula test", formula);

            var filtersAfterAddingCustomAggregate = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithAddedCustomAggregate = filtersAfterAddingCustomAggregate.Filters
                .First(f => f.Id.Equals(secondFilterWithCustomAggregates.Id));

            filterWithAddedCustomAggregate.Items.First().Children.Any(c => c.Id.Equals(createdFormula.Id)).Should()
                .BeTrue($"Formula should be available in taxonomy filters");

            createdFormula.Formula.Should().BeEquivalentTo(formula, $"Formula should be {formula}");

            var savedItem = await _customAggregateModule.GetUserObject(createdFormula);

            await _cleaner.DeleteCustomAggregate(savedItem.Id.Value);

            var filtersAfterRemovingCustomAggregate = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithDeletedCustomAggregate = filtersAfterRemovingCustomAggregate.Filters
                .First(f => f.Id.Equals(secondFilterWithCustomAggregates.Id));

            filterWithDeletedCustomAggregate.Items.First().Children.Any(c => c.Id.Equals(createdFormula.Id)).Should()
                .BeFalse($"Formula should be deleted correctly");
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        public async Task EditingFormulaTest(Dataset dataset)
        {
            var filters = await _buildQueryModule.GetTaxonomyFilters(dataset);

            var secondFilterWithCustomAggregates = filters.GetTaxonomyFilterWithCustomAggregates();

            var solidFilterItems = secondFilterWithCustomAggregates.Items.Where(x => (x.Type == FilterItemType.Checkable) && (x.IsDeletable == null || x.IsDeletable == false)).ToList();

            string formula = "";
            for (int i = 0; i < solidFilterItems.Count; i++)
            {
                formula += $"[{solidFilterItems[i].Name}]";

                if (i != solidFilterItems.Count - 1)
                {
                    formula += "+";
                }
            }

            var createdFormula = await _customAggregateModule.CreateFormula(dataset,
                    secondFilterWithCustomAggregates, $"{dataset.GetDatasetName()} Formula test", formula);

            var filtersAfterAddingCustomAggregate = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithAddedCustomAggregate = filtersAfterAddingCustomAggregate.Filters
                .First(f => f.Id.Equals(secondFilterWithCustomAggregates.Id));

            filterWithAddedCustomAggregate.Items.First().Children.Any(c => c.Id.Equals(createdFormula.Id)).Should()
                .BeTrue($"Formula should be available in taxonomy filters");

            createdFormula.Formula.Should().BeEquivalentTo(formula, $"Formula should be {formula}");

            var newFormula = "";
            for (int i = 0; i < solidFilterItems.Count; i++)
            {
                newFormula += $"[{solidFilterItems[i].Name}]";

                if (i != solidFilterItems.Count - 1)
                {
                    newFormula += "-";
                }
            }

            var updatedFormula = await _customAggregateModule.UpdateFormula(dataset,
                    secondFilterWithCustomAggregates, $"{dataset.GetDatasetName()} Formula test - UPDATED", createdFormula.Id, newFormula);

            filtersAfterAddingCustomAggregate = await _buildQueryModule.GetTaxonomyFilters(dataset);
            filterWithAddedCustomAggregate = filtersAfterAddingCustomAggregate.Filters
                .First(f => f.Id.Equals(secondFilterWithCustomAggregates.Id));

            filterWithAddedCustomAggregate.Items.First().Children.Any(c => c.Id.Equals(updatedFormula.Id)).Should()
                .BeTrue($"Formula should be available in taxonomy filters");

            updatedFormula.Formula.Should().BeEquivalentTo(newFormula, $"Formula should be updated from {formula} to {newFormula}");

            var savedItem = await _customAggregateModule.GetUserObject(updatedFormula);

            await _cleaner.DeleteCustomAggregate(savedItem.Id.Value);

            var filtersAfterRemovingCustomAggregate = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithDeletedCustomAggregate = filtersAfterRemovingCustomAggregate.Filters
                .First(f => f.Id.Equals(secondFilterWithCustomAggregates.Id));

            filterWithDeletedCustomAggregate.Items.First().Children.Any(c => c.Id.Equals(createdFormula.Id)).Should()
                .BeFalse($"Updated formula should be deleted correctly");
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task CustomAggregatedSeriesInWorkspaceTest(Dataset dataset)
        {
            var filters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithCustomAggregates = filters.GetFirstTaxonomyFilterWithCustomAggregates();
            var createdCustomAggregate = await _customAggregateModule.CreateCustomAggregate(dataset,
                filterWithCustomAggregates, $"{dataset.GetDatasetName()} workspace test");

            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var seriesFromGrid = await _customAggregateModule.GetTestDataSeriesFromGrid(dataset, displaySettings, filterWithCustomAggregates.Id, createdCustomAggregate.Id);
            var seriesToAdd = seriesFromGrid.Take(2).ToArray();

            var results = await _workspaceModule.AddSeriesToWorkspace(displaySettings, true, seriesToAdd);

            results.SavedWorkspaceInDatabase.GetSeries().Should().BeEquivalentTo(seriesToAdd, "Series should be saved in workspace");

            var savedItem = await _customAggregateModule.GetUserObject(createdCustomAggregate);
            await _cleaner.DeleteCustomAggregate(savedItem.Id.Value);
        }

        [TestCase(Dataset.HealthcareForecast)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task AddingCustomAggregatedSeriesToWorkspaceWithNormalSeriesTest(Dataset dataset)
        {
            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
            var filters = await _buildQueryModule.GetTaxonomyFilters(dataset, displaySettings);
            var filterWithCustomAggregates = filters.GetFirstTaxonomyFilterWithCustomAggregates();
            var createdCustomAggregate = await _customAggregateModule.CreateCustomAggregate(dataset,
                filterWithCustomAggregates, $"{dataset.GetDatasetName()} workspace test");


            var seriesFromGrid = await _customAggregateModule.GetTestDataSeriesFromGrid(dataset, displaySettings, filterWithCustomAggregates.Id, createdCustomAggregate.Id);
            var seriesToAdd = seriesFromGrid.Take(2).ToArray();
            TestSeries wtsSeries;
            if (dataset == Dataset.ComparativeIndustryRev4)
            {
                wtsSeries = SeriesTestData.GetSeries(TestData.Series.ComparativeIndustryRev4Cube1);
            }
            else
            {
                wtsSeries = SeriesTestData.GetSeries(TestData.Series.GTAForecastingAnnual1);
            }

            await _workspaceModule.AddSeriesToWorkspace(displaySettings, false, wtsSeries);
            var results = await _workspaceModule.AddSeriesToWorkspace(displaySettings, true, seriesToAdd);


            results.SavedWorkspaceInDatabase.GetSeries().Should().Contain(wtsSeries);
            results.SavedWorkspaceInDatabase.GetSeries().Should().Contain(seriesToAdd);

            var savedItem = await _customAggregateModule.GetUserObject(createdCustomAggregate);
            await _cleaner.DeleteCustomAggregate(savedItem.Id.Value);
        }

        [TestCase(Dataset.HealthcareForecast)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task AddingNormalSeriesToWorkspaceWithCustomAggregatedSeriesTest(Dataset dataset)
        {
            var filters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var filterWithCustomAggregates = filters.GetFirstTaxonomyFilterWithCustomAggregates();
            var createdCustomAggregate = await _customAggregateModule.CreateCustomAggregate(dataset,
                filterWithCustomAggregates, $"{dataset.GetDatasetName()} workspace test");


            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var seriesFromGrid = await _customAggregateModule.GetTestDataSeriesFromGrid(dataset, displaySettings, filterWithCustomAggregates.Id, createdCustomAggregate.Id);
            var seriesToAdd = seriesFromGrid.Take(2).ToArray();
            var wtsSeries = SeriesTestData.GetSeries(TestData.Series.ComparativeIndustryRev4Cube1);

            await _workspaceModule.AddSeriesToWorkspace(displaySettings, true, seriesToAdd);
            var result = await _workspaceModule.AddSeriesToWorkspace(displaySettings, false, wtsSeries);

            result.SavedWorkspaceInDatabase.GetSeries().Should().Contain(seriesToAdd);
            result.SavedWorkspaceInDatabase.GetSeries().Should().Contain(wtsSeries);

            var savedItem = await _customAggregateModule.GetUserObject(createdCustomAggregate);
            await _cleaner.DeleteCustomAggregate(savedItem.Id.Value);
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task CustomAggregatedAllTypesSeriesSortingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var secondFilterWithCustomAggregates = taxonomyFilters.GetTaxonomyFilterWithCustomAggregates();

            var createdCustomAggregate = await _customAggregateModule.CreateCustomAggregate(dataset,
                secondFilterWithCustomAggregates, TaxonomyFiltersData.GetCustomAggregatesExpected(dataset)[secondFilterWithCustomAggregates.Name][0]);

            var solidFilterItems = secondFilterWithCustomAggregates.Items.Where(x => (x.Type == FilterItemType.Checkable) && (x.IsDeletable == null || x.IsDeletable == false)).ToList();

            string formula = "";
            for (int i = 0; i < solidFilterItems.Count; i++)
            {
                formula += $"[{solidFilterItems[i].Name}]";

                if (i != solidFilterItems.Count - 1)
                {
                    formula += "+";
                }
            }

            var createdFormula = await _customAggregateModule.CreateFormula(dataset,
                    secondFilterWithCustomAggregates, TaxonomyFiltersData.GetCustomAggregatesExpected(dataset)[secondFilterWithCustomAggregates.Name][1], formula);


            List<string> createdCustomAggregatesIds = new List<string>
            {
                createdCustomAggregate.Id,
                createdFormula.Id
            };

            if (dataset == Dataset.ComparativeIndustryRev4)
            {
                await GroupingSortingOptions.IterateOverCubeWithCustomAggregatesSortingOptions(_buildQueryModule, _datasetSettingsModule, dataset, createdCustomAggregatesIds);
            }
            else
            {
                await GroupingSortingOptions.IterateOverGtafWithCustomAggregatesSortingOptions(_buildQueryModule, _datasetSettingsModule, dataset, createdCustomAggregatesIds);
            }

            await _buildQueryModule.ResetSettingsInGrid(dataset);
            var savedItem = await _customAggregateModule.GetUserObject(createdCustomAggregate);
            await _cleaner.DeleteCustomAggregate(savedItem.Id.Value);

            savedItem = await _customAggregateModule.GetUserObject(createdFormula);
            await _cleaner.DeleteCustomAggregate(savedItem.Id.Value);
        }

        [TestCase(Dataset.ComparativeIndustryRev4)]
        [TestCase(Dataset.GtaForecasting)]
        public async Task CustomAggregatedAllTypesSeriesGroupingTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var taxonomyFilters = await _buildQueryModule.GetTaxonomyFilters(dataset);
            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var secondFilterWithCustomAggregates = taxonomyFilters.GetTaxonomyFilterWithCustomAggregates();
            var createdCustomAggregate = await _customAggregateModule.CreateCustomAggregate(dataset,
                secondFilterWithCustomAggregates, TaxonomyFiltersData.GetCustomAggregatesExpected(dataset)[secondFilterWithCustomAggregates.Name][0]);

            var solidFilterItems = secondFilterWithCustomAggregates.Items.Where(x => (x.Type == FilterItemType.Checkable) && (x.IsDeletable == null || x.IsDeletable == false)).ToList();

            string formula = "";
            for (int i = 0; i < solidFilterItems.Count; i++)
            {
                formula += $"[{solidFilterItems[i].Name}]";

                if (i != solidFilterItems.Count - 1)
                {
                    formula += "+";
                }
            }

            var createdFormula = await _customAggregateModule.CreateFormula(dataset,
                    secondFilterWithCustomAggregates, TaxonomyFiltersData.GetCustomAggregatesExpected(dataset)[secondFilterWithCustomAggregates.Name][1], formula);


            List<string> createdCustomAggregatesIds = new List<string>
            {
                createdCustomAggregate.Id,
                createdFormula.Id
            };

            if (dataset == Dataset.ComparativeIndustryRev4)
            {
                await GroupingSortingOptions.IterateOverCubeWithCustomAggregatesGroupingOptions(_buildQueryModule, _datasetSettingsModule, dataset, createdCustomAggregatesIds);
            }
            else
            {
                await GroupingSortingOptions.IterateOverGtafWithCustomAggregatesGroupingOptions(_buildQueryModule, _datasetSettingsModule, dataset, createdCustomAggregatesIds);
            }

            await _buildQueryModule.ResetSettingsInGrid(dataset);

            var savedItem = await _customAggregateModule.GetUserObject(createdCustomAggregate);
            await _cleaner.DeleteCustomAggregate(savedItem.Id.Value);

            savedItem = await _customAggregateModule.GetUserObject(createdFormula);
            await _cleaner.DeleteCustomAggregate(savedItem.Id.Value);
        }
    }
}
