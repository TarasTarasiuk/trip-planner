﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Skyscanner.Contracts.BrowseRoutes;
using Skyscanner.Contracts.Places;
using Skyscanner.Contracts.Settings;
using Skyscanner.Contracts.Settings.Countries;
using Skyscanner.Contracts.Settings.Currencies;
using Skyscanner.Contracts.Settings.Locales;
using Tripper.Contracts.Search;
using Tripper.Implementation.Search;
using Tripper.Models.TripSearch;

namespace Tripper.Controllers.Dashboard
{
    public class SearchController : Controller
    {
        private ISearchService _searchService;

        public SearchController()
        {
            _searchService = new SearchService();
        }

        [HttpGet]
        public async Task<JsonResult> GetAvailableCultures()
        {
            var partialUrl = "reference/v1.0/locales?";

            var locales = await _searchService.GetFromUrl<LocalesWrapper>(partialUrl);

            var json = Json(locales.Locales, JsonRequestBehavior.AllowGet);

            return json;
        }

        [HttpGet]
        public async Task<JsonResult> GetAvailableCurrencies()
        {
            var partialUrl = "reference/v1.0/currencies?";

            var currencies = await _searchService.GetFromUrl<CurrenciesWrapper>(partialUrl);

            var json = Json(currencies.Currencies, JsonRequestBehavior.AllowGet);
            
            return json;
        }

        [HttpGet]
        public async Task<JsonResult> GetAvailableCountries(string locale)
        {
            var partialUrl = string.Format(CultureInfo.CurrentCulture, @"/reference/v1.0/countries/{0}?", locale);

            var countries = await _searchService.GetFromUrl<CountriesWrapper>(partialUrl);

            var json = Json(countries.Countries, JsonRequestBehavior.AllowGet);

            return json;
        }

        [HttpGet]
        public async Task<JsonResult> GetAutosuggestedPlace(string query)
        {
            var defaultLocale = "en-US";
            var defaultCurrency = "USD";
            var defaultMarket = "US";

            var partialUrl = string.Format(CultureInfo.CurrentCulture, $"autosuggest/v1.0/{defaultMarket}/{defaultCurrency}/{defaultLocale}?query={query}&");

            var places = await _searchService.GetFromUrl<PlacesWrapper>(partialUrl);

            var json = Json(places.Places, JsonRequestBehavior.AllowGet);

            return json;
        }

        [HttpPost]
        public async Task<JsonResult> GetItinerariesByTripDescription(CreateSessionViewModel viewModel)
        {
            //var parsedOutboundDate = DateTime.Parse(viewModel.OutboundDate);

            //var parsedInboundDate = DateTime.Parse(viewModel.InboundDate);

            var parsedOutboundDate = DateTime.Now;

            var parsedInboundDate = DateTime.Now.AddDays(10);

            viewModel.OutboundDate = parsedOutboundDate.ToString("yyyy-MM-dd");

            viewModel.InboundDate = parsedInboundDate.ToString("yyyy-MM-dd");

            /*
            // Create a session with ScyScanner

            var subUrl = $"pricing/v1.0";

            var content = ToUrlEncodedContent(viewModel);

            string userApi = Request.UserHostAddress;
            string bodyString = $"cabinclass={viewModel.CabinClass}&country={viewModel.Country}&currency={viewModel.Currency}&locale={viewModel.Locale}&locationSchema=iata&originplace={viewModel.OriginPlace}&destinationplace={viewModel.DestinationPlace}&outbounddate={viewModel.OutboundDate}&inbounddate={viewModel.InboundDate}&adults={viewModel.Adults}&children={viewModel.Children}&infants={viewModel.Infants}";
            string locationHeader = await _searchService.GetLocationHeader(subUrl, content, bodyString, userApi);
            */

            var partialUrl = string.Format(CultureInfo.CurrentCulture, $"apiservices/browseroutes/v1.0/{viewModel.Country}/{viewModel.Currency}/{viewModel.Locale}/{viewModel.OriginPlace}/{viewModel.DestinationPlace}/{viewModel.OutboundDate}/{viewModel.InboundDate}?");

            var routes = await _searchService.GetFromUrl<RoutesWrapper>(partialUrl);

            var json = Json(routes.Routes, JsonRequestBehavior.AllowGet);

            return json;
        }

        [NonAction]
        public IEnumerable<KeyValuePair<string, string>> ToUrlEncodedContent(CreateSessionViewModel viewModel)
        {
            var kvPairs = new List<KeyValuePair<string, string>>();

            var allProps = viewModel.GetType().GetProperties();

            foreach (var propertyInfo in allProps)
            {
                kvPairs.Add(new KeyValuePair<string, string>(propertyInfo.Name, viewModel.GetType().GetProperty(propertyInfo.Name).GetValue(viewModel)?.ToString()?? String.Empty));
            }

            return kvPairs;
        }
    }
}