using Application.AutomatedBackendTests.Modules;
using Application.Contract.SearchResults;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.AutomatedBackendTests.TestData
{
    public static class GroupingSortingOptions
    {
        #region Ordinary GroupingSorting
        public static async Task IterateOverCubeSortingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            List<string> comparativeIndustryRev4Columns = new List<string>() { "Concept", "Geography", "Industry" };
            List<string> gtafColumns = new List<string>() { "Concept", "Import Country/Territory", "Export Country/Territory" };
            List<string> initialColumnNames;

            if (dataset == Dataset.ComparativeIndustryRev4)
            {
                initialColumnNames = comparativeIndustryRev4Columns;
            }
            else
            {
                initialColumnNames = gtafColumns;
            }

            columnNamesToBeSorted = new List<string>() { initialColumnNames[2] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);




            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1], initialColumnNames[2] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Tree, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);




            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Tree, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Tree, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverGtafSortingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            List<string> initialColumnNames = new List<string>() { "Concept", "Import Country/Territory" };

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverMagellanSortingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            columnNamesToBeSorted = new List<string>() { "Concept" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { "Concept" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);


            columnNamesToBeSorted = new List<string>() { "Concept", "Geography", "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { "Concept", "Geography", "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { "Concept", "Geography", "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { "Concept", "Geography", "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverCubeGroupingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeGrouped;
            List<SortOrder> sortOrders;

            List<string> comparativeIndustryRev4Columns = new List<string>() { "Concept", "Geography", "Industry" };
            List<string> gtafColumns = new List<string>() { "Concept", "Import Country/Territory", "Export Country/Territory" };
            List<string> initialColumnNames;

            if (dataset == Dataset.ComparativeIndustryRev4)
            {
                initialColumnNames = comparativeIndustryRev4Columns;
            }
            else
            {
                initialColumnNames = gtafColumns;
            }

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[2] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);




            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1], initialColumnNames[2] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Tree, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);




            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Tree, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Tree, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverGtafGroupingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeGrouped;
            List<SortOrder> sortOrders;

            List<string> initialColumnNames = new List<string>() { "Concept", "Import Country/Territory" };

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverMagellanGroupingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeGrouped;
            List<SortOrder> sortOrders;

            columnNamesToBeGrouped = new List<string>() { "Concept" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Concept" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);




            columnNamesToBeGrouped = new List<string>() { "Concept", "Geography", "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Concept", "Geography", "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Concept", "Geography", "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Concept", "Geography", "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverMagellanGroupingSortingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeGrouped;
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            columnNamesToBeGrouped = new List<string>() { "Geography", "Concept" };
            columnNamesToBeSorted = new List<string>() { "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Geography", "Concept" };
            columnNamesToBeSorted = new List<string>() { "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Geography", "Concept" };
            columnNamesToBeSorted = new List<string>() { "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Geography", "Concept" };
            columnNamesToBeSorted = new List<string>() { "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);


            columnNamesToBeGrouped = new List<string>() { "Geography" };
            columnNamesToBeSorted = new List<string>() { "Concept", "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Geography" };
            columnNamesToBeSorted = new List<string>() { "Concept", "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Geography" };
            columnNamesToBeSorted = new List<string>() { "Concept", "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Geography" };
            columnNamesToBeSorted = new List<string>() { "Concept", "Company" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverCubesGroupingSortingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeGrouped;
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            List<string> comparativeIndustryRev4Columns = new List<string>() { "Concept", "Geography", "Industry" };
            List<string> gtafColumns = new List<string>() { "Concept", "Import Country/Territory", "Export Country/Territory" };
            List<string> initialColumnNames;

            if (dataset == Dataset.ComparativeIndustryRev4)
            {
                initialColumnNames = comparativeIndustryRev4Columns;
            }
            else
            {
                initialColumnNames = gtafColumns;
            }

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            columnNamesToBeSorted = new List<string>() { initialColumnNames[2] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Tree, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Tree, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Tree, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);



            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0] };
            columnNamesToBeSorted = new List<string>() { initialColumnNames[1], initialColumnNames[2] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Tree, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Tree, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Tree, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverGtafGroupingSortingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeGrouped;
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            List<string> initialColumnNames = new List<string>() { "Concept", "Import Country/Territory" };

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0] };
            columnNamesToBeSorted = new List<string>() { initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);



            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1] };
            columnNamesToBeSorted = new List<string>() { initialColumnNames[0] };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped, columnNamesToBeSorted, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverMagellanSortingReorderingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            columnNamesToBeSorted = new List<string>() { "Geography", "Concept" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);



            columnNamesToBeSorted = new List<string>() { "Geography", "Concept" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);



            columnNamesToBeSorted = new List<string>() { "Geography", "Concept" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverCubesSortingReorderingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            List<string> comparativeIndustryRev4Columns = new List<string>() { "Concept", "Geography", "Industry" };
            List<string> gtafColumns = new List<string>() { "Concept", "Import Country/Territory", "Export Country/Territory" };
            List<string> initialColumnNames;

            if (dataset == Dataset.ComparativeIndustryRev4)
            {
                initialColumnNames = comparativeIndustryRev4Columns;
            }
            else
            {
                initialColumnNames = gtafColumns;
            }

            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);



            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);



            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);



            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);



            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);



            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverGtafSortingReorderingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            List<string> initialColumnNames = new List<string>() { "Concept", "Import Country/Territory" };

            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);



            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);



            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverMagellanGroupingReorderingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeGrouped;
            List<SortOrder> sortOrders;

            columnNamesToBeGrouped = new List<string>() { "Geography", "Concept" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);



            columnNamesToBeGrouped = new List<string>() { "Geography", "Concept" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);



            columnNamesToBeGrouped = new List<string>() { "Geography", "Concept" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { "Concept", "Geography" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverCubesGroupingReorderingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeGrouped;
            List<SortOrder> sortOrders;

            List<string> comparativeIndustryRev4Columns = new List<string>() { "Concept", "Geography", "Industry" };
            List<string> gtafColumns = new List<string>() { "Concept", "Import Country/Territory", "Export Country/Territory" };
            List<string> initialColumnNames;

            if (dataset == Dataset.ComparativeIndustryRev4)
            {
                initialColumnNames = comparativeIndustryRev4Columns;
            }
            else
            {
                initialColumnNames = gtafColumns;
            }

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);



            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);



            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);



            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);



            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);



            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverGtafGroupingReorderingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeGrouped;
            List<SortOrder> sortOrders;

            List<string> initialColumnNames = new List<string>() { "Concept", "Import Country/Territory" };

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);



            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);



            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1], initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, isDynamicFiltersChosen: false);
        }
        #endregion

        #region CustomAggregates GroupingSorting
        public static async Task IterateOverCubeWithCustomAggregatesSortingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            List<string> comparativeIndustryRev4Columns = new List<string>() { "Concept", "Geography", "Industry" };
            List<string> gtafColumns = new List<string>() { "Concept", "Import Country/Territory", "Export Country/Territory" };
            List<string> initialColumnNames;

            if (dataset == Dataset.ComparativeIndustryRev4)
            {
                initialColumnNames = comparativeIndustryRev4Columns;
            }
            else
            {
                initialColumnNames = gtafColumns;
            }

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverGtafWithCustomAggregatesSortingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            List<string> initialColumnNames = new List<string>() { "Concept", "Import Country/Territory" };

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverCubeWithCustomAggregatesGroupingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeGrouped;
            List<SortOrder> sortOrders;

            List<string> comparativeIndustryRev4Columns = new List<string>() { "Concept", "Geography", "Industry" };
            List<string> gtafColumns = new List<string>() { "Concept", "Import Country/Territory", "Export Country/Territory" };
            List<string> initialColumnNames;

            if (dataset == Dataset.ComparativeIndustryRev4)
            {
                initialColumnNames = comparativeIndustryRev4Columns;
            }
            else
            {
                initialColumnNames = gtafColumns;
            }

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Tree };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Tree, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverGtafWithCustomAggregatesGroupingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeGrouped;
            List<SortOrder> sortOrders;

            List<string> initialColumnNames = new List<string>() { "Concept", "Import Country/Territory" };

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            columnNamesToBeGrouped = new List<string>() { initialColumnNames[0], initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Asc, SortOrder.Desc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);

            sortOrders = new List<SortOrder>() { SortOrder.Desc, SortOrder.Asc };
            await CommonTestsParts.CheckGroupingSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeGrouped: columnNamesToBeGrouped, aggregationIds: aggregationIds, isDynamicFiltersChosen: false);
        }
        #endregion

        #region Sorting through multiple pages
        public static async Task IterateOverCubeMultiplePagesSortingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            List<string> comparativeIndustryRev4Columns = new List<string>() { "Concept", "Geography", "Industry" };
            List<string> gtafColumns = new List<string>() { "Concept", "Import Country/Territory", "Export Country/Territory" };
            List<string> initialColumnNames;

            if (dataset == Dataset.ComparativeIndustryRev4)
            {
                initialColumnNames = comparativeIndustryRev4Columns;
            }
            else
            {
                initialColumnNames = gtafColumns;
            }

            columnNamesToBeSorted = new List<string>() { initialColumnNames[2] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc };
            await CommonTestsParts.CheckMultiplePagesSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc };
            await CommonTestsParts.CheckMultiplePagesSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[1] };
            sortOrders = new List<SortOrder>() { SortOrder.Tree };
            await CommonTestsParts.CheckMultiplePagesSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverMagellanMultiplePagesSortingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            columnNamesToBeSorted = new List<string>() { "Concept" };
            sortOrders = new List<SortOrder>() { SortOrder.Asc };
            await CommonTestsParts.CheckMultiplePagesSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { "Concept" };
            sortOrders = new List<SortOrder>() { SortOrder.Desc };
            await CommonTestsParts.CheckMultiplePagesSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);
        }

        public static async Task IterateOverGtafMultiplePagesSortingOptions
            (BuildQueryModule buildQueryModule, DatasetSettingsModule datasetSettingsModule, Dataset dataset, List<string> aggregationIds = null, bool isDynamicFiltersChosen = true)
        {
            List<string> columnNamesToBeSorted;
            List<SortOrder> sortOrders;

            List<string> initialColumnNames = new List<string>() { "Concept", "Import Country/Territory" };

            columnNamesToBeSorted = new List<string>() { initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Asc };
            await CommonTestsParts.CheckMultiplePagesSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);

            columnNamesToBeSorted = new List<string>() { initialColumnNames[0] };
            sortOrders = new List<SortOrder>() { SortOrder.Desc };
            await CommonTestsParts.CheckMultiplePagesSorting
                (buildQueryModule, datasetSettingsModule, dataset, sortOrders, columnNamesToBeSorted: columnNamesToBeSorted, isDynamicFiltersChosen: false);
        }
        #endregion
    }
}
