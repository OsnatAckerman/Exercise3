using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace Exercise3.Models
{
    public class Simulator
    {
        TcpClient client;
        string latCommand = "get /position/latitude-deg\r\n";
        string lonCommand = "get /position/longitude-deg\r\n";
        string rudderCommand = "get /controls/flight/rudder\r\n";
        string throttleCommand = "get /controls/engines/current-engine/throttle\r\n";
        /// singelton
        private static Simulator m_Instance = null;
        public static Simulator Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = new Simulator();
                }
                return m_Instance;
            }
        }

        //private constructor will be called from singelton
        private Simulator()
        {
            isConnected = false;
        }

        private bool isConnected;
        public bool IsConnected
        {
            get;
            set;
        }

        public double Lat
        {
            get { return GetValFromSimulator(latCommand); }
            set
            {
                Lat = value;
            }
        }

        public double Lon
        {
            get
            {
                return GetValFromSimulator(lonCommand);
            }
            set
            {
                Lon = value;
            }
        }

        public double Rudder
        {
            get
            {
                return GetValFromSimulator(rudderCommand);
            }
            set
            {
                Rudder = value;
            }
        }

        public double Throttle
        {
            get
            {
                return GetValFromSimulator(throttleCommand);
            }
            set
            {
                Throttle = value;
            }
        }

        /* connect to flight simulater as client on the setted IP and port */
        public void ConnetAsClient(string IP, int port)
        {
            //extract the saved IP and port from the ApllicationSettingModel 
            IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(IP), port);
            client = new TcpClient();
            client.Connect(iPEndPoint);
            isConnected = true;
        }


        public void DisConnect()
        {
            isConnected = false;
            client.Close();
        }

        /* the function to run in thread, send the orders to simulator */
        private double GetValFromSimulator(string command)
        {
            if (!isConnected)
            {
                return -1;
            }
            NetworkStream stream = client.GetStream();
            //message to bytes
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(command);
            stream.Write(data, 0, data.Length);
            //preprocess to get answer
            data = new Byte[client.ReceiveBufferSize];

            // String to store the response ASCII representation.
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            string[] answer =  responseData.Split('\'');
            return Convert.ToDouble(answer[1]);
        }



        public double AskLat()
        {
            return GetValFromSimulator(latCommand);
        }

        public double AskLon()
        {
            return GetValFromSimulator(lonCommand);
        }
    }
}