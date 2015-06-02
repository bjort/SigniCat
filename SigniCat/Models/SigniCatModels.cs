using SigniCat.ServiceReference1;
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
        public string upload_document_to_SDS( string path )
        {
            byte[] document = File.ReadAllBytes( path );

            using (var webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential("shared", "Bond007");
                webClient.Headers.Add( "Content-Type", "application/pdf" );
                //byte[] document = webClient.DownloadData(path);
                byte[] response = webClient.UploadData( "https://preprod.signicat.com/doc/shared/sds", "POST", document );
                return System.Text.Encoding.Default.GetString( response );
            }
                //Assert.AreEqual( HttpStatusCode.Created, response.StatusCode );
                //Assert.IsTrue( documentId.Length > 0 );
        }

        public string get_sign_url_from_SDS( string did )
        {
            createrequestrequest request = new createrequestrequest
            {
                password = "Bond007",
                service = "shared",
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
                                 id = "doc_1", /* must match the documentaction in "task_1" further down */
                                 refsdsid = did,
                                 description = "Terms and conditions"

                             }
                         },
                         subject = new subject[]
                         {
                             new subject
                             {
                                 id = "subj_1", /* must match the subjectref in "task_1" further down */
                                 nationalid = "1909740939" /* en kode vi bestemmer - f.eks. personnr */
                             }
                         },
                         task = new task[]
                         {
                             new task
                             {
                                bundle = false,
                                bundleSpecified = true,
                                id = "task_1",
                                 subjectref = "subj_1",
                                 documentaction = new documentaction[]
                                 {
                                      new documentaction
                                      {
                                          type = documentactiontype.sign,
                                          documentref = "doc_1"
                                      }
                                 },

                                 signature = new signature[]
                                 {
                                     new signature
                                     {
                                         method = new string[]
                                         {
                                             "nbid-sign"
                                         }
                                     }
                                 }
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
            string signHereUrl = 
                string.Format( "https://preprod.signicat.com/std/docaction/shared?request_id={0}&task_id={1}", response.requestid[ 0 ], request.request[ 0 ].task[ 0 ].id );

            return signHereUrl;

            //Assert.AreEqual( HttpStatusCode.Created, response.StatusCode );
            //Assert.IsTrue( documentId.Length > 0 );
        }

        public void download_document_from_SDS( string did, string path )
        {
            using( var webClient = new WebClient() )
            {
                webClient.Credentials = new NetworkCredential( "shared", "Bond007" );
                byte[] document = webClient.DownloadData( "https://preprod.signicat.com/doc/shared/sds/" + did );
                File.WriteAllBytes( path, document );
            }
            //Assert.AreEqual( HttpStatusCode.Created, response.StatusCode );
            //Assert.IsTrue( documentId.Length > 0 );
        }
    }
}