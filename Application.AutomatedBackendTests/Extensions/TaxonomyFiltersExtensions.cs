using Application.AutomatedBackendTests.TestData;
using Application.Contract;
using Application.Contract.Cube;
using Application.Contract.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.AutomatedBackendTests.Extensions
{
    internal static class TaxonomyFiltersExtensions
    {
        private const string MyCustomDimensions = "MyCustomDimensions";

        internal static OlapAggregation CreateOlapAggregation(this TaxonomyFilter taxonomyFilter, Dataset dataset, string name, int countOfFilters = 2, string aggregationType = "SUM", string id = null)
        {
            var activeMembers = new List<AggregationMember>();
            var dataSet = new DataSetId(dataset.GetDatasetInfo().GetId(), DataSetType.DataSet);

            var filtersForAggregate = taxonomyFilter.Items
                .First(i => !i.Id.Equals(MyCustomDimensions))
                .Children
                .Take(countOfFilters);

            if (filtersForAggregate.Count() == 0)
            {
                filtersForAggregate = taxonomyFilter.Items
                .Where(i => !i.Id.Equals(MyCustomDimensions))
                .Take(countOfFilters);
            }

            foreach (var taxonomyFilterItem in filtersForAggregate)
            {
                activeMembers.Add(new AggregationMember
                {
                    Id = taxonomyFilterItem.Id,
                    Weight = 0
                });
            }

            return new OlapAggregation
            {
                Name = name,
                ActiveMembers = activeMembers,
                AggregationType = aggregationType,
                DimensionId = taxonomyFilter.Id,
                DataSet = dataSet,
                Id = id
            };
        }

        internal static OlapFormula CreateOlapFormula(this TaxonomyFilter taxonomyFilter, Dataset dataset, string name, string formula, int countOfFilters = 2, string id = null)
        {
            var dataSet = new DataSetId(dataset.GetDatasetInfo().GetId(), DataSetType.DataSet);

            return new OlapFormula
            {
                Name = name,
                Formula = formula,
                DimensionId = taxonomyFilter.Id,
                DataSet = dataSet,
                Id = id
            };
        }

        internal static TaxonomyFilter GetFirstTaxonomyFilterWithCustomAggregates(this TaxonomyFilters taxonomyFilters)
        {
            return taxonomyFilters.Filters.First(f => f.Items.Any(i => i.Id.Equals(MyCustomDimensions)));
        }

        internal static TaxonomyFilter GetTaxonomyFilterWithCustomAggregates(this TaxonomyFilters taxonomyFilters, int taxonomyFilterNumber = 1)
        {
            return taxonomyFilters.Filters.Where(f => f.Items.Any(i => i.Id.Equals(MyCustomDimensions))).ToList()[taxonomyFilterNumber];
        }

        internal static void SetFilterValues(this IEnumerable<TaxonomyFilterValues> taxonomyFilters, string taxonomyFilterId, params string[] values)
        {
            var taxonomyFilterValueses = taxonomyFilters.ToList();
            var taxonomyFilter = taxonomyFilterValueses.FirstOrDefault(f => f.FilterId.Equals(taxonomyFilterId));
            if (taxonomyFilter == null)
                throw new Exception($"\"{taxonomyFilterId}\" is not in filters");
            var filterValues = taxonomyFilter.Values?.ToList() ?? new List<string>();
            filterValues.AddRange(values);
            taxonomyFilter.Values = filterValues;
        }

        internal static List<TaxonomyFilterValues> MapToTaxonomyFilterValues(this TaxonomyFilters taxonomyFilters)
        {
            return taxonomyFilters.Filters.Select(f => new TaxonomyFilterValues
            {
                FilterId = f.Id,
                Values = null
            }).ToList();
        }
    }
}
