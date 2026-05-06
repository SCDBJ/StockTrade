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
        public decimal TradePrice
        {
            get; set;
        }
        public int TradeShares
        {
            get; set;
        }
        public int TradeAmount => (int)(TradePrice * TradeShares);
        public decimal ProfitLossAmount
        {
            get; set;
        }
        public string TradeType
        {
            get; set;
        }
    }
}
