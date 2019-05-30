using Exercise3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public ActionResult display(string ip, int port)
        {
            Simulator.Instance.ConnetAsClient(ip, port);
            ViewBag.Lat = Simulator.Instance.AskLat();
            ViewBag.Lon = Simulator.Instance.AskLon();
            return View();
        }
    }



    
}