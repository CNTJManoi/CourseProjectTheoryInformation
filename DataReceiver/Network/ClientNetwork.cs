using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DataReceiver.Network;

public class ClientNetwork
{
    private readonly TcpClient _tcpСlient = new();

    private readonly int port = 5555;
    private bool _stopNetwork;

    private NetworkStream ns;

    public ClientNetwork()
    {
        Code = "Ожидание данных...";
        Connect();
    }

    public string Code { get; set; }

    private void Connect()
    {
        try
        {
            _tcpСlient.Connect("127.0.0.1", port);

            ns = _tcpСlient.GetStream();

            var th = new Thread(ReceiveRun);
            th.Start();
        }
        catch
        {
            ErrorSound();
        }
    }

    public void CloseClient()
    {
        if (ns != null) ns.Close();
        if (_tcpСlient != null) _tcpСlient.Close();

        _stopNetwork = true;
    }

    private void ReceiveRun()
    {
        while (true)
        {
            try
            {
                string s = null;
                while (ns.DataAvailable)
                {
                    var buffer = new byte[_tcpСlient.Available];

                    ns.Read(buffer, 0, buffer.Length);
                    s += Encoding.Default.GetString(buffer);
                }

                if (s != null)
                {
                    Code = s;
                    s = string.Empty;
                }

                Thread.Sleep(100);
            }
            catch
            {
                ErrorSound();
            }

            if (_stopNetwork) break;
        }
    }

    private void ErrorSound()
    {
        Console.Beep(2000, 80);
        Console.Beep(3000, 120);
    }
}