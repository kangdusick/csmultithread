﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ServerCore
{
    internal class Session
    {
        Socket _socket;
        int _disconnected = 0;

        object _lock = new object();
        Queue<byte[]> _sendQueue = new();
        List<ArraySegment<byte>> _pendinglist = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();
        public Session(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _recvArgs.SetBuffer(new byte[1024], 0, 1024);
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            RegisterRecv();
        }
        public void Send(byte[] sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendinglist.Count==0)
                {
                    RegisterSend();
                }
            }
        }
        public void Disconnect()
        {
            if (Interlocked.Exchange(ref _disconnected, 1) == 1)
            {
                return;
            }
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        #region NetworkCommunication 
        void RegisterSend()
        {
            while (_sendQueue.Count>0)
            {
                var buff = _sendQueue.Dequeue();
                _pendinglist.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }
            _sendArgs.BufferList = _pendinglist;
            bool pending = _socket.SendAsync(_sendArgs);
            if(pending==false)
            {
                OnSendCompleted(null , _sendArgs);
            }
        }
        void OnSendCompleted(object sender , SocketAsyncEventArgs args)
        {
            lock(_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendinglist.Clear();

                        Console.WriteLine(_sendArgs.BytesTransferred);

                        if (_sendQueue.Count>0)
                        {
                            RegisterSend();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted Failed {e}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }
        void RegisterRecv()
        {
            bool pending = _socket.ReceiveAsync(_recvArgs);
            if (!pending)
            {
                OnRecvCompleted(null, _recvArgs);
            }


        }
        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    Console.WriteLine($"[From Client] {recvData}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"OnRecvCompleted Failed {e}");
                }

            }
            else
            {
                Disconnect();
            }
        }
        #endregion
    }
}
