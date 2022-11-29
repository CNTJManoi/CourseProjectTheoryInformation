using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using DataSender.Algorithms;
using DataSender.Models;
using DataSender.Network;

namespace DataSender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<HammingRow> GenerativeMatr { get; set; }
        private List<List<int>> GenMatr { get; set; }
        private NetworkListener nl { get; set; }
        public ObservableCollection<HammingRow> CheckingMatr { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            FillCheckingMatr();
            CheckingMatrixGrid.ItemsSource = CheckingMatr;
            FillGenerativeMatr();
            GenerativeMatrixGrid.ItemsSource = GenerativeMatr;
            nl = new NetworkListener(5555);
        }
        private void BeginWorkButton_Click(object sender, RoutedEventArgs e)
        {
            var resultCode = "";
            var message = CodeTextBox.Text;
            if (message == "" || message == null)
            {
                MessageBox.Show(
                    "Введите сообщение в соответствующее поле!");
                return;
            }
            Clear();
            var fa = new FanoAlgorithm(message);
            var Codes = fa.Res;
            var Table = fa.ReturnTable();
            InfoDataGrid.ItemsSource = Table;
            foreach (var charing in Codes) ResultCode.Text += charing;
            List<float> LmFloats = new List<float>();
            List<float> PxFloats = new List<float>();
            foreach (var symbol in Table)
            {
                LmFloats.Add(symbol.Lm);
                PxFloats.Add(symbol.Px);
            }
            OptimalityTextBox.Text = InfoAboutCode.FindCodeOptimality(PxFloats, LmFloats, message);
            RedundancyTextBox.Text =
                Math.Round(InfoAboutCode.FindRelativeRedundancy(PxFloats, LmFloats, message), 2) * 100 + "%";
            if (File.Exists(@"C:\Data\Info.txt")) File.Delete(@"C:\Data\Info.txt");
            File.Create(@"C:\Data\Info.txt").Close();
            using (var sw = new StreamWriter(@"C:\Data\Info.txt"))
            {
                for (var j = 0; j < Table.Count; j++) sw.WriteLine(Table[j].Char + " " + Table[j].Code);
            }

        }
        private void Clear()
        {
            ResultCode.Text = "";
            RedundancyTextBox.Text = "";
        }
        private void FillCheckingMatr()
        {
            CheckingMatr = new ObservableCollection<HammingRow>();
            CheckingMatr.Add(new HammingRow(0, 1, 1, 1, 1, 0, 0 ));
            CheckingMatr.Add(new HammingRow(1, 0, 1, 1, 0, 1, 0));
            CheckingMatr.Add(new HammingRow( 1, 1, 0, 1, 0, 0, 1 ));
        }

        private void FillGenerativeMatr()
        {
            GenerativeMatr = new ObservableCollection<HammingRow>();
            GenMatr = new List<List<int>>();
            List<List<int>> CheckMatr = new List<List<int>>();
            CheckMatr.Add(new List<int> { 0, 1, 1, 1, 1, 0, 0 });
            CheckMatr.Add(new List<int> { 1, 0, 1, 1, 0, 1, 0 });
            CheckMatr.Add(new List<int> { 1, 1, 0, 1, 0, 0, 1 });
            var MatrixWithoutOne = new List<List<int>>();
            for (var i = 0; i < CheckingMatr.Count; i++)
            {
                MatrixWithoutOne.Add(new List<int>());
                for (var j = 0; j < CheckMatr[i].Count - 3; j++) MatrixWithoutOne[i].Add(CheckMatr[i][j]);
            }

            var Trans = MatrixOptions.TransposeMatrix(MatrixWithoutOne);
            for (var i = 0; i < Trans.Count; i++)
            {
                GenMatr.Add(new List<int>());
                for (var j = 0; j < Trans.Count; j++)
                    if (i == j) GenMatr[i].Add(1);
                    else GenMatr[i].Add(0);
            }

            for (var i = 0; i < Trans.Count; i++)
            for (var j = 0; j < Trans[i].Count; j++)
                GenMatr[i].Add(Trans[i][j]);
            for (int i = 0; i < GenMatr.Count; i++)
            {
                GenerativeMatr.Add(new HammingRow(GenMatr[i][0], GenMatr[i][1], GenMatr[i][2], GenMatr[i][3]
                , GenMatr[i][4], GenMatr[i][5], GenMatr[i][0]));
            }

        }
        private void BeginWorkButton_Click_1(object sender, RoutedEventArgs e)
        {
            if (ResultCode.Text == "" || ResultCode == null)
            {
                MessageBox.Show("Сгенерируйте код методом побуквенного кодирования!");
                return;
            }

            var result = Hamming.CodeText(ResultCode.Text, GenMatr);
            ResultCode2.Text = result;
            nl.SendToClients(ResultCode2.Text); 
        }
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            nl.StopServer();
            nl = null;
        }
    }
}
