﻿using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;
        public sealed override int OnRecv(ArraySegment<byte> buffer)
        {
            int processLen = 0;
            while (true)
            {
                //최소한 헤더는 파싱할 수 있는지 확인
                if (buffer.Count < HeaderSize)
                {
                    break;
                }
                //패킷이 완전체로 도착했는지 확인
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                {
                    break;
                }

                //여기까지 왔으면 패킷 조립 가능
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));

                processLen += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);

            }
            return processLen;
        }
        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }
    public abstract class Session
    {
        Socket _socket;
        int _disconnected = 0;

        RecvBuffer _recvBuffer = new RecvBuffer(1024);

        object _lock = new object();
        Queue<ArraySegment<byte>> _sendQueue = new();
        List<ArraySegment<byte>> _pendinglist = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs _sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs _recvArgs = new SocketAsyncEventArgs();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer);
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);
        public void Start(Socket socket)
        {
            _socket = socket;

            _recvArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            _sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            RegisterRecv();
        }
        public void Send(ArraySegment<byte> sendBuff)
        {
            lock (_lock)
            {
                _sendQueue.Enqueue(sendBuff);
                if (_pendinglist.Count == 0)
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
            OnDisconnected(_socket.RemoteEndPoint);
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        #region NetworkCommunication 
        void RegisterSend()
        {
            while (_sendQueue.Count > 0)
            {
                var buff = _sendQueue.Dequeue();
                _pendinglist.Add(buff);
            }
            _sendArgs.BufferList = _pendinglist;
            var pending = _socket.SendAsync(_sendArgs);
            if (pending == false)
            {
                OnSendCompleted(null, _sendArgs);
            }
        }
        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        _sendArgs.BufferList = null;
                        _pendinglist.Clear();

                        OnSend(_sendArgs.BytesTransferred);

                        if (_sendQueue.Count > 0)
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
            _recvBuffer.Clean();
            ArraySegment<byte> segment = _recvBuffer.WriteSegment;
            _recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);
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
                    if (_recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    int prossesion = OnRecv(_recvBuffer.ReadSegment);
                    if (prossesion < 0 || _recvBuffer.DataSize < prossesion)
                    {
                        Disconnect();
                        return;
                    }
                    if (_recvBuffer.OnRead(prossesion) == false)
                    {
                        Disconnect();
                        return;
                    }



                    RegisterRecv();
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
