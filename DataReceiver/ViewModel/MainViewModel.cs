using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using DataReceiver.Command;
using DataReceiver.Models;
using DataReceiver.Network;
using HammingAlgorithm;

namespace DataReceiver.ViewModel;

internal class MainViewModel : INotifyPropertyChanged, IContext
{
    #region Поля

    private readonly Dispatcher _dispatcher;
    private DelegateCommand _buttonClick;
    private DelegateCommand _buttonClickTwo;

    #endregion

    #region Свойства

    private Random Rand { get; }
    public DelegateCommand ButtonBegins => _buttonClick ?? (_buttonClick = new DelegateCommand(BeginWork));
    public DelegateCommand ButtonBeginsTwo => _buttonClickTwo ?? (_buttonClickTwo = new DelegateCommand(DecodeChar));
    private ClientNetwork cn { get; set; }
    public string Code { get; set; }
    public int nab { get; set; }
    public string DecodeText { get; set; }
    public string CodeInput { get; set; }
    public ObservableCollection<CharInfo> DataGridDataInfo { get; set; }
    public ObservableCollection<DecodeText> DecodeGridDataInfo { get; set; }
    public List<List<int>> CheckingMatr { get; }
    private Dictionary<string, char> Codes { get; }
    public string CodeChar { get; set; }
    public string DecodeTextChar { get; set; }
    public ObservableCollection<CharInfoOne> DataGridDataInfoChar { get; set; }

    #endregion

    #region Конструкторы

    public MainViewModel() : this(Dispatcher.CurrentDispatcher)
    {
        CodeInput = "Ожидание данных...";
        cn = new ClientNetwork();
        var refresh = new Thread(Refresh);
        refresh.Start();
        DataGridDataInfo = new ObservableCollection<CharInfo>();
        Rand = new Random(DateTime.Now.Millisecond);
        CheckingMatr = new List<List<int>>();
        FillCheckingMatr();
        Codes = new Dictionary<string, char>();
        
    }

    public MainViewModel(Dispatcher dispatcher)
    {
        Debug.Assert(dispatcher != null);

        _dispatcher = dispatcher;
    }

    #endregion

    #region Методы

    private void BeginWork(object obj)
    {
        DecodeTextCode();
    }

    private void Refresh()
    {
        while (true)
        {
            var prev = cn.Code;
            while (prev == CodeInput)
            {
                prev = cn.Code;
                Thread.Sleep(100);
            }

            Action action3 = () => CodeInput = cn.Code;
            Action action4 = () => Code = cn.Code;
            var action2 = () => Decode();
            var action = () => OnPropertyChanged(nameof(Code));
            Invoke(action4);
            Invoke(action3);
            Invoke(action2);
            Invoke(action);
            Thread.Sleep(100);
        }
    }

    private void Decode()
    {
        if (Code != "Ожидание данных...")
        {
            string[] m = Code.Split(' ');
            Code = m[0];
            nab = int.Parse(m[1]);
            DataGridDataInfo = new ObservableCollection<CharInfo>();
            var Bytes = TextUtils.Split(Code, 8).ToList();
            foreach (var sym in Bytes)
            {
                var s1 = "";
                var s2 = sym.Substring(4, 3);
                var s3 = sym[sym.Length - 1].ToString();
                //if (Rand.Next(0, 10) == 4) s1 = Rfb.FlipOneByte(sym.Substring(0, 4));
                //else if (Rand.Next(0, 10) == 5) s1 = Rfb.FlipTwoByte(sym.Substring(0, 4));
                //else if (Rand.Next(0, 10) == 3) s1 = Rfb.FlipThreeByte(sym.Substring(0, 4));
                s1 = sym.Substring(0, 4);
                DataGridDataInfo.Add(new CharInfo(s1, s2, s3));
            }

            OnPropertyChanged(nameof(DataGridDataInfo));
            DecodeTextCode();
        }
    }

