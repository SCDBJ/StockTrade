using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockTradingRecord
{
    public class StockTradeModel
    {
        public string StockId
        {
            get; set;
        }
        public string StockCode
        {
            get; set;
        }
        public string StockName
        {
            get; set;
        }
        public DateTime TradeDate
        {
            get; set;
        }
        public string TradePrice
        {
            get; set;
        }
        public string TradeShares
        {
            get; set;
        }
        public string TradeAmount => (double.Parse(TradePrice) * double.Parse(TradeShares)).ToString();
        public string ProfitLossAmount
        {
            get; set;
        }
        public string TradeType
        {
            get; set;
        }
    }
}
