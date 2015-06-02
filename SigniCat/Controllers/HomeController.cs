using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace SigniCat.Controllers
{
    public class HomeController:Controller
    {
        SigniCat.Models.SigniCatModels SigniCat = new SigniCat.Models.SigniCatModels();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload()
        {
            try
            {
                string documentId = SigniCat.upload_document_to_SDS( Server.MapPath( "/tobesigned.pdf" ) );
                ViewBag.documentId = documentId;
            }
            catch( Exception e )
            {
                ViewBag.documentId = e.Message;
            }

            return View();
        }

        public ActionResult Sign( string did )
        {
            try
            {
                string redirecturl = SigniCat.get_sign_url_from_SDS( did );
                Response.Redirect( redirecturl );
                ViewBag.result = "Feil! Ble ikke redirigert!";

            }
            catch( Exception e )
            {
                ViewBag.result = e.Message;
            }

            return View();
        }

        public ActionResult Download( string did )
        {
            try
            {
                SigniCat.download_document_from_SDS( did, Server.MapPath( "/downloaded.pdf" ) );
                ViewBag.result = "Success";
            }
            catch( Exception e )
            {
                ViewBag.result = e.Message;
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}