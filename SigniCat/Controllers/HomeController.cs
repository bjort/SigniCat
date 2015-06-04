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
                //string redirecturl = SigniCat.get_sign_url_from_SDS( did );
                string requestid = SigniCat.get_requestId_from_SDS( did );
                ViewBag.requestId = requestid;
                ViewBag.result = "Success!";

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

        public ActionResult DownloadSigned( string did )
        {
            try
            {
                string url = SigniCat.get_url_to_signed_doc( did );
                ViewBag.result = "Feil! Ble ikke redirigert!";
                Response.Redirect( url );
            }
            catch( Exception e )
            {
                ViewBag.result = e.Message;
            }

            return View();
        }

    }
}