using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using Newtonsoft.Json;
using SingleKinect.EngagerTrack;

namespace SingleKinect.SendData
{

    public class SendData
    {
        private static SendData sendData;

        private SendData() { }

        public static SendData Instance
        {
            get
            {
                if (sendData == null)
                {
                    sendData = new SendData();
                }
                return sendData;
            }
        }

        private static TcpClient client;

        private NetworkStream nwStream;
        private StreamReader reader;
        private StreamWriter _writer;

        private bool isConnected;

        public void connect()
        {
            try
            {
                client = new TcpClient("127.0.0.1", 5000);
            }
            catch
            {
                Debug.Print("not connected to server");
                isConnected = false;
                return;
            }

            nwStream = client.GetStream();
            reader = new StreamReader(nwStream);
            _writer = new StreamWriter(nwStream) { AutoFlush = true };

            isConnected = true;
        }

        public void send(DataToSend sendingData)
        {
            if (!isConnected)
            {
                return;
            }

            string lineToSend = getSendingJson(sendingData);
            try
            {
                _writer.WriteLine(lineToSend);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                isConnected = false;
            }
        

        //receive();
        }

        public void receive()
        {
            string lineReceived = reader.ReadLine();
            //Console.WriteLine("Received from server: " + lineReceived);
        }

        private string getSendingJson(DataToSend sendingData)
        {
            string json = JsonConvert.SerializeObject(sendingData);

            return json;
        }
    }
}