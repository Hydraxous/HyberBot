using Discord.Rest;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HyberBot.KnightsTryIntegration.Net
{
    public class KnightClient
    {
        private Socket connection;
        private IPEndPoint endpoint;

        public bool Connected { get; private set; }

        public Action<string> OnCommandReceived;
        public Action OnDisconnect;

        public Queue<string> CommandQueue = new Queue<string>();

        private byte[] buffer;

        public KnightClient() 
        {
            connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task<bool> ConnectAsync(string ip, int port)
        {
            if (CheckConnection())
                Disconnect();

            if(!IPAddress.TryParse(ip, out IPAddress address))
            {
                throw new ArgumentException("Invalid Ip address.");
            }

            IPEndPoint localEndPoint = new IPEndPoint(address, port);

            try
            {
                await connection.ConnectAsync(localEndPoint);
                Connected = true;
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }


            Connected = false;
            return false;
        }

        public bool StartCommandListener()
        {
            if (!CheckConnection() || !Connected)
                return false;

            Task.Run(CommandListener);
            return true;
        }

        private async Task CommandListener()
        {
            while(Connected)
            {
                buffer = new byte[connection.ReceiveBufferSize];

                bool waitingForCommand = true;

                IAsyncResult result = connection.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, null, null);

                while(!result.IsCompleted && Connected)
                {
                    await Task.Yield();
                }

                if(!Connected)
                {
                    break;
                }

                int bytesRead = connection.EndReceive(result);

                if (bytesRead <= 0)
                    continue;

                byte[] formatted = new byte[bytesRead];

                for (int i = 0; i < bytesRead; i++)
                {
                    formatted[i] = buffer[i];
                }

                string command = Encoding.ASCII.GetString(formatted);

                CommandQueue.Enqueue(command);
                OnCommandReceived?.Invoke(command); 
            }
        }

        public bool Connect(string ip, int port)
        {
            if (CheckConnection())
                Disconnect();

            if (!IPAddress.TryParse(ip, out IPAddress address))
            {
                return false;
            }

            IPEndPoint localEndPoint = new IPEndPoint(address, port);

            try
            {
                connection.Connect(localEndPoint);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return false;
            }

            return true;
        }

        private bool CheckConnection()
        {
            if (connection == null)
                return false;

            Connected = connection.Connected;
            return Connected;
        }

        public void Disconnect()
        {
            if (!CheckConnection())
                return;

            Connected = false;
            connection.Disconnect(true);
            CommandQueue.Clear();
            OnDisconnect?.Invoke();
        }


        public void SendCommandRaw(string rawCommand)
        {
            if (!CheckConnection())
                return;

            byte[] data = Encoding.ASCII.GetBytes(rawCommand);
            connection.Send(data);
        }

    }
}
