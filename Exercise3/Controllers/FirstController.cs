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

        [HttpGet]
        public ActionResult display_options(string ip, int port)
        {
            IPAddress address;
            if(IPAddress.TryParse(ip, out address))
            {
                return display(ip, port);
            } else
            {
                return readFromFile(ip, port);
            }
        }


        [HttpGet]
        public ActionResult display(string ip, int port)
        {
            Simulator.Instance.ConnetAsClient(ip, port);
            ViewBag.Lat = Simulator.Instance.Lat;
            ViewBag.Lon = Simulator.Instance.Lon;
            return View();
        }
        [HttpGet]
        public ActionResult displayFour(string ip, int port,int second)
        {   
            ViewBag.time = second;
            ViewBag.ip = ip;
            ViewBag.port = port;
            Simulator.Instance.ConnetAsClient(ip, port);
            Session["time"] = second;
            return View();
        }

        [HttpGet]
        public ActionResult save(string ip, int port, int second, int duration, string file)
        {
            ViewBag.ip = ip;
            ViewBag.port = port;
            Simulator.Instance.ConnetAsClient(ip, port);
            Session["file"] = file;
            Session["time"] = second;
            Session["duration"] = duration;
            return View();
        }

        [HttpGet]
        public ActionResult readFromFile(string file, int second)
        {
            return View();
        }




        [HttpPost]
        public string GetLonLat()
        {

            double Lat = Simulator.Instance.Lat;
            double Lon = Simulator.Instance.Lon;
            return ToXml(Lat,Lon);
        }


        [HttpPost]
        public string GetData()
        {

            double Lat = Simulator.Instance.Lat;
            double Lon = Simulator.Instance.Lon;
            double Throttle = Simulator.Instance.Throttle;
            double Rudder = Simulator.Instance.Rudder;

            string fileName = (string)Session["file"];
            string path = AppDomain.CurrentDomain.BaseDirectory +@"\App_Data/" + fileName + ".txt";
            using (StreamWriter streamWriter = System.IO.File.AppendText(path))
            {
                streamWriter.WriteLine(Convert.ToString(Lon) + ',' + Convert.ToString(Lat)
                            + ',' + Convert.ToString(Throttle) + ',' + Convert.ToString(Rudder));
            }
            return ToXml(Lat, Lon);
        }

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