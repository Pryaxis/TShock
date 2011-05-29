namespace Terraria
{
    using System;
    using System.Net.Sockets;

    public class ClientSock
    {
        public bool active;
        public bool locked;
        public NetworkStream networkStream;
        public byte[] readBuffer;
        public int state;
        public int statusCount;
        public int statusMax;
        public string statusText;
        public TcpClient tcpClient = new TcpClient();
        public int timeOut;
        public byte[] writeBuffer;

        public void ClientReadCallBack(IAsyncResult ar)
        {
            int streamLength = 0;
            if (!Netplay.disconnect)
            {
                streamLength = this.networkStream.EndRead(ar);
                if (streamLength == 0)
                {
                    Netplay.disconnect = true;
                    Main.statusText = "Lost connection";
                }
                else if (Main.ignoreErrors)
                {
                    try
                    {
                        NetMessage.RecieveBytes(this.readBuffer, streamLength, 9);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    NetMessage.RecieveBytes(this.readBuffer, streamLength, 9);
                }
            }
            this.locked = false;
        }

        public void ClientWriteCallBack(IAsyncResult ar)
        {
            messageBuffer buffer1 = NetMessage.buffer[9];
            buffer1.spamCount--;
        }
    }
}

