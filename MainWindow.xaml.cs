using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StockTradingRecord
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string stockTradeAddUrl = "http://42.194.146.7:26500/StockTrade/AddStockTrade";
        private string stockTradeGetUrl = "http://42.194.146.7:26500/StockTrade/GetStockTrade";
        private string stockTradeDeleteUrl = "http://42.194.146.7:26500/StockTrade/DeleteStockTrade";
        StockTradeModel stockTrade = null;
        public MainWindow()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            string startDate = datePickerStart.Text;
            string endDate = datePickerEnd.Text;
            string stockName = tboxStockName.Text;
            string stockCode = tboxStockCode.Text;
            if (cboxQueryTradeDate.Text != "")
            {
                switch (cboxQueryTradeDate.Text)
                {
                    case "近一月":
                        startDate = DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
                        datePickerStart.SelectedDate = DateTime.Now.AddMonths(-1);
                        break;
                    case "近三月":
                        startDate = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd");
                        datePickerStart.SelectedDate = DateTime.Now.AddMonths(-3);
                        break;
                    case "近半年":
                        startDate = DateTime.Now.AddMonths(-6).ToString("yyyy-MM-dd");
                        datePickerStart.SelectedDate = DateTime.Now.AddMonths(-6);
                        break;
                    case "近一年":
                        startDate = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");
                        datePickerStart.SelectedDate = DateTime.Now.AddYears(-1);
                        break;
                    case "近两年":
                        datePickerStart.SelectedDate = DateTime.Now.AddYears(-2);
                        break;
                    case "近三年":
                        startDate = DateTime.Now.AddYears(-3).ToString("yyyy-MM-dd");
                        datePickerStart.SelectedDate = DateTime.Now.AddYears(-3);
                        break;
                    case "近五年":
                        startDate = DateTime.Now.AddYears(-5).ToString("yyyy-MM-dd");
                        datePickerStart.SelectedDate = DateTime.Now.AddYears(-5);
                        break;
                        break;
                }
                datePickerEnd.SelectedDate = DateTime.Now;
            }

            string clientId = App.clientId;
            string secretKey = App.secretKey;
            long nonce = new Random().Next(10000, 999999999);
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            string request = "{\"StockCode\":\"" + stockCode + "\",\"StockName\":\"" + stockName + "\",\"TradeStartDate\":\"" + startDate + "\",\"TradeEndDate\":\"" + endDate + "\"}";
            var sign = SignService.CalcSignature(clientId, secretKey, nonce, timestamp, request);
            var param = "{" + "\"request\":" + request + ",\"clientId\":\"" + clientId + "\",\"nonce\":" + nonce + ",\"timestamp\":" + timestamp + ",\"sign\":\"" + sign + "\"}";

            string result = HttpService.HttpPost(stockTradeGetUrl, null, param);
            if (!string.IsNullOrEmpty(result))
            {
                List<StockTradeModel> stockTradeList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StockTradeModel>>(result);
                if (cboxQueryTradeType.Text != "")
                {
                    stockTradeList = stockTradeList.Where(o => o.TradeType == cboxQueryTradeType.Text).ToList();
                }
                dataGridRecords.ItemsSource = stockTradeList.OrderByDescending(o => o.TradeDate).ThenByDescending(o => o.TradeType);
                var sumProfitLoss = stockTradeList.Sum(o => double.Parse(o.ProfitLossAmount));
                tboxProfitLossAmount.Text = sumProfitLoss.ToString();
            }
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (tboxAddStockCode.Text == "")
            {
                MessageBox.Show("请输入股票代码!");
                return;
            }
            if (tboxAddStockName.Text == "")
            {
                MessageBox.Show("请输入股票名称!");
                return;
            }
            if (cboxTradeType.Text == "")
            {
                MessageBox.Show("请选择交易类型!");
                return;
            }
            if (tboxBuyPrice.Text == "")
            {
                MessageBox.Show("请输入交易价格!");
                return;
            }
            if (tboxBuyShares.Text == "")
            {
                MessageBox.Show("请输入交易数量!");
                return;
            }
            if (tboxProfitLoss.Text == "")
            {
                MessageBox.Show("请输入盈亏金额!");
                return;
            }
            string stockCode = tboxAddStockCode.Text;
            string stockName = tboxAddStockName.Text;
            string tradeDate = dateTradePicker.Text;
            string tradeType = cboxTradeType.Text;
            string buyPrice = tboxBuyPrice.Text;
            string buyShares = tboxBuyShares.Text;
            string profitLoss = tboxProfitLoss.Text;

            string clientId = App.clientId;
            string secretKey = App.secretKey;
            long nonce = new Random().Next(10000, 999999999);
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            StockTradeModel stockTradeModel = new StockTradeModel { StockCode = stockCode, StockName = stockName, TradeDate = DateTime.Parse(tradeDate), TradePrice = buyPrice, TradeShares = buyShares,  TradeType = tradeType, ProfitLossAmount = profitLoss };
            string request = Newtonsoft.Json.JsonConvert.SerializeObject(stockTradeModel);

            var sign = SignService.CalcSignature(clientId, secretKey, nonce, timestamp, request);
            var param = "{" + "\"request\":" + request + ",\"clientId\":\"" + clientId + "\",\"nonce\":" + nonce + ",\"timestamp\":" + timestamp + ",\"sign\":\"" + sign + "\"}";

            string result = HttpService.HttpPost(stockTradeAddUrl, null, param);
            if (result != null)
            {
                if (result.Contains("保存成功"))
                {
                    string startDate = datePickerStart.Text;
                    string endDate = datePickerEnd.Text;
                    tboxStockCode.Text = "";
                    tboxStockName.Text = "";
                    BindDataGrid(cboxQueryTradeType.Text);
                    MessageBox.Show("新增成功!");
                }
                else
                {
                    MessageBox.Show("新增失败!" + result);
                }
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BindDataGrid(cboxQueryTradeType.Text);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void dataGridRecords_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }
        private void DataGridRow_GotFocus(object sender, RoutedEventArgs e)
        {
            var item = (DataGridRow)sender;
            FrameworkElement objElement = dataGridRecords.Columns[0].GetCellContent(item);
            if (objElement != null)
            {
                var dataContent = objElement.DataContext;
                if (dataContent != null)
                {
                    StockTradeModel stockTradeModel = (StockTradeModel)dataContent;
                    stockTrade = stockTradeModel;
                }
            }
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("确定要删除吗?", "提示", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBoxResult == MessageBoxResult.No)
            {
                return;
            }
            if (stockTrade != null)
            {
                string clientId = App.clientId;
                string secretKey = App.secretKey;
                long nonce = new Random().Next(10000, 999999999);
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                string request = "{\"StockTrade\":\"DeleteStockTrade\",\"StockId\":\"" + stockTrade.StockId + "\"}";
                var sign = SignService.CalcSignature(clientId, secretKey, nonce, timestamp, request);
                var deleteJson = "{" + "\"request\":" + request + ",\"clientId\":\"" + clientId + "\",\"nonce\":" + nonce + ",\"timestamp\":" + timestamp + ",\"sign\":\"" + sign + "\"}";

                string deleteResult = HttpService.HttpPost(stockTradeDeleteUrl, null, deleteJson);
                if (deleteResult.Contains("删除成功"))
                {
                    BindDataGrid(cboxQueryTradeType.Text);
                }
            }
        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            tboxAddStockCode.Text = "";
            tboxAddStockName.Text = "";
            cboxTradeType.Text = "";
            tboxBuyPrice.Text = "";
            tboxBuyShares.Text = "";
            tboxProfitLoss.Text = "";
            cboxQueryTradeType.Text = "";
            cboxQueryTradeDate.Text = "近一月";

            tboxStockName.Text = "";
            tboxStockCode.Text = "";
            datePickerStart.SelectedDate = DateTime.Now.AddMonths(-1);
            datePickerEnd.SelectedDate = DateTime.Now;
        }
        private void BindDataGrid(string tradeType)
        {
            string startDate = datePickerStart.Text;
            string endDate = datePickerEnd.Text;
            string clientId = App.clientId;
            string secretKey = App.secretKey;
            long nonce = new Random().Next(10000, 999999999);
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            string request = "{\"StockCode\":\"\",\"StockName\":\"\",\"TradeStartDate\":\"" + startDate + "\",\"TradeEndDate\":\"" + endDate + "\"}";
            if(startDate=="" || endDate == "")
            {
                return;
            }
            var sign = SignService.CalcSignature(clientId, secretKey, nonce, timestamp, request);
            var param = "{" + "\"request\":" + request + ",\"clientId\":\"" + clientId + "\",\"nonce\":" + nonce + ",\"timestamp\":" + timestamp + ",\"sign\":\"" + sign + "\"}";

            string result = HttpService.HttpPost(stockTradeGetUrl, null, param);
            if (!string.IsNullOrEmpty(result))
            {
                List<StockTradeModel> stockTradeList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StockTradeModel>>(result);
                if (dataGridRecords != null)
                {
                    List<StockTradeModel> filteredList = stockTradeList;
                    if (tradeType == "")
                    {
                        dataGridRecords.ItemsSource = filteredList.OrderByDescending(o => o.TradeDate).ThenByDescending(o => o.TradeType);
                    }
                    else
                    {
                        filteredList= filteredList.Where(o => o.TradeType == tradeType).ToList();
                        dataGridRecords.ItemsSource = filteredList.OrderByDescending(o => o.TradeDate).ThenByDescending(o => o.TradeType);
                    }
                    var sumProfitLoss = filteredList.Sum(o => double.Parse(o.ProfitLossAmount));
                    tboxProfitLossAmount.Text = sumProfitLoss.ToString();
                }
            }
        }

        private void cboxTradeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ComboBox).SelectedItem;
            if (selectedItem != null)
            {
                var comboBoxItem = selectedItem as ComboBoxItem;
                string tradeType = comboBoxItem.Content.ToString();
                if (tboxProfitLoss != null)
                {
                    tboxProfitLoss.Text = "";
                    if (tradeType != "清仓")
                    {
                        tboxProfitLoss.Text = "0";
                    }
                    BindDataGrid(tradeType);
                }
            }
        }

        private void cboxQueryTradeDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ComboBox).SelectedItem;
            if (selectedItem != null)
            {
                var comboBoxItem = selectedItem as ComboBoxItem;
                string queryTradeDate = comboBoxItem.Content.ToString();
                switch (queryTradeDate)
                {
                    case "近一月":
                        datePickerStart.SelectedDate = DateTime.Now.AddMonths(-1);
                        break;
                    case "近三月":
                        datePickerStart.SelectedDate = DateTime.Now.AddMonths(-3);
                        break;
                    case "近半年":
                        datePickerStart.SelectedDate = DateTime.Now.AddMonths(-6);
                        break;
                    case "近一年":
                        datePickerStart.SelectedDate = DateTime.Now.AddYears(-1);
                        break;
                    case "近两年":
                        datePickerStart.SelectedDate = DateTime.Now.AddYears(-2);
                        break;
                    case "近三年":
                        datePickerStart.SelectedDate = DateTime.Now.AddYears(-3);
                        break;
                    case "近五年":
                        datePickerStart.SelectedDate = DateTime.Now.AddYears(-5);
                        break;
                    default:
                        datePickerStart.SelectedDate = DateTime.Now;
                        break;
                }
                datePickerEnd.SelectedDate = DateTime.Now;
                BindDataGrid(cboxQueryTradeType.Text);
            }
        }
        private void btnCopyStockName_Click(object sender, RoutedEventArgs e)
        {
            if (stockTrade != null)
            {
                if (Clipboard.ContainsText())
                {
                    Clipboard.Clear();
                }
                Clipboard.SetText(stockTrade.StockName);
            }
        }

        private void btnCopyStockCode_Click(object sender, RoutedEventArgs e)
        {
            if (stockTrade != null)
            {
                if (Clipboard.ContainsText())
                {
                    Clipboard.Clear();
                }
                Clipboard.SetText(stockTrade.StockCode);
            }
        }
    }
}
