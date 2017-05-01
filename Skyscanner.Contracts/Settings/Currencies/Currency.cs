﻿namespace Skyscanner.Contracts.Settings.Currencies
{
    public class Currency
    {
        public string Code { get; set; }
        public string Symbol { get; set; }
        public string ThousandsSeparator { get; set; }
        public string DecimalSeparator { get; set; }
        public string SymbolOnLeft { get; set; }
        public string SpaceBetweenAmountAndSymbol { get; set; }
        public string RoundingCoefficient { get; set; }
        public string DecimalDigits { get; set; }
    }
}
