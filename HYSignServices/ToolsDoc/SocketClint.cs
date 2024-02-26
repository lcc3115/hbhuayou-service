using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Xml;

namespace HYSignServices.ToolsDoc
{
    public class SocketClint
    {
        private Socket ClientSocket;

        public SocketClint()
        {
            //初始化
            if (ClientSocket == null)
            {
                ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }

            try
            {
                //连接
                if (!ClientSocket.Connected)
                {
                    ClientSocket.Connect("172.92.1.105", 8010);
                }
                //回调
                if (ClientSocket.Connected)
                {
                    Thread thread = new Thread(new ParameterizedThreadStart(ReceiveData));
                    thread.IsBackground = true;
                    thread.Start(ClientSocket);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        static int inData = -1;
        static string tempStr = "";
        static int inStart = 0;
        private void ReceiveData(object obj)
        {
            Socket Socket = obj as Socket;
            byte[] data = new byte[1024 * 265];
            while (true)
            {
                int readlen = 0;
                try
                {
                    readlen = Socket.Receive(data, 0, data.Length, SocketFlags.None);
                    string txt = Encoding.UTF8.GetString(data, 0, readlen);

                    //截取包含相同序列号响应的报文 inData控制排除序列号条件后是否继续执行后续操作
                    if (!string.IsNullOrEmpty(Tools.localSeq) && txt.Contains(Tools.localSeq) || inData == 0)
                    {
                        inData = 0;
                        int start = -1;
                        int end = -1;

                        start = txt.IndexOf("<?xml");

                        tempStr += txt;
                        //处理报文头部被上一条报文尾部粘包的情况，inStart控制如果尾部被下一条报文粘包不继续截取
                        if (start > 0 && inStart == 0)
                        {
                            tempStr = tempStr.Substring(start);
                            inStart = -1;
                        }

                        end = tempStr.IndexOf("</Message>");
                        //截取到尾部，并初始化所有参数
                        if (end > -1)
                        {
                            tempStr = tempStr.Substring(0, end + 10);
                            //Default.WriteLog(tempStr);
                            inData = -1;
                            inStart = 0;
                            tempStr = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    StopConnetct();
                    return;
                }
            }
        }

        public void SendObj(string msg)
        {
            if (ClientSocket.Connected)
            {
                byte[] data = Encoding.UTF8.GetBytes(msg);
                ClientSocket.Send(data, 0, data.Length, SocketFlags.None);
            }
        }

        public void StopConnetct()
        {
            try
            {

                if (ClientSocket.Connected)
                {
                    //ClientSocket.Shutdown(SocketShutdown.Both);
                    ClientSocket.Close();
                }
                //thread.Abort();
            }
            catch (Exception ex)
            {
            }
        }

        
        
    }
}