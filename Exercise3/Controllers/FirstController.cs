using Exercise3.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace Exercise3.Controllers
{
    public class FirstController : Controller
    {
        // GET: First
        public ActionResult Index()
        {
            return View();
        }
        /*this is return  view for two mission becouse in the url we accept 2 argument 1:ip and port 2:name of file and second. */
        [HttpGet]
        public ActionResult display_options(string ip, int port)
        {
            //if the firsy element is ip go to the display.
            IPAddress address;
            if(IPAddress.TryParse(ip, out address))
            {
                return display(ip, port);
             
            } else
            {
                //go to the readFromFile.
                return readFromFile(ip, port);
            }
        }

        /*this is return  view that display map of world and we connect to flightSimulator and take the location of simulator. */
        [HttpGet]
        public ActionResult display(string ip, int port)
        {
            //all display view we to connect from the beggining.
            if (Simulator.Instance.IsConnected)
            {
                Simulator.Instance.DisConnect();
            }
            //we connect to simulator.and take the values of lat and lon.
            Simulator.Instance.ConnetAsClient(ip, port);
            //save the value in the view bag. for the view can to access them.
            ViewBag.Lat = Simulator.Instance.Lat;
            ViewBag.Lon = Simulator.Instance.Lon;
            return View("display");
        }
        /*this is return  view that display map of world and we connect to flightSimulator and all 4 second we take the location of simulator.
         * and draw the route of the simulator do */
        [HttpGet]
        public ActionResult displayFour(string ip, int port,int second)
        {
            //save the value in the view bag. for the view can to access them.
            ViewBag.time = second;
            ViewBag.ip = ip;
            ViewBag.port = port;
            //all display view we to connect from the beggining.
            if (Simulator.Instance.IsConnected)
            {
                Simulator.Instance.DisConnect();
            }
            Simulator.Instance.ConnetAsClient(ip, port);
            Session["time"] = second;
            return View();
        }
        /*this is return  view that display map of world and we connect to flightSimulator and all 4 second we take the location of simulator,
         * and save the lon lat rudder and throttle of simulator in file*/
        [HttpGet]
        public ActionResult save(string ip, int port, int second, int duration, string file)
        {
            //save the value in the view bag. for the view can to access them.
            ViewBag.ip = ip;
            ViewBag.port = port;
            //all display view we to connect from the beggining.
            if (Simulator.Instance.IsConnected)
            {
                Simulator.Instance.DisConnect();
            }
            //we connect to simulator.
            Simulator.Instance.ConnetAsClient(ip, port);
            Session["file"] = file;
            Session["time"] = second;
            Session["duration"] = duration;
            return View();
        }
        /*this is return  view that read fron the file the lon lat that save there and display on the map and draw the route of the simulator do  */
        [HttpGet]
        public ActionResult readFromFile(string file, int second)
        {
            Session["time"] = second;
            //we open the fila.
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\App_Data/" + file + ".txt";
            //we save the data in array.
            string[] lines = System.IO.File.ReadAllLines(path);
            Session["lines"] = lines;
            return View("readFromFile");
        }
        //all 4 second the view ask data from the file (lon and lat) and save in xml. 
        [HttpPost]
        public string DataFromFile()
        {
            string[] lines =(string[]) @Session["lines"];
            //if the file finish.
            if(lines.Length == 0)
            {
                return ToXml(-1000, -1000);
            }
            string line = lines[0];
            lines = lines.Skip(1).ToArray();
            Session["lines"] = lines;
            string[] values = line.Split(',');
            //we take from the array the lon and lat value.
            double lon = Convert.ToDouble(values[0]);
            double lat = Convert.ToDouble(values[1]);
            lines = lines.Skip(1).ToArray();
            //save in xml the data.
            return ToXml(lat, lon);
        }

        //all 4 second the view ask data (lon and lat) that save in xml.
        [HttpPost]
        public string GetLonLat()
        {
            //we take from the simulator the value.
            double Lat = Simulator.Instance.Lat;
            double Lon = Simulator.Instance.Lon;
            return ToXml(Lat,Lon);
        }

        //all 4 second the view ask data (lon and lat) that save in file and save too in xml.
        [HttpPost]
        public string GetData()
        {
            //we take from the simulator the value.
            double Lat = Simulator.Instance.Lat;
            double Lon = Simulator.Instance.Lon;
            double Throttle = Simulator.Instance.Throttle;
            double Rudder = Simulator.Instance.Rudder;
            //create file and save the data there.
            string fileName = (string)Session["file"];
            string path = AppDomain.CurrentDomain.BaseDirectory +@"\App_Data/" + fileName + ".txt";
            using (StreamWriter streamWriter = System.IO.File.AppendText(path))
            {
                streamWriter.WriteLine(Convert.ToString(Lon) + ',' + Convert.ToString(Lat)
                            + ',' + Convert.ToString(Throttle) + ',' + Convert.ToString(Rudder));
            }
            return ToXml(Lat, Lon);
        }
        //this function create xml and save tha data lon and lat there.
        private string ToXml(double lat, double lon)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);

            writer.WriteStartDocument();
            writer.WriteStartElement("Position");

            writer.WriteElementString("Lon", lon.ToString());
            writer.WriteElementString("Lat", lat.ToString());


            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }
    }



    
}