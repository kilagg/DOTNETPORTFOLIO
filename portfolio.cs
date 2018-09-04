using PricingLibrary.Computations;
using PricingLibrary.FinancialProducts;
using PricingLibrary.Utilities.MarketDataFeed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projet1
{
    class portfolio
    {
 
        public double getRisklessRate(double timespan)
        {
             return PricingLibrary.Utilities.MarketDataFeed.RiskFreeRateProvider.GetRiskFreeRateAccruedValue(timespan);

        }
        public DateTime ComputeNextDay(DateTime currentDate, double window)
        {
            DateTime nextDay = currentDate.AddDays(window);
            return nextDay;
        }

        public PricingResults ComputeDeltaCall(VanillaCall call, DateTime atTime, int nbDaysPerYear, double spot, double volatility)
        {
            PricingResults price = new PriceCall(call, atTime, nbDaysPerYear, spot, volatility);
            return price;
        }

        public static double UpdatePortfolio(DateTime maturity, double risklessRate, DateTime date, SimulatedDataFeedProvider data, VanillaCall call, int nbDaysPerYear,
            double volatility)
        {
            List<DataFeed> spotList = data.GetDataFeed(call, date); //
            decimal spot = spotList[0].PriceList[call.UnderlyingShare.Id];

            PricingResults price;
            price = Pricer.PriceCall(call, date, nbDaysPerYear, (double)spot, volatility); //
            double pricePortfolio = price.Price;
            double delta = price.Deltas[0];
            double riskyPosition = delta;
            //Dictionary<string, decimal> price = datafeed.PriceList.TryGetValue(call.Name,out 0);
            double riskFreePosition = pricePortfolio - delta * (double)spot;
            while (date < maturity)
            {
                date.AddDays(1);
                spotList = data.GetDataFeed(call, date); //
                spot = spotList[0].PriceList[call.UnderlyingShare.Id];
                pricePortfolio = riskyPosition * (double)spot + riskFreePosition * Math.Exp(risklessRate / 365);
                price = Pricer.PriceCall(call, date, nbDaysPerYear, (double)spot, volatility); //
                delta = price.Deltas[0];
                riskyPosition = delta;
                riskFreePosition = pricePortfolio - delta * (double)spot;
            }
            return riskyPosition * (double)spot + riskFreePosition * Math.Exp(risklessRate / 365);

        }
    }
}
