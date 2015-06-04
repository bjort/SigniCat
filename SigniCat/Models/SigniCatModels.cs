using SigniCat.ServiceReference1;
using SigniCat.ServiceReference2;
using System.IO;
/*using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;*/
using System.Net;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SigniCat.Models
{

    public class SigniCatModels
    {
        private string passwd = "Bond007";
        private string service = "shared";
        private string taskid =  "task_1";
        private string docid  =  "doc_1";
        private string subid  = "sub_1";

        public string upload_document_to_SDS( string path )
        {
            byte[] document = File.ReadAllBytes( path );

            using (var webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential("shared", "Bond007");
                webClient.Headers.Add( "Content-Type", "application/pdf" );
                byte[] response = webClient.UploadData( "https://preprod.signicat.com/doc/shared/sds", "POST", document );
                return System.Text.Encoding.Default.GetString( response );
            }
        }

        public string get_requestId_from_SDS( string did )
        {
            createrequestrequest request = new createrequestrequest
            {
                password = passwd,
                service = service,                
                request = new request[]
                 {
                     
                     new request
                     {
                         
                         clientreference = "cliref1",
                         language = "nb",
                         profile = "default",
                         document = new document[]
                         {
                             new sdsdocument
                             {
                                 id = docid, 
                                 refsdsid = did,
                                 description = "Terms and conditions"

                             }
                         },
                         subject = new subject[]
                         {
                             new subject
                             {
                                 id = subid, 
                                 nationalid = "02082213530" /* Brukerens personnr */
                             }
                         }/*,
                        notification = new notification[]
                        {
                            new notification
                            {
                                type = notificationtype.URL,
                                notificationid = "not_1",
                                message = "don't give up!",
                                recipient = "http://campus.inkrement.no/Show/ShowNode/"
                            }
                        }*/,
                        task = new task[]
                         {
                             new task
                             {
                                bundle = false,
                                bundleSpecified = true,
                                ontaskcomplete = "http://campus.inkrement.no/Show/SaveAnswer?nodeId=1652928&ans=SigniCat&score=null&userId=0&courseId=90530",
                                ontaskcancel = "",
                                ontaskpostpone = "",
                                id = taskid,
                                 subjectref = subid,
                                 documentaction = new documentaction[]
                                 {
                                      new documentaction
                                      {
                                          type = documentactiontype.sign,
                                          documentref = docid
                                      }
                                 },

                                 signature = new signature[]
                                 {
                                     new signature
                                     {
                                         method = new string[]
                                         {
                                             "nbid-ltv-sign"
                                         }
                                     }
                                 }/*,
                                 notification = new notification[]{
                                        new notification
                                        {
                                            type = notificationtype.URL,
                                            notificationid = "not_2",
                                            message = "don't give up!",
                                            recipient = "http://campus.inkrement.no/Show/ShowNode/",
                                            schedule = new schedule[]
                                            {
                                                 new schedule
                                                 {
                                                     stateis = taskstatus.completed
                                                 }
                                            }

                                        }
                                 }                                   */
                             }
                         }
                     }
                 }
            };

            
            createrequestresponse response;
            using( var client = new DocumentEndPointClient() )
            {
                response = client.createRequest( request );
            }
            return response.requestid[ 0 ];

            //Assert.AreEqual( HttpStatusCode.Created, response.StatusCode );
            //Assert.IsTrue( documentId.Length > 0 );
        }

        public void download_document_from_SDS( string did, string path )
        {
            using( var webClient = new WebClient() )
            {
                webClient.Credentials = new NetworkCredential(  service, passwd );
                byte[] document = webClient.DownloadData( "https://preprod.signicat.com/doc/shared/sds/" + did );
                File.WriteAllBytes( path, document );
            }
            //Assert.AreEqual( HttpStatusCode.Created, response.StatusCode );
            //Assert.IsTrue( documentId.Length > 0 );
        }

        public string get_url_to_signed_doc( string did )
        {
            string requestId = did;
            string originalUri = null;
            string resultUri = null;

            using( var client = new DocumentEndPointClient() )
            {
                var request = new getstatusrequest
                {
                    password = passwd,
                    service = service,
                    requestid = new string[]
                    {
                        requestId
                    }
                };

                var taskStatusInfo = client.getStatus( request );

                if( taskStatusInfo.Length != 1 ) throw new System.Exception( "Oppgaven har ingen status!" );
                if( taskstatus.completed != taskStatusInfo[ 0 ].taskstatus ) throw new System.Exception( "Oppgaven har ikke status 'complete'!" );
                if( taskStatusInfo[ 0 ].documentstatus[ 0 ].id != docid ) throw new System.Exception( "Feil dokument id: " + taskStatusInfo[ 0 ].documentstatus[ 0 ].id + " != " + docid ); 

                originalUri = string.Format( "https://preprod.signicat.com/doc/shared/order/{0}/{1}/{2}/original", requestId, taskid, docid );
                resultUri = string.Format( "https://preprod.signicat.com/doc/shared/order/{0}/{1}/{2}/sdo", requestId, taskid, docid );
            }

            using (var client = new PackagingEndPointClient())
            {
                var request = new createpackagerequest
                {
                    service = service,
                    password = passwd,
                    version = "1",
                    packagingmethod = "pades",
                    validationpolicy =  "ltvsdo-validator",
                    sdo = new documentid[]
                    {
                        new documentid
                        {
                            uridocumentid = resultUri
                        }
                    }
                };
 
                var createPackageResponse = client.createpackage(request);
                string padesDocumentId = createPackageResponse.id;
                string padesDownloadUrl = "https://preprod.signicat.com/doc/shared/sds/" + padesDocumentId;
                return padesDownloadUrl;
            }
        }
    }
}