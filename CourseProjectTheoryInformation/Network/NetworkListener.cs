using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace DataSender.Network;

internal class NetworkListener
{
    private const int MAXNUMCLIENTS = 10;

    private readonly TcpClient[] clients = new TcpClient[MAXNUMCLIENTS];

    private int _countClient;
    private TcpListener _server;

    private bool _stopNetwork;

    public NetworkListener(int port)
    {
        Port = port;
        StartServer();
    }

    private int Port { get; }

    private void StartServer()
    {
        if (_server == null)
            try
            {
                _stopNetwork = false;
                _countClient = 0;
                _server = new TcpListener(IPAddress.Any, Port);
                _server.Start();


                var acceptThread = new Thread(AcceptClients);
                acceptThread.Start();
            }
            catch
            {
                ErrorSound();
            }
    }

    public void StopServer()
    {
        if (_server != null)
        {
            _server.Stop();
            _server = null;
            _stopNetwork = true;

            for (var i = 0; i < MAXNUMCLIENTS; i++)
                if (clients[i] != null)
                    clients[i].Close();
        }
    }

    private void AcceptClients()
    {
        while (true)
        {
            try
            {
                clients[_countClient] = _server.AcceptTcpClient();
                var readThread = new Thread(ReceiveRun);
                readThread.Start(_countClient);
                _countClient++;
            }
            catch
            {
                ErrorSound();
            }


            if (_countClient == MAXNUMCLIENTS || _stopNetwork) break;
        }
    }

    private void ReceiveRun(object num)
    {
        while (true)
        {
            try
            {
                string s = null;
                var ns = clients[(int)num].GetStream();
                while (ns.DataAvailable)
                {
                    var buffer = new byte[clients[(int)num].Available];

                    ns.Read(buffer, 0, buffer.Length);
                    s += Encoding.Default.GetString(buffer);
                }

                if (s != null)
                {
                    s = "№" + (int)num + ": " + s;
                    SendToClients(s, (int)num);
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

    public void SendToClients(string text, int skipindex = -1)
    {
        for (var i = 0; i < MAXNUMCLIENTS; i++)
            if (clients[i] != null)
            {
                if (i == skipindex) continue;
                var ns = clients[i].GetStream();
                var myReadBuffer = Encoding.Default.GetBytes(text);
                ns.BeginWrite(myReadBuffer, 0, myReadBuffer.Length,
                    AsyncSendCompleted, ns);
            }
    }

    public void AsyncSendCompleted(IAsyncResult ar)
    {
        var ns = (NetworkStream)ar.AsyncState;
        ns.EndWrite(ar);
    }

    private void ErrorSound()
    {
        Console.Beep(3000, 80);
        Console.Beep(1000, 100);
    }
}