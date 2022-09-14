using Application.AutomatedBackendTests.Builders;
using Application.Contract.Filters;
using Application.Contract.SearchResults;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Application.AutomatedBackendTests.TestData
{
    public static class TaxonomyFiltersData
    {
        public static SearchFilters GetSearchFilters(Dataset dataset, TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , List<string> aggregationIds = null, string frequency = "Annual")
        {
            var taxonomyFiltersWithFiltersIds = GetFilterIdByName(taxonomyFilters, taxonomyFiltersWithItems);

            for (int i = 0; i < aggregationIds?.Count(); i++)
            {
                foreach (var taxonomyFilter in taxonomyFilters.Filters)
                {
                    var isCustomAggregateBelongsToTaxonomyFilter = IsCustomAggregateBelongsToTaxonomyFilter(taxonomyFilter, aggregationIds[i]);
                    if (isCustomAggregateBelongsToTaxonomyFilter)
                    {
                        if (taxonomyFiltersWithFiltersIds.ContainsKey(taxonomyFilter))
                        {
                            taxonomyFiltersWithFiltersIds[taxonomyFilter].Add(aggregationIds[i]);
                        }
                    }
                }
            }

            SearchFilters searchFilters;
            if (frequency != "")
            {
                searchFilters = new SearchFiltersBuilder()
                .SetFilterValues(taxonomyFilters, taxonomyFiltersWithFiltersIds)
                .SetDataset(dataset)
                .SetFrequency(frequency)
                .Build();
            }
            else
            {
                searchFilters = new SearchFiltersBuilder()
                .SetFilterValues(taxonomyFilters, taxonomyFiltersWithFiltersIds)
                .SetDataset(dataset)
                .Build();
            }
            return searchFilters;
        }

        public static SearchFilters SetSearchFiltersNoTaxonomyChosen(Dataset dataset, string frequency = "Annual", string keyword = "")
        {
            SearchFilters searchFilters;
            if (frequency != "")
            {
                searchFilters = new SearchFiltersBuilder()
                .SetDataset(dataset)
                .SetFrequency(frequency)
                .SetKeyword(keyword)
                .Build();
            }
            else
            {
                searchFilters = new SearchFiltersBuilder()
                .SetDataset(dataset)
                .SetKeyword(keyword)
                .Build();
            }
            return searchFilters;
        }

        public static SearchFilters GetSearchFilters(Sector sector, TaxonomyFilters taxonomyFilters = null, Dictionary<string, List<string>> taxonomyFiltersWithItems = null
            , bool isFrequencyChosen = true, string frequency = "Annual", string keyword = "")
        {
            Dictionary<TaxonomyFilter, List<string>> taxonomyFiltersWithFiltersIds = null;
            if (taxonomyFilters != null)
            {
                taxonomyFiltersWithFiltersIds = GetFilterIdByName(taxonomyFilters, taxonomyFiltersWithItems);
            }

            SearchFilters searchFilters;
            if (isFrequencyChosen)
            {
                searchFilters = SetSearchFiltersWithFrequency(sector, taxonomyFilters, taxonomyFiltersWithFiltersIds, frequency, keyword);
            }
            else
            {
                searchFilters = SetSearchFilters(sector, taxonomyFilters, taxonomyFiltersWithFiltersIds, keyword);
            }
            return searchFilters;
        }

        private static SearchFilters SetSearchFiltersWithFrequency(Sector sector, TaxonomyFilters taxonomyFilters, Dictionary<TaxonomyFilter
            , List<string>> taxonomyFiltersWithFiltersIds, string frequency = "Annual", string keyword = "")
        {
            SearchFilters searchFilters;

            if (taxonomyFilters != null)
            {
                searchFilters = new SearchFiltersBuilder()
                .SetFilterValues(taxonomyFilters, taxonomyFiltersWithFiltersIds)
                .SetDataset(sector)
                .SetFrequency(frequency)
                .SetKeyword(keyword)
                .Build();
            }
            else
            {
                searchFilters = new SearchFiltersBuilder()
                .SetDataset(sector)
                .SetFrequency(frequency)
                .SetKeyword(keyword)
                .Build();
            }

            return searchFilters;
        }

        private static SearchFilters SetSearchFilters(Sector sector, TaxonomyFilters taxonomyFilters, Dictionary<TaxonomyFilter, List<string>> taxonomyFiltersWithFiltersIds, string keyword = "")
        {
            SearchFilters searchFilters;

            if (taxonomyFilters != null)
            {
                searchFilters = new SearchFiltersBuilder()
                .SetFilterValues(taxonomyFilters, taxonomyFiltersWithFiltersIds)
                .SetDataset(sector)
                .SetKeyword(keyword)
                .Build();
            }
            else
            {
                searchFilters = new SearchFiltersBuilder()
                .SetDataset(sector)
                .SetKeyword(keyword)
                .Build();
            }

            return searchFilters;
        }

        public static List<string> GetTaxonomyFiltersNameWithCustomAggregate(TaxonomyFilters taxonomyFilters, List<string> aggregationIds = null)
        {
            List<string> TaxonomyFiltersNumberWithCustomAggregate = new List<string>();
            for (int i = 0; i < aggregationIds?.Count(); i++)
            {
                for (int number = 0; number < taxonomyFilters.Filters.Count(); number++)
                {
                    var isCustomAggregateBelongsToTaxonomyFilter = IsCustomAggregateBelongsToTaxonomyFilter(taxonomyFilters.Filters[number], aggregationIds[i]);
                    if (isCustomAggregateBelongsToTaxonomyFilter)
                    {
                        TaxonomyFiltersNumberWithCustomAggregate.Add(taxonomyFilters.Filters[number].Name);
                    }
                }
            }
            return TaxonomyFiltersNumberWithCustomAggregate;
        }

        private static bool IsCustomAggregateBelongsToTaxonomyFilter(TaxonomyFilter taxonomyFilter, string aggregationId = "")
        {
            string MyCustomDimensions = "MyCustomDimensions";

            bool isfiltersContainsCustomAggregate = false;
            if (taxonomyFilter.Items.FirstOrDefault(i => i.Id.Equals(MyCustomDimensions)) != null)
            {
                isfiltersContainsCustomAggregate = taxonomyFilter.Items
                    .FirstOrDefault(i => i.Id.Equals(MyCustomDimensions))
                    .Children.Any(x => x.Id.Equals(aggregationId));
            }

            return isfiltersContainsCustomAggregate;
        }

        public static Dictionary<string, List<string>> GetTaxomonyFiltersItems(Dataset dataset)
        {
            switch (dataset)
            {
                case Dataset.Banking:
                    {
                        Dictionary<string, List<string>> taxonomyFiltersWithItems = new Dictionary<string, List<string>>()
                        {
                            { "Concept", new List<string>() { "Lending to Households", "Lending to Real Estate and Construction", "Interbank Liabilities", "Deposits" } },
                            { "Geography", new List<string>() { "India", "Azerbaijan", "Argentina", "Qatar" } },
                            //{ "Concept", new List<string>() { "Lending to Households", "Lending to Real Estate and Construction"} },
                            //{ "Geography", new List<string>() { "India", "Azerbaijan"} },
                            { "Company", new List<string>() },
                            { "Frequency", new List<string>() },
                            { "Unit", new List<string>() }
                        };
                        return taxonomyFiltersWithItems;
                    }
                case Dataset.ComparativeIndustryRev4:
                    {
                        Dictionary<string, List<string>> taxonomyFiltersWithItems = new Dictionary<string, List<string>>()
                        {
                            { "Concept", new List<string>() { "Total Sales (Gross Output), Nominal", "Index of Total Sales (Gross Output), Nominal", "Total Sales (Gross Output), Real", "Index of Total Sales (Gross Output), Real" } },
                            { "Geography", new List<string>() { "NAFTA (3)", "Canada", "Mexico", "United States" } },
                            { "Industry", new List<string>() { "(101010) Equipment and Services", "(10101010) Oil and Gas Drilling", "(10101020) Oil and Gas Equipment and Services" } }
                        };
                        return taxonomyFiltersWithItems;
                    }
                case Dataset.ConstructionUS:
                    {
                        Dictionary<string, List<string>> taxonomyFiltersWithItems = new Dictionary<string, List<string>>()
                        {
                            { "Geography", new List<string>() { "United States" } },
                            { "Concept", new List<string>() { "Single Family, nominal (Mill. Of US Dollars)", "Multi-Family, nominal (Mill. Of US Dollars)", "Improvements, nominal (Mill. Of US Dollars)" } }
                        };
                        return taxonomyFiltersWithItems;
                    }
                case Dataset.AssetCapacityByCompany:
                    {
                        Dictionary<string, List<string>> taxonomyFiltersWithItems = new Dictionary<string, List<string>>()
                        {
                            { "Concept", new List<string>() { "Capacity to produce" } },
                            { "Product", new List<string>() { "1,1,1-Trichloroethane", "1,4-Butanediol (BDO)" } },
                            { "Geography", new List<string>() },
                            { "Company", new List<string>() }
                        };
                        return taxonomyFiltersWithItems;
                    }
                case Dataset.AssetCapacityByShareholder:
                    {
                        Dictionary<string, List<string>> taxonomyFiltersWithItems = new Dictionary<string, List<string>>()
                        {
                            { "Concept", new List<string>() { "Capacity to produce" } },
                            { "Product", new List<string>() { "1,1,1-Trichloroethane", "Adiponitrile" } },
                            { "Geography", new List<string>() },
                            { "Shareholder", new List<string>() },
                            { "Company", new List<string>() }
                        };
                        return taxonomyFiltersWithItems;
                    }
                case Dataset.GtaForecasting:
                    {
                        Dictionary<string, List<string>> taxonomyFiltersWithItems = new Dictionary<string, List<string>>()
                        {
                            { "Concept", new List<string>() {"Total Trade Real Value", "Total Trade Nominal Value" } },
                            { "Commodity", new List<string>() { "Animal originated products", "Aquatic invertebrates, other", "Beef, fresh or chilled" } },
                            { "Export Country/Territory", new List<string>() { "Canada", "Mexico", "United States" } },
                            { "Import Country/Territory", new List<string>() { "Canada", "Mexico", "United States" } },
                            { "Frequency", new List<string>() { "Annual", "Quarterly" } }
                        };
                        return taxonomyFiltersWithItems;
                    }
                default:
                    throw new Exception("Incorrect dataset name. Choose corect dataset or add new one to TaxonomyFiltersData.GetSearchFilters method");
            }
        }

        public static Dictionary<string, List<string>> GetChinaUserTaxomonyFiltersItems(Dataset dataset)
        {
            switch (dataset)
            {
                case Dataset.ComparativeIndustryRev4:
                    {
                        Dictionary<string, List<string>> taxonomyFiltersWithItems = new Dictionary<string, List<string>>()
                        {
                            { "Concept", new List<string>() { "Total Sales (Gross Output), Nominal", "Total Sales (Gross Output), Real" } },
                            { "Geography", new List<string>() { "Hong Kong, China", "Taiwan, China" } },
                            { "Industry", new List<string>() { "(A) Agriculture, Forestry and Fishing" } }
                        };
                        return taxonomyFiltersWithItems;
                    }
                case Dataset.CountryRiskScores:
                    {
                        Dictionary<string, List<string>> taxonomyFiltersWithItems = new Dictionary<string, List<string>>()
                        {
                            { "Concept", new List<string>() { "Government instability" } },
                            { "Geography", new List<string>() { "Hong Kong, China", "Macau, China", "Taiwan, China" } },
                            { "Frequency", new List<string>() { "Annual" } }
                        };
                        return taxonomyFiltersWithItems;
                    }
                case Dataset.GtaForecasting:
                    {
                        Dictionary<string, List<string>> taxonomyFiltersWithItems = new Dictionary<string, List<string>>()
                        {
                            { "Concept", new List<string>() {"Total Trade Real Value" } },
                            { "Commodity", new List<string>() { "Animal originated products" } },
                            { "Export Country/Territory", new List<string>() { "Hong Kong, China", "Macao, China", "Taiwan, China" } },
                            { "Import Country/Territory", new List<string>() { "Hong Kong, China", "Macao, China", "Taiwan, China" } },
                            { "Frequency", new List<string>() { "Annual" } }
                        };
                        return taxonomyFiltersWithItems;
                    }
                default:
                    throw new Exception("Incorrect dataset name. Choose corect dataset or add new one to TaxonomyFiltersData.GetChinaUserTaxomonyFiltersItems method");
            }
        }

        public static Dictionary<TaxonomyFilter, List<string>> GetFilterIdByName(TaxonomyFilters taxonomyFilters, Dictionary<string, List<string>> taxonomyFiltersWithItems)
        {
            Dictionary<TaxonomyFilter, List<string>> taxonomyFiltersWithFiltersIds = new Dictionary<TaxonomyFilter, List<string>>();

            foreach (var taxonomyFilter in taxonomyFilters.Filters)
            {
                var taxonomyFilterToBeChecked = taxonomyFiltersWithItems.Single(x => x.Key == taxonomyFilter.Name);

                List<string> filterValues = new List<string>();

                foreach (var filterNameToBeFound in taxonomyFilterToBeChecked.Value)
                {
                    var filterId = taxonomyFilter.Items.SingleOrDefault(x => x.Name == filterNameToBeFound)?.Id;
                    if (filterId == null)
                    {
                        foreach (var child in taxonomyFilter.Items)
                        {
                            filterId = GetFilterIdItem(child, filterNameToBeFound);

                            if (filterId != "")
                            {
                                break;
                            }
                        }
                    }
                    filterValues.Add(filterId);
                }
                taxonomyFiltersWithFiltersIds.Add(taxonomyFilter, filterValues);
            }

            return taxonomyFiltersWithFiltersIds;
        }

        private static string GetFilterIdItem(TaxonomyFilterItem taxonomyFilterItem, string filterNameToBeFound)
        {
            string filterId = "";
            if (taxonomyFilterItem.Name == filterNameToBeFound)
            {
                filterId = taxonomyFilterItem.Id;
            }
            else
            {
                for (int i = 0; i < taxonomyFilterItem.Children.Count(); i++)
                {
                    filterId = GetFilterIdItem(taxonomyFilterItem.Children.ToList()[i], filterNameToBeFound);
                    if (filterId != "")
                        break;
                }
            }

            return filterId;
        }

        public static Dictionary<string, List<string>> GetCustomAggregatesExpected(Dataset dataset)
        {
            switch (dataset)
            {
                case Dataset.ComparativeIndustryRev4:
                    {
                        Dictionary<string, List<string>> customAggregatesExpected = new Dictionary<string, List<string>>()
                        {
                            { "Geography", new List<string>() { "AAA Custom Aggregate test", "Formula Custom Aggregate test" } }
                        };
                        return customAggregatesExpected;
                    }
                case Dataset.GtaForecasting:
                    {
                        Dictionary<string, List<string>> customAggregatesExpected = new Dictionary<string, List<string>>()
                        {
                            { "Commodity", new List<string>() { "AAA Custom Aggregate test", "Formula Custom Aggregate test" } }
                        };
                        return customAggregatesExpected;
                    }
                default:
                    throw new Exception("Incorrect dataset name. Choose corect dataset or add new one to TaxonomyFiltersData.GetSearchFilters method");
            }
        }
    }
}
