using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StockTradingRecord
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string secretKey = File.ReadAllText(Environment.CurrentDirectory + @"\secretKey.txt");
        public static string clientId = "CECE92A6760448DFA75543263DBD93B2";
    }
}
