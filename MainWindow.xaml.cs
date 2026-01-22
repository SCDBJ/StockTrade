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
            if(cboxTradeType.Text=="")
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
            string stockName= tboxAddStockName.Text;
            string tradeDate = dateTradePicker.Text;
            string tradeType = cboxTradeType.Text;
            string buyPrice = tboxBuyPrice.Text;
            string buyShares = tboxBuyShares.Text;
            string profitLoss = tboxProfitLoss.Text;
            StockTradeModel stockTradeModel = new StockTradeModel { StockCode = stockCode, StockName = stockName, TradeDate = DateTime.Parse(tradeDate), TradePrice=float.Parse(buyPrice), TradeShares=int.Parse(buyShares), TradeType=tradeType, ProfitLossAmount=decimal.Parse(profitLoss) };
            string param= Newtonsoft.Json.JsonConvert.SerializeObject(stockTradeModel);
            string result= HttpService.HttpPost(stockTradeAddUrl,null,param);
            if(result!=null)
            {
                if (result.Contains("保存成功"))
                {
                    string startDate= datePickerStart.Text;
                    string endDate= datePickerEnd.Text;
                    tboxStockCode.Text = "";
                    tboxStockName.Text = "";
                    BindDataGrid();
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
            datePickerStart.SelectedDate= DateTime.Now.AddMonths(-12);
            BindDataGrid();
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
                string deleteJson = "{\"StockTrade\":\"DeleteStockTrade\",\"StockId\":\"" + stockTrade.StockId + "\"}";
                string deleteResult = HttpService.HttpPost(stockTradeDeleteUrl,null, deleteJson);
                if (deleteResult.Contains("删除成功"))
                {
                    BindDataGrid();
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
        }
        private void BindDataGrid()
        {
            string startDate = datePickerStart.Text;
            string endDate = datePickerEnd.Text;
            string param = "{\"StockCode\":\"\",\"StockName\":\"\",\"TradeStartDate\":\"" + startDate + "\",\"TradeEndDate\":\"" + endDate + "\"}";
            string result = HttpService.HttpPost(stockTradeGetUrl, null, param);
            List<StockTradeModel> stockTradeList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StockTradeModel>>(result);
            dataGridRecords.ItemsSource = stockTradeList.OrderByDescending(o=>o.TradeDate);
        }

        private void cboxTradeType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = (sender as ComboBox).SelectedItem;
            if (selectedItem != null)
            {
                var comboBoxItem = selectedItem as ComboBoxItem;
                string tradeType = comboBoxItem.Content.ToString();
                tboxProfitLoss.Text = "";
                if (tradeType != "清仓")
                {
                    tboxProfitLoss.Text="0";
                }
            }
        }
    }
}