    private void DecodeTextCode()
    {
        DecodeGridDataInfo = new ObservableCollection<DecodeText>();
        foreach (var data in DataGridDataInfo)
        {
            var oneMatr = new List<List<int>>();
            oneMatr.Add(new List<int>());
            for (var i = 0; i < 8; i++)
                if (i >= 0 && i <= 3)
                    oneMatr[0].Add(int.Parse(data.Info[i].ToString()));
                else if (i >= 4 && i <= 6)
                    oneMatr[0].Add(int.Parse(data.Examination[i % 4].ToString()));
                else
                    oneMatr[0].Add(int.Parse(data.Countability[0].ToString()));
            var vs = new List<int>();
            var resultS = "";
            for (var i = 0; i < CheckingMatr.Count; i++) //по первой матрице
            {
                var r = 0;
                for (var j = 0; j < oneMatr[0].Count; j++) //столбец 2 матрицы
                    r += CheckingMatr[i][j] & oneMatr[0][j];
                r = r % 2;
                vs.Add(r);
                resultS += r.ToString();
            }

            var comment = "";
            var sb = new StringBuilder(data.Info + data.Examination + data.Countability);
            if (vs[0] == 0 && vs[1] == 0 && vs[2] == 0 && vs[3] == 0)
            {
                comment = "Ошибки нет";
            }
            else if ((vs[0] != 0 || vs[1] != 0 || vs[2] != 0) && vs[3] == 1)
            {
                comment = "Одна ошибка";
                try
                {
                    var position = 0;
                    for (var i = 0; i < CheckingMatr[0].Count; i++)
                    {
                        if (CheckingMatr[0][i] == vs[0] && CheckingMatr[1][i] == vs[1]
                                                        && CheckingMatr[2][i] == vs[2]) break;
                        position++;
                    }

                    if (sb[position] == '1') sb[position] = '0';
                    else sb[position] = '1';
                }
                catch
                {
                }
            }
            else if ((vs[0] != 0 || vs[1] != 0 || vs[2] != 0) && vs[3] == 0)
            {
                comment = "Более одной ошибки";
            }
            else if (vs[0] == 0 && vs[1] == 0 && vs[2] == 0 && vs[3] == 1)
            {
                comment = "Ошибка в бите четности";
                var position = 7;
                if (sb[position] == '1') sb[position] = '0';
                else sb[position] = '1';
            }

            DecodeGridDataInfo.Add(new DecodeText(
                data.Info,
                data.Examination,
                resultS[resultS.Length - 1].ToString(),
                resultS.Substring(0, 3),
                comment,
                sb.ToString()
            ));
            
            var p = "";
            DecodeText = "";
            foreach (var param in DecodeGridDataInfo)
            {
                p += param.Result.Substring(0, 4);
            }
            p = p.Remove(0, nab);
            DecodeText = p;
        }
        OnPropertyChanged(nameof(DecodeGridDataInfo));
        OnPropertyChanged(nameof(DecodeText));
    }

        private void FillCheckingMatr()
    {
        CheckingMatr.Add(new List<int> { 0, 1, 1, 1, 1, 0, 0, 0 });
        CheckingMatr.Add(new List<int> { 1, 0, 1, 1, 0, 1, 0, 0 });
        CheckingMatr.Add(new List<int> { 1, 1, 0, 1, 0, 0, 1, 0 });
        CheckingMatr.Add(new List<int> { 1, 1, 1, 1, 1, 1, 1, 1 });
    }
    private void DecodeChar(object obj)
    {
        if (DecodeText != "")
        {
            Codes.Clear();
            using (var sr = new StreamReader(@"C:\Data\Info.txt"))
            {
                var line = "";
                while ((line = sr.ReadLine()) != null)
                    if (line.IndexOf(' ') == 0) Codes.Add(line.Substring(2), ' ');
                    else
                        Codes.Add(line.Substring(line.IndexOf(' ') + 1),
                            line.Substring(0, line.IndexOf(' ')).ToArray()[0]);
            }

            DataGridDataInfoChar = new ObservableCollection<CharInfoOne>();
            foreach (var code in Codes) DataGridDataInfoChar.Add(new CharInfoOne(code.Value, code.Key));
            OnPropertyChanged(nameof(DataGridDataInfoChar));
            DecodeCharCode();
        }
    }

    private void DecodeCharCode()
    {
        var result = "";
        var code = "";
        foreach (var letter in DecodeText)
        {
            code += letter;
            if (Codes.ContainsKey(code))
            {
                result += Codes[code];
                code = "";
            }
        }

        DecodeTextChar = result;
        OnPropertyChanged(nameof(DecodeTextChar));
    }

    public void OnWindowClosing(object sender, CancelEventArgs e)
    {
        cn.CloseClient();
        cn = null;
    }

    #endregion

    #region PropertyChanged

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region Threading

    public bool IsSynchronized => _dispatcher.Thread == Thread.CurrentThread;

    public void Invoke(Action action)
    {
        Debug.Assert(action != null);

        _dispatcher.Invoke(action);
    }

    public void BeginInvoke(Action action)
    {
        Debug.Assert(action != null);

        _dispatcher.BeginInvoke(action);
    }

    #endregion
}