using Application.AutomatedBackendTests.TestData;
using Application.Contract;
using Application.Contract.SearchResults;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.AutomatedBackendTests.Extensions
{
    internal static class SortInfoExtensions
    {
        internal static void ChangeFieldToColumnName(this SortInfo info,
            IEnumerable<ColumnSettings> columnSettingsAfterMapping, DataSetSettings settings)
        {
            var columnFromDataSetSettings = settings.UserSettings.Columns.FirstOrDefault(c => c.Key.Equals(info.Field));
            if (columnFromDataSetSettings == null)
                throw new Exception($"Columns in user settings don't contain column with \"{info.Field}\" field");
            var name = columnSettingsAfterMapping.FirstOrDefault(c =>
                c.HeaderText.Equals(columnFromDataSetSettings.HeaderText));
            if (name == null)
                throw new Exception($"Columns saved in database don't contain column with \"{columnFromDataSetSettings.HeaderText}\" header text");
            info.Field = name.Key;
        }

        public static void CheckDataInColumnsSorting
            (List<string> columnValues, SortOrder sortOrder, string columnNameToBeSorted = "", Dataset dataset = Dataset.ComparativeIndustryRev4, bool isAggregationAdded = false)
        {
            List<string> sortOrderAfterDataSort = new List<string>();
            switch (sortOrder)
            {
                case SortOrder.Asc:
                    {
                        sortOrderAfterDataSort = columnValues.OrderBy(x => x).ToList();
                        columnValues.SequenceEqual(sortOrderAfterDataSort).Should().BeTrue();
                        break;
                    }
                case SortOrder.Desc:
                    {
                        sortOrderAfterDataSort = columnValues.OrderByDescending(x => x).ToList();
                        columnValues.SequenceEqual(sortOrderAfterDataSort).Should().BeTrue();
                        break;
                    }
                case SortOrder.Tree:
                    {
                        CheckTreeSorting(columnValues, sortOrder, columnNameToBeSorted, dataset, isAggregationAdded);
                        break;
                    }
            }
        }

        private static List<string> GetTreeSortOrderExpected(string columnNameToBeSorted, Dataset dataset, bool isAggregationAdded)
        {
            List<string> treeSortOrderCustomAggregatesExpected;
            var treeSortOrderExpectedAllColumns = TaxonomyFiltersData.GetTaxomonyFiltersItems(dataset);
            var treeSortOrderExpectedWithoutAggregates = treeSortOrderExpectedAllColumns[columnNameToBeSorted];
            List<string> treeSortOrderExpected = new List<string>();

            if (isAggregationAdded)
            {
                treeSortOrderCustomAggregatesExpected = TaxonomyFiltersData.GetCustomAggregatesExpected(dataset)[columnNameToBeSorted];
                treeSortOrderExpected.AddRange(treeSortOrderCustomAggregatesExpected);
            }
            treeSortOrderExpected.AddRange(treeSortOrderExpectedWithoutAggregates);

            return treeSortOrderExpected;
        }

        private static void CheckTreeSorting(List<string> columnValues, SortOrder sortOrder, string columnNameToBeSorted, Dataset dataset, bool isAggregationAdded)
        {
            int iterator = 0;
            var treeSortOrderExpected = GetTreeSortOrderExpected(columnNameToBeSorted, dataset, isAggregationAdded);

            foreach (var columnValue in columnValues)
            {
                if (columnValue != treeSortOrderExpected[iterator])
                {
                    if (iterator + 1 < treeSortOrderExpected.Count())
                    {
                        columnValue.Should().BeEquivalentTo(treeSortOrderExpected[iterator + 1], $" {columnNameToBeSorted} column sorting Tree order is incorrect");
                        iterator++;
                    }
                    else
                    {
                        throw new Exception($"{columnNameToBeSorted} column sorting Tree order is incorrect.");
                    }
                }
                else
                {
                    columnValue.Should().BeEquivalentTo(treeSortOrderExpected[iterator], $" {columnNameToBeSorted} column sorting Tree order is incorrect");
                }
            }

            if (iterator + 1 < treeSortOrderExpected.Count())
            {
                throw new Exception($"{columnNameToBeSorted} column sorting Tree order is incorrect.");
            }
        }

        public static void CheckDataForMultipleColumnsSorted
            (List<List<string>> columnsWithValues, List<SortOrder> sortOrder, List<string> columnNamesToBeSorted = null
            , Dataset dataset = Dataset.ComparativeIndustryRev4, List<string> taxonomyFiltersWithCustomAggregatesNames = null)
        {
            var numberOfRepetitiveElementsLists = GetNumbersOfRepetitiveElementsForColumns(columnsWithValues);

            for (int columnNumber = 0; columnNumber < columnsWithValues.Count(); columnNumber++)
            {
                bool isAggregationAdded = false;
                if ((taxonomyFiltersWithCustomAggregatesNames != null) && (taxonomyFiltersWithCustomAggregatesNames.Contains(columnNamesToBeSorted[columnNumber])))
                {
                    isAggregationAdded = true;
                }

                if (columnNumber == 0)
                {
                    CheckDataInColumnsSorting(columnsWithValues[columnNumber], sortOrder[columnNumber], columnNamesToBeSorted[columnNumber], dataset, isAggregationAdded);
                }
                else
                {
                    List<string> columnValuesWithinMultipleSorting = new List<string>();

                    int elementNumber = 0;
                    var number = numberOfRepetitiveElementsLists[columnNumber - 1][elementNumber];
                    for (int columnValueNumber = 0; columnValueNumber < columnsWithValues[columnNumber].Count(); columnValueNumber++)
                    {
                        if (columnValueNumber != number)
                        {
                            columnValuesWithinMultipleSorting.Add(columnsWithValues[columnNumber][columnValueNumber]);
                        }
                        else
                        {
                            CheckDataInColumnsSorting(columnValuesWithinMultipleSorting, sortOrder[columnNumber], columnNamesToBeSorted[columnNumber], dataset, isAggregationAdded);
                            columnValuesWithinMultipleSorting = new List<string>
                            {
                                columnsWithValues[columnNumber][columnValueNumber]
                            };
                            elementNumber++;
                            try
                            {
                                number = numberOfRepetitiveElementsLists[columnNumber - 1][elementNumber];
                            }
                            catch { };
                        }

                        if (columnValueNumber == columnsWithValues[columnNumber].Count() - 1)
                        {
                            CheckDataInColumnsSorting(columnValuesWithinMultipleSorting, sortOrder[columnNumber], columnNamesToBeSorted[columnNumber], dataset, isAggregationAdded);
                        }
                    }
                }
            }
        }

        private static List<List<int>> GetNumbersOfRepetitiveElementsForColumns(List<List<string>> columnsWithValues)
        {
            List<List<int>> numberOfRepetitiveElementsLists = new List<List<int>>();
            foreach (var columnValues in columnsWithValues)
            {
                var isRepetitiveNumberAddedFlag = false;
                List<int> numberOfRepetiveElements = new List<int>();
                for (int i = 1; i < columnValues.Count(); i++)
                {
                    if (columnValues[i] != columnValues[i - 1])
                    {
                        numberOfRepetiveElements.Add(i);
                        isRepetitiveNumberAddedFlag = true;
                    }
                }

                if (!isRepetitiveNumberAddedFlag)
                {
                    numberOfRepetiveElements.Add(0);
                }
                numberOfRepetitiveElementsLists.Add(numberOfRepetiveElements);
            }
            return numberOfRepetitiveElementsLists;
        }
    }
}