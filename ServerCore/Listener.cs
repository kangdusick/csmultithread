using System;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace ServerCore
{
    internal class Listener
    {
        Socket _listenSocket;
        Func<Session> _sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            _listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _sessionFactory += sessionFactory;

            //문지기 교육
            _listenSocket.Bind(endPoint);
            //영업 시작
            //backlog:최대 대기수
            _listenSocket.Listen(10);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAcceept(args);
        }

        void RegisterAcceept(SocketAsyncEventArgs args)
        {
            args.AcceptSocket = null;

            bool pending = _listenSocket.AcceptAsync(args);
            if (!pending)
            {
                OnAcceptCompleted(null, args);
            }
        }
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args )
        {
            //async함수: 스레드풀에서 작동하기때문에 멀티스레드임을 인지하고 코딩해야함.
            if(args.SocketError == SocketError.Success)
            {
                Session session = _sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            RegisterAcceept(args);
        }
    }
}
