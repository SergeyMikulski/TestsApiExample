using Application.AutomatedBackendTests.Extensions;
using Application.AutomatedBackendTests.Models;
using Application.AutomatedBackendTests.Modules;
using Application.AutomatedBackendTests.TestData;
using Application.Common.Model;
using Application.Contract;
using Application.Contract.Filters;
using Application.Contract.Functions;
using Application.Contract.SearchResults;
using FluentAssertions;
using Frequencies;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Series = Application.Contract.SearchResults.Series;

namespace Application.AutomatedBackendTests.Tests.Grid
{
    [TestFixture]
    [Category("ApplicationBackendTests")]
    public class WmmCalculations
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

        [TestCase(Dataset.WorldMarketMonitor)]
        [TestCase(Dataset.GlobalEconomyNew)]
        public async Task RescaleWmmTest(Dataset dataset)
        {
            var wmmNewMnemonics = "AFGN1CUX$.Q";
            var globalEconomyNewNewMnemonicsValue = "X_JPN.Q";
            string mnemonicsValue;

            if (dataset == Dataset.WorldMarketMonitor)
            {
                mnemonicsValue = wmmNewMnemonics;
            }
            else
            {
                mnemonicsValue = globalEconomyNewNewMnemonicsValue;
            }

            await _buildQueryModule.ResetSettingsInGrid(dataset);
            TaxonomyFilters taxonomyFilters = null;
            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;
            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            List<AppliedFunction> appliedFunctions = new List<AppliedFunction>()
            {
                new AppliedFunction
                {
                    FunctionId = (int)Function.Rescaling,
                    Parameter = "Original"
                }
            }.ToList();

            await _buildQueryModule.AddFunctionsToQuery(dataset, _datasetSettingsModule, displaySettings, appliedFunctions);

            displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var frequency = GetRequiredFrequency(mnemonicsValue);

            var series = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: mnemonicsValue);
            var seriesWithDefiniteNewMnemonics = series.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonicsValue)).Any());

            var scaleColumnValues = _buildQueryModule.GetGridValuesForParticularColumn(series, "scale").First();
            var wmmDefaultTargetScaleColumnValues = _buildQueryModule.GetGridValuesForParticularColumn(series, "WmmDefaultTargetScale").First();
            scaleColumnValues.Should().NotBe(wmmDefaultTargetScaleColumnValues);

            var seriesRecalculated = RescaleCalculationAdd(seriesWithDefiniteNewMnemonics);

            appliedFunctions = new List<AppliedFunction>()
            {
                new AppliedFunction
                {
                    FunctionId = (int)Function.Rescaling,
                    Parameter = "Harmonized"
                }
            }.ToList();

            await _buildQueryModule.AddFunctionsToQuery(dataset, _datasetSettingsModule, displaySettings, appliedFunctions);

            var displaySettingsAfterChange = await _datasetSettingsModule.GetDataSetSettings(dataset);
            series = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettingsAfterChange, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: mnemonicsValue);
            seriesWithDefiniteNewMnemonics = series.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonicsValue)).Any());

            VerifyWmmCalculationsResults(seriesRecalculated, seriesWithDefiniteNewMnemonics);
            scaleColumnValues = _buildQueryModule.GetGridValuesForParticularColumn(series, "scale").First();
            wmmDefaultTargetScaleColumnValues = _buildQueryModule.GetGridValuesForParticularColumn(series, "WmmDefaultTargetScale").First();
            scaleColumnValues.Should().Be(wmmDefaultTargetScaleColumnValues);

            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.WorldMarketMonitor, Annualization.NonAnnualized)]
        [TestCase(Dataset.WorldMarketMonitor, Annualization.Annualized)]
        public async Task AnnualizationWmmTest(Dataset dataset, Annualization annualization)
        {
            var wmmNonAnnualyzedNewMnemonicsQuarterly = "ALBS1XNET1H.Q";
            var wmmNonAnnualyzedNewMnemonicsMonthly = "ARGS1GCSAV1H.M";
            var wmmNonAnnualyzedNewMnemonicsAnnualShouldNotCalculate = "ARGS1CGVRPY.A";

            var wmmAnnualyzedNewMnemonicsQuarterly = "AFGN1BPTFGV$.Q";
            var wmmAnnualyzedNewMnemonicsAnnualShouldNotCalculate = "AFGN1GDPCO.A";

            List<string> mnemonicsValueList;

            if (annualization == Annualization.NonAnnualized)
            {
                mnemonicsValueList = new List<string>() { wmmNonAnnualyzedNewMnemonicsQuarterly, wmmNonAnnualyzedNewMnemonicsMonthly, wmmNonAnnualyzedNewMnemonicsAnnualShouldNotCalculate };
            }
            else
            {
                mnemonicsValueList = new List<string>() { wmmAnnualyzedNewMnemonicsQuarterly, wmmAnnualyzedNewMnemonicsAnnualShouldNotCalculate };
            }

            List<string> calculationsNotPerformedList = new List<string>() { wmmNonAnnualyzedNewMnemonicsAnnualShouldNotCalculate, wmmAnnualyzedNewMnemonicsAnnualShouldNotCalculate };

            await _buildQueryModule.ResetSettingsInGrid(dataset);
            TaxonomyFilters taxonomyFilters = null;
            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            foreach (var mnemonicsValue in mnemonicsValueList)
            {
                var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

                List<AppliedFunction> appliedFunctions = new List<AppliedFunction>()
                {
                    new AppliedFunction
                    {
                        FunctionId = (int)Function.Annualization,
                        Parameter = "Original"
                    }
                }.ToList();

                await _buildQueryModule.AddFunctionsToQuery(dataset, _datasetSettingsModule, displaySettings, appliedFunctions);

                displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

                var frequency = GetRequiredFrequency(mnemonicsValue);

                var series = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: mnemonicsValue);
                var seriesWithDefiniteNewMnemonics = series.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonicsValue)).Any());

                var wmmAnnualizedColumnValues = _buildQueryModule.GetGridValuesForParticularColumn(series, "WmmAnnualized").First();
                wmmAnnualizedColumnValues.Should().Be("Original");

                var seriesRecalculated = AnnualizationCalculationAdd(seriesWithDefiniteNewMnemonics, annualization);

                appliedFunctions = new List<AppliedFunction>()
                {
                    new AppliedFunction
                    {
                        FunctionId = (int)Function.Annualization,
                        Parameter = annualization.ToString()
                    }
                }.ToList();

                await _buildQueryModule.AddFunctionsToQuery(dataset, _datasetSettingsModule, displaySettings, appliedFunctions);

                var displaySettingsAfterChange = await _datasetSettingsModule.GetDataSetSettings(dataset);
                series = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettingsAfterChange, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: mnemonicsValue);
                seriesWithDefiniteNewMnemonics = series.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonicsValue)).Any());

                VerifyWmmCalculationsResults(seriesRecalculated, seriesWithDefiniteNewMnemonics);
                if (calculationsNotPerformedList.Contains(mnemonicsValue))
                {
                    wmmAnnualizedColumnValues = _buildQueryModule.GetGridValuesForParticularColumn(series, "WmmAnnualized").First();
                    wmmAnnualizedColumnValues.Should().Be("Original");
                }
                else
                {
                    wmmAnnualizedColumnValues = _buildQueryModule.GetGridValuesForParticularColumn(series, "WmmAnnualized").First();
                    wmmAnnualizedColumnValues.Should().Be(annualization.ToDisplayName());
                }
            }
            await _buildQueryModule.ResetSettingsInGrid(dataset);
        }

        [TestCase(Dataset.WorldMarketMonitor)]
        [TestCase(Dataset.GlobalEconomyNew)]
        [TestCase(Dataset.Banking)]
        public async Task CurrencyConversionNominalWmmTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);
            await _buildQueryModule.ResetSettingsInTaxonomyFilters(dataset, _datasetSettingsModule);

            var mnemonicsPreparedLists = CurrencyConversionNominalDataPreparation(dataset);

            await CurrencyConversionCommonPart(dataset, mnemonicsPreparedLists, CurrencyConversionSeriesType.Nominal);
        }

        [TestCase(Dataset.WorldMarketMonitor)]
        [TestCase(Dataset.GlobalEconomyNew)]
        public async Task CurrencyConversionRealWmmTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);
            await _buildQueryModule.ResetSettingsInTaxonomyFilters(dataset, _datasetSettingsModule);

            var mnemonicsPreparedLists = CurrencyConversionRealDataPreparation(dataset);

            await CurrencyConversionCommonPart(dataset, mnemonicsPreparedLists, CurrencyConversionSeriesType.Real);
        }

        [TestCase(Dataset.WorldMarketMonitor)]
        [TestCase(Dataset.GlobalEconomyNew)]
        public async Task CurrencyConversionExchangeRateWmmTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);
            await _buildQueryModule.ResetSettingsInTaxonomyFilters(dataset, _datasetSettingsModule);

            var mnemonicsPreparedLists = CurrencyConversionExchangeRateDataPreparation(dataset);

            await CurrencyConversionCommonPart(dataset, mnemonicsPreparedLists, CurrencyConversionSeriesType.ExchangeRate);
        }

        [TestCase(Dataset.WorldMarketMonitor)]
        [TestCase(Dataset.GlobalEconomyNew)]
        public async Task RebaseIndicesWmmTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);
            await _buildQueryModule.ResetSettingsInTaxonomyFilters(dataset, _datasetSettingsModule);

            TaxonomyFilters taxonomyFilters = null;
            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            List<string> mnemonicsList;
            if (dataset == Dataset.WorldMarketMonitor)
            {
                mnemonicsList = new List<string>() { "AFGS1CPI1H.A", "AFGN1CPIEOP.Q", "AFGS1CPI1H.M" };
            }
            else
            {
                mnemonicsList = new List<string>() { "JRX_LKA.A", "JSRT_GRC.Q", "CPI_KAZ.M" };
            }

            var frequenciesList = new List<Frequency>() { Frequency.Annual, Frequency.Quarterly, Frequency.Monthly };

            foreach (var mnemonics in mnemonicsList)
            {
                foreach (var baseFrequency in frequenciesList)
                {
                    var expectedDateRange = DateRangesTestData.GetDateRange(UserDateRange.WmmTestCustomDateForStartAndEnd);

                    var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
                    displaySettings.ChangeUserSettings()
                        .ChangeDateRange(expectedDateRange);

                    var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
                    var expectedResponse = new UpdateFiltersResponse { Success = true };
                    updateResult.Should().BeEquivalentTo(expectedResponse);

                    var appliedFunctions = new List<AppliedFunction>()
                    {
                        new AppliedFunction
                        {
                            FunctionId = (int)Function.Rescaling,
                            Parameter = "Original"
                        }
                    }.ToList();

                    await _buildQueryModule.AddFunctionsToQuery(dataset, _datasetSettingsModule, displaySettings, appliedFunctions);

                    displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

                    var frequency = GetRequiredFrequency(mnemonics);

                    var series = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: mnemonics);
                    var seriesWithDefiniteNewMnemonics = series.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonics)).Any());

                    var indicesBaseDate = "2005-01-01T09:00:00.000Z";
                    var indicesBaseFrequency = baseFrequency;
                    var indicesBaseValue = 100;
                    var realValuesBaseFrequency = Frequency.Annual;
                    var rebasingSettings = new RebasingSettings
                    {
                        IndicesBaseDate = indicesBaseDate,
                        IndicesBaseFrequency = GetRebasingBaseFromFrequency(indicesBaseFrequency),
                        IndicesBaseValue = indicesBaseValue,
                        RebaseIndices = true,
                        RealValuesBaseDate = "",
                        RealValuesBaseFrequency = GetRebasingBaseFromFrequency(realValuesBaseFrequency),
                        RealValuesBaseValue = 0,
                        RebaseRealValues = false
                    };

                    displaySettings.UserSettings.AppliedFunctions.Rebasing = rebasingSettings;

                    updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
                    expectedResponse = new UpdateFiltersResponse { Success = true };
                    updateResult.Should().BeEquivalentTo(expectedResponse);


                    var seriesFromServer = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: mnemonics);
                    var seriesFromServerWithDefiniteNewMnemonics = seriesFromServer.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonics)).Any());

                    Series seriesConverted;
                    if (IsRebasingShouldBeCalculated(displaySettings, seriesWithDefiniteNewMnemonics, true))
                    {
                        seriesConverted = IndicesRebasingCalculationAdd(seriesWithDefiniteNewMnemonics, indicesBaseFrequency, DateTime.Parse(indicesBaseDate).Date, indicesBaseValue);
                    }
                    else
                    {
                        seriesConverted = seriesWithDefiniteNewMnemonics;
                    }
                    VerifyWmmCalculationsResults(seriesConverted, seriesFromServerWithDefiniteNewMnemonics, baseFrequency);
                    CheckBasePeriodCorrect(seriesFromServerWithDefiniteNewMnemonics, displaySettings, baseFrequency, DateTime.Parse(indicesBaseDate).Date, true);
                }
            }

            await _buildQueryModule.ResetSettingsInGrid(dataset);
            await _buildQueryModule.ResetSettingsInTaxonomyFilters(dataset, _datasetSettingsModule);
        }

        [TestCase(Dataset.WorldMarketMonitor)]
        [TestCase(Dataset.GlobalEconomyNew)]
        public async Task RebaseRealValuesWmmTest(Dataset dataset)
        {
            await _buildQueryModule.ResetSettingsInGrid(dataset);
            await _buildQueryModule.ResetSettingsInTaxonomyFilters(dataset, _datasetSettingsModule);

            TaxonomyFilters taxonomyFilters = null;
            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            var frequenciesList = new List<Frequency>() { Frequency.Annual, Frequency.Quarterly };

            foreach (var seriesFrequency in frequenciesList)
            {
                foreach (var baseFrequency in frequenciesList)
                {
                    var mnemonicsPreparedLists = RealValuesRebasingDataPreparation(dataset, baseFrequency, seriesFrequency);

                    var expectedDateRange = DateRangesTestData.GetDateRange(UserDateRange.WmmTestCustomDateForStartAndEnd);

                    var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
                    displaySettings.ChangeUserSettings()
                        .ChangeDateRange(expectedDateRange);

                    var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
                    var expectedResponse = new UpdateFiltersResponse { Success = true };
                    updateResult.Should().BeEquivalentTo(expectedResponse);

                    var appliedFunctions = new List<AppliedFunction>()
                    {
                        new AppliedFunction
                        {
                            FunctionId = (int)Function.Rescaling,
                            Parameter = "Original"
                        }
                    }.ToList();

                    await _buildQueryModule.AddFunctionsToQuery(dataset, _datasetSettingsModule, displaySettings, appliedFunctions);

                    displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

                    var series = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: seriesFrequency.ToString(), keyword: mnemonicsPreparedLists[0]);
                    var seriesWithDefiniteNewMnemonics = series.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonicsPreparedLists[0])).Any());

                    var nominalSeries = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: seriesFrequency.ToString(), keyword: mnemonicsPreparedLists[1]);
                    var targetBasePeriodSeries = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: seriesFrequency.ToString(), keyword: mnemonicsPreparedLists[2]);

                    Series nominalSeriesWithDefiniteNewMnemonics;
                    Series targetBasePeriodSeriesWithDefiniteNewMnemonics;
                    if (!((seriesFrequency == Frequency.Annual) && (baseFrequency == Frequency.Quarterly)))
                    {
                        nominalSeriesWithDefiniteNewMnemonics = nominalSeries.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonicsPreparedLists[1])).Any());
                        targetBasePeriodSeriesWithDefiniteNewMnemonics = targetBasePeriodSeries.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonicsPreparedLists[2])).Any());
                    }
                    else
                    {
                        nominalSeriesWithDefiniteNewMnemonics = nominalSeries.First();
                        targetBasePeriodSeriesWithDefiniteNewMnemonics = targetBasePeriodSeries.First();
                    }

                    var indicesBaseFrequency = Frequency.Annual;
                    var indicesBaseValue = 100;
                    var realValuesBaseDate = "2005-01-01T10:00:00.000Z";
                    var realValuesBaseFrequency = baseFrequency;
                    var rebasingSettings = new RebasingSettings
                    {
                        IndicesBaseDate = null,
                        IndicesBaseFrequency = GetRebasingBaseFromFrequency(indicesBaseFrequency),
                        IndicesBaseValue = indicesBaseValue,
                        RebaseIndices = false,
                        RealValuesBaseDate = realValuesBaseDate,
                        RealValuesBaseFrequency = GetRebasingBaseFromFrequency(realValuesBaseFrequency),
                        RealValuesBaseValue = 0,
                        RebaseRealValues = true
                    };

                    displaySettings.UserSettings.AppliedFunctions.Rebasing = rebasingSettings;

                    updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
                    expectedResponse = new UpdateFiltersResponse { Success = true };
                    updateResult.Should().BeEquivalentTo(expectedResponse);


                    var seriesFromServer = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: seriesFrequency.ToString(), keyword: mnemonicsPreparedLists[0]);
                    var seriesFromServerWithDefiniteNewMnemonics = seriesFromServer.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonicsPreparedLists[0])).Any());

                    Series seriesConverted;
                    if (IsRebasingShouldBeCalculated(displaySettings, seriesWithDefiniteNewMnemonics, false))
                    {
                        seriesConverted = RealValuesRebasingCalculationAdd(seriesWithDefiniteNewMnemonics, nominalSeriesWithDefiniteNewMnemonics, targetBasePeriodSeriesWithDefiniteNewMnemonics, realValuesBaseFrequency, DateTime.Parse(realValuesBaseDate).Date);
                    }
                    else
                    {
                        seriesConverted = seriesWithDefiniteNewMnemonics;
                    }
                    VerifyWmmCalculationsResults(seriesConverted, seriesFromServerWithDefiniteNewMnemonics, baseFrequency);
                    CheckBasePeriodCorrect(seriesFromServerWithDefiniteNewMnemonics, displaySettings, baseFrequency, DateTime.Parse(realValuesBaseDate).Date, false);
                }
            }

            await _buildQueryModule.ResetSettingsInGrid(dataset);
            await _buildQueryModule.ResetSettingsInTaxonomyFilters(dataset, _datasetSettingsModule);
        }

        [TestCase(Dataset.WorldMarketMonitor)]
        public async Task WmmSequenceCalculationsTest(Dataset dataset)
        {
            var seriesToBeCalculatedMnemonics = "ARGN1IFR.Q";
            var usdExchangeRateMnemonicRX = "ARGN1RX.Q";
            var cambodianRielExchangeRateMnemonicRX = "KHMN1RX.Q";
            var nominalMnemonics = "ARGN1IF.Q";
            var targetBasePeriodSeriesMnemonics = "ARGN1IFR.Q";

            await _buildQueryModule.ResetSettingsInGrid(dataset);
            await _buildQueryModule.ResetSettingsInTaxonomyFilters(dataset, _datasetSettingsModule);

            TaxonomyFilters taxonomyFilters = null;
            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            var frequency = GetRequiredFrequency(seriesToBeCalculatedMnemonics);
            var baseFrequency = Frequency.Quarterly;
            var baseDate = "2005-01-01T10:00:00.000Z";
            var annualization = Annualization.NonAnnualized;


            var displaySettings = await SetSequenceCalculationInitialParameters(dataset);

            var series = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: seriesToBeCalculatedMnemonics);
            var seriesWithDefiniteNewMnemonics = series.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(seriesToBeCalculatedMnemonics)).Any());

            var usdExchangeRateSeries = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: usdExchangeRateMnemonicRX);
            var usdExchangeRateSeriesWithDefiniteNewMnemonics = usdExchangeRateSeries.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(usdExchangeRateMnemonicRX)).Any());

            var cambodianRielExchangeRateSeries = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: cambodianRielExchangeRateMnemonicRX);
            var cambodianRielExchangeRateSeriesWithDefiniteNewMnemonics = cambodianRielExchangeRateSeries?.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(cambodianRielExchangeRateMnemonicRX)).Any());

            var nominalSeries = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: nominalMnemonics);
            var nominalSeriesWithDefiniteNewMnemonics = nominalSeries.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(nominalMnemonics)).Any());

            var targetBasePeriodSeries = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: targetBasePeriodSeriesMnemonics);
            var targetBasePeriodSeriesWithDefiniteNewMnemonics = targetBasePeriodSeries.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(targetBasePeriodSeriesMnemonics)).Any());


            var currencyConversionSeriesType = CurrencyConversionSeriesType.Real;
            var expectedFunctionColumn = "";
            var expectedUnitColumn = SetInitExpectedUnitColumn(currencyConversionSeriesType, dataset);
            var functionColumnValue = seriesWithDefiniteNewMnemonics.Function;
            functionColumnValue.Should().Be(expectedFunctionColumn);
            var unitColumnValue = _buildQueryModule.GetGridValuesForParticularColumn(series, "unit").First();
            unitColumnValue.Should().Be(expectedUnitColumn);

            var scaleColumnValues = _buildQueryModule.GetGridValuesForParticularColumn(series, "scale").First();
            var wmmDefaultTargetScaleColumnValues = _buildQueryModule.GetGridValuesForParticularColumn(series, "WmmDefaultTargetScale").First();
            scaleColumnValues.Should().NotBe(wmmDefaultTargetScaleColumnValues);

            var wmmAnnualizedColumnValues = _buildQueryModule.GetGridValuesForParticularColumn(series, "WmmAnnualized").First();
            wmmAnnualizedColumnValues.Should().Be("Original");


            displaySettings = await AddAllFunctionsToWmmSequence(displaySettings, dataset, baseFrequency, baseDate, annualization);


            var seriesConverted = RescaleCalculationAdd(seriesWithDefiniteNewMnemonics);
            seriesConverted = RealValuesRebasingCalculationAdd(seriesConverted, nominalSeriesWithDefiniteNewMnemonics, targetBasePeriodSeriesWithDefiniteNewMnemonics, baseFrequency, DateTime.Parse(baseDate).Date);
            seriesConverted = AddCorrectCurrencyCalculation(currencyConversionSeriesType, seriesConverted, usdExchangeRateSeriesWithDefiniteNewMnemonics, cambodianRielExchangeRateSeriesWithDefiniteNewMnemonics, true, DateTime.Parse(baseDate).Date);
            seriesConverted = AnnualizationCalculationAdd(seriesConverted, annualization);


            var seriesFromServer = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: seriesToBeCalculatedMnemonics);
            var seriesFromServerWithDefiniteNewMnemonics = seriesFromServer.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(seriesToBeCalculatedMnemonics)).Any());

            VerifyWmmCalculationsResults(seriesConverted, seriesFromServerWithDefiniteNewMnemonics);


            scaleColumnValues = _buildQueryModule.GetGridValuesForParticularColumn(seriesFromServer, "scale").First();
            wmmDefaultTargetScaleColumnValues = _buildQueryModule.GetGridValuesForParticularColumn(seriesFromServer, "WmmDefaultTargetScale").First();
            scaleColumnValues.Should().Be(wmmDefaultTargetScaleColumnValues);


            expectedFunctionColumn = "Convert(to Cambodian Riel)";
            functionColumnValue = seriesFromServerWithDefiniteNewMnemonics.Function;
            functionColumnValue.Should().Be(expectedFunctionColumn);

            expectedUnitColumn = "Cambodian Riel";
            unitColumnValue = _buildQueryModule.GetGridValuesForParticularColumn(seriesFromServer, "unit").First();
            unitColumnValue.Should().Be(expectedUnitColumn);

            CheckBasePeriodCorrect(seriesFromServerWithDefiniteNewMnemonics, displaySettings, baseFrequency, DateTime.Parse(baseDate).Date, false);

            wmmAnnualizedColumnValues = _buildQueryModule.GetGridValuesForParticularColumn(seriesFromServer, "WmmAnnualized").First();
            wmmAnnualizedColumnValues.Should().Be(annualization.ToDisplayName());


            await _buildQueryModule.ResetSettingsInGrid(dataset);
            await _buildQueryModule.ResetSettingsInTaxonomyFilters(dataset, _datasetSettingsModule);
        }

        public async Task<DataSetSettings> SetSequenceCalculationInitialParameters(Dataset dataset)
        {
            var expectedDateRange = DateRangesTestData.GetDateRange(UserDateRange.WmmTestCustomDateForStartAndEnd);

            var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
            displaySettings.ChangeUserSettings()
                .ChangeDateRange(expectedDateRange);

            var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            var appliedFunctions = new List<AppliedFunction>()
                    {
                        new AppliedFunction
                        {
                            FunctionId = (int)Function.Rescaling,
                            Parameter = "Original"
                        }
                    }.ToList();

            await _buildQueryModule.AddFunctionsToQuery(dataset, _datasetSettingsModule, displaySettings, appliedFunctions);

            displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
            return displaySettings;
        }

        public async Task<DataSetSettings> AddAllFunctionsToWmmSequence(DataSetSettings displaySettings, Dataset dataset, Frequency baseFrequency, string baseDate, Annualization annualization)
        {
            var appliedFunctions = new List<AppliedFunction>()
            {
                new AppliedFunction
                {
                    FunctionId = (int)Function.Rescaling,
                    Parameter = "Harmonized"
                },
                new AppliedFunction
                {
                    FunctionId = (int)Function.CurrencyConversion,
                    Parameter = "KHR"
                },
                new AppliedFunction
                {
                    FunctionId = (int)Function.Annualization,
                    Parameter = annualization.ToString()
                }
            }.ToList();

            await _buildQueryModule.AddFunctionsToQuery(dataset, _datasetSettingsModule, displaySettings, appliedFunctions);
            displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

            var indicesBaseFrequency = Frequency.Annual;
            var indicesBaseValue = 100;
            var realValuesBaseDate = baseDate;
            var realValuesBaseFrequency = baseFrequency;
            var rebasingSettings = new RebasingSettings
            {
                IndicesBaseDate = null,
                IndicesBaseFrequency = GetRebasingBaseFromFrequency(indicesBaseFrequency),
                IndicesBaseValue = indicesBaseValue,
                RebaseIndices = false,
                RealValuesBaseDate = realValuesBaseDate,
                RealValuesBaseFrequency = GetRebasingBaseFromFrequency(realValuesBaseFrequency),
                RealValuesBaseValue = 0,
                RebaseRealValues = true
            };

            displaySettings.UserSettings.AppliedFunctions.Rebasing = rebasingSettings;

            var updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
            var expectedResponse = new UpdateFiltersResponse { Success = true };
            updateResult.Should().BeEquivalentTo(expectedResponse);

            return displaySettings;
        }

        public Series RescaleCalculationAdd(Series series)
        {
            List<Series> seriesList = new List<Series>() { series };
            var scaleColumnValue = _buildQueryModule.GetGridValuesForParticularColumn(seriesList, "scale");
            var scale = GetScaleForCalculation(scaleColumnValue.First());
            var default_target_scaleColumnValue = _buildQueryModule.GetGridValuesForParticularColumn(seriesList, "default_target_scale");
            var default_target_scale = GetScaleForCalculation(default_target_scaleColumnValue.First());
            var divider = default_target_scale / scale;
            series.Values.All(c => { c.Value /= divider; return true; });

            return series;
        }

        public Series AnnualizationCalculationAdd(Series series, Annualization annualization)
        {
            List<Series> seriesList = new List<Series>() { series };
            var annualizationColumnValue = _buildQueryModule.GetGridValuesForParticularColumn(seriesList, "frequency");

            if (annualization == Annualization.Annualized && series.Frequency == Frequency.Quarterly)
            {
                series.Values.All(c => { c.Value *= 4; return true; });
                series.Attributes["WmmAnnualized"].All(c => { c.Value = annualization.ToDisplayName(); return true; });
            }

            if (annualization == Annualization.NonAnnualized && series.Frequency == Frequency.Quarterly)
            {
                series.Values.All(c => { c.Value /= 4; return true; });
                series.Attributes["WmmAnnualized"].All(c => { c.Value = annualization.ToDisplayName(); return true; });
            }
            if (annualization == Annualization.NonAnnualized && series.Frequency == Frequency.Monthly)
            {
                series.Values.All(c => { c.Value /= 12; return true; });
                series.Attributes["WmmAnnualized"].All(c => { c.Value = annualization.ToDisplayName(); return true; });
            }

            return series;
        }

        private async Task CurrencyConversionCommonPart(Dataset dataset, List<List<string>> mnemonicsPreparedLists, CurrencyConversionSeriesType currencyConversionSeriesType)
        {
            TaxonomyFilters taxonomyFilters = null;
            Dictionary<string, List<string>> taxonomyFiltersWithItems = null;

            foreach (var mnemonicsList in mnemonicsPreparedLists)
            {
                Dataset datasetForAdditionalMnemonics;
                DataSetSettings displaySettingsAdditionalMnemonics;
                UpdateFiltersResponse updateResult;
                UpdateFiltersResponse expectedResponse;
                var expectedDateRange = DateRangesTestData.GetDateRange(UserDateRange.WmmTestCustomDateForStartAndEnd);
                if (dataset == Dataset.Banking)
                {
                    datasetForAdditionalMnemonics = Dataset.GlobalEconomyNew;
                    await _buildQueryModule.ResetSettingsInTaxonomyFilters(dataset, _datasetSettingsModule);
                    displaySettingsAdditionalMnemonics = await _datasetSettingsModule.GetDataSetSettings(datasetForAdditionalMnemonics);
                    displaySettingsAdditionalMnemonics.ChangeUserSettings()
                        .ChangeDateRange(expectedDateRange);

                    updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettingsAdditionalMnemonics);
                    expectedResponse = new UpdateFiltersResponse { Success = true };
                    updateResult.Should().BeEquivalentTo(expectedResponse);
                }
                else
                {
                    datasetForAdditionalMnemonics = dataset;
                }


                var displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
                displaySettings.ChangeUserSettings()
                    .ChangeDateRange(expectedDateRange);

                updateResult = await _datasetSettingsModule.UpdateDatasetSettings(displaySettings);
                expectedResponse = new UpdateFiltersResponse { Success = true };
                updateResult.Should().BeEquivalentTo(expectedResponse);

                var appliedFunctions = new List<AppliedFunction>()
                    {
                        new AppliedFunction
                        {
                            FunctionId = (int)Function.Rescaling,
                            Parameter = "Original"
                        }
                    }.ToList();

                await _buildQueryModule.AddFunctionsToQuery(datasetForAdditionalMnemonics, _datasetSettingsModule, displaySettings, appliedFunctions);

                displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);
                displaySettingsAdditionalMnemonics = await _datasetSettingsModule.GetDataSetSettings(datasetForAdditionalMnemonics);

                var frequency = GetRequiredFrequency(mnemonicsList[0]);

                var series = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: mnemonicsList[0]);
                var seriesWithDefiniteNewMnemonics = series.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonicsList[0])).Any());

                Series usdExchangeRateSeriesWithDefiniteNewMnemonics;
                if (currencyConversionSeriesType != CurrencyConversionSeriesType.ExchangeRate)
                {
                    var usdExchangeRateSeries = await _buildQueryModule.GetSearchResultsSeriesFromGrid(datasetForAdditionalMnemonics, displaySettingsAdditionalMnemonics, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: mnemonicsList[1]);
                    usdExchangeRateSeriesWithDefiniteNewMnemonics = usdExchangeRateSeries.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonicsList[1])).Any());
                }
                else
                {
                    usdExchangeRateSeriesWithDefiniteNewMnemonics = null;
                }

                var cambodianRielExchangeRateSeries = await _buildQueryModule.GetSearchResultsSeriesFromGrid(datasetForAdditionalMnemonics, displaySettingsAdditionalMnemonics, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: mnemonicsList[2]);
                var cambodianRielExchangeRateSeriesWithDefiniteNewMnemonics = cambodianRielExchangeRateSeries?.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonicsList[2])).Any());


                var expectedFunctionColumn = "";
                var expectedUnitColumn = SetInitExpectedUnitColumn(currencyConversionSeriesType, dataset);
                var functionColumnValue = seriesWithDefiniteNewMnemonics.Function;
                functionColumnValue.Should().Be(expectedFunctionColumn);
                var unitColumnValue = _buildQueryModule.GetGridValuesForParticularColumn(series, "unit").First();
                unitColumnValue.Should().Be(expectedUnitColumn);

                var seriesConverted = AddCorrectCurrencyCalculation(currencyConversionSeriesType, seriesWithDefiniteNewMnemonics, usdExchangeRateSeriesWithDefiniteNewMnemonics, cambodianRielExchangeRateSeriesWithDefiniteNewMnemonics);

                if (dataset != Dataset.Banking)
                {
                    appliedFunctions = new List<AppliedFunction>()
                    {
                        new AppliedFunction
                        {
                            FunctionId = (int)Function.CurrencyConversion,
                            Parameter = "KHR"
                        },
                        new AppliedFunction
                        {
                            FunctionId = (int)Function.Rescaling,
                            Parameter = "Original"
                        }
                    }.ToList();
                }
                else
                {
                    appliedFunctions = new List<AppliedFunction>()
                    {
                        new AppliedFunction
                        {
                            FunctionId = (int)Function.CurrencyConversion,
                            Parameter = "KHR"
                        }
                    }.ToList();
                }
                await _buildQueryModule.AddFunctionsToQuery(dataset, _datasetSettingsModule, displaySettings, appliedFunctions);

                displaySettings = await _datasetSettingsModule.GetDataSetSettings(dataset);

                var seriesFromServer = await _buildQueryModule.GetSearchResultsSeriesFromGrid(dataset, displaySettings, taxonomyFilters, taxonomyFiltersWithItems, frequency: frequency, keyword: mnemonicsList[0]);
                var seriesFromServerWithDefiniteNewMnemonics = seriesFromServer.Single(x => x.Attributes["default_mnemonic"].Where(x => x.Value.Contains(mnemonicsList[0])).Any());

                VerifyWmmCalculationsResults(seriesConverted, seriesFromServerWithDefiniteNewMnemonics);

                if (currencyConversionSeriesType != CurrencyConversionSeriesType.ExchangeRate)
                {
                    expectedFunctionColumn = "Convert(to Cambodian Riel)";
                    functionColumnValue = seriesFromServerWithDefiniteNewMnemonics.Function;
                    functionColumnValue.Should().Be(expectedFunctionColumn);
                }
                expectedUnitColumn = "Cambodian Riel";
                unitColumnValue = _buildQueryModule.GetGridValuesForParticularColumn(seriesFromServer, "unit").First();
                unitColumnValue.Should().Be(expectedUnitColumn);

                await _buildQueryModule.ResetSettingsInGrid(dataset);
                await _buildQueryModule.ResetSettingsInTaxonomyFilters(dataset, _datasetSettingsModule);
            }
        }

        private Series AddCorrectCurrencyCalculation(CurrencyConversionSeriesType currencyConversionSeriesType, Series seriesWithDefiniteNewMnemonics
            , Series usdExchangeRateSeriesWithDefiniteNewMnemonics, Series cambodianRielExchangeRateSeriesWithDefiniteNewMnemonics, bool isRebasingApplied = false, DateTime basePeriod = new DateTime())
        {
            switch (currencyConversionSeriesType)
            {
                case CurrencyConversionSeriesType.Nominal:
                    {
                        var seriesConverted = NominalCurrencyCalculationAdd(seriesWithDefiniteNewMnemonics, usdExchangeRateSeriesWithDefiniteNewMnemonics, cambodianRielExchangeRateSeriesWithDefiniteNewMnemonics);
                        return seriesConverted;
                    }
                case CurrencyConversionSeriesType.Real:
                    {
                        DateTime basePeriodDate;
                        if (isRebasingApplied)
                        {
                            basePeriodDate = basePeriod.Date;
                        }
                        else
                        {
                            var basePeriodSeriesInternal = seriesWithDefiniteNewMnemonics.Attributes["WmmBasePeriodStart"].First().Value;
                            basePeriodDate = Convert.ToDateTime(basePeriodSeriesInternal);
                        }
                        var seriesConverted = RealCurrencyCalculationAdd(seriesWithDefiniteNewMnemonics, usdExchangeRateSeriesWithDefiniteNewMnemonics, cambodianRielExchangeRateSeriesWithDefiniteNewMnemonics, basePeriodDate);
                        return seriesConverted;
                    }
                case CurrencyConversionSeriesType.ExchangeRate:
                    {
                        var seriesConverted = ExchangeRateCalculationAdd(seriesWithDefiniteNewMnemonics, cambodianRielExchangeRateSeriesWithDefiniteNewMnemonics);
                        return seriesConverted;
                    }
                default:
                    throw new Exception("Incorrect series type for Currency conversion!");
            }
        }

        private string SetInitExpectedUnitColumn(CurrencyConversionSeriesType currencyConversionSeriesType, Dataset dataset)
        {
            switch (currencyConversionSeriesType)
            {
                case CurrencyConversionSeriesType.Nominal:
                    {
                        if (dataset != Dataset.Banking)
                        {
                            return "Afghanistan Afghani";
                        }
                        else
                        {
                            return "Billion euros";
                        }
                    }
                case CurrencyConversionSeriesType.Real:
                    {
                        return "Argentine Peso";
                    }
                case CurrencyConversionSeriesType.ExchangeRate:
                    {
                        return "Afghanistan Afghani";
                    }
                default:
                    throw new Exception("Incorrect series type for Currency conversion!");
            }
        }

        private List<List<string>> CurrencyConversionNominalDataPreparation(Dataset dataset)
        {
            List<List<string>> MnemonicsPrepared = new List<List<string>>();

            switch (dataset)
            {
                case Dataset.WorldMarketMonitor:
                    {
                        var seriesToBeCalculatedNominalRX = "AFGN1GDPCO.A";
                        var usdExchangeRateMnemonicRX = "AFGN1RX.A";
                        var cambodianRielExchangeRateMnemonicRX = "KHMN1RX.A";

                        var seriesToBeCalculatedNominalRXEnd = "AFGN1MQ.A";
                        var usdExchangeRateMnemonicRXEnd = "AFGN1RXEND.A";
                        var cambodianRielExchangeRateMnemonicRXEnd = "KHMN1RXEND.A";

                        List<string> RXMnemonics = new List<string>() { seriesToBeCalculatedNominalRX, usdExchangeRateMnemonicRX, cambodianRielExchangeRateMnemonicRX };
                        List<string> RXEndMnemonics = new List<string>() { seriesToBeCalculatedNominalRXEnd, usdExchangeRateMnemonicRXEnd, cambodianRielExchangeRateMnemonicRXEnd };

                        MnemonicsPrepared.Add(RXMnemonics);
                        MnemonicsPrepared.Add(RXEndMnemonics);

                        return MnemonicsPrepared;
                    }
                case Dataset.GlobalEconomyNew:
                    {
                        var seriesToBeCalculatedNominalRX = "CT_AFG.A";
                        var usdExchangeRateMnemonicRX = "RX_AFG.A";
                        var cambodianRielExchangeRateMnemonicRX = "RX_KHM.A";

                        var seriesToBeCalculatedNominalRXEnd = "DBTCGF_AFG.A";
                        var usdExchangeRateMnemonicRXEnd = "RXEND_AFG.A";
                        var cambodianRielExchangeRateMnemonicRXEnd = "RXEND_KHM.A";

                        List<string> RXMnemonics = new List<string>() { seriesToBeCalculatedNominalRX, usdExchangeRateMnemonicRX, cambodianRielExchangeRateMnemonicRX };
                        List<string> RXEndMnemonics = new List<string>() { seriesToBeCalculatedNominalRXEnd, usdExchangeRateMnemonicRXEnd, cambodianRielExchangeRateMnemonicRXEnd };

                        MnemonicsPrepared.Add(RXMnemonics);
                        MnemonicsPrepared.Add(RXEndMnemonics);

                        return MnemonicsPrepared;
                    }
                case Dataset.Banking:
                    {
                        var seriesToBeCalculatedNominalRX = "ISPTP_ESP.A";
                        var usdExchangeRateMnemonicRX = "RX_ESP.A";
                        var cambodianRielExchangeRateMnemonicRX = "RX_KHM.A";

                        var seriesToBeCalculatedNominalRXEnd = "FSLLT_ESP.A";
                        var usdExchangeRateMnemonicRXEnd = "RXEND_ESP.A";
                        var cambodianRielExchangeRateMnemonicRXEnd = "RXEND_KHM.A";

                        List<string> RXMnemonics = new List<string>() { seriesToBeCalculatedNominalRX, usdExchangeRateMnemonicRX, cambodianRielExchangeRateMnemonicRX };
                        List<string> RXEndMnemonics = new List<string>() { seriesToBeCalculatedNominalRXEnd, usdExchangeRateMnemonicRXEnd, cambodianRielExchangeRateMnemonicRXEnd };

                        MnemonicsPrepared.Add(RXMnemonics);
                        MnemonicsPrepared.Add(RXEndMnemonics);

                        return MnemonicsPrepared;
                    }
                default:
                    throw new Exception($"Incorrect dataset chosen = {dataset}! Choose correct one!");
            }
        }

        private List<List<string>> CurrencyConversionRealDataPreparation(Dataset dataset)
        {
            List<List<string>> MnemonicsPrepared = new List<List<string>>();

            switch (dataset)
            {
                case Dataset.WorldMarketMonitor:
                    {
                        var seriesToBeCalculatedRealRX = "ARGN1GDPINXCOR.A";
                        var usdExchangeRateMnemonicRX = "ARGN1RX.A";
                        var cambodianRielExchangeRateMnemonicRX = "KHMN1RX.A";

                        List<string> RXMnemonics = new List<string>() { seriesToBeCalculatedRealRX, usdExchangeRateMnemonicRX, cambodianRielExchangeRateMnemonicRX };

                        MnemonicsPrepared.Add(RXMnemonics);

                        return MnemonicsPrepared;
                    }
                case Dataset.GlobalEconomyNew:
                    {
                        var seriesToBeCalculatedNominalRX = "GDDR_ARG.A";
                        var usdExchangeRateMnemonicRX = "RX_ARG.A";
                        var cambodianRielExchangeRateMnemonicRX = "RX_KHM.A";

                        List<string> RXMnemonics = new List<string>() { seriesToBeCalculatedNominalRX, usdExchangeRateMnemonicRX, cambodianRielExchangeRateMnemonicRX };

                        MnemonicsPrepared.Add(RXMnemonics);

                        return MnemonicsPrepared;
                    }
                default:
                    throw new Exception($"Incorrect dataset chosen = {dataset}! Choose correct one!");
            }
        }

        private List<List<string>> CurrencyConversionExchangeRateDataPreparation(Dataset dataset)
        {
            List<List<string>> MnemonicsPrepared = new List<List<string>>();

            switch (dataset)
            {
                case Dataset.WorldMarketMonitor:
                    {
                        var seriesToBeCalculatedNominalRX = "AFGN2RX.A";
                        var usdExchangeRateMnemonicRX = "";
                        var cambodianRielExchangeRateMnemonicRX = "KHMN1RX.A";

                        var seriesToBeCalculatedNominalRXEnd = "AFGN1RXEUROEND.A";
                        var usdExchangeRateMnemonicRXEnd = "";
                        var cambodianRielExchangeRateMnemonicRXEnd = "KHMN1RXEND.A";

                        List<string> RXMnemonics = new List<string>() { seriesToBeCalculatedNominalRX, usdExchangeRateMnemonicRX, cambodianRielExchangeRateMnemonicRX };
                        List<string> RXEndMnemonics = new List<string>() { seriesToBeCalculatedNominalRXEnd, usdExchangeRateMnemonicRXEnd, cambodianRielExchangeRateMnemonicRXEnd };

                        MnemonicsPrepared.Add(RXMnemonics);
                        MnemonicsPrepared.Add(RXEndMnemonics);

                        return MnemonicsPrepared;
                    }
                case Dataset.GlobalEconomyNew:
                    {
                        var seriesToBeCalculatedNominalRX = "RX_AFG.A";
                        var usdExchangeRateMnemonicRX = "";
                        var cambodianRielExchangeRateMnemonicRX = "RX_KHM.A";

                        var seriesToBeCalculatedNominalRXEnd = "RXEND_AFG.A";
                        var usdExchangeRateMnemonicRXEnd = "";
                        var cambodianRielExchangeRateMnemonicRXEnd = "RXEND_KHM.A";

                        List<string> RXMnemonics = new List<string>() { seriesToBeCalculatedNominalRX, usdExchangeRateMnemonicRX, cambodianRielExchangeRateMnemonicRX };
                        List<string> RXEndMnemonics = new List<string>() { seriesToBeCalculatedNominalRXEnd, usdExchangeRateMnemonicRXEnd, cambodianRielExchangeRateMnemonicRXEnd };

                        MnemonicsPrepared.Add(RXMnemonics);
                        MnemonicsPrepared.Add(RXEndMnemonics);

                        return MnemonicsPrepared;
                    }
                default:
                    throw new Exception($"Incorrect dataset chosen = {dataset}! Choose correct one!");
            }
        }

        private Series NominalCurrencyCalculationAdd(Series series, Series usdExchangeRateSeries, Series targetCurrencyExchangeRateSeries)
        {
            var seriesInitialValues = series.Values;
            var usdExchangeRateSeriesValues = usdExchangeRateSeries.Values.ToList();
            var targetCurrencyExchangeRateSeriesValues = targetCurrencyExchangeRateSeries.Values.ToList();

            for (int i = 0; i < seriesInitialValues.Count(); i++)
            {
                series.Values.ToList()[i].Value = series.Values.ToList()[i].Value / usdExchangeRateSeriesValues[i].Value * targetCurrencyExchangeRateSeriesValues[i].Value;
            }

            return series;
        }

        private Series RealCurrencyCalculationAdd(Series series, Series usdExchangeRateSeries, Series targetCurrencyExchangeRateSeries, DateTime basePeriod)
        {
            var seriesInitialValues = series.Values;
            var usdExchangeRateSeriesValue = usdExchangeRateSeries.Values.Single(x => x.Date == basePeriod).Value;
            var targetCurrencyExchangeRateSeriesValues = targetCurrencyExchangeRateSeries.Values.Single(x => x.Date == basePeriod).Value;

            series.Values.All(c => { c.Value = c.Value / usdExchangeRateSeriesValue * targetCurrencyExchangeRateSeriesValues; return true; });

            return series;
        }

        private Series ExchangeRateCalculationAdd(Series series, Series exchangeRateSeries)
        {
            var seriesInitialValues = series.Values;
            var exchangeRateSeriesValues = exchangeRateSeries.Values.ToList();

            for (int i = 0; i < seriesInitialValues.Count(); i++)
            {
                series.Values.ToList()[i].Value = series.Values.ToList()[i].Value / exchangeRateSeriesValues[i].Value;
            }

            return series;
        }

        private bool IsRebasingShouldBeCalculated(DataSetSettings displaySettings, Series series, bool isIndicesRebasingShouldBeApplied)
        {
            var rebasingApplied = displaySettings.UserSettings.AppliedFunctions.Rebasing;
            var rebasingIndicesBaseFrequencyFromDataset = rebasingApplied.IndicesBaseFrequency;
            var rebasingIndicesBaseFrequency = GetFrequencyFromRebasingBase(rebasingIndicesBaseFrequencyFromDataset);
            var rebasingRealValuesBaseFrequencyFromDataset = rebasingApplied.RealValuesBaseFrequency;
            var rebasingRealValuesBaseFrequency = GetFrequencyFromRebasingBase(rebasingRealValuesBaseFrequencyFromDataset);

            var seriesFrequency = series.Frequency;

            if (isIndicesRebasingShouldBeApplied)
            {
                switch (rebasingIndicesBaseFrequency)
                {
                    case Frequency.Annual:
                        return true;
                    case Frequency.Quarterly:
                        {
                            if ((seriesFrequency == Frequency.Quarterly) || (seriesFrequency == Frequency.Monthly))
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    case Frequency.Monthly:
                        {
                            if (seriesFrequency == Frequency.Monthly)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    default:
                        throw new Exception($"Indices rebasing can be of Annual, Quarterly or Monthly frequency! {rebasingIndicesBaseFrequency} is incorrect!");
                }
            }
            else
            {
                switch (rebasingRealValuesBaseFrequency)
                {
                    case Frequency.Annual:
                        return true;
                    case Frequency.Quarterly:
                        {
                            if (seriesFrequency == Frequency.Quarterly)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    default:
                        throw new Exception($"Real monetary rebasing can be of Annual or Quarterly frequency! {rebasingRealValuesBaseFrequency} is incorrect!");
                }
            }
        }

        private Frequency GetFrequencyFromRebasingBase(string baseFrequency)
        {
            return baseFrequency switch
            {
                "ANNL" => Frequency.Annual,
                "QUAR" => Frequency.Quarterly,
                "MONT" => Frequency.Monthly,
                _ => throw new Exception($"incorrect frequency {baseFrequency} chosen!"),
            };
        }

        private string GetRebasingBaseFromFrequency(Frequency frequency)
        {
            return frequency switch
            {
                Frequency.Annual => "ANNL",
                Frequency.Quarterly => "QUAR",
                Frequency.Monthly => "MONT",
                _ => throw new Exception($"incorrect frequency {frequency} chosen!"),
            };
        }

        private double? IndicesAverageSeriesCalculate(Series series, Frequency baseFrequency, DateTime basePeriod)
        {
            var seriesFrequency = series.Frequency;
            var seriesInitialValues = series.Values;
            double? seriesValueForRebasing = 0;
            if (seriesFrequency == baseFrequency)
            {
                seriesValueForRebasing = seriesInitialValues.Single(x => x.Date == basePeriod).Value;
            }

            if ((seriesFrequency == Frequency.Quarterly) && (baseFrequency == Frequency.Annual))
            {
                var basePeriodYear = basePeriod.Year;
                var seriesValuesYear = seriesInitialValues.Where(x => x.Date.Year == basePeriod.Year).Select(x => x.Value).ToList();
                seriesValueForRebasing = seriesValuesYear.Sum() / 4;
            }

            if ((seriesFrequency == Frequency.Monthly) && (baseFrequency == Frequency.Annual))
            {
                var basePeriodYear = basePeriod.Year;
                var seriesValuesYear = seriesInitialValues.Where(x => x.Date.Year == basePeriod.Year).Select(x => x.Value).ToList();
                seriesValueForRebasing = seriesValuesYear.Sum() / 12;
            }

            if ((seriesFrequency == Frequency.Monthly) && (baseFrequency == Frequency.Quarterly))
            {
                var basePeriodYear = basePeriod.Year;
                var monthsList = GetQuarterlyMonthsList(basePeriod);
                List<double?> seriesValuesQuarter = new List<double?>();
                foreach (var month in monthsList)
                {
                    var seriesValuesYear = seriesInitialValues.Where(x => x.Date.Year == basePeriod.Year).ToList();
                    seriesValuesQuarter.Add(seriesValuesYear.Single(x => x.Date.Month == month).Value);
                }
                seriesValueForRebasing = seriesValuesQuarter.Sum() / 3;
            }

            return seriesValueForRebasing;
        }

        private List<int> GetQuarterlyMonthsList(DateTime basePeriod)
        {
            return basePeriod.Month switch
            {
                1 => new List<int>() { 1, 2, 3 },
                4 => new List<int>() { 4, 5, 6 },
                7 => new List<int>() { 7, 8, 9 },
                10 => new List<int>() { 10, 11, 12 },
                _ => throw new Exception("Incorrect Date chosen!"),
            };
        }

        private Series IndicesRebasingCalculationAdd(Series series, Frequency baseFrequency, DateTime basePeriod, int targetBaseValue)
        {
            var seriesInitialValues = series.Values;
            var seriesValueForRebasing = IndicesAverageSeriesCalculate(series, baseFrequency, basePeriod);
            series.Values.All(c => { c.Value = c.Value / seriesValueForRebasing * targetBaseValue; return true; });

            return series;
        }

        private List<string> RealValuesRebasingDataPreparation(Dataset dataset, Frequency baseFrequency, Frequency seriesFrequency)
        {
            string seriesToBeCalculatedMnemonics;
            string nominalMnemonics;
            string targetBasePeriodSeriesMnemonics;

            switch (dataset)
            {
                case Dataset.WorldMarketMonitor:
                    {

                        if (seriesFrequency == Frequency.Annual)
                        {
                            seriesToBeCalculatedMnemonics = "ARGN1IFR.A";
                            nominalMnemonics = "ARGN1IF";
                            targetBasePeriodSeriesMnemonics = "ARGN1IFR";
                        }
                        else
                        {
                            seriesToBeCalculatedMnemonics = "ARGN1IFR.Q";
                            nominalMnemonics = "ARGN1IF";
                            targetBasePeriodSeriesMnemonics = "ARGN1IFR";
                        }

                        break;
                    }
                case Dataset.GlobalEconomyNew:
                    {
                        if (seriesFrequency == Frequency.Annual)
                        {
                            seriesToBeCalculatedMnemonics = "XNETR_MEX.A";
                            nominalMnemonics = "XNET_MEX";
                            targetBasePeriodSeriesMnemonics = "XNETR_MEX";
                        }
                        else
                        {
                            seriesToBeCalculatedMnemonics = "XNETR_MEX.Q";
                            nominalMnemonics = "XNET_MEX";
                            targetBasePeriodSeriesMnemonics = "XNETR_MEX";
                        }

                        break;
                    }
                default:
                    throw new Exception($"Incorrect dataset chosen = {dataset}! Choose correct one!");
            }

            if (baseFrequency == Frequency.Annual)
            {
                nominalMnemonics += ".A";
                targetBasePeriodSeriesMnemonics += ".A";
            }
            else
            {
                nominalMnemonics += ".Q";
                targetBasePeriodSeriesMnemonics += ".Q";
            }

            List<string> MnemonicsPrepared = new List<string>() { seriesToBeCalculatedMnemonics, nominalMnemonics, targetBasePeriodSeriesMnemonics };

            return MnemonicsPrepared;
        }

        private Series RealValuesRebasingCalculationAdd(Series series, Series nominalSeries, Series targetBasePeriodSeries, Frequency baseFrequency, DateTime basePeriod)
        {
            var seriesInitialValues = series.Values;
            var nominalSeriesValue = nominalSeries.Values.Single(x => x.Date == basePeriod).Value;
            var targetBasePeriodSeriesValue = targetBasePeriodSeries.Values.Single(x => x.Date == basePeriod).Value;

            series.Values.All(c => { c.Value = c.Value * nominalSeriesValue / targetBasePeriodSeriesValue; return true; });

            return series;
        }

        private void CheckBasePeriodCorrect(Series series, DataSetSettings displaySettings, Frequency baseFrequency, DateTime basePeriod, bool isIndicesRebasingShouldBeApplied)
        {
            var seriesBasePeriod = series.Attributes["base_period"].First().Value;
            var seriesBasePeriodInternal = series.Attributes["WmmBasePeriodStart"].First().Value;
            var testedMnemonics = series.Attributes["default_mnemonic"].First().Value;

            if (IsRebasingShouldBeCalculated(displaySettings, series, isIndicesRebasingShouldBeApplied))
            {
                switch (baseFrequency)
                {
                    case Frequency.Annual:
                        {
                            seriesBasePeriod.Should().Be(basePeriod.Year.ToString(), $"if Rebasing is applied seriesBasePeriod should be as set for Rebasing, tested mnemonics = {testedMnemonics}, baseFrequency = {baseFrequency}");
                            break;
                        }
                    case Frequency.Quarterly:
                        {
                            var quarterSignExpected = GetQuarterFromMonth(basePeriod);
                            seriesBasePeriod.Should().Be($"{basePeriod.Year}-{quarterSignExpected}", $"if Rebasing is applied seriesBasePeriod should be as set for Rebasing, tested mnemonics = {testedMnemonics}, baseFrequency = {baseFrequency}");
                            break;
                        }
                    case Frequency.Monthly:
                        {
                            seriesBasePeriod.Should().Be($"{basePeriod.Year}-{basePeriod.Month}", $"if Rebasing is applied seriesBasePeriod should be as set for Rebasing, tested mnemonics = {testedMnemonics}, baseFrequency = {baseFrequency}");
                            break;
                        }
                }
            }
            else
            {
                seriesBasePeriod.Should().Be(Convert.ToDateTime(seriesBasePeriodInternal).Year.ToString(), $"if no Rebasing is applied values for seriesBasePeriod and seriesBasePeriodInternal should be equal, tested mnemonics = {testedMnemonics}, baseFrequency = {baseFrequency}");
            }
        }

        private void VerifyWmmCalculationsResults(Series manualCalculatedSeries, Series serverCalculatedSeries, Frequency baseFrequency = Frequency.Annual)
        {
            var testedMnemonics = manualCalculatedSeries.Attributes["default_mnemonic"].First().Value;
            var manualCalculatedSeriesValues = manualCalculatedSeries.Values.ToList();
            List<SeriesValue> manualValues = new List<SeriesValue>();
            foreach (var value in manualCalculatedSeriesValues)
            {
                var date = value.Date;
                var observation = value.Value;
                if (observation != null)
                {
                    observation = Math.Round((double)observation, 9);
                }
                manualValues.Add(new SeriesValue(date, observation));
            }

            var serverCalculatedSeriesValues = serverCalculatedSeries.Values.ToList();
            List<SeriesValue> serverValues = new List<SeriesValue>();
            foreach (var value in serverCalculatedSeriesValues)
            {
                var date = value.Date;
                var observation = value.Value;
                if (observation != null)
                {
                    observation = Math.Round((double)observation, 9);
                }
                serverValues.Add(new SeriesValue(date, observation));
            }

            for (int i = 0; i < serverCalculatedSeriesValues.Count(); i++)
            {
                serverValues[i].Value.Should().Be(manualValues[i].Value, $"values for serverCalculatedSeriesValues date {serverValues[i].Date} should be the same as for {manualValues[i].Date}, tested mnemonics = {testedMnemonics}, baseFrequency = {baseFrequency}");
            }
        }

        private string GetQuarterFromMonth(DateTime dateTime)
        {
            if ((dateTime.Month == 1) || (dateTime.Month == 2) || (dateTime.Month == 3))
            {
                return "Q1";
            }

            if ((dateTime.Month == 4) || (dateTime.Month == 5) || (dateTime.Month == 6))
            {
                return "Q2";
            }

            if ((dateTime.Month == 7) || (dateTime.Month == 8) || (dateTime.Month == 9))
            {
                return "Q3";
            }
            else
            {
                return "Q4";
            }
        }

        private double GetScaleForCalculation(string initialScale)
        {
            return initialScale switch
            {
                "Unit" => 1,
                "Thousands" => 1000,
                "Millions" => 1000000,
                "Billions" => 1000000000,
                "Trillions" => (double)1000000000000,
                _ => throw new Exception("Incorrect Scale chosen!"),
            };
        }

        private string GetRequiredFrequency(string mnemonicsValue)
        {
            var frequencySign = mnemonicsValue.Substring(mnemonicsValue.Length - 1);
            return frequencySign switch
            {
                "A" => "Annual",
                "Q" => "Quarterly",
                "M" => "Monthly",
                _ => throw new Exception("Incorrect Mnemonic end! Check your mnemonic!"),
            };
        }
    }

    public enum CurrencyConversionSeriesType
    {
        Nominal,
        Real,
        ExchangeRate
    }
}
