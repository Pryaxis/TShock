namespace Terraria
{
    using System;
    using System.Net.Sockets;

    public class ServerSock
    {
        public bool active;
        public bool announced;
        public Socket clientSocket;
        public bool kill;
        public bool locked;
        public string name = "Anonymous";
        public NetworkStream networkStream;
        public string oldName = "";
        public byte[] readBuffer;
        public int state;
        public int statusCount;
        public int statusMax;
        public string statusText = "";
        public string statusText2;
        public TcpClient tcpClient = new TcpClient();
        public bool[,] tileSection = new bool[Main.maxTilesX / 200, Main.maxTilesY / 150];
        public int timeOut;
        public int whoAmI;
        public byte[] writeBuffer;

        public void Reset()
        {
            for (int i = 0; i < Main.maxSectionsX; i++)
            {
                for (int j = 0; j < Main.maxSectionsY; j++)
                {
                    this.tileSection[i, j] = false;
                }
            }
            if (this.whoAmI < 8)
            {
                Main.player[this.whoAmI] = new Player();
            }
            this.timeOut = 0;
            this.statusCount = 0;
            this.statusMax = 0;
            this.statusText2 = "";
            this.statusText = "";
            this.name = "Anonymous";
            this.state = 0;
            this.locked = false;
            this.kill = false;
            this.active = false;
            NetMessage.buffer[this.whoAmI].Reset();
            if (this.networkStream != null)
            {
                this.networkStream.Close();
            }
            if (this.tcpClient != null)
            {
                this.tcpClient.Close();
            }
        }

        public void ServerReadCallBack(IAsyncResult ar)
        {
            int streamLength = 0;
            if (!Netplay.disconnect)
            {
                try
                {
                    streamLength = this.networkStream.EndRead(ar);
                }
                catch
                {
                }
                if (streamLength == 0)
                {
                    this.kill = true;
                }
                else
                {
                    if (Main.ignoreErrors)
                    {
                        try
                        {
                            NetMessage.RecieveBytes(this.readBuffer, streamLength, this.whoAmI);
                            goto Label_0057;
                        }
                        catch
                        {
                            goto Label_0057;
                        }
                    }
                    NetMessage.RecieveBytes(this.readBuffer, streamLength, this.whoAmI);
                }
            }
        Label_0057:
            this.locked = false;
        }

        public void ServerWriteCallBack(IAsyncResult ar)
        {
            messageBuffer buffer1 = NetMessage.buffer[this.whoAmI];
            buffer1.spamCount--;
            if (this.statusMax > 0)
            {
                this.statusCount++;
            }
        }
    }
}

